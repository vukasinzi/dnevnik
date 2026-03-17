using journal.Model;
using System.Runtime.Versioning;
using Terminal.Gui;

namespace journal
{


    class Program
    {

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            UI.Instance.login();    
            Application.Init();
            UI.Instance.setup();
            UI.Instance._kostur();
            Application.Run();
        }
    }
}