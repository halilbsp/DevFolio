using DevFolio.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace DevFolio.Application.Interfaces;

public interface IPortfolioRepository
{
    Task<Portfolio?> GetPortfolioWithDetailsAsync(Guid userId);
    Task<Portfolio?> GetPortfolioByUsernameAsync(string username);
    Task UpdatePortfolioAsync(Portfolio portfolio);

    // Açık ve Kesin Proje İşlemleri 🛠️
    Task AddProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Project project);

    // Açık ve Kesin Yetenek İşlemleri 🧠
    Task AddSkillAsync(Skill skill);
    Task UpdateSkillAsync(Skill skill);
    Task DeleteSkillAsync(Skill skill);
}