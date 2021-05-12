using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebullApi.UI;

using System.Drawing;

namespace lacunachat
{
    namespace uimain
    {
        public class UiMain : uicommon.LacunaChatUi
        {
            public UIState LoginUi { get; private set; }

            public UIState MainUi { get; private set; }

            public JObject State { get; private set; }

            public void Initialize(Form host, JObject state)
            {

                LoginUi = new UIState(host);
                MainUi = new UIState(host);

                LoginUi.Enabled =
                    MainUi.Enabled = false;

                LoginUi.BeforeUIPaint += LoginUi_BeforeUIPaint;

                LoginUi.Controls.Add(new WatcherLabel
                {
                    Text = "lacuna chat",
                    Font = new Font("Segoe UI", 18),
                    TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color)),
                    Bindings = new List<Action<WatcherControl, WatcherControl>> {
                        (p, c) => {
                            c.X = host.Width / 2.0f - c.Width / 2.0f;
                            c.Y = host.Height / 5.0f - c.Height / 2.0f;
                        }
                    }
                });



                LoginUi.Controls.Add(new WatcherTextbox {
                    Font = new Font ("Segoe UI", 12, FontStyle.Italic),
                    Text = "username",
                    TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color)),
                    Tag = true,
                    Width = 240,
                    Height = -1,
                    Owner = host,
                    OnFocusChanged = (c, f) =>
                    {
                        if (f)
                        {
                            if (c.Tag != null && ((bool)c.Tag))
                            {
                                c.Text = "";
                                c.Tag = false;

                                c.Font = new Font("Segoe UI", 12);
                                c.TextBrush = new SolidBrush(Color.FromArgb(255, ((SolidBrush)c.TextBrush).Color));
                            }
                        }
                        else
                        {
                            if (c.Text.Length == 0)
                            {
                                c.Tag = true;
                                c.Font = new Font("Segoe UI", 12, FontStyle.Italic);
                                c.TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)c.TextBrush).Color));
                                c.Text = "username";
                            }
                        }
                    },
                    Bindings = new List<Action<WatcherControl, WatcherControl>> { 
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 15;
                        }
                    }
                });
                
                LoginUi.Controls.Add(new WatcherTextbox {
                    Font = new Font ("Segoe UI", 12, FontStyle.Italic),
                    Text = "password",
                    TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color)),
                    Tag = true,
                    Width = 240,
                    Height = -1,
                    Owner = host,
                    OnFocusChanged = (c, f) =>
                    {
                        if (f)
                        {
                            if (c.Tag != null && ((bool)c.Tag))
                            {
                                c.Text = "";
                                c.Tag = false;

                                c.Font = new Font("Segoe UI", 12);
                                c.TextBrush = new SolidBrush(Color.FromArgb(255, ((SolidBrush)c.TextBrush).Color));
                                c.IsPassword = true;
                            }
                        }
                        else
                        {
                            if (c.Text.Length == 0)
                            {
                                c.Tag = true;
                                c.Font = new Font("Segoe UI", 12, FontStyle.Italic);
                                c.TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)c.TextBrush).Color));
                                c.Text = "password";
                                c.IsPassword = false;
                            }
                        }
                    },
                    Bindings = new List<Action<WatcherControl, WatcherControl>> { 
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 5;
                        }
                    }
                });



            }

            private void LoginUi_BeforeUIPaint(object arg1, PaintEventArgs e, MouseState arg3, WindowState arg4)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }

            public void Dispose()
            {
                LoginUi.BeforeUIPaint -= LoginUi_BeforeUIPaint;

                LoginUi.Dispose();
                MainUi.Dispose();
            }

        }
    }
}
