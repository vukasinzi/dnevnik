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

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        internal bool izvrsiProveru(string folder, string loz, string idejePutanja)
        {
            string[] fajlovi = Directory.GetFiles(folder, "*.json");
            string fajl = string.Empty;

            foreach (string f in fajlovi)
            {
                fajl = f;
                break;
            }

            if (fajl == string.Empty && File.Exists(idejePutanja))
                fajl = idejePutanja;

            if (fajl == string.Empty)
            {
                lozinka = loz;
                return true;
            }

            try
            {
                using FileStream fs = new FileStream(fajl, FileMode.Open, FileAccess.Read);
                Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
                if (e == null) return false;
                e.dekriptuj(loz);
                lozinka = loz;
                return true;
            }
            catch { return false; }
        }
     
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

        internal void sacuvajTekst(string putanja, string tekst)
        {
            string folder = Path.GetDirectoryName(putanja) ?? ".";
            Directory.CreateDirectory(folder);
            Enkripcija e = Enkripcija.enkriptuj(tekst, lozinka);
            using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fs, e, _jsonOptions);
        }

        internal string ucitajTekst(string putanja)
        {
            if (!File.Exists(putanja))
                return string.Empty;
            using FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read);
            Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
            if (e == null) return string.Empty;
            return e.dekriptuj(lozinka);
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
                unosi.Add(JsonSerializer.Deserialize<Unos>(dekriptovano, _jsonOptions));
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
            Enkripcija e = Enkripcija.enkriptuj(JsonSerializer.Serialize(novi), lozinka);
            using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fs, e, _jsonOptions);
        }

        internal void obrisiUnos(string putanja)
        {
            if (!File.Exists(putanja)) 
                return;
            File.Delete(putanja);
        }
    }
}
