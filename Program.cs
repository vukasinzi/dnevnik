using journal.Model;
using System.Runtime.Versioning;
using Terminal.Gui;

namespace journal
{


    class Program
    {

        static void Main()
        {
            Application.Init();
            UI.Instance.setup();
         
            UI.Instance._kostur();
            Application.Run();
        }
    }
}