using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetBookingHistory
{
    public class GetBookingHistorySpec : BaseSpecification<Booking>
    {
        public GetBookingHistorySpec(
            GetBookingHistoryParams parameters,
            Guid userId,
            string userRole) : base(x =>
            x.IsEnabled
            && ((userRole == ProjectConstant.UserRoles.Customer && x.CustomerId == userId)
                || (userRole == ProjectConstant.UserRoles.FreelancePT && x.PtId == userId)
                || (userRole == ProjectConstant.UserRoles.GymPT && x.PtId == userId))
            && (!parameters.StartDate.HasValue || x.BookingDate >= parameters.StartDate.Value)
            && (!parameters.EndDate.HasValue || x.BookingDate <= parameters.EndDate.Value)
            && (!parameters.Status.HasValue || x.SessionStatus == parameters.Status.Value)
            && (string.IsNullOrEmpty(parameters.SearchTerm) ||
                (x.BookingName != null && x.BookingName.ToLower().Contains(parameters.SearchTerm.ToLower())) ||
                (x.Note != null && x.Note.ToLower().Contains(parameters.SearchTerm.ToLower()))))
        {
            if (userRole == ProjectConstant.UserRoles.Customer)
            {
                AddInclude(x => x.Pt);
                AddInclude(x => x.PTGymSlot);
                AddInclude("PTGymSlot.GymSlot");
                AddInclude("PTGymSlot.PT");
            }
            else if (userRole == ProjectConstant.UserRoles.FreelancePT)
            {
                AddInclude(x => x.Customer);
                AddInclude(x => x.CustomerPurchased);
                AddInclude("CustomerPurchased.OrderItems");
                AddInclude("CustomerPurchased.OrderItems.FreelancePTPackage");
            }
            else if (userRole == ProjectConstant.UserRoles.GymPT)
            {
                AddInclude(x => x.Customer);
                AddInclude(x => x.PTGymSlot);
                AddInclude("PTGymSlot.GymSlot");
            }

            if (parameters.SortBy.Equals("BookingDate", StringComparison.OrdinalIgnoreCase))
            {
                if (parameters.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                    AddOrderBy(x => x.BookingDate);
                else
                    AddOrderByDesc(x => x.BookingDate);
            }
            else if (parameters.SortBy.Equals("Status", StringComparison.OrdinalIgnoreCase))
            {
                if (parameters.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                    AddOrderBy(x => x.SessionStatus);
                else
                    AddOrderByDesc(x => x.SessionStatus);
            }
            else if (parameters.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                if (parameters.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                    AddOrderBy(x => x.CreatedAt);
                else
                    AddOrderByDesc(x => x.CreatedAt);
            }
            else
            {
                AddOrderByDesc(x => x.BookingDate);
            }

            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
        }
    }
}