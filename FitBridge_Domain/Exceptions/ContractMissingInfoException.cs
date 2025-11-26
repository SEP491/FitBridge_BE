using System;

namespace FitBridge_Domain.Exceptions;

public class ContractMissingInfoException(string message) : BusinessException(message)
{
}
