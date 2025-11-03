using System;
using MediatR;

namespace FitBridge_Application.Features.SystemConfigurations;

public class GetSystemConfigQuery : IRequest<object>
{
    public string Key { get; set; }
}
