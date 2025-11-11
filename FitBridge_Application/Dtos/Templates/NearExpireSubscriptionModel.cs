using System;

namespace FitBridge_Application.Dtos.Templates;

public class NearExpireSubscriptionModel(string titleSubscriptionName, int bodySubscriptionDays) : IBaseTemplateModel
{
    public string TitleSubscriptionName { get; set; } = titleSubscriptionName;
    public int BodySubscriptionDays { get; set; } = bodySubscriptionDays;
}
