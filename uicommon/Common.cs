using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

using WebullApi.UI;

namespace lacunachat
{
    namespace uicommon
    {
        public enum UiPage
        {
            Login,
            Main
        }

        public interface Client
        {
            public void SetPage(UiPage page);
        }

        public interface LacunaChatUi : IDisposable
        {
            public UIState LoginUi { get; }
            public UIState MainUi { get; }

            public void Initialize(Form host, Client client, JObject state);
            public JObject State { get; }
        }
    }
}
