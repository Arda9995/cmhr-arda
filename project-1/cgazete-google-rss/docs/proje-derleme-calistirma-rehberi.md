# AvvaMobile.News Çözümü — Proje Yapısı, Derleme ve Çalıştırma Rehberi

## Çalıştırılabilir Projeler (Run edildiğinde koşanlar)

Çözümde yalnızca aşağıdaki 5 proje çalıştırılabilir (web/host uygulaması); diğerleri sınıf kütüphanesidir ve kendi başına koşmaz.

| Proje | Rolü |
|---|---|
| **Quark.Web** | Son kullanıcı sitesi (haber sitesi) |
| **Quark.CMS** | Yönetim paneli (editörler) |
| **Quark.API** | API host — açılışta `LoadAllSettingsToCache` ile ayarları Redis'e yükler |
| **Quark.Hangfire** | Arka plan işleri (cache yenileme vb.) — açılışta ayarları Redis'e yükler |
| **Quark.Hangfire.Api** | Hangfire işlerini tetikleyen API |

### Başlangıç projesi
- Solution Explorer'da **kalın (bold)** görünen proje başlangıç projesidir; Run (F5) sadece onu başlatır.
- Birden çok projeyi birlikte koşturmak için: Solution'a sağ tık → **Configure Startup Projects** → **Multiple startup projects** → `Quark.Web`, `Quark.API`, `Quark.Hangfire` için *Start* seçin.

## Sadece Derlenen (Koşmayan) Kütüphaneler

`Quark.Core`, `Quark.Data`, `Quark.Resources`, `Quark.Business.Common`, `Quark.Business.Caching`,
`Quark.Business.ElasticSearch`, `Quark.API.Business`, `Quark.CMS.Business`, `Quark.Hangfire.Business`,
`Quark.Cloudflare.Adapter`, `Quark.Notification.Adapter`, `Quark.Subscription.Adapter`

## Quark.Web Derlendiğinde Hangi Projeler Derlenir?

### Doğrudan referanslar
- Quark.API.Business
- Quark.Business.Common
- Quark.Business.ElasticSearch
- Quark.Data

### Dolaylı (transitif) referanslar
- Quark.Business.Caching ← Quark.API.Business üzerinden (`NewsCachingService` burada)
- Quark.Subscription.Adapter ← Quark.API.Business üzerinden
- Quark.Notification.Adapter ← Quark.API.Business üzerinden
- Quark.Cloudflare.Adapter ← Quark.Business.Caching üzerinden
- Quark.Core ← Quark.Business.Common üzerinden (`StringExtensions` burada)
- Quark.Resources ← Quark.Core üzerinden

Toplam: **Quark.Web + 10 bağımlı proje** derlenir.

### Quark.Web derlemesine dahil olmayanlar
- Quark.CMS, Quark.CMS.Business
- Quark.API (host projesi)
- Quark.Hangfire, Quark.Hangfire.Api, Quark.Hangfire.Business

## Proje Referans Grafiği

```
Quark.Web      -> Quark.API.Business, Quark.Business.Common, Quark.Business.ElasticSearch, Quark.Data
Quark.API      -> Quark.API.Business, Quark.Business.Caching, Quark.Business.ElasticSearch, Quark.Notification.Adapter
Quark.CMS      -> Quark.CMS.Business, Quark.Notification.Adapter
Quark.Hangfire -> Quark.API.Business, Quark.Business.Common, Quark.Hangfire.Business
Quark.Hangfire.Api -> Quark.Hangfire.Business

Quark.API.Business      -> Quark.Business.Caching, Quark.Business.Common, Quark.Notification.Adapter, Quark.Subscription.Adapter
Quark.CMS.Business      -> Quark.Business.Caching, Quark.Business.Common, Quark.Business.ElasticSearch, Quark.Cloudflare.Adapter, Quark.Notification.Adapter
Quark.Hangfire.Business -> Quark.Business.Caching, Quark.Business.Common, Quark.Cloudflare.Adapter, Quark.CMS.Business
Quark.Business.Caching  -> Quark.Business.Common, Quark.Business.ElasticSearch, Quark.Cloudflare.Adapter
Quark.Business.ElasticSearch -> Quark.Business.Common, Quark.Core, Quark.Data
Quark.Business.Common   -> Quark.Core, Quark.Data
Quark.Cloudflare.Adapter -> Quark.Business.Common
Quark.Notification.Adapter -> Quark.Business.Common
Quark.Core              -> Quark.Resources
```

## Önemli Not — `Common_ClientWebSite` Ayarı

- `Common_ClientWebSite` değeri `appsettings.json`'dan **gelmez**; veritabanındaki **`AppSettings` tablosundan** okunur ve uygulama açılışında Redis'e yüklenir.
- Bu yükleme (`LoadAllSettingsToCache`) yalnızca **Quark.API** ve **Quark.Hangfire** açılışında çalışır.
- DB'de bu ayarı düzelttiyseniz, değişikliğin etkili olması için `Quark.API` veya `Quark.Hangfire`'ı en az bir kez çalıştırın (veya Redis'teki `Common_ClientWebSite` anahtarını elle güncelleyin). Sadece `Quark.Web`'i çalıştırmak yeterli olmaz.
