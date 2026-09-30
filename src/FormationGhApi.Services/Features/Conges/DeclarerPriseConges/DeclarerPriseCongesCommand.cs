using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Conges.DeclarerPriseConges;

public record DeclarerPriseCongesCommand(int UtilisateurId, int Annee, double NombreJours) : IRequest<DeclarerPriseCongesResult>;

public enum DeclarerPriseCongesStatut
{
    Succes,
    UtilisateurOuCompteurIntrouvable,
    SoldeInsuffisant,
}

public record DeclarerPriseCongesResult(DeclarerPriseCongesStatut Statut, SoldeCongesDto? Solde);
