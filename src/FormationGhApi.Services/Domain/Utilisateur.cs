namespace FormationGhApi.Services.Domain;

public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<CompteurConges> CompteursConges { get; set; } = new List<CompteurConges>();
    public ICollection<DemandeConges> DemandesConges { get; set; } = new List<DemandeConges>();
}
