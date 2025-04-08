using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class LargeInventory : CharacterInventory
    {
        public override string AddonName => "InventoryLarge";
        public override int OffsetX => Plugin.Settings.LargeInventoryOffset;

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            int offset = GetGridOffset();
            if (offset == -1) { return; }

            for (int i = 0; i < 2; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Services.GameGui.GetAddonByName("InventoryGrid" + i, 1);
                UpdateGridHighlights(grid, 3, offset + i);
            }

            HighlightTabs(forced);
        }

        public unsafe void HighlightTabs(bool forced = false)
        {
            if (_node->UldManager.NodeListCount < 70) { return; }

            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[70];
            bool resultsInFirstTab = _filter != null && (_filter[0].Any(b => b == true) || _filter[1].Any(b => b == true));
            SetTabHighlight(firstBagTab, resultsInFirstTab);

            AtkResNode* secondBagTab = _node->UldManager.NodeList[69];
            bool resultsInSecondTab = _filter != null && (_filter[2].Any(b => b == true) || _filter[3].Any(b => b == true));
            SetTabHighlight(secondBagTab, resultsInSecondTab);
        }

        public unsafe int GetGridOffset()
        {
            if (_node->UldManager.NodeListCount < 70) { return -1; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[70];
            if (GetTabEnabled(firstBagTab->GetComponent()))
            {
                return 0;
            }

            AtkResNode* secondBagTab = _node->UldManager.NodeList[69];
            if (GetTabEnabled(secondBagTab->GetComponent()))
            {
                return 2;
            }

            return -1;
        }
    }
}
