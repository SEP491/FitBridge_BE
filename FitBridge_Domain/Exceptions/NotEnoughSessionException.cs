using System;

namespace FitBridge_Domain.Exceptions;

public class NotEnoughSessionException(string message) : BusinessException(message)
{
}
