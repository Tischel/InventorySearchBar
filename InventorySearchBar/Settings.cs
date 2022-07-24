using Dalamud.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

namespace InventorySearchBar
{
    public class Settings
    {
        // general
        public bool AutoFocus = false;
        public bool AutoClear = true;
        public bool HightlightTabs = true;

        // keybind
        public KeyBind Keybind = new KeyBind(70, true, false, false); // ctrl + f
        public bool KeybindOnly = true;
        public bool KeybindPassthrough = false;

        // style
        public int SearchBarWidth = 100;
        public Vector4 SearchBarBackgroundColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
        public Vector4 SearchBarTextColor = Vector4.One;

        // offsets
        public int NormalInventoryOffset = 20;
        public int LargeInventoryOffset = 0;
        public int LargestInventoryOffset = 0;
        public int ChocoboInventoryOffset = 0;
        public int RetainerInventoryOffset = 18;
        public int LargeRetainerInventoryOffset = 0;
        public int ArmouryInventoryOffset = 30;

        // filters
        public string TagSeparatorCharacter = ":";
        public string SearchTermsSeparatorCharacter = " ";

        public bool NameFilterEnabled = true;
        public bool NameFilterRequireTag = false;
        public string NameFilterTag = "name";
        public string NameFilterAbbreviatedTag = "n";

        public bool TypeFilterEnabled = true;
        public bool TypeFilterRequireTag = true;
        public string TypeFilterTag = "type";
        public string TypeFilterAbbreviatedTag = "t";

        public bool JobFilterEnabled = true;
        public bool JobFilterRequireTag = true;
        public string JobFilterTag = "job";
        public string JobFilterAbbreviatedTag = "j";

        public bool LevelFilterEnabled = true;
        public bool LevelFilterRequireTag = true;
        public string LevelFilterTag = "level";
        public string LevelFilterAbbreviatedTag = "l";

        #region load / save
        private static string JsonPath = Path.Combine(Plugin.PluginInterface.GetPluginConfigDirectory(), "Settings.json");
        public static Settings Load()
        {
            string path = JsonPath;
            Settings? settings = null;

            try
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);
                    settings = JsonConvert.DeserializeObject<Settings>(jsonString);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("Error reading settings file: " + e.Message);
            }

            if (settings == null)
            {
                settings = new Settings();
                Save(settings);
            }

            return settings;
        }

        public static void Save(Settings settings)
        {
            try
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.Objects
                };
                string jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented, serializerSettings);

                File.WriteAllText(JsonPath, jsonString);
            }
            catch (Exception e)
            {
                PluginLog.Error("Error saving settings file: " + e.Message);
            }
        }
        #endregion
    }
}
