using journal.Model;
using System.Text;
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

        private string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "dnevnik");
        private static ColorScheme OsnovnaShema;
        private static ColorScheme unosiShema;
        private static ColorScheme polje;

        private TreeView unosi;
        private TextField datumField;
        private TextField raspolozenjeField;
        private TextField naslovField;
        private TextView desavanjaView;
        private TextView misaoView;
        private TextView idejeView;
        private FrameView okvir;
        private View unosiPogled;
        private View idejePogled;
        private Button unosiDugme;
        private Button idejeDugme;
        private bool idejeOtvorene;

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
                if (unosi.SelectedObject?.Tag is not UnosIndex i)
                {
                    okvir.Visible = false;
                    return;
                }
                okvir.Visible = true;
                PrikaziUnos(i.guid);
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
            UcitajIdeje();
            OcistiPolja();
        }

        private string IdejePutanja()
        {
            return Path.Combine(folder, "ideje.dnevnik");
        }

        private void OcistiPolja()
        {
            datumField.Text = string.Empty;
            raspolozenjeField.Text = string.Empty;
            desavanjaView.Text = string.Empty;
            misaoView.Text = string.Empty;
            naslovField.Text = string.Empty;
        }

        private void Novi()
        {
            if (IdejeAktivne())
                PrikaziUnose();

            Unos novi = new Unos();
            novi.guid = Guid.NewGuid();
            novi.datum = DateTime.Now;
            novi.raspolozenje = 3;
            novi.naslov = "Novi unos";
            novi.desavanja = string.Empty;
            novi.misao = string.Empty;

            string putanja = Path.Combine(folder, novi.guid + ".json");
            if (!FileManager.Instance.upisiUnos(putanja, novi))
            {
                MessageBox.ErrorQuery("Greška", "Neuspešno kreiranje unosa.", "OK");
                return;
            }

            FileManager.Instance.AzurirajIndeks(folder, novi);
            PopuniStablo();
            PrikaziUnos(novi.guid);
            SelektujUnos(novi.guid);
        }

        private void PrikaziUnos(Guid guid)
        {
            string putanja = Path.Combine(folder, guid + ".json");
            Unos izabrani = FileManager.Instance.UcitajUnosDetaljno(putanja);
            if (izabrani == null) return;

            datumField.Text = izabrani.datum.ToString("yyyy-MM-dd");
            raspolozenjeField.Text = izabrani.raspolozenje.ToString();
            naslovField.Text = izabrani.naslov ?? "";
            desavanjaView.Text = izabrani.desavanja ?? "";
            misaoView.Text = izabrani.misao ?? "";
        }

        private void Obrisi()
        {
            if (IdejeAktivne())
                return;

            if (unosi.SelectedObject?.Tag is not UnosIndex i)
                return;

            int odgovor = MessageBox.Query("Brisanje", $"Da li ste sigurni da želite da obrišete unos '{i.naslov}'?", "Da", "Ne");
            if (odgovor != 0)
                return;

            var svi = FileManager.Instance.VratiSveUnoseMeta(folder);
            svi.Sort((a, b) => b.datum.CompareTo(a.datum));

            int izabrani = svi.FindIndex(u => u.guid == i.guid);
            if (izabrani < 0 || izabrani >= svi.Count)
                return;

            Guid sledeci;
            if (izabrani != 0)
                sledeci = svi[izabrani - 1].guid;
            else if (svi.Count > 1)
                sledeci = svi[izabrani + 1].guid;
            else
                sledeci = Guid.Empty;

            string putanja = Path.Combine(folder, i.guid + ".json");
            FileManager.Instance.ObrisiUnosDetaljno(putanja);
            FileManager.Instance.ObrisiIzIndeksa(folder, i.guid);

            PopuniStablo();

            if (sledeci == Guid.Empty)
            {
                OcistiPolja();
                return;
            }

            SelektujUnos(sledeci);
            PrikaziUnos(sledeci);
            MessageBox.Query("Info", "Unos je obrisan.", "OK");
        }

        private void Sacuvaj()
        {
            if (IdejeAktivne())
            {
                SacuvajIdeje();
                return;
            }

            if (unosi.SelectedObject?.Tag is not UnosIndex i) return;

            try
            {
                Unos noviUnos = new Unos();
                noviUnos.guid = i.guid;

                if (DateTime.TryParse(datumField.Text?.ToString(), out DateTime datum))
                    noviUnos.datum = datum + DateTime.Now.TimeOfDay;
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
                FileManager.Instance.SacuvajUnosDetaljno(putanja, noviUnos);
                FileManager.Instance.AzurirajIndeks(folder, noviUnos);

                PopuniStablo();
                PrikaziUnos(i.guid);
                SelektujUnos(noviUnos.guid);

                MessageBox.Query("Info", "Unos je sačuvan.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno čuvanje unosa: {ex.Message}", "OK");
            }
        }

        private void UcitajIdeje()
        {
            try
            {
                idejeView.Text = FileManager.Instance.ucitajTekst(IdejePutanja());
                idejeView.SetNeedsDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno učitavanje ideja: {ex.Message}", "OK");
            }
        }

        private void SacuvajIdeje()
        {
            try
            {
                FileManager.Instance.sacuvajTekst(IdejePutanja(), idejeView.Text?.ToString() ?? "");
                MessageBox.Query("Info", "Ideje sačuvane!", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Greška", $"Neuspešno čuvanje ideja: {ex.Message}", "OK");
            }
        }

        private bool IdejeAktivne()
        {
            return idejeOtvorene;
        }

        private void PrikaziUnose()
        {
            if (IdejeAktivne())
                SacuvajIdeje();

            idejeOtvorene = false;
            unosiPogled.Visible = true;
            idejePogled.Visible = false;
            unosi.SetFocus();
            Application.Refresh();
        }

        private void PrikaziIdeje()
        {
            idejeOtvorene = true;
            UcitajIdeje();
            unosiPogled.Visible = false;
            idejePogled.Visible = true;
            if (!File.Exists(IdejePutanja()))
                SacuvajIdeje();
            idejeView.SetFocus();
            Application.Refresh();
        }

        private void PopuniStablo()
        {
            unosi.ClearObjects();
            var svi = FileManager.Instance.VratiSveUnoseMeta(folder);
            if (svi == null || svi.Count == 0) return;

            svi.Sort((a, b) => b.datum.CompareTo(a.datum));

            var meseci = svi.GroupBy(u => u.datum.ToString("yyyy-MM"))
                           .OrderByDescending(g => g.Key);

            foreach (var grupa in meseci)
            {
                TreeNode mesec = new TreeNode(grupa.Key);
                foreach (var u in grupa.OrderByDescending(x => x.datum))
                {
                    TreeNode node = new TreeNode($"{u.datum:dd.MM.} | R:{u.raspolozenje} | {u.naslov}");
                    node.Tag = u;
                    mesec.Children.Add(node);
                }
                unosi.AddObject(mesec);
            }

            foreach (TreeNode tn in unosi.Objects)
                unosi.Expand(tn);
        }

        private void SelektujUnos(Guid guid)
        {
            foreach (TreeNode mesec in unosi.Objects)
            {
                foreach (TreeNode node in mesec.Children)
                {
                    if (node.Tag is UnosIndex u && u.guid == guid)
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

                if (FileManager.Instance.izvrsiProveru(folder, loz.ToString(), IdejePutanja()))
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