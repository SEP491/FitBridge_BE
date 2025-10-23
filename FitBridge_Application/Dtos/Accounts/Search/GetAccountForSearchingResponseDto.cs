using System;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Dtos.Accounts.Search;

namespace FitBridge_Application.Dtos.Accounts.Search;

public class GetAccountForSearchingResponseDto
{
    public PagingResultDto<GetAllFreelancePTsResponseDto> FreelancePts { get; set; }
    public PagingResultDto<GetAllGymsForSearchDto> Gyms { get; set; }
}
