using journal.Model;
using MessagePack;
using System;
using System.Collections.Generic;
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

        public void setup()
        {
            folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            putanja = Path.Combine(folder, "dnevnik.enc");

            OsnovnaShema = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.White)
            };

            unosiShema = new ColorScheme()
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Black),
                Focus = Application.Driver.MakeAttribute(Color.Black, Color.DarkGray)
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

            var win = new Window("Дневњак.")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = OsnovnaShema
            };
            top.Add(win);
            List<string> skr = FileManager.Instance.skraceniUnosi(putanja);
            
            var unosi = new ListView()
            {
                X = 0,
                Y = 0,
                Width = 30,
                Height = Dim.Fill(),
                ColorScheme = unosiShema
            };
            if (skr != null)
                unosi = new ListView(skr);

            win.Add(unosi);


            var okvir = new FrameView()
            {
                X = Pos.Right(unosi) + 1,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };
            win.Add(okvir);

            var detalji = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
             

                Text = "Datum: 2026-03-15\nMood: 5\nDesavanja:\nVibe-codeovao ovaj dnevnik. dodata enkripcija AES256. Valjda je dobro\n\nMisao:\n\nIdeje:",
                ColorScheme = polje
            };
            okvir.Add(detalji);

            var footer = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Novi", () => {  }),
                new StatusItem(Key.F2, "~F2~ Sacuvaj", () => {  }),
                new StatusItem(Key.F3, "~F3~ Obrisi", () => {  })
            })
            {
                ColorScheme = OsnovnaShema
            };
            top.Add(footer);

          
        }
      
    }
}
