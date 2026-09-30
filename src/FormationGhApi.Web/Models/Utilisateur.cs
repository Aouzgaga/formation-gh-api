namespace FormationGhApi.Web.Models;

public sealed class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double JoursAcquis { get; set; } = 25;
    public List<Conge> Conges { get; set; } = [];

    public double JoursPris => Conges.Sum(conge => conge.NombreJours);
    public double JoursSolde => JoursAcquis - JoursPris;
}
