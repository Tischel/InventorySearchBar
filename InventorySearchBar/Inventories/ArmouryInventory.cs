using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using InventorySearchBar.Filters;
using System.Collections.Generic;
using System.Linq;

namespace InventorySearchBar.Inventories
{
    internal class ArmouryInventory : Inventory
    {
        public override string AddonName => "ArmouryBoard";
        protected override ulong CharacterId => Plugin.CharacterMonitor.ActiveCharacterId;
        protected override InventoryCategory Category => InventoryCategory.CharacterArmoryChest;
        protected override int FirstBagOffset => _currentBag;
        protected override int GridItemCount => 50;
        public override int OffsetX => Plugin.Settings.ArmouryInventoryOffset;

        private int _currentBag = (int)InventoryType.ArmoryMain;

        private static Dictionary<int, int> _tabsMap = new Dictionary<int, int>()
        {
            [109] = (int)InventoryType.ArmorySoulCrystal,
            [110] = (int)InventoryType.ArmoryRing,
            [111] = (int)InventoryType.ArmoryWrist,
            [112] = (int)InventoryType.ArmoryNeck,
            [113] = (int)InventoryType.ArmoryEar,
            [114] = (int)InventoryType.ArmoryOff,
            [115] = (int)InventoryType.ArmoryFeet,
            [116] = (int)InventoryType.ArmoryLegs,
            [117] = (int)InventoryType.ArmoryHand,
            [118] = (int)InventoryType.ArmoryBody,
            [119] = (int)InventoryType.ArmoryHead,
            [120] = (int)InventoryType.ArmoryMain,
        };

        public ArmouryInventory()
        {

        }

        protected override List<List<bool>> GetEmptyFilter()
        {
            // 1 grid of 50 items
            List<List<bool>> emptyFilter = new List<List<bool>>();
            List<bool> list = new List<bool>(GridItemCount);
            for (int j = 0; j < GridItemCount; j++)
            {
                list.Add(false);
            }

            emptyFilter.Add(list);

            return emptyFilter;
        }

        public override void ApplyFilters(List<Filter> filters, string searchTerm)
        {
            _currentBag = GetCurrentBag();

            base.ApplyFilters(filters, searchTerm);
        }

        private unsafe int GetCurrentBag()
        {
            if (_node->UldManager.NodeListCount < 120) { return _currentBag; }

            foreach (KeyValuePair<int, int> tab in _tabsMap)
            {
                if (GetSmallTabEnabled(_node->UldManager.NodeList[tab.Key]->GetComponent()))
                {
                    return tab.Value;
                }
            }

            return _currentBag;
        }

        protected override List<InventoryItem> GetSortedItems()
        {
            return Plugin.InventoryMonitor.GetSpecificInventory(CharacterId, Category).Where(item => (int)item.SortedContainer == _currentBag).ToList();
        }

        protected override unsafe void InternalUpdateHighlights(bool forced = false)
        {
            if (_node == null || _node->UldManager.NodeListCount < 56) { return; }

            UpdateGridHighlights(_node, 7, 0);
        }
    }
}
