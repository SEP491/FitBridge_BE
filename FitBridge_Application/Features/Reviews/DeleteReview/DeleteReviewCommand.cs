using System;
using MediatR;

namespace FitBridge_Application.Features.Reviews.DeleteReview;

public class DeleteReviewCommand : IRequest<bool>
{
    public Guid ReviewId { get; set; }
}
