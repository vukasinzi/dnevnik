using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace journal.Model
{
   
    public class Unos
    {
        public Guid guid { get; set; } = Guid.NewGuid();
        public DateTime datum { get; set; }
     
        public int raspolozenje { get; set; }
        public string naslov { get; set; } = "Nema.";
      
        public string desavanja { get; set; }
       
        public string misao { get; set; }

        
    }
}
