using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class LargeRetainerInventory : RetainerInventory
    {
        public override string AddonName => "InventoryRetainerLarge";
        public override int OffsetX => Plugin.Settings.LargeRetainerInventoryOffset;

        public LargeRetainerInventory() : base()
        {
            _tabCount = 3;
            _tabIndexStart = 67;
        }

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            int offset = GetGridOffset();
            if (offset == -1) { return; }

            int count = offset == 2 ? 1 : 2;
            int start = offset * 2;

            for (int i = start; i < start + count; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Services.GameGui.GetAddonByName("RetainerGrid" + i, 1);
                UpdateGridHighlights(grid, 3, i);
            }

            HighlightTabs(forced);
        }

        protected override unsafe void UpdateTabHighlight(int index)
        {
            if (_node == null || _node->UldManager.NodeListCount < _tabIndexStart) { return; }

            AtkResNode* tab = _node->UldManager.NodeList[_tabIndexStart - index];

            bool resultsInTab = false;
            if (index == 2)
            {
                resultsInTab = _filter != null && _filter[index * 2].Any(b => b == true);
            }
            else
            {
                resultsInTab = _filter != null && (_filter[index * 2].Any(b => b == true) || _filter[index * 2 + 1].Any(b => b == true));
            }

            SetTabHighlight(tab, resultsInTab);
        }

        public override unsafe int GetGridOffset()
        {
            if (_node == null || _node->UldManager.NodeListCount < _tabIndexStart) { return -1; }

            for (int i = 0; i < _tabCount; i++)
            {
                AtkResNode* bagNode = _node->UldManager.NodeList[_tabIndexStart - i];
                if (GetTabEnabled(bagNode->GetComponent()))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
