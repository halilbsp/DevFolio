using DevFolio.Web.Components;
using DevFolio.Infrastructure.Context;
using DevFolio.Application.Interfaces;
using DevFolio.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using DevFolio.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("DevFolio.Infrastructure")));

builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

var app = builder.Build();

// ----- SEED DATA (VERİTABANI BOŞSA ÖRNEK VERİ EKLE) 🌱 -----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Users.Any())
    {
        var testUser = new User { Email = "iletisim@halil.com", PasswordHash = "Sifre123", Role = "Admin" };
        var testPortfolio = new Portfolio {
            User = testUser, FullName = "Halil", JobTitle = "Kıdemli Full-Stack Geliştirici",
            Bio = "Modern web teknolojileri, mikroservis mimarileri ve ölçeklenebilir SaaS platformları üzerine odaklanmış Full-Stack Geliştirici.",
            ProfileImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBaXnf6IaW7mka5-KeXY8SFyjqbQ8G4TTFy_9MpsuaPIJjVtDZjWkmuACYEhiaOR1PwbF2R8qPAau1ZJ8KuQhgjEZ9ogxsakOnQUMegh-B3YnQlP8JnmAXy8RpUjVB2IPzJ-MGILaYW0OH31Y1RlI2JkW46Lsy8bACQrNo699neav7eRSHKkYJ-WN7aTHFX6oSu8QuN0dobM2FeglR-I8dLYoYlUXGKi6I4q08v0NG1rKJeXogy-LgxAu1oNi7LrsCC6et4n5PrsC8", StatusText = "Bulutlarda kodluyor"
        };
        testPortfolio.Skills.Add(new Skill { Name = ".NET 10", ColorClass = "text-secondary" });
        testPortfolio.Skills.Add(new Skill { Name = "Next.js", ColorClass = "text-primary" });
        testPortfolio.Skills.Add(new Skill { Name = "FastAPI", ColorClass = "text-secondary" });
        testPortfolio.Skills.Add(new Skill { Name = "PostgreSQL", ColorClass = "text-primary" });
        testPortfolio.Skills.Add(new Skill { Name = "Docker", ColorClass = "text-secondary" });
        
        testPortfolio.Projects.Add(new Project { Title = "B2B SaaS ERP Sistemi", Description = "Endüstriyel kullanım için geliştirilen, kurumsal seviyede çok kiracılı ERP platformu.", ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuD6GRzDJct74aUpBKmIK4oxJIq49dBXffYiaDbgPwKpZMVYdG3kRS7CZZMlJ81HR7gY6j5B6HJ4lqTNB7sWsyHvPVOeM5rH7KwkV2_4hXGUBZ5eD5cp18ClVoZy90Byv_edRfJrnsYW6sHGss2zPSvHS1Uah9-OUpeKT5aKFa5FjYic42_iudWlf-5vzdqnPpHO0s-eZlQwxtJw7Df7K7bBDJ1WMKC_svlkq0CbmJLqWPca3HAKB-DT2AvBH54bBXXh-IDKQraOVaU", Tags = "Next.js, FastAPI, PostgreSQL" });
        testPortfolio.Projects.Add(new Project { Title = "Randevum Online", Description = "Çevrimiçi randevu web siteleri için modern abonelik tabanlı SaaS mimarisi.", ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuC9r03jNoRWChJXFuROQk1a-XpA2PV-p0pI1Sc83s0KPJr0fjMSrdJUM0r9CMtp8s4Om2KiifGPjZZPDiJjBot2tVqLGMCTtq1-1XyXjkVnRTScbVsmrh-ZSxcBQe80briFr7kU2fGnXRo4Fj7V5j43ryX9f0PjDrKMb9kq-bcD9ZmFqkh5taEl_MgnBltWjn1LaKlPW0rZLbH_WpWav79NOFbjpNSK4Pj11aIVOPYXU1Rz8iW939QucV_bnu635cr3Z-DSg9C-Pak", Tags = "SaaS, Mimari" });
        
        context.Users.Add(testUser);
        context.Portfolios.Add(testPortfolio);
        context.SaveChanges(); // Veritabanına kaydet! 💾
    }
}
// ------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();