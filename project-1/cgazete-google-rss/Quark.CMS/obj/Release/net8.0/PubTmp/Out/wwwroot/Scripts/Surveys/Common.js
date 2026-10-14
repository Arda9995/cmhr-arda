function ToggleSurveyUrlGenerate() {
    var titleExist = document.getElementById('Question');

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


    if (title) {
        prefix = prefix.replace("{Title}", titleKeyWord);
    }

    if (prefix.includes("{Title}") && title === "") {
        swal.fire("Hata", "Lütfen anket adı girin.", "error");
        return;
    }


    prefix = prefix.replace("{CategoryID}", "")
        .replace("{CategoryName}", "anketler");

    var slugInput = document.getElementById('basic-url');
    slugInput.value = "";
    slugInput.value += prefix;

    
}