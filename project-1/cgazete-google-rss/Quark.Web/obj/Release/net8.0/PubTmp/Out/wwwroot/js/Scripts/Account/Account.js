document.addEventListener("DOMContentLoaded", function () {
    // onchange attribute olarak fonksiyonlarý ata
    const country = document.getElementById("SelectedCountryID");
    if (country) country.onchange = function () {
        loadCities(this.value, "sehir_select_list");
    };

    const city = document.getElementById("sehir_select_list");
    if (city) city.onchange = function () {
        loadDistricts(this.value, "ilce_select_list");
    };

    const firmaCountry = document.getElementById("SelectedCompanyCountryID");
    if (firmaCountry) firmaCountry.onchange = function () {
        loadCities(this.value, "firma_sehir_select_list");
    };

    const firmaCity = document.getElementById("firma_sehir_select_list");
    if (firmaCity) firmaCity.onchange = function () {
        loadDistricts(this.value, "firma_ilce_select_list");
    };

    // Þehirleri getir
    function loadCities(countryId, targetSelectId) {
        if (!countryId) {
            clearSelect(targetSelectId);
            return;
        }

        const xhr = new XMLHttpRequest();
        xhr.open("GET", `/Location/GetCitiesByCountry?countryId=${countryId}`, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                try {
                    const data = JSON.parse(xhr.responseText);
                    const select = document.getElementById(targetSelectId);
                    clearSelect(targetSelectId);
                    addOption(select, '', 'Seciniz');
                    data.forEach(item => {
                        addOption(select, item.id, item.name);
                    });
                } catch (e) {
                }
            }
        };
        xhr.send();
    }

    // Ýlçeleri getir
    function loadDistricts(cityId, targetSelectId) {
        if (!cityId) {
            clearSelect(targetSelectId);
            return;
        }

        const xhr = new XMLHttpRequest();
        xhr.open("GET", `/Location/GetDistrictsByCity?cityId=${cityId}`, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                try {
                    const data = JSON.parse(xhr.responseText);
                    const select = document.getElementById(targetSelectId);
                    clearSelect(targetSelectId);
                    addOption(select, '', 'Ilce Seciniz');
                    data.forEach(item => {
                        addOption(select, item.id, item.name);
                    });
                } catch (e) {
                    console.error("loadDistricts parse error:", e);
                }
            }
        };
        xhr.send();
    }

    // Seçimi temizle
    function clearSelect(selectId) {
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '';
        }
    }

    // Seçenek ekle
    function addOption(select, value, text) {
        const option = document.createElement("option");
        option.value = value;
        option.text = text;
        select.appendChild(option);
    }
});
