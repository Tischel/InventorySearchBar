using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using System.Collections.Generic;

namespace InventorySearchBar.Inventories
{
    internal class ChocoboInventory : Inventory
    {
        public override string AddonName => "InventoryBuddy";
        protected override ulong CharacterId => Plugin.CharacterMonitor.ActiveCharacter;
        protected override InventoryCategory Category => InventoryCategory.CharacterSaddleBags;
        protected override int FirstBagOffset => (int)InventoryType.SaddleBag0;

        public ChocoboInventory()
        {
            // 2 grids of 35 items
            _emptyFilter = new List<List<bool>>();
            for (int i = 0; i < 2; i++)
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
            if (_node == null || _node->UldManager.NodeListCount < 78) { return; }

            UpdateGridHighlights(_node, 44, 0); // left grid
            UpdateGridHighlights(_node, 8, 1); // right grid
        }
    }

    internal class ChocoboInventory2 : ChocoboInventory
    {
        public override string AddonName => "InventoryBuddy2";
    }
}
