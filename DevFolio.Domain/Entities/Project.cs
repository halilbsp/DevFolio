using System;

namespace DevFolio.Domain.Entities;

public class Project : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string LiveUrl { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}