using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.Identities;
using MediatR;

namespace FitBridge_Application.Features.Identities.Registers.RegisterGymPT;

public class RegisterGymPtCommand : IRequest<CreateNewPTResponse>
{
        [JsonIgnore]
        public string? GymOwnerId { get; set; }

        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public CreateNewGymPTRequest CreateNewPT { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsTestAccount { get; set; }
}
