using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

namespace InventorySearchBar.Inventories
{
    internal class LargestInventory : CharacterInventory
    {
        public override string AddonName => "InventoryExpansion";
        public override int OffsetX => Plugin.Settings.LargestInventoryOffset;

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_addon == IntPtr.Zero) { return; }

            for (int i = 0; i < 4; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Services.GameGui.GetAddonByName("InventoryGrid" + i + "E", 1);
                UpdateGridHighlights(grid, 3, i);
            }
        }
    }
}
