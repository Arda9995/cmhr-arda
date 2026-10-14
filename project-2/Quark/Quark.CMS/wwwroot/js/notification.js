/**
 * NetmeraNotification - Web Push Notification Komponenti
 * 
 * Kullanım Örneği:
 * 
 * // Komponenti başlat
 * const notificationManager = new NetmeraNotification({
 *     endpointUrl: '/Notification/SendWebPushNotification',
 *     onSuccess: function(response) { console.log('Bildirim gönderildi:', response); },
 *     onError: function(error) { console.error('Hata oluştu:', error); }
 * });
 * 
 * // Bildirim gönder
 * notificationManager.sendNotification({
 *     title: 'Bildirim Başlığı',
 *     message: 'Bildirim içeriği buraya gelir',
 *     iconUrl: 'https://site.com/icon.png',
 *     url: 'https://site.com/haber/123'
 * });
 */

class NetmeraNotification {
    /**
     * NetmeraNotification sınıfı constructor
     * @param {Object} config - Konfigürasyon nesnesi
     * @param {string} config.endpointUrl - Notification isteğinin gönderileceği endpoint URL
     * @param {Function} config.onSuccess - Başarılı istek sonrası çalışacak callback
     * @param {Function} config.onError - Hatalı istek sonrası çalışacak callback
     * @param {boolean} config.debug - Debug modunu aktif eder (varsayılan: false)
     * @param {Object} config.defaultValues - Varsayılan değerler
     */
    constructor(config = {}) {
        this.config = {
            endpointUrl: config.endpointUrl || '/Notification/SendWebPushNotification',
            onSuccess: config.onSuccess || function() {},
            onError: config.onError || function() {},
            debug: config.debug || false,
            defaultValues: config.defaultValues || {
                iconUrl: 'https://cumhuriyetreklam.com/wp-content/uploads/2019/02/cumhuriyet-logo-icon-512-512.jpg',
                url: 'https://www.cumhuriyet.com.tr/',
                title: 'Cumhuriyet Gazetesi',
                message: 'Cumhuriyet Gazetesi, Türkiye’nin en köklü gazetelerinden biridir. Güncel haberler, analizler ve yorumlarla dolu bir platform sunar.',
            }
        };

        this.log('NetmeraNotification komponenti başlatıldı', this.config);
    }

    /**
     * Console'a log basar (sadece debug mode aktif ise)
     * @param {string} message - Log mesajı
     * @param {any} data - Loglanan veri
     */
    log(message, data = null) {
        if (this.config.debug) {
            if (data) {
                console.log(`NetmeraNotification: ${message}`, data);
            } else {
                console.log(`NetmeraNotification: ${message}`);
            }
        }
    }

    /**
     * Bildirim gönderir
     * @param {Object} notification - Bildirim verileri
     * @param {string} notification.title - Bildirim başlığı (zorunlu)
     * @param {string} notification.message - Bildirim mesajı (zorunlu)
     * @param {string} notification.iconUrl - Bildirim ikonu URL
     * @param {string} notification.url - Bildirim tıklandığında gidilecek URL
     * @param {string} notification.personalizedTitle - Kişiselleştirilmiş başlık
     * @param {string} notification.wpChromeImage - Chrome bildirimlerinde gösterilecek resim
     * @param {boolean} notification.interactionRequired - Etkileşim gerekiyorsa true
     * @returns {Promise} - AJAX isteği sonucu
     */
    sendNotification(notification) {
        // Zorunlu alanları kontrol et
        if (!notification.title) {
            const error = 'Bildirim başlığı zorunludur';
            this.log(error);
            return Promise.reject(error);
        }

        if (!notification.message) {
            const error = 'Bildirim mesajı zorunludur';
            this.log(error);
            return Promise.reject(error);
        }

        // Varsayılan değerleri ekle
        const formData = {
            title: notification.title || this.config.defaultValues.title,
            message: notification.message || this.config.defaultValues.message,
            iconUrl: notification.iconUrl || this.config.defaultValues.iconUrl,
            url: notification.url || this.config.defaultValues.url,
            wpChromeImage: notification.wpChromeImage || notification.iconUrl || this.config.defaultValues.iconUrl,
            interactionRequired: notification.interactionRequired !== undefined ? notification.interactionRequired : true
        };

        this.log('Bildirim gönderiliyor', formData);

        // AJAX isteği gönder
        return new Promise((resolve, reject) => {
            $.ajax({
                url: this.config.endpointUrl,
                type: 'POST',
                data: formData,
                success: (response) => {
                    this.log('Bildirim gönderildi', response);
                    
                    if (response.success) {
                        this.config.onSuccess(response);
                        resolve(response);
                    } else {
                        const error = {
                            message: response.message || 'Bildirim gönderilirken bir hata oluştu',
                            response: response
                        };
                        this.log('Bildirim hatası', error);
                        this.config.onError(error);
                        reject(error);
                    }
                },
                error: (xhr, status, error) => {
                    const errorData = {
                        message: 'Bildirim isteği başarısız oldu',
                        status: status,
                        error: error,
                        xhr: xhr
                    };
                    
                    this.log('AJAX hatası', errorData);
                    this.config.onError(errorData);
                    reject(errorData);
                }
            });
        });
    }

