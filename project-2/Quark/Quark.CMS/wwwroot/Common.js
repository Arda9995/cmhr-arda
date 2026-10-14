var NewsStatusEnum = {
    Draft : 1,
    Published : 2,
    Unpublished : 3,
    Scheduled : 4
}

function Confirm(title, url, id, successFunc, errorFunc){
    Swal.fire({
        html: title,
        icon: "question",
        buttonsStyling: false,
        showCancelButton: true,
        confirmButtonText: `${localizer.yes}`,
        cancelButtonText: `${localizer.no}`,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: 'btn btn-danger'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                data : {id : id},
                success: function(response) {
                    successFunc();
                },
                error: function(response) {
                    errorFunc();
                }
            });
        }
    });
}
 
function SelectDate(title, url, id, successFunc, errorFunc){
    Swal.fire({
        html: `
            <div id="datetimetest">
                <label class="form-label">${title}</label>
                <input class="form-control datetimepicker" id="kt_ecommerce_add_product_status_datepicker" placeholder="${localizer.dateTimePlaceholder}"/>
            </div>`,
        icon: "info",
        buttonsStyling: false,
        showCancelButton: true,
        confirmButtonText: `${localizer.save}`,
        cancelButtonText: `${localizer.cancel}`,
        didOpen: function() {
            $(".datetimepicker").flatpickr({
                "enableTime": true,
                "dateFormat": "d.m.Y H:i",
                "locale": "tr",
                "time_24hr": true,
                "defaultDate": new Date(),
            });
        },
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: 'btn btn-danger'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            var selectedDateTime = $("#kt_ecommerce_add_product_status_datepicker").val();
            $.ajax({
                url: url,
                type: 'POST',
                data: { id : id, date: selectedDateTime },
                success: function(response) {
                    successFunc();

                },
                error: function(xhr) {
                    errorFunc();
                }
            });
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    const overlay = document.createElement("div");
    overlay.className = "lightbox-overlay";
    overlay.innerHTML = `
    <div class="lightbox-content">
      <span class="lightbox-close">&times;</span>
      <img src="" alt="Lightbox Image">
    </div>
  `;
    document.body.appendChild(overlay);

    const lightboxImage = overlay.querySelector("img");
    const closeButton = overlay.querySelector(".lightbox-close");

    document.addEventListener("click", function (event) {
     
        const target = event.target.closest(".custom-lightbox");
        if (target) {
            const imageSrc = target.getAttribute("data-src"); 
            const title = target.getAttribute("data-title") || "Resim"; 
            lightboxImage.src = imageSrc;
            lightboxImage.alt = title;
            overlay.style.display = "flex";
        }
    });

    closeButton.addEventListener("click", () => {
        overlay.style.display = "none";
        lightboxImage.src = "";
    });

    overlay.addEventListener("click", event => {
        if (event.target === overlay) {
            overlay.style.display = "none";
            lightboxImage.src = "";
        }
    });
});

