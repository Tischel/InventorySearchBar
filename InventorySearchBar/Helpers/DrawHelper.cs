using ImGuiNET;

namespace TPie.Helpers
{
    internal static class DrawHelper
    {
        public static void SetTooltip(string message)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(message);
            }
        }
    }
}