    /**
     * Bildirim göndermek için UI oluşturur ve dökümanın belirtilen yerine ekler
     * @param {string} targetSelector - UI'ın ekleneceği element seçicisi
     * @param {Object} options - UI seçenekleri
     * @returns {Object} - DOM referansları
     */
    createUI(targetSelector, options = {}) {
        const targetElement = document.querySelector(targetSelector);
        
        if (!targetElement) {
            this.log(`Hedef element bulunamadı: ${targetSelector}`);
            return null;
        }

        // UI yapısını oluştur
        const container = document.createElement('div');
        container.className = 'card card-flush mb-5';
        
        container.innerHTML = `
            <div class="card-header">
                <h3 class="card-title">Web Push Bildirimleri</h3>
            </div>
            <div class="card-body">
                <form id="netmera-notification-form">
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label class="form-label">Bildirim Başlığı</label>
                            <input type="text" class="form-control" id="netmera-title" placeholder="Bildirim başlığı" value="${this.config.defaultValues.title}"required>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Bildirim Mesajı</label>
                            <input type="text" class="form-control" id="netmera-message" placeholder="Bildirim mesajı" value="${this.config.defaultValues.message}"required>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label class="form-label">İkon URL</label>
                            <input type="text" class="form-control" id="netmera-icon-url" placeholder="https://site.com/icon.png" value="${this.config.defaultValues.iconUrl}">
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Yönlendirme URL</label>
                            <input type="text" class="form-control" id="netmera-url" placeholder="https://site.com/haber/123" value="${this.config.defaultValues.url}">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12">
                            <button type="submit" class="btn btn-primary">Bildirimi Gönder</button>
                        </div>
                    </div>
                </form>
                <div class="mt-3 d-none" id="netmera-notification-result">
                    <div class="alert alert-success"></div>
                </div>
            </div>
        `;
        
        targetElement.appendChild(container);
        
        // Event listener ekle
        const form = container.querySelector('#netmera-notification-form');
        const resultDiv = container.querySelector('#netmera-notification-result');
        const resultAlert = resultDiv.querySelector('.alert');
        
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            
            const notification = {
                title: container.querySelector('#netmera-title').value,
                message: container.querySelector('#netmera-message').value,
                iconUrl: container.querySelector('#netmera-icon-url').value,
                url: container.querySelector('#netmera-url').value
            };
            
            this.sendNotification(notification)
                .then(response => {
                    resultDiv.classList.remove('d-none');
                    resultAlert.classList.remove('alert-danger');
                    resultAlert.classList.add('alert-success');
                    resultAlert.textContent = response.message + (response.notificationKey ? ` (Bildirim anahtarı: ${response.notificationKey})` : '');
                })
                .catch(error => {
                    resultDiv.classList.remove('d-none');
                    resultAlert.classList.remove('alert-success');
                    resultAlert.classList.add('alert-danger');
                    resultAlert.textContent = error.message || 'Bildirim gönderilirken bir hata oluştu';
                });
        });
        
        return {
            container,
            form,
            resultDiv,
            resultAlert
        };
    }
}

// Global scope'a ekle
window.NetmeraNotification = NetmeraNotification; 