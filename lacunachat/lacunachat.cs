﻿using System;
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

namespace lacunachat
{
    public partial class lacunachat : Form
    {

        UIState hotUi = null;

        uicommon.LacunaChatUi hostUI = null;

        console console = new console();

        String currentState = "login";

        FileSystemWatcher fsw = null;
        string lastShadow = "";

        public lacunachat()
        {
            InitializeComponent();

            this.Resize += Lacunachat_Resize;
            this.ResizeEnd += Lacunachat_ResizeEnd;
            this.ResizeRedraw = true;

            hotUi = new UIState(this);
            hotUi.Add(new WatcherTextbox
            {
                X = 5,
                Y = 5,
                Width = 100,
                Height = -1,
                Text = "C:/Users/gianc/source/repos/lacunachat/uimain/bin/x64/Debug/net5.0-windows/uimain.dll"
            });

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

                            newHost.Initialize(this, hostUI?.State);

                            hostUI?.Dispose();
                            hostUI = newHost;

                            hostUI.LoginUi.Enabled =
                                hostUI.MainUi.Enabled = false;

                            if (currentState == "login")
                            {
                                hostUI.LoginUi.Enabled = true;
                                hostUI.LoginUi.Repaint();
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

            hostUI = new uimain.UiMain();
            hostUI.Initialize(this, null);

            hostUI.LoginUi.Enabled = true;
            hostUI.LoginUi.Repaint();

            console.Show();
        }

        private void Lacunachat_ResizeEnd(object sender, EventArgs e)
        {
            hostUI?.LoginUi?.Repaint();
        }

        private void Lacunachat_Resize(object sender, EventArgs e)
        {
            hostUI?.LoginUi?.Repaint();
        }
    }
}