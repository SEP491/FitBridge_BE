using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.Blogging;

public class Blog : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid AuthorId { get; set; }
    public List<string> Images { get; set; }
    public ApplicationUser Author { get; set; }
}