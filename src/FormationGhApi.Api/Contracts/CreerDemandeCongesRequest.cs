namespace FormationGhApi.Api.Contracts;

public record CreerDemandeCongesRequest(string NomUtilisateur, DateOnly DateDebut, DateOnly DateFin);
