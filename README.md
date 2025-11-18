# 🚀 ShortLink - URL Shortener & DevOps Project

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Container-blue?style=for-the-badge&logo=docker)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-336791?style=for-the-badge&logo=postgresql)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=github-actions)
![Portainer](https://img.shields.io/badge/Orchestration-Portainer-00bbff?style=for-the-badge&logo=portainer)

**ShortLink**, ASP.NET Core 8 MVC mimarisi kullanılarak geliştirilmiş, Docker üzerinde koşan, tam otomatik CI/CD süreçlerine sahip ve kendi sunucumda (Self-Hosted) yayınladığım modern bir URL kısaltma servisidir.

Bu proje, sadece bir web uygulaması değil; **Network yönetimi, Container orkestrasyonu, SSL güvenliği ve DNS yapılandırmasını** içeren uçtan uca bir DevOps çözümüdür.

---

## 🏗️ Mimari & Teknoloji Yığını (Tech Stack)

Bu projede **Modern DevOps** pratikleri ve **Mikroservis** yaklaşımına uygun bir yapı kurulmuştur.

### 💻 Backend & Database
* **Framework:** ASP.NET Core 8.0 (MVC & Entity Framework Core)
* **Veritabanı:** PostgreSQL 15 (Code-First Approach & Auto-Migrations)
* **Güvenlik:** ASP.NET Core Identity (Kullanıcı Yönetimi & Auth)
* **Veri Güvenliği:** HTTPS Zorunluluğu & Secure Headers

### ⚙️ DevOps & Altyapı
* **Containerization:** Docker & Docker Compose (Multi-Stage Build ile optimize edilmiş imajlar).
* **CI/CD Pipeline:** GitHub Actions (Push anında otomatik build ve Docker Hub'a dağıtım).
* **CD (Sürekli Dağıtım):** Watchtower (Sunucudaki konteynerlerin güncellemeyi otomatik algılayıp kendini yenilemesi).
* **Orkestrasyon:** Portainer (ASUSTOR NAS üzerinde yönetim).

### 🌐 Network & Güvenlik
* **Reverse Proxy:** Nginx tabanlı ASUSTOR Reverse Proxy.
* **SSL/TLS:** Let's Encrypt (Otomatik yenilenen sertifikalar).
* **DNS Yönetimi:** AdGuard Home (Split-Horizon DNS ile NAT Loopback sorunu çözümü ve yerel ağ optimizasyonu).

---

## 🔄 CI/CD Akışı (Nasıl Çalışıyor?)

Bu proje, kod değişikliğinden canlı ortama (Production) kadar **sıfır manuel müdahale** ile çalışacak şekilde tasarlanmıştır.

1.  **Development:** Visual Studio üzerinde kod geliştirilir ve `main` dalına `push` edilir.
2.  **Continuous Integration (CI):** GitHub Actions tetiklenir; projeyi derler, test eder ve Docker imajını oluşturur.
3.  **Registry:** Oluşan imaj etiketlenerek **Docker Hub**'a yüklenir.
4.  **Continuous Deployment (CD):** Sunucuda (NAS) çalışan **Watchtower** servisi, Docker Hub'daki değişikliği algılar.
5.  **Update:** Eski konteyneri durdurur, yeni imajı çeker ve aynı konfigürasyonlarla (Veri kaybı olmadan) sistemi yeniden başlatır.

---

## 📸 Proje Özellikleri

* ✅ **Kullanıcı Sistemi:** Kayıt ol, Giriş yap, Şifremi Unuttum (Identity).
* ✅ **Dashboard:** Kişisel linklerin listelenmesi, tarih ve tıklanma sayıları.
* ✅ **CRUD İşlemleri:** Link oluşturma, listeleme ve güvenli silme.
* ✅ **Akıllı Yönlendirme:** Kısa linke tıklandığında istatistiği artırıp hedefe yönlendirme.
* ✅ **Güvenli Erişim:** `https://yilmaz.wtf` üzerinden SSL ile şifreli iletişim.

---

## 🚀 Kurulum (Local Development)

Projeyi kendi bilgisayarınızda çalıştırmak için:

```bash
# Repoyu klonlayın
git clone [https://github.com/guvenchemy/ShortLink.git](https://github.com/guvenchemy/ShortLink.git)

# Dizin değiştirin
cd ShortLink

# Docker Compose ile ayağa kaldırın (Veritabanı dahil)
docker-compose up -d