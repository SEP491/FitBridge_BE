using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Coupons.ApplyCoupon;
using FitBridge_Application.Features.Coupons.CreateCoupon;
using FitBridge_Application.Features.Coupons.GetCouponById;
using FitBridge_Application.Features.Coupons.GetUserCreatedCoupons;
using FitBridge_Application.Features.Coupons.RemoveCoupon;
using FitBridge_Application.Features.Coupons.UpdateCoupon;
using FitBridge_Application.Specifications.Coupons.GetCouponByCreatorId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing coupons, including creation, retrieval, update, application, and deletion.
    /// </summary>
    [Authorize]
    public class CouponsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of coupons created by the current user.
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve.</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page.</description>
        /// </item>
        /// <item>
        /// <term>SearchTerm</term>
        /// <description>An optional search term to filter the results.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of coupons created by the user.</returns>
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

        /// <summary>
        /// Retrieves a specific coupon by its unique identifier.
        /// </summary>
        /// <param name="couponId">The unique identifier (GUID) of the coupon to retrieve:
        /// <list type="bullet">
        /// <item>
        /// <term>Id</term>
        /// <description>The unique identifier of the coupon. Only active coupons are returned.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The coupon details including code, discount information, quantity, and usage statistics.</returns>
        [HttpGet("{couponId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetCouponsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCouponById([FromRoute] Guid couponId)
        {
            var result = await mediator.Send(new GetCouponByIdQuery { Id = couponId });
            return Ok(
                new BaseResponse<GetCouponsDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon retrieved successfully",
                    result));
        }

        /// <summary>
        /// Creates a new coupon with the specified details.
        /// </summary>
        /// <param name="command">The details of the coupon to create, including:
        /// <list type="bullet">
        /// <item>
        /// <term>CouponCode</term>
        /// <description>The unique code for the coupon.</description>
        /// </item>
        /// <item>
        /// <term>MaxDiscount</term>
        /// <description>The maximum discount amount the coupon can provide.</description>
        /// </item>
        /// <item>
        /// <term>DiscountPercent</term>
        /// <description>The discount percentage the coupon applies.</description>
        /// </item>
        /// <item>
        /// <term>Quantity</term>
        /// <description>The total number of times the coupon can be used.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The created coupon details.</returns>
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

        /// <summary>
        /// Updates an existing coupon with the specified ID.
        /// </summary>
        /// <param name="couponId">The unique identifier of the coupon to update.</param>
        /// <param name="updateCouponCommand">The updated details of the coupon, including:
        /// <list type="bullet">
        /// <item>
        /// <term>MaxDiscount</term>
        /// <description>The updated maximum discount amount (optional).</description>
        /// </item>
        /// <item>
        /// <term>DiscountPercent</term>
        /// <description>The updated discount percentage (optional).</description>
        /// </item>
        /// <item>
        /// <term>Quantity</term>
        /// <description>The updated total number of uses (optional).</description>
        /// </item>
        /// <item>
        /// <term>IsActive</term>
        /// <description>Whether the coupon is active (optional).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A success response if the update is successful.</returns>
        [HttpPut("{couponId}")]
        public async Task<IActionResult> UpdateCoupon([FromRoute] Guid couponId, [FromBody] UpdateCouponCommand updateCouponCommand)
        {
            updateCouponCommand.CouponId = couponId;
            await mediator.Send(updateCouponCommand);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon updated successfully",
                    Empty));
        }

        /// <summary>
        /// Applies a coupon to a transaction and calculates the discount.
        /// </summary>
        /// <param name="applyCouponQuery">The details of the coupon application, including:
        /// <list type="bullet">
        /// <item>
        /// <term>CouponCode</term>
        /// <description>The code of the coupon to apply.</description>
        /// </item>
        /// <item>
        /// <term>TotalPrice</term>
        /// <description>The total price of the transaction before applying the coupon.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>The result of the coupon application, including the discount amount and percentage.</returns>
        [HttpPost("apply")]
        public async Task<IActionResult> CheckApplyCoupon([FromBody] ApplyCouponQuery applyCouponQuery)
        {
            var response = await mediator.Send(applyCouponQuery);
            return Ok(
                new BaseResponse<ApplyCouponDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon applied successfully",
                    response));
        }

        /// <summary>
        /// Deletes a coupon with the specified ID.
        /// </summary>
        /// <param name="couponId">The unique identifier of the coupon to delete.</param>
        /// <returns>A success response if the deletion is successful.</returns>
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