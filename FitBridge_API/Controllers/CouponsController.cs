using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Features.Coupons.ApplyCoupon;
using FitBridge_Application.Features.Coupons.CreateCoupon;
using FitBridge_Application.Features.Coupons.GetCouponById;
using FitBridge_Application.Features.Coupons.GetUserCreatedCoupons;
using FitBridge_Application.Features.Coupons.GiftCoupon;
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
        /// Applies a coupon to a transaction and calculates the final price after discount.
        /// </summary>
        /// <remarks>
        /// This endpoint validates the coupon and applies it to a cart of items. It performs comprehensive validation including:
        /// - Coupon existence and availability
        /// - Expiration date and active status
        /// - Whether the user has already used this coupon
        /// - Product type compatibility
        /// - Creator matching (for non-system coupons)
        /// 
        /// **Supported Product Types:**
        /// 
        /// 1. **FreelancePTPackage** - Personal training packages offered by freelance trainers
        ///    - All items must belong to the same freelance PT
        ///    - Coupon must be created by that specific PT or be a system coupon
        /// 
        /// 2. **GymCourse** - Gym courses offered by gym owners
        ///    - All items must belong to the same gym owner
        ///    - Coupon must be created by that specific gym owner or be a system coupon
        /// 
        /// 3. **Product** - E-commerce products
        ///    - Only system coupons can be applied to products
        ///    - Merchant-created coupons are not valid for products
        /// 
        /// **Validation Rules:**
        /// - Coupon must be active and not expired
        /// - Coupon must have remaining quantity available
        /// - User cannot use the same coupon twice
        /// - For FreelancePTPackage and GymCourse: all items must be from the same creator
        /// - For Product: only system coupons are allowed
        /// 
        /// **Example Request:**
        /// ```json
        /// {
        ///   "couponCode": "NEWYEAR2024",
        ///   "productType": "GymCourse",
        ///   "itemsId": ["3fa85f64-5717-4562-b3fc-2c963f66afa6", "7b8c1f23-9a45-4b12-8e76-3d4a5f67b890"],
        ///   "totalPrice": 1000000
        /// }
        /// ```
        /// </remarks>
        /// <param name="applyCouponQuery">The coupon application request containing:
        /// <list type="bullet">
        /// <item>
        /// <term>CouponCode</term>
        /// <description>The unique code of the coupon to apply (required).</description>
        /// </item>
        /// <item>
        /// <term>ProductType</term>
        /// <description>The type of products being purchased. Must be one of: "FreelancePTPackage", "GymCourse", or "Product" (required).</description>
        /// </item>
        /// <item>
        /// <term>ItemsId</term>
        /// <description>List of product IDs in the cart. All items must be of the same ProductType (required, cannot be empty).</description>
        /// </item>
        /// <item>
        /// <term>TotalPrice</term>
        /// <description>The total price of all items before discount (required).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>
        /// Returns the coupon application result including:
        /// - Coupon ID
        /// - Final price after discount (DiscountAmount)
        /// - Discount percentage applied
        /// </returns>
        /// <response code="200">Coupon applied successfully. Returns the discounted price and coupon details.</response>
        /// <response code="400">
        /// Bad Request - Validation failed. Possible reasons:
        /// - Invalid product type (not "FreelancePTPackage", "GymCourse", or "Product")
        /// - Empty cart (ItemsId list is empty)
        /// - Items from different creators (for FreelancePTPackage or GymCourse)
        /// - Non-system coupon applied to Product
        /// - Coupon already used by this user
        /// - Some items not found
        /// </response>
        /// <response code="404">
        /// Not Found - Coupon with the specified code does not exist.
        /// </response>
        /// <response code="409">
        /// Conflict - Possible reasons:
        /// - Coupon is out of stock (no remaining quantity)
        /// - Coupon has expired or is inactive
        /// - Coupon does not belong to the product creator
        /// </response>
        [HttpPost("apply")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<ApplyCouponDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Gifts a coupon to one or more customers by sending them a notification.
        /// </summary>
        /// <param name="command">The gift coupon request details, including:
        /// <list type="bullet">
        /// <item>
        /// <term>CouponCode</term>
        /// <description>The code of the coupon to gift.</description>
        /// </item>
        /// <item>
        /// <term>CustomerIds</term>
        /// <description>A list of customer IDs who will receive the coupon notification.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A success response if the coupon was successfully gifted.</returns>
        [HttpPost("gift")]
        public async Task<IActionResult> GiftCoupon([FromBody] GiftCouponCommand command)
        {
            await mediator.Send(command);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Coupon gifted successfully",
                    Empty));
        }
    }
}