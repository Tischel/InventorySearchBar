using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace InventorySearchBar
{
    public class SettingsWindow : Window
    {
        public SettingsWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse;

            Size = new Vector2(220, 190);
        }

        public override void Draw()
        {
            ImGui.Checkbox("Automatically clear search", ref Plugin.Settings.AutoClear);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("If enabled, the search bar will be emptied every time the inventory is opened.");
            }

            ImGui.Checkbox("Automatically focus search bar", ref Plugin.Settings.AutoFocus);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("If enabled, the search bar will focused every time the inventory is opened.");
            }

            if (ImGui.Checkbox("Highlight tabs", ref Plugin.Settings.HightlightTabs))
            {
                if (!Plugin.Settings.HightlightTabs)
                {
                    Plugin.ClearTabHighlights();
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("If enabled, the inventory tabs will be highlighted if they contain an item that satisfies the search.");
            }

            ImGui.NewLine();

            ImGui.ColorEdit4("Search Bar Background Color", ref Plugin.Settings.SearchBarBackgroundColor, ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Search Bar Text Color", ref Plugin.Settings.SearchBarTextColor, ImGuiColorEditFlags.NoInputs);
        }
    }
}
