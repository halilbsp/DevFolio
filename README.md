# 🚀 DevFolio | SaaS Portfolio Generator

![.NET](https://img.shields.io/badge/-NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/-Blazor-5C2D91?style=for-the-badge&logo=blazor&logoColor=white)
![EF](https://img.shields.io/badge/-EFCore-0078D4?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Tailwind](https://img.shields.io/badge/-Tailwind-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)
![Docker](https://img.shields.io/badge/-Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**DevFolio**, yazılımcıların dijital kimliklerini saniyeler içerisinde oluşturup yönetebildikleri, .NET ve Clean Architecture prensipleriyle inşa edilmiş kurumsal seviyede bir SaaS platformudur. 🛠️✨

Sıradan bir barındırma hizmetinden farklı olarak DevFolio, geliştiricilere **kendi kendine yetebilen, sıfır bağımlılıklı statik web siteleri** üretme gücü sunar. 🌍💻

---

## ✨ "Killer Feature": Zero-Dependency Code Export

DevFolio'nun kalbinde yer alan benzersiz dışa aktarma (Export) motoru, veritabanındaki dinamik JSON verilerini anlık olarak işler ve tamamen bağımsız bir statik web sitesi üretir. ⚙️🚀

* 📦 **NPM veya SDK Yok:** Çıktı, Node.js veya herhangi bir sunucu bağımlılığı gerektirmez.
* 🖼️ **Base64 Gömülü Görseller:** Çevrimdışı (offline) çalışabilmesi ve resim linklerinin kırılmaması için tüm görseller `Base64 Encoding` ile doğrudan `index.html` içerisine şifrelenerek gömülür. 🔒
* ⚡ **Minimal API & ZIP Arşivleme:** Kullanıcı "Kodu İndir" dediğinde, C# Minimal API üzerinden bir ZIP arşivi oluşturulur. İster Vercel'e yükle, ister USB bellekten çalıştır! 💾

---

## 🏗️ Mimari & Teknoloji Yığını (Tech Stack)

Proje, sürdürülebilirlik ve yüksek performans (High Availability) hedefiyle **Clean Architecture** kurallarına uygun olarak katmanlandırılmıştır. 🏛️

* **Frontend:** Blazor (InteractiveServer), HTML5, Tailwind CSS 🎨
* **Backend:** .NET 10 (C#), ASP.NET Core 🧠
* **Database:** Entity Framework Core, SQL Server 🗄️
* **Infrastructure & DevOps:** Docker, Docker Compose (İzole ve hızlı kurulum için konteyner mimarisi) 🐳
* **Performance:** `IDbContextFactory` kullanılarak Blazor Server üzerindeki DbContext Eşzamanlılık (Concurrency) kilitlenmeleri tamamen izole edilmiş, Multi-Tenant yapıya uygun Thread-Safe bir veri erişimi sağlanmıştır. 🛡️🔥

---

## ⚙️ Temel Özellikler

- 👑 **Super Admin Console:** Gerçek zamanlı sistem metrikleri, global proje/yetenek havuzu denetimi ve anlık kullanıcı yönetimi. 📈
- 🧑‍💻 **Dinamik Geliştirici Dashboard'u:** "Dark Terminal" ve "Minimal" tema seçenekleriyle kişiselleştirilebilir portfolyo editörü. 🎨
- 🔗 **Özel URL Routing:** Her kullanıcı için anında oluşturulan `devfolio.com/{kullaniciadi}` public portfolyo erişimi. 🌐
- 📸 **Güvenli Medya Yönetimi:** Guid tabanlı resim yükleme ve boyutlandırma optimizasyonu. 🖼️

---

## 🛠️ Kurulum & Çalıştırma (Local Setup)

Projeyi kendi bilgisayarınızda çalıştırmak için iki farklı yöntem kullanabilirsiniz: 💻👇

### 🐳 Seçenek 1: Docker ile Hızlı Kurulum (Önerilen)
Bilgisayarınızda .NET SDK veya SQL Server kurulu olmasına gerek yok! Sadece Docker yüklüyse şu tek komutla tüm sistemi (Veritabanı + Web App) ayağa kaldırabilirsiniz:

```bash
docker-compose up --build -d
```

### 🖥️ Seçenek 2: Manuel Kurulum
1. **Repoyu Klonlayın:**

```bash
git clone https://github.com/kullaniciadin/DevFolio.git
cd DevFolio
```

2. **Veritabanı Bağlantısını Ayarlayın:**

appsettings.json içerisindeki DefaultConnection stringini kendi yerel SQL Server ayarlarınıza göre güncelleyin. 🔌

4. **Migration İşlemlerini Uygulayın:***

```bash
dotnet ef database update
```

## 🤝 Katkıda Bulunma (Contributing)
Bu proje açık kaynaklıdır ve her türlü pull request (PR) veya issue bildirimine açıktır. Clean Architecture ve Blazor Best Practice'lerine uygun geliştirmeler yapmaktan mutluluk duyarız! 💡🙌
