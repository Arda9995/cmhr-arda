$(document).ready(function () {
    var booksColumns = [
        {
            "data": "Id", "name": "ID", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id]); }
        },
        {
            "data": "Name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.name]); }
        },
        {
            "data": "Description", "name": "Description", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.description]); }
        },
        {
            "data": "SeoTitle", "name": "SeoTitle", "autoWidth": true, "class": "text-center",
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.SeoTitle]); }
        },
        {
            "data": "SeoDescription", "name": "SeoDescription", "autoWidth": true, "class": "text-center",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.passiveUserCount]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    var datatable = CreateTable("#datatable", "/Books/GetNewsPaged/", `${localizer.books}`, booksColumns, applyFilterData, 20, booksOptions);
    handleSearchDatatable();
})

var booksOptions = {
    "createdRow": generateChart
}

function applyFilterData(data) {
    data.ID = document.getElementById("bookId").value;
}

var applyFilter = () => {
    ReloadAjax();
}