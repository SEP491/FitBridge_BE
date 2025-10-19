using System;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class SessionReportDto
{
    public Guid BookingId { get; set; }
    public string? SessionName { get; set; }
    public DateOnly DateTraining { get; set; }
    public TimeOnly? PlannedStartTime { get; set; }
    public TimeOnly? PlannedEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }

    //Overview
    public SessionTotalsDto SessionTotalSummary { get; set; } = new(); // có % hoàn thành
    public List<ActivityType> ActivityTypesPerformed { get; set; } = new(); // Danh sách các loại hoạt động đã thực hiện

    // Chi tiết
    public List<ActivitySummaryDto> ActivitiesSummary { get; set; } = new();     // mỗi bài tập
    public List<MuscleGroupAggregateDto> MuscleGroupAggregates { get; set; } = new();

    public string? Note { get; set; }
    public string? NutritionTip { get; set; }


}

public class SessionTotalsDto
{
    public int SessionActivityCount { get; set; }

    //Set/Reps toàn buổi
    public int TotalCompletedSets { get; set; }
    public int TotalCompletedReps { get; set; }
    public int TotalCompletedDistanceMeters { get; set; }

    public int TotalPlannedDistanceMeters { get; set; }
    public double TotalPlannedPracticeTimeSec { get; set; } // Tổng thời gian tập luyện của các Reps

    //Planned vs actual sets
    public int PlannedSets { get; set; }
    public int PlannedReps { get; set; }
    public double? CompletionPercentage { get; set; }

    public double TotalCompletedPracticeTimeSec { get; set; }
    public int TotalRestTimeSec { get; set; } // Tổng thời gian nghỉ giữa các reps
}

public class ActivitySummaryDto
{
    public Guid SessionActivityId { get; set; }
    public string? ActivityName { get; set; }
    public ActivityType ActivityType { get; set; }
    public MuscleGroupEnum MuscleGroup { get; set; }

    //Kết cấu bài
    public int CompletedReps { get; set; }
    public int PlannedSets { get; set; }
    public int PlannedReps { get; set; }
    public int PlannedDistanceMeters { get; set; }
    public double PlannedPracticeTimeSeconds { get; set; }
    public int CompletedDistanceMeters { get; set; }
    public double CompletedPracticeTimeSeconds { get; set; }
    public int CompletedSets { get; set; }
    public int? LongestDistanceMeters { get; set; }
    public int? ShortestDistanceMeters { get; set; }

    public double? LongestPracticeTimeSeconds { get; set; }
    public double? ShortestPracticeTimeSeconds { get; set; }
    public double? HeaviestWeightLifted { get; set; } = 0.0;
    public double? LightestWeightLifted { get; set; } = 0.0;

}

public class MuscleGroupAggregateDto
{
    public MuscleGroupEnum MuscleGroup { get; set; }
    public int SessionActivitiesCount { get; set; }
    public int TotalSetsCompleted { get; set; }
    public int TotalRepsCompleted { get; set; }
    public double VolumeActualWeightLifted { get; set; }
    public double VolumePlannedWeightLifted { get; set; }
    public double PracticeTimeActualSeconds { get; set; }
    public double PracticeTimePlannedSeconds { get; set; }
    public int DistancePlannedMeters { get; set; }
    public int DistanceActualMeters { get; set; }
}


