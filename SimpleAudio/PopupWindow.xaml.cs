using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private Player<Track> player = null;

        public PopupWindow()
        {
            InitializeComponent();
        }

        public PopupWindow(Player<Track> player)
            : this()
        {
            this.player = player;
        }
    }
}
