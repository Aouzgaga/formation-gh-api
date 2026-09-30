using FormationGhApi.Services.Domain;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Data;

public static class DatabaseSeeder
{
    public static void Seed(CongesDbContext dbContext)
    {
        dbContext.Database.OpenConnection();
        dbContext.Database.ExecuteSqlRaw("PRAGMA journal_mode = 'DELETE';");
        dbContext.Database.EnsureCreated();

        if (dbContext.Utilisateurs.Any())
        {
            return;
        }

        var anneeCourante = DateTime.UtcNow.Year;

        var utilisateurs = new List<Utilisateur>
        {
            new() { Nom = "Dupont", Prenom = "Jean", Email = "jean.dupont@formation.local" },
            new() { Nom = "Martin", Prenom = "Sophie", Email = "sophie.martin@formation.local" },
            new() { Nom = "Bernard", Prenom = "Luc", Email = "luc.bernard@formation.local" },
        };

        dbContext.Utilisateurs.AddRange(utilisateurs);
        dbContext.SaveChanges();

        var compteurs = utilisateurs.Select(u => new CompteurConges
        {
            UtilisateurId = u.Id,
            Annee = anneeCourante,
            JoursAcquis = 25,
            JoursPris = 0,
        });

        dbContext.CompteursConges.AddRange(compteurs);
        dbContext.SaveChanges();
    }
}
