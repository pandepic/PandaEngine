﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PandaEngine
{
    public class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Dictionary<string, string> OtherAttributes { get; set; }
    } // Setting

    public class SettingsSection
    {
        public string Name { get; set; }
        public Dictionary<string, Setting> Settings { get; set; }

        public SettingsSection()
        {
            Settings = new Dictionary<string, Setting>();
        }
    } // SettingsSection

    public static class SettingsManager
    {
        public static Dictionary<string, SettingsSection> Sections { get; set; } = new Dictionary<string, SettingsSection>();

        public static void Load(string filePath)
        {
            var stopWatch = Stopwatch.StartNew();
            var loadedCount = 0;

            Sections.Clear();

            using (var fs = AssetManager.GetFileStream(filePath))
            {
                XDocument doc = XDocument.Load(fs);
                XElement settingsRoot = doc.Element("Settings");
                List<XElement> docSections = settingsRoot.Elements("Section").ToList();

                foreach (var docSection in docSections)
                {
                    SettingsSection section = new SettingsSection
                    {
                        Name = docSection.Attribute("Name").Value
                    };

                    List<XElement> sectionSettings = docSection.Elements("Setting").ToList();

                    foreach (var sectionSetting in sectionSettings)
                    {
                        var newSetting = new Setting()
                        {
                            Name = sectionSetting.Attribute("Name").Value,
                            Value = sectionSetting.Attribute("Value").Value,
                            OtherAttributes = new Dictionary<string, string>(),
                        };

                        foreach (var att in sectionSetting.Attributes())
                        {
                            if (att.Name != "Name" && att.Name != "Value")
                                newSetting.OtherAttributes.Add(att.Name.ToString(), att.Value);
                        }

                        section.Settings.Add(sectionSetting.Attribute("Name").Value, newSetting);
                        loadedCount += 1;

                        Logging.Logger.Information("[{component}] ({section}) loaded setting {name} - {value}", "SettingsManager", section.Name, newSetting.Name, newSetting.Value);
                    } // foreach

                    Sections.Add(section.Name, section);

                } // foreach
            }

            stopWatch.Stop();
            Logging.Logger.Information("[{component}] loaded {count} settings from {path} in {time:0.00} ms.", "SettingsManager", loadedCount, filePath, stopWatch.Elapsed.TotalMilliseconds);
        } // load

        public static void Save(string filePath)
        {
            Logging.Logger.Information("[{component}] saving settings to {path}", "SettingsManager", filePath);

            XDocument doc = new XDocument();

            XElement root = new XElement("Settings");

            foreach (var section in Sections)
            {
                XElement xSection = new XElement("Section");
                xSection.SetAttributeValue("Name", section.Value.Name);

                foreach (var setting in section.Value.Settings)
                {
                    XElement xSetting = new XElement("Setting");
                    xSetting.SetAttributeValue("Name", setting.Value.Name);
                    xSetting.SetAttributeValue("Value", setting.Value.Value);

                    foreach (var kvp in setting.Value.OtherAttributes)
                        xSetting.SetAttributeValue(kvp.Key, kvp.Value);

                    xSection.Add(xSetting);
                } // foreach

                root.Add(xSection);
            } // foreach

            // root node
            doc.Add(root);

            doc.Save(filePath);
        } // Save

        public static T GetSetting<T>(string section, string name)
        {
            var setting = Sections[section].Settings[name].Value;

            return setting.ConvertTo<T>();
        } // GetSetting

        public static string UpdateSetting(string section, string name, string value)
        {
            return Sections[section].Settings[name].Value = value;
        } // UpdateSetting

        public static List<Setting> GetSettings(string section)
        {
            var settings = new List<Setting>();

            foreach (var kvp in Sections[section].Settings)
                settings.Add(kvp.Value);

            return settings;
        } // GetSettings

    } // SettingsManager
}