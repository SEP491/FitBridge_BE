using FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;
using FitBridge_Application.Dtos.UserGoals;
using System;

namespace FitBridge_Application.Dtos.CustomerPurchaseds;

public class CustomerPurchasedOverallResultResponseDto
{
    public Guid CustomerPurchasedId { get; set; }

    public int TotalSessions { get; set; }

    public int CompletedSessions { get; set; }

    public int CancelledSessions { get; set; }

    public int UpcomingSessions { get; set; }

    public int AvailableSessions { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public double CompletionRate { get; set; }

    // Activity metrics
    public int TotalActivities { get; set; }

    public int TotalActivitySets { get; set; }

    public int CompletedActivitySets { get; set; }

    public double ActivityCompletionRate { get; set; }

    // Average metrics
    public double AverageSessionTimeSeconds { get; set; }

    public double AverageWeightLifted { get; set; }

    public double AverageSetsPerSession { get; set; }

    // Peak performance
    public HighestPerformanceDto? HighestPerformance { get; set; }

    // Muscle group insights
    public MuscleGroupInsightDto? MostTrainedMuscleGroup { get; set; }

    public MuscleGroupInsightDto? LeastTrainedMuscleGroup { get; set; }

    // Workout statistics
    public WorkoutStatisticsDto WorkoutStatistics { get; set; } = new();

    // User goals
    public UserGoalsDto UserGoals { get; set; } = new();
}