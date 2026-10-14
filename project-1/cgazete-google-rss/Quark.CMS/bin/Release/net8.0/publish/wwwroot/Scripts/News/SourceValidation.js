$(document).ready(function () {
    $("#newsFormCreate").on('submit', function (e) {
        const fields = [
            { id: "#SourceID", messageClass: ".sourceValidationMessage" },
            { id: "#AuthorID", messageClass: ".AuthorValidationMessage" },
            { id: "#SelectedZodiacSignID", messageClass: ".ZodiacSignValidationMessage" },
            { id: "#CaricaturistID", messageClass: ".CaricaturistValidationMessage" }
        ];

        fields.forEach(field => {
            const input = $(field.id);
            const message = $(field.messageClass);
            if (input.length && input.val() == -1) {
                e.preventDefault();
                message.text(localizer.pleaseFillTheSourceField);
            } else {
                message.text("");
            }
        });
    });
});
