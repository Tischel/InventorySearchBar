using Lumina.Excel.GeneratedSheets;

namespace InventorySearchBar.Filters
{
    internal class NameFilter : Filter
    {
        protected override bool Enabled => Plugin.Settings.NameFilterEnabled;
        protected override bool NeedsTag => Plugin.Settings.NameFilterRequireTag;
        protected override string Tag => Plugin.Settings.NameFilterTag.ToUpper();
        protected override string AbbreviatedTag => Plugin.Settings.NameFilterAbbreviatedTag.ToUpper();

        protected override bool Execute(Item item, string term)
        {
            return item.Name.ToString().ToUpper().Contains(term);
        }
    }
}
