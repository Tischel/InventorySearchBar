using ImGuiNET;
using InventorySearchBar.Helpers;

namespace InventorySearchBar
{
    public class KeyBind
    {
        public int Key;
        public bool Ctrl;
        public bool Alt;
        public bool Shift;

        public KeyBind(int key, bool ctrl = false, bool alt = false, bool shift = false)
        {
            Ctrl = ctrl;
            Alt = alt;
            Shift = shift;
            Key = key;
        }

        public override string ToString()
        {
            string ctrl = Ctrl ? "Ctrl + " : "";
            string alt = Alt ? "Alt + " : "";
            string shift = Shift ? "Shift + " : "";
            string key = ((Keys)Key).ToString();

            return ctrl + alt + shift + key;
        }

        public bool IsActive()
        {
            if (ChatHelper.IsInputTextActive() == true || ImGui.GetIO().WantCaptureKeyboard)
            {
                return false;
            }

            ImGuiIOPtr io = ImGui.GetIO();
            bool ctrl = Ctrl ? io.KeyCtrl : !io.KeyCtrl;
            bool alt = Alt ? io.KeyAlt : !io.KeyAlt;
            bool shift = Shift ? io.KeyShift : !io.KeyShift;
            bool key = KeyboardHelper.Instance?.IsKeyPressed(Key) == true;
            bool active = ctrl && alt && shift && key;

            // block keybind for the game?
            if (active && !Plugin.Settings.KeybindPassthrough)
            {
                try
                {
                    Plugin.KeyState[Key] = false;
                }
                catch { }
            }

            return active;
        }

        public bool Draw(float width)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            string dispKey = ToString();

            ImGui.PushItemWidth(width);
            ImGui.InputText($"Keybind##InventorySearchBar", ref dispKey, 200, ImGuiInputTextFlags.ReadOnly);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Use this keybind to focus on the search bar. Backspace to clear.");
            }

            if (ImGui.IsItemActive())
            {
                if (KeyboardHelper.Instance?.IsKeyPressed((int)Keys.Back) == true)
                {
                    Reset();
                }
                else
                {
                    int keyPressed = KeyboardHelper.Instance?.GetKeyPressed() ?? 0;
                    if (keyPressed > 0)
                    {
                        Ctrl = io.KeyCtrl;
                        Alt = io.KeyAlt;
                        Shift = io.KeyShift;
                        Key = keyPressed;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
            Key = 0;
            Ctrl = false;
            Alt = false;
            Shift = false;
        }
    }
}
