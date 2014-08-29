using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAudio
{
    public static class TaskbarHider
    {
        public static void HideMe(Window window)
        {
            Window w = new Window(); // Create helper window
            w.Top = -100; // Location of new window is outside of visible part of screen
            w.Left = -100;
            w.Width = 1; // size of window is enough small to avoid its appearance at the beginning
            w.Height = 1;
            w.WindowStyle = WindowStyle.ToolWindow; // Set window style as ToolWindow to avoid its icon in AltTab 
            w.Show(); // We need to show window before set is as owner to our main window
            window.Owner = w; // Okey, this will result to disappear icon for main window.
            w.Hide(); // Hide helper window just in case}

            window.Closed += (s, e) => w.Close();
        }
    }
}
