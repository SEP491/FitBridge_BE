using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Dashboards.GetOrderItemForPendingBalanceDetail;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Dashboards.GetPendingBalanceDetail
{
    internal class GetPendingBalanceDetailQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ITransactionService transactionService,
        IUserUtil userUtil) : IRequestHandler<GetPendingBalanceDetailQuery, PagingResultDto<PendingBalanceOrderItemDto>>
    {
        public async Task<PagingResultDto<PendingBalanceOrderItemDto>> Handle(GetPendingBalanceDetailQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var accountRole = userUtil.GetUserRole(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException("User role");

            var orderItemSpec = new GetOrderItemForPendingBalanceDetailSpec(accountId, accountRole, request.Params);
            var orderItems = await unitOfWork.Repository<OrderItem>()
                .GetAllWithSpecificationAsync(orderItemSpec);

            var countSpec = new GetOrderItemForPendingBalanceDetailSpec(accountId, accountRole, request.Params);
            var totalCount = await unitOfWork.Repository<OrderItem>()
                .CountAsync(countSpec);

            var tasks = orderItems.Select(async oi =>
            {
                var isGymOwner = accountRole == ProjectConstant.UserRoles.GymOwner;
                var profit = await transactionService.CalculateMerchantProfit(oi, oi.Order.Coupon);
                return new PendingBalanceOrderItemDto
                {
                    OrderItemId = oi.Id,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    SubTotal = oi.Price * oi.Quantity,
                    TotalProfit = profit,
                    CouponCode = oi.Order.Coupon?.CouponCode,
                    CouponDiscountPercent = oi.Order.Coupon?.DiscountPercent,
                    CouponId = oi.Order.CouponId,
                    CourseId = isGymOwner ? oi.GymCourseId!.Value : oi.FreelancePTPackageId!.Value,
                    CourseName = isGymOwner ? oi.GymCourse!.Name : oi.FreelancePTPackage!.Name,
                    CustomerId = oi.Order.AccountId,
                    CustomerFullName = oi.Order.Account.FullName,
                    PlannedDistributionDate = oi.ProfitDistributePlannedDate
                };
            });

            var mappedOrderItems = await Task.WhenAll(tasks);

            return new PagingResultDto<PendingBalanceOrderItemDto>(totalCount, mappedOrderItems.ToList());
        }
    }
}