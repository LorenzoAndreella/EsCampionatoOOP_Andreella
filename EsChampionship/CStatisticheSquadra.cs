using System;

namespace EsChampionship
{
    public class CStatisticheSquadra : IEquatable<CStatisticheSquadra>, IComparable<CStatisticheSquadra>
    {
        public string Nome { get; set; }
        public int Giocate { get; set; }
        public int Vinte { get; set; }
        public int Pareggiate { get; set; }
        public int Perse { get; set; }
        public int GolFatti { get; set; }
        public int GolSubiti { get; set; }
        public int DifferenzaGol { get { return GolFatti - GolSubiti; } }

        public int Punti { get; set; }
        
        public CStatisticheSquadra(string nome)
        {
            Nome = nome;
        }

        public void AggiungiPartita(CPartitaGirone partita)
        {
            bool casa = string.Equals(Nome.ToLower(), partita.SquadraCasa.ToLower());
            bool ospite = string.Equals(Nome.ToLower(), partita.SquadraOspite.ToLower());
            if (!casa && !ospite)
                return;
            
            int fatti = casa ? partita.GolCasa : partita.GolOspite;
            int subiti = casa ? partita.GolOspite : partita.GolCasa;

            Giocate++;
            GolFatti += fatti;
            GolSubiti += subiti;
            Punti += partita.PuntiPer(Nome);

            if (fatti > subiti) Vinte++;
            else if (fatti == subiti) Pareggiate++;
            else Perse++;
        }

        public bool Equals(CStatisticheSquadra altra)
        {
            if (altra is null) return false;
            return (string.Equals(Nome.ToLower(), altra.Nome.ToLower()));
        }

        public int CompareTo(CStatisticheSquadra altra)
        {
            int c = altra.Punti.CompareTo(Punti);
            if (c != 0) return c;
            c = altra.DifferenzaGol.CompareTo(DifferenzaGol);
            if (c != 0) return c;
            c = altra.GolFatti.CompareTo(GolFatti);
            if (c != 0) return c;
            return Nome.ToLower().CompareTo(altra.Nome.ToLower());
        }

    }

}