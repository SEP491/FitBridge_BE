using System;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Subscriptions.UpdateSubscriptionPlan;

public class UpdateSubscriptionPlanCommand : IRequest<bool> 
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal? Charge { get; set; }
    public int? Duration { get; set; }
    public string? Description { get; set; }
    public IFormFile? ImageUrl { get; set; }
}
