using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CefNet.Avalonia;
using System.Application.UI.Resx;
using System.Application.UI.Views.Controls;

namespace System.Application.UI.Views.Pages
{
    public class About_FAQPage : UserControl, IDisposable
    {
        readonly WebView webViewQA;
        bool disposedValue;

        public About_FAQPage()
        {
            InitializeComponent();

            webViewQA = this.FindControl<WebView3>(nameof(webViewQA));
            webViewQA.InitialUrl = string.Format(
                "https://steampp.net/faqBox?theme={0}&language={1}",
                CefNetApp.GetTheme(),
                R.Language);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: �ͷ��й�״̬(�йܶ���)
                    if (webViewQA != null)
                    {
                        ((IDisposable)webViewQA).Dispose();
                    }
                }

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)������ս���
                // TODO: �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}