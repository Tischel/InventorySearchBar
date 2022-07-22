using CriticalCommonLib;
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
using InventorySearchBar.Filters;
using InventorySearchBar.Helpers;
using InventorySearchBar.Inventories;
using InventorySearchBar.Windows;
using System;
using System.Collections.Generic;
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
        private static FiltersWindow _filtersWindow = null!;
        private static SearchBarWindow _searchBarWindow = null!;

        private static OdrScanner OdrScanner { get; set; } = null!;
        public static InventoryMonitor InventoryMonitor { get; private set; } = null!;
        public static CharacterMonitor CharacterMonitor { get; private set; } = null!;
        public static GameUiManager GameUi { get; private set; } = null!;

        private static InventoriesManager _manager = null!;
        public static bool IsKeybindActive = false;

        public static List<Filter> Filters = new List<Filter>()
        {
            new NameFilter(),
            new JobFilter(),
            new Filters.TypeFilter(),
            new LevelFilter()
        };

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

            _manager = new InventoriesManager();
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
            _filtersWindow = new FiltersWindow("Inventory Search Bar Filters");
            _searchBarWindow = new SearchBarWindow("InventorySearchBar");

            _windowSystem = new WindowSystem("InventorySearchBar_Windows");
            _windowSystem.AddWindow(_settingsWindow);
            _windowSystem.AddWindow(_filtersWindow);
            _windowSystem.AddWindow(_searchBarWindow);
        }

        public static void OpenFiltersWindow()
        {
            _filtersWindow.IsOpen = true;
        }

        private unsafe void Update(Framework framework)
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            KeyboardHelper.Instance?.Update();
            _manager.Update();

            if (_manager.ActiveInventory != null)
            {
                IsKeybindActive = Settings.Keybind.IsActive();
            }
        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            if (_manager.ActiveInventory == null)
            {
                _searchBarWindow.InventoryAddon = IntPtr.Zero;
                _searchBarWindow.IsOpen = false;
            }
            else
            {
                _searchBarWindow.InventoryAddon = _manager.ActiveInventory.Addon;
                _searchBarWindow.IsOpen = true;

                _manager.ActiveInventory.ApplyFilters(Filters, _searchBarWindow.SearchTerm);
                _manager.ActiveInventory.UpdateHighlights();
            }

            _windowSystem?.Draw();
        }

        public static unsafe void ClearHighlights()
        {
            _manager.ClearHighlights();
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

            ClearHighlights();

            KeyboardHelper.Instance?.Dispose();

            Settings.Save(Settings);

            InventoryMonitor.Dispose();
            GameUi.Dispose();
            CharacterMonitor.Dispose();
            OdrScanner.Dispose();

            ExcelCache.Destroy();
            GameInterface.Dispose();

            _windowSystem.RemoveAllWindows();
            _manager.Dispose();

            CommandManager.RemoveHandler("/inventorysearchbar");
            CommandManager.RemoveHandler("/isb");

            Framework.Update -= Update;
            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
