document.addEventListener("DOMContentLoaded", function () {
    var addOptionButton = document.getElementById("add-option");
    var removeOptionButton = document.getElementById("remove-option");
    var optionsContainer = document.getElementById("options-container");
    var surveyOptionsInput = document.getElementById("SurveyOptions");

    function parseSurveyOptions(value) {
        try {
            var parsed = JSON.parse(value);
            return Array.isArray(parsed) ? parsed : [];
        } catch (e) {
            return [];
        }
    }

    function updateSurveyOptions() {
        var selectedOptions = document.querySelectorAll('[name="SurveyOptions"]');
        surveyOptionsInput.value = JSON.stringify(Array.from(selectedOptions).map(x => x.value));
    }

    function addOption(value = "", index) {
        var newOptionHtml = `
            <div class="row mb-6 option-row">
                <div class="col-1w">
                    <label class="required form-label">${localizer.option} ${index + 1}</label>
                    <div class="input-group">
                        <input name="SurveyOptions" class="form-control maxlength option-${index + 1}" maxlength="100" value="${value}">
                    </div>
                </div>
            </div>`;

        optionsContainer.insertAdjacentHTML('beforeend', newOptionHtml);
    }

    var surveyOptions = parseSurveyOptions(surveyOptionsInput.value);

    if (surveyOptions.length > 0) {
        document.querySelector(".option-1").value = surveyOptions[0] || "";
        document.querySelector(".option-2").value = surveyOptions[1] || "";

        for (let i = 2; i < surveyOptions.length; i++) {
            addOption(surveyOptions[i], i);
        }
    }

    addOptionButton.addEventListener("click", function () {
        var optionRows = document.querySelectorAll(".option-row");
        addOption("", optionRows.length);
        updateSurveyOptions();
    });

    removeOptionButton.addEventListener("click", function () {
        var optionRows = document.querySelectorAll("#options-container .option-row");
        if (optionRows.length > 0) {
            optionRows[optionRows.length - 1].remove();
            updateSurveyOptions();
        }
    });

    document.addEventListener("input", function () {
        updateSurveyOptions();
    });
});
