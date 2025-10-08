using System;
using System.ComponentModel.DataAnnotations;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class CreateRequestBookingDto
{
    public string? BookingName { get; set; }
    public DateOnly BookingDate { get; set; }

    public TimeOnly PtFreelanceStartTime { get; set; }

    public TimeOnly PtFreelanceEndTime { get; set; }

    [EnumDataType(typeof(RequestType), ErrorMessage = "RequestType must be a valid value: CustomerUpdate, PtUpdate, CustomerCreate, or PtCreate")]
    public RequestType RequestType { get; set; }
}
