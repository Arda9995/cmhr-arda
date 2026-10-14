$(document).ready(function () {
    var pagesColumns = [
        {
            "data": "id", "key": "ID", "autoWidth": true, "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id]); }
        },
        {
            "data": "name", "key": "name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.name, row.id]); }
        },
        {
            "data": "description", "name": "description", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.description]); }
        },
        {
            "data": "key", "name": "key", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.key]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Pages/GetAllPaged", `${localizer.pages}`, pagesColumns)
    handleSearchDatatable();
})