using FormationGhApi.Services.Data;
using FormationGhApi.Services.Dtos;
using FormationGhApi.Services.Features.Conges.CreerConge;
using FormationGhApi.Services.Features.Conges.DeclarerPriseConges;
using FormationGhApi.Services.Features.Conges.ObtenirSoldeConges;
using FormationGhApi.Services.Features.Conges.ReinitialiserCompteurConges;
using FormationGhApi.Services.Features.Utilisateurs.ObtenirListeUtilisateurs;
using FormationGhApi.Services.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormationGhApi.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCongesServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CongesDatabase") ?? "Data Source=conges.db";

        services.AddDbContext<CongesDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IMediator, Mediator.Mediator>();

        services.AddScoped<IRequestHandler<ObtenirSoldeCongesQuery, SoldeCongesDto?>, ObtenirSoldeCongesHandler>();
        services.AddScoped<IRequestHandler<DeclarerPriseCongesCommand, DeclarerPriseCongesResult>, DeclarerPriseCongesHandler>();
        services.AddScoped<IRequestHandler<ReinitialiserCompteurCongesCommand, SoldeCongesDto?>, ReinitialiserCompteurCongesHandler>();
        services.AddScoped<IRequestHandler<ObtenirListeUtilisateursQuery, IReadOnlyList<UtilisateurDto>>, ObtenirListeUtilisateursHandler>();
        services.AddScoped<IRequestHandler<CreerCongeCommand, CreerCongeResult>, CreerCongeHandler>();

        return services;
    }
}
