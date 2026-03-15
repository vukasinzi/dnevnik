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
            Unos u = new Unos();
            u.datum = DateTime.Now;
            u.ideje = "napraviti kostura";
            u.desavanja = "pojeo krastavice. hopital";
            u.misao = "nebo";
            u.raspolozenje = 5;
            FileManager.Instance.upisiUnos(UI.Instance.putanja,u);
            UI.Instance._kostur();
            Application.Run();
        }
    }
}