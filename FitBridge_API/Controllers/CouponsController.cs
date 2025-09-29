using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Coupons.ApplyCoupon;
using FitBridge_Application.Features.Coupons.CreateCoupon;
using FitBridge_Application.Features.Coupons.GetUserCreatedCoupons;
using FitBridge_Application.Features.Coupons.RemoveCoupon;
using FitBridge_Application.Features.Coupons.UpdateCoupon;
using FitBridge_Application.Specifications.Coupons.GetCouponByCreatorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    [Authorize]
    public class CouponsController(IMediator mediator) : _BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCoupons([FromQuery] GetCouponsByCreatorIdParam parameters)
        {
            var result = await mediator.Send(new GetUserCreatedCouponsQuery { Params = parameters });
            var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
            return Ok(
                new BaseResponse<Pagination<GetCouponsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupons retrieved successfully",
                    pagination));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponCommand command)
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
        public async Task<IActionResult> UpdateCoupon([FromBody] UpdateCouponCommand updateCouponCommand)
        {
            await mediator.Send(updateCouponCommand);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon updated successfully",
                    Empty));
        }

        [HttpPost("apply/{couponId}")]
        public async Task<IActionResult> CheckApplyCoupon([FromRoute] Guid couponId, [FromBody] ApplyCouponQuery applyCouponQuery)
        {
            applyCouponQuery.CouponId = couponId;
            var response = await mediator.Send(applyCouponQuery);
            return Ok(
                new BaseResponse<ApplyCouponDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon applied successfully",
                    response));
        }

        [HttpDelete("{couponId}")]
        public async Task<IActionResult> DeleteCoupon([FromRoute] string couponId)
        {
            await mediator.Send(new RemoveCouponCommand { CouponId = Guid.Parse(couponId) });
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon deleted successfully",
                    Empty));
        }
    }
}