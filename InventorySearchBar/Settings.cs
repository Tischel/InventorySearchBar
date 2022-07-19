using Dalamud.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

namespace InventorySearchBar
{
    public class Settings
    {
        public bool AutoFocus = false;
        public bool AutoClear = true;
        public bool HightlightTabs = true;

        public KeyBind Keybind = new KeyBind(70, true, false, false); // ctrl + f
        public bool KeybindOnly = true;
        public bool KeybindPassthrough = false;

        public int SearchBarWidth = 100;
        public Vector4 SearchBarBackgroundColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
        public Vector4 SearchBarTextColor = Vector4.One;

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
