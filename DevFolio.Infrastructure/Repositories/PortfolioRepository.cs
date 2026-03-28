using DevFolio.Application.Interfaces;
using DevFolio.Domain.Entities;
using DevFolio.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DevFolio.Infrastructure.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly AppDbContext _context;

    public PortfolioRepository(AppDbContext context) { _context = context; }

    public async Task<Portfolio?> GetPortfolioWithDetailsAsync(Guid userId)
    {
        return await _context.Portfolios
            .Include(p => p.Projects).Include(p => p.Skills)
            .Include(p => p.Experiences).Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<Portfolio?> GetPortfolioByUsernameAsync(string username)
    {
        // AsNoTracking() EKLENDİ! Artık public sayfa ve dashboard her zaman en güncel veriyi çekecek! 🚀
        return await _context.Portfolios
            .AsNoTracking()
            .Include(p => p.User).Include(p => p.Projects)
            .Include(p => p.Skills).Include(p => p.Experiences).Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.FullName.ToLower().Replace(" ", "") == username.ToLower());
    }

    public async Task UpdatePortfolioAsync(Portfolio portfolio)
    {
        // Veritabanındaki kaydı bul ve yeni verilerle zorla değiştir (Kurşun Geçirmez Kayıt 🛡️)
        var existing = await _context.Portfolios.FindAsync(portfolio.Id);
        if (existing != null) {
            _context.Entry(existing).CurrentValues.SetValues(portfolio);
            await _context.SaveChangesAsync();
        }
    }

    // --- PROJE (CRUD) ---
    public async Task AddProjectAsync(Project project) {
        _context.Projects.Add(project); await _context.SaveChangesAsync();
    }
    public async Task UpdateProjectAsync(Project project) {
        var existing = await _context.Projects.FindAsync(project.Id);
        if (existing != null) { _context.Entry(existing).CurrentValues.SetValues(project); await _context.SaveChangesAsync(); }
    }
    public async Task DeleteProjectAsync(Project project) {
        var existing = await _context.Projects.FindAsync(project.Id);
        if (existing != null) { _context.Projects.Remove(existing); await _context.SaveChangesAsync(); }
    }

    // --- YETENEK (CRUD) ---
    public async Task AddSkillAsync(Skill skill) {
        _context.Skills.Add(skill); await _context.SaveChangesAsync();
    }
    public async Task UpdateSkillAsync(Skill skill) {
        var existing = await _context.Skills.FindAsync(skill.Id);
        if (existing != null) { _context.Entry(existing).CurrentValues.SetValues(skill); await _context.SaveChangesAsync(); }
    }
    public async Task DeleteSkillAsync(Skill skill) {
        var existing = await _context.Skills.FindAsync(skill.Id);
        if (existing != null) { _context.Skills.Remove(existing); await _context.SaveChangesAsync(); }
    }
}