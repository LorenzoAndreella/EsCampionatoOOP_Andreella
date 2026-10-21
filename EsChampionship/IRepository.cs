using System.Collections.Generic;

namespace EsChampionship
{
    public interface IRepository<T>
    {
        IReadOnlyList<T> GetElenco();
        
        bool Aggiungi(T elemento);
        bool Rimuovi(T elemento);
        void Carica();
        void Salva();
    }
}