using System;
using FitBridge_Application.Dtos.Reviews;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Reviews.CreateReview;

public class CreateReviewCommand : IRequest<ReviewProductResponseDto>
{
    public Guid? OrderItemId { get; set; }
    public double Rating { get; set; }
    public string Content { get; set; }
    public List<IFormFile>? Images { get; set; }
}
