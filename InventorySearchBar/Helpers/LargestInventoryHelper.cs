using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySearchBar.Helpers
{
    public class LargestInventoryHelper : BaseInventoryHelper
    {
        public static unsafe IntPtr GetNode()
        {
            return Plugin.GameGui.GetAddonByName("InventoryExpansion", 1);
        }

        public static unsafe void HighlightItems(List<List<bool>>? results)
        {
            for (int i = 0; i < 4; i++)
            {
                AtkUnitBase* grid = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("InventoryGrid" + i + "E", 1);
                if (grid == null) { continue; }

                for (int j = 3; j < grid->UldManager.NodeListCount; j++)
                {
                    bool highlight = true;
                    if (results != null && results[i].Count > j - 3)
                    {
                        highlight = results[i][j - 3];
                    }

                    SetNodeHighlight(grid->UldManager.NodeList[j], highlight);
                }
            }
        }
    }
}
