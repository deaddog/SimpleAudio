using System;
using Autofac;
using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using SimpleAudio.ViewModels;
using DeadDog.Audio.Playlist;

namespace SimpleAudio
{
    public class ViewModelLocator
    {
        #region IsDesignMode property hack

        private System.Windows.DependencyObject dummy = new System.Windows.DependencyObject();
        private bool IsDesignMode => System.ComponentModel.DesignerProperties.GetIsInDesignMode(dummy);

        #endregion

        private static IContainer _container;
        private static IContainer Container => _container ?? (_container = CreateContainer());
        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<StatusViewModel>().SingleInstance();
            builder.RegisterType<SearchViewModel>().SingleInstance();
            builder.RegisterType<PlayerViewModel>().SingleInstance();

            builder.RegisterType<Library>().SingleInstance();
            builder.RegisterType<LibraryPlaylist>().Named<IPlaylist<Track>>("playlist").SingleInstance();
            builder.Register(_ => new QueuePlaylist<Track>(_.ResolveNamed<IPlaylist<Track>>("playlist"))).AsSelf().As<IPlaylist<Track>>().As<IPlayable<Track>>().SingleInstance();

            builder.Register(_ => new FilePlayback<Track>(new AudioControl(), x => x.FilePath)).As<IPlayback<Track>>().SingleInstance();
            builder.RegisterType<Player<Track>>().SingleInstance();

            return builder.Build();
        }

        public MainViewModel MainViewModel
        {
            get { return IsDesignMode ? CreateDesignMainViewModel() : Container.Resolve<MainViewModel>(); }
        }
        public StatusViewModel StatusViewModel
        {
            get { return IsDesignMode ? CreateDesignStatusViewModel() : Container.Resolve<StatusViewModel>(); }
        }

        private MainViewModel CreateDesignMainViewModel()
        {
            var vm = new MainViewModel(new SearchViewModel(), CreateDesignStatusViewModel(), null, null);

            return vm;
        }
        private StatusViewModel CreateDesignStatusViewModel()
        {
            var library = new Library();
            var playlist = new LibraryPlaylist(library);

            var track = library.AddTrack(new RawTrack(@"C:\song.mp3", "Atlas, Rise!", "Hardwired... to Self-Destruct", 2, "Metallica", 2016));

            var player = new Player<Track>(playlist, new DesignPlayback());
            var vm = new StatusViewModel(new PlayerViewModel(player));

            playlist.MoveToEntry(track);
            player.Play();

            return vm;
        }

        #region Designer classes

        private class DesignPlayback : IPlayback<Track>
        {
            public PlayerStatus Status => PlayerStatus.Playing;

            public uint Position => (1 * 60 + 14) * 1000;
            public uint Length => (5 * 60 + 36) * 1000;

            public event EventHandler StatusChanged;
            public event PositionChangedEventHandler PositionChanged;

            public bool CanOpen(Track element)
            {
                return true;
            }

            public bool Close()
            {
                return true;
            }

            public void Dispose()
            {
            }

            public void GetVolume(out double left, out double right)
            {
                throw new NotImplementedException();
            }
            public void SetVolume(double left, double right)
            {
                throw new NotImplementedException();
            }

            public bool Open(Track element)
            {
                return true;
            }

            public bool Pause()
            {
                return true;
            }

            public bool Play()
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(false));
                return true;
            }

            public bool Seek(PlayerSeekOrigin origin, uint offset)
            {
                return true;
            }

            public bool Stop()
            {
                return true;
            }
        }

        #endregion
    }
}
