using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Dashboards.GetOrderItemForRevenueDetail;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Dashboards.GetRevenueDetail
{
    internal class GetRevenueDetailQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ITransactionService transactionService,
        IUserUtil userUtil) : IRequestHandler<GetRevenueDetailQuery, DashboardPagingResultDto<RevenueOrderItemDto>>
    {
        public async Task<DashboardPagingResultDto<RevenueOrderItemDto>> Handle(GetRevenueDetailQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var accountRole = userUtil.GetUserRole(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException("User role");

            var orderItemSpec = new GetOrderItemForRevenueDetailSpec(accountId, accountRole, request.Params);
            var orderItems = await unitOfWork.Repository<OrderItem>()
                .GetAllWithSpecificationAsync(orderItemSpec);

            var countSpec = new GetOrderItemForRevenueDetailSpec(accountId, accountRole, request.Params);
            var totalCount = await unitOfWork.Repository<OrderItem>()
                .CountAsync(countSpec);

            var tasks = orderItems.Select(async oi =>
            {
                var isGymOwner = accountRole == ProjectConstant.UserRoles.GymOwner;
                var profit = await transactionService.CalculateMerchantProfit(oi, oi.Order.Coupon);
                var commissionAmount = await transactionService.CalculateCommissionAmount(oi, oi.Order.Coupon);
                var commissionRate = oi.Order.CommissionRate;

                return new RevenueOrderItemDto
                {
                    OrderItemId = oi.Id,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    SubTotal = oi.Price * oi.Quantity,
                    TotalProfit = profit,
                    SystemProfit = commissionAmount,
                    CommissionRate = (double)commissionRate,
                    CouponCode = oi.Order.Coupon?.CouponCode,
                    CouponDiscountPercent = oi.Order.Coupon?.DiscountPercent,
                    CouponId = oi.Order.CouponId,
                    CourseId = isGymOwner ? oi.GymCourseId!.Value : oi.FreelancePTPackageId!.Value,
                    CourseName = isGymOwner ? oi.GymCourse!.Name : oi.FreelancePTPackage!.Name,
                    CustomerId = oi.Order.AccountId,
                    CustomerFullName = oi.Order.Account.FullName,
                    PlannedDistributionDate = oi.ProfitDistributePlannedDate,
                    ActualDistributionDate = oi.ProfitDistributeActualDate
                };
            });

            var mappedOrderItems = await Task.WhenAll(tasks);
            var totalProfitSum = mappedOrderItems.Sum(oi => oi.TotalProfit);

            return new DashboardPagingResultDto<RevenueOrderItemDto>(totalCount, mappedOrderItems.ToList(), totalProfitSum);
        }
    }
}