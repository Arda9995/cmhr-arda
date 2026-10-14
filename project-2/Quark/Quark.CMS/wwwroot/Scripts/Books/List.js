$(document).ready(function () {
    var columns = [
        {
            "data": "imageURL", "name": "ImageURL", "orderable": false, "class": "align-top text-center",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.imageURL, row.name]); }
        },
        {
            "data": "name", "name": "Name", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.name]); }
        },
        {
            "data": "author", "name": "Author", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.author]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Books/GetAllPaged", "Books", columns/*, applyFilterData*/)
    handleSearchDatatable();
})

//function applyFilterData(data) {
//    data.isEnabled = document.querySelector('#IsEnabled').value;
//}

//var applyFilter = () => {
//    ReloadAjax();
//}


