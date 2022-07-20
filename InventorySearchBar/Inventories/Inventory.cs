using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Inventories
{
    internal abstract unsafe class Inventory
    {
        public abstract string AddonName { get; }

        protected IntPtr _addon = IntPtr.Zero;
        public IntPtr Addon => _addon;

        protected AtkUnitBase* _node => (AtkUnitBase*)_addon;

        protected List<List<bool>> _emptyFilter = null!;
        protected List<List<bool>>? _filter = null!;

        public bool IsVisible => _node != null && _node->IsVisible;
        public bool IsFocused()
        {
            if (_node == null || !_node->IsVisible) { return false; }
            if (_node->UldManager.NodeListCount < 2) { return false; }

            AtkComponentNode* window = _node->UldManager.NodeList[1]->GetAsAtkComponentNode();
            if (window == null || window->Component->UldManager.NodeListCount < 4) { return false; }

            return window->Component->UldManager.NodeList[3]->IsVisible;
        }

        public void UpdateAddonReference()
        {
            _addon = Plugin.GameGui.GetAddonByName(AddonName, 1);
        }

        public abstract void ApplyFilter(string searchTerm);

        public void UpdateHighlights()
        {
            InternalUpdateHighlights(false);
        }

        protected abstract void InternalUpdateHighlights(bool forced = false);

        public void ClearHighlights()
        {
            _filter = null;
            InternalUpdateHighlights(true);
        }

        protected const int kGridItemCount = 35;

        protected unsafe void UpdateGridHighlights(AtkUnitBase* grid, int startIndex, int bagIndex, int count = kGridItemCount)
        {
            if (grid == null) { return; }

            for (int j = startIndex; j < startIndex + count; j++)
            {
                bool highlight = true;
                if (_filter != null && _filter[bagIndex].Count > j - startIndex)
                {
                    highlight = _filter[bagIndex][j - startIndex];
                }

                SetNodeHighlight(grid->UldManager.NodeList[j], highlight);
            }
        }

        protected static unsafe void SetNodeHighlight(AtkResNode* node, bool highlight)
        {
            node->MultiplyRed = highlight ? (byte)100 : (byte)20;
            node->MultiplyGreen = highlight ? (byte)100 : (byte)20;
            node->MultiplyBlue = highlight ? (byte)100 : (byte)20;
        }

        public static unsafe void SetTabHighlight(AtkResNode* tab, bool highlight)
        {
            tab->MultiplyRed = highlight ? (byte)250 : (byte)100;
            tab->MultiplyGreen = highlight ? (byte)250 : (byte)100;
            tab->MultiplyBlue = highlight ? (byte)250 : (byte)100;
        }

        public static unsafe bool GetTabEnabled(AtkComponentBase* tab)
        {
            if (tab->UldManager.NodeListCount < 2) { return false; }

            return tab->UldManager.NodeList[2]->IsVisible;
        }
    }
}
