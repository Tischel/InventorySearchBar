using Lumina.Excel.Sheets;

namespace InventorySearchBar.Filters
{
    internal class NameFilter : Filter
    {
        public override string Name => "Name";
        public override string HelpText => "Allow to filter items by checking if their name contains the search term.\nExamples: '" + Plugin.Settings.NameFilterTag + ":Materia', '" + Plugin.Settings.NameFilterAbbreviatedTag + ":token'.";

        protected override bool Enabled
        {
            get => Plugin.Settings.NameFilterEnabled;
            set => Plugin.Settings.NameFilterEnabled = value;
        }

        protected override bool NeedsTag
        {
            get => Plugin.Settings.NameFilterRequireTag;
            set => Plugin.Settings.NameFilterRequireTag = value;
        }

        protected override string Tag
        {
            get => Plugin.Settings.NameFilterTag;
            set => Plugin.Settings.NameFilterTag = value;
        }

        protected override string AbbreviatedTag
        {
            get => Plugin.Settings.NameFilterAbbreviatedTag;
            set => Plugin.Settings.NameFilterAbbreviatedTag = value;
        }

        protected override bool Execute(Item item, string term)
        {
            return item.Name.ToString().ToUpper().Contains(term);
        }
    }
}
