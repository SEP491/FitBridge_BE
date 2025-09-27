using System;
using System.Security.Cryptography.X509Certificates;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymSlotForCreateValidationSpecification : BaseSpecification<GymSlot>
{
    public GetGymSlotForCreateValidationSpecification(CreateNewSlotResponse request)
        : base(x =>
            x.IsEnabled
            && x.Name == request.Name
            && !(x.EndTime <= request.StartTime || request.EndTime <= x.StartTime))
    {
    }
}
