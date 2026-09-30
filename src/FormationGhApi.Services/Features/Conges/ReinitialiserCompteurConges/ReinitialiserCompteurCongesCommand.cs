using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Mediator;

namespace FormationGhApi.Services.Features.Conges.ReinitialiserCompteurConges;

public record ReinitialiserCompteurCongesCommand(int UtilisateurId, int Annee) : IRequest<SoldeCongesDto?>;
