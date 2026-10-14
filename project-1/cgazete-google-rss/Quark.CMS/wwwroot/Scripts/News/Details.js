$(document).ready(function () {
    var videoPlayerID = $('#videoPlayerID').val();
    var videoCode = $('#videoCode').val();
    LoadVideoPlayer(videoPlayerID, videoCode);
});

var isLocked = document.getElementById('lock-check').value;
var isLockedByCurrentUser = document.getElementById('user-check').value;
var lockedBy = document.getElementById('locked-by').value;
var toolbarLeft = new KTBlockUI(document.querySelector('#kt_app_content'));
var newsRelationsDatatable;
var unlockHtml = "";

if (isLocked === "True" && isLockedByCurrentUser === "False") {
    unlockHtml = `${localizer.unlockMsg1}` + lockedBy + `${localizer.unlockMsg2}`
} else {
    unlockHtml = `${localizer.unlockMsg3}`
}

try {
    const publishButton = document.getElementById('publish');
    publishButton.addEventListener('click', handlePublishButtonClick);
} catch (error) {

}

try {
    const scheduledButton = document.getElementById('scheduled');
    scheduledButton.addEventListener('click', handleScheduledButtonClick);
} catch (error) {

}

try {
    const unScheduleButton = document.getElementById('unschedule');
    unScheduleButton.addEventListener('click', handleUnScheduleButtonClick);
} catch (error) {

}

try {
    const lockButton = document.getElementById('lock');
    lockButton.addEventListener('click', handleLockButtonClick);
} catch (error) {

}


try {
    const unlockButton = document.getElementById('unlock');
    unlockButton.addEventListener('click', handleUnLockButtonClick);
} catch (error) {

}

function ReloadFunc() {
    location.reload();
}



function handlePublishButtonClick(e) {
    e.preventDefault();
    var id = $("#newsID").val();
    
    Swal.fire({
        html: `${localizer.confirmPublishMsg}`,
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
            var root = new KTBlockUI(document.querySelector('#kt_app_root'));
            root.block();

            $.ajax({
                url: '/News/PublishNowOrScheduled',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    root.release();
                    ReloadFunc();
                },
                error: function (response) {
                    root.release();
                    ReloadFunc();
                }
            });
        }
    });
}

function unPublish(id) {
    var root = new KTBlockUI(document.querySelector('#kt_app_root'));
    var desc = document.getElementById('description').value;
    root.block();
    if (desc === '') {
        toastr.error("Yorum alanı boş olamaz", "Hata");
    } else {
        bootstrap.Modal.getInstance(document.getElementById('unpublishModal'))?.hide();
        $.ajax({
            url: "/News/UnPublish/",
            type: 'POST',
            data: { id: id, description: desc },
            success: function (response) {
                root.release();
                ReloadFunc();
            },
            error: function (response) {
                root.release();
                ReloadFunc();
            }
        });
    }
}

function handleScheduledButtonClick(e) {
    e.preventDefault();
    var id = $("#newsID").val();
    SelectDate(`${localizer.selectDateMsg}`, '/News/PublishScheduled/', id, ReloadFunc, ReloadFunc);
}

function handleUnScheduleButtonClick(e) {
    e.preventDefault();
    var id = $("#newsID").val();
    Confirm(`${localizer.confirmUnScheduleMsg}`, '/News/UnSchedule', id, ReloadFunc, ReloadFunc);
}

function handleLockButtonClick(e) {
    e.preventDefault();
    var id = $("#newsID").val();
    Confirm(`${localizer.lockMsg}`, '/News/Lock', id, ReloadFunc, ReloadFunc);
}

function handleUnLockButtonClick(e) {
    e.preventDefault();
    var id = $("#newsID").val();
    Confirm(unlockHtml, '/News/UnLock', id, ReloadFunc, ReloadFunc);
}



