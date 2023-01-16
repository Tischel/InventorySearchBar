using CriticalCommonLib;
using CriticalCommonLib.Crafting;
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

        private static GameInterface GameInterface { get; set; } = null!;
        private static InventoryScanner InventoryScanner { get; set; } = null!;
        public static InventoryMonitor InventoryMonitor { get; private set; } = null!;
        public static CharacterMonitor CharacterMonitor { get; private set; } = null!;
        public static CraftMonitor CraftMonitor { get; private set; } = null!;
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
            Service.ExcelCache = new ExcelCache(Service.Data);
            GameInterface = new GameInterface();

            CharacterMonitor = new CharacterMonitor();
            GameUi = new GameUiManager();
            CraftMonitor = new CraftMonitor(GameUi);
            InventoryScanner = new InventoryScanner(CharacterMonitor, GameUi, GameInterface);
            InventoryMonitor = new InventoryMonitor(CharacterMonitor, CraftMonitor, InventoryScanner);
            InventoryScanner.Enable();

            if (pluginInterface.AssemblyLocation.DirectoryName != null)
            {
                AssemblyLocation = pluginInterface.AssemblyLocation.DirectoryName + "\\";
            }
            else
            {
                AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            }

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.1.0.1";

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
            if (Settings == null || ClientState.LocalPlayer == null || _manager == null) return;

            KeyboardHelper.Instance?.Update();
            _manager.Update();

            if (_manager.ActiveInventory != null)
            {
                IsKeybindActive = Settings.Keybind.IsActive();
            }
        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null || _manager == null) return;

            if (_manager.ActiveInventory == null)
            {
                _searchBarWindow.Inventory = null;
                _searchBarWindow.IsOpen = false;
            }
            else
            {
                _searchBarWindow.Inventory = _manager.ActiveInventory;
                _searchBarWindow.UpdateCanShow();
                _searchBarWindow.IsOpen = _searchBarWindow.CanShow;

                _manager.ActiveInventory.ApplyFilters(Filters, _searchBarWindow.SearchTerm);
                _manager.ActiveInventory.UpdateHighlights();
            }

            _windowSystem?.Draw();
        }

        public static unsafe void ClearHighlights()
        {
            _manager?.ClearHighlights();
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
            CraftMonitor.Dispose();
            InventoryScanner.Dispose();

            GameInterface.Dispose();
            Service.ExcelCache.Destroy();

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
