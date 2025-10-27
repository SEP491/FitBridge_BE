using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym.GetAllGymOwnerCustomer;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetAllGymCustomer;

public class GetAllGymOwnerCustomerQuery(GetAllGymOwnerCustomerParams parameters) : IRequest<PagingResultDto<GetAllGymOwnerCustomer>>
{
    public GetAllGymOwnerCustomerParams Params { get; set; } = parameters;
}
