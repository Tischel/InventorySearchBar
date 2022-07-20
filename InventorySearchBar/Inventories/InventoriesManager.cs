using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
                new ChocoboInventory()
            };
        }

        public void Update()
        {
            foreach (Inventory inventory in _inventories)
            {
                inventory.UpdateAddonReference();
            }

            ActiveInventory = null;
            List<Inventory> visible = _inventories.Where(o => o.IsVisible).ToList();

            if (visible.Count == 1)
            {
                ActiveInventory = visible[0];
            }
            else if (visible.Count > 1)
            {
                ActiveInventory = visible.FirstOrDefault(o => o.IsFocused());
            }
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
