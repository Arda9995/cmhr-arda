var timer;
var base64String;

var blockUI = new KTBlockUI(document.querySelector('#divPhotoPage'));
const form = document.querySelector('#form1');

form.addEventListener('change', function () {
    if (timer) {
        clearTimeout(timer);
    }
    timer = setTimeout(preparePhoto, 1000);
});

$('.original-foto').on('change', function (event) {
    var photo = document.getElementById("edited-photo");
    var fileInput = event.target.files[0];
    
    
    if (fileInput != null) {
        var reader = new FileReader();
        reader.onload = function (e) {
            debugger;
            base64String = e.target.result.split(',')[1];
            photo.src = "data:image/png;base64," + base64String;
            
        };
        reader.readAsDataURL(fileInput);
    }
});


function preparePhoto() {
    blockUI.block();
    var bgColor = document.getElementById("BackgroundColor").value;
    var opacity = document.getElementById("Opacity").value;
    var textFontSize = document.getElementById("FontSize").value;
    var textColor = document.getElementById("TextColor").value;
    
    var formData = {
        "BackgroundColor" : bgColor,
        "Opacity" : opacity,
        "TextFontSize" : textFontSize,
        "TextColor" : textColor,
    }
    
    $.ajax({
        url: "/PhotoEditTemplates/PreparePhoto",
        type: "POST",
        data: formData,
        success: function (response) {
            blockUI.release();
            var previewImage = document.querySelector('#preview-img');
            previewImage.src = "data:image/png;base64," + response.data.base64Image;
        },
        error: function (response) {
            blockUI.release();
        },
    });
}