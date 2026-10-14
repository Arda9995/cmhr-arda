//
// BU JS DOSYASI SADECE LOCAL ÇALIŞMALARDA DEV İÇİN KULLANILIR. CANLIYA ÇIKARKEN MUTLAKA QUARK CDN ÜZERİDEN KULLANINIZ.
// Bu dosyayı CDN yüklerken MAC OS kullanıyorsanız mutlaka encoding olarak UTF-8 kullanınız.
//

///////////////////////////////////////////////////////////////////////
// Version 1.0.29
// Updated Date: 05.12.2023
// Updated By: Murat Yılmaz
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
// INPUT FORMAT
///////////////////////////////////////////////////////////////////////

// Textboxların içinde IBAN class kullananları bularak yanlızca IBAN girilmesini sağlar.
Inputmask({mask: "AA99 9999 9999 9999 9999 99"}).mask(".IBAN");

// Textboxların içinde phonenumber class kullananları bularak yanlızca telefon numarası girilmesini sağlar.
Inputmask({mask: "+99 (999) 999 99 99"}).mask(".phonenumber");

// Textboxların içinde phonenumber class kullananları bularak yanlızca email adresi girilmesini sağlar.
Inputmask({
    mask: "*{1,20}[.*{1,20}][.*{1,20}][.*{1,20}]@*{1,20}[.*{2,6}][.*{1,2}]",
    greedy: false,
    onBeforePaste: function (pastedValue, opts) {
        pastedValue = pastedValue.toLowerCase();
        return pastedValue.replace("mailto:", "");
    },
    definitions: {
        "*": {
            validator: '[0-9A-Za-z!#$%&"*+/=?^_`{|}~\-]',
            cardinality: 1,
            casing: "lower"
        }
    }
}).mask(".emailaddress");

///////////////////////////////////////////////////////////////////////
// INPUT FORMAT
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
// DATE  RANGE PICKER
///////////////////////////////////////////////////////////////////////

var start = moment().subtract(29, "days");
var end = moment();

function cb2(start, end) {
    $(".daterange").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));
}

$('.daterange').attr("placeholder", "Tarih Aralığı Seçiniz");

$('.daterange').daterangepicker({
    startDate: start,
    endDate: end,
    ranges: {
        "Bugün": [moment(), moment()],
        "Dün": [moment().subtract(1, "days"), moment().subtract(1, "days")],
        "Son 7 Gün": [moment().subtract(6, "days"), moment()],
        "Son 30 Gün": [moment().subtract(29, "days"), moment()],
        "Bu Ay": [moment().startOf("month"), moment().endOf("month")],
        "Önceki Ay": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")]
    },
    "locale": {
        "format": "DD.MM.YYYY",
        "separator": " - ",
        "applyLabel": "Uygula",
        "cancelLabel": "Vazgeç",
        "fromLabel": "From",
        "toLabel": "ile",
        "customRangeLabel": "Özel",
        "daysOfWeek": [
            "Paz",
            "Pzt",
            "Sal",
            "Çar",
            "Per",
            "Cum",
            "Cts"
        ],
        "monthNames": [
            "Ocak",
            "Şubat",
            "Mart",
            "Nisan",
            "Mayıs",
            "Haziran",
            "Temmuz",
            "Ağustos",
            "Eylül",
            "Ekim",
            "Kasım",
            "Aralık"
        ],
        "firstDay": 1
    }
}, cb2);

$('.daterange-empty').daterangepicker({
    startDate: start,
    endDate: end,
    autoUpdateInput: false,
    ranges: {
        "Bugün": [moment(), moment()],
        "Dün": [moment().subtract(1, "days"), moment().subtract(1, "days")],
        "Son 7 Gün": [moment().subtract(6, "days"), moment()],
        "Son 30 Gün": [moment().subtract(29, "days"), moment()],
        "Bu Ay": [moment().startOf("month"), moment().endOf("month")],
        "Önceki Ay": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")]
    },
    "locale": {
        "format": "DD.MM.YYYY",
        "separator": " - ",
        "applyLabel": "Uygula",
        "cancelLabel": "Vazgeç",
        "fromLabel": "From",
        "toLabel": "ile",
        "customRangeLabel": "Özel",
        "daysOfWeek": [
            "Paz",
            "Pzt",
            "Sal",
            "Çar",
            "Per",
            "Cum",
            "Cts"
        ],
        "monthNames": [
            "Ocak",
            "Şubat",
            "Mart",
            "Nisan",
            "Mayıs",
            "Haziran",
            "Temmuz",
            "Ağustos",
            "Eylül",
            "Ekim",
            "Kasım",
            "Aralık"
        ],
        "firstDay": 1
    }
}, cb2);

$('.daterange-empty').on('apply.daterangepicker', function (ev, picker) {
    $(this).val(picker.startDate.format('DD.MM.YYYY') + ' - ' + picker.endDate.format('DD.MM.YYYY'));
});

$('.daterange-empty').on('cancel.daterangepicker', function (ev, picker) {
    $(this).val('');
});

///////////////////////////////////////////////////////////////////////
// DATE TIME PICKER
///////////////////////////////////////////////////////////////////////

$(".datetimepicker").flatpickr({
    "enableTime": true,
    "dateFormat": "d.m.Y H:i:S",
    "locale": "tr",
    "time_24hr": true,
    "defaultDate": 'null',
});
///////////////////////////////////////////////////////////////////////
// DATE PICKER
///////////////////////////////////////////////////////////////////////
function cb(start, end) {
    $(".datepicker").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));
}

