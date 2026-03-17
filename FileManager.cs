using journal.Model;
using System.Diagnostics;
using System.Text.Json;

namespace journal
{
    public class FileManager
    {
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

        public bool upisiUnos(string putanja, Unos unos)
        {
            try
            {
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, unos, _jsonOptions);
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
                using FileStream fs = new FileStream(fajl, FileMode.Open, FileAccess.Read);
                unosi.Add(JsonSerializer.Deserialize<Unos>(fs, _jsonOptions));
            }
            return unosi;
        }

        public Unos ucitajUnos(string putanja)
        {
            Unos u = new();
            if (!File.Exists(putanja))
                return u;
            using FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read);
            return JsonSerializer.Deserialize<Unos>(fs, _jsonOptions);
        }

        internal void sacuvajUnos(string putanja, Unos novi)
        {
            if (!File.Exists(putanja))
                return;
            try
            {
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, novi, _jsonOptions);
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