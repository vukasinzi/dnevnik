using journal.Model;
using System.Text.RegularExpressions;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace journal
{
    public class UI
    {
        private static UI instance;
        public static UI Instance
        {
            get
            {
                if (instance == null)
                    instance = new UI();
                return instance;
            }
        }

        string folder;
        public string putanja;
        static ColorScheme OsnovnaShema;
        static ColorScheme unosiShema;
        static ColorScheme polje;

        TreeView unosi;
        TextField datumField;
        TextField raspolozenjeField;
        TextView desavanjaView;
        TextView misaoView;
        TextView idejeView;

        public void setup()
        {
            folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            putanja = Path.Combine(folder, "dnevnik.json");

            OsnovnaShema = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.White)
            };
            unosiShema = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.Black, Color.DarkGray),
                HotFocus = Application.Driver.MakeAttribute(Color.Black, Color.DarkGray)
            };
            polje = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.Black)
            };
          
        }

      // List<string> skr = new();
        public void _kostur()
        {
            var top = Application.Top;

            var win = new Window("Dневњаk.")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = OsnovnaShema
            };
            top.Add(win);

            //skr = FileManager.Instance.skraceniUnosi(putanja);

            var unosiOkvir = new FrameView("Unosi")
            {
                X = 1,
                Y = 1,
                Width = 40,
                Height = Dim.Fill(),
           
            };
            unosi = new TreeView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = unosiShema
            };
           /* if (skr != null)
                unosi.SetSource(skr);*/

            unosiOkvir.Add(unosi);
            win.Add(unosiOkvir);

            var okvir = new FrameView()
            {
                X = Pos.Right(unosiOkvir) + 1, 
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            win.Add(okvir);

            var datumLabel = new Label("Datum:")
            {
                X = 0,
                Y = 0
            };
            datumField = new TextField("")
            {
                X = Pos.Right(datumLabel) + 1,
                Y = 0,
                Width = 12,
                ColorScheme = polje
            };

            var raspolozenjeLabel = new Label("Raspolozenje:")
            {
                X = Pos.Right(datumField) + 3,
                Y = 0
            };
            raspolozenjeField = new TextField("")
            {
                X = Pos.Right(raspolozenjeLabel) + 1,
                Y = 0,
                Width = 3,
                ColorScheme = polje
            };

            okvir.Add(datumLabel, datumField, raspolozenjeLabel, raspolozenjeField);

            var desavanjaOkvir = new FrameView("Desavanja")
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Percent(45),
            };
            desavanjaView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                WordWrap = true,
                ColorScheme = polje
            };
            desavanjaOkvir.Add(desavanjaView);
            okvir.Add(desavanjaOkvir);

            var misaoOkvir = new FrameView("Misao")
            {
                X = 0,
                Y = Pos.Bottom(desavanjaOkvir),
                Width = Dim.Fill(),
                Height = Dim.Percent(25),
            };
            misaoView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                WordWrap = true,
                ColorScheme = polje
            };
            misaoOkvir.Add(misaoView);
            okvir.Add(misaoOkvir);

            var idejeOkvir = new FrameView("Ideje")
            {
                X = 0,
                Y = Pos.Bottom(misaoOkvir),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            idejeView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                WordWrap = true,
                ColorScheme = polje
            };
            idejeOkvir.Add(idejeView);
            okvir.Add(idejeOkvir);

            prikaziUnos(0);

            unosi.SelectionChanged += (s,e) =>
            {
                if (unosi.SelectedObject?.Tag is not Unos i) 
                    return;
                var svi = FileManager.Instance.ucitajUnose(putanja);
                int izabrani = svi.FindIndex(u => u.guid == i.guid);
                prikaziUnos(izabrani);
            };

            var footer = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Novi", Novi),
                new StatusItem(Key.F2, "~F2~ Sacuvaj", Sacuvaj),
                new StatusItem(Key.F5, "~F5~ Obrisi", Obrisi)
            })
            {
                ColorScheme = OsnovnaShema
            };
            top.Add(footer);
            PopuniStablo();
        }
        void ocisti()
        {
            datumField.Text = string.Empty;
            raspolozenjeField.Text = string.Empty;
            desavanjaView.Text = string.Empty;
            misaoView.Text = string.Empty;
            idejeView.Text = string.Empty;
        }
        void Novi()
        {
            Unos novi = new Unos();
            novi.guid = Guid.NewGuid();
            novi.datum = DateTime.Now;
            novi.raspolozenje = 3;
            novi.desavanja = string.Empty;
            novi.misao = string.Empty;
            novi.ideje = string.Empty;

            FileManager.Instance.upisiUnos(putanja, novi);
            PopuniStablo();
            prikaziUnos(0);
            selektujUnos(novi.guid);
        }
        void prikaziUnos(int index)
        {
            List<Unos> svi = FileManager.Instance.ucitajUnose(putanja);
            if (svi == null || index < 0 || index >= svi.Count)
                return;

            Unos izabrani = svi[index];
            datumField.Text = izabrani.datum.ToString("yyyy-MM-dd");
            raspolozenjeField.Text = izabrani.raspolozenje.ToString();
            desavanjaView.Text = izabrani.desavanja;
            misaoView.Text = izabrani.misao;
            idejeView.Text = izabrani.ideje;
        }
        void Obrisi()
        {
            List<Unos> svi = FileManager.Instance.ucitajUnose(putanja);
            if (unosi.SelectedObject?.Tag is not Unos i)
                return;
            int izabrani = svi.FindIndex(u => u.guid == i.guid);
            if (svi == null || izabrani < 0 || izabrani >= svi.Count)
                return;
            FileManager.Instance.obrisiUnos(putanja,izabrani);
            PopuniStablo();
            ocisti();
            svi = FileManager.Instance.ucitajUnose(putanja);
            if (svi.Count == 0) return;

            prikaziUnos(0);
            if (izabrani != 0)
                selektujUnos(svi[izabrani-1].guid);
            else
                selektujUnos(svi.FirstOrDefault().guid);


        }
        void Sacuvaj()
        {
            if (unosi.SelectedObject?.Tag is not Unos i) return;
            Unos noviUnos = new Unos();
            noviUnos.guid = i.guid;
            if (DateTime.TryParse(datumField.Text.ToString(), out DateTime datum))
                noviUnos.datum = datum+i.datum.TimeOfDay;
            else
                noviUnos.datum = DateTime.Now;

            if (int.TryParse(raspolozenjeField.Text.ToString(), out int raspolozenje))
                noviUnos.raspolozenje = raspolozenje;
            else
                noviUnos.raspolozenje = 3;

            noviUnos.desavanja = desavanjaView.Text.ToString();
            noviUnos.misao = misaoView.Text.ToString();
            noviUnos.ideje = idejeView.Text.ToString();
            
            var svi = FileManager.Instance.ucitajUnose(putanja);
            int izabrani = svi.FindIndex(u => u.guid == i.guid);
            FileManager.Instance.sacuvajUnos(putanja, izabrani, noviUnos);
            PopuniStablo();
            prikaziUnos(izabrani);
            selektujUnos(noviUnos.guid);
        }
       // List<string> otvoreni_nodeovi = new();

        void PopuniStablo()
        {
            //otvoreni_nodeovi = vratiOtvoreneNodeove();

            Dictionary<string, List<Unos>> meseci_godine = new();
            unosi.ClearObjects();
            var svi = FileManager.Instance.ucitajUnose(putanja);
            if (svi == null) return;

            svi.Sort((a, b) => b.datum.CompareTo(a.datum));//lambda funkcija za sort descenidng

            foreach (Unos u in svi)
            {
                string datum = u.datum.ToString("yyyy-MM");
                if (!meseci_godine.ContainsKey(datum))
                    meseci_godine[datum] = new List<Unos>();
                meseci_godine[datum].Add(u);
            }
            foreach(string kljuc in meseci_godine.Keys)
            {
                TreeNode datum = new TreeNode($"{kljuc}");
                foreach(Unos u in meseci_godine[kljuc])
                {
                    TreeNode node = new TreeNode($"{u.datum:yyyy-MM-dd} | Raspolozenje: {u.raspolozenje}");
                    node.Tag = u;
                    datum.Children.Add(node);
                }
                unosi.AddObject(datum);
            }
            
            foreach(TreeNode tn in unosi.Objects)
            {
                    unosi.Expand(tn);
            }
            

        }
        //helper funkcije
        /*private List<string> vratiOtvoreneNodeove()
        {
            List<string> otvoreni = new();
            foreach(TreeNode tn in unosi.Objects)
            {
                if (unosi.IsExpanded(tn))
                    otvoreni.Add(tn.Text);
            }
            return otvoreni;
        }*/ //realno je nepotrebno, kul al smara stalno da otvaras sve...
        void selektujUnos(Guid guid)
        {
            foreach (TreeNode mesec_godina in unosi.Objects)
            {
                foreach (TreeNode node in mesec_godina.Children)
                {
                    if (node.Tag is Unos u && u.guid == guid)
                    {
                        unosi.SelectedObject = node;
                        return;
                    }
                }
            }
        }
    }
}