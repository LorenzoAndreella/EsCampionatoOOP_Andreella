using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EsChampionship
{
    public class CRepositoryPartite : IRepository<CPartita>
    {
        private readonly string fileName;
        List<CPartita> partite = new List<CPartita>();

        public CRepositoryPartite(string fileName)
        {
            this.fileName = fileName;
        }

        public IReadOnlyList<CPartita> GetElenco()
        {
            return partite.AsReadOnly();
        }

        public bool Aggiungi(CPartita elemento)
        {
            if (partite.Contains(elemento))
                return false;

            partite.Add(elemento);
            return true;
        }

        public bool Rimuovi(CPartita elemento)
        {
            return partite.Remove(elemento);
        }

        public void Carica()
        {
            partite.Clear();
            if (!File.Exists(fileName))
                return;

            string json = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(json))
                return;

            List<CPartitaDati> dati = JsonSerializer.Deserialize<List<CPartitaDati>>(json);
            if (dati == null)
                return;

            foreach (CPartitaDati p in dati)
            {
                try
                {
                    CPartita partita;
                    if (p.tipo == "finale")
                        partita = new CPartitaFinale(p.giornata, p.squadraCasa, p.squadraOspite, p.golCasa, p.golOspite,
                            p.data, p.rigoriCasa, p.rigoriOspite);
                    else if (p.tipo == "eliminazione")
                        partita = new CPartitaEliminazione(p.giornata, p.squadraCasa, p.squadraOspite, p.golCasa,
                            p.golOspite,
                            p.data, p.rigoriCasa, p.rigoriOspite);
                    else
                        partita = new CPartitaGirone(p.giornata, p.squadraCasa, p.squadraOspite, p.golCasa, p.golOspite,
                            p.data);

                    Aggiungi(partita);
                }
                catch (Exception ex)
                {
                    //partita non valida
                }
            }
        }

        public void Salva()
        {
            List<CPartitaDati> dati = new List<CPartitaDati>();
            foreach (CPartita p in partite)
            {
                CPartitaDati d = new CPartitaDati();
                d.giornata = p.Giornata;
                d.squadraCasa = p.SquadraCasa;
                d.squadraOspite = p.SquadraOspite;
                d.golCasa = p.GolCasa;
                d.golOspite = p.GolOspite;
                d.data = p.Data;

                if (p is CPartitaFinale)
                {
                    d.tipo = "finale";
                }
                else if (p is CPartitaEliminazione)
                {
                    d.tipo = "eliminazione";
                }
                else
                {
                    d.tipo = "girone";
                }

                if (p is CPartitaEliminazione e)
                {
                    d.rigoriCasa = e.RigoriCasa;
                    d.rigoriOspite = e.RigoriOspite;
                }

                dati.Add(d);
            }

            File.WriteAllText(fileName, JsonSerializer.Serialize(dati));
        }
    }
}