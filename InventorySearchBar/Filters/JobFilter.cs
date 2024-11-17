

using AllaganLib.GameSheets.Sheets.Rows;

namespace InventorySearchBar.Filters
{
    internal class JobFilter : Filter
    {
        public override string Name => "Job";
        public override string HelpText => "Allow to filter items by checking if they are usable by a specific job.\nExamples: 'job:BLM', 'j:war'.";

        protected override bool Enabled
        {
            get => Plugin.Settings.JobFilterEnabled;
            set => Plugin.Settings.JobFilterEnabled = value;
        }

        protected override bool NeedsTag
        {
            get => Plugin.Settings.JobFilterRequireTag;
            set => Plugin.Settings.JobFilterRequireTag = value;
        }

        protected override string Tag
        {
            get => Plugin.Settings.JobFilterTag;
            set => Plugin.Settings.JobFilterTag = value;
        }

        protected override string AbbreviatedTag
        {
            get => Plugin.Settings.JobFilterAbbreviatedTag;
            set => Plugin.Settings.JobFilterAbbreviatedTag = value;
        }

        protected override bool Execute(ItemRow item, string term)
        {
            if (item.ClassJobCategory == null) { return false; }

            return item.ClassJobCategory.Base.Name.ExtractText().ToUpper().Contains(term);
        }
    }
}
