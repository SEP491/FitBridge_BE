using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.QAndA;

public class Question : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid AccountId { get; set; }
    public ApplicationUser Account { get; set; }
}
