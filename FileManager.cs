using journal.Model;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Terminal.Gui;

namespace journal
{
    public class FileManager
    {
        private string lozinka;
        private static FileManager instance;
        public static FileManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new FileManager();
                return instance;
            }
        }

        internal bool izvrsiProveru(string folder, string loz)
        {
            string[] fajlovi = Directory.GetFiles(folder, "*.json");
            if (fajlovi.Length == 0) { 
                lozinka = loz; 
                return true; 
            }
            try
            {
                using FileStream fs = new FileStream(fajlovi[0], FileMode.Open, FileAccess.Read);
                Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
                e.dekriptuj(loz);
                lozinka = loz;
                return true;
            }
            catch { return false; }
        }
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
     
        public bool upisiUnos(string putanja, Unos unos)
        {
            try
            {
                Enkripcija e = Enkripcija.enkriptuj(JsonSerializer.Serialize(unos),lozinka);
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, e, _jsonOptions);
            }
            catch (Exception x)
            {
                return false;
            }
            return true;
        }

        public List<Unos> ucitajUnose(string putanja)
        {
            List<Unos> unosi = new();
            if (!Directory.Exists(putanja))
                return unosi;
            string[] fajlovi = Directory.GetFiles(putanja, "*.json");
            foreach (string fajl in fajlovi)
            {
                if (Path.GetFileName(fajl) == "provera.json") continue;

                using FileStream fs = new FileStream(fajl, FileMode.Open, FileAccess.Read);
                Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
                string dekriptovano = e.dekriptuj(lozinka);
                try {
                    unosi.Add(JsonSerializer.Deserialize<Unos>(dekriptovano, _jsonOptions));
                }
                catch { 
                }
            }
            return unosi;
        }
   
        public Unos ucitajUnos(string putanja)
        {
            Unos u = new();
            if (!File.Exists(putanja))
                return u;
            using FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read);
            Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
            string dekriptovano = e.dekriptuj(lozinka);

            return JsonSerializer.Deserialize<Unos>(dekriptovano, _jsonOptions);
        }

        internal void sacuvajUnos(string putanja, Unos novi)
        {
            if (!File.Exists(putanja))
                return;
            try
            {
                Enkripcija e = Enkripcija.enkriptuj(JsonSerializer.Serialize(novi), lozinka);

                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, e, _jsonOptions);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }

        internal void obrisiUnos(string putanja)
        {
            if (!File.Exists(putanja)) 
                return;
            try
            {
                File.Delete(putanja);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }

      
    }
}