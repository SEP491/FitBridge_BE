using System;
using FitBridge_Application.Dtos.Orders;
using MediatR;
using FitBridge_Application.Specifications.Orders.GetCourseOrders;
using FitBridge_Application.Dtos;

namespace FitBridge_Application.Features.Orders.GetCourseOrders;

public class GetCourseOrderQuery(GetCourseOrderParams parameters) : IRequest<PagingResultDto<CourseOrderResponseDto>>
{
    public GetCourseOrderParams Parameters { get; set; } = parameters;
}
