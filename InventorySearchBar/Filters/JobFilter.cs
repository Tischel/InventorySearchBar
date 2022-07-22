using Lumina.Excel.GeneratedSheets;

namespace InventorySearchBar.Filters
{
    internal class JobFilter : Filter
    {
        protected override bool Enabled => Plugin.Settings.JobFilterEnabled;
        protected override bool NeedsTag => Plugin.Settings.JobFilterRequireTag;
        protected override string Tag => Plugin.Settings.JobFilterTag.ToUpper();
        protected override string AbbreviatedTag => Plugin.Settings.JobFilterAbbreviatedTag.ToUpper();

        protected override bool Execute(Item item, string term)
        {
            if (item.ClassJobCategory.Value == null) { return false; }

            return item.ClassJobCategory.Value.Name.ToString().ToUpper().Contains(term);
        }
    }
}
