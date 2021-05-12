using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WebullApi.UI;

namespace lacunachat
{
    public partial class console : Form
    {
        UIState ui;
        WatcherConsole wconsole = null;

        public console()
        {
            InitializeComponent();

            ui = new UIState(this);
            ui.Add(wconsole = new WatcherConsole
            {
                Width = -1,
                Height = this.Height
            });

            this.DoubleBuffered = true;
            this.ResizeEnd += Console_ResizeEnd;
            this.Resize += Console_Resize;
            this.ResizeRedraw = true;
        }

        private void Console_Resize(object sender, EventArgs e)
        {
            wconsole.Height = this.Height;
        }

        private void Console_ResizeEnd(object sender, EventArgs e)
        {
            wconsole.Height = this.Height;
        }

        public WatcherConsole Host => wconsole;
    }
}
