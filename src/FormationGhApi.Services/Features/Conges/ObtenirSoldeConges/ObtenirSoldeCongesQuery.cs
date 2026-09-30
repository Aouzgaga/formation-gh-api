using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Conges.ObtenirSoldeConges;

public record ObtenirSoldeCongesQuery(int UtilisateurId, int Annee) : IRequest<SoldeCongesDto?>;
