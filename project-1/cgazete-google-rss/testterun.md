# Google AI RSS – Development Modunda Test

Development modunda test etmek doğru yoldur; test sunucusundaki Hangfire'ı **Production yapmayın**.

## Production yapınca site neden bozuluyor?

Ortam değişince Hangfire tamamen farklı sunuculara bağlanır:

| | Development (`appsettings.Development.json`) | Production (`appsettings.json`) |
|---|---|---|
| SQL (haber + Hangfire) | 192.168.35.**40** | 192.168.35.**35** |
| Elasticsearch | 192.168.35.**38** | 192.168.35.**34** |
| Redis | 192.168.35.**39** | 192.168.35.**33** |
| Index adlarının başı | `dev_` | `prod_` |

Test sunucusundaki Hangfire Production yapılınca test kodu canlı veritabanında, canlı Elasticsearch'te ve canlı Redis'te iş yapmaya başlar (haber yayınlama, kilit kaldırma, önbellek işleri vb.). Sorunların nedeni budur. Development ise tamamen ayrı test altyapısını kullandığı için güvenlidir.

## Development modunda sonuç almak için gerekenler

### 1. Hangfire ve Web'i birlikte yayınlayın (en önemlisi)

Google AI RSS önbellek index adı ortamdan bağımsız hale getirildi (`google_ai_rss`, bkz. `Quark.Business.ElasticSearch\Constants\IndexNames.cs`). Eski kod `dev_google_ai_rss` kullanıyordu.

Yalnızca biri yayınlanırsa biri yeni index'e yazar, diğeri eskisinden okur ve site ile XML dosyası farklı görünür. Test sitesindeki **Hangfire, Web ve CMS'i aynı derlemeyle** yayınlayın.

### 2. Aynı anda tek bir Hangfire çalışsın

Visual Studio'dan Hangfire Development profiliyle başlatılırsa, test sunucusundaki Hangfire ile aynı Hangfire veritabanını (.40 / `News-Hangfire`) kullanır. İki sunucu aynı işleri paylaşır ve hangi kodun çalıştığı bilinemez. Yerelde test ederken test sunucusundaki Hangfire'ın uygulama havuzunu durdurun veya sadece yayınlanmış sürümü kullanın.

### 3. Ağ ve klasör erişimi

- 192.168.35.38, .39 ve .40 adreslerine erişim olmalı.
- `E:\GitHub\cgazete-google-rss\GoogleAiFeedArchive` klasörüne yazılabilmeli.

### 4. Test verisi

.40 veritabanında, son 48 saat içinde `AllowToSendRSS` ve `AllowGoogleAiFeed` işaretli, yayınlanmış bir haber veya köşe yazısı olmalı. Yoksa akış boş gelir. Test CMS'inden bir haber yayınlanabilir.

## Test adımları

1. Yeni derlemeyi test sunucusundaki **Hangfire, Web ve CMS**'e yayınlayın. Hangfire `web.config` içindeki `ASPNETCORE_ENVIRONMENT` değeri **Development** kalsın.
2. Hangfire panelini açın (`/hangfire`, kullanıcı bilgileri `DashboardUser` ayarında). **Recurring Jobs** altında **Refresh Google AI RSS** işini **Trigger now** ile çalıştırın ve **Succeeded** listesine düştüğünü görün.
3. Doğrulama betiğini çalıştırın:

   ```powershell
   .\scripts\Test-GoogleAiRss.ps1 -Url "https://yenisitetest.cumhuriyet.com.tr/feeds/google-ai.xml" -Username google-ai-rss -Password "<şifre>"
   ```

4. Beklenen sonuç:
   - **0 hata.**
   - Haber yayınlandıysa: "Endpoint (cache) ile gunluk dosya ayni haberleri iceriyor".
   - Elasticsearch .38'deki `google_ai_rss` belgesinin `lastBuildDate` değeri her dakika güncellenir.

## Dosya ve önbellek konumları

- Günlük akış: `E:\GitHub\cgazete-google-rss\GoogleAiFeedArchive\<yıl>\feed-yyyy-MM-dd.xml` (her gün ayrı dosya, gün içinde yeniden yazılır)
- Kilit dosyası: `...\GoogleAiFeedArchive\google-ai.lock`
- Hata günlüğü: `...\GoogleAiFeedArchive\logs\google-ai-errors-yyyy-MM-dd.txt`
- Önbellek: Elasticsearch `google_ai_rss` index'i, belge ID `2`

## Canlıya geçiş notu

Canlıya geçerken Hangfire'ı Production yapmak gerekmez; Google AI RSS index adı artık ortamdan bağımsızdır. Ancak canlıdaki Hangfire'ın hangi Elasticsearch'e yazdığı, yayınlanan `appsettings` dosyalarından kontrol edilmelidir. Web ile Hangfire aynı Elasticsearch sunucusunu kullanmalıdır.
