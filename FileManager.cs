using journal.Model;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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

        public bool upisiUnos(string putanja, Unos unos)
        {
            try
            {
                List<Unos> prethodniUnosi = ucitajUnose(putanja);
                if (prethodniUnosi == null)
                    prethodniUnosi = new();
                prethodniUnosi.Insert(0,unos);
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                MessagePackSerializer.Serialize<List<Unos>>(fs, prethodniUnosi);
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
            return MessagePackSerializer.Deserialize<List<Unos>>(fs);

            
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

    }
}
