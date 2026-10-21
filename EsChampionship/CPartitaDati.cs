using System;

namespace EsChampionship
{
    public class CPartitaDati
    {
        public string tipo { get; set; }
        public int giornata { get; set; }
        public string squadraCasa { get; set; }
        public string squadraOspite { get; set; }
        public int golCasa { get; set; }
        public int golOspite { get; set; }
        public DateTime data { get; set; }
        public int? rigoriCasa { get; set; }
        public int? rigoriOspite { get; set; }
    }
}