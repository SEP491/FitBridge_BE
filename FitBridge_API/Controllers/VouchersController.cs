using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Vouchers;
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
    public class VouchersController(IMediator mediator) : _BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersByCreatorIdParam parameters)
        {
            var result = await mediator.Send(new GetUserCreatedVouchersQuery { Params = parameters });
            var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
            return Ok(
                new BaseResponse<Pagination<GetVouchersDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Vouchers retrieved successfully",
                    pagination));
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
        {
            var voucherDto = await mediator.Send(command);
            return Created(
                nameof(CreateVoucher),
                new BaseResponse<CreateNewVoucherDto>(
                    StatusCodes.Status201Created.ToString(),
                    "Voucher created successfully",
                    voucherDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucher([FromBody] UpdateVoucherCommand updateVoucherCommand)
        {
            await mediator.Send(updateVoucherCommand);
            return Ok(
                new BaseResponse<EmptyResult>(
                    StatusCodes.Status200OK.ToString(),
                    "Voucher updated successfully",
                    Empty));
        }

        [HttpDelete("{voucherId}")]
        public async Task<IActionResult> DeleteVoucher([FromRoute] string voucherId)
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