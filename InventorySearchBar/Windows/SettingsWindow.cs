using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;
using TPie.Helpers;

namespace InventorySearchBar.Windows
{
    public class SettingsWindow : Window
    {
        private float _scale => ImGuiHelpers.GlobalScale;
        private int _width = 236;

        public SettingsWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse;

            Size = new Vector2(_width, 196);
        }

        public override void Draw()
        {
            if (!ImGui.BeginTabBar("##InventorySearchBar_Settings_TabBar"))
            {
                return;
            }

            if (ImGui.BeginTabItem("General##ISB_Settings"))
            {
                Size = new Vector2(_width, 196);
                ImGui.Spacing();
                DrawGeneralTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Keybind##ISB_Settings"))
            {
                Size = new Vector2(_width, 148);
                ImGui.Spacing();
                DrawKeybindTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Style##ISB_Settings"))
            {
                Size = new Vector2(_width, 148);
                ImGui.Spacing();
                DrawStyleTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Offsets##ISB_Settings"))
            {
                Size = new Vector2(_width, 254);
                ImGui.Spacing();
                DrawOffsetsTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        private void DrawGeneralTab()
        {
            ImGui.Checkbox("Automatically clear search", ref Plugin.Settings.AutoClear);
            DrawHelper.SetTooltip("If enabled, the search bar will be emptied every time the inventory is opened.");

            ImGui.Checkbox("Automatically focus search bar", ref Plugin.Settings.AutoFocus);
            DrawHelper.SetTooltip("If enabled, the search bar will focused every time the inventory is opened.");

            if (ImGui.Checkbox("Highlight tabs", ref Plugin.Settings.HightlightTabs))
            {
                if (!Plugin.Settings.HightlightTabs)
                {
                    Plugin.ClearHighlights();
                }
            }
            DrawHelper.SetTooltip("If enabled, the inventory tabs will be highlighted if they contain an item that satisfies the search.");

            ImGui.NewLine();
            if (ImGui.Button("Edit Filters", new Vector2(220, 24)))
            {
                Plugin.OpenFiltersWindow();
            }
        }

        private void DrawKeybindTab()
        {
            Plugin.Settings.Keybind.Draw(100);

            ImGui.Checkbox("Keybind only", ref Plugin.Settings.KeybindOnly);
            DrawHelper.SetTooltip("When enabled, the search bar will only appear when the keybind is used.");

            ImGui.Checkbox("Keybind Passthrough", ref Plugin.Settings.KeybindPassthrough);
            DrawHelper.SetTooltip("When enabled, the plugin wont prevent the game from receiving the key presses for the keybind.");

        }

        private void DrawStyleTab()
        {
            ImGui.PushItemWidth(80);
            ImGui.DragInt("Search Bar Width", ref Plugin.Settings.SearchBarWidth, 1, 50, 500);
            ImGui.ColorEdit4("Search Bar Background Color", ref Plugin.Settings.SearchBarBackgroundColor, ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Search Bar Text Color", ref Plugin.Settings.SearchBarTextColor, ImGuiColorEditFlags.NoInputs);
        }

        private void DrawOffsetsTab()
        {
            ImGui.PushItemWidth(50);

            ImGui.DragInt("Normal Inventory", ref Plugin.Settings.NormalInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Large Inventory", ref Plugin.Settings.LargeInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Largest Inventory", ref Plugin.Settings.LargestInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Chocobo Saddle", ref Plugin.Settings.ChocoboInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Retainer Inventory", ref Plugin.Settings.RetainerInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Large Retainer Inventory", ref Plugin.Settings.LargeRetainerInventoryOffset, 0.5f, -500, 500);
            ImGui.DragInt("Armoury", ref Plugin.Settings.ArmouryInventoryOffset, 0.5f, -500, 500);
        }
    }
}
