var txtTitle = $("#Title");

txtTitle.click(function () {
    $("#spanTitlePulse").addClass("pulse-ring");
})

txtTitle.blur(function () {
    $("#spanTitlePulse").removeClass("pulse-ring");
});

function ToggleTitleAIDiv() {
    if ($("#divTitleAI").is(":visible")) {
        $("#divTitleAI").hide();
    } else {
        $("#divTitleAI").show();
        $("#divSubtitleAI").hide(); 
    }
}

function ToggleSubtitleAIDiv() {
    if ($("#divSubtitleAI").is(":visible")) {
        $("#divSubtitleAI").hide();
    } else {
        $("#divSubtitleAI").show();
        $("#divTitleAI").hide(); 
    }
}


document.addEventListener('click', function (event) {
    var titleAI = document.getElementById('divTitleAI');
    var titleAIbutton = document.getElementById('openTitleAIPopup');

    var subTitleAI = document.getElementById('divSubtitleAI');
    var subTitleAIbutton = document.getElementById('openSubtitleAIPopup');

    if (!titleAI.contains(event.target) && event.target !== titleAIbutton && !titleAIbutton.contains(event.target)) {
        $("#divTitleAI").hide();
    }


    if (!subTitleAI.contains(event.target) && event.target !== subTitleAIbutton && !subTitleAIbutton.contains(event.target)) {
        $("#divSubtitleAI").hide();
    }
});
