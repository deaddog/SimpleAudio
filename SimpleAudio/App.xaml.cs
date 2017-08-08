﻿using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ApplicationDataPath { get; }
        private static string SettingsPath => Path.Combine(ApplicationDataPath, "settings.xml");

        static App()
        {
            var roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);

            roamingPath = Path.Combine(roamingPath, "DeadDog", "SimpleAudio");
            EnsurePath(roamingPath);
            ApplicationDataPath = roamingPath;
        }
        private static void EnsurePath(string path)
        {
            string[] levels = path.Split(Path.DirectorySeparatorChar);
            var drive = new DriveInfo(levels[0]);
            if (drive.DriveType == DriveType.NoRootDirectory ||
                drive.DriveType == DriveType.Unknown)
                throw new ArgumentException("Unable to evaluate path drive; " + levels[0], "path");

            if (!drive.IsReady)
                throw new ArgumentException("Drive '" + levels[0] + "' is not ready.", "path");

            path = levels[0] + "\\";
            for (int i = 1; i < levels.Length; i++)
            {
                path = Path.Combine(path, levels[i]);
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                    dir.Create();
            }
        }

        public static App CurrentApp
        {
            get { return SimpleAudio.App.Current as App; }
        }

        private XDocument settingsDoc;
        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.settingsDoc = File.Exists(SettingsPath) ? XDocument.Load(SettingsPath) : new XDocument();

            XElement settingsElement = settingsDoc.Element("settings");
            if (settingsElement == null)
                settingsDoc.Add(settingsElement = new XElement("settings"));

            this.settings = new Settings(settingsElement);

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            this.settingsDoc.Save(SettingsPath);
            base.OnExit(e);
        }
    }
}
