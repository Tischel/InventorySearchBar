using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace InventorySearchBar.Filters
{
    internal class LevelFilter : Filter
    {
        public override string Name => "Level";
        public override string HelpText => "Allow to filter items by checking if their level requirement.\nExamples: '" + Plugin.Settings.LevelFilterTag + ":60', '" + Plugin.Settings.LevelFilterAbbreviatedTag + ":>=70', '" + Plugin.Settings.LevelFilterTag + ":=90'.";

        protected override bool Enabled
        {
            get => Plugin.Settings.LevelFilterEnabled;
            set => Plugin.Settings.LevelFilterEnabled = value;
        }

        protected override bool NeedsTag
        {
            get => Plugin.Settings.LevelFilterRequireTag;
            set => Plugin.Settings.LevelFilterRequireTag = value;
        }

        protected override string Tag
        {
            get => Plugin.Settings.LevelFilterTag;
            set => Plugin.Settings.LevelFilterTag = value;
        }

        protected override string AbbreviatedTag
        {
            get => Plugin.Settings.LevelFilterAbbreviatedTag;
            set => Plugin.Settings.LevelFilterAbbreviatedTag = value;
        }

        private enum ComparisonType
        {
            Less = 0,
            LessOrEqual = 1,
            Equal = 2,
            GreaterOrEqual = 3,
            Greater = 4
        }

        private static Dictionary<string, ComparisonType> _operatorsMap = new Dictionary<string, ComparisonType>()
        {
            ["<="] = ComparisonType.LessOrEqual,
            [">="] = ComparisonType.GreaterOrEqual,
            ["<"] = ComparisonType.Less,
            ["="] = ComparisonType.Equal,
            [">"] = ComparisonType.Greater
        };

        protected override bool Execute(Item item, string term)
        {
            ComparisonType comparison = ComparisonType.LessOrEqual;
            byte value = 0;
            bool found = false;

            try
            {
                foreach (KeyValuePair<string, ComparisonType> o in _operatorsMap)
                {
                    if (term.StartsWith(o.Key))
                    {
                        comparison = o.Value;
                        value = byte.Parse(term[o.Key.Length..]);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    value = byte.Parse(term);
                }
            }
            catch
            {
                return false;
            }

            switch (comparison)
            {
                case ComparisonType.Less: return item.LevelEquip < value;
                case ComparisonType.LessOrEqual: return item.LevelEquip <= value;
                case ComparisonType.Equal: return item.LevelEquip == value;
                case ComparisonType.GreaterOrEqual: return item.LevelEquip >= value;
                case ComparisonType.Greater: return item.LevelEquip > value;
            }

            return false;
        }
    }
}
