using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevFolio.Application.Interfaces;
using DevFolio.Domain.Entities;

namespace DevFolio.Application.Services;

public class ExportService
{
    private readonly IPortfolioRepository _portfolioRepo;

    public ExportService(IPortfolioRepository portfolioRepo)
    {
        _portfolioRepo = portfolioRepo;
    }

    public async Task<byte[]?> GeneratePortfolioZipAsync(string username, string webRootPath)
    {
        var portfolio = await _portfolioRepo.GetPortfolioByUsernameAsync(username);
        
        if (portfolio == null)
            return null;

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
        {
            // Resim URL'lerini zip içerisine çekmek için işliyoruz. HTML metinlerini üretirken `archive` üzerinden entry açacağız.
            var htmlContent = GenerateHtmlContent(portfolio, archive, webRootPath);

            // 1. Create index.html
            var entryHtml = archive.CreateEntry("index.html");
            using (var streamWriter = new StreamWriter(entryHtml.Open(), Encoding.UTF8))
            {
                await streamWriter.WriteAsync(htmlContent);
            }

            // 2. Create README.md (Usage Guide)
            var entryReadme = archive.CreateEntry("YAYINLAMA_REHBERI.txt");
            using (var streamWriter = new StreamWriter(entryReadme.Open(), Encoding.UTF8))
            {
                var readmeContent = GenerateReadme(portfolio);
                await streamWriter.WriteAsync(readmeContent);
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream.ToArray();
    }

    private string GenerateReadme(Portfolio p)
    {
        return $@"TEBRİKLER! PORTFOLYONUZ {p.FullName?.ToUpper()} TARAFINDAN DEVFOLIO İLE OLUŞTURULDU!
========================================================================

Bu klasör, DevFolio panelinde hazırladığınız profesyonel portfolyonuzun 
sıfır bağımlılıklı (Saf HTML + Tailwind CSS) kaynak dosyasıdır.
Cihazınıza hiçbir ek yazılım dahi kurmadan tamamen yayına hazır statik bir şablondur.

NASIL ÇALIŞTIRABİLİRİM?
------------------------------------------------------------------------
Sadece `index.html` dosyasına çift tıklayarak tarayıcınızda açmanız yeterlidir!
Hiçbir Node.js, .NET SDK veya React kurulumu (npm install vs.) GEREKTİRMEZ! 
Veritabanındaki metinleriniz, CSS sınıfları ve resimleriniz tamamen bu klasöre entegre edilmiştir.

NASIL YAYINLAYABİLİRİM? (DÜNYAYA AÇMAK)
------------------------------------------------------------------------
1. Netlify (netlify.com) veya Vercel (vercel.com) adlı ücretsiz modern sitelerde oturum açın.
2. 'Add New Site' -> 'Deploy manually' seçeneğine tıklayın.
3. Elinizdeki bu zip klasörünü (veya içindeki tüm dosyaları - assets klasörü dahil!) sürükleyin.
4. Saniyeler içinde ücretsiz hızlı siteniz yayınlanacaktır.

DevFolio Altyapısı - Profesyonel Mimarlar İçin Portfolyo Aracı";
    }

    private string ProcessImageUrl(string imageUrl, ZipArchive archive, string webRootPath)
    {
        if (string.IsNullOrEmpty(imageUrl)) return "";
        if (imageUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase)) return imageUrl;
        if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return imageUrl;

        // URL muhtemelen "/uploads/..." gibi bir yapıya sahip
        var cleanPath = imageUrl.TrimStart('/', '\\');
        var physicalPath = Path.Combine(webRootPath, cleanPath);

        if (File.Exists(physicalPath))
        {
            try 
            {
                var bytes = File.ReadAllBytes(physicalPath);
                var base64 = Convert.ToBase64String(bytes);
                var ext = Path.GetExtension(physicalPath).TrimStart('.').ToLower();
                if (ext == "jpg") ext = "jpeg";
                var mimeType = ext switch {
                    "png" => "image/png",
                    "webp" => "image/webp",
                    "gif" => "image/gif",
                    "svg" => "image/svg+xml",
                    _ => "image/jpeg"
                };

                return $"data:{mimeType};base64,{base64}"; // HTML'de doğrudan render edilmesi için
            }
            catch { return imageUrl; }
        }

        return imageUrl; // Eğer bulunamadıysa aynı bırak
    }

    private string GenerateHtmlContent(Portfolio p, ZipArchive archive, string webRootPath)
    {
        // Temel verilerin null kontrolü ile hazırlanması
        var bio = p.Bio ?? "Mükemmel kodlar yazan bir geliştirici.";
        var title = p.JobTitle ?? "Software Developer";
        var fullName = p.FullName ?? "DevFolio User";
        var email = p.User?.Email ?? "";
        
        string rawProfileImage = string.IsNullOrEmpty(p.ProfileImageUrl) 
            ? "https://lh3.googleusercontent.com/aida-public/AB6AXuBaXnf6IaW7mka5-KeXY8SFyjqbQ8G4TTFy_9MpsuaPIJjVtDZjWkmuACYEhiaOR1PwbF2R8qPAau1ZJ8KuQhgjEZ9ogxsakOnQUMegh-B3YnQlP8JnmAXy8RpUjVB2IPzJ-MGILaYW0OH31Y1RlI2JkW46Lsy8bACQrNo699neav7eRSHKkYJ-WN7aTHFX6oSu8QuN0dobM2FeglR-I8dLYoYlUXGKi6I4q08v0NG1rKJeXogy-LgxAu1oNi7LrsCC6et4n5PrsC8" 
            : p.ProfileImageUrl;
        var profileImageUrl = ProcessImageUrl(rawProfileImage, archive, webRootPath);

        // Yeteneklerin dinamik üretimi
        var skillsHtml = new StringBuilder();
        if (p.Skills != null && p.Skills.Any())
        {
            foreach (var skill in p.Skills)
            {
                skillsHtml.AppendLine($@"
                    <div class=""flex items-center gap-3 group"">
                        <span class=""{skill.ColorClass} font-headline font-bold text-xl md:text-2xl group-hover:scale-110 transition-transform"">{System.Net.WebUtility.HtmlEncode(skill.Name)}</span>
                        <div class=""h-px w-6 bg-outline-variant group-hover:w-12 transition-all""></div>
                    </div>");
            }
        }

        // Projelerin dinamik üretimi
        var projectsHtml = new StringBuilder();
        if (p.Projects != null && p.Projects.Any())
        {
            foreach (var project in p.Projects)
            {
                var tagsHtml = "";
                if (!string.IsNullOrEmpty(project.Tags))
                {
                    var tags = project.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var tag in tags) {
                        tagsHtml += $@"<span class=""px-3 py-1 text-xs font-medium rounded bg-transparent border border-primary/20 text-[#81ecff]"">{System.Net.WebUtility.HtmlEncode(tag.Trim())}</span>";
                    }
                }

                var finalProjectImageUrl = ProcessImageUrl(project.ImageUrl, archive, webRootPath);

                var imgTag = !string.IsNullOrEmpty(finalProjectImageUrl)
                    ? $@"<img class=""w-full h-full object-cover opacity-80 group-hover:opacity-100 group-hover:scale-105 transition-all duration-700"" src=""{finalProjectImageUrl}"" />"
                    : $@"<div class=""w-full h-full flex items-center justify-center bg-gradient-to-br from-surface-container-highest to-black""><span class=""material-symbols-outlined text-6xl text-on-surface-variant opacity-20"">deployed_code</span></div>";

                var gitBtn = !string.IsNullOrEmpty(project.GithubUrl) ? $@"<a href=""{project.GithubUrl}"" target=""_blank"" class=""hover:text-white transition-colors""><span class=""material-symbols-outlined text-[22px]"">code</span></a>" : "";
                var liveBtn = !string.IsNullOrEmpty(project.LiveUrl) ? $@"<a href=""{project.LiveUrl}"" target=""_blank"" class=""hover:text-white transition-colors""><span class=""material-symbols-outlined text-[22px]"">open_in_new</span></a>" : "";

                projectsHtml.AppendLine($@"
                <div class=""bg-[#131313] border border-white/5 rounded-2xl overflow-hidden group hover:border-white/10 hover:-translate-y-1 transition-all duration-500 shadow-xl"">
                    <div class=""h-56 overflow-hidden relative bg-black"">
                        {imgTag}
                        <div class=""absolute inset-0 bg-gradient-to-t from-[#131313] to-transparent opacity-60""></div>
                    </div>
                    <div class=""p-8"">
                        <div class=""flex justify-between items-start mb-4"">
                            <h3 class=""font-headline text-2xl font-bold text-white tracking-tight"">{System.Net.WebUtility.HtmlEncode(project.Title ?? "")}</h3>
                            <div class=""flex gap-3 text-on-surface-variant"">
                                {gitBtn} {liveBtn}
                            </div>
                        </div>
                        <p class=""text-on-surface-variant text-sm leading-relaxed line-clamp-2 mb-8 h-10"">
                            {System.Net.WebUtility.HtmlEncode(project.Description ?? "")}
                        </p>
                        <div class=""flex flex-wrap gap-2"">
                            {tagsHtml}
                        </div>
                    </div>
                </div>");
            }
        }
        else
        {
            projectsHtml.AppendLine($@"<div class=""col-span-full py-20 text-center border border-white/5 rounded-2xl bg-surface-container-low""><span class=""material-symbols-outlined text-4xl text-on-surface-variant mb-4"">construction</span><p class=""text-on-surface-variant text-lg"">Bu geliştirici henüz bir proje sergilemiyor.</p></div>");
        }

        // HTML Şablonunu Tamamla
        return $@"<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{System.Net.WebUtility.HtmlEncode(fullName)} | {System.Net.WebUtility.HtmlEncode(title)}</title>
    <!-- Tailwind CSS (CDN - No Compilation Required) -->
    <script src=""https://cdn.tailwindcss.com""></script>
    <!-- Google Fonts -->
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=Space+Grotesk:wght@500;700;900&display=swap"" rel=""stylesheet"">
    <!-- Google Material Icons -->
    <link href=""https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined"" rel=""stylesheet"" />
    
    <script>
        tailwind.config = {{
            darkMode: ""class"",
            theme: {{
                extend: {{
                    colors: {{
                        ""on-surface"": ""#ffffff"",
                        ""outline-variant"": ""#484847"",
                        ""error"": ""#ff716c"",
                        ""secondary"": ""#a68cff"",
                        ""surface-bright"": ""#2c2c2c"",
                        ""surface-container-lowest"": ""#000000"",
                        ""on-surface-variant"": ""#adaaaa"",
                        ""surface-dim"": ""#0e0e0e"",
                        ""primary"": ""#81ecff"",
                        ""surface-container-highest"": ""#262626"",
                        ""surface-container-high"": ""#20201f"",
                        ""surface-container"": ""#1a1a1a"",
                        ""primary-container"": ""#00e3fd""
                    }},
                    fontFamily: {{
                        ""headline"": [""Space Grotesk""],
                        ""body"": [""Inter""]
                    }}
                }}
            }}
        }}
    </script>
    <style>
        html {{ scroll-behavior: smooth; }}
        body {{ background-color: #0e0e0e; color: #ffffff; font-family: 'Inter', sans-serif; }}
        .material-symbols-outlined {{ font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }}
    </style>
</head>
<body class=""selection:bg-primary/30 selection:text-primary"">

    <nav class=""sticky top-0 z-50 flex justify-between items-center px-6 md:px-12 py-6 w-full max-w-screen-2xl mx-auto bg-surface-dim/80 backdrop-blur-xl border-b border-white/5"">
        <div class=""flex items-center gap-2"">
            <span class=""text-[#81ecff] font-black italic font-headline text-xl tracking-tighter"">DevFolio</span>
        </div>
        <div class=""hidden md:flex items-center space-x-8"">
            <a class=""font-headline uppercase tracking-widest text-sm text-primary border-b border-primary transition-all cursor-pointer"" onclick=""document.getElementById('projects').scrollIntoView({{behavior: 'smooth'}})"">Projeler</a>
            <a class=""font-headline uppercase tracking-widest text-sm text-neutral-400 hover:text-primary transition-all cursor-pointer"" onclick=""document.getElementById('skills').scrollIntoView({{behavior: 'smooth'}})"">Yetenekler</a>
            <a class=""font-headline uppercase tracking-widest text-sm text-neutral-400 hover:text-primary transition-all cursor-pointer"" onclick=""document.getElementById('contact').scrollIntoView({{behavior: 'smooth'}})"">İletişim</a>
        </div>
        <div class=""flex items-center gap-6"">
            <a href=""mailto:{email}"" class=""bg-gradient-to-br from-primary to-primary-container text-black font-headline font-bold px-6 py-2 rounded-lg hover:scale-105 transition-transform shadow-[0_0_20px_rgba(129,236,255,0.2)]"">
                İletişime Geç
            </a>
        </div>
    </nav>

    <main class=""max-w-screen-2xl mx-auto px-6 md:px-12 pb-32"">
        <section class=""min-h-[80vh] flex flex-col-reverse md:flex-row items-center justify-between gap-12 py-20"">
            <div class=""flex-1 space-y-8 z-10"">
                <div class=""inline-flex items-center px-3 py-1 rounded-full bg-surface-container-highest border border-outline-variant/20"">
                    <span class=""w-2 h-2 rounded-full bg-primary mr-2 animate-pulse""></span>
                    <span class=""text-xs font-bold uppercase tracking-widest text-on-surface-variant"">Yeni projelere açık</span>
                </div>
                <h1 class=""font-headline text-5xl md:text-7xl font-bold tracking-tight leading-[1.1] text-white"">
                    {System.Net.WebUtility.HtmlEncode(fullName)} <br/>
                    <span class=""text-transparent bg-clip-text bg-gradient-to-r from-primary to-secondary"">{System.Net.WebUtility.HtmlEncode(title)}</span>
                </h1>
                <p class=""text-on-surface-variant text-lg md:text-xl max-w-2xl leading-relaxed"">
                    {System.Net.WebUtility.HtmlEncode(bio)}
                </p>
                <div class=""flex flex-wrap gap-4 pt-4"">
                    <a class=""px-8 py-4 bg-primary text-black font-headline font-bold rounded-xl shadow-[0px_10px_30px_rgba(129,236,255,0.2)] hover:scale-[1.02] transition-transform cursor-pointer"" onclick=""document.getElementById('projects').scrollIntoView({{behavior: 'smooth'}})"">
                        Projeleri İncele
                    </a>
                </div>
            </div>
            <div class=""relative flex-1 flex justify-end items-center"">
                <div class=""relative w-72 h-72 md:w-[450px] md:h-[450px]"">
                    <div class=""absolute inset-0 bg-gradient-to-tr from-primary/30 to-secondary/30 rounded-[3rem] rotate-6 blur-3xl""></div>
                    <img class=""relative w-full h-full object-cover rounded-[2.5rem] border border-white/10 shadow-2xl grayscale hover:grayscale-0 transition-all duration-700"" src=""{profileImageUrl}"" />
                </div>
            </div>
        </section>

        <section id=""skills"" class=""py-16 border-y border-white/5 relative"">
            <div class=""absolute inset-0 bg-primary/5 blur-3xl rounded-full""></div>
            <div class=""relative flex flex-wrap justify-center gap-6 md:gap-12"">
                {skillsHtml}
            </div>
        </section>

        <section class=""py-32"" id=""projects"">
            <div class=""flex flex-col md:flex-row justify-between items-end mb-16 gap-4"">
                <div>
                    <h2 class=""font-headline text-4xl font-bold mb-4 text-white"">Öne Çıkan Projeler</h2>
                    <p class=""text-on-surface-variant max-w-xl text-lg"">Geliştirdiğim sistemler, mimariler ve canlıya alınan ürünler.</p>
                </div>
            </div>
            <div class=""grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8"">
                {projectsHtml}
            </div>
        </section>

        <section id=""contact"" class=""py-32 border-t border-white/5 text-center mt-12 bg-surface-container-lowest/50 rounded-3xl p-12 relative overflow-hidden"">
            <div class=""absolute inset-0 bg-gradient-to-t from-primary/5 to-transparent""></div>
            <h2 class=""font-headline text-4xl md:text-5xl font-bold mb-6 text-white relative z-10"">Birlikte Çalışalım</h2>
            <p class=""text-on-surface-variant text-lg md:text-xl max-w-2xl mx-auto mb-10 relative z-10"">Yeni projelerde işbirliği yapmak veya fikir alışverişinde bulunmak isterseniz bana ulaşabilirsiniz.</p>
            <a href=""mailto:{email}"" class=""inline-flex items-center gap-3 px-10 py-5 bg-white text-black font-headline font-bold text-xl rounded-xl hover:scale-105 transition-all relative z-10 shadow-[0_0_40px_rgba(255,255,255,0.2)]"">
                <span class=""material-symbols-outlined"">mail</span>
                E-Posta Gönder
            </a>
        </section>
    </main>
</body>
</html>";
    }
}
