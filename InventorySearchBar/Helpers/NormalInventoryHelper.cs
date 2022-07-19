using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySearchBar.Helpers
{
    public class NormalInventoryHelper : BaseInventoryHelper
    {
        public static unsafe IntPtr GetNode()
        {
            return Plugin.GameGui.GetAddonByName("Inventory", 1);
        }

        public static unsafe void HighlightItems(IntPtr addon, List<List<bool>>? results)
        {
            int offset = GetGridOffset(addon);
            if (offset == -1) { return; }

            AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("InventoryGrid", 1);
            if (grid == null) { return; }

            for (int j = 3; j < grid->UldManager.NodeListCount; j++)
            {
                bool highlight = true;
                if (results != null && results[offset].Count > j - 3)
                {
                    highlight = results[offset][j - 3];
                }

                SetNodeHighlight(grid->UldManager.NodeList[j], highlight);
            }

            HighlightTabs(addon, results);
        }

        public static unsafe void HighlightTabs(IntPtr addon, List<List<bool>>? results, bool forced = false)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
            if (inventory->UldManager.NodeListCount < 14) { return; }

            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            AtkResNode* firstBagTab = inventory->UldManager.NodeList[14];
            bool resultsInFirstTab = results != null && results[0].Any(b => b == true);
            SetTabHighlight(firstBagTab, resultsInFirstTab);

            AtkResNode* secondBagTab = inventory->UldManager.NodeList[13];
            bool resultsInSecondTab = results != null && results[1].Any(b => b == true);
            SetTabHighlight(secondBagTab, resultsInSecondTab);

            AtkResNode* thirdBagTab = inventory->UldManager.NodeList[12];
            bool resultsInThirdTab = results != null && results[2].Any(b => b == true);
            SetTabHighlight(thirdBagTab, resultsInThirdTab);

            AtkResNode* fourthBagTab = inventory->UldManager.NodeList[11];
            bool resultsInFourthTab = results != null && results[3].Any(b => b == true);
            SetTabHighlight(fourthBagTab, resultsInFourthTab);
        }

        public static unsafe int GetGridOffset(IntPtr addon)
        {
            AtkUnitBase* inventory = (AtkUnitBase*)addon;
            if (inventory->UldManager.NodeListCount < 14) { return -1; }

            AtkResNode* firstBagTab = inventory->UldManager.NodeList[14];
            if (GetTabEnabled(firstBagTab->GetComponent()))
            {
                return 0;
            }

            AtkResNode* secondBagTab = inventory->UldManager.NodeList[13];
            if (GetTabEnabled(secondBagTab->GetComponent()))
            {
                return 1;
            }

            AtkResNode* thirdBagTab = inventory->UldManager.NodeList[12];
            if (GetTabEnabled(thirdBagTab->GetComponent()))
            {
                return 2;
            }

            AtkResNode* fourthBagTab = inventory->UldManager.NodeList[11];
            if (GetTabEnabled(fourthBagTab->GetComponent()))
            {
                return 3;
            }

            return -1;
        }
    }
}
