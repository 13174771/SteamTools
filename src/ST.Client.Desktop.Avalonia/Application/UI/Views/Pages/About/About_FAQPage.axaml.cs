using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CefNet.Avalonia;
using System.Application.Models;

namespace System.Application.UI.Views.Pages
{
    public class About_FAQPage : UserControl, IDisposable
    {
        readonly WebView webViewQA;
        private bool disposedValue;

        public About_FAQPage()
        {
            InitializeComponent();

            webViewQA = this.FindControl<WebView3>(nameof(webViewQA));
            var theme = AppHelper.Current.Theme;
            webViewQA.InitialUrl = string.Format("https://steampp.net/faqBox?theme={0}", theme switch
            {
                AppTheme.FollowingSystem => "auto",
                _ => theme.ToString(),
            });
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
                        Avalonia.Platform.IPlatformHandle
                        Avalonia.Platform.IPlatformHandle
                           var cursor = new Avalonia.Input.Cursor(cursorType);
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