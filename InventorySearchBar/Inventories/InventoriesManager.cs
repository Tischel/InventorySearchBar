using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class InventoriesManager : IDisposable
    {
        private List<Inventory> _inventories;
        public Inventory? ActiveInventory = null;

        public InventoriesManager()
        {
            _inventories = new List<Inventory>()
            {
                new NormalInventory(),
                new LargeInventory(),
                new LargestInventory(),
                new ChocoboInventory(),
                new ChocoboInventory2(),
                new RetainerInventory(),
                new LargeRetainerInventory(),
                new ArmouryInventory()
            };
        }

        public void Update()
        {
            foreach (Inventory inventory in _inventories)
            {
                inventory.UpdateAddonReference();
            }

            ActiveInventory = _inventories.FirstOrDefault(o => o.IsVisible && o.IsFocused());
        }

        public void ClearHighlights()
        {
            foreach (Inventory inventory in _inventories)
            {
                inventory.ClearHighlights();
            }
        }

        public void Dispose()
        {
            _inventories.Clear();
        }
    }
}
