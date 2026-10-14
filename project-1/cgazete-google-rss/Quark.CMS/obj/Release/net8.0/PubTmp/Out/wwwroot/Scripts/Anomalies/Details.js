function markAsResolved(id){
    $.ajax({
        url: `/Anomalies/MarkAsResolved/${id}`,
        type: "POST",
        success: function(response) {
            window.location.reload();
        }
    })
}