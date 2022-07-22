using Lumina.Excel.GeneratedSheets;

namespace InventorySearchBar.Filters
{
    internal abstract class Filter
    {
        protected abstract bool Enabled { get; }
        protected abstract string Tag { get; }
        protected abstract string AbbreviatedTag { get; }
        protected abstract bool NeedsTag { get; }

        private string TagCharacter => Plugin.Settings.TagSeparatorCharacter;

        public bool FilterItem(Item item, string term)
        {
            if (!Enabled) { return true; }

            bool hasAnyTag = term.Contains(TagCharacter);
            bool hasTag = HasTag(term);

            if (NeedsTag && !hasTag) { return false; }
            if (!hasTag && hasAnyTag) { return false; }

            string t = term;
            if (hasTag)
            {
                t = RemoveTag(t);
            }

            return Execute(item, t);
        }

        private bool HasTag(string text)
        {
            return text.StartsWith(Tag + TagCharacter) || text.StartsWith(AbbreviatedTag + TagCharacter);
        }

        private string RemoveTag(string text)
        {
            string t = text.Replace(Tag + TagCharacter, "");
            t = text.Replace(AbbreviatedTag + TagCharacter, "");

            return t;
        }

        protected abstract bool Execute(Item item, string term);
    }
}
