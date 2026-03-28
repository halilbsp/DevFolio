using System;
using System.Collections.Generic;

namespace DevFolio.Domain.Entities;

public class Portfolio : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Hero Section Verileri (Örn: Alex Chen, Senior Full-Stack Architect)
    public string FullName { get; set; } = string.Empty; 
    public string JobTitle { get; set; } = string.Empty; 
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string StatusText { get; set; } = "Coding in the clouds"; // Sağ alttaki floating UI için
    
    public string ThemePreference { get; set; } = "Dark"; // Tasarımı korumak için
    
    // İlişkisel Veriler
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
}