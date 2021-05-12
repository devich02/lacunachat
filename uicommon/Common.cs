using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

using WebullApi.UI;

namespace lacunachat
{
    namespace uicommon
    {
        public interface LacunaChatUi : IDisposable
        {
            public UIState LoginUi { get; }
            public UIState MainUi { get; }

            public void Initialize(Form host, JObject state);
            public JObject State { get; }
        }
    }
}
