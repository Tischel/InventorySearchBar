using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using InventorySearchBar.Filters;
using System.Numerics;
using TPie.Helpers;

namespace InventorySearchBar.Windows
{
    public class FiltersWindow : Window
    {
        private float _scale => ImGuiHelpers.GlobalScale;

        public FiltersWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse;

            Size = new Vector2(236, 175);
        }

        public override void Draw()
        {
            if (!ImGui.BeginTabBar("##InventorySearchBar_Filters_TabBar"))
            {
                return;
            }

            if (ImGui.BeginTabItem("General##ISB_Filters"))
            {
                DrawGeneralTab();
                ImGui.EndTabItem();
            }

            foreach (Filter filter in Plugin.Filters)
            {
                if (ImGui.BeginTabItem(filter.Name + "##ISB_Settings"))
                {
                    DrawHelper.SetTooltip(filter.HelpText);

                    filter.Draw(_scale);
                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
        }

        private void DrawGeneralTab()
        {
            ImGui.PushItemWidth(20);

            string original = Plugin.Settings.SearchTermsSeparatorCharacter;
            if (ImGui.InputText("Search terms separator", ref Plugin.Settings.SearchTermsSeparatorCharacter, 1))
            {
                if (Plugin.Settings.SearchTermsSeparatorCharacter.Length == 0)
                {
                    Plugin.Settings.SearchTermsSeparatorCharacter = original;
                }
            }
            DrawHelper.SetTooltip("You can search for multiple things by separating each search term with this character");

            original = Plugin.Settings.SearchTermsSeparatorCharacter;
            if (ImGui.InputText("Filter tag separator", ref Plugin.Settings.TagSeparatorCharacter, 1))
            {
                if (Plugin.Settings.TagSeparatorCharacter.Length == 0)
                {
                    Plugin.Settings.TagSeparatorCharacter = original;
                }
            }
            DrawHelper.SetTooltip("This is the character used to identify filter tags.");
        }
    }
}
