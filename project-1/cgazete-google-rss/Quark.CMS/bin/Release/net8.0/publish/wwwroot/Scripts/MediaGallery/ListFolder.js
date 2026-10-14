$(document).ready(function () {
    var folderColumns = [
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.name,row.id]); }
        },
        {
            "data": "createdUserName", "name": "CreatedUserName", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.createdUserName,row.id]); }
        },
        {
            "data": "id", "name": "Id", "autoWidth": true,"orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.createdDateTime,row.id]); }
        },
        {
            "data": "createdDateTime", "name": "CreatedDateTime", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id]); }
        }
    ];
    CreateTable("#foldersDatatable", "/MediaGallery/GetAllFolders", `${localizer.folders}`, folderColumns)
    handleSearchDatatable();
})
