using System;

namespace DevFolio.Domain.Entities;

public class Skill : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;

    public string Name { get; set; } = string.Empty; // Örn: .NET 10
    public string ColorClass { get; set; } = "text-primary"; // Tasarımdaki renkleri bozmamak için (text-primary, text-secondary vs.)
}