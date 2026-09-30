using FormationGhApi.Services.Data;
using FormationGhApi.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCongesServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CongesDbContext>();
    DatabaseSeeder.Seed(dbContext);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
