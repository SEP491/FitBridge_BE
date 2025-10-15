using System;

namespace FitBridge_Application.Dtos.Bookings;

public class TrainingResultResponseDto
{
    public Guid BookingId { get; set; }
    public string BookingName { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int SetsPlan { get; set; }
    public int SetsCompleted { get; set; }
    public double RestTime { get; set; }
    public RepsProgressDto RepsProgress { get; set; }
    public WeightLiftedProgressDto WeightLiftedProgress { get; set; }
    public PracticeTimeProgressDto PracticeTimeProgress { get; set; }
    public string NutritionTip { get; set; }
    public string Notes { get; set; }
    public string PtName { get; set; }
    public string PtAvatarUrl { get; set; }
}

public class RepsProgressDto
{
    public double RepsCompleted { get; set; }
    public double RepsPlan { get; set; }
    public double RepsProgressPercentage => RepsPlan == 0 ? 0 : (double)(RepsCompleted / RepsPlan) * 100;
}

public class WeightLiftedProgressDto
{
    public double WeightLiftedCompleted { get; set; }
    public double WeightLiftedPlan { get; set; }
    public double WeightLiftedProgressPercentage => WeightLiftedPlan == 0 ? 0 : (double)(WeightLiftedCompleted / WeightLiftedPlan) * 100;
}

public class PracticeTimeProgressDto
{
    public double PracticeTimeCompleted { get; set; }
    public double PracticeTimePlan { get; set; }
    public double PracticeTimeProgressPercentage => PracticeTimePlan == 0 ? 0 : (double)(PracticeTimeCompleted / PracticeTimePlan) * 100;
}

