using System;

namespace FitBridge_Application.Dtos.Subscriptions;

public class CheckHotResearchDto
{
    public bool IsHotResearchSubscriptionAvailable { get; set; }
    public int NumOfCurrentHotResearchSubscription { get; set; }
    public int MaxHotResearchSubscription { get; set; }
}
