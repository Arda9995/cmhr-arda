var timer;
var locationId = 4;
var modal = document.getElementById('kt_modal_stacked_1');

var blockUI = new KTBlockUI(document.querySelector('#block-modal'));
const form = document.querySelector('#form1');

function getTemplates(){

    $.ajax({
        url: "/News/GetEditTemplates",
        type: "GET",
        success: function (response) {
            var templates1 = document.getElementById("templates1");
            var templates2 = document.getElementById("templates2");

            response.forEach(function(eleman) {
                var optionElement1 = document.createElement("option");
                var optionElement2 = document.createElement("option");

                optionElement1.text = eleman.name;
                optionElement1.value = eleman.id;

                optionElement2.text = eleman.name;
                optionElement2.value = eleman.id;

                templates1.appendChild(optionElement1);
                templates2.appendChild(optionElement2);
            });
        },
        error: function (response) {
            console.error(response);
        },
    });
}

getTemplates()

form.addEventListener('change', function () {
    if (timer) {
        clearTimeout(timer);
    }
    timer = setTimeout(preparePhoto, 1000);
});

document.querySelectorAll('#groupInput input[type="radio"]').forEach(input => {
    input.addEventListener('change', function() {
        if (this.checked) {
            locationId = this.value;
        }
    });
});

function preparePhoto() {
    var photoUrl =document.querySelector('#edited-photo-org');
    var classId = photoUrl.className;
    var locationID = locationId;
    var text1 = document.getElementById("text1").value;
    var thema1 = document.getElementById("templates1").value;
    var text2 = document.getElementById("text2").value;
    var thema2 = document.getElementById("templates2").value;
    
    var formData = {
        "PhotoUrl" : photoUrl.src,
        "LocationId" : locationID,
        "Text1" : text1,
        "Text2" : text2,
        "Thema1Id" : thema1,
        "Thema2Id" : thema2,
    }

    $.ajax({
        url: "/News/PreparePhoto",
        type: "POST",
        data: formData,
        success: function (response) {
            var html = "<img id='edited-photo' class='edited-photo' src='data:image/png;base64," + response.data.base64Image + "' alt='' style='width: 100%; height: 100%'/>";
            html += "<img id='edited-photo-org' class='"+ classId +"' src='" + photoUrl.src + "' alt='' style='display: none;'/>"
            $("#photo").html(html);
        },
        error: function (response) {
        },
    });
}

function savePhoto(){
    blockUI.block();
    var photoUrl = document.querySelector('.edited-photo');
    var orgPhoto =document.querySelector('#edited-photo-org');
    var base64Image = photoUrl.src.split(',')[1];
    
    var formData = {
        "Base64Photo" : base64Image,
    }
    $.ajax({
        url: "/News/SaveEditedPhoto",
        type: "POST",
        data: formData,
        success: function (response) {
            blockUI.release();
            var responseUrl = response.data.photoUrl;
            var divClassName = orgPhoto.className;
            
            var div = document.getElementById(divClassName);
            div.style.backgroundImage = "url('" + responseUrl + "')";
            
            var model_img_id = divClassName.split('_')[1];
            var model_img = document.getElementById('hiddenPhotoUrl_' + model_img_id);
            model_img.value = responseUrl;
            
            ShowSuccess("Fotoğraf başarıyla kaydedildi.");
            $('#kt_modal_stacked_1').modal('hide');
        },
        error: function (response) {
            blockUI.release();
        },
    });
}

modal.addEventListener('show.bs.modal', function(event) {
    var text1 = document.getElementById("text1");
    var text2 = document.getElementById("text2");
    text1.value = "";
    text2.value = "";
    
    var selectItem1 = document.getElementById("templates1");
    var selectItem2 = document.getElementById("templates2");
    
    selectItem1.innerHTML = "";
    var optionElement1 = document.createElement("option");
    optionElement1.text = "--Seçiniz--";
    optionElement1.selected = true;
    optionElement1.disabled = true;
    selectItem1.appendChild(optionElement1);
    
    selectItem2.innerHTML = "";
    var optionElement2 = document.createElement("option");
    optionElement2.text = "--Seçiniz--";
    optionElement2.selected = true;
    optionElement2.disabled = true;
    selectItem2.appendChild(optionElement2);

    locationId = 4;
    var radioButtons = document.querySelectorAll('#groupInput input[type="radio"]');
    radioButtons.forEach(function(radio) {
        if (radio.value == 4) {
            radio.checked = true;
        }
    });
    
    
    getTemplates()
});