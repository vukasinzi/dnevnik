using journal.Model;
using System.Runtime.Versioning;
using Terminal.Gui;
using System.Runtime.InteropServices;
namespace journal
{


    class Program
    {

     static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            UI.Instance.login();
            Application.UseSystemConsole = true;
            Application.Init();

            PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ =>
            {
                Application.MainLoop?.Invoke(() => Application.Refresh());
            });

            UI.Instance.setup();
            UI.Instance._kostur();
            Application.Run();
            Application.Shutdown();
        }
    }
}
