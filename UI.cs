using journal.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Terminal.Gui;

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

        ListView unosi;
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

        List<string> skr = new();
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

            skr = FileManager.Instance.skraceniUnosi(putanja);

            var unosiOkvir = new FrameView("Unosi")
            {
                X = 1,
                Y = 1,
                Width = 32,
                Height = Dim.Fill(),
           
            };
            unosi = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = unosiShema
            };
            if (skr != null)
                unosi.SetSource(skr);

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

            unosi.SelectedItemChanged += (args) =>
            {
                prikaziUnos(args.Item);
                
            };

            var footer = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Novi", Novi),
                new StatusItem(Key.CtrlMask | Key.S, "~CTRL+S~ Sacuvaj", Sacuvaj),
                new StatusItem(Key.CtrlMask | Key.D, "~CTRL+D~ Obrisi", Obrisi)
            })
            {
                ColorScheme = OsnovnaShema
            };
            top.Add(footer);
        }
        void Novi()
        {
            Unos novi = new Unos();
            novi.datum = DateTime.Now;
            novi.raspolozenje = 3;
            novi.desavanja = string.Empty;
            novi.misao = string.Empty;
            novi.ideje = string.Empty;

            FileManager.Instance.upisiUnos(putanja, novi); 
            skr = FileManager.Instance.skraceniUnosi(putanja);
            unosi.SetSource(skr);
            unosi.SelectedItem = 0; 
            prikaziUnos(0);
        }
        void prikaziUnos(int index)
        {
            List<Unos> svi = FileManager.Instance.ucitajUnose(putanja);
            if (svi == null || index < 0 || index >= svi.Count)
                return;

            Unos izabrani = svi[index];
            datumField.Text = izabrani.datum.ToString("yyyy-MM-dd");
            raspolozenjeField.Text = izabrani.raspolozenje.ToString();
            desavanjaView.Text = izabrani.desavanja ?? string.Empty;
            misaoView.Text = izabrani.misao ?? string.Empty;
            idejeView.Text = izabrani.ideje ?? string.Empty;
        }
        void Obrisi()
        {
            int izabrani = unosi.SelectedItem;
            List<Unos> svi = FileManager.Instance.ucitajUnose(putanja);
            if (svi == null || izabrani < 0 || izabrani >= svi.Count)
                return;
            FileManager.Instance.obrisiUnos(putanja,izabrani);
            skr = FileManager.Instance.skraceniUnosi(putanja);
            unosi.SetSource(skr);
            prikaziUnos(izabrani-1);

        }
        void Sacuvaj()
        {
            Unos noviUnos = new Unos();
            if (DateTime.TryParse(datumField.Text.ToString(), out DateTime datum))
                noviUnos.datum = datum;
            else
                noviUnos.datum = DateTime.Now;

            if (int.TryParse(raspolozenjeField.Text.ToString(), out int raspolozenje))
                noviUnos.raspolozenje = raspolozenje;
            else
                noviUnos.raspolozenje = 3;

            noviUnos.desavanja = desavanjaView.Text.ToString();
            noviUnos.misao = misaoView.Text.ToString();
            noviUnos.ideje = idejeView.Text.ToString();

            FileManager.Instance.sacuvajUnos(putanja, unosi.SelectedItem, noviUnos);
            skr = FileManager.Instance.skraceniUnosi(putanja);
            unosi.SetSource(skr);
        }
     
    }
}