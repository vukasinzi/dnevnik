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
                List<Unos> prethodniUnosi = ucitajUnose(putanja);
                if (prethodniUnosi == null)
                    prethodniUnosi = new();
                prethodniUnosi.Insert(0,unos);
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, prethodniUnosi,_jsonOptions);
            }
            catch (Exception x)
            {
                return false;
            }
            return true;
        }
        public List<Unos> ucitajUnose(string putanja)
        {
            if (!File.Exists(putanja))
                return null;
            using FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read);
            return JsonSerializer.Deserialize<List<Unos>>(fs,_jsonOptions);

            
        }
        public List<string> skraceniUnosi(string putanja)
        {
            List<string> skr = new();
            List<Unos> unosi = ucitajUnose(putanja);
            if (unosi == null)
                return null;
            foreach (Unos u in unosi)
            {
                skr.Add($"{u.datum:yyyy-MM-dd} | Raspolozenje: {u.raspolozenje}");
            }
            return skr;

        }

       
     
        internal void sacuvajUnos(string putanja, int index,Unos novi)
        {
            List<Unos> sviUnosi = FileManager.Instance.ucitajUnose(putanja);
            if (sviUnosi == null || index > sviUnosi.Count)
                return;
            if (sviUnosi.Count != 0)
                sviUnosi[index] = novi;
            else
                sviUnosi.Insert(0, novi);
          
            try
            {
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, sviUnosi, _jsonOptions);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
           
        }

        internal void obrisiUnos(string putanja, int index)
        {
            List<Unos> sviUnosi = FileManager.Instance.ucitajUnose(putanja);
            if (sviUnosi == null || index >= sviUnosi.Count)
                return;
            sviUnosi.RemoveAt(index);
            try
            {
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, sviUnosi, _jsonOptions);
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
    }
}
