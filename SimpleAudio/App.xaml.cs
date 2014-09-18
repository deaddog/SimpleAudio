using System;
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
        private static readonly string applicationDataPath;
        public static string ApplicationDataPath
        {
            get { return applicationDataPath; }
        }

        static App()
        {
            var roamingPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);

            roamingPath = Path.Combine(roamingPath, "DeadDog", "SimpleAudio");
            ensurePath(roamingPath);
            applicationDataPath = roamingPath;
        }
        private static void ensurePath(string path)
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

        private string settingsPath;
        private XDocument settingsDoc;
        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.settingsPath = Path.Combine(applicationDataPath, "settings.xml");
            this.settingsDoc = File.Exists(settingsPath) ? XDocument.Load(settingsPath) : new XDocument();

            XElement settingsElement = settingsDoc.Element("settings");
            if (settingsElement == null)
                settingsDoc.Add(settingsElement = new XElement("settings"));

            this.settings = new Settings(settingsElement);

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            this.settingsDoc.Save(settingsPath);
            base.OnExit(e);
        }
    }
}
