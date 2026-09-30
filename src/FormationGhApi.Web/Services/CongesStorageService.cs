using System.Text.Json;
using FormationGhApi.Web.Models;
using Microsoft.JSInterop;

namespace FormationGhApi.Web.Services;

public sealed class CongesStorageService(IJSRuntime jsRuntime)
{
    private const string StorageKey = "formation-gh-api.conges";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private List<Utilisateur>? utilisateurs;

    public async Task<IReadOnlyList<Utilisateur>> ObtenirUtilisateursAsync()
    {
        await ChargerAsync();
        return utilisateurs!;
    }

    public async Task<Utilisateur?> ObtenirUtilisateurAsync(int id)
    {
        await ChargerAsync();
        return utilisateurs!.SingleOrDefault(utilisateur => utilisateur.Id == id);
    }

    public async Task AjouterCongeAsync(int utilisateurId, DateOnly dateDebut, DateOnly dateFin)
    {
        await ChargerAsync();
        var utilisateur = utilisateurs!.SingleOrDefault(item => item.Id == utilisateurId)
            ?? throw new InvalidOperationException("Cet utilisateur n’existe pas.");

        var nombreJours = CompterJoursOuvres(dateDebut, dateFin);
        if (nombreJours == 0)
        {
            throw new InvalidOperationException("La période doit contenir au moins un jour ouvré.");
        }

        if (utilisateur.Conges.Any(conge => dateDebut <= conge.DateFin && dateFin >= conge.DateDebut))
        {
            throw new InvalidOperationException("Cette période chevauche un congé déjà posé.");
        }

        if (nombreJours > utilisateur.JoursSolde)
        {
            throw new InvalidOperationException("Le solde disponible est insuffisant pour cette période.");
        }

        utilisateur.Conges.Add(new Conge(Guid.NewGuid(), dateDebut, dateFin, nombreJours));
        await EnregistrerAsync();
    }

    public async Task SupprimerCongeAsync(int utilisateurId, Guid congeId)
    {
        await ChargerAsync();
        var utilisateur = utilisateurs!.SingleOrDefault(item => item.Id == utilisateurId);
        if (utilisateur?.Conges.RemoveAll(conge => conge.Id == congeId) > 0)
        {
            await EnregistrerAsync();
        }
    }

    public static int CompterJoursOuvres(DateOnly dateDebut, DateOnly dateFin)
    {
        if (dateFin < dateDebut)
        {
            return 0;
        }

        var nombreJours = 0;
        for (var date = dateDebut; date <= dateFin; date = date.AddDays(1))
        {
            if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                nombreJours++;
            }
        }

        return nombreJours;
    }

    private async Task ChargerAsync()
    {
        if (utilisateurs is not null)
        {
            return;
        }

        var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        utilisateurs = string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<List<Utilisateur>>(json, JsonOptions);

        if (utilisateurs is null)
        {
            utilisateurs =
            [
                new() { Id = 1, Nom = "Dupont", Prenom = "Jean", Email = "jean.dupont@formation.local" },
                new() { Id = 2, Nom = "Martin", Prenom = "Sophie", Email = "sophie.martin@formation.local" },
                new() { Id = 3, Nom = "Bernard", Prenom = "Luc", Email = "luc.bernard@formation.local" },
            ];
            await EnregistrerAsync();
        }
    }

    private async Task EnregistrerAsync()
    {
        var json = JsonSerializer.Serialize(utilisateurs, JsonOptions);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
