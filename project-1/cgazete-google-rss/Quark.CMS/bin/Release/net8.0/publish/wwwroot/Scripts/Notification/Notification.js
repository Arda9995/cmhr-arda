function SendWebNotification() {
    var notificationData = {
        Title: $('#notification-title').val(),
        Message: $('#notification-description').val(),
        Url: $('#news-full-url').val(),
        WpChromeImage : $('#cover-photo').val(),
    };

    $.ajax({
        url: '/Notification/SendWebPushNotification',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(notificationData),
        success: function (response) {
            toastr.success('Bildirim başarıyla gönderildi!');
        },
        error: function (xhr, status, error) {
            toastr.error('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}

function SendMobileNotification() {
    var notificationData = {
        Title: $('#notification-title').val(),
        Description: $('#notification-description').val(),
        NewsID: $('#notification-newsid').val(),
        NewsTypeName: $('#notification-type').val(),
    };

    $.ajax({
        url: '/Notification/SendMobilPushNotification',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(notificationData),
        success: function (response) {
            toastr.success('Bildirim başarıyla gönderildi!');
        },
        error: function (xhr, status, error) {
            toastr.error('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}