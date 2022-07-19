using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Helpers
{
    public class LargeInventoryHelper : BaseInventoryHelper
    {
        public static unsafe IntPtr GetNode()
        {
            return Plugin.GameGui.GetAddonByName("InventoryLarge", 1);
        }

        public static unsafe void HighlightItems(IntPtr addon, List<List<bool>>? results)
        {
            int offset = GetGridOffset(addon);
            if (offset == -1) { return; }

            for (int i = 0; i < 2; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("InventoryGrid" + i, 1);
                if (grid == null) { continue; }

                for (int j = 3; j < grid->UldManager.NodeListCount; j++)
                {
                    bool highlight = true;
                    if (results != null && results[offset + i].Count > j - 3)
                    {
                        highlight = results[offset + i][j - 3];
                    }

                    SetNodeHighlight(grid->UldManager.NodeList[j], highlight);
                }
            }

            HighlightTabs(addon, results);
        }

        public static unsafe void HighlightTabs(IntPtr addon, List<List<bool>>? results, bool forced = false)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
            if (inventory->UldManager.NodeListCount < 69) { return; }

            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            AtkResNode* firstBagTab = inventory->UldManager.NodeList[69];
            bool resultsInFirstTab = results != null && (results[0].Any(b => b == true) || results[1].Any(b => b == true));
            SetTabHighlight(firstBagTab, resultsInFirstTab);

            AtkResNode* secondBagTab = inventory->UldManager.NodeList[68];
            bool resultsInSecondTab = results != null && (results[2].Any(b => b == true) || results[3].Any(b => b == true));
            SetTabHighlight(secondBagTab, resultsInSecondTab);
        }

        public static unsafe int GetGridOffset(IntPtr addon)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
            if (inventory->UldManager.NodeListCount < 69) { return -1; }

            AtkResNode* firstBagTab = inventory->UldManager.NodeList[69];
            if (GetTabEnabled(firstBagTab->GetComponent()))
            {
                return 0;
            }

            AtkResNode* secondBagTab = inventory->UldManager.NodeList[68];
            if (GetTabEnabled(secondBagTab->GetComponent()))
            {
                return 2;
            }

            return -1;
        }
    }
}
