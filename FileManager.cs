using journal.Model;
using System.Text.Json;

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
                Enkripcija e = Enkripcija.enkriptuj(JsonSerializer.Serialize(unos), lozinka);
                using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(fs, e, _jsonOptions);
            }
            catch
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

        private string IndeksPutanja(string folder)
        {
            return Path.Combine(folder, "index.json");
        }

        private List<UnosIndex> UcitajIndeks(string folder)
        {
            string putanja = IndeksPutanja(folder);
            if (!File.Exists(putanja))
                return new List<UnosIndex>();
            using FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read);
            Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
            string dekriptovano = e.dekriptuj(lozinka);
            return JsonSerializer.Deserialize<List<UnosIndex>>(dekriptovano, _jsonOptions) ?? new List<UnosIndex>();
        }

        private void SacuvajIndeks(string folder, List<UnosIndex> indeks)
        {
            string putanja = IndeksPutanja(folder);
            Enkripcija e = Enkripcija.enkriptuj(JsonSerializer.Serialize(indeks), lozinka);
            using FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fs, e, _jsonOptions);
        }

        internal void AzurirajIndeks(string folder, Unos unos)
        {
            var indeks = UcitajIndeks(folder);
            var postojeci = indeks.FirstOrDefault(i => i.guid == unos.guid);
            if (postojeci != null)
            {
                postojeci.datum = unos.datum;
                postojeci.raspolozenje = unos.raspolozenje;
                postojeci.naslov = unos.naslov;
            }
            else
            {
                indeks.Add(new UnosIndex
                {
                    guid = unos.guid,
                    datum = unos.datum,
                    raspolozenje = unos.raspolozenje,
                    naslov = unos.naslov
                });
            }
            SacuvajIndeks(folder, indeks);
        }

        internal void ObrisiIzIndeksa(string folder, Guid guid)
        {
            var indeks = UcitajIndeks(folder);
            var zaBrisanje = indeks.FirstOrDefault(i => i.guid == guid);
            if (zaBrisanje != null)
            {
                indeks.Remove(zaBrisanje);
                SacuvajIndeks(folder, indeks);
            }
        }

        internal List<UnosIndex> VratiSveUnoseMeta(string folder)
        {
            string indeksPutanja = IndeksPutanja(folder);

            if (File.Exists(indeksPutanja))
                return UcitajIndeks(folder);

            List<UnosIndex> noviIndeks = new();
            string[] fajlovi = Directory.GetFiles(folder, "*.json");

            foreach (string fajl in fajlovi)
            {
                string ime = Path.GetFileName(fajl);
                if (ime == "index.json" || ime == "provera.json") continue;

                try
                {
                    using FileStream fs = new FileStream(fajl, FileMode.Open, FileAccess.Read);
                    Enkripcija e = JsonSerializer.Deserialize<Enkripcija>(fs, _jsonOptions);
                    if (e == null) continue;

                    string dekriptovano = e.dekriptuj(lozinka);
                    Unos unos = JsonSerializer.Deserialize<Unos>(dekriptovano, _jsonOptions);
                    if (unos == null) continue;

                    noviIndeks.Add(new UnosIndex
                    {
                        guid = unos.guid,
                        datum = unos.datum,
                        raspolozenje = unos.raspolozenje,
                        naslov = unos.naslov ?? "Nema."
                    });
                }
                catch
                {
                    continue;
                }
            }

            if (noviIndeks.Count > 0)
                SacuvajIndeks(folder, noviIndeks);

            return noviIndeks;
        }

        public Unos UcitajUnosDetaljno(string putanja) => ucitajUnos(putanja);
        public void SacuvajUnosDetaljno(string putanja, Unos unos) => sacuvajUnos(putanja, unos);
        public void ObrisiUnosDetaljno(string putanja) => obrisiUnos(putanja);
    }
}