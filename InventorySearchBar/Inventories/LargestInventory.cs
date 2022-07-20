using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Inventories
{
    internal class LargestInventory : CharacterInventory
    {
        public override string AddonName => "InventoryExpansion";

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            for (int i = 0; i < 4; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("InventoryGrid" + i + "E", 1);
                UpdateGridHighlights(grid, 3, i);
            }
        }
    }
}
