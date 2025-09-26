using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Specifications.GymCoursePts.GetPurchasedGymCoursePtForScheduleGetPurchasedGymCoursePtForSchedule;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.GetPurchasedGymCoursePtForSchedule;

public class GetPurchasedGymCoursePtForScheduleQuery : IRequest<PagingResultDto<GymCoursesPtResponse>>
{
    public GetPurchasedGymCoursePtForScheduleParams? Params { get; set; }
}