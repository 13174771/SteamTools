// ѹ�������ԣ�ѹ�����ѹ��������UI
using System.Application.UI.Views;
using System.Threading;
using System.Windows.Forms;
using WinFormsApplication = System.Windows.Forms.Application;

var t = new Thread(() =>
{
    WinFormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
    WinFormsApplication.EnableVisualStyles();
    WinFormsApplication.SetCompatibleTextRenderingDefault(false);
    WinFormsApplication.Run(new Form1());
});
t.SetApartmentState(ApartmentState.STA);
t.Start();