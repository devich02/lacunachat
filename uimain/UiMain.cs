using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebullApi.UI;

using System.Drawing;
using System.Net.Sockets;

using WebullApi;

namespace lacunachat
{
    namespace uimain
    {
        public class UiMain : uicommon.LacunaChatUi
        {
            public UIState LoginUi { get; private set; }

            public UIState MainUi { get; private set; }

            public JObject State => JObject.FromObject(InternalState);


            class StateData
            {
                public String Username { get; set; } = "";
                public String ChatHost { get; set; } = "";


                public uicommon.UiPage CurrentPage = uicommon.UiPage.Login;
                public JToken Friends { get; set; }


                public JObject Session { get; set; }
            }

            StateData InternalState = new StateData();

            WatcherLabel lblMessage = null;
            WatcherTextbox txtPassword = null;

            WatcherPanel pnlContacts = null;
            WatcherPanel pnlAddContacts = null;

            ChatServer Session = null;

            uicommon.Client client;

            private void SetupLoginPage(Form host)
            {
                LoginUi.BeforeUIPaint += BeforeUIPaint;

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



                LoginUi.Controls.Add(new WatcherTextbox
                {
                    Font = new Font("Segoe UI", 12, InternalState.Username.Length == 0 ? FontStyle.Italic : FontStyle.Regular),
                    Text = InternalState.Username.Length > 0 ? InternalState.Username : "username",
                    TextBrush = new SolidBrush(Color.FromArgb(InternalState.Username.Length == 0 ? 100 : 255, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color)),
                    Tag = InternalState.Username.Length == 0,
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
                    OnTextChanged = (c, t) => InternalState.Username = t,
                    Bindings = new List<Action<WatcherControl, WatcherControl>> {
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 25;
                        }
                    }
                });

