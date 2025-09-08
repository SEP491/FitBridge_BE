using FitBridge_API.Extensions;
using FitBridge_Application.Extensions;
using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Infrastructure.Extensions;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplications(builder.Configuration);
builder.AddPresentation(builder.Configuration);
builder.Services.AddControllers();
var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllOrigin");

app.UseHttpLogging();
app.UseHttpsRedirection();

app.MapControllers();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var applicationDbContext = scope.ServiceProvider.GetRequiredService<FitBridgeDbContext>();
var identitySeeder = scope.ServiceProvider.GetRequiredService<IIdentitySeeder>();

var ShouldReseedData = app.Configuration.GetValue<bool>("ClearAndReseedData");
try
{
    if (ShouldReseedData)
    {
        logger.LogInformation("Clearing data...");
        await applicationDbContext.Database.EnsureDeletedAsync();
    }

    await applicationDbContext.Database.MigrateAsync();
    if (ShouldReseedData) await identitySeeder.SeedAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "Error happens during migrations!");
}

await app.RunAsync();