var currentID = 0;
var currentIDHasChildren = false;
var archiveCurrentID = 0;
var archiveCurrentIDHasChildren = false;
var dataTable;
var favDataTable;
var archiveDataTable;
var blockUI = new KTBlockUI(document.querySelector('#media-gallery-main'));
var transferImageId;


$(document).ready(function () {
    var fileColumns = [
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": true, orderable: false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.imageUrl, row.fileName, row.id]); }
        },
        {
            "data": "uploadDate", "name": "UploadDate", "autoWidth": true, orderable: true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.uploadDate, row.formattedUploadDate]); }
        },
        {
            "data": "id", "name": "Id", "class": "",
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id, manageFavouriteIcon(row)]); }
        },
    ];
    dataTable = CreateTable("#filesDatatable", "/MediaGallery/FolderImages/", `${localizer.images}`, fileColumns, applyFilter);

    var favColumns = [
        {
            "data": "imageID", "name": "ImageId", "class": "",
            "render": function (data, type, row) { return GetTableHtml("ColA1", [row.imageID]); }
        },
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": false, orderable: false,
            "render": function (data, type, row) { return GetTableHtml("ColA2", [row.imageUrl, row.fileName, row.tooltip]); }
        },
    ];
    favDataTable = CreateTable("#favouritesDatatable", "/MediaGallery/FavouriteImages/", "favcolumns", favColumns, applyFavFilter);

    var archiveColumns = [
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("ColB1", [row.imageUrl, row.fileName, row.tooltip]); }
        },
        {
            "data": "uploadedBy", "name": "UploadedBy", "class": "", orderable: false,
            "render": function (data, type, row) { return GetTableHtml("ColB2", [row.uploadedBy]); }
        },
    ];
    archiveDataTable = CreateTable("#archivesDatatable", "/MediaGallery/ArchiveFolderImages/", "archivecolumns", archiveColumns, applyArchiveFilter);

    $('#folderSelect').select2({
        dropdownParent: $('#folderSelectModal') // Modal içinde düzgün çalışması için
    });

    $.ajax({
        url: '/MediaGallery/GetTransferFolders',
        type: 'GET',
        success: function (data) {
            if (data.length > 0) {
                const select = document.getElementById('folderSelect');
                select.innerHTML = '<option value="">-- Seçiniz --</option>';
                data.forEach(folder => {
                    const option = document.createElement('option');
                    option.value = folder.id;
                    option.textContent = folder.name;
                    select.appendChild(option);
                });
            }
            else{
                console.log("No transfer folders found.");
            }
        },
        error: function () {
           console.log("Error fetching transfer folders.");
        }
    });

})

function applyFavFilter(c) {
    let search = document.getElementById("search-value").value;
    c.SearchValue = search;
}

function applyFilter(d) {
    let search = document.getElementById("search-value").value;
    d.FolderId = currentID;
    d.SearchValue = search;
}

function applyArchiveFilter(e) {
    let search = document.getElementById("search-value").value;
    e.FolderId = archiveCurrentID;
    e.SearchValue = search;
}

var applyFilterData = () => {
    ReloadAjax(dataTable);
}

var applyFavFilterData = () => {
    // favDataTable.ajax.reload(function()
    //     {
    //         $('[data-bs-toggle="tooltip"]').tooltip()
    //
    //     });
    ReloadAjax(favDataTable);
}

var applyArchiveFilterData = () => {
    ReloadAjax(archiveDataTable);
}

function manageFavouriteIcon(row) {
    if (row.isFavourite) {
        return '<a onClick=" deleteFavourite(' + row.id + ' ) " ' +
            'class="btn btn-icon btn-bg-light btn-active-color-danger btn-sm me-3" data-bs-toggle="tooltip" data-bs-placement="top">' +
            '<i class="ki-solid ki-star fs-2 text-warning"> </i>' +
            '</a>';
    } else {
        return '<a onClick=" addFavourite(' + row.id + ' ) " ' +
            'class="btn btn-icon btn-bg-light btn-active-color-danger btn-sm me-3" data-bs-toggle="tooltip" data-bs-placement="top"> ' +
            '<i class="ki-outline ki-star fs-2 text-warning"> </i>' +
            '</a>';
    }
}

function addFavourite(imageId) {
    blockUI.block();
    $.ajax({
        url: '/MediaGallery/AddFavourite',
        type: 'POST',
        data: {
            imageId: imageId
        },
        success: function (data) {
            ReloadAjax(dataTable);
            ReloadAjax(favDataTable);
            blockUI.release();
        },
        error: function () {
            blockUI.release();
        }
    });
}

function deleteFavourite(imageId) {
    blockUI.block();
    $.ajax({
        url: '/MediaGallery/DeleteFavourite',
        type: 'POST',
        data: {
            imageId: imageId
        },
        success: function (data) {
            ReloadAjax(dataTable);
            ReloadAjax(favDataTable);
            blockUI.release();
        },
        error: function () {
            blockUI.release();
        }
    });
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
        ReloadAjax(dataTable);
        ReloadAjax(favDataTable);
        ReloadAjax(archiveDataTable);
        $("#spanSelectedMediaGallery").text(data.instance.get_node(data.selected[0]).text);
    } else {
        $("#spanSelectedMediaGallery").text("--");
    }
});

