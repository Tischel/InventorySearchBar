using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using System;
using System.Collections.Generic;

namespace InventorySearchBar.Inventories
{
    internal class CharacterInventory : Inventory
    {
        public override string AddonName => throw new NotImplementedException();
        protected override ulong CharacterId => Plugin.CharacterMonitor.ActiveCharacter;
        protected override InventoryCategory Category => InventoryCategory.CharacterBags;
        protected override int FirstBagOffset => (int)InventoryType.Bag0;
        protected override int GridItemCount => 35;
        public override int OffsetX => 0;

        public CharacterInventory()
        {
            // 4 grids of 35 items
            _emptyFilter = new List<List<bool>>();
            for (int i = 0; i < 4; i++)
            {
                List<bool> list = new List<bool>(GridItemCount);
                for (int j = 0; j < GridItemCount; j++)
                {
                    list.Add(false);
                }

                _emptyFilter.Add(list);
            }
        }

        protected override void InternalUpdateHighlights(bool forced = false)
        {
            throw new NotImplementedException();
        }
    }
}
