$(document).ready(function () {
    $('.editButton').removeClass('d-none').addClass('d-flex'); // Hide edit buttons initially
    $('.photoTextButton').removeClass('d-none').addClass('d-flex'); // Hide edit buttons initially
    // SendImageToCDN();

});

function mainPageCropper() {
    var editButton = document.getElementById('editButton');
    editButton.classList.remove('d-none');
    editButton.classList.add('d-block');
}

function SendImageToCDN(i) {
    debugger;
    var imageInputWrapper = $("#divImageInputWrapper_" + i);
    var fileImage = $("#fileImage_" + i);
    var hiddenPhotoUrl = $("#hiddenPhotoUrl_" + i);
    var formData = new FormData();
    var photoTypeId = document.getElementById('hiddenPhotoTypeID_' + i).value;
    formData.append('file', fileImage[0].files[0]);
    formData.append('photoTypeId', photoTypeId);

    $.ajax({
        url: '/News/UploadImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.isSuccess === false) {
                ShowError(data.message);
            }
            else{
                imageInputWrapper.css('background-image', 'url(' + data.data.photoURL + ')');
                hiddenPhotoUrl.val(data.data.photoURL);
                fileImage[0].value = '';
                //mainPageCropper();
            }
        },
        error: function (response) {
            console.error('Resim yüklenirken bir hata oluştu.', error);
        }
    });
}

$(document).ready(function () {
    // Edit button click event
    $(document).on('click', '.editButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');
        $('.cropperjs-iframe').attr('src', `/Cropper/Edit?imgSrc=${backgroundImageUrl}&imgId=${this.id.replace('editButton_', '')}&isNotEditor=true`);
        $('#cropperModal').modal('show');
    });

    $('#removeButton').on('click', function () {
        var editButton = document.getElementById('editButton');
        var hiddenImgInput = document.getElementById('photo-url-input');
        editButton.classList.remove('d-block');
        editButton.classList.add('d-none');
        hiddenImgInput.textContent = '';
    });

    $(document).on('click', '.photoTextButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        var img_id = $(this).siblings('.image-input-wrapper').attr('id');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');
        var html = "<img id='edited-photo' class='edited-photo' src='" + backgroundImageUrl + "' alt='' style='width: 100%; height: 100%'/>";
        html += "<img id='edited-photo-org' class='"+ img_id +"' src='" + backgroundImageUrl + "' alt='' style='display: none;'/>"
        $("#photo").html(html);
        $('#kt_modal_stacked_1').modal('show');
    });
});

var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
var eventer = window[eventMethod];
var messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

eventer(messageEvent, function (e) {

    var hiddenImgInput = document.getElementById('photo-url-input');

    var imgElements = document.querySelectorAll('.image-input-wrapper');

    var updatedImgURL = e.data;

    var iframe = document.querySelector('.cropperjs-iframe');
    var iframeParameter = iframe.src;

    var imgSrc = getParamFormURL(iframeParameter, '=');

    imgElements.forEach(function (imgElement) {
        var backgroundImageOfPreview = imgElement.style.backgroundImage;
        const cleanedUrl = backgroundImageOfPreview.replace(/^url\(["']?|["']?\)$/g, '');

        if (cleanedUrl == imgSrc) {
            imgElement.style.backgroundImage = 'url("' + updatedImgURL + '")';
            hiddenImgInput.textContent = updatedImgURL;
        }
    });


}, false);

function getParamFormURL(str, isaret) {
    var index = str.indexOf(isaret);
    if (index === -1 || index === str.length - 1) {
        return str;
    } else {
        return str.substring(index + 1);
    }
}

const inputElement = document.getElementById('basic-url');

inputElement.addEventListener('keypress', handleKeyPress);

function handleKeyPress(event) {
    const keyPressed = event.key;

    const isLowerCaseEnglishLetterOrDigitOrDash = /^[a-z0-9-]$/.test(keyPressed);

    if (!isLowerCaseEnglishLetterOrDigitOrDash) {
        event.preventDefault();
    }
}

function ToggleUrlGenerate(){
    var title = document.getElementById('Title').value;
    var titleKeyWord = title
        .toLowerCase() // Convert to lowercase
        .replace(/[üÜ]/g, "u") // Convert 'ü' to 'u'
        .replace(/[ğĞ]/g, "g") // Convert 'ğ' to 'g'
        .replace(/[çÇ]/g, "c") // Convert 'ç' to 'c'
        .replace(/[şŞ]/g, "s") // Convert 'ş' to 's'
        .replace(/[öÖ]/g, "o") // Convert 'ö' to 'o'
        .replace(/[ıİ]/g, "i") // Convert 'ı' to 'i'
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/-+/g, "-")
        .replace(/^-+|-+$/g, "");

    var category = document.getElementById('CategoryID');
    var categoryText = category.options[category.selectedIndex].text
    categoryText = categoryText.toLowerCase() // Convert to lowercase
        .replace(/[üÜ]/g, "u") // Convert 'ü' to 'u'
        .replace(/[ğĞ]/g, "g") // Convert 'ğ' to 'g'
        .replace(/[çÇ]/g, "c") // Convert 'ç' to 'c'
        .replace(/[şŞ]/g, "s") // Convert 'ş' to 's'
        .replace(/[öÖ]/g, "o") // Convert 'ö' to 'o'
        .replace(/[ıİ]/g, "i") // Convert 'ı' to 'i'
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/-+/g, "-")
        .replace(/^-+|-+$/g, "");

    var prefix = document.getElementById('url-prefix').value;

    if ((prefix.includes("{CategoryID}") || prefix.includes("{CategoryName}")) && category.value === "-1") {
        swal.fire("Hata", "Lütfen bir kategori seçiniz.", "error");
        return;
    }

    if(prefix.includes("{Title}") && title === ""){
        swal.fire("Hata", "Lütfen bir başlık giriniz.", "error");
        return;
    }

    var ID = document.getElementById('newsId').value;

    prefix = prefix.replace("{Title}", titleKeyWord)
        .replace("{CategoryID}", category.value)
        .replace("{CategoryName}", categoryText)
        .replace("{ID}", ID);

    var slugInput = document.getElementById('basic-url');
    slugInput.value = "";
    slugInput.value += prefix;

    if(prefix.includes("{Year}") || prefix.includes("{Month}")  || prefix.includes("{Date}")){
        var note = document.getElementById('url-note');
        note.innerHTML += " { } ile gösterilen alanlar haber kaydedildikten sonra güncellenecektir.";
    }
}



