using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace journal.Model
{
    [MessagePackObject]
    public class Unos
    {
        [MessagePack.Key(0)]
        public DateTime datum { get; set; }
        [MessagePack.Key(1)]
        public int raspolozenje { get; set; }
        [MessagePack.Key(2)]
        public string desavanja { get; set; }
        [MessagePack.Key(3)]
        public string misao { get; set; }
        [MessagePack.Key(4)]
        public string ideje { get; set; }

        
    }
}
