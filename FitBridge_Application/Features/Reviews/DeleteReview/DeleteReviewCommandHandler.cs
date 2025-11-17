using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Reviews.DeleteReview;

public class DeleteReviewCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteReviewCommand, bool>
{
    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Repository<Review>().GetByIdAsync(request.ReviewId);
        if (review == null)
        {
            throw new NotFoundException($"Review {request.ReviewId} not found");
        }
        _unitOfWork.Repository<Review>().Delete(review);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
