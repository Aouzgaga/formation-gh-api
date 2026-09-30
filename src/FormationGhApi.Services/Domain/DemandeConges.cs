namespace FormationGhApi.Services.Domain;

public class DemandeConges
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public DateOnly DateDebut { get; set; }
    public DateOnly DateFin { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    public double NombreJours => DateFin.DayNumber - DateDebut.DayNumber + 1;
}
