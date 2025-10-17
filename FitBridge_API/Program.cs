using FitBridge_API.Extensions;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Extensions;
using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Extensions;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using FitBridge_Infrastructure.Services.Meetings;
using FitBridge_Infrastructure.Services.Notifications;
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
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitbridge API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpLogging();
app.UseHttpsRedirection();

app.MapHub<NotificationHub>("hub/notifications");
app.MapHub<SignalingHub>("hub/signaling");
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

await app.RunAsync();