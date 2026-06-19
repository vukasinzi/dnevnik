using System;

namespace journal.Model
{
    public class UnosIndex
    {
        public Guid guid { get; set; }
        public DateTime datum { get; set; }
        public int raspolozenje { get; set; }
        public string naslov { get; set; } = "Nema.";
    }
}