﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BabyPuncher.OriginGameLauncher.UI
{
    public partial class Settings
    {
        private KeyValuePair<string, string> gameKeyValuePair;
        private KeyValuePair<string, string> gameIdKeyValuePair;
        private KeyValuePair<string, string> gameProcessExeKeyValuePair;
        private KeyValuePair<string, string> silentKeyValuePair;

        private static readonly string settingSection = "BabyPuncher.OriginGameLauncher.UI.Properties.Settings";
        
        
        public static Settings GetSettings()
        {
            return new Settings()
            {
                Game = Properties.Settings.Default.Game,
                GameId = Properties.Settings.Default.GameId,
                GameProcessExe = Properties.Settings.Default.GameProcessExe.Trim(),
                Silent = Properties.Settings.Default.Silent
            };
        }

        public void Save()
        {
            saveSettings(settingSection, SettingsDictionary);
        }

        private static void saveSettings(string settingSection, IDictionary<string, string> settings)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var group = config.SectionGroups[@"userSettings"];

            if (group == null) return;

            var clientSection = group.Sections[settingSection] as ClientSettingsSection;
            if (clientSection == null) return;

            var settingElementsToRemove = new List<SettingElement>();
            var settingElementsToAdd = new List<SettingElement>();

            foreach (SettingElement settingElement in clientSection.Settings)
            {
                var settingKey = settings.Where(x => x.Key == settingElement.Name).First().Value;
                settingElementsToRemove.Add(settingElement);
                settingElement.Value.ValueXml.InnerText = settingKey;
                settingElementsToAdd.Add(settingElement);
            }

            settingElementsToRemove
                .ForEach(delegate (SettingElement setting)
                {
                    clientSection.Settings.Remove(setting);
                });

            settingElementsToAdd
                .ForEach(delegate(SettingElement setting)
                {
                    clientSection.Settings.Add(setting);
                });

            config.Save(ConfigurationSaveMode.Full);
        }
    }
}