                LoginUi.Controls.Add(txtPassword = new WatcherTextbox
                {
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
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

                LoginUi.Controls.Add(new WatcherTextbox
                {
                    Font = new Font("Segoe UI", 12, InternalState.ChatHost.Length == 0 ? FontStyle.Italic : FontStyle.Regular),
                    Text = InternalState.ChatHost.Length > 0 ? InternalState.ChatHost : "chat-host",
                    TextBrush = new SolidBrush(Color.FromArgb(InternalState.ChatHost.Length == 0 ? 100 : 255, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color)),
                    Tag = InternalState.ChatHost.Length == 0,
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
                                c.TextBrush = new SolidBrush(Color.FromArgb(255, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color));
                            }
                        }
                        else
                        {
                            if (c.Text.Length == 0)
                            {
                                c.Tag = true;
                                c.Font = new Font("Segoe UI", 12, FontStyle.Italic);
                                c.TextBrush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)(new WatcherTextbox()).TextBrush).Color));
                                c.Text = "chat-host";
                            }
                        }
                    },
                    OnTextChanged = (c, t) => {
                        InternalState.ChatHost = t;
                    },
                    Bindings = new List<Action<WatcherControl, WatcherControl>> {
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 10;
                        }
                    }
                });


                LoginUi.Controls.Add(new WatcherButton
                {
                    Font = new Font("Segoe UI", 12),
                    Text = "Login  •  Create",
                    Width = 240,
                    Height = -1,
                    Tag = false,
                    OnClicked = (b) => {

                        Session = new ChatServer(InternalState.ChatHost);
                        try
                        {
                            Session.Login(InternalState.Username, txtPassword.Text);

                            lblMessage.TextBrush = new SolidBrush(Color.LightGreen);
                            lblMessage.Text = "Logged in";

                            InternalState.CurrentPage = uicommon.UiPage.Main;
                            InternalState.Session = Session.ToJson();

                            LoginUi.Enabled = false;
                            MainUi.Enabled = true;
                            MainUi.Repaint();
                        }
                        catch
                        {
                            Session = null;
                            lblMessage.TextBrush = new SolidBrush(Color.FromArgb(255, 144, 144));
                            lblMessage.Text = "Login failed";
                        }

                        LoginUi.Repaint();
                    },
                    Bindings = new List<Action<WatcherControl, WatcherControl>> {
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 15;
                        }
                    }
                });

                LoginUi.Controls.Add(lblMessage = new WatcherLabel
                {
                    Font = new Font("Segoe UI", 12),
                    Text = "",
                    Width = 240,
                    Height = -1,
                    Bindings = new List<Action<WatcherControl, WatcherControl>> {
                        (p, c) => {
                            c.X = p.X + p.Width / 2.0f - c.Width / 2.0f;
                            c.Y = p.Y + p.Height + 20;
                        }
                    }
                });
            }

            private void SetupMainPage(Form host)
            {
                MainUi.BeforeUIPaint += BeforeUIPaint;

                MainUi.Controls.Add(new WatcherPanel
                {
                    X = 0,
                    Y = 0,
                    Width = 150,
                    Height = -1,
                    BackBrush = new SolidBrush(Color.FromArgb(20, 24, 30)),
                    BorderPen = new Pen(Color.FromArgb(8, 12, 23), 3),
                    Children = new List<WatcherControl> {
                        new WatcherLabel {
                            Text = "Contacts",
                            Font = new Font ("Consolas", 11),
                            X = 8,
                            Y = 8,
                        },
                        new WatcherButton { 
                            Width = 25,
                            Height = -1,
                            Text = "+",
                            BorderPen = new Pen (Color.FromArgb(8, 12, 23), 1),
                            BackBrush = new SolidBrush(Color.FromArgb(30, 34, 40)),
                            HighlightBrush = new SolidBrush(Color.FromArgb(40, 44, 50)),
                            Font = new Font ("Consolas", 11),
                            OnClicked = (b) => {
                                if ((pnlAddContacts.Visible = !pnlAddContacts.Visible))
                                {
                                    b.Text = "-";
                                }
                                else
                                {
                                    b.Text = "+";
                                }
                            },
                            Bindings = new List<Action<WatcherControl, WatcherControl>> { (p, c) => {
                                c.X = p.X + p.Width + 10;
                                c.Y = p.Y-2;
                                pnlAddContacts.X = c.X + c.Width + 4;
                                pnlAddContacts.Y = c.Y - 2;
                            } }
                        },
                        new WatcherPanel {
                            X = 0,
                            Height = 3,
                            Width = -1,
                            BorderPen = new Pen(Color.Transparent, 1),
                            BackBrush = new SolidBrush(Color.FromArgb(8, 12, 23)),
                            Bindings = new List<Action<WatcherControl, WatcherControl>> { (p, c) => c.Y = p.Y + p.Height + 5 }
                        },
                        (pnlContacts = new WatcherPanel {
                            X = 0,
                            Width = -1,
                            Height = -1,
                            BorderPen = Pens.Transparent,
                            BackBrush = Brushes.Transparent,
                            Bindings = new List<Action<WatcherControl, WatcherControl>> { (p, c) => c.Y = p.Y + p.Height + 5 }
                        })
                    }
                });


                MainUi.Add(pnlAddContacts = new WatcherPanel
                {
                    Visible = false,
                    Width = 130,
                    Height = 20,
                    Owner = host,
                    Children = new List<WatcherControl> {
                        new WatcherTextbox {
                            Width = 130,
                            Height = 20,
                            Font = new Font ("Consolas", 9),
                            OnPreKeyed = (b, k) => { 
                                if (k.KeyCode == Keys.Enter)
                                {
                                    pnlAddContacts.Visible = false;
                                }
                            },
                            Owner = host
                        }
                    }
                });
            }


            public void Initialize(Form host, uicommon.Client client, JObject state)
            {

                this.client = client;

                if (state != null)
                {
                    try
                    {
                        InternalState = state.ToObject<StateData>();
                        Session = ChatServer.FromJson(InternalState.Session);
                    }
                    catch 
                    {
                        InternalState.CurrentPage = uicommon.UiPage.Login;
                    }
                }


                LoginUi = new UIState(host);
                MainUi = new UIState(host);

                LoginUi.Enabled =
                    MainUi.Enabled = false;

                SetupLoginPage(host);
                SetupMainPage(host);

                client.SetPage(InternalState.CurrentPage);
            }

            private void BeforeUIPaint(object arg1, PaintEventArgs e, MouseState arg3, WindowState arg4)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }

            public void Dispose()
            {
                LoginUi.BeforeUIPaint -= BeforeUIPaint;
                MainUi.BeforeUIPaint -= BeforeUIPaint;

                LoginUi.Dispose();
                MainUi.Dispose();
            }

        }
    }
}
