using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class RetainerInventory : Inventory
    {
        public override string AddonName => "InventoryRetainer";
        protected override ulong CharacterId => Plugin.CharacterMonitor.ActiveRetainer;
        protected override InventoryCategory Category => InventoryCategory.RetainerBags;
        protected override int FirstBagOffset => (int)InventoryType.RetainerBag0;

        public RetainerInventory()
        {
            // 5 grids of 35 items
            _emptyFilter = new List<List<bool>>();
            for (int i = 0; i < 5; i++)
            {
                List<bool> list = new List<bool>(kGridItemCount);
                for (int j = 0; j < kGridItemCount; j++)
                {
                    list.Add(false);
                }

                _emptyFilter.Add(list);
            }
        }

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            int offset = GetGridOffset();
            if (offset == -1) { return; }

            AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("RetainerGrid", 1);
            UpdateGridHighlights(grid, 3, offset);

            HighlightTabs(forced);
        }

        public unsafe void HighlightTabs(bool forced = false)
        {
            if (!Plugin.Settings.HightlightTabs && !forced) { return; }

            for (int i = 0; i < 5; i++)
            {
                UpdateTabHighlight(i);
            }
        }

        private unsafe void UpdateTabHighlight(int index)
        {
            if (_node == null || _node->UldManager.NodeListCount < 12) { return; }

            AtkResNode* firstBagTab = _node->UldManager.NodeList[12 - index];
            bool resultsInFirstTab = _filter != null && _filter[index].Any(b => b == true);
            SetTabHighlight(firstBagTab, resultsInFirstTab);
        }

        public unsafe int GetGridOffset()
        {
            if (_node == null || _node->UldManager.NodeListCount < 12) { return -1; }

            for (int i = 0; i < 5; i++)
            {
                AtkResNode* bagNode = _node->UldManager.NodeList[12 - i];
                if (GetSmallTabEnabled(bagNode->GetComponent()))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
