using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using InventorySearchBar.Inventories;
using System;
using System.Numerics;

namespace InventorySearchBar.Windows
{
    public class SearchBarWindow : Window
    {
        public string SearchTerm = "";
        public Inventory? Inventory = null;
        private bool _needsFocus = false;

        private bool _canShow = true;
        public bool CanShow => _canShow;

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

            Size = new Vector2(Settings.SearchBarWidth * 0.86f, 22);
            SizeCondition = ImGuiCond.Always;

            RespectCloseHotkey = false;
        }

        public void UpdateCanShow()
        {
            if (Plugin.Settings.KeybindOnly)
            {
                _canShow = _canShow || Plugin.IsKeybindActive;
            }
            else
            {
                _canShow = true;
            }
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
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(1, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);


            if (Inventory != null && Inventory.Addon != IntPtr.Zero)
            {
                AtkUnitBase* inventory = (AtkUnitBase*)Inventory.Addon;
                AtkCollisionNode* window = inventory->WindowCollisionNode;
                if (window == null) { return; }

                float width = window->AtkResNode.Width * inventory->Scale;
                float x = inventory->X + width / 2f - Settings.SearchBarWidth / 2f + Inventory.OffsetX;
                float y = inventory->Y + 13 * inventory->Scale;

                Position = new Vector2(x, y);
                Size = new Vector2(Settings.SearchBarWidth * 0.86f, 22);
            }
        }

        public override void PostDraw()
        {
            ImGui.PopStyleVar(5);
        }

        public override unsafe void Draw()
        {
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
