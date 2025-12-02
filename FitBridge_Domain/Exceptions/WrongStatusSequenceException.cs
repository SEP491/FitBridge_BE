using System;

namespace FitBridge_Domain.Exceptions;

public class WrongStatusSequenceException(string message) : BusinessException(message)
{
}
