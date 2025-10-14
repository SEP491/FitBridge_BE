using System;

namespace FitBridge_Application.Dtos.Bookings;

public class TrainingResultResponseDto
{
    public Guid BookingId { get; set; }
    public string BookingName { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SetsPlan { get; set; }
    public int SetsCompleted { get; set; }
    public double PracticeTime { get; set; }
    public double RestTime { get; set; }
    public double WeightLifted { get; set; }
    public double NumOfReps { get; set; }
    public RepsProgressDto RepsProgress { get; set; }
    public WeightLiftedProgressDto WeightLiftedProgress { get; set; }
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

