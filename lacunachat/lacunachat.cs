using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WebullApi;
using WebullApi.UI;

using System.IO;
using System.Reflection;
using lacunachat.uicommon;

namespace lacunachat
{
    public partial class lacunachat : Form, Client
    {


        uicommon.LacunaChatUi hostUI = null;

        console console = null;

        UiPage currentPage = UiPage.Login;

#if DEBUG
        UIState hotUi = null;
        FileSystemWatcher fsw = null;
        string lastShadow = "";
#endif

        public lacunachat()
        {
            InitializeComponent();

            this.Resize += Lacunachat_Resize;
            this.ResizeEnd += Lacunachat_ResizeEnd;
            this.ResizeRedraw = true;

#if DEBUG
            hotUi = new UIState(this);
            hotUi.Add(new WatcherTextbox
            {
                X = 5,
                Y = 5,
                Width = 100,
                Height = -1,
                Text = "C:/Users/gianc/source/repos/lacunachat/uimain/bin/x64/Debug/net5.0-windows/uimain.dll"
            });
            hotUi.Enabled = false;

            fsw = new FileSystemWatcher(@"C:\Users\gianc\source\repos\lacunachat\uimain\bin\x64\Debug\net5.0-windows");
            fsw.Changed += (object sender, FileSystemEventArgs e) =>
                {
                    if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(((WatcherTextbox)hotUi.Controls[0]).Text))
                    {
                        try
                        {
                            console.Host.LogLine("File " + e.FullPath + " changed");

                            Assembly uiAsm = null;
                            String path = $"{Guid.NewGuid()}.temp.asm";

                            File.Copy(e.FullPath, path);

                            try
                            {
                                uiAsm = Assembly.LoadFile(Path.GetFullPath(path));
                            }
                            catch (Exception ex) { File.Delete(path); throw new AggregateException(ex); }

                            try { File.Delete(lastShadow); } catch { }

                            lastShadow = path;



                            uicommon.LacunaChatUi newHost = uiAsm.CreateFirstInstance<uicommon.LacunaChatUi>();

                            newHost.Initialize(this, this, hostUI?.State);

                            hostUI?.Dispose();
                            hostUI = newHost;

                            hostUI.LoginUi.Enabled =
                                hostUI.MainUi.Enabled = false;

                            if (currentPage == UiPage.Login)
                            {
                                hostUI.LoginUi.Enabled = true;
                                hostUI.LoginUi.Repaint();
                            }
                            else
                            {
                                hostUI.MainUi.Enabled = true;
                                hostUI.MainUi.Repaint();
                            }
                        }
                        catch (Exception ex) 
                        {
                            console.Host.LogLine(ex.ToString(), Color.Red);
                        }
                    }
                };
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.EnableRaisingEvents = true;

            console = new console();
            console.Show();
#endif

            hostUI = new uimain.UiMain();
            hostUI.Initialize(this, this, null);

            hostUI.LoginUi.Enabled = true;
            hostUI.LoginUi.Repaint();

        }

        private void Lacunachat_ResizeEnd(object sender, EventArgs e)
        {
            hostUI?.LoginUi?.Repaint();
        }

        private void Lacunachat_Resize(object sender, EventArgs e)
        {
            hostUI?.LoginUi?.Repaint();
        }

        public void SetPage(UiPage page)
        {
            currentPage = page;
        }

    }
}