function DeleteNewsRelations(targetUrl) {

    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: targetUrl,
        success: function (response) {
            if (response.isSuccess) {
                ReloadAjax(newsRelationsDatatable);
                ShowSuccess(response.message)
            } else {
                ShowError(response.message);
            }
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

$(document).ready(function () {
    var newsColumns = [
        {
            "data": "photoUrl", "name": "Photo", "orderable": false, "class": "align-top",
            "render": function (data, type, row) {
                return GetTableHtml("Col10", ['<img src=' + row.photoUrl + ' alt=' + row.title + ' class="w-75px shadow-sm rounded border" />']);
            }
        },
        {
            "data": "title", "name": "Title", "autoWidth": true, "class": "align-top",
            "render": function (data, type, row) {
                return GetTableHtml("Col11", [row.id, row.title, row.category, row.type, row.newsID]);
            }
        },
        {
            "data": "publishedDate", "name": "PublishedDate", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col12", [row.publishedDate]);
            }
        },
        {
            "data": "ID", "name": "ID", "class": "w-50px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col13", [row.id]);
            }
        }
    ];
    var datatable = CreateTable("#newsDatatable", "/News/GetRelatedNewsPaged/" + newsID, `${localizer.relatedNews}`, newsColumns);

    var publishedNewsColumns = [
        {
            "data": "photoUrl",
            "name": "Photo",
            "orderable": false,
            "class": "align-top",
            "render": function (data, type, row) {
                return GetTableHtml("Col20", [
                    '<img src="' + row.photoUrl + '" alt="' + row.title + '" class="w-75px shadow-sm rounded border" onclick="addSelectedNews(\'' + row.id + '\')" style="cursor: pointer;" />',
                    row.id
                ]);
            }

        },
        {
            "data": "title", "name": "Title", "autoWidth": true, "class": "align-top w-100px",
            "render": function (data, type, row) {
                return GetTableHtml("Col21", [row.id, row.title.truncate(40), row.category, row.type]);
            }
        },
        {
            "data": "updatedDate", "name": "UpdatedDate", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col22", [row.updatedDate]);
            }
        },
        {
            "data": "publishedDate", "name": "PublishedDate", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col23", [row.publishedDate]);
            }
        },
        {
            "data": "id", "name": "ID", "class": "w-25px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col24", [row.id]);
            }
        },
        {
            "data": "id", "name": "ID", "class": "w-25px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col25", [row.id]);
            }
        },
        {
            "data": "id", "name": "ID", "class": "w-100px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col26", [row.id]);
            }
        }
    ];
    newsRelationsDatatable = CreateTable("#publishedNewsDatatable", "/News/GetAllPublishedNewsPaged/" + newsID, `${localizer.relatedNews}`, publishedNewsColumns);
})

var applyFilterGetAllPublishedNewsPaged = () => {
    var searchValue = document.getElementById("search-value").value;
    newsRelationsDatatable.search(searchValue).draw();
}

