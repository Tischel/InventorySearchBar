using CriticalCommonLib.Models;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InventorySearchBar.Filters;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

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

        protected abstract ulong CharacterId { get; }
        protected abstract InventoryCategory Category { get; }
        protected abstract int FirstBagOffset { get; }
        protected abstract int GridItemCount { get; }

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

        public virtual void ApplyFilters(List<Filter> filters, string text)
        {
            if (text.Length < 2)
            {
                _filter = null;
                return;
            }

            _filter = new List<List<bool>>(_emptyFilter);
            string[] searchTerms = text.ToUpper().Split(Plugin.Settings.SearchTermsSeparatorCharacter);

            // get items
            List<InventoryItem> items = GetSortedItems();

            foreach (InventoryItem item in items)
            {
                try
                {
                    // apply filters
                    bool highlight = false;

                    if (item.Item != null)
                    {
                        int successCount = 0;
                        foreach (string term in searchTerms)
                        {
                            foreach (Filter filter in filters)
                            {
                                if (filter.FilterItem(item.Item, term))
                                {
                                    successCount++;
                                    break;
                                }
                            }
                        }

                        // all search terms need to be found
                        highlight = successCount == searchTerms.Length;
                    }

                    // map
                    int bagIndex = (int)item.SortedContainer - FirstBagOffset;
                    if (_filter.Count > bagIndex)
                    {
                        List<bool> bag = _filter[bagIndex];
                        int slot = GridItemCount - 1 - item.SortedSlotIndex;
                        if (bag.Count > slot)
                        {
                            bag[slot] = highlight;
                        }
                    }
                }
                catch { }
                //catch (Exception e)
                //{
                //  PluginLog.Log(e.Message);
                //}
            }
        }

        protected virtual List<InventoryItem> GetSortedItems()
        {
            return Plugin.InventoryMonitor.GetSpecificInventory(CharacterId, Category);
        }

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

        protected unsafe void UpdateGridHighlights(AtkUnitBase* grid, int startIndex, int bagIndex)
        {
            if (grid == null) { return; }

            for (int j = startIndex; j < startIndex + GridItemCount; j++)
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
            node->MultiplyRed = highlight || !node->IsVisible ? (byte)100 : (byte)20;
            node->MultiplyGreen = highlight || !node->IsVisible ? (byte)100 : (byte)20;
            node->MultiplyBlue = highlight || !node->IsVisible ? (byte)100 : (byte)20;
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

        public static unsafe bool GetSmallTabEnabled(AtkComponentBase* tab)
        {
            if (tab->UldManager.NodeListCount < 1) { return false; }

            return tab->UldManager.NodeList[1]->IsVisible;
        }
    }
}
