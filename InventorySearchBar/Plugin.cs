using CriticalCommonLib;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Ui;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InventorySearchBar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InventorySearchBar
{
    public class Plugin : IDalamudPlugin
    {
        public static ClientState ClientState { get; private set; } = null!;
        public static CommandManager CommandManager { get; private set; } = null!;
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        public static Framework Framework { get; private set; } = null!;
        public static GameGui GameGui { get; private set; } = null!;
        public static UiBuilder UiBuilder { get; private set; } = null!;
        public static DataManager DataManager { get; private set; } = null!;
        public static KeyState KeyState { get; private set; } = null!;

        public static string AssemblyLocation { get; private set; } = "";
        public string Name => "InventorySearchBar";

        public static string Version { get; private set; } = "";

        public static Settings Settings { get; private set; } = null!;

        private static WindowSystem _windowSystem = null!;
        private static SettingsWindow _settingsWindow = null!;
        private static SearchBarWindow _searchBarWindow = null!;

        private static OdrScanner OdrScanner { get; set; } = null!;
        public static InventoryMonitor InventoryMonitor { get; private set; } = null!;
        public static CharacterMonitor CharacterMonitor { get; private set; } = null!;
        public static GameUiManager GameUi { get; private set; } = null!;

        private List<List<bool>> _itemsMap = null!;
        private Tuple<IntPtr, GameInventoryType> _activeInventory = null!;
        public static bool IsKeybindActive = false;

        internal enum GameInventoryType
        {
            Normal = 0,
            Large = 1,
            Largest = 2,
            None = 3
        }

        public Plugin(
            ClientState clientState,
            CommandManager commandManager,
            DalamudPluginInterface pluginInterface,
            Framework framwork,
            DataManager dataManager,
            GameGui gameGui,
            KeyState keyState
        )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            PluginInterface = pluginInterface;
            Framework = framwork;
            DataManager = dataManager;
            GameGui = gameGui;
            UiBuilder = pluginInterface.UiBuilder;
            KeyState = keyState;

            KeyboardHelper.Initialize();

            pluginInterface.Create<Service>();
            ExcelCache.Initialise();
            GameInterface.Initialise(Service.Scanner);

            CharacterMonitor = new CharacterMonitor();
            OdrScanner = new OdrScanner(CharacterMonitor);
            GameUi = new GameUiManager();
            InventoryMonitor = new InventoryMonitor(OdrScanner, CharacterMonitor, GameUi);

            if (pluginInterface.AssemblyLocation.DirectoryName != null)
            {
                AssemblyLocation = pluginInterface.AssemblyLocation.DirectoryName + "\\";
            }
            else
            {
                AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            }

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0.0";

            Framework.Update += Update;
            UiBuilder.Draw += Draw;
            UiBuilder.OpenConfigUi += OpenConfigUi;

            CommandManager.AddHandler(
                "/inventorysearchbar",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Opens the Inventory Search Bar configuration window.",

                    ShowInHelp = true
                }
            );

            CommandManager.AddHandler(
                "/isb",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Opens the Inventory Search Bar configuration window.",

                    ShowInHelp = true
                }
            );

            Settings = Settings.Load();

            CreateWindows();

            // dummy map
            _itemsMap = new List<List<bool>>();
            for (int i = 0; i < 4; i++)
            {
                List<bool> list = new List<bool>(35);
                for (int j = 0; j < 35; j++)
                {
                    list.Add(false);
                }

                _itemsMap.Add(list);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void PluginCommand(string command, string arguments)
        {
            _settingsWindow.IsOpen = !_settingsWindow.IsOpen;
        }

        private void CreateWindows()
        {
            _settingsWindow = new SettingsWindow("Inventory Search Bar v" + Version);
            _searchBarWindow = new SearchBarWindow("InventorySearchBar");

            _windowSystem = new WindowSystem("InventorySearchBar_Windows");
            _windowSystem.AddWindow(_settingsWindow);
            _windowSystem.AddWindow(_searchBarWindow);
        }

        private void Update(Framework framework)
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            KeyboardHelper.Instance?.Update();

            _activeInventory = FindInventory();
            if (_activeInventory.Item2 != GameInventoryType.None)
            {
                IsKeybindActive = Settings.Keybind.IsActive();
            }
        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null || _activeInventory == null) return;

            _windowSystem?.Draw();

            if (_activeInventory.Item1 == IntPtr.Zero || _activeInventory.Item2 == GameInventoryType.None)
            {
                _searchBarWindow.InventoryAddon = IntPtr.Zero;
                _searchBarWindow.IsOpen = false;
                return;
            }

            _searchBarWindow.InventoryAddon = _activeInventory.Item1;
            _searchBarWindow.IsOpen = true;

            SearchItems(_activeInventory.Item1, _activeInventory.Item2, _searchBarWindow.SearchTerm);
        }

        private unsafe Tuple<IntPtr, GameInventoryType> FindInventory()
        {
            // normal
            IntPtr ptr = NormalInventoryHelper.GetNode();
            AtkUnitBase* addon = (AtkUnitBase*)ptr;
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.Normal);
            }

            // expanded
            ptr = LargeInventoryHelper.GetNode();
            addon = (AtkUnitBase*)ptr;
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.Large);
            }

            // all
            ptr = LargestInventoryHelper.GetNode();
            addon = (AtkUnitBase*)ptr;
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.Largest);
            }

            // none
            return new Tuple<IntPtr, GameInventoryType>(IntPtr.Zero, GameInventoryType.None);
        }

        private unsafe void SearchItems(IntPtr addon, GameInventoryType type, string searchTerm)
        {
            List<List<bool>>? results = null;
            if (searchTerm.Length > 1)
            {
                results = FindItems(searchTerm);
            }

            switch (type)
            {
                case GameInventoryType.Normal: NormalInventoryHelper.HighlightItems(addon, results); break;
                case GameInventoryType.Large: LargeInventoryHelper.HighlightItems(addon, results); break;
                case GameInventoryType.Largest: LargestInventoryHelper.HighlightItems(results); break;
            }
        }

        public unsafe List<List<bool>> FindItems(string searchTerm)
        {
            List<List<bool>> results = new List<List<bool>>(_itemsMap);

            string text = searchTerm.ToUpper();
            List<InventoryItem> items = InventoryMonitor.GetSpecificInventory(CharacterMonitor.ActiveCharacter, InventoryCategory.CharacterBags);

            foreach (InventoryItem item in items)
            {
                bool highlight = false;
                if (item.Item != null)
                {
                    highlight = item.Item.Name.ToString().ToUpper().Contains(text);
                }

                results[(int)item.SortedContainer][34 - item.SortedSlotIndex] = highlight;
            }

            return results;
        }

        private static unsafe void ClearNodeHighlights()
        {
            IntPtr normalInventory = NormalInventoryHelper.GetNode();
            if (normalInventory != IntPtr.Zero)
            {
                NormalInventoryHelper.HighlightItems(normalInventory, null);
                NormalInventoryHelper.HighlightTabs(normalInventory, null, true);
            }

            IntPtr largeInventory = LargeInventoryHelper.GetNode();
            if (largeInventory != IntPtr.Zero)
            {
                LargeInventoryHelper.HighlightItems(largeInventory, null);
                LargeInventoryHelper.HighlightTabs(largeInventory, null, true);
            }

            IntPtr largestInventory = LargestInventoryHelper.GetNode();
            if (largestInventory != IntPtr.Zero)
            {
                LargestInventoryHelper.HighlightItems(null);
            }
        }

        public static unsafe void ClearTabHighlights()
        {
            IntPtr normalInventory = NormalInventoryHelper.GetNode();
            if (normalInventory != IntPtr.Zero)
            {
                NormalInventoryHelper.HighlightTabs(normalInventory, null, true);
            }

            IntPtr largeInventory = LargeInventoryHelper.GetNode();
            if (largeInventory != IntPtr.Zero)
            {
                LargeInventoryHelper.HighlightTabs(largeInventory, null, true);
            }
        }

        private void OpenConfigUi()
        {
            _settingsWindow.IsOpen = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ClearNodeHighlights();

            KeyboardHelper.Instance?.Dispose();

            Settings.Save(Settings);

            InventoryMonitor.Dispose();
            GameUi.Dispose();
            CharacterMonitor.Dispose();
            OdrScanner.Dispose();

            ExcelCache.Destroy();
            GameInterface.Dispose();

            _windowSystem.RemoveAllWindows();

            CommandManager.RemoveHandler("/inventorysearchbar");
            CommandManager.RemoveHandler("/isb");

            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
