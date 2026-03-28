using System;

namespace DevFolio.Domain.Entities;

public class SocialLink : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;

    public string PlatformName { get; set; } = string.Empty; // Örn: LinkedIn
    public string Url { get; set; } = string.Empty;
}