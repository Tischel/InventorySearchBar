using FFXIVClientStructs.FFXIV.Component.GUI;

namespace InventorySearchBar.Helpers
{
    public class BaseInventoryHelper
    {
        public static unsafe void SetNodeHighlight(AtkResNode* node, bool highlight)
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
