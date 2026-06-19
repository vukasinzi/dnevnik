using journal.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "dnevnik");
        static ColorScheme OsnovnaShema;
        static ColorScheme unosiShema;
        static ColorScheme polje;

        TreeView unosi;
        TextField datumField;
        TextField raspolozenjeField;
        TextField naslovField;
        TextView desavanjaView;
        TextView misaoView;
        TextView idejeView;
        FrameView okvir;
        View unosiPogled;
        View idejePogled;
        Button unosiDugme;
        Button idejeDugme;
        bool idejeOtvorene;

        public void setup()
        {
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

            unosiDugme = new Button("Unosi")
            {
                X = 1,
                Y = 0,
                ColorScheme = OsnovnaShema
            };
            idejeDugme = new Button("Ideje")
            {
                X = Pos.Right(unosiDugme) + 2,
                Y = 0,
                ColorScheme = OsnovnaShema
            };
            unosiDugme.Clicked += PrikaziUnose;
            idejeDugme.Clicked += PrikaziIdeje;
            win.Add(unosiDugme, idejeDugme);

            Application.RootKeyEvent = (e) =>
            {
                if (e.Key == Key.F2)
                {
                    Sacuvaj();
                    return true;
                }
                return false;
            };

            unosiPogled = new View()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = OsnovnaShema
            };
            idejePogled = new View()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = OsnovnaShema,
                Visible = false
            };
            win.Add(unosiPogled, idejePogled);

            var unosiOkvir = new FrameView("Unosi")
            {
                X = 1,
                Y = 1,
                Width = 45,
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

            unosiOkvir.Add(unosi);
            unosiPogled.Add(unosiOkvir);

            okvir = new FrameView()
            {
                X = Pos.Right(unosiOkvir) + 1,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Visible = false
            };
            unosiPogled.Add(okvir);

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

            var raspolozenjeLabel = new Label("R:")
            {
                X = Pos.Right(datumField) + 2,
                Y = 0
            };
            raspolozenjeField = new TextField("")
            {
                X = Pos.Right(raspolozenjeLabel) + 1,
                Y = 0,
                Width = 2,
                ColorScheme = polje
            };

            var naslovLabel = new Label("Naslov:")
            {
                X = Pos.Right(raspolozenjeField) + 2,
                Y = 0
            };
            naslovField = new TextField("")
            {
                X = Pos.Right(naslovLabel) + 1,
                Y = 0,
                Width = Dim.Fill(),
                ColorScheme = polje
            };

            okvir.Add(datumLabel, datumField, raspolozenjeLabel, raspolozenjeField, naslovLabel, naslovField);
            var desavanjaOkvir = new FrameView("Desavanja")
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Percent(55),
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
                Height = Dim.Fill(),
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
                X = 1,
                Y = 1,
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
            idejePogled.Add(idejeOkvir);

            unosi.SelectionChanged += (s, e) =>
            {
                if (unosi.SelectedObject?.Tag is not Unos i)
                {
                    okvir.Visible = false;
                    return;
                }
                okvir.Visible = true;
                prikaziUnos(i.guid);
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
            ucitajIdeje();
            ocisti();
        }

        string idejePutanja()
        {
            return Path.Combine(folder, "ideje.dnevnik");
        }

        void ocisti()
        {
            datumField.Text = string.Empty;
            raspolozenjeField.Text = string.Empty;
            desavanjaView.Text = string.Empty;
            misaoView.Text = string.Empty;
            naslovField.Text = string.Empty;
        }

        void Novi()
        {
            if (idejeAktivne())
                PrikaziUnose();

            Unos novi = new Unos();
            novi.guid = Guid.NewGuid();
            novi.datum = DateTime.Now;
            novi.raspolozenje = 3;
            novi.naslov = "Novi unos";
            novi.desavanja = string.Empty;
            novi.misao = string.Empty;
            string putanja = Path.Combine(folder, novi.guid + ".json");
            FileManager.Instance.upisiUnos(putanja, novi);
            PopuniStablo();
            prikaziUnos(novi.guid);
            selektujUnos(novi.guid);
        }

        void prikaziUnos(Guid guid)
        {
            Unos izabrani = FileManager.Instance.ucitajUnos(Path.Combine(folder, guid + ".json"));
            if (izabrani == null) return;

            datumField.Text = izabrani.datum.ToString("yyyy-MM-dd");
            raspolozenjeField.Text = izabrani.raspolozenje.ToString();
            naslovField.Text = izabrani.naslov ?? "";
            desavanjaView.Text = izabrani.desavanja ?? "";
            misaoView.Text = izabrani.misao ?? "";
        }

        void Obrisi()
        {
            if (idejeAktivne())
                return;

            if (unosi.SelectedObject?.Tag is not Unos i)
                return;

            // Potvrda brisanja
            int odgovor = MessageBox.Query("Brisanje", $"Da li ste sigurni da želite da obrišete unos '{i.naslov}'?", "Da", "Ne");
            if (odgovor != 0) // 0 je "Da"
                return;

            List<Unos> svi = FileManager.Instance.ucitajUnose(folder);
            svi.Sort((a, b) => b.datum.CompareTo(a.datum));
            int izabrani = svi.FindIndex(u => u.guid == i.guid);
            if (svi == null || izabrani < 0 || izabrani >= svi.Count)
                return;

            Guid sledeci;
            if (izabrani != 0)
                sledeci = svi[izabrani - 1].guid;
            else if (svi.Count > 1)
                sledeci = svi[izabrani + 1].guid;
            else
                sledeci = Guid.Empty;

            string putanja = Path.Combine(folder, i.guid + ".json");
            FileManager.Instance.obrisiUnos(putanja);
            PopuniStablo();

            if (sledeci == Guid.Empty)
            {
                ocisti();
                return;
            }
            selektujUnos(sledeci);
            prikaziUnos(sledeci);

            MessageBox.Query("Info", "Unos je obrisan.", "OK");
        }

        void Sacuvaj()
        {
            if (idejeAktivne())
            {
                SacuvajIdeje();
                return;
            }

            if (unosi.SelectedObject?.Tag is not Unos i) return;

            try
            {
                Unos noviUnos = new Unos();
                noviUnos.guid = i.guid;

                if (DateTime.TryParse(datumField.Text?.ToString(), out DateTime datum))
                    noviUnos.datum = datum + i.datum.TimeOfDay;
                else
                    noviUnos.datum = DateTime.Now;

                if (int.TryParse(raspolozenjeField.Text?.ToString(), out int raspolozenje))
                    noviUnos.raspolozenje = raspolozenje;
                else
                    noviUnos.raspolozenje = 3;

                noviUnos.naslov = naslovField.Text?.ToString() ?? "";
                noviUnos.desavanja = desavanjaView.Text?.ToString() ?? "";
                noviUnos.misao = misaoView.Text?.ToString() ?? "";

                string putanja = Path.Combine(folder, i.guid + ".json");
                FileManager.Instance.sacuvajUnos(putanja, noviUnos);
                PopuniStablo();
                prikaziUnos(i.guid);
                selektujUnos(noviUnos.guid);

                MessageBox.Query("Info", "Unos je sačuvan.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno čuvanje unosa: {ex.Message}", "OK");
            }
        }

        void ucitajIdeje()
        {
            try
            {
                idejeView.Text = FileManager.Instance.ucitajTekst(idejePutanja());
                idejeView.SetNeedsDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno učitavanje ideja: {ex.Message}", "OK");
            }
        }

        void SacuvajIdeje()
        {
            try
            {
                FileManager.Instance.sacuvajTekst(idejePutanja(), idejeView.Text?.ToString() ?? "");
                MessageBox.Query("Info", "Ideje sačuvane!", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno čuvanje ideja: {ex.Message}", "OK");
            }
        }

        bool idejeAktivne()
        {
            return idejeOtvorene;
        }

        void PrikaziUnose()
        {
            if (idejeAktivne())
                SacuvajIdeje();

            idejeOtvorene = false;
            unosiPogled.Visible = true;
            idejePogled.Visible = false;
            unosi.SetFocus();
            Application.Refresh();
        }

        void PrikaziIdeje()
        {
            idejeOtvorene = true;
            ucitajIdeje();
            unosiPogled.Visible = false;
            idejePogled.Visible = true;
            if (!File.Exists(idejePutanja()))
                SacuvajIdeje();
            idejeView.SetFocus();
            Application.Refresh();
        }

        void PopuniStablo()
        {
            Dictionary<string, List<Unos>> meseci_godine = new();
            unosi.ClearObjects();
            var svi = FileManager.Instance.ucitajUnose(folder);
            if (svi == null) return;

            svi.Sort((a, b) => b.datum.CompareTo(a.datum));

            foreach (Unos u in svi)
            {
                string datum = u.datum.ToString("yyyy-MM");
                if (!meseci_godine.ContainsKey(datum))
                    meseci_godine[datum] = new List<Unos>();
                meseci_godine[datum].Add(u);
            }
            foreach (string kljuc in meseci_godine.Keys)
            {
                TreeNode datum = new TreeNode($"{kljuc}");
                foreach (Unos u in meseci_godine[kljuc])
                {
                    TreeNode node = new TreeNode($"{u.datum:dd.MM.} | R:{u.raspolozenje} | {u.naslov}");
                    node.Tag = u;
                    datum.Children.Add(node);
                }
                unosi.AddObject(datum);
            }

            foreach (TreeNode tn in unosi.Objects)
            {
                unosi.Expand(tn);
            }
        }

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

        public bool login()
        {
            Directory.CreateDirectory(folder);
            bool ulogovan = false;

            while (!ulogovan)
            {
                Console.Clear();
                Console.Write("\r\n\r\n  dневњak - vukasin zivaljevic / v1.0\r\n  ───────────────────────────────────────\r\n\r\n  lozinka: ");

                ConsoleKeyInfo key;
                StringBuilder loz = new();
                do
                {
                    key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                    {
                        loz.Append(key.KeyChar);
                        Console.Write("*");
                    }
                    else if (key.Key == ConsoleKey.Backspace && loz.Length > 0)
                    {
                        loz.Length--;
                        Console.Write("\b \b");
                    }
                } while (key.Key != ConsoleKey.Enter);

                if (FileManager.Instance.izvrsiProveru(folder, loz.ToString(), idejePutanja()))
                    ulogovan = true;
                else
                {
                    Console.WriteLine("\r\n  pogresna lozinka.");
                    Thread.Sleep(1000);
                }
            }

            return true;
        }
    }
}