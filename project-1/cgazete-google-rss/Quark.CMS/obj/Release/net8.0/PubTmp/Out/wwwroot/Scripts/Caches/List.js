
function Refresh(cacheName){
    Swal.fire({
        html: `${localizer.refreshCache}`,
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
                url: '/Caches/Refresh',
                type: 'POST',
                data : {cacheName : cacheName},
                success: function(response) {
                    ShowSuccess(`${localizer.refreshCacheSuccessMsg}`)

                },
                error: function(response) {
                    ShowError(response.message);
                }
            });
        }
    });
}