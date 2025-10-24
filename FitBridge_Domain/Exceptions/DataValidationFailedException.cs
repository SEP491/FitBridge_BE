using System;

namespace FitBridge_Domain.Exceptions;

public class DataValidationFailedException(string message) : BusinessException(message)
{
}
