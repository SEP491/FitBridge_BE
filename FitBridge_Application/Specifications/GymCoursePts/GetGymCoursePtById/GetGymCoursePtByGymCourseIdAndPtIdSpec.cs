using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;

public class GetGymCoursePtByGymCourseIdAndPtIdSpec : BaseSpecification<GymCoursePT>
{
    public GetGymCoursePtByGymCourseIdAndPtIdSpec(Guid gymCourseId, Guid ptId) : base(x => x.GymCourseId == gymCourseId && x.PTId == ptId)
    {
        AddInclude(x => x.GymCourse);
    }

}
