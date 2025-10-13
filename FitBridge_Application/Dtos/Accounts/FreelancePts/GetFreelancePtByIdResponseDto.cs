using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using FitBridge_Application.Dtos.FreelancePTPackages;

namespace FitBridge_Application.Dtos.Accounts.FreelancePts;

public class GetFreelancePtByIdResponseDto
{
    public GetAllFreelancePTsResponseDto FreelancePt { get; set; }
    public UserDetailDto UserDetail { get; set; }
    public List<GetAllFreelancePTPackagesDto> FreelancePTPackages { get; set; }
}
