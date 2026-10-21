using System;

namespace EsChampionship
{
    public abstract class CPartita : IRisultato, IStampabile, IEquatable<CPartita>, IComparable<CPartita>
    {
        private int giornata;
        private string squadraCasa;
        private string squadraOspite;
        private int golCasa;
        private int golOspite;
        private DateTime data;

        public static string Criterio = "naturale";
        public CPartita(int giornata, string squadraCasa, string squadraOspite, int golCasa, int golOspite, DateTime data)
        {
            Giornata = giornata;
            SquadraCasa = squadraCasa;
            SquadraOspite = squadraOspite;
            GolCasa = golCasa;
            GolOspite = golOspite;
            Data = data;
        }

        public int Giornata
        {
            get => giornata;
            set
            {
                if (value <= 0) throw new ArgumentException("La giornata deve essere maggiore di 0");
                giornata = value;
            }
        }

        public string SquadraCasa
        {
            get => squadraCasa;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La squadra casa non può essere nulla o vuota");
                squadraCasa = value;
            }
        }

        public string SquadraOspite
        {
            get => squadraOspite;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La squadra ospite non può essere nulla o vuota");
                if (SquadraCasa.ToLower() == value.ToLower())
                    throw new ArgumentException("La squadra ospite non può essere la stessa della casa");
                squadraOspite = value;
            }
        }

        public int GolCasa
        {
            get => golCasa;
            set
            {
                if (value < 0) throw new ArgumentException("I gol non possono essere negativi");
                golCasa = value;
            }
        }

        public int GolOspite
        {
            get => golOspite;
            set
            {
                if (value < 0) throw new ArgumentException("I gol non possono essere negativi");
                golOspite = value;
            }
        }
        public DateTime Data
        {
            get => data;
            set
            {
                if (value.Year < 1900 || value > DateTime.Now) throw new ArgumentException("La data non è valida");
                data = value;
            }
        }
        public int CalcolaGolTotali()
        {
            return GolCasa + GolOspite;
        }

        public int CalcolaScarto()
        {
            return Math.Abs(GolCasa - GolOspite);
        }

        public Esito CalcolaEsito()
        {
            if (GolCasa > GolOspite) return Esito.VittoriaCasa;
            if (GolCasa < GolOspite) return Esito.VittoriaOspite;
            return Esito.Pareggio;
        }

        public virtual string? CalcolaVincitore()
        {
            if (CalcolaEsito() == Esito.Pareggio) return null;
            return CalcolaEsito() == Esito.VittoriaCasa ? SquadraCasa : SquadraOspite;
        }

        public virtual string Descrizione()
        {
            return
                $"Giornata: {giornata},     Data: {data.ToString("dd/MM/yyyy")},      Fase: {Fase},       {SquadraCasa} {GolCasa} - {GolOspite} {SquadraOspite}";
        }
        public bool Equals(CPartita altra)
        {
            if (altra is null)
                return false;

            if (Giornata != altra.Giornata)
                return false;

            if (SquadraCasa.ToLower() != altra.SquadraCasa.ToLower())
                return false;

            if (SquadraOspite.ToLower() != altra.SquadraOspite.ToLower())
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CPartita);
        }

        public int CompareTo(CPartita altra)
        {
            if (Criterio == "gol")
                return altra.CalcolaGolTotali().CompareTo(this.CalcolaGolTotali());

            if (Criterio == "scarto")
                return altra.CalcolaScarto().CompareTo(this.CalcolaScarto());

            if (Criterio == "casaAZ")
                return SquadraCasa.ToLower().CompareTo(altra.SquadraCasa.ToLower());

            if (Criterio == "casaZA")
                return altra.SquadraCasa.ToLower().CompareTo(SquadraCasa.ToLower());

            if (Criterio == "data")
                return Data.CompareTo(altra.Data);

            // "naturale"
            if (Giornata != altra.Giornata)
                return Giornata.CompareTo(altra.Giornata);

            return SquadraCasa.ToLower().CompareTo(altra.SquadraCasa.ToLower());
        }

        public static bool operator ==(CPartita a, CPartita b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(CPartita a, CPartita b)
        {
            return !(a == b);
        }

        public abstract int PuntiPer(string squadra);
        public abstract string Fase { get; }

        public bool Partecipa(string altra)
        {
            if (string.Equals(altra.ToLower(), SquadraCasa.ToLower()) ||
                string.Equals(altra.ToLower(), SquadraOspite.ToLower()))
                return true;
            return false;
        }
    }
}