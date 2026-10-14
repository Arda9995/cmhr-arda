    var newsId = document.querySelector('#ID');
    if (newsId) {
        AddViewCount(newsId.value);
    }


function AddViewCount(newsId) {
    $.ajax({
        type: "GET",
        url: "/News/AddViewCount/" + newsId,
        success: function (response) {
            if (response.isSuccess) {
                
            }
        },
        error: function (error) {
            console.error("Error updating view count:");
        }
    });
}