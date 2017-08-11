using Autofac;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using DeadDog.Audio.Playlist;
using Newtonsoft.Json;
using SimpleAudio.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ApplicationDataPath { get; }
        private static string SettingsPath => Path.Combine(ApplicationDataPath, "settings.json");

        private IContainer _appContainer = null;

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

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register(_ => File.Exists(SettingsPath) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath)) : new Settings(new MediaSource[0]));

            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<StatusViewModel>().SingleInstance();
            builder.RegisterType<PlayerViewModel>().SingleInstance();

            builder.RegisterType<Library>().SingleInstance();
            builder.RegisterType<LibraryPlaylist>().Named<IPlaylist<Track>>("playlist").SingleInstance();
            builder.Register(_ => new QueuePlaylist<Track>(_.ResolveNamed<IPlaylist<Track>>("playlist"))).AsSelf().As<IPlaylist<Track>>().As<IPlayable<Track>>().SingleInstance();

            builder.Register(_ => new FilePlayback<Track>(new AudioControl(), x => x.FilePath)).As<IPlayback<Track>>().SingleInstance();
            builder.RegisterType<Player<Track>>().SingleInstance();

            return builder.Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _appContainer = CreateContainer();

            var main = new MainWindow()
            {
                DataContext = _appContainer.Resolve<MainViewModel>()
            };
            var popup = new PopupWindow()
            {
                DataContext = _appContainer.Resolve<StatusViewModel>()
            };

            var hotkeys = new Hotkeys.HotKeyManager(main);
            hotkeys.AddHotKey(Key.Q, ModifierKeys.Control | ModifierKeys.Alt, Shutdown);

            main.Show();
            main.Hide();

            popup.Show();
            popup.Hide();

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(_appContainer.Resolve<Settings>()));
            base.OnExit(e);
        }
    }
}
