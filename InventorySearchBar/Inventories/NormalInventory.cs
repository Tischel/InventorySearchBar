using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class NormalInventory : CharacterInventory
    {
        public override string AddonName => "Inventory";
        public override int OffsetX => Plugin.Settings.NormalInventoryOffset;

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
            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            for (int i = 0; i < 4; i++)
            {
                UpdateTabHighlight(i);
            }
        }

        private unsafe void UpdateTabHighlight(int index)
        {
            if (_node == null || _node->UldManager.NodeListCount < 15) { return; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[15 - index];
            bool resultsInFirstTab = _filter != null && _filter[index].Any(b => b == true);
            SetTabHighlight(firstBagTab, resultsInFirstTab);
        }

        public unsafe int GetGridOffset()
        {
            if (_node == null || _node->UldManager.NodeListCount < 15) { return -1; }

            for (int i = 0; i < 4; i++)
            {
                AtkResNode* bagNode = _node->UldManager.NodeList[15 - i];
                if (GetTabEnabled(bagNode->GetComponent()))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