$("#divArchiveTree").jstree({
    "core": {
        'data': {
            'url': function (node) {
                if (node.id === "#") {
                    return '/MediaGallery/GetArchiveTree'
                } else {
                    return '/MediaGallery/GetArchiveTree?parent=' + node.id
                }
            },

            'data': function (node) { return { 'parent': node.id }; }
        }
    },
    "plugins": ["state", "types"]
});

$('#divArchiveTree').on("changed.jstree", function (e, data) {
    archiveCurrentID = 0;
    if (data.selected.length > 0) {
        archiveCurrentIDHasChildren = data.node.children.length > 0;
        archiveCurrentID = data.selected;
        console.log(archiveCurrentID)
        ReloadAjax(dataTable);
        ReloadAjax(favDataTable);
        ReloadAjax(archiveDataTable);
        $("#spanSelectedArchive").text(data.instance.get_node(data.selected[0]).text);
    } else {
        $("#spanSelectedArchive").text("--");
    }
});

function getFilePath(foldersIds) {
    $.ajax({
        url: '/MediaGallery/GetFilePath',
        type: 'POST',
        data: {
            foldersIds: foldersIds,
            currentId: currentID
        },
        success: function (data) {
            $('#filePath').empty();
            $('#filePath').append(data);
        },
        error: function (response) {
            blockUI.release();
        },
    });
}

function getImages() {
    var api = "/MediaGallery/FolderImages/?" + "folderId=" + currentID;
    var fileColumns = [
        {
            "data": "imageUrl", "name": "ImageUrl", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.imageUrl, row.fileName]); }
        },
        {
            "data": "uploadDate", "name": "UploadDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.uploadDate]); }
        },
        {
            "data": "uploadedBy", "name": "UploadedBy", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.uploadedBy]); }
        },
        {
            "data": "id", "name": "Id", "class": "",
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id]); }
        },
    ];
    dataTable = CreateTable("#filesDatatable", api, `${localizer.images}`, fileColumns);
}
function RefreshData() {
    $("#divMediaGalleryTree").jstree().refresh();
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
    blockUI.block();

    $.ajax({
        url: '/MediaGallery/UploadImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            location.reload();
            blockUI.release();
        },
        error: function (response) {
            blockUI.release();
        },
    });
}

function AddGallery() {

    var folderName = document.getElementById("folder-name").value;

    var formData = new FormData();
    formData.append("Name", folderName);
    formData.append("ParentId", currentID);

    blockUI.block();
    $.ajax({
        url: '/MediaGallery/AddFolder',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            location.reload();
            blockUI.release();
        },
        error: function (response) {
            blockUI.release();
        },
    });
}

function convertToInput(spanElement, paramValue) {
    spanElement.style.display = 'none';

    const inputElement = document.createElement('input');
    inputElement.type = 'text';
    inputElement.value = spanElement.innerText;
    inputElement.classList.add('form-control');
    inputElement.style.fontSize = '12px';  
    inputElement.style.padding = '5px';    
    inputElement.style.width='70%'

    const buttonElement = document.createElement('button');
    buttonElement.innerText = localizer.save;
    buttonElement.classList.add('btn', 'btn-primary');
    buttonElement.style.fontSize = '12px';  
    buttonElement.style.padding = '5px';    


    buttonElement.onclick = function () {
        saveInput(inputElement.value, paramValue);
    };

    spanElement.parentElement.appendChild(inputElement);
    spanElement.parentElement.appendChild(buttonElement);


    const container = spanElement.parentElement;
    container.appendChild(inputElement);
    container.appendChild(buttonElement);

    document.addEventListener('click', function handleOutsideClick(event) {
        
        if (!container.contains(event.target)) {
      
            spanElement.style.display = 'inline';
            container.removeChild(inputElement);
            container.removeChild(buttonElement);

            document.removeEventListener('click', handleOutsideClick);
        }
    });
}

function saveInput(inputValue, folderImageID) {
    $.ajax({
        url: '/MediaGallery/ChangeImageName',  
        type: 'POST',
        data: {
            imageName: inputValue,
            id: folderImageID 
        },
        success: function (response) {
            toastr.success("Foto�raf ad� de�i�tirildi");
            dataTable.ajax.reload();
        },
        error: function (xhr, status, error) {
       
        }
    });
}

function openTransferModal(imageID){
    transferImageId = imageID;
    $('#folderSelectModal').modal('show');
    
}

function handleFolderSelection() {
    const selectedFolder = document.getElementById("folderSelect").value;

    if (!selectedFolder) {
        alert("Lütfen bir klasör seçin.");
        return;
    }

    $.ajax({
        url: '/MediaGallery/TransferPhoto',
        type: 'POST',
        data: {
            FolderID: selectedFolder,
            ImageID: transferImageId
        },
        success: function (data) {
            if (data.isSuccess) {
                $('#folderSelectModal').modal('hide');
                ReloadAjax(dataTable);
            }
            else{
                console.log("No transfer folders found.");
                $('#folderSelectModal').modal('hide');
                ReloadAjax(dataTable);
            }
        },
        error: function () {
            console.log("Error fetching transfer folders.");
            $('#folderSelectModal').modal('hide');
            ReloadAjax(dataTable);
        }
    });

    $('#folderSelectModal').modal('hide');
}