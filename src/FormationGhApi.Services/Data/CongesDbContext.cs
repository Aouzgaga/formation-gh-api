using FormationGhApi.Services.Domain;
using Microsoft.EntityFrameworkCore;

namespace FormationGhApi.Services.Data;

public class CongesDbContext : DbContext
{
    public CongesDbContext(DbContextOptions<CongesDbContext> options) : base(options)
    {
    }

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<CompteurConges> CompteursConges => Set<CompteurConges>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nom).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Prenom).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<CompteurConges>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => new { c.UtilisateurId, c.Annee }).IsUnique();
            entity.HasOne(c => c.Utilisateur)
                .WithMany(u => u.CompteursConges)
                .HasForeignKey(c => c.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(c => c.JoursSolde);
        });
    }
}
