using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Vouchers.ApplyCoupon;
using FitBridge_Application.Features.Vouchers.CreateFreelancePTVoucher;
using FitBridge_Application.Features.Vouchers.GetUserVouchers;
using FitBridge_Application.Features.Vouchers.RemoveVoucher;
using FitBridge_Application.Features.Vouchers.UpdateVoucher;
using FitBridge_Application.Specifications.Vouchers.GetVoucherByCreatorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    [Authorize]
    public class CouponsController(IMediator mediator) : _BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCoupons([FromQuery] GetVouchersByCreatorIdParam parameters)
        {
            var result = await mediator.Send(new GetUserCreatedVouchersQuery { Params = parameters });
            var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
            return Ok(
                new BaseResponse<Pagination<GetCouponsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Vouchers retrieved successfully",
                    pagination));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateVoucherCommand command)
        {
            var couponDto = await mediator.Send(command);
            return Created(
                nameof(CreateCoupon),
                new BaseResponse<CreateNewCouponDto>(
                    StatusCodes.Status201Created.ToString(),
                    "Coupon created successfully",
                    couponDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCoupon([FromBody] UpdateVoucherCommand updateVoucherCommand)
        {
            await mediator.Send(updateVoucherCommand);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Voucher updated successfully",
                    Empty));
        }

        [HttpPost("apply/{voucherId}")]
        public async Task<IActionResult> CheckApplyCoupon([FromRoute] Guid voucherId, [FromBody] ApplyCouponQuery applyVoucherCommand)
        {
            applyVoucherCommand.VoucherId = voucherId;
            var response = await mediator.Send(applyVoucherCommand);
            return Ok(
                new BaseResponse<ApplyCouponDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Voucher applied successfully",
                    response));
        }

        [HttpDelete("{voucherId}")]
        public async Task<IActionResult> DeleteCoupon([FromRoute] string voucherId)
        {
            await mediator.Send(new RemoveVoucherCommand { VoucherId = Guid.Parse(voucherId) });
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Voucher deleted successfully",
                    Empty));
        }
    }
}