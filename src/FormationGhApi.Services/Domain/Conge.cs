namespace FormationGhApi.Services.Domain;

public class Conge
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public DateOnly DateDebut { get; set; }
    public DateOnly DateFin { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    public int NombreJours => DateFin.DayNumber - DateDebut.DayNumber + 1;
}
