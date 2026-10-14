$(document).ready(function () {

    $('.summernote').summernote({
        lang: 'tr-TR',
        tabsize: 10,
        height: 400,
        styleTags: [
            'p',
            { title: 'Blockquote', tag: 'blockquote', className: 'blockquote', value: 'blockquote' },
            'pre', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6'
        ],
        toolbar: [
            ['style', ['style']],
            ['misc', ['undo', 'redo', 'cleanEmptyTags']],
            ['font', ['bold', 'italic', 'underline', 'clear', 'strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['table', ['table']],
            ['insert', ['link', 'picture', 'photoGallery']],
            ['view', ['codeview', 'help']],
            ['ai', ['ai']]
        ],
        buttons: {
            // ai: aiButton
            photoGallery: photoGalleryButton,
            cleanEmptyTags: cleanEmptyTagsButton
        },
        popover: {
            image: [
                ['custom', ['imageAttributes'/*, 'photoEditor'*/]],
                ['image', ['resizeFull', 'resizeHalf', 'resizeQuarter', 'resizeNone']],
                ['float', ['floatLeft', 'floatRight', 'floatNone']],
                ['remove', ['removeMedia']]
            ],
            link: [
                ['link', ['linkDialogShow', 'unlink']]
            ],
            table: [
                ['add', ['addRowDown', 'addRowUp', 'addColLeft', 'addColRight']],
                ['delete', ['deleteRow', 'deleteCol', 'deleteTable']],
            ],
            air: [
                ['color', ['color']],
                ['font', ['bold', 'underline', 'clear']],
                ['para', ['ul', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture']]
            ]
        },
        imageAttributes: {
            icon: '<i class="fa fa-crop"/>',
            removeEmpty: false, // true = remove attributes | false = leave empty if present
            disableUpload: false // true = don't display Upload Options | Display Upload Options
        },
        photoEditor: {
            icon: '<i class="fa-solid fa-pen-to-square"></i>',
            removeEmpty: false, // true = remove attributes | false = leave empty if present
            disableUpload: false // true = don't display Upload Options | Display Upload Options
        },
        templates: {
            templates: '/', // The folder where the templates are stored.
            insertDetails: false, // true|false This toggles whether the below options are automatically filled when inserting the chosen page template.
            dateFormat: 'longDate',
            yourName: 'Your Name',
            yourTitle: 'Your Title',
            yourCompany: 'Your Comapny',
            yourPhone: 'Your Phone',
            yourAddress: 'Your Address',
            yourCity: 'Your City',
            yourState: 'Your State',
            yourCountry: 'Your Country',
            yourPostcode: 'Your Postcode',
            yourEmail: 'your@email.com'
        },

        callbacks: {
            onImageUpload: function (files) {
                var formData = new FormData();
                formData.append('file', files[0])

                $.ajax({
                    url: '/News/UploadImage',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        $('.summernote').summernote('editor.insertImage', data.data.photoURL);
                    },
                    error: function (error) {
                        console.error('Resim yüklenirken bir hata oluştu.', error);
                    }

                });
            }
        }

    });

    var eventMethod = window.addEventListener ? "addEventListener" : "attachEvent";
    var eventer = window[eventMethod];
    var messageEvent = eventMethod == "attachEvent" ? "onmessage" : "message";

    eventer(messageEvent, function (e) {

        var editableDiv = document.querySelector('.note-editable');

        var imgElements = editableDiv.querySelectorAll('img');
        var iframe = document.querySelector('.cropperjs-iframe');

        var updatedImgURL = e.data;
        var iframeParameter = iframe.src;

        var imgSrc = getParamFormURL(iframeParameter, '=');

        imgElements.forEach(function (imgElement) {
            if (imgElement.src == imgSrc) {
                imgElement.src = updatedImgURL;
            }
        });


    }, false);

    function getParamFormURL(str, separator) {
        var index = str.indexOf(separator);
        if (index === -1 || index === str.length - 1) {
            return str;
        } else {
            return str.substring(index + 1);
        }
    }

    // $('#yapayZekaModal').on('shown.bs.modal', function (e) {
    //     // Get the HTML content from the Summernote editor
    //     var editorContent = $('.summernote').summernote('code');
    //
    //     // Set the HTML content to the 'editorContent' div within the modal
    //     $('#editorContent').html(editorContent);
    // });
});

// var aiButton = function (context) {
//     var ui = $.summernote.ui;
//
//     // create button
//     var button = ui.button({
//         contents: '<span data-kt-drawer-show="true" data-kt-drawer-target="#drawerEditorAI"><img src="/img/AI.png" alt="Yapay Zekaya Sor" class="w-20px"/> Yapay Zeka</span>',
//         tooltip: 'Yapay Zekaya Sor',
//         click: function () {
//             // invoke insertText method with 'hello' on editor module.
//             //$('#yapayZekaModal').modal('show');
//         }
//     });
//
//     return button.render();   // return button as jquery object
// }
var photoGalleryButton = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: `<i class="ki-duotone ki-file fs-4">
                         <span class= "path1" ></span>
                     <span class="path2"></span> </i>`,
        tooltip: 'Medya Galeri',
        click: function () {

            var img_id = "summernote-photogallery";
            var modalBody = $('#media_gallery_modal').find('.modal-body');
            modalBody.attr('id', img_id);
            $('#media_gallery_modal').modal('show');
        }
    });

    var $button = button.render();
    
    $($button).tooltip({
        title: 'Medya Galeri',
        placement: 'top', 
        trigger: 'hover'
    });

    return $button;  
}

