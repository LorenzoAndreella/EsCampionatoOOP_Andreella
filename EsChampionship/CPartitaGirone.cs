using System;

namespace EsChampionship
{
    public class CPartitaGirone : CPartita
    {
        public CPartitaGirone(int giornata, string squadraCasa, string squadraOspite, int golCasa, int golOspite, DateTime data) : base(giornata, squadraCasa, squadraOspite, golCasa, golOspite, data)
        {
        }
        public override int PuntiPer(string squadra)
        {
            if (!Partecipa(squadra))
                return 0;
            
            string? vincitore = CalcolaVincitore();
            
            if (vincitore is null) return 1;
            if (string.Equals(vincitore.ToLower(), squadra.ToLower()))
                return 3;
            return 0;
        }

        public override string Fase
        {
            get
            {
                return "Girone";
            }
        }
    }
}
