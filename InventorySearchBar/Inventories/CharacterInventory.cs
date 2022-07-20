using CriticalCommonLib;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Inventories
{
    internal class CharacterInventory : Inventory
    {
        public override string AddonName => throw new NotImplementedException();

        public CharacterInventory()
        {
            _emptyFilter = new List<List<bool>>();
            for (int i = 0; i < 4; i++)
            {
                List<bool> list = new List<bool>(35);
                for (int j = 0; j < 35; j++)
                {
                    list.Add(false);
                }

                _emptyFilter.Add(list);
            }
        }

        public override void ApplyFilter(string searchTerm)
        {
            if (searchTerm.Length < 1)
            {
                _filter = null;
                return;
            }

            _filter = new List<List<bool>>(_emptyFilter);

            string text = searchTerm.ToUpper();
            List<InventoryItem> items = Plugin.InventoryMonitor.GetSpecificInventory(Plugin.CharacterMonitor.ActiveCharacter, InventoryCategory.CharacterBags);

            foreach (InventoryItem item in items)
            {
                bool highlight = false;
                if (item.Item != null)
                {
                    highlight = item.Item.Name.ToString().ToUpper().Contains(text);
                }

                _filter[(int)item.SortedContainer][34 - item.SortedSlotIndex] = highlight;
            }
        }

        protected override void InternalUpdateHighlights(bool forced = false)
        {
            throw new NotImplementedException();
        }
    }
}
