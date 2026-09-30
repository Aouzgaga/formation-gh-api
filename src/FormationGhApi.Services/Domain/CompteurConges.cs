namespace FormationGhApi.Services.Domain;

public class CompteurConges
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public int Annee { get; set; }
    public double JoursAcquis { get; set; }
    public double JoursPris { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    public double JoursSolde => JoursAcquis - JoursPris;
}
