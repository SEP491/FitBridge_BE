using System;
using FitBridge_Application.Dtos.Flavours;
using MediatR;

namespace FitBridge_Application.Features.Flavours.GetAllFlavours;

public class GetAllFlavoursQuery : IRequest<List<FlavourResponseDto>>
{
}
