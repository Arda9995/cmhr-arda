var currentID = 0;
var currentIDHasChildren = false;
var dataTable;
var blockMediaGalleryUI = new KTBlockUI(document.querySelector('#media-gallery-main'));
var modal = document.getElementById('media_gallery_modal');

$(document).ready(function () {
    CreateDataTable();
})

function CreateDataTable() {
    var api = "/MediaGallery/FolderImages/";
    var fileColumns = [
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": true, orderable: false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.imageUrl, row.fileName]); }
        },
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": true, orderable: true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.imageUrl, row.fileName]); }
        },
        {
            "data": "uploadDate", "name": "UploadDate", "autoWidth": true, orderable: true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.uploadDate, row.formattedUploadDate]); }
        },
        {
            "data": "selectPhoto", "name": "SelectPhoto", "class": "text-end", "autoWidth": true, orderable: false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.imageUrl]); }
        },
    ];
    dataTable = CreateTable("#mediaGalleryFilesDatatable", api, "Resimler", fileColumns, applyFilter);
}

function applyFilter(d) {
    d.FolderId = currentID;
    d.SearchValue = document.getElementById("search-value-input").value;
}

var applyFilterData = () => {
    ReloadAjax(dataTable);
}

$("#divMediaGalleryTree").jstree({
    "core": {
        'data': {
            'url': function (node) {
                if (node.id === "#") {
                    return '/MediaGallery/GetGalleryTree'
                } else {
                    return '/MediaGallery/GetGalleryTree?parent=' + node.id
                }
            },

            'data': function (node) { return { 'parent': node.id }; }
        }
    },
    "plugins": ["state", "types"]
});

$('#divMediaGalleryTree').on("changed.jstree", function (e, data) {
    currentID = 0;
    if (data.selected.length > 0) {
        currentIDHasChildren = data.node.children.length > 0;
        currentID = data.selected;
        getFilePath(data.node.parents);
        ReloadAjax();
        $("#spanSelectedMediaGallery").text(data.instance.get_node(data.selected[0]).text);
    } else {
        $("#spanSelectedMediaGallery").text("--");
    }
});
function getFilePath(foldersIds) {
    blockMediaGalleryUI.block();
    $.ajax({
        url: '/MediaGallery/GetFilePath',
        type: 'POST',
        data: {
            foldersIds: foldersIds,
            currentId: currentID
        },
        success: function (data) {
            blockMediaGalleryUI.release();
            $('#filePath').empty();
            $('#filePath').append(data);
        },
        error: function (response) {
            blockMediaGalleryUI.release();
        },
    });
}

function SelectPhoto(imageUrl) {
    var mediaGalleryModal = $('#media_gallery_modal').find('.modal-body');
    var mg_img_id = mediaGalleryModal.attr('id');

    if (mg_img_id === "tinymce-photogallery") {
        if (tinymce.activeEditor) {
            tinymce.activeEditor.insertContent(`<p><img src="${imageUrl}" alt="Image" class="img-fluid" /></p>`);
            ShowSuccess("Fotoğraf başarıyla TinyMCE editörüne eklendi.");
        } else {
            console.warn("TinyMCE editör tanımlı değil.");
        }

        $('#media_gallery_modal').modal('hide');
    }
    else if (mg_img_id == "news-photogallery-type") {
        var photoGallerySelector = "tab-content-photo-gallery";
        var container = document.querySelector("." + photoGallerySelector);
        var tabPanes = container.getElementsByClassName("photo-item");

        var tabCount = tabPanes.length + 1;
        var newTabId = `tab_${tabCount}`;

        var newPhoto = document.createElement("div");
        newPhoto.classList.add("col-md-3", "p-2", "photo-item");
        newPhoto.setAttribute("id", newTabId);
        newPhoto.setAttribute("data-order", tabCount);

        newPhoto.innerHTML = `
                <div class="photo-items position-relative">
                    <img src="${imageUrl}" alt="${imageUrl}" class="img-fluid cursor-move" />
                        <button class="btn p-0 btn-danger position-absolute top-0 end-0 m-1 delete-photo" data-id="${tabCount}">
                                <i class="ki-duotone ki-cross-circle p-0 fs-1 text-light">
                                    <span class="path1"></span><span class="path2"></span>
                                </i>
                        </button>
                </div>`;

        container.appendChild(newPhoto);

        updatePhotoOrders();

        ShowSuccess("Fotoğraf başarıyla seçildi.");
        $('#media_gallery_modal').modal('hide');

    }
    else {
        var divInput = document.getElementById(mg_img_id);
        divInput.style.backgroundImage = "url('" + imageUrl + "')";

        var model_img_id = mg_img_id.split('_')[1];
        var model_img = document.getElementById('hiddenPhotoUrl_' + model_img_id);
        var file_img = $("#fileImage_" + model_img_id);
        var imageInputWrapper = $("#divImageInputWrapper_" + model_img_id);

        imageInputWrapper.css('background-image', 'url(' + imageUrl + ')');
        model_img.value = imageUrl;
        file_img[0].value = '';

        ShowSuccess("Fotoğraf başarıyla seçildi.");
        $('#media_gallery_modal').modal('hide');

        $(`#photoEditButton_${model_img_id}`).removeClass('d-none').addClass('d-flex');
        $(`#removeButton_${model_img_id}`).removeClass('d-none').addClass('d-flex');
    }
}


function uploadPhotos(input) {
    const files = input.files;
    for (let i = 0; i < files.length; i++) {
        uploadPhoto(files[i]);
    }
}

function uploadPhoto(file) {

    if (currentID == 0) {
        ShowError(`${localizer.selectMenuErrorMsg}`);
        return;
    }


    var formData = new FormData();
    formData.append("Image", file);
    formData.append("FolderId", currentID);
    blockMediaGalleryUI.block();

    $.ajax({
        url: '/MediaGallery/UploadImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            dataTable.ajax.reload();
            blockMediaGalleryUI.release();
        },
        error: function (response) {
            blockMediaGalleryUI.release();
        },
    });
}



function updatePhotoOrders() {
    var items = document.querySelectorAll(".photo-item");
    items.forEach((item, index) => {
        var newOrder = index + 1;
        item.setAttribute("id", "tab_" + newOrder);
        item.setAttribute("data-order", newOrder);
    });
}