$(".datepicker").daterangepicker({
    singleDatePicker: true,
    showDropdowns: true,
    minYear: 1901,
    maxYear: parseInt(moment().format("YYYY"), 12),
    "locale": {
        "format": "DD.MM.YYYY",
        "separator": " - ",
        "applyLabel": "Uygula",
        "cancelLabel": "Vazgeç",
        "fromLabel": "From",
        "toLabel": "To",
        "customRangeLabel": "Özel",
        "daysOfWeek": [
            "Paz",
            "Pzt",
            "Sal",
            "Çar",
            "Per",
            "Cum",
            "Cts"
        ],
        "monthNames": [
            "Ocak",
            "Şubat",
            "Mart",
            "Nisan",
            "Mayıs",
            "Haziran",
            "Temmuz",
            "Ağustos",
            "Eylül",
            "Ekim",
            "Kasım",
            "Aralık"
        ],
        "firstDay": 1
    }
});
///////////////////////////////////////////////////////////////////////
// MESSAGE BOX
///////////////////////////////////////////////////////////////////////

function ShowSuccess(message) {
    //Swal.fire({
    //    icon: 'success',
    //    title: 'İşlem Başarılı',
    //    text: message,
    //});

    ToastrSuccess(message);
}

function ShowError(message) {
    Swal.fire({
        icon: 'error',
        title: 'İşlem Başarısız',
        text: message,
    })
}

function ShowWarning(message) {
    Swal.fire({
        icon: 'warning',
        title: 'Dikkat',
        text: message,
    })
}

///////////////////////////////////////////////////////////////////////
// MESSAGE BOX
///////////////////////////////////////////////////////////////////////

function removeXss(value) {
    return value.replace(/javascript:/gi, '').replace(/[^\w-_. ]/gi, function (c) {
        return `&#${c.charCodeAt(0)};`;
    });
}

function DeleteRecord(targetUrl) {
    Swal.fire({
        title: "Silmek istediğinize emin misiniz?",
        text: "Bu işlem geri dönülemeyecek şekilde verileri silecektir.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: targetUrl,
                success: function (response) {
                    if (response.isSuccess) {
                        ReloadAjax();
                        ShowSuccess(response.message)
                    } else {
                        ShowError(response.message);
                    }
                },
                error: function (err) {
                    ShowError(err.message);
                }
            });
        }
    });
}

function DeleteRecordAndRefreshPage(targetUrl) {
    Swal.fire({
        title: "Silmek istediğinize emin misiniz?",
        text: "Bu işlem geri dönülemeyecek şekilde verileri silecektir.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: targetUrl,
                success: function (response) {
                    if (response.isSuccess) {
                        location.reload();
                    } else {
                        ShowError(response.message);
                    }
                },
                error: function (err) {
                    ShowError(err.message);
                }
            });
        }
    });
}

toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toastr-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

function ToastrSuccess(text) {
    toastr.success(text, "Başarılı İşlem");
}

function ToastrError(text) {
    toastr.error(text, "Hata Oluştu");
}

function ToastrWarning(text) {
    toastr.warning(text, "Uyarı");
}

function ToastrInfo(text) {
    toastr.info(text, "Bilgi");
}

///////////////////////////////////////////////////////////////////////
// TINY MCE EDITOR
///////////////////////////////////////////////////////////////////////
/*function TinyMceInit() {
    var editors = document.getElementsByClassName("tinymce");
    if (editors.length > 0) {
        var config = {
            selector: 'textarea#' + editors[0].id,
            language_url: 'https://quark.avvamobile.com/js/tinymce/langs/tr.js',
            language: "tr",
            width: '100%',
            height: 800,
            resize: false,
            autosave_ask_before_unload: false,
            powerpaste_allow_local_images: true,
            menubar: false,
            plugins: [
                '  advlist anchor autolink fullscreen',
                ' lists link noneditable preview',
                ' table wordcount code image'
            ],
            toolbar: 'bold italic | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | link image | code'
        };

        var uploadFolderName = editors[0].dataset.tinymceImageUploadFolder;
        if (uploadFolderName != null) {
            config.automatic_uploads = true;
            config.images_upload_url = "/Upload/UploadToCDN?folder=" + uploadFolderName;
        }

        tinymce.init(config);
    }
}

TinyMceInit();*/

///////////////////////////////////////////////////////////////////////
// DROPDOWNLIST
///////////////////////////////////////////////////////////////////////
function LoadDropDownList(id, url) {
    var dropdown = $("#" + id);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: url,
        success: function (response) {
            if (response.isSuccess) {
                dropdown.empty();
                var array = response.data;
                for (i in array) {
                    var newOption = new Option(array[i].text, array[i].value);
                    dropdown.append(newOption);
                }
            } else {
                ShowError(response.message);
            }
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

function ClearDropDownList(id) {
    var dropdown = $("#" + id);
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: "/Common/GetDefaultSelectItems",
        success: function (response) {
            if (response.isSuccess) {
                dropdown.empty();
                var array = response.data;
                for (i in array) {
                    var newOption = new Option(array[i].text, array[i].value);
                    dropdown.append(newOption);
                }
            } else {
                ShowError(response.message);
            }
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

///////////////////////////////////////////////////////////////////////
// TAGIFY
///////////////////////////////////////////////////////////////////////

var tagifyElements = document.querySelectorAll(".tagify");
if (tagifyElements.length > 0) {
    for (var i = 0; i < tagifyElements.length; i++) {
        new Tagify(tagifyElements[i]);
    }
}

///////////////////////////////////////////////////////////////////////
// MAX LENGTH
///////////////////////////////////////////////////////////////////////
$('.maxlength').maxlength({
    warningClass: "badge badge-primary",
    limitReachedClass: "badge badge-success"
});