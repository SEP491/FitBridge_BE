using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;
using MediatR;
using Redis.OM.Searching.Query;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedDailyTrainingResults
{
    public class GetCustomerPurchasedDailyTrainingResultsQueryHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<GetCustomerPurchasedDailyTrainingResultsQuery, CustomerPurchasedDailyResultsResponseDto>
    {
        public async Task<CustomerPurchasedDailyResultsResponseDto> Handle(GetCustomerPurchasedDailyTrainingResultsQuery request, CancellationToken cancellationToken)
        {
            // group the bookings by muscle group
            // calculate the total sets, total weight lifted, total time spent for each muscle group
            // calculate average metrics for each muscle group
            // group the bookings by date for each muscle group
            // calculate daily totals for each booking

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
            var completedBookings = bookings.Where(b => b.SessionStatus == SessionStatus.Finished).ToList();
            var completedSessions = completedBookings.Count;

            var muscleGroups = completedBookings.SelectMany(b => b.SessionActivities)
                                            .GroupBy(sa => sa.MuscleGroup)
                                            .ToList();

            var muscleGroupActivities = muscleGroups.Select(mg =>
            {
                var allActivitySets = mg.SelectMany(sa => sa.ActivitySets).ToList();
                var completedActivitySets = mg.SelectMany(sa => sa.ActivitySets)
                                                  .Where(set => set.IsCompleted).ToList();
                var totalSetsCompleted = completedActivitySets.Count;
                SummarizeSessionActivitiesByMuscleGroup(
                    completedActivitySets,
                    out int totalReps,
                    out double totalWeightLifted,
                    out double totalTimeSpentSeconds);

                CalculateAverageMetrics(
                    completedSessions,
                    totalSetsCompleted,
                    totalWeightLifted,
                    totalTimeSpentSeconds,
                    out double averageSetsPerSession,
                    out double averageWeightLifted,
                    out double averageSessionTimeSeconds);

                List<DailyTrainingResultsDto> dailyTrainingResultsDtos = new List<DailyTrainingResultsDto>();

                GetDailyTrainingResults(ref dailyTrainingResultsDtos, mg);

                return new MuscleGroupActivityDto
                {
                    MuscleGroup = mg.Key.ToString(),
                    TotalReps = totalReps,
                    TotalWeight = totalWeightLifted,
                    SetsCount = allActivitySets.Count,
                    SetsCompleted = totalSetsCompleted,
                    AverageSetsPerSession = averageSetsPerSession,
                    AverageWeightLifted = averageWeightLifted,
                    AverageSessionTimeSeconds = averageSessionTimeSeconds,
                    DailyResults = dailyTrainingResultsDtos
                };
            }).ToList();

            return new CustomerPurchasedDailyResultsResponseDto
            {
                MuscleGroupActivities = muscleGroupActivities
            };
        }

        private static void GetDailyTrainingResults(
            ref List<DailyTrainingResultsDto> dailyTrainingResultsDtos,
            IGrouping<MuscleGroupEnum, SessionActivity> mg)
        {
            var groupedDates = mg.GroupBy(mg => mg.Booking.BookingDate).Distinct()
                .ToList();

            foreach (var trainingDate in groupedDates)
            {
                foreach (var activities in trainingDate.DistinctBy(a => a.BookingId))
                {
                    var dailyActivitySets = trainingDate.SelectMany(sa => sa.ActivitySets).ToList();
                    var dailyCompletedSets = dailyActivitySets.Where(set => set.IsCompleted).ToList();
                    SummarizeSessionActivitiesByDate(
                        dailyCompletedSets,
                        out int dailyTotalReps,
                        out double dailyTotalWeight,
                        out double dailyTotalTime);

                    var dto = new DailyTrainingResultsDto
                    {
                        PracticeDay = trainingDate.Key,
                        TotalReps = dailyTotalReps,
                        TotalWeights = dailyTotalWeight,
                        TotalTime = dailyTotalTime,
                        //SetsCount = dailyActivitySets.Count,
                        //SetsCompleted = dailyCompletedSets.Count
                    };

                    dailyTrainingResultsDtos.Add(dto);
                }
            }
        }

        private static void CalculateAverageMetrics(int completedSessions,
            int totalSetsCompleted,
            double totalWeightLifted,
            double totalTimeSpentSeconds,
            out double averageSetsPerSession,
            out double averageWeightLifted,
            out double averageSessionTimeSeconds)
        {
            averageSetsPerSession = completedSessions > 0 ? (double)totalSetsCompleted / completedSessions : 0;
            averageWeightLifted = completedSessions > 0 ? (double)totalWeightLifted / completedSessions : 0;
            averageSessionTimeSeconds = completedSessions > 0 ? (double)totalTimeSpentSeconds / completedSessions : 0;
        }

        private static void SummarizeSessionActivitiesByMuscleGroup(
            List<ActivitySet> completedActivitySets,
            out int totalReps,
            out double totalWeightLifted,
            out double totalTimeSpentSeconds)
        {
            totalReps = completedActivitySets
                              .Where(set => set.NumOfReps.HasValue)
                              .Sum(set => set.NumOfReps!.Value);
            totalWeightLifted = completedActivitySets
                                      .Where(set => set.WeightLifted.HasValue)
                                      .Sum(set => set.WeightLifted!.Value);
            totalTimeSpentSeconds = completedActivitySets
                                        .Where(set => set.PracticeTime.HasValue)
                                        .Sum(set => set.PracticeTime!.Value);
        }

        private static void SummarizeSessionActivitiesByDate(
            List<ActivitySet> dailyCompletedSets,
            out int dailyTotalReps, out double dailyTotalWeight, out double dailyTotalTime)
        {
            dailyTotalReps = dailyCompletedSets
                                  .Where(set => set.NumOfReps.HasValue)
                                  .Sum(set => set.NumOfReps!.Value);
            dailyTotalWeight = dailyCompletedSets
                                    .Where(set => set.WeightLifted.HasValue)
                                    .Sum(set => set.WeightLifted!.Value);
            dailyTotalTime = dailyCompletedSets
                                    .Where(set => set.PracticeTime.HasValue)
                                    .Sum(set => set.PracticeTime!.Value);
        }
    }
}