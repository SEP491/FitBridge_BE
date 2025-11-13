using System;

namespace FitBridge_Application.Dtos.Reviews;

public class ReviewProductResponseDto
{
    public Guid Id { get; set; }
    public double Rating { get; set; }
    public string Content { get; set; }
    public bool IsEdited { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string? UserAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
