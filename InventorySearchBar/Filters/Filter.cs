using ImGuiNET;
using Lumina.Excel.Sheets;
using TPie.Helpers;

namespace InventorySearchBar.Filters
{
    public abstract class Filter
    {
        public abstract string Name { get; }
        public abstract string HelpText { get; }

        protected abstract bool Enabled { get; set; }
        protected abstract bool NeedsTag { get; set; }
        protected abstract string Tag { get; set; }
        protected abstract string AbbreviatedTag { get; set; }

        private string _tag => Tag.ToUpper();
        private string _abbreviatedTag => AbbreviatedTag.ToUpper();

        private string _tagCharacter => Plugin.Settings.TagSeparatorCharacter;

        public bool FilterItem(Item item, string term)
        {
            if (!Enabled) { return true; }

            bool hasAnyTag = term.Contains(_tagCharacter);
            bool hasTag = HasTag(term);

            if (NeedsTag && !hasTag) { return false; }
            if (!hasTag && hasAnyTag) { return false; }

            string t = term;
            if (hasTag)
            {
                t = RemoveTag(t);
            }

            if (t.Length == 0) { return true; }

            return Execute(item, t);
        }

        private bool HasTag(string text)
        {
            return text.StartsWith(_tag + _tagCharacter) || text.StartsWith(_abbreviatedTag + _tagCharacter);
        }

        private string RemoveTag(string text)
        {
            string t = text.Replace(_tag + _tagCharacter, "");
            t = t.Replace(_abbreviatedTag + _tagCharacter, "");

            return t;
        }

        protected abstract bool Execute(Item item, string term);

        public void Draw(float scale)
        {
            bool enabled = Enabled;
            if (ImGui.Checkbox("Enabled", ref enabled))
            {
                Enabled = enabled;
            }

            bool needsTag = NeedsTag;
            if (ImGui.Checkbox("Requires Tag", ref needsTag))
            {
                NeedsTag = needsTag;
            }
            DrawHelper.SetTooltip("If enabled, the filter will only be applied if the search term begins with '" + Tag + _tagCharacter + "' or '" + AbbreviatedTag + _tagCharacter + "'.");

            ImGui.PushItemWidth(100);
            string tag = Tag;
            if (ImGui.InputText("Tag", ref tag, 10))
            {
                Tag = tag;
            }

            string abbreviatedTag = AbbreviatedTag;
            if (ImGui.InputText("Abbreviated tag", ref abbreviatedTag, 1))
            {
                if (abbreviatedTag.Length > 0)
                {
                    AbbreviatedTag = abbreviatedTag;
                }
            }
        }
    }
}
