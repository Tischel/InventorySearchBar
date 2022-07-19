using FFXIVClientStructs.FFXIV.Component.GUI;
using System;

namespace InventorySearchBar.Helpers
{
    public class ChatHelper
    {
        public static unsafe bool IsInputTextActive()
        {
            IntPtr ptr = *(IntPtr*)((IntPtr)AtkStage.GetSingleton() + 0x28) + 0x188E;
            return ptr != IntPtr.Zero && *(bool*)ptr;
        }
    }
}
