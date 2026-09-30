namespace FormationGhApi.Web.Models;

public sealed record Conge(Guid Id, DateOnly DateDebut, DateOnly DateFin, int NombreJours);
