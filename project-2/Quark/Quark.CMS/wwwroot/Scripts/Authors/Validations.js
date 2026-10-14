//$(document).ready(function () {
//    $("#authorForm").on('submit', function (e) {
//        var backgroundPhotoUrl = $('#backgroundPhoto').css('background-image');
//        var backgroundClichePhotoUrl = $('#backgroundClichePhoto').css('background-image');
//        var fileInputValue = $('input[name="ProfilePhoto"]').val();
//        var fileInputClicheValue = $('input[name="ClichePhoto"]').val();

//        var isBackgroundPhotoEmpty = !backgroundPhotoUrl || backgroundPhotoUrl === 'none';
//        var isBackgroundClichePhotoEmpty = !backgroundClichePhotoUrl || backgroundClichePhotoUrl === 'none';
//        var isFileInputEmpty = !fileInputValue;
//        var isFileInputClicheEmpty = !fileInputClicheValue;

//        if (isBackgroundPhotoEmpty || isFileInputEmpty) {
//            e.preventDefault();
//            console.log("test");
//            $("#imageValidation").text(localizer.pleaseUploadPhoto);
//        } else {
//            $("#imageValidation").text("");
//        }

//        if (isBackgroundClichePhotoEmpty || isFileInputClicheEmpty) {
//            e.preventDefault();
//            console.log("test");
//            $("#imageValidationCliche").text(localizer.pleaseUploadPhoto);
//        } else {
//            $("#imageValidationCliche").text("");
//        }
//    });
//});
