namespace EsChampionship
{
    public enum Esito
    {
        VittoriaCasa,
        Pareggio,
        VittoriaOspite
    }

    public interface IRisultato
    {
        int CalcolaGolTotali();
        int CalcolaScarto();
        Esito CalcolaEsito();
        string? CalcolaVincitore();
    }
}