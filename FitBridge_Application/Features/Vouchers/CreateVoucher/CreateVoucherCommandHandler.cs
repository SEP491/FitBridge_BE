using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher
{
    internal class CreateVoucherCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContext,
        IApplicationUserService applicationUserService) : IRequestHandler<CreateVoucherCommand, CreateNewVoucherDto>
    {
        public async Task<CreateNewVoucherDto> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
        {
            var creatorId = userUtil.GetAccountId(httpContext.HttpContext)
                ?? throw new NotFoundException("User not found");

            var creator = await applicationUserService.GetByIdAsync(creatorId) ?? throw new NotFoundException(nameof(ApplicationUser));

            var creatorRole = await applicationUserService.GetUserRoleAsync(creator);

            var voucherType = creatorRole.Equals(ProjectConstant.UserRoles.FreelancePT) ? VoucherType.FreelancePT
                : VoucherType.System;

            var newVoucher = new Voucher
            {
                MaxDiscount = request.MaxDiscount,
                Type = voucherType,
                DiscountPercent = request.DiscountPercent,
                Quantity = request.Quantity,
                CreatorId = creatorId,
                Id = Guid.NewGuid()
            };

            unitOfWork.Repository<Voucher>().Insert(newVoucher);

            await unitOfWork.CommitAsync();

            return mapper.Map<CreateNewVoucherDto>(newVoucher);
        }
    }
}