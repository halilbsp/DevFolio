using System;

namespace DevFolio.Domain.Entities;

public class Experience : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;

    public string Period { get; set; } = string.Empty; // Örn: "2021 — PRESENT"
    public string Role { get; set; } = string.Empty; // Örn: "Principal Architect"
    public string Company { get; set; } = string.Empty; // Örn: "NovaStream Tech"
    public string Description { get; set; } = string.Empty;
    
    // Tasarımdaki Timeline noktalarının renklerini belirlemek için (primary, secondary)
    public string TimelineDotColor { get; set; } = "bg-primary"; 
}