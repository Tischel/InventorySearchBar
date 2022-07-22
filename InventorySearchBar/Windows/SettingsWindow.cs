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

        public SettingsWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse;

            Size = new Vector2(236, 432);
        }

        public override void Draw()
        {
            ImGui.Text("General");
            ImGui.BeginChild("##General", new Vector2(220 * _scale, 94 * _scale), true);
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
            }
            ImGui.EndChild();

            ImGui.Spacing();

            ImGui.Text("Keybind");
            ImGui.BeginChild("##Keybind", new Vector2(220 * _scale, 94 * _scale), true);
            {
                Plugin.Settings.Keybind.Draw("ISB", 200);

                ImGui.Checkbox("Keybind only", ref Plugin.Settings.KeybindOnly);
                DrawHelper.SetTooltip("When enabled, the search bar will only appear when the keybind is used.");

                ImGui.Checkbox("Keybind Passthrough", ref Plugin.Settings.KeybindPassthrough);
                DrawHelper.SetTooltip("When enabled, the plugin wont prevent the game from receiving the key presses for the keybind.");
            }
            ImGui.EndChild();

            ImGui.Spacing();

            ImGui.Text("Style");
            ImGui.BeginChild("##Style", new Vector2(220 * _scale, 94 * _scale), true);
            {
                ImGui.PushItemWidth(80);
                ImGui.DragInt("Search Bar Width", ref Plugin.Settings.SearchBarWidth, 1, 50, 500);
                ImGui.ColorEdit4("Search Bar Background Color", ref Plugin.Settings.SearchBarBackgroundColor, ImGuiColorEditFlags.NoInputs);
                ImGui.ColorEdit4("Search Bar Text Color", ref Plugin.Settings.SearchBarTextColor, ImGuiColorEditFlags.NoInputs);
            }
            ImGui.EndChild();

            ImGui.Spacing();

            if (ImGui.Button("Edit Filters", new Vector2(220, 24)))
            {
                Plugin.OpenFiltersWindow();
            }
        }
    }
}
