using System;
using System.Text.Json.Serialization;
using MediatR;

namespace FitBridge_Application.Features.Contracts.ConfirmContract;

public class ConfirmContractCommand : IRequest<Guid>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}
