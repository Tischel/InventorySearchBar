using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Numerics;

namespace InventorySearchBar.Windows
{
    public class SearchBarWindow : Window
    {
        public string SearchTerm = "";
        public IntPtr InventoryAddon = IntPtr.Zero;
        private bool _needsFocus = false;
        private bool _canShow = false;

        private Settings Settings => Plugin.Settings;

        public SearchBarWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoTitleBar
                | ImGuiWindowFlags.NoSavedSettings;

            Size = new Vector2(Settings.SearchBarWidth, 24);
        }

        public override void OnOpen()
        {
            if (Settings.AutoFocus)
            {
                _needsFocus = true;
            }

            _canShow = !Plugin.Settings.KeybindOnly;
        }

        public override void OnClose()
        {
            if (Plugin.Settings.AutoClear)
            {
                SearchTerm = "";
            }
        }

        public override unsafe void PreDraw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

            if (InventoryAddon != IntPtr.Zero)
            {
                AtkUnitBase* inventory = (AtkUnitBase*)InventoryAddon;
                AtkCollisionNode* window = inventory->WindowCollisionNode;
                if (window == null) { return; }

                float width = window->AtkResNode.Width * inventory->Scale;
                float x = inventory->X + width / 2f - Settings.SearchBarWidth / 2f;
                float y = inventory->Y + 13 * inventory->Scale;

                Position = new Vector2(x, y);
                Size = new Vector2(Settings.SearchBarWidth, 24);
            }
        }

        public override void PostDraw()
        {
            ImGui.PopStyleVar(3);
        }

        public override unsafe void Draw()
        {
            if (Plugin.Settings.KeybindOnly && Plugin.IsKeybindActive)
            {
                _canShow = true;
            }

            if (!_canShow) return;

            ImGui.PushItemWidth(Settings.SearchBarWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarBackgroundColor));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarBackgroundColor));
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(Settings.SearchBarTextColor));

            if (Plugin.IsKeybindActive || _needsFocus)
            {
                ImGui.SetKeyboardFocusHere(0);
                _needsFocus = false;
            }

            ImGui.InputText("", ref SearchTerm, 100);

            if (Plugin.Settings.KeybindOnly && !ImGui.IsItemActive())
            {
                _canShow = false;
            }

            ImGui.PopStyleColor(3);
        }
    }
}
