using SimpleAudio.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAudio.Tests.Ui.PlayerUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new PlayerWindow { DataContext = PureConfig.CreateViewModel() };
            window.Topmost = true;
            window.Show();
        }
    }
}
