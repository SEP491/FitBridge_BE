using System;

namespace FitBridge_Application.Dtos.CustomerPurchaseds;

public class CustomerPurchasedForMyPackageDto
{
    public PagingResultDto<CustomerPurchasedFreelancePtResponseDto> freelancePtPackages { get; set; } = new PagingResultDto<CustomerPurchasedFreelancePtResponseDto>(0, new List<CustomerPurchasedFreelancePtResponseDto>());
    public PagingResultDto<CustomerPurchasedResponseDto> gymCourses { get; set; } = new PagingResultDto<CustomerPurchasedResponseDto>(0, new List<CustomerPurchasedResponseDto>());
}
