using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using Microsoft.Net.Http.Headers;

namespace FitBridge_Application.Features.Bookings.GetTrainingResult;

public class GetTrainingResultQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetTrainingResultQuery, SessionReportDto>
{
    public async Task<SessionReportDto> Handle(GetTrainingResultQuery request, CancellationToken cancellationToken)
    {
        var bookingEntity = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, false, new List<string>
        {
            "SessionActivities",
            "SessionActivities.ActivitySets",
            "Pt"
        });
        if (bookingEntity == null)
        {
            throw new NotFoundException("Booking not found");
        }
        var trainingResult = _mapper.Map<SessionReportDto>(bookingEntity);
        trainingResult.SessionTotalSummary.CompletionPercentage = trainingResult.SessionTotalSummary.PlannedReps == 0 && trainingResult.SessionTotalSummary.TotalPlannedDistanceMeters == 0 && trainingResult.SessionTotalSummary.TotalPlannedPracticeTimeSec == 0 ? 0 : Math.Round((double)(trainingResult.SessionTotalSummary.TotalCompletedReps
        + trainingResult.SessionTotalSummary.TotalCompletedDistanceMeters
        + trainingResult.SessionTotalSummary.TotalCompletedPracticeTimeSec
        ) / (
            trainingResult.SessionTotalSummary.PlannedReps + trainingResult.SessionTotalSummary.TotalPlannedDistanceMeters + trainingResult.SessionTotalSummary.TotalPlannedPracticeTimeSec
            ) * 100, 1, MidpointRounding.AwayFromZero);
        var muscleGroups = bookingEntity.SessionActivities.Select(x => x.MuscleGroup).Distinct().ToList();
        foreach (var muscleGroup in muscleGroups)
        {
            var muscleGroupAggregate = new MuscleGroupAggregateDto();
            foreach (var sessionActivity in bookingEntity.SessionActivities.Where(x => x.MuscleGroup == muscleGroup))
            {
                muscleGroupAggregate.SessionActivitiesCount++;
                muscleGroupAggregate.MuscleGroup = muscleGroup;

                foreach (var activitySet in sessionActivity.ActivitySets)
                {
                    if (activitySet.IsCompleted)
                    {
                        muscleGroupAggregate.TotalSetsCompleted++;
                        muscleGroupAggregate.TotalRepsCompleted += activitySet.NumOfReps ?? 0;
                    }
                    muscleGroupAggregate.VolumeActualWeightLifted += activitySet.IsCompleted ? (activitySet.WeightLifted ?? 0) * (activitySet.NumOfReps ?? 0) : 0; 
                    muscleGroupAggregate.VolumePlannedWeightLifted += (activitySet.WeightLifted ?? 0) * (activitySet.PlannedNumOfReps ?? 0);
                    muscleGroupAggregate.DistanceActualMeters += activitySet.IsCompleted ? activitySet.ActualDistance ?? 0 : 0;
                    muscleGroupAggregate.DistancePlannedMeters += activitySet.PlannedDistance ?? 0;
                    muscleGroupAggregate.PracticeTimeActualSeconds += activitySet.IsCompleted ? activitySet.PracticeTime ?? 0 : 0;
                    muscleGroupAggregate.PracticeTimePlannedSeconds += activitySet.PlannedPracticeTime ?? 0;
                }
            }
            trainingResult.MuscleGroupAggregates.Add(muscleGroupAggregate);
        }
        return trainingResult;
    }

}
