using System;

namespace FitBridge_Domain.Exceptions;

public class PackageExistException(string message) : BusinessException(message)
{
}
