using System;
using System.Collections.Generic;
using System.Linq;

namespace EsChampionship
{
    internal class Program
    {
        public static string fileName = "partite.json";
        public static CRepositoryPartite repo = new CRepositoryPartite(fileName);

        static void Main(string[] args)
        {
            try
            {
                repo.Carica();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File non valido ({ex.Message})");
                Console.WriteLine("Creazione file vuoto");
            }

            while (true)
            {
                Console.WriteLine("\n1. Visualizza le partite in ordine naturale");
                Console.WriteLine("2. Visualizza le partite ordinate secondo uno dei criteri, a scelta dell’utente");
                Console.WriteLine(
                    "3. Inserisci una partita (gironi o eliminazione; i rigori vengono chiesti solo in caso di pareggio)");
                Console.WriteLine("4. Mostra la classifica dei gironi");
                Console.WriteLine("5. Cerca le partite di una squadra");
                Console.WriteLine("6. Elimina una partita");
                Console.WriteLine("7. Mostra il ranking storico");
                Console.WriteLine("0. Esci");

                int scelta = LeggiIntero("Inserisci la tua scelta", 0, 7);
                Console.WriteLine();

                switch (scelta)
                {
                    case 1:
                        VisualizzaOrdinate("naturale");
                        break;
                    case 2:
                        InserisciCriterio();
                        break;
                    case 3:
                        InserisciPartita();
                        break;
                    case 4:
                        MostraClassificaGironi();
                        break;
                    case 5:
                        CercaPartitaSquadra();
                        break;
                    case 6:
                        EliminaPartita();
                        break;
                    case 7:
                        MostraRanking();
                        break;
                    case 0:
                        Console.WriteLine("Uscita...");
                        return;
                    default:
                        Console.WriteLine("Scelta non valida");
                        break;
                }
            }
        }


        public static void VisualizzaOrdinate(string criterio)
        {
            List<CPartita> partite = Ordinate(criterio);
            if (partite.Count > 0)
            {
                foreach (CPartita p in partite)
                {
                    Console.WriteLine(p.Descrizione());
                }
            }
            else
            {
                Console.WriteLine("Nessuna partita trovata");
            }
        }

        public static void InserisciPartita()
        {
            Console.WriteLine("1. Girone");
            Console.WriteLine("2. Eliminazione");
            Console.WriteLine("3. Finale");

            int tipo = LeggiIntero("Scegli il tipo di partita", 1, 3);

            int giornata = LeggiIntero("Scegli la giornata", 1, 999);
            string casa = LeggiTesto("Scegli la squadra di casa");
            string ospite = LeggiTesto("Scegli la squadra ospite");
            int golCasa = LeggiIntero("Scrivi il numero di gol della squadra di casa", 0, 999);
            int golOspite = LeggiIntero("Scrivi il numero di gol della squadra ospite", 0, 999);
            DateTime data = LeggiData("Scrivi la data della partita (gg/mm/aaaa)");
            try
            {
                CPartita partita;
                if (tipo == 1)
                {
                    partita = new CPartitaGirone(giornata, casa, ospite, golCasa, golOspite, data);
                }
                else
                {
                    int? rigoriCasa = null, rigoriOspite = null;
                    if (golCasa == golOspite)
                    {
                        Console.WriteLine("Pareggio: servono i calci di rigore");
                        while (true)
                        {
                            rigoriCasa = LeggiIntero("Scrivi il numero di rigori della squadra di casa", 0, 999);
                            rigoriOspite = LeggiIntero("Scrivi il numero di rigori della squadra ospite", 0, 999);
                            if (rigoriCasa != rigoriOspite) break;
                            Console.WriteLine("I rigori devono avere un vincitore, riscrivi");
                        }
                    }

                    if (tipo == 2)
                    {
                        partita = new CPartitaEliminazione(giornata, casa, ospite, golCasa, golOspite, data, rigoriCasa,
                            rigoriOspite);
                    }
                    else
                    {
                        partita = new CPartitaFinale(giornata, casa, ospite, golCasa, golOspite, data, rigoriCasa,
                            rigoriOspite);
                    }



                }

                if (!repo.Aggiungi(partita))
                {
                    Console.WriteLine("Partita gia presente (stessa giornata e squadre)");
                    return;
                }

                repo.Salva();
                Console.WriteLine("Partita aggiunta con successo");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }

        public static void MostraClassificaGironi()
        {
            Dictionary<string, CStatisticheSquadra> classifica = new Dictionary<string, CStatisticheSquadra>(StringComparer.OrdinalIgnoreCase); // es: milano = Milano

            foreach (var partita in repo.GetElenco())
            {
                if (partita is CPartitaGirone p)
                {
                    if (!classifica.ContainsKey(p.SquadraCasa))
                    {
                        classifica.Add(p.SquadraCasa, new CStatisticheSquadra(p.SquadraCasa));
                    }

                    classifica[p.SquadraCasa].AggiungiPartita(p);


                    if (!classifica.ContainsKey(p.SquadraOspite))
                    {
                        classifica.Add(p.SquadraOspite, new CStatisticheSquadra(p.SquadraOspite));
                    }

                    classifica[p.SquadraOspite].AggiungiPartita(p);

                }
            }

            if (classifica.Count == 0)
            {
                Console.WriteLine("Non ci sono partite da analizzare");
                return;
            }

            List<CStatisticheSquadra> valori = classifica.Values.ToList();
            valori.Sort();

            Console.WriteLine(
                $"{"-",-3}{"SQUADRA",-20}{"GIOCATE",-12}{"VINTE",-12}{"PAREGGIATE",-12}{"PERSE",-12}{"GOL FATTI",-12}{"GOL SUBITI",-12}" +
                $"{"SCARTO",-12}{"PUNTI",-12}");
            for (int i = 0; i < valori.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1,-3}{valori[i].Nome,-20}{valori[i].Giocate,-12}{valori[i].Vinte,-12}{valori[i].Pareggiate,-12}{valori[i].Perse,-12}{valori[i].GolFatti,-12}{valori[i].GolSubiti,-12}" +
                    $"{valori[i].DifferenzaGol,-12}{valori[i].Punti,-12}");
            }
        }

