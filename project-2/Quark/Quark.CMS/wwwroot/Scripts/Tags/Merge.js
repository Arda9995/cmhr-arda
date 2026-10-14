$(document).ready(function() {
    $('.ajax-search-select2').select2({
        language: "tr",
        ajax: {
            url: '/Tags/SearchTags',
            dataType: 'json',
            delay: 250,
            data: function(params) {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function(data) {
                var formattedData = data.map(function(item) {
                    return {
                        id: item.value,
                        text: item.text
                    };
                });

                return {
                    results: formattedData
                };
            },
            cache: true
        },
        minimumInputLength: 3
    });
});

function confirmMerge(form){
    Swal.fire({
        html: `${localizer.confirmTagMergeMsg}`,
        icon: "warning",
        buttonsStyling: false,
        showCancelButton: true,
        confirmButtonText: `${localizer.yes}`,
        cancelButtonText: `${localizer.no}`,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: 'btn btn-danger'
        }
    }).then((result) => {
        if (!result.isConfirmed) {
            return false;
        }
        form.submit();
    });
}