var maxCount = $("#MaxNewsCount");
var minCount = $("#MinNewsCount");
var blockForm = $("#blockForm");
var maxCountMessage = $(".maxCountMessage");
var minCountMessage = $(".minCountMessage");

$(document).ready(function () {
    blockForm.submit(function (event) {
        var minVal = parseInt(minCount.val(), 10);
        var maxVal = parseInt(maxCount.val(), 10);

        var valid = true;

        if (minVal > maxVal) {
            minCountMessage.text(localizer.blockMinCountWarnMessage).show();
            maxCountMessage.text(localizer.blockMaxCountWarnMessage).show();
            valid = false;
        } else {
            minCountMessage.hide();
            maxCountMessage.hide();
        }

        if (!valid) {
            event.preventDefault();
        }
    });
});
