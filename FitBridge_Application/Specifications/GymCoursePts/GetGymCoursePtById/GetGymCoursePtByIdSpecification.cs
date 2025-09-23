using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;

public class GetGymCoursePtByIdSpecification : BaseSpecification<GymCoursePT>
{
    public GetGymCoursePtByIdSpecification(Guid gymCoursePtId) : base(x => x.Id == gymCoursePtId)
    {
        AddInclude(x => x.GymCourse);
    }
}
