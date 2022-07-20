using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Inventories
{
    internal class ChocoboInventory : Inventory
    {
        public override string AddonName => "InventoryBuddy";

        public ChocoboInventory()
        {
            _emptyFilter = new List<List<bool>>();
            for (int i = 0; i < 2; i++)
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
            List<InventoryItem> items = Plugin.InventoryMonitor.GetSpecificInventory(Plugin.CharacterMonitor.ActiveCharacter, InventoryCategory.CharacterSaddleBags);

            foreach (InventoryItem item in items)
            {
                bool highlight = false;
                if (item.Item != null)
                {
                    highlight = item.Item.Name.ToString().ToUpper().Contains(text);
                }

                int bagIndex = (int)item.SortedContainer - (int)InventoryType.SaddleBag0;
                _filter[bagIndex][34 - item.SortedSlotIndex] = highlight;
            }
        }

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_node == null || _node->UldManager.NodeListCount < 78) { return; }

            UpdateGridHighlights(_node, 44, 0); // left grid
            UpdateGridHighlights(_node, 8, 1); // right grid
        }
    }
}
