# Canlı Domain Geçişi — BaseURL / Common_ClientWebSite Yapılandırması

## Özet

Anasayfa ve kategori sayfalarındaki haber linkleri (resim + başlık) **mutlak URL** olarak üretilir. Bu URL'lerin temelini oluşturan domain, kod veya `appsettings.json` dosyasında değil, **veritabanı → Redis** zincirinde tanımlıdır.

## Nasıl Çalışıyor

1. Veritabanındaki `AppSettings` tablosunda `Key = 'Common_ClientWebSite'` kaydı domain değerini tutar.
2. `AppSettingsCachingService.RefreshAll()` bu tabloyu okuyup tüm anahtarları **Redis cache'e** yazar:

```csharp
// Quark.Business.Caching\Services\AppSettingsCachingService.cs
var appSettings = await (from ap in _context.AppSettings ...).ToListAsync();
foreach (var item in appSettings)
	await _cache.Set(item.Key, item.Value);   // Redis'e yazar
```

3. `AppSettingsKeys.Common_ClientWebSite` bu Redis anahtarını okur.
4. Link üretimi cache oluşturma anında yapılır ve URL'ler cache içine **gömülür**:
   - `NewsCachingService.cs` → `NewsSlug = Common_ClientWebSite + news.Slug` (satır 2067 vb.)
   - `BlockCachingService.cs:97` → `PrepareWebsiteUrl(...)`
   - `XmlFeedCachingService.cs` → sitemap/RSS linkleri
5. View'larda doğrudan kullanılır:
   - `Views\Shared\BlockContainers\_CategoryNews.cshtml` → `<a href="@news.NewsSlug">`
   - `Views\Category\NewsListPartials\_TopAndMidCategoryNews.cshtml`, `_BottomNews.cshtml` → `<a href="@news.Slug">`

Web, CMS ve Hangfire **aynı veritabanını ve aynı Redis'i** (`192.168.35.33:6390`) kullandığından domain tek yerden yönetilir; **Hangfire için ayrı bir ayar gerekmez**.

## Canlıya Geçiş Adımları

### 1. Veritabanını güncelle

```sql
UPDATE AppSettings
   SET Value = 'https://www.cumhuriyet.com.tr/'
   WHERE [Key] = 'Common_ClientWebSite';
```

> ⚠️ Sondaki `/` önemli — birleştirme `baseUrl + slug.TrimStart('/')` şeklinde yapılır.

### 2. Redis cache'ini yenile

CMS'teki cache yönetim ekranından AppSettings cache'ini yenileyin (`CachesController` → `RefreshAll`). Bu, DB'deki yeni değeri Redis'e yazar.

### 3. İçerik cache'lerini yeniden ürettir

Linkler (`NewsSlug`, `CategorySlug`, `TagSlug` vb.) cache üretim anında gömüldüğü için eski `localhost` linkleri mevcut cache'lerde kalmış olabilir:

- CMS cache ekranından News / Block / Category cache'lerini yenileyin,
- veya ilgili Hangfire refresh job'larının bir kez çalışmasını bekleyin (ör. sitemap her 3 dakikada bir yenilenir).

### 4. Hangfire — ekstra yapılandırma gerekmez

Job'lar aynı `AppSettingsKeys` üzerinden aynı Redis'ten okur. Tek koşul: Hangfire `appsettings.json` içindeki `RedisConnectionString` ve veritabanı bağlantısının Web/CMS ile aynı olması (mevcut durumda aynı).

## Sonuç

Domain tanımı **DB → Redis** zincirindedir. SQL güncellemesi + cache yenileme yeterlidir; sonrasında anasayfa ve kategori sayfalarındaki resim/başlık linkleri `https://www.cumhuriyet.com.tr/...` olarak üretilir ve haber detayına doğru şekilde yönlendirir.
