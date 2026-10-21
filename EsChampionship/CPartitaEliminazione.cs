using System;

namespace EsChampionship
{
    public class CPartitaEliminazione : CPartita
    {
        private int? rigoriCasa;
        private int? rigoriOspite;

        public CPartitaEliminazione(int giornata, string squadraCasa, string squadraOspite, int golCasa, int golOspite,
            DateTime data,
            int? rigoriCasa, int? rigoriOspite)
            : base(giornata, squadraCasa, squadraOspite, golCasa, golOspite, data)
        {
            if (golCasa == golOspite)
            {
                if (rigoriCasa is null || rigoriOspite is null)
                {
                    throw new ArgumentException("Sono necessari rigori a causa del pareggio");
                }

                if (rigoriCasa == rigoriOspite)
                {
                    throw new ArgumentException("I rigori non possono essere pari, ci deve essere un vincitore");
                }

                RigoriCasa = rigoriCasa;
                RigoriOspite = rigoriOspite;
            }
        }

        public int? RigoriCasa
        {
            get => rigoriCasa;
            set
            {
                if (value >= 0)
                {
                    rigoriCasa = value;
                }
                else
                {
                    throw new ArgumentException("Il numero di rigori non può essere negativo");
                }
            }
        }

        public int? RigoriOspite
        {
            get => rigoriOspite;
            set
            {
                if (value >= 0)
                {
                    rigoriOspite = value;
                }
                else
                {
                    throw new ArgumentException("Il numero di rigori non può essere negativo");
                }
            }
        }

        public override string Fase
        {
            get { return "Eliminazione"; }
        }

        public override string? CalcolaVincitore()
        {
            if (GolCasa != GolOspite)
            {
                return base.CalcolaVincitore();
            }

            if (RigoriCasa is null || RigoriOspite is null)
            {
                throw new ArgumentException("Errore, partita con pareggio senza rigori");
            }

            if (RigoriCasa > RigoriOspite)
            {
                return SquadraCasa;
            }

            return SquadraOspite;
        }

        public override string Descrizione()
        {
            if (RigoriCasa is null || RigoriOspite is null)
            {
                return base.Descrizione();
            }
            return base.Descrizione() + $",   (Rigori: {RigoriCasa} - {RigoriOspite}, passa {CalcolaVincitore()})";
        }

        public override int PuntiPer(string squadra)
        {
            return 0;
        }
    }
}