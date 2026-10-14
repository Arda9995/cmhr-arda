$(document).ready(function () {
    //SendImageToCDN();

    var initialStatusID = $('#statu-select').val(); // Get the initial status ID
    hideOrShowDateTimePicker(initialStatusID);
    changeSelectStatu();
});

function toggleEditButtonVisibility() {
    if($(this).css('background-image') !== baseUrl){
        $('.photoTextButton').removeClass('d-none').addClass('d-block');
        $('.editButton').removeClass('d-none').addClass('d-block');
        $('.mediaGalleryButton').removeClass('d-none').addClass('d-block');
    }
}

function SendImageToCDN(i) {
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
                imageInputWrapper.css('background-image', '');
                hiddenPhotoUrl.val('');
            }
            else{
                imageInputWrapper.css('background-image', 'url(' + data.data.photoURL + ')');
                hiddenPhotoUrl.val(data.data.photoURL);
                fileImage[0].value = '';
                $(`#editButton_${i}`).removeClass('d-none').addClass('d-flex');
                $(`#photoTextButton_${i}`).removeClass('d-none').addClass('d-flex');
                $(`#removeButton_${i}`).removeClass('d-none').addClass('d-flex');
                $('.mediaGalleryButton').removeClass('d-none').addClass('d-flex');
                //mainPageCropper();
            }
        },
        error: function (response) {
            ShowError(response.message);
        }
    });
}

$(document).ready(function () {
    // Attach event handler to document and delegate the click event to dynamically added elements
    $(document).on('click', '.editButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');
        $('.cropperjs-iframe').attr('src', `/Cropper/Edit?imgSrc=${backgroundImageUrl}&imgId=${this.id.replace('editButton_', '')}&isNotEditor=true`);
        $('#cropperModal').modal('show');
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

    $(document).on('click', '.mediaGalleryButton', function () {
        var img_id = $(this).siblings('.image-input-wrapper').attr('id');
        var modalBody = $('#media_gallery_modal').find('.modal-body');
        modalBody.attr('id', img_id);
        $('#media_gallery_modal').modal('show');
    });

    $(document).on('click', '.cancelButton', function () {
        var imageInputWrapper = $(this).siblings('.image-input-wrapper');
        var hiddenImgInput = $(this).siblings('input[type=hidden]');
        $('#removeButton').trigger('click');
    });

    $(document).on('click', '.removeButton', function () {
        var editButton = $(this).siblings('.editButton');
        var photoTextButton = $(this).siblings('.photoTextButton');
        var removeButton = $(this);

        editButton.removeClass('d-flex').addClass('d-none');
        photoTextButton.removeClass('d-flex').addClass('d-none');

        removeButton.removeClass('d-flex').addClass('d-none');
        var imageInputWrapper = $(this).siblings('.image-input-wrapper');
        var hiddenImgInput = $(this).siblings('input[type=hidden]');
        imageInputWrapper.css('background-image', ''); // Clear background image
        hiddenImgInput.val(''); // Clear hidden input value

    });

    $('#mainpage-photo').on('change', function (event) {
        var imageInputWrapper = $(this).siblings('.image-input-wrapper');
        var wrapperId = imageInputWrapper.attr('id').split('_')[1];
        var photoTypeId = document.getElementById('hiddenPhotoTypeID_' + wrapperId).value;
        var hiddenImgInput = $(this).siblings('input[type=hidden]');
        var formData = new FormData();
        formData.append('file', event.target.files[0]);

        $.ajax({
            url: '/News/UploadImage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                imageInputWrapper.css('background-image', 'url(' + data.data.photoURL + ')');
                hiddenImgInput.val(data.data.photoURL);
                mainPageCropper();
            },
            error: function (xhr, status, error) {
                // Handle error
            }
        });
    });
});

function onImageCancel(i) {
    // var imageInputWrapper = $(e).siblings('.image-input-wrapper');
    // var hiddenImgInput = $(e).siblings('input[type=hidden]');
    // imageInputWrapper.css('background-image', ''); // Clear background image
    // hiddenImgInput.val(''); // Clear hidden input value
    var removeButton = document.querySelector(`#removeButton_${i}`)
    removeButton.style.display = 'flex';
    removeButton.click();
}

function mainPageCropper() {
    toggleEditButtonVisibility(); // Toggle edit button visibility based on displayed photos
}

var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
var eventer = window[eventMethod];
var messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

eventer(messageEvent, function (e) {
    var hiddenImgInput = document.getElementById('HeadlinePhotoURL');

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

function hideOrShowDateTimePicker(initialStatusID) {
    var dateTimePicker = $('#kt_ecommerce_add_product_status_datepicker');
    var dateTimeDiv = $('#datetimetest');

    if (initialStatusID !== '3') {
        dateTimePicker.hide();
        dateTimeDiv.hide();
    } else {
        dateTimePicker.show();
        dateTimeDiv.hide();
    }
}

function changeSelectStatu() {
    var selectElement = document.getElementById("statu-select");
    var statusID = selectElement.options[selectElement.selectedIndex].value;

    var statusDiv = $('#kt_ecommerce_add_product_status');
    var dateTimePicker = $('#kt_ecommerce_add_product_status_datepicker');
    var dateTimeDiv = $('#datetimetest');

    statusDiv.removeClass('bg-success bg-warning bg-info bg-danger');

    switch (statusID) {
        case '1':
            statusDiv.addClass('bg-warning');
            dateTimePicker.hide();
            dateTimeDiv.hide();
            break;
        case '2':
            statusDiv.addClass('bg-success');
            dateTimePicker.hide();
            dateTimeDiv.hide();
            break;
        case '3':
            statusDiv.addClass('bg-danger');
            dateTimePicker.hide();
            dateTimeDiv.hide();
            break;
        case '4':
            statusDiv.addClass('bg-info');
            dateTimePicker.show();
            dateTimeDiv.show();
            break;
        default:
            statusDiv.removeClass('bg-success bg-warning bg-info bg-danger');
            break;
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

    prefix = prefix.replace("{Title}", titleKeyWord)
        .replace("{CategoryID}", category.value)
        .replace("{CategoryName}", categoryText);


    var slugInput = document.getElementById('basic-url');
    slugInput.value = "";
    slugInput.value += prefix;

    if(prefix.includes("{ID}") || prefix.includes("{Year}") || prefix.includes("{Month}")  || prefix.includes("{Date}")){
        var note = document.getElementById('url-note');
        note.innerHTML += " { } ile gösterilen alanlar haber kaydedildikten sonra güncellenecektir.";
    }
}

const initProposalItemRepeater = () => {
    $('#gallery_items').repeater({
        initEmpty: true,
        show: function () {
            $(this).slideDown();
            
            initSummernote({PhotoUpload : false});
        },
        hide: function (deleteElement) {
            $(this).slideUp(deleteElement);
        },
    });
}

initProposalItemRepeater();


