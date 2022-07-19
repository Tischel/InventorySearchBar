using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using SigScanner = Dalamud.Game.SigScanner;

namespace InventorySearchBar
{
    public class Plugin : IDalamudPlugin
    {
        public static ClientState ClientState { get; private set; } = null!;
        public static CommandManager CommandManager { get; private set; } = null!;
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        public static DataManager DataManager { get; private set; } = null!;
        public static Framework Framework { get; private set; } = null!;
        public static GameGui GameGui { get; private set; } = null!;
        public static SigScanner SigScanner { get; private set; } = null!;
        public static UiBuilder UiBuilder { get; private set; } = null!;
        public static KeyState KeyState { get; private set; } = null!;

        public static string AssemblyLocation { get; private set; } = "";
        public string Name => "InventorySearchBar";

        public static string Version { get; private set; } = "";

        public static Settings Settings { get; private set; } = null!;

        private static WindowSystem _windowSystem = null!;
        //private static SettingsWindow _settingsWindow = null!;

        private bool _isInventoryOpened = false;
        private string _searchTerm = "";
        private bool _needsFocus = false;

        internal enum GameInventoryType
        {
            Normal = 0,
            Expanded = 1,
            All = 2,
            None = 3
        }

        public Plugin(
            ClientState clientState,
            CommandManager commandManager,
            DalamudPluginInterface pluginInterface,
            DataManager dataManager,
            Framework framework,
            GameGui gameGui,
            SigScanner sigScanner,
            KeyState keyState
        )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            PluginInterface = pluginInterface;
            DataManager = dataManager;
            Framework = framework;
            GameGui = gameGui;
            SigScanner = sigScanner;
            UiBuilder = PluginInterface.UiBuilder;
            KeyState = keyState;

            if (pluginInterface.AssemblyLocation.DirectoryName != null)
            {
                AssemblyLocation = pluginInterface.AssemblyLocation.DirectoryName + "\\";
            }
            else
            {
                AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            }

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0.0";

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

            InventoryHelper.Initialize();
            Settings = Settings.Load();

            CreateWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void PluginCommand(string command, string arguments)
        {
            //_settingsWindow.IsOpen = !_settingsWindow.IsOpen;
        }

        private void CreateWindows()
        {
            //_settingsWindow = new SettingsWindow("Inventory Search Bar Settings");

            _windowSystem = new WindowSystem("InventorySearchBar_Windows");
            //_windowSystem.AddWindow(_settingsWindow);
        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            _windowSystem?.Draw();

            Tuple<IntPtr, GameInventoryType> result = FindInventory();
            if (result.Item1 == IntPtr.Zero || result.Item2 == GameInventoryType.None) { return; }

            if (!_isInventoryOpened)
            {
                if (Settings.AutoFocus)
                {
                    _needsFocus = true;
                }

                if (Settings.AutoClear)
                {
                    _searchTerm = "";
                }
            }
            _isInventoryOpened = true;

            DrawSearchBar(result.Item1, result.Item2);
        }

        private unsafe Tuple<IntPtr, GameInventoryType> FindInventory()
        {
            // normal
            AtkUnitBase* addon = (AtkUnitBase*)GameGui.GetAddonByName("Inventory", 1);
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.Normal);
            }

            // expanded
            addon = (AtkUnitBase*)GameGui.GetAddonByName("InventoryLarge", 1);
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.Expanded);
            }

            // all
            addon = (AtkUnitBase*)GameGui.GetAddonByName("InventoryExpansion", 1);
            if (addon != null && addon->IsVisible)
            {
                return new Tuple<IntPtr, GameInventoryType>((IntPtr)addon, GameInventoryType.All);
            }

            // none
            return new Tuple<IntPtr, GameInventoryType>(IntPtr.Zero, GameInventoryType.None);
        }

        private unsafe void DrawSearchBar(IntPtr addon, GameInventoryType type)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

            float width = inventory->WindowCollisionNode->AtkResNode.Width * inventory->Scale;
            float x = inventory->X + width / 2f - Settings.SearchBarWidth / 2f;
            float y = inventory->Y + 13 * inventory->Scale;

            ImGui.SetNextWindowPos(new Vector2(x, y));
            ImGui.SetNextWindowSize(new Vector2(Settings.SearchBarWidth, 24));

            var begin = ImGui.Begin(
                "InventorySearchBar",
                ImGuiWindowFlags.NoTitleBar
              | ImGuiWindowFlags.NoScrollbar
              | ImGuiWindowFlags.AlwaysAutoResize
              | ImGuiWindowFlags.NoBackground
              | ImGuiWindowFlags.NoSavedSettings
            );

            ImGui.PopStyleVar(3);

            if (!begin)
            {
                ImGui.End();
                return;
            }

            ImGui.PushItemWidth(Settings.SearchBarWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarBackgroundColor));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarBackgroundColor));
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarTextColor));

            ImGui.InputText("", ref _searchTerm, 100);

            if (_needsFocus)
            {
                ImGui.SetKeyboardFocusHere(0);
                _needsFocus = false;
            }

            List<List<bool>>? results = null;

            if (_searchTerm.Length > 1)
            {
                results = InventoryHelper.Instance.FindItems(_searchTerm);
            }

            switch (type)
            {
                case GameInventoryType.Normal: HighlightNormalInventory(addon, results); break;
                case GameInventoryType.Expanded: HighlightExpandedInventory(addon, results); break;
                case GameInventoryType.All: HighlightAllInventory(addon, results); break;
            }

            ImGui.PopStyleColor(3);
            ImGui.End();
        }

        private unsafe void HighlightNormalInventory(IntPtr addon, List<List<bool>>? results)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
        }

        private unsafe void HighlightExpandedInventory(IntPtr addon, List<List<bool>>? results)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
        }

        private unsafe void HighlightAllInventory(IntPtr addon, List<List<bool>>? results)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;

            for (int i = 0; i < 4; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)GameGui.GetAddonByName("InventoryGrid" + i + "E", 1);
                if (grid == null) { continue; }

                for (int j = 3; j < grid->UldManager.NodeListCount; j++)
                {
                    bool highlight = true;
                    if (results != null && results[i].Count > j - 3)
                    {
                        highlight = results[i][j - 3];
                    }

                    grid->UldManager.NodeList[j]->MultiplyRed = highlight ? (byte)100 : (byte)20;
                    grid->UldManager.NodeList[j]->MultiplyGreen = highlight ? (byte)100 : (byte)20;
                    grid->UldManager.NodeList[j]->MultiplyBlue = highlight ? (byte)100 : (byte)20;
                }
            }
        }

        private void OpenConfigUi()
        {
            //_settingsWindow.IsOpen = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Settings.Save(Settings);
            InventoryHelper.Instance?.Dispose();

            _windowSystem.RemoveAllWindows();

            CommandManager.RemoveHandler("/inventorysearchbar");
            CommandManager.RemoveHandler("/isb");

            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