function addSelectedNews(relatedNewsID) {
    var id = $("#newsID").val();
    $.ajax({
        type: 'POST',
        dataType: 'json',
        data: {
            newsID: id,
            relatedNewsID: relatedNewsID
        },
        url: "/News/SelectRelatedNews",
        success: function (response) {
            if (response.isSuccess) {
                var button = $('.selectButton_' + relatedNewsID);
                var removeButton = $('.deleteButton_' + relatedNewsID);
                button.removeClass('btn-primary');
                removeButton.removeClass('d-none');
                button.addClass('btn-success');
                button.addClass('pe-none');
                button.html('<i class="fas fa-check"></i>');
                ShowSuccess(response.message)
            } else {
                ShowError(response.message);
            }
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

function removeSelectedNews(relatedNewsID) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        data: {
            newsID: newsID,
            relatedNewsID: relatedNewsID
        },
        url: "/News/RemoveRelatedNewsByNewsID/",
        success: function (response) {
            if (response.isSuccess) {
                var button = $('.selectButton_' + relatedNewsID);
                var removeButton = $('.deleteButton_' + relatedNewsID);
                removeButton.addClass('d-none');
                button.addClass('btn-primary');
                button.removeClass('btn-success');
                button.removeClass('pe-none');
                button.text(localizer.select);
                ShowSuccess(response.message)
            } else {
                ShowError(response.message);
            }
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

$(document).ready(function () {
    var relatedNewsModal = $('#publishedNewsModal');
    relatedNewsModal.on('hidden.bs.modal', function () {
        location.reload();
    });
});


function ChangeOrder() {
    var id = $("#newsID").val();

    var items = document.getElementsByClassName("photoGalleryItem");

    var orders = [];

    for (var i = 0; i < items.length; i++) {
        orders.push(items[i].id);
    }

    $.ajax({
        type: "POST",
        url: "/News/ChangePhotoGalleryItemOrder/",
        data: {
            id: id,
            orders: orders
        },
        success: function (response) {
            ReloadFunc();
        },
        error: function (err) {
            ShowError(err.message);
        }
    });
}

var KTDraggableSwappable = {
    init: function () {
        var zones = document.querySelectorAll(".draggable-zone");
        if (zones.length === 0) return false;

        function shuffleElements(container) {
            var elementsArray = Array.prototype.slice.call(container.children);
            for (var i = elementsArray.length - 1; i > 0; i--) {
                var j = Math.floor(Math.random() * (i + 1));
                container.appendChild(elementsArray[j]);
                elementsArray.splice(j, 1);
            }
        }

        new Draggable.Sortable(zones, {
            draggable: ".draggable",
            handle: ".draggable .draggable-handle",
            mirror: {
                appendTo: "modal-body",
                constrainDimensions: true
            },
        });

        var shuffleButton = document.getElementById("shuffleButton");
        if (shuffleButton) {
            shuffleButton.addEventListener("click", function () {
                zones.forEach(function (zone) {
                    shuffleElements(zone);
                });
            });
        }
    }
};

KTUtil.onDOMContentLoaded(function () {
    KTDraggableSwappable.init();
});

function CreatePhotoGallery() {
    var subtitle = $("#create_subTitle").val();

}

const savePhotoGalleryBtn = document.getElementById('savePhotoGalleryItemBtn');

if (savePhotoGalleryBtn) {
    savePhotoGalleryBtn.addEventListener("click", function () {
        var content = $("#modalContent").val();
        var photoUrl = $("#photo_url").val();
        var subtitle = $("#modalSubtitle").val();
        var newsId = $("#newsID").val();
        var itemId = $("#itemID").val();
        var formData = new FormData();
        formData.append('file', $("#create_photo")[0].files[0]);
        formData.append('newsId', newsId);
        formData.append('subtitle', subtitle);
        formData.append('content', content);
        formData.append('itemId', itemId);
        var file = formData.get('file')

        if (!(itemId > 0)) {
            $.ajax({
                type: "POST",
                url: "/News/CreatePhotoGalleryItem/",
                data: formData,
                processData: false,  // Important
                contentType: false,  // Important
                success: function (response) {
                    window.location.replace("/News/Details/" + newsId + "/#photoGalleryItems")
                },
                error: function (err) {
                    ShowError(err.message);
                }
            });
        } else {
            $.ajax({
                type: "POST",
                url: "/News/UpdatePhotoGalleryItem/",
                data: formData,
                processData: false,  // Important
                contentType: false,  // Important
                success: function (response) {
                    ReloadToPhotoGalleryFunc(newsId);
                },
                error: function (err) {
                    ShowError(err.message);
                }
            });
        }

    });
}

function TriggerModal(itemId, subtitle, content, photoUrl) {
    var createModal = new bootstrap.Modal(document.getElementById('createPhotoGalleryModal'));
    if (itemId === undefined) {
        document.getElementById("modalSubtitle").value = null;
        $('#modalContent').summernote('code', null);
        document.getElementById("photoUrl").value = null;
        document.getElementById("create_backgroundPhoto").style.backgroundImage = null;
        document.getElementById("itemID").value = undefined;

        createModal.show();
    } else {
        document.getElementById("modalSubtitle").value = subtitle;
        $('#modalContent').summernote('code', content);
        document.getElementById("photoUrl").value = null;
        document.getElementById("create_backgroundPhoto").style.backgroundImage = `url(${photoUrl})`;
        document.getElementById("itemID").value = itemId;

        createModal.show();
    }
}

function DeletePhotoGalleryItem(id) {
    Confirm(`${localizer.confirmDeletePhotoGalleryMsg}`, '/News/DeletePhotoGalleryItem', id, ReloadFunc, ReloadFunc);
}

function ReloadToPhotoGalleryFunc(id) {
    window.location.href("/News/Details/" + id + "/#photoGalleryItems")

}

function LoadVideoPlayer(videoPlayerID, videoCode) {
    if (videoPlayerID == 1) {

    }
    else if (videoPlayerID == 2) {
        dailymotion
            .createPlayer("my-dailymotion-player", {
                video: videoCode,
                referrerPolicy: 'no-referrer-when-downgrade',
                params: {
                    mute: true,
                    customConfig: {
                        autoplay: true,
                        controls: true,
                        // keyvalues: 'video_position=inpage&page_category=' + slugKategoriArray.toString(),
                        plcmt: 2
                    }
                },
            })
            .then((player) => { console.log(player); player.mute(); })
            .catch((e) => console.error(e));
    }
}

function TruncateString(str, limit = 20) {
    return str.length > limit ? str.substring(0, limit) + "..." : str;
}
String.prototype.truncate = function (length = 20) {
    if (this.length > length) {
        return this.substring(0, length) + '...';
    }
    return this;
};


$(document).ready(function () {
    var el = document.querySelector('.tab-content-photo-gallery');
    var sortable = new Sortable(el, {
        animation: 150,
        onEnd: function (evt) {
            var items = document.querySelectorAll(".photo-item");
            items.forEach((item, index) => {
                var newOrder = index + 1;
                item.setAttribute("id", "tab_" + newOrder);
                item.setAttribute("data-order", newOrder);
            });
        }
    });

});

$(document).on('click', '.mediaGalleryForPhotoGallery', function () {
    var img_id = "news-photogallery-type";
    var modalBody = $('#media_gallery_modal').find('.modal-body');
    modalBody.attr('id', img_id);
    $('#media_gallery_modal').modal('show');
});

$(document).on('click', '.delete-photo', function () {
    var photoItem = $(this).closest('.photo-item');
    photoItem.remove();
    updatePhotoOrders();
});


function SendPhotoGalleryItems() {
    var id = $("#newsID").val();
    var photoItems = document.querySelectorAll(".photo-item");

    var photoData = [];
    photoItems.forEach(function (item) {
        var imgUrl = item.querySelector("img").src;
        var content = item.querySelector(".content");
        var contentValue = "";
        if (content) {
            contentValue = content.value;
        }
        var order = item.getAttribute("data-order");
        photoData.push({ ImageURL: imgUrl, Order: order, Content: contentValue });
    });

    var requestData = {
        Items: photoData,
        NewsID: id
    };

    $.ajax({
        url: '/News/SavePhotoGalleryItems',
        type: 'POST',
        data: JSON.stringify(requestData),
        contentType: 'application/json',
        success: function (response) {
            toastr.success('Fotoğraflar başarıyla gönderildi!');
            location.reload();
        },
        error: function (error) {
            toastr.error('Fotoğraflar gönderilemedi!');
        }
    });
}
function BlendAndSendPhotoGalleryItems() {
    var id = $("#newsID").val();
    var photoItems = document.querySelectorAll(".photo-item");

    var photoData = [];
    var orderNumbers = [];
    photoItems.forEach(function (item) {
        var imgUrl = item.querySelector("img").src;
        var content = item.querySelector(".content").value;
        var order;
        do {
            order = Math.floor(Math.random() * photoItems.length) + 1;
        } while (orderNumbers.includes(order));

        orderNumbers.push(order);

        photoData.push({ ImageURL: imgUrl, Order: order, Content: content });
    });

    var requestData = {
        Items: photoData,
        NewsID: id
    };

    $.ajax({
        url: '/News/SavePhotoGalleryItems',
        type: 'POST',
        data: JSON.stringify(requestData),
        contentType: 'application/json',
        success: function (response) {
            toastr.success('Fotoğraflar başarıyla gönderildi!');
            location.reload();
        },
        error: function (error) {
            toastr.error('Fotoğraflar gönderilemedi!');
        }
    });
}

function ClearCloudflareCache() {
    var url = $("#news-full-url").val();
    $.ajax({
        url: `/News/ClearCloudflareCache?url=${url}`,
        type: 'POST',
        contentType: 'application/json',
        success: function (response) {
            toastr.success('Cloudflare cache guncellendi');
        },
        error: function (error) {
            toastr.error('Cloudflare cache guncellenemedi');
        }
    });
}


$(document).ready(function () {

    $('input[id^="photogallery-content-input_"]').each(function () {
        var contentValue = $(this).val(); 
        var id = $(this).attr('id').split('_')[1]; 

        if (contentValue) {
            $('#preview-content_' + id).html(contentValue);
        }
    });

    $('.edit-photo').on('click', function () {
        var photoId = $(this).data('id');
        var currentContent = $('#photogallery-content-input_' + photoId).val();
        $('#photo-id').val(photoId);
        tinymce.get("photo-content").setContent(currentContent)
        //$('#photo-content').summernote('code', currentContent);
        $('#photo-gallery-content-modal').modal('show');
    });

    $('#save-photo-content').on('click', function () {
        var photoId = $('#photo-id').val();
        var updatedContent = tinymce.get("photo-content").getContent();

        $('#photogallery-content-input_' + photoId).val(updatedContent);

        var contentValue = $('#photogallery-content-input_' + photoId).val();

        $('#preview-content_' + photoId).html(contentValue);
        $('#photo-gallery-content-modal').modal('hide');

    });
});
