using System;
using FitBridge_Domain.Entities.Gyms;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.GetAllGymCourseForSchedule;

public class GetAllGymCourseForScheduleCommand : IRequest<List<GymCourse>>
{

}
