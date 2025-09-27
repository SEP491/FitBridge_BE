using System;

namespace FitBridge_Domain.Exceptions;

public class DuplicateException(string message) : BusinessException(message)
{
}
