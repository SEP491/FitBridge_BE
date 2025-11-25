using System;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Contracts.UpdateContract;

public class UpdateContractCommand : IRequest<Guid>
{
    public Guid ContractId { get; set; }
    public IFormFile? ContractUrl { get; set; }
    public IFormFile? CompanySignatureUrl { get; set; }
    public IFormFile? CustomerSignatureUrl { get; set; }
}
