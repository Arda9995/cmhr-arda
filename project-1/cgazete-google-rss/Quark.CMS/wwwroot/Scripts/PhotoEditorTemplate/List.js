$(document).ready(function () {
    var templateColumns = [
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.name]); }
        },
        {
            "data": "createdUsername", "name": "CreatedUsername", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.createdUsername]); }
        },
        {
            "data": "createdDatetime", "name": "CreatedDatetime", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.createdDatetime]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/PhotoEditTemplates/GetAllPaged", `${localizer.template}`, templateColumns)
    handleSearchDatatable();
})