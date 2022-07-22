using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace InventorySearchBar.Filters
{
    internal class LevelFilter : Filter
    {
        protected override bool Enabled => Plugin.Settings.LevelFilterEnabled;
        protected override bool NeedsTag => Plugin.Settings.LevelFilterRequireTag;
        protected override string Tag => Plugin.Settings.LevelFilterTag.ToUpper();
        protected override string AbbreviatedTag => Plugin.Settings.LevelFilterAbbreviatedTag.ToUpper();

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
