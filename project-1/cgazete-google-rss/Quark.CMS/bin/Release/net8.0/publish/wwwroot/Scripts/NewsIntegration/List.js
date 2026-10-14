$(document).ready(function () {
    var newsIntegrationColums = [
        {
            "data": "newsID", "name": "NewsID", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id, row.newsID]); }
        },
        {
            "data": "title", "name": "Title", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id,row.title]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id]); }
        }
    ];
    CreateTable("#newsIntegrationDatatable", "/NewsIntegration/GetAllPaged", `${localizer.users}`, newsIntegrationColums)
    handleSearchDatatable();
})