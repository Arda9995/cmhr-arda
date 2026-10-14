# Foto Galeri Haberleri — Resim Değiştirme Sorunu Düzeltmesi

## Sorun

Foto galeri haberleri düzenlenirken mevcut bir galeri öğesinin resmi değiştirilemiyordu. İki hata birlikte bu soruna yol açıyordu.

## Bulunan Hatalar ve Yapılan Düzeltmeler

### 1. `Quark.CMS\wwwroot\Scripts\News\Details.js` — Güncelleme AJAX çağrısı

- **Hata:** Yeni öğe oluşturma (`CreatePhotoGalleryItem`) doğru şekilde `FormData` kullanırken, güncelleme (`UpdatePhotoGalleryItem`) dosyayı düz bir JS nesnesinde `photo: file` olarak gönderiyordu.
  - Dosya yüklemek için `FormData` + `processData: false` / `contentType: false` zorunludur.
  - Controller'ın beklediği parametre adı `file` iken `photo` gönderiliyordu.
- **Sonuç:** Sunucuya dosya hiç ulaşmıyor, `photo` parametresi `null` geliyor ve resim güncellenmiyordu.
- **Düzeltme:** Güncelleme çağrısı da create işlemiyle aynı şekilde `formData` gönderecek biçimde değiştirildi:

```javascript
} else {
	$.ajax({
		type: "POST",
		url: "/News/UpdatePhotoGalleryItem/",
		data: formData,
		processData: false,  // Important
		contentType: false,  // Important
		success: function (response) {
			ReloadToPhotoGalleryFunc(newsId);
		},
		error: function (err) {
			ShowError(err.message);
		}
	});
}
```

### 2. `Quark.CMS.Business\Services\NewsService.cs` — `UpdatePhotoGalleryItem`

- **Hata:** Metot `content` parametresini alıyor ancak kayda hiç atamıyordu; yalnızca `PhotoUrl` güncelleniyordu.
- **Düzeltme:** Kaydetmeden önce içerik ataması eklendi:

```csharp
photoGalleryItem.Content = content;
```

## Doğrulama

- `Quark.CMS.Business` projesi başarıyla derlendi. ✅

## Notlar

- `NewsPhotoGalleryItem` modelinde `Subtitle` alanı bulunmuyor; alt başlık değişikliği hâlâ veritabanında saklanamıyor. Gerekirse model + veritabanı şemasına bu alan eklenmelidir (ilgili `NewsVersionsPhotoGalleryItem` ve `NewsPublishedPhotoGalleryItem` tabloları da dahil).
- `Quark.CMS.Business` paylaşımlı bir katman olduğundan, bu değişiklik yayınlanırken **Hangfire uygulamalarının da yeniden yayınlanması** gerekir (proje kuralları gereği `Quark.CMS.Business` → CMS **ve** Hangfire'ı etkiler).
