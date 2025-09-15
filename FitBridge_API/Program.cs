using FitBridge_API.Extensions;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Extensions;
using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Extensions;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (feature != null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = new BaseResponse<EmptyClass>(
                StatusCodes.Status500InternalServerError.ToString(),
                "An unexpected error occurred.",
                null);
            if (feature.Error is BusinessException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse = new BaseResponse<EmptyClass>(
                    StatusCodes.Status400BadRequest.ToString(),
                    feature.Error.Message,
                    null);
            }

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});

using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var applicationDbContext = scope.ServiceProvider.GetRequiredService<FitBridgeDbContext>();
var identitySeeder = scope.ServiceProvider.GetRequiredService<IIdentitySeeder>();

// test api
app.MapGet("/api/cats", () =>
{
    var cats = new List<string>
    {
        "Persian",
        "Maine Coon",
        "British Shorthair",
        "Ragdoll",
        "Bengal",
        "Abyssinian",
        "Birman",
        "Oriental Shorthair",
        "Manx",
        "Russian Blue",
        "American Shorthair",
        "Scottish Fold",
        "Sphynx",
        "Siamese",
        "Norwegian Forest Cat"
    };

    return Results.Ok(cats);
});

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