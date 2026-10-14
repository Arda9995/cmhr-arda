# CMS Sistem Ayarları — Redis Otomatik Yenileme

## Özet

Yönetim panelindeki **Sistem Ayarları** ekranından bir ayar kaydedildiğinde Redis cache **otomatik olarak yenilenir**. Elle SQL güncellemesi veya manuel Redis yenileme gerekmez.

## Kod Doğrulaması

```csharp
// Quark.CMS.Business\Services\AppSettingsService.cs → Update(List<AppSettingsGroup>)
await _context.SaveChangesAsync();             // 1) DB'deki AppSettings tablosu güncellenir
await _appSettingsCachingService.RefreshAll(); // 2) Tüm anahtarlar Redis'e yeniden yazılır
```

`RefreshAll()` metodu (`Quark.Business.Caching\Services\AppSettingsCachingService.cs`), `AppSettings` tablosundaki tüm kayıtları okuyup her anahtarı Redis'e yazar. Web, CMS ve Hangfire aynı Redis'i kullandığından değişiklik üç uygulamaya birden yansır.

## Canlı Domain Değişikliği İçin Adımlar

1. **CMS → Sistem Ayarları** ekranını açın.
2. `Common_ClientWebSite` anahtarını bulun ve değerini `https://www.cumhuriyet.com.tr/` yapın.
   - ⚠️ Sondaki `/` önemli — birleştirme `baseUrl + slug.TrimStart('/')` şeklinde yapılır.
3. **Kaydet** — veritabanı güncellenir ve Redis otomatik yenilenir. ✅

## Tek Kalan Adım: İçerik Cache'leri

Redis'teki `Common_ClientWebSite` değeri anında güncellenir; ancak haber linkleri (`NewsSlug`, `CategorySlug`, `TagSlug` vb.) daha önce üretilmiş **içerik cache'lerinin içine gömülü** olduğundan eski `localhost` linkleri bir süre daha görünebilir. Çözüm:

- CMS'teki **cache yönetim ekranından** (`CachesController`) News / Block / Category cache'lerini yenileyin,
- **veya** Hangfire refresh job'larının çalışmasını bekleyin (sitemap her 3 dakikada bir, bazı listeler saatte bir yenilenir).

## Sonuç

Sistem Ayarları ekranından domain değişikliği + içerik cache yenilemesi sonrasında, anasayfa ve kategori sayfalarındaki tüm resim/başlık linkleri `https://www.cumhuriyet.com.tr/...` olarak üretilir ve haber detayına doğru yönlendirir.
