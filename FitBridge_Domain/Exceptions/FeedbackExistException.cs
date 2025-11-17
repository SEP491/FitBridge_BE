using System;

namespace FitBridge_Domain.Exceptions;

public class FeedbackExistException(string message) : BusinessException(message)
{

}
