using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using CefNet.Avalonia;

namespace System.Application.UI.Views.Pages
{
    public class DebugWebViewPage : UserControl, IDisposable
    {
        readonly WebView webView;
        readonly TextBox urlTextBox;

        public DebugWebViewPage()
        {
            InitializeComponent();

            webView = this.FindControl<WebView3>("webView");
            webView.InitialUrl = "http://pan.mossimo.net:9710/c/";
            //"chrome://version";
            webView.BrowserCreated += WebView_BrowserCreated;

            urlTextBox = this.FindControl<TextBox>("urlTextBox");
            urlTextBox.KeyUp += UrlTextBox_KeyUp;
        }

        private void UrlTextBox_KeyUp(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                webView.Navigate(urlTextBox.Text);
            }
        }

        private async void WebView_BrowserCreated(object sender, EventArgs e)
        {
            var value = await webView.GetUserAgentAsync();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            Dispose();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            Dispose();
        }

        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: �ͷ��й�״̬(�йܶ���)
                    if (webView != null)
                    {
                        ((IDisposable)webView).Dispose();
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
            //GC.SuppressFinalize(this);
        }
    }
}