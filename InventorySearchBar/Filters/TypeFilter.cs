using Lumina.Excel.GeneratedSheets;

namespace InventorySearchBar.Filters
{
    internal class TypeFilter : Filter
    {
        protected override bool Enabled => Plugin.Settings.TypeFilterEnabled;
        protected override bool NeedsTag => Plugin.Settings.TypeFilterRequireTag;
        protected override string Tag => Plugin.Settings.TypeFilterTag.ToUpper();
        protected override string AbbreviatedTag => Plugin.Settings.TypeFilterAbbreviatedTag.ToUpper();

        protected override bool Execute(Item item, string term)
        {
            if (item.ItemUICategory.Value == null) { return false; }

            return item.ItemUICategory.Value.Name.ToString().ToUpper().Contains(term);
        }
    }
}
