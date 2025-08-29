using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Extensions;
using Autofac;
using Autofac.Core;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using InventorySearchBar.Filters;
using InventorySearchBar.Helpers;
using InventorySearchBar.Inventories;
using InventorySearchBar.Windows;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace InventorySearchBar
{
    public class Plugin : IDalamudPlugin
    {
        public static IClientState ClientState { get; private set; } = null!;
        public static ICommandManager CommandManager { get; private set; } = null!;
        public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        public static IFramework Framework { get; private set; } = null!;
        public static IGameGui GameGui { get; private set; } = null!;
        public static IUiBuilder UiBuilder { get; private set; } = null!;
        public static IDataManager DataManager { get; private set; } = null!;
        public static IKeyState KeyState { get; private set; } = null!;
        public static IPluginLog Logger { get; private set; } = null!;
        public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
        public static ICondition Condition { get; private set; } = null!;
        public static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

        public static string AssemblyLocation { get; private set; } = "";
        public string Name => "InventorySearchBar";

        public static string Version { get; private set; } = "";

        public static Settings Settings { get; private set; } = null!;

        private static WindowSystem _windowSystem = null!;
        private static SettingsWindow _settingsWindow = null!;
        private static FiltersWindow _filtersWindow = null!;
        private static SearchBarWindow _searchBarWindow = null!;

        private static GameInterface GameInterface { get; set; } = null!;
        public static OdrScanner OdrScanner { get; private set; } = null!;
        private static InventoryScanner InventoryScanner { get; set; } = null!;
        public static InventoryMonitor InventoryMonitor { get; private set; } = null!;
        public static CharacterMonitor CharacterMonitor { get; private set; } = null!;
        public static CraftMonitor CraftMonitor { get; private set; } = null!;
        public static GameUiManager GameUi { get; private set; } = null!;
        public static SheetManager SheetManager { get; private set; } = null!;

        private static InventoriesManager _manager = null!;
        public static bool IsKeybindActive = false;

        private bool _libLoaded = false;

        private IContainer Container { get; set; } = null!;
        private ILifetimeScope ContainerLifetime { get; set; } = null!;
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public static List<Filter> Filters = new List<Filter>()
        {
            new NameFilter(),
            new JobFilter(),
            new Filters.TypeFilter(),
            new LevelFilter()
        };

        public Plugin(
            IClientState clientState,
            ICommandManager commandManager,
            IDalamudPluginInterface pluginInterface,
            IFramework framework,
            IDataManager dataManager,
            IGameGui gameGui,
            IKeyState keyState,
            IPluginLog logger,
            IGameInteropProvider gameInteropProvider,
            ICondition condition,
            IAddonLifecycle addonLifecycle
        )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            PluginInterface = pluginInterface;
            Framework = framework;
            DataManager = dataManager;
            GameGui = gameGui;
            UiBuilder = pluginInterface.UiBuilder;
            KeyState = keyState;
            Logger = logger;
            GameInteropProvider = gameInteropProvider;
            Condition = condition;
            AddonLifecycle = addonLifecycle;

            KeyboardHelper.Initialize();

            // allagan tools setup
            SheetManager = new SheetManager(DataManager.GameData, new SheetManagerStartupOptions()
            {
                BuildNpcLevels = false,
                BuildNpcShops = false,
                BuildItemInfoCache = false
            });

            if (pluginInterface.AssemblyLocation.DirectoryName != null)
            {
                AssemblyLocation = pluginInterface.AssemblyLocation.DirectoryName + "\\";
            }
            else
            {
                AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            }

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.8.0.0";

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

            //LoadLib();
        }

        private void LoadLib()
        {
            //Build a mini container to handle resolution
            var builder = new ContainerBuilder();
            builder.RegisterGameSheetManager(new SheetManagerStartupOptions()
            {
                BuildNpcLevels = false,
                BuildNpcShops = false,
                BuildItemInfoCache = false,
            });

            builder.RegisterInstance(Condition).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(GameInteropProvider).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(Framework).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(Logger).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(ClientState).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(GameGui).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(DataManager.GameData).AsImplementedInterfaces().AsSelf().ExternallyOwned();
            builder.RegisterInstance(AddonLifecycle).AsImplementedInterfaces().AsSelf().ExternallyOwned();

            builder.RegisterType<GameInterface>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CharacterMonitor>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<GameUiManager>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CraftMonitor>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<ClassJobService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<OdrScanner>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<InventoryScanner>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<MarketOrderService>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<InventoryMonitor>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterType<CriticalCommonLib.Models.Inventory>().AsSelf();
            builder.RegisterType<Character>().AsSelf();
            builder.RegisterType<InventoryItem>().AsSelf();
            builder.RegisterType<InventoryChange>().AsSelf();

            Container = builder.Build();

            ContainerLifetime = Container.BeginLifetimeScope();
            InventoryMonitor = ContainerLifetime.Resolve<InventoryMonitor>();
            GameInterface = ContainerLifetime.Resolve<GameInterface>();
            CharacterMonitor = ContainerLifetime.Resolve<CharacterMonitor>();
            GameUi = ContainerLifetime.Resolve<GameUiManager>();
            CraftMonitor = ContainerLifetime.Resolve<CraftMonitor>();
            OdrScanner = ContainerLifetime.Resolve<OdrScanner>();
            InventoryScanner = ContainerLifetime.Resolve<InventoryScanner>();

            OdrScanner.StartAsync(_cancelTokenSource.Token).Wait();
            InventoryMonitor.Start();
            InventoryScanner.Enable();
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

        private unsafe void Update(IFramework framework)
        {
            if (Settings == null || ClientState.LocalPlayer == null || _manager == null) return;

            if (!_libLoaded)
            {
                LoadLib();
                _libLoaded = true;
            }

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

            _cancelTokenSource.Cancel();
            ContainerLifetime.Dispose();
            Container.Dispose();

            KeyboardHelper.Instance?.Dispose();

            Settings.Save(Settings);

            InventoryMonitor.Dispose();
            GameUi.Dispose();
            CharacterMonitor.Dispose();
            CraftMonitor.Dispose();
            InventoryScanner.Dispose();
            OdrScanner.Dispose();

            OdrScanner.StopAsync(CancellationToken.None).Wait();
            GameInterface.Dispose();
            SheetManager.Dispose();

            _windowSystem.RemoveAllWindows();
            _manager.Dispose();

            CommandManager.RemoveHandler("/inventorysearchbar");
            CommandManager.RemoveHandler("/isb");

            Framework.Update -= Update;
            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }

    internal sealed class DalamudLogger<T> : ILogger<T>
    {
        private readonly string _name;
        private readonly IPluginLog _pluginLog;

        public DalamudLogger(string name, IPluginLog pluginLog)
        {
            _name = name;
            _pluginLog = pluginLog;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            //return (int)_configuration.LogLevel <= (int)logLevel;
            return true;
        }

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return BeginScope(state);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;


            StringBuilder sb = new();
            sb.Append($"[{_name}]{{{(int)logLevel}}} {state}: {exception?.Message}");
            if (exception != null)
            {
                sb.AppendLine(exception.StackTrace);
                var innerException = exception?.InnerException;
                while (innerException != null)
                {
                    sb.AppendLine($"InnerException {innerException}: {innerException.Message}");
                    sb.AppendLine(innerException.StackTrace);
                    innerException = innerException.InnerException;
                }
            }

            if (logLevel == LogLevel.Trace)
                _pluginLog.Verbose(sb.ToString());
            else if (logLevel == LogLevel.Debug)
                _pluginLog.Debug(sb.ToString());
            else if (logLevel == LogLevel.Information)
                _pluginLog.Information(sb.ToString());
            else if (logLevel == LogLevel.Warning)
                _pluginLog.Warning(sb.ToString());
            else if (logLevel == LogLevel.Error)
                _pluginLog.Error(sb.ToString());
            else if (logLevel == LogLevel.Critical)
                _pluginLog.Fatal(sb.ToString());

        }
    }
}
