using System;
using AutoMapper;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Features.BodyMeasurements.CreateBodyMeasurement;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.MappingProfiles;

public class BodyMeasurementMappingProfile : Profile
{
    public BodyMeasurementMappingProfile()
    {
        CreateMap<BodyMeasurementRecord, BodyMeasurementRecordDto>();
        CreateMap<CreateBodyMeasurementCommand, BodyMeasurementRecord>();
    }
}
