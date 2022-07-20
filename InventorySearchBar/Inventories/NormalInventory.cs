using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class NormalInventory : CharacterInventory
    {
        public override string AddonName => "Inventory";

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            int offset = GetGridOffset();
            if (offset == -1) { return; }

            AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("InventoryGrid", 1);
            UpdateGridHighlights(grid, 3, offset);

            HighlightTabs(forced);
        }

        public unsafe void HighlightTabs(bool forced = false)
        {
            if (_node->UldManager.NodeListCount < 14) { return; }

            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[14];
            bool resultsInFirstTab = _filter != null && _filter[0].Any(b => b == true);
            SetTabHighlight(firstBagTab, resultsInFirstTab);

            AtkResNode* secondBagTab = _node->UldManager.NodeList[13];
            bool resultsInSecondTab = _filter != null && _filter[1].Any(b => b == true);
            SetTabHighlight(secondBagTab, resultsInSecondTab);

            AtkResNode* thirdBagTab = _node->UldManager.NodeList[12];
            bool resultsInThirdTab = _filter != null && _filter[2].Any(b => b == true);
            SetTabHighlight(thirdBagTab, resultsInThirdTab);

            AtkResNode* fourthBagTab = _node->UldManager.NodeList[11];
            bool resultsInFourthTab = _filter != null && _filter[3].Any(b => b == true);
            SetTabHighlight(fourthBagTab, resultsInFourthTab);
        }

        public unsafe int GetGridOffset()
        {
            if (_node->UldManager.NodeListCount < 14) { return -1; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[14];
            if (GetTabEnabled(firstBagTab->GetComponent()))
            {
                return 0;
            }

            AtkResNode* secondBagTab = _node->UldManager.NodeList[13];
            if (GetTabEnabled(secondBagTab->GetComponent()))
            {
                return 1;
            }

            AtkResNode* thirdBagTab = _node->UldManager.NodeList[12];
            if (GetTabEnabled(thirdBagTab->GetComponent()))
            {
                return 2;
            }

            AtkResNode* fourthBagTab = _node->UldManager.NodeList[11];
            if (GetTabEnabled(fourthBagTab->GetComponent()))
            {
                return 3;
            }

            return -1;
        }
    }
}
