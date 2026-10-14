$(document).ready(function () {
    $('.editButton').removeClass('d-none').addClass('d-flex'); // Hide edit buttons initially
    $('.photoTextButton').removeClass('d-none').addClass('d-flex'); // Hide edit buttons initially
    // SendImageToCDN();

});


$(document).ready(function () {
    // Attach event handler to document and delegate the click event to dynamically added elements
    $(document).on('click', '.editButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');
        $('.cropperjs-iframe').attr('src', `/Cropper/Edit?imgSrc=${backgroundImageUrl}&imgId=${this.id.replace('editButton_', '')}&isNotEditor=true`);
        $('#cropperModal').modal('show');
    });

    $(document).on('click', '.photoEditButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        var img_id = $(this).siblings('.image-input-wrapper').attr('id');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');

        var html = "<img id='edited-photo' class='edited-photo' src='" + backgroundImageUrl + "' alt='' style='width: 100%; height: 100%'/>";
        html += "<img id='edited-photo-org' class='" + img_id + "' src='" + backgroundImageUrl + "' alt='' style='display: none;'/>"
        $("#photo").html(html);

        $("#edited-photo").src = backgroundImageUrl;
        $("#edited-photo-org").src = backgroundImageUrl;

        $('.cropperjs-iframe').attr('src', `/Cropper/Edit?imgSrc=${backgroundImageUrl}&imgId=${this.id.replace('editButton_', '')}&isNotEditor=true`);

        $('#photo-edit-card-modal').modal('show');
    });

    $(document).on('click', '.photoTextButton', function () {
        var backgroundImageUrl = $(this).siblings('.image-input-wrapper').css('background-image');
        var img_id = $(this).siblings('.image-input-wrapper').attr('id');
        backgroundImageUrl = backgroundImageUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/g, '');
        var html = "<img id='edited-photo' class='edited-photo' src='" + backgroundImageUrl + "' alt='' style='width: 100%; height: 100%'/>";
        html += "<img id='edited-photo-org' class='" + img_id + "' src='" + backgroundImageUrl + "' alt='' style='display: none;'/>"
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
        var photoEditButton = $(this).siblings('.photoEditButton');
        var photoTextButton = $(this).siblings('.photoTextButton');
        var removeButton = $(this);

        editButton.removeClass('d-flex').addClass('d-none');
        photoEditButton.removeClass('d-flex').addClass('d-none');
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

document.addEventListener("DOMContentLoaded", function () {
    // Get the hidden input for the selected video player
    const hiddenInput = document.querySelector('input[name="SelectedVideoPlayer"]');

    // Get all the radio buttons for video players
    const videoPlayerRadios = document.querySelectorAll('input[name="VideoPlayerID"]');

    // Function to update the hidden input with the selected radio button's value
    function updateHiddenInput() {
        const selectedRadio = document.querySelector('input[name="VideoPlayerID"]:checked');
        if (selectedRadio) {
            hiddenInput.value = selectedRadio.value;
        }
    }

    // Function to check the appropriate radio button based on the hidden input's value
    function checkRadioButtonFromHiddenInput() {
        if (hiddenInput.value) {
            const matchingRadio = document.querySelector(`input[name="VideoPlayerID"][value="${hiddenInput.value}"]`);
            if (matchingRadio) {
                matchingRadio.checked = true;
            }
        }
    }

    // Add event listeners to update hidden input when a radio button is selected
    videoPlayerRadios.forEach(radio => {
        radio.addEventListener('change', updateHiddenInput);
    });

    // Set the radio button based on hidden input value (if present) on page load
    checkRadioButtonFromHiddenInput();

    // Set the initial value of the hidden input on page load
    updateHiddenInput();
});

function mainPageCropper() {
    var editButton = document.getElementById('editButton');
    editButton.classList.remove('d-none');
    editButton.classList.add('d-block');
}

function SendImageToCDN(i) {
    var imageInputWrapper = $("#divImageInputWrapper_" + i);
    var fileImage = $("#fileImage_" + i);
    var hiddenPhotoUrl = $("#hiddenPhotoUrl_" + i);
    var hiddenPhotoWidth = $("#hiddenPhotoWidth_" + i);
    var hiddenPhotoHeight = $("#hiddenPhotoHeight_" + i);
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
            else {
                imageInputWrapper.css('background-image', 'url(' + data.data.photoURL + ')');
                hiddenPhotoUrl.val(data.data.photoURL);
                if (hiddenPhotoWidth.length) hiddenPhotoWidth.val(data.data.width || 0);
                if (hiddenPhotoHeight.length) hiddenPhotoHeight.val(data.data.height || 0);
                fileImage[0].value = '';
                $(`#editButton_${i}`).removeClass('d-none').addClass('d-flex');
                $(`#photoTextButton_${i}`).removeClass('d-none').addClass('d-flex');
                $(`#removeButton_${i}`).removeClass('d-none').addClass('d-flex');
                $('.mediaGalleryButton').removeClass('d-none').addClass('d-flex');
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
        html += "<img id='edited-photo-org' class='" + img_id + "' src='" + backgroundImageUrl + "' alt='' style='display: none;'/>"
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

function ToggleUrlGenerate() {
    var titleExist = document.getElementById('Title');
    var categoryExist = document.getElementById('spanSelectedMainCategory');
    var categoryId = document.getElementById('mainCategoryId');

    var prefix = document.getElementById('url-prefix').value;
    var title = "";
    var titleKeyword = "";

    if (titleExist) {
        title = titleExist.value;
        titleKeyWord = title
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
    }
    var headlineKeyword = "";
    if (!titleExist) {
        var headline = document.getElementById('Headline').value;
        headlineKeyword = headline
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
    }

    if (categoryExist) {
        var categoryText = categoryExist.innerText;
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

        if ((prefix.includes("{CategoryID}") || prefix.includes("{CategoryName}")) && categoryText == "---") {
            swal.fire("Hata", "Lütfen bir kategori seçiniz.", "error");
            return;
        }
    }


    if (title) {
        prefix = prefix.replace("{Title}", titleKeyWord);
    } else {
        prefix = prefix.replace("{Title}", headlineKeyword);
    }

    if (prefix.includes("{Title}") && title === "") {
        swal.fire("Hata", "Lütfen bir başlık giriniz.", "error");
        return;
    }

    if (categoryId && categoryId.value) {
        prefix = prefix.replace("{CategoryID}", categoryId.value)
            .replace("{CategoryName}", categoryText);
    }
    else {
        prefix = prefix.replace("{CategoryID}", "")
            .replace("{CategoryName}", "");
    }

    var slugInput = document.getElementById('basic-url');
    slugInput.value = "";
    slugInput.value += prefix;

    if (prefix.includes("{Year}") || prefix.includes("{Month}") || prefix.includes("{Date}")) {
        var note = document.getElementById('url-note');
        note.textContent += " { } ile gösterilen alanlar haber kaydedildikten sonra güncellenecektir.";
    }
}

$(document).ready(function () {
    var $select = $('.tag-search-ajax');

    $select.select2({
        language: "tr",
        ajax: {
            url: '/Tags/SearchTags',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function (data) {
                var formattedData = data.map(function (item) {
                    return {
                        id: item.value,
                        text: item.text
                    };
                });

                return {
                    results: formattedData
                };
            },
            cache: true
        },
        minimumInputLength: 3
    });

    var selectedTagIds = $('#selected-tag-ids').val();

    if (selectedTagIds) {
        var idsArray = selectedTagIds.split(',').map(Number); 

        idsArray.forEach(function (id) {
            $.ajax({
                type: 'GET',
                url: '/Tags/TagSearchByID', 
                data: { id: id }, 
                success: function (item) {
                    var option = new Option(item.text, item.value, true, true);
                    $select.append(option);
                    $select.trigger('change');
                }
            });
        });
    }

    var $sourceSelect = $('.sources-search-ajax');

    $sourceSelect.select2({
        language: "tr",
        ajax: {
            url: '/Sources/SearchSources',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function (data) {
                var formattedData = data.map(function (item) {
                    return {
                        id: item.value,
                        text: item.text
                    };
                });

                return {
                    results: formattedData
                };
            },
            cache: true
        },
        minimumInputLength: 2
    });

  
    var selectedSourceIds = $('#source-id-string').val();

    if (selectedSourceIds) {
        var idsArray = selectedSourceIds.split(',').map(Number);
        idsArray.forEach(function (id) {
            $.ajax({
                dataType: 'json',
                url: '/Sources/SearchSourcesByID', 
                contentType: 'application/json',
                data:{
                    id: id 
                },
                success: function (data) {
                    var option = new Option(data.text, data.value, true, true); 
                    $sourceSelect.append(option);
                    $sourceSelect.trigger('change');
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching source with ID ' + id + ':', error);
                }
            });
        });
    }
});

function CancelUpdate(id) {
    $.ajax({
        url: '/News/UnLock/' + id,
        contentType: 'application/json',
        type: 'POST',
        success: function (data) {
            if (data.success) {
                window.location.href = "/News/List";
            } else {
                console.error('Error fetching source with ID ' + id);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching source with ID ' + id + ':', error);
        }
    });
}

$(function () {
    $('.maxlength').maxlength({
        warningClass: "badge badge-primary",
        limitReachedClass: "badge badge-success"
    });
});