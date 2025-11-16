using System;
using FitBridge_Application.Dtos.Addresses;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Addresses.UpdateAddress;

public class UpdateAddressCommand : IRequest<AddressResponseDto>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string ReceiverName { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string Note { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? GoogleMapAddressString { get; set; }
}
