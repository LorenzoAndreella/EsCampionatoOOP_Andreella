using System;

namespace EsChampionship
{
    public class CPartitaFinale : CPartitaEliminazione
    {
        const int moltiplicatore = 2;
        public CPartitaFinale(int giornata, string squadraCasa, string squadraOspite, int golCasa, int golOspite,
            DateTime data,
            int? rigoriCasa, int? rigoriOspite)
            : base(giornata, squadraCasa, squadraOspite, golCasa, golOspite, data, rigoriCasa, rigoriOspite)
        {
        }
        public override string Fase
        {
            get { return "Finale"; }
        }
        
        public override int PuntiPer(string squadra)
        {
            if (!Partecipa(squadra))
                return 0;
            
            string? vincitore = CalcolaVincitore();

            if (vincitore is null)
                return 0;

            if (string.Equals(vincitore.ToLower(), squadra.ToLower()))
                return 3*moltiplicatore;
            return 0;
        }
        
    }
}