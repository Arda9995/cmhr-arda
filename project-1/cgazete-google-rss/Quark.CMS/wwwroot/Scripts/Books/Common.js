function ToggleBookUrlGenerate() {
    var titleExist = document.getElementById('Name');

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
        swal.fire("Hata", "Lütfen bir kitap adı giriniz.", "error");
        return;
    }


    prefix = prefix.replace("{CategoryID}", "")
        .replace("{CategoryName}", "kitaplar");

    var slugInput = document.getElementById('basic-url');
    slugInput.value = "";
    slugInput.value += prefix;

    if (prefix.includes("{ID}") || prefix.includes("{Year}") || prefix.includes("{Month}") || prefix.includes("{Date}")) {
        var note = document.getElementById('url-note');
        if (!note.dataset.noteAdded) {
            note.innerHTML += " { } ile gösterilen alanlar haber kaydedildikten sonra güncellenecektir.";
            note.dataset.noteAdded = "true";
        }
    }
}