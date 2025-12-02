using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Orders.GetCourseOrders;
using FitBridge_Domain.Entities.Orders;
using MediatR;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;

namespace FitBridge_Application.Features.Orders.GetCourseOrders;

public class GetCourseOrderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCourseOrderQuery, PagingResultDto<CourseOrderResponseDto>>
{
    public async Task<PagingResultDto<CourseOrderResponseDto>> Handle(GetCourseOrderQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetCourseOrderSpec(request.Parameters);
        var orders = await unitOfWork.Repository<Order>().GetAllWithSpecificationAsync(spec);
        var orderDtos= new List<CourseOrderResponseDto>();
        if (!request.Parameters.IsFreelancePtCourse)
        {
            foreach (var order in orders)
            {
                var orderDto = mapper.Map<CourseOrderResponseDto>(order);
                var manualOrderItems = new List<OrderItemForCourseOrderResponseDto>();
                foreach (var orderItem in order.OrderItems)
                {
                    var orderItemDto = mapper.Map<OrderItemForCourseOrderResponseDto>(orderItem);

                    var gymCourseDto = mapper.Map<GymCourseResponse>(orderItem.GymCourse);
                    if (orderItem.GymPtId != null)
                    {
                        var gymCoursePt = await unitOfWork.Repository<GymCoursePT>().GetBySpecificationAsync(new GetGymCoursePtByGymCourseIdAndPtIdSpec(orderItem.GymCourseId.Value, orderItem.GymPtId.Value));

                        gymCourseDto.Session = gymCoursePt.Session ?? 0;
                        gymCourseDto.GymPtId = orderItem.GymPtId;
                        gymCourseDto.Pt = new GymPtResponseDto();
                        gymCourseDto.Pt.Id = orderItem.GymPtId.Value;
                        gymCourseDto.Pt.FullName = orderItem.GymPt.FullName;
                        gymCourseDto.Pt.AvatarUrl = orderItem.GymPt.AvatarUrl;
                        gymCourseDto.Pt.IsMale = orderItem.GymPt.IsMale;
                        gymCourseDto.Pt.Dob = orderItem.GymPt.Dob;
                        gymCourseDto.Pt.Experience = orderItem.GymPt.UserDetail!.Experience;
                    }
                    orderItemDto.GymCourse = gymCourseDto;
                    orderItemDto.FreelancePTPackage = null;
                    manualOrderItems.Add(orderItemDto);
                }
                orderDto.OrderItems = manualOrderItems;
                orderDtos.Add(orderDto);              
            }
        } else {
            orderDtos = mapper.Map<List<CourseOrderResponseDto>>(orders);
        }

        var totalItems = await unitOfWork.Repository<Order>().CountAsync(spec);
        return new PagingResultDto<CourseOrderResponseDto>(totalItems, orderDtos);
    }
}
