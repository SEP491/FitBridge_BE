using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetGymPtSchedule
{
    public class GetGymPtScheduleSpec : BaseSpecification<Booking>
    {
     public GetGymPtScheduleSpec(GetGymPtScheduleParams parameters, Guid ptId) : base(x => 
   x.PtId == ptId 
   && x.BookingDate == parameters.Date
   && x.PTGymSlotId != null)
    {
   AddInclude(x => x.Customer);
       AddInclude(x => x.CustomerPurchased);
  AddInclude(x => x.PTGymSlot);
       AddInclude("PTGymSlot.GymSlot");
     AddInclude("CustomerPurchased.OrderItems");
      AddInclude("CustomerPurchased.OrderItems.GymCourse");

   if (parameters.DoApplyPaging)
   {
      AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
  }
   else
{
    parameters.Size = -1;
    parameters.Page = -1;
    }
        }
    }
}
