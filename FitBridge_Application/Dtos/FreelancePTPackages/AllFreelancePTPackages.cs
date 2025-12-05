using System;

namespace FitBridge_Application.Dtos.FreelancePTPackages;

public class AllFreelancePTPackagesDto
{
    public FreelancePtPackageSummaryDto Summary { get; set; }
    public PagingResultDto<GetAllFreelancePTPackagesDto> Packages { get; set; }
}
