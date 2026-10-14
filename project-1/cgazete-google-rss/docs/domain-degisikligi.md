# Domain Değişikliği

## Proje özeti

Cumhuriyet haber sistemi şu uygulamalardan oluşur:

- **Quark.CMS**: yönetim paneli
- **Quark.Hangfire**: arka plan işleri
- **Quark.Web**: web sitesi
- **Quark.API**: API

Hepsi aynı SQL Server veritabanını, Redis'i ve Elasticsearch'ü kullanır.

Domain bilgisi appsettings dosyalarında değil, veritabanında `[AppSettings.Keys]` tablosunda tutulur. Uygulamalar bu değerleri Redis'e yükleyip oradan okur.

## Domain içeren ayarlar

| Key | Örnek değer | Ne için kullanılıyor |
|---|---|---|
| `Common_ClientWebSite` | `https://www.cumhuriyet.com.tr/` | Haber linkleri, RSS, sitemap, Google AI feed |
| `CDN_BaseUrl` | `https://media.cumhuriyet.com.tr/` | Görsel adresleri |
| `Common_CMSAppUrl` | `https://cms.cumhuriyet.com.tr` | CMS adresi |
| `Tags_PrefixUrl` | `https://www.cumhuriyet.com.tr/haberleri/` | Etiket sayfası adresi |
| `Pages_Body_End` | HTML içinde domain geçiyor | Sayfa sonuna eklenen kod |

## Domain nasıl değiştirilir

### 1. Tercih edilen yol: CMS uygulama ayarları ekranı

Değeri CMS'teki uygulama ayarları ekranından değiştirip kaydedin. CMS değeri kaydedip önbelleği kendisi yeniler. `Common_ClientWebSite` ekranda "Web Sitesi Adresi" etiketiyle görünür.

### 2. SQL ile

```sql
UPDATE [AppSettings.Keys] SET [Value] = 'https://yeni-domain.com/' WHERE [Key] = 'Common_ClientWebSite';
UPDATE [AppSettings.Keys] SET [Value] = 'https://yeni-domain.com/haberleri/' WHERE [Key] = 'Tags_PrefixUrl';
```

- Değerlerin sonundaki `/` işaretini koruyun; linkler bu işarete göre birleştirilir.
- SQL ile değiştirince Redis'teki eski değer kalır. Önbelleği CMS'ten yenileyin ya da uygulamaları yeniden başlatın. Hangfire ve CMS açılışta ayarları önbelleğe yeniden yükler.

### 3. Feed ve sitemap'leri yenileyin

Elasticsearch'te kayıtlı feed'ler eski domainle oluşturulmuştur.

- Google AI feed dakikalık Hangfire işiyle kendiliğinden yenilenir.
- Diğer RSS ve sitemap'ler kendi zamanlanmış işlerini bekler. Hemen yenilensin isterseniz CMS'teki önbellek yenileme ekranından tetikleyin.

## Ayarlardan okunmayan, kodda sabit değer

`Quark.Business.Caching/Services/XmlFeedCachingService.cs` içindeki `GoogleAiFeedSelfUrl` koda yazılıdır:

```
https://www.cumhuriyet.com.tr/feeds/google-ai.xml
```

Bu değer, feed'in kendi adresini bildiren `atom:link` etiketinde kullanılır. Domain değişince güncellenmez; kodda elle değiştirilmesi ya da `Common_ClientWebSite` ayarından üretilecek şekilde düzenlenmesi gerekir.
