using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySearchBar
{
    internal class InventoryHelper
    {
        #region Singleton
        private InventoryHelper()
        {
            _cache = new Dictionary<uint, string>();
            ExcelSheet<Item>? itemsSheet = Plugin.DataManager.GetExcelSheet<Item>();
            if (itemsSheet == null) { return; }

            foreach (Item row in itemsSheet)
            {
                _cache.Add(row.RowId, row.Name.ToString().ToUpper());
            }
        }

        public static void Initialize() { Instance = new InventoryHelper(); }

        public static InventoryHelper Instance { get; private set; } = null!;

        ~InventoryHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Instance = null!;
        }
        #endregion

        private Dictionary<uint, string> _cache;

        public unsafe List<List<bool>> FindItems(string searchTerm)
        {
            List<List<bool>> results = new List<List<bool>>();

            InventoryManager* manager = InventoryManager.Instance();
            InventoryType[] inventoryTypes = new InventoryType[]
            {
                InventoryType.Inventory1,
                InventoryType.Inventory2,
                InventoryType.Inventory3,
                InventoryType.Inventory4
            };

            try
            {
                foreach (InventoryType inventoryType in inventoryTypes)
                {
                    InventoryContainer* container = manager->GetInventoryContainer(inventoryType);
                    if (container == null) continue;

                    List<bool> list = new List<bool>();

                    for (int i = 0; i < container->Size; i++)
                    {
                        try
                        {
                            InventoryItem* item = container->GetInventorySlot(i);
                            if (item == null)
                            {
                                list.Add(false);
                                continue;
                            }

                            if (item->ItemID == 0)
                            {
                                list.Add(true);
                            }
                            else if (_cache.TryGetValue(item->ItemID, out string? name) && name != null)
                            {
                                list.Add(name.Contains(searchTerm.ToUpper()));
                            }
                            else
                            {
                                list.Add(false);
                            }
                        }
                        catch { }
                    }

                    results.Add(list);
                }
            }
            catch { }

            return results;
        }
    }

    public struct InventorySearchResult
    {
        public readonly int Grid;
        public readonly int Slot;

        public InventorySearchResult(int grid, int slot)
        {
            Grid = grid;
            Slot = slot;
        }
    }
}
