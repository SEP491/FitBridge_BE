using FitBridge_Application.Dtos.Tokens;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitBridge_Application.Features.Identities.Token
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
