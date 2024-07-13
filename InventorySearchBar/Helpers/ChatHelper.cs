using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace InventorySearchBar.Helpers
{
    public class ChatHelper
    {
        public static unsafe bool IsInputTextActive()
        {
            Framework* framework = Framework.Instance();
            if (framework == null) { return false; }

            UIModule* module = framework->GetUIModule();
            if (module == null) { return false; }

            RaptureAtkModule* atkModule = module->GetRaptureAtkModule();
            if (atkModule == null) { return false; }

            return atkModule->AtkModule.IsTextInputActive();
        }
    }
}