        public static void CercaPartitaSquadra()
        {
            string nome = LeggiTesto("Inserisci il nome della squadra da cercare");
            bool trovata = false;
            foreach (var p in repo.GetElenco())
            {
                if (p.SquadraCasa.ToLower() == nome.ToLower() || p.SquadraOspite.ToLower() == nome.ToLower())
                {
                    Console.WriteLine(p.Descrizione());
                    trovata = true;
                }
            }

            if (!trovata)
            {
                Console.WriteLine("Nessuna partita trovata");
            }
        }

        public static void EliminaPartita()
        {
            List<CPartita> partite = Ordinate("naturale");
            if (partite.Count == 0)
            {
                Console.WriteLine("Non ci sono partite da eliminare");
                return;
            }
            for (int i = 0; i < partite.Count; i++)
            {
                Console.WriteLine($"{i + 1}- {partite[i].Descrizione()}");
            }
            int n = LeggiIntero("Inserisci il numero della partita da eliminare (0 annulla)", 0, partite.Count);
            if (n == 0)
            {
                Console.WriteLine("Eliminazione annullata");
                return;
            }

            if (repo.Rimuovi(partite[n - 1]))
            {
                try
                {
                    repo.Salva();
                    Console.WriteLine("Partita eliminata");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            else
            {
                Console.WriteLine("Partita non trovata");
            }
        }

        public static void MostraRanking()
        {
            Dictionary<string, int> ranking = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var p in repo.GetElenco())
            {

                if (!ranking.ContainsKey(p.SquadraCasa))
                {
                    ranking.Add(p.SquadraCasa, 0);
                }

                ranking[p.SquadraCasa] += p.PuntiPer(p.SquadraCasa);


                if (!ranking.ContainsKey(p.SquadraOspite))
                {
                    ranking.Add(p.SquadraOspite, 0);
                }

                ranking[p.SquadraOspite] += p.PuntiPer(p.SquadraOspite);

            }

            if (ranking.Count == 0)
            {
                Console.WriteLine("Non ci sono partite");
                return;
            }

            Console.WriteLine("Ranking storico (vittoria 3 punti, pareggio 1, la finale vale doppio)");

            List<KeyValuePair<string, int>> lista = new List<KeyValuePair<string, int>>(ranking);
            lista.Sort(ConfrontaPunti);

            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {lista[i].Key} - {lista[i].Value}");
            }

        }

        //utili
        public static int LeggiIntero(string mess, int min, int max)
        {
            int n;
            do
            {
                Console.WriteLine(mess);
            } while (!int.TryParse(Console.ReadLine(), out n) || n < min || n > max);

            return n;
        }

        public static string LeggiTesto(string mess)
        {
            string s;
            do
            {
                Console.WriteLine(mess);
                s = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(s));

            return s;
        }

        public static DateTime LeggiData(string mess)
        {
            DateTime data;
            do
            {
                Console.WriteLine(mess);
            } while (!DateTime.TryParse(Console.ReadLine(), out data));

            return data;
        }
        public static void InserisciCriterio()
        {
            Console.WriteLine("1. Piu gol totali");
            Console.WriteLine("2. Scarto maggiore");
            Console.WriteLine("3. Squadra di casa ordine alfabetico");
            Console.WriteLine("4. Squadra di casa ordine invertito");
            Console.WriteLine("5. Naturale (giornata, squadra casa in ordine alfabetico)");
            Console.WriteLine("6. Data (dalla meno recente)");
            int c = LeggiIntero("Inserisci criterio", 1, 6);
            string criterio;
            switch (c)
            {
                case 1:
                    criterio = "gol";
                    break;
                case 2:
                    criterio = "scarto";
                    break;
                case 3:
                    criterio = "casaAZ";
                    break;
                case 4:
                    criterio = "casaZA";
                    break;
                case 5:
                    criterio = "naturale";
                    break;
                case 6:
                    criterio = "data";
                    break;
                default:
                    Console.WriteLine("Criterio non valido");
                    return;
            }

            Console.WriteLine();
            VisualizzaOrdinate(criterio);
        }

        public static List<CPartita> Ordinate(string criterio)
        {
            List<CPartita> copia = new List<CPartita>(repo.GetElenco());
            CPartita.Criterio = criterio;
            copia.Sort();
            CPartita.Criterio = "naturale";
            return copia;
        }

        public static int ConfrontaPunti(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
        {
            int c = b.Value.CompareTo(a.Value);
            if (c != 0) return c;
            return string.Compare(a.Key.ToLower(), b.Key.ToLower());
        }

    }
}