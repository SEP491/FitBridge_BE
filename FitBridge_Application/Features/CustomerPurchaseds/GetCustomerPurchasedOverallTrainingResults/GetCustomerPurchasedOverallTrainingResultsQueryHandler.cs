using AutoMapper;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedOverallTrainingResults;

public class GetCustomerPurchasedOverallTrainingResultsQueryHandler(
    IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetCustomerPurchasedOverallTrainingResultsQuery, CustomerPurchasedOverallResultResponseDto>
{
    public async Task<CustomerPurchasedOverallResultResponseDto> Handle(
        GetCustomerPurchasedOverallTrainingResultsQuery request, CancellationToken cancellationToken)
    {
        var customerPurchasedSpec = new GetCustomerPurchasedByIdSpec(
            request.CustomerPurchasedId,
            isIncludeBooking: true,
            isIncludeActivitySets: true,
            isIncludeSessionActivities: true,
            isIncludeUserGoals: true);
        var customerPurchased = await unitOfWork.Repository<CustomerPurchased>()
            .GetBySpecificationAsync(customerPurchasedSpec)
            ?? throw new NotFoundException(nameof(CustomerPurchased));

        var bookings = customerPurchased.Bookings.ToList();
        var userGoals = mapper.Map<UserGoalsDto?>(customerPurchased.UserGoal);
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

        var workoutStats = CalculateWorkoutStatistics(allActivities, allActivitySets);

        CalculateAverageMetrics(completedSessions,
            completedSets,
            workoutStats,
            out double averageSessionTimeSeconds,
            out double averageWeightLifted,
            out double averageSetsPerSession);

        // Calculate highest performance (session with most weight lifted)
        CalculateBestPerformance(out HighestPerformanceDto? highestPerformance, completedBookings);
        List<MuscleGroupActivityDto> muscleGroupBreakdown = BreakdownMuscleGroups(allActivities);

        MuscleGroupInsightDto? mostTrainedMuscleGroup = null;
        MuscleGroupInsightDto? leastTrainedMuscleGroup = null;
        if (muscleGroupBreakdown.Count > 0)
        {
            var mostTrained = muscleGroupBreakdown[0];
            mostTrainedMuscleGroup = GetMostTrained(mostTrained);

            var leastTrained = muscleGroupBreakdown[^1];
            leastTrainedMuscleGroup = GetLeastTrained(leastTrained);
        }

        return new CustomerPurchasedOverallResultResponseDto
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
            UserGoals = userGoals
        };
    }

    private static MuscleGroupInsightDto GetLeastTrained(MuscleGroupActivityDto leastTrained)
    {
        return new MuscleGroupInsightDto
        {
            MuscleGroup = leastTrained.MuscleGroup,
            TotalSets = leastTrained.SetsCompleted,
            TotalWeight = leastTrained.TotalWeight,
            SetsCount = leastTrained.SetsCount
        };
    }

    private static MuscleGroupInsightDto GetMostTrained(MuscleGroupActivityDto mostTrained)
    {
        return new MuscleGroupInsightDto
        {
            MuscleGroup = mostTrained.MuscleGroup,
            TotalSets = mostTrained.SetsCompleted,
            TotalWeight = mostTrained.TotalWeight,
            SetsCount = mostTrained.SetsCount
        };
    }

    private static List<MuscleGroupActivityDto> BreakdownMuscleGroups(List<SessionActivity> allActivities)
    {
        return allActivities
            .GroupBy(a => a.MuscleGroup.ToString())
            .Select(g => new MuscleGroupActivityDto
            {
                MuscleGroup = g.Key,
                SetsCount = g.SelectMany(a => a.ActivitySets).Count(),
                SetsCompleted = g.SelectMany(a => a.ActivitySets)
                    .Count(s => s.IsCompleted),
                TotalWeight = g.SelectMany(a => a.ActivitySets)
                    .Where(s => s.IsCompleted && s.WeightLifted.HasValue)
                    .Sum(s => s.WeightLifted!.Value),
                TotalReps = g.SelectMany(a => a.ActivitySets)
                    .Where(s => s.IsCompleted && s.NumOfReps.HasValue)
                    .Sum(s => s.NumOfReps!.Value)
            })
            .OrderByDescending(m => m.SetsCompleted)
            .ToList();
    }

    private static void CalculateBestPerformance(out HighestPerformanceDto? highestPerformance, List<Booking> completedBookings)
    {
        highestPerformance = null;
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
    }

    private static void CalculateAverageMetrics(int completedSessions,
        int completedSets,
        WorkoutStatisticsDto workoutStats,
        out double averageSessionTimeSeconds, out double averageWeightLifted, out double averageSetsPerSession)
    {
        averageSessionTimeSeconds = completedSessions > 0
            ? workoutStats.TotalPracticeTimeSeconds / completedSessions
            : 0;

        averageWeightLifted = completedSessions > 0
            ? Math.Round(workoutStats.TotalWeightLifted / completedSessions, 1)
            : 0;

        averageSetsPerSession = completedSessions > 0
            ? Math.Round((double)completedSets / completedSessions, 1)
            : 0;
    }

    private static WorkoutStatisticsDto CalculateWorkoutStatistics(List<SessionActivity> allActivities, List<ActivitySet> allActivitySets)
    {
        return new WorkoutStatisticsDto
        {
            TotalWeightLifted = allActivitySets
            .Where(s => s.IsCompleted && s.WeightLifted.HasValue)
            .Sum(s => s.WeightLifted.Value),

            PlannedDistance = allActivitySets
                .Where(s => s.IsCompleted && s.PlannedDistance.HasValue)
                .Sum(s => s.PlannedDistance.Value),
            TotalDistance = allActivitySets
                .Where(s => s.IsCompleted && s.ActualDistance.HasValue)
                .Sum(s => s.ActualDistance.Value),

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
    }
}