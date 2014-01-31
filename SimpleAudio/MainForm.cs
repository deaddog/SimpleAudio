using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleAudio
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.managerControl1.LocationChanged += (s, e) => HandleLocationChange();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            HandleLocationChange();
            base.OnLocationChanged(e);
        }

        private void HandleLocationChange()
        {
            var p = this.PointToScreen(new Point(0, 0));
            this.managerControl1.SetMouseOffset(-p.X, -p.Y);
        }
    }
}