var cleanEmptyTagsButton = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: '<i class="fa fa-broom"></i>',
        tooltip: 'Boş Satırları Temizle',
        click: function () {
            var $editable = context.layoutInfo.editable;
            var content = $editable.html();
            for (var i = 0; i < 20; i++) {
                content = content.replace('<p></p>', '').replace('<p> </p>', '').replace('<p><br data-mce-bogus="1"></p>', '').replace('<p><br></p>', '');
            }
            $editable.html(content);
        }
    });

    var $button = button.render();

    $($button).tooltip({
        title: 'Boş Satırları Temizle',
        placement: 'top',
        trigger: 'hover'
    });

    return $button;
}

function initSummernote(settings) {
    if (settings === undefined) {
        settings = {};
    }

    //Summernote initialize edilirken fotoğraf yükleme aktivasyonu işlemi property ismi kolay okunulması için yapıldı -Gökay
    settings.PhotoUpload = settings.PhotoUpload === true ? 'picture' : '';

    $('.summernote').summernote({
        lang: 'tr-TR',
        tabsize: 10,
        height: 400,
        styleTags: [
            'p',
            { title: 'Blockquote', tag: 'blockquote', className: 'blockquote', value: 'blockquote' },
            'pre', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6'
        ],
        toolbar: [
            ['style', ['style']],
            ['misc', ['undo', 'redo', 'cleanEmptyTags']],
            ['font', ['bold', 'underline', 'clear', 'strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['table', ['table']],
            ['insert', ['link', settings.PhotoUpload, 'photoGallery']],
            ['view', ['codeview', 'help']],
            ['ai', ['ai']]
        ],
        buttons: {
            // ai: aiButton
            photoGallery: photoGalleryButton,
            cleanEmptyTags: cleanEmptyTagsButton
        },
        popover: {
            image: [
                ['custom', ['imageAttributes', 'photoEditor']],
                ['image', ['resizeFull', 'resizeHalf', 'resizeQuarter', 'resizeNone']],
                ['float', ['floatLeft', 'floatRight', 'floatNone']],
                ['remove', ['removeMedia']]
            ],
            link: [
                ['link', ['linkDialogShow', 'unlink']]
            ],
            table: [
                ['add', ['addRowDown', 'addRowUp', 'addColLeft', 'addColRight']],
                ['delete', ['deleteRow', 'deleteCol', 'deleteTable']],
            ],
            air: [
                ['color', ['color']],
                ['font', ['bold', 'underline', 'clear']],
                ['para', ['ul', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture']]
            ]
        },
        imageAttributes: {
            icon: '<i class="fa fa-crop"/>',
            removeEmpty: false, // true = remove attributes | false = leave empty if present
            disableUpload: false // true = don't display Upload Options | Display Upload Options
        },
        photoEditor: {
            icon: '<i class="fa-solid fa-pen-to-square"></i>',
            removeEmpty: false, // true = remove attributes | false = leave empty if present
            disableUpload: false // true = don't display Upload Options | Display Upload Options
        },
        templates: {
            templates: '/', // The folder where the templates are stored.
            insertDetails: false, // true|false This toggles whether the below options are automatically filled when inserting the chosen page template.
            dateFormat: 'longDate',
            yourName: 'Your Name',
            yourTitle: 'Your Title',
            yourCompany: 'Your Comapny',
            yourPhone: 'Your Phone',
            yourAddress: 'Your Address',
            yourCity: 'Your City',
            yourState: 'Your State',
            yourCountry: 'Your Country',
            yourPostcode: 'Your Postcode',
            yourEmail: 'your@email.com'
        },

        callbacks: {
            onImageUpload: function (files) {
                var formData = new FormData();
                formData.append('file', files[0])

                $.ajax({
                    url: '/News/UploadImage',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        $('.summernote').summernote('editor.insertImage', data.data.photoURL);
                    },
                    error: function (error) {
                        toastr.error('Resim yüklenirken bir hata oluştu.');
                    }

                });
            }
        }
    });
}

$(document).ready(function () {
    var formIds = ["#newsFormCreate", "#createPageForm", "#updatePageForm"];

    formIds.forEach(function (formId) {
        $(formId).on('submit', function (e) {
            var summernote = $(formId).find(".summernote").val();
            var validationInterval = setInterval(function () {
                summernote = $(formId).find(".summernote").val();

                if (!summernote) {
                    $(formId).find("#summernoteValidation").text(localizer.pleaseFillTheContentField);
                } else {
                    $(formId).find("#summernoteValidation").text("");
                }
            }, 500);

            if (!summernote) {
                e.preventDefault();
                $(formId).find("#summernoteValidation").text(localizer.pleaseFillTheContentField);
                clearInterval(validationInterval);
            } else {
                clearInterval(validationInterval);
            }
        });
    });
});

