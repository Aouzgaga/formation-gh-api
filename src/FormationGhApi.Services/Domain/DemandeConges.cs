namespace FormationGhApi.Services.Domain;

public class DemandeConges
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public DateOnly DateDebut { get; set; }
    public DateOnly DateFin { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    /// <summary>
    /// Nombre de jours calendaires (samedis, dimanches et jours fériés inclus) entre DateDebut et DateFin, bornes incluses.
    /// </summary>
    public int NombreJours => DateFin.DayNumber - DateDebut.DayNumber + 1;
}
