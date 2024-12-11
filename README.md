# Envanter Yönetim Sistemi

## 🚀 Proje Özeti

Bu proje, envanter yönetimi için geliştirilmiş bir API'dir. Kullanıcı kaydı, e-posta doğrulama, rol yönetimi, brute force saldırılarına karşı hız sınırlandırma, ve daha fazlasını içeren işlevler sunar. Proje, kullanıcıların ürünleri yönetebilmesini, kendi hesaplarını yönetebilmesini ve sistemdeki verileri güvenli bir şekilde işlemesini sağlar.

## 🛠 Kullanılan Teknolojiler

- **Backend Framework**: .NET 9.0
- **Web Framework**: ASP.NET Core (RESTful API)
- **Veritabanı ORM**: Entity Framework Core
  - Desteklenen Veritabanları:
    - PostgreSQL
    - MongoDB
    - Redis Entegrasyonu

### Güvenlik ve Kimlik Doğrulama
- **Kimlik Doğrulama**: JWT (JSON Web Tokens)
- **Şifre Güvenliği**: BCrypt.Net
- **Hız Sınırlandırma**: IP bazlı rate limiting

### Diğer Teknolojiler
- **Loglama**: Serilog
- **Doğrulama**: FluentValidation
- **Test Araçları**: 
  - XUnit
  - Moq

## ✨ Temel Özellikler

### 1. Kullanıcı Yönetimi
- **Kayıt İşlemi**
  - Güvenli kullanıcı kaydı
  - Bcrypt ile şifre hashleme
- **Giriş İşlemi**
  - JWT token bazlı kimlik doğrulama
  - Rol tabanlı erişim kontrolü

### 2. E-posta Doğrulama
- Otomatik doğrulama e-postaları
- Hesap aktivasyon bağlantıları

### 3. Rol Yönetimi
- Admin seviyesinde rol atama
- Detaylı izin kontrolleri
- "Viewer" haricinde özel roller atama

### 4. Güvenlik Önlemleri
- IP bazlı giriş deneme sınırlandırması
- Brute force saldırılarına karşı koruma
- Merkezi hata yönetimi

## 🏗️ Proje Mimarisi

### Katmanlı Mimari Yapısı

#### 1. Domain Katmanı
- Temel varlıklar ve iş mantığı
- Kullanıcı, Ürün ve Kategori modelleri

#### 2. Application Katmanı
- Servis implementasyonları
- İş kuralları
- **Alt Servisler**:
  - UserService
  - CategoryService
  - ProductService

#### 3. Infrastructure Katmanı
- Dış servis entegrasyonları
- E-posta servisi
- Repository arayüzleri ve implementasyonları

#### 4. API Katmanı
- Controller'lar
- Endpoint tanımlamaları
- **İşlem Kontrolleri**:
  - Kullanıcı işlemleri
  - Ürün yönetimi
  - Kategori yönetimi

## 📦 Veritabanı Migrasyonları

### Migrasyon Komutları

```bash
# Yeni migrasyon ekle
dotnet ef migrations add MigrationName

# Veritabanını güncelle
dotnet ef database update

# Son migrasyonu kaldır
dotnet ef migrations remove
```

## 🔧 Yapılandırma

Proje yapılandırması `appsettings.json` dosyasında yönetilir.

### Örnek Yapılandırma

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=InventoryDB;Username=postgres;Password=password",
    "Redis": "localhost:6379"
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "InventoryManagement"
  },
  "Jwt": {
    "Key": "MySuperSecretKey12345",
    "Issuer": "MyApp",
    "Audience": "MyAppUsers"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🚀 Projeyi Çalıştırma

### Uygulamayı Başlatma
```bash
dotnet run
```
Proje `http://localhost:5000` üzerinden erişilebilir.

### Testleri Çalıştırma
```bash
dotnet test
```

## 🧪 Test Stratejisi

- Kapsamlı birim testleri
- Entegrasyon testleri
- Test araçları: XUnit ve Moq
- Test edilen özellikler:
  - Kullanıcı kaydı
  - E-posta doğrulama
  - Rol yönetimi

