using System;
using System.Linq;
using FitBridge_Application.Dtos.TrainingResults;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.TrainingResults.GetPackageTrainingResults;

public class GetPackageTrainingResultsQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<GetPackageTrainingResultsQuery, CustomerPurchasedAnalyticsDto>
{
    public async Task<CustomerPurchasedAnalyticsDto> Handle(
        GetPackageTrainingResultsQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetCustomerPurchasedByIdSpec(
            request.CustomerPurchasedId,
            isIncludeBooking: true,
            isIncludeActivitySets: true,
            isIncludeSessionActivities: true);
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>()
            .GetBySpecificationAsync(spec)
            ?? throw new NotFoundException(nameof(ApplicationUser));

        var bookings = customerPurchased.Bookings.ToList();
        var completedBookings = bookings.Where(b => b.SessionStatus == SessionStatus.Finished).ToList();
        var completedSessions = completedBookings.Count;
        var cancelledSessions = bookings.Count(b => b.SessionStatus == SessionStatus.Cancelled);
        var upcomingSessions = bookings.Count(b => b.SessionStatus == SessionStatus.Booked);

        // Get all activities and sets
        var allActivities = bookings
            .SelectMany(b => b.SessionActivities)
            .ToList();

        var allActivitySets = allActivities
            .SelectMany(a => a.ActivitySets)
            .ToList();

        var completedSets = allActivitySets.Count(s => s.IsCompleted);

        // Calculate workout statistics
        var workoutStats = new WorkoutStatisticsDto
        {
            TotalWeightLifted = allActivitySets
                .Where(s => s.IsCompleted && s.WeightLifted.HasValue)
                .Sum(s => s.WeightLifted.Value),
            PlannedNumOfReps = allActivitySets
                .Where(s => s.IsCompleted && s.PlannedNumOfReps.HasValue)
                .Sum(s => s.PlannedNumOfReps.Value),
            TotalRepsCompleted = allActivitySets
                .Where(s => s.IsCompleted && s.NumOfReps.HasValue)
                .Sum(s => s.NumOfReps.Value),
            PlannedPracticeTime = allActivitySets
                .Where(s => s.IsCompleted && s.PlannedPracticeTime.HasValue)
                .Sum(s => s.PlannedPracticeTime.Value),
            TotalPracticeTimeSeconds = allActivitySets
                .Where(s => s.IsCompleted && s.PracticeTime.HasValue)
                .Sum(s => s.PracticeTime.Value),
            AverageRestTimeSeconds = allActivitySets
                .Where(s => s.RestTime.HasValue)
                .Any()
                ? (int)allActivitySets.Where(s => s.RestTime.HasValue).Average(s => s.RestTime!.Value)
                : 0,
            ActivityTypeBreakdown = allActivities
                .GroupBy(a => a.ActivityType.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Calculate average metrics
        var averageSessionTimeSeconds = completedSessions > 0
            ? workoutStats.TotalPracticeTimeSeconds / completedSessions
            : 0;

        var averageWeightLifted = completedSessions > 0
            ? Math.Round(workoutStats.TotalWeightLifted / completedSessions, 1)
            : 0;

        var averageSetsPerSession = completedSessions > 0
            ? Math.Round((double)completedSets / completedSessions, 1)
            : 0;

        // Calculate highest performance (session with most weight lifted)
        HighestPerformanceDto? highestPerformance = null;
        var sessionPerformances = completedBookings
            .Select(b => new
            {
                Booking = b,
                TotalWeight = b.SessionActivities
                    .SelectMany(a => a.ActivitySets)
                    .Where(s => s.IsCompleted && s.WeightLifted.HasValue)
                    .Sum(s => s.WeightLifted!.Value)
            })
            .Where(x => x.TotalWeight > 0)
            .OrderByDescending(x => x.TotalWeight)
            .FirstOrDefault();

        if (sessionPerformances != null)
        {
            highestPerformance = new HighestPerformanceDto
            {
                TotalWeight = sessionPerformances.TotalWeight,
                Date = sessionPerformances.Booking.BookingDate,
                SessionName = sessionPerformances.Booking.BookingName
            };
        }

        // Muscle group breakdown
        var muscleGroupBreakdown = allActivities
            .SelectMany(a => a.MuscleGroups.Select(mg => new { Activity = a, MuscleGroup = mg.ToString() }))
            .GroupBy(x => x.MuscleGroup)
            .Select(g => new MuscleGroupActivityDto
            {
                MuscleGroup = g.Key,
                ActivityCount = g.Count(),
                SetsCompleted = g.SelectMany(x => x.Activity.ActivitySets)
                    .Count(s => s.IsCompleted),
                TotalWeight = g.SelectMany(x => x.Activity.ActivitySets)
                    .Where(s => s.IsCompleted && s.WeightLifted.HasValue)
                    .Sum(s => s.WeightLifted!.Value),
                TotalReps = g.SelectMany(x => x.Activity.ActivitySets)
                    .Where(s => s.IsCompleted && s.NumOfReps.HasValue)
                    .Sum(s => s.NumOfReps!.Value)
            })
            .OrderByDescending(m => m.SetsCompleted)
            .ToList();

        // Most and least trained muscle groups
        MuscleGroupInsightDto? mostTrainedMuscleGroup = null;
        MuscleGroupInsightDto? leastTrainedMuscleGroup = null;

        if (muscleGroupBreakdown.Any())
        {
            var mostTrained = muscleGroupBreakdown.First();
            mostTrainedMuscleGroup = new MuscleGroupInsightDto
            {
                MuscleGroup = mostTrained.MuscleGroup,
                TotalSets = mostTrained.SetsCompleted,
                TotalWeight = mostTrained.TotalWeight,
                ActivityCount = mostTrained.ActivityCount
            };

            var leastTrained = muscleGroupBreakdown.Last();
            leastTrainedMuscleGroup = new MuscleGroupInsightDto
            {
                MuscleGroup = leastTrained.MuscleGroup,
                TotalSets = leastTrained.SetsCompleted,
                TotalWeight = leastTrained.TotalWeight,
                ActivityCount = leastTrained.ActivityCount
            };
        }

        return new CustomerPurchasedAnalyticsDto
        {
            CustomerPurchasedId = customerPurchased.Id,
            TotalSessions = bookings.Count,
            CompletedSessions = completedSessions,
            CancelledSessions = cancelledSessions,
            UpcomingSessions = upcomingSessions,
            AvailableSessions = customerPurchased.AvailableSessions,
            ExpirationDate = customerPurchased.ExpirationDate,
            CompletionRate = bookings.Count > 0
                ? Math.Round((double)completedSessions / bookings.Count * 100, 2)
                : 0,
            TotalActivities = allActivities.Count,
            TotalActivitySets = allActivitySets.Count,
            CompletedActivitySets = completedSets,
            ActivityCompletionRate = allActivitySets.Count > 0
                ? Math.Round((double)completedSets / allActivitySets.Count * 100, 2)
                : 0,
            AverageSessionTimeSeconds = averageSessionTimeSeconds,
            AverageWeightLifted = averageWeightLifted,
            AverageSetsPerSession = averageSetsPerSession,
            HighestPerformance = highestPerformance,
            MostTrainedMuscleGroup = mostTrainedMuscleGroup,
            LeastTrainedMuscleGroup = leastTrainedMuscleGroup,
            WorkoutStatistics = workoutStats,
            MuscleGroupBreakdown = muscleGroupBreakdown
        };
    }
}