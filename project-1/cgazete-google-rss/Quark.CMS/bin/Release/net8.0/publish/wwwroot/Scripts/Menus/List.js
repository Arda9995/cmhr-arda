$(document).ready(function () {
    var menusColumns = [
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.name]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center","orderable" : false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Menus/GetAllPaged", `${localizer.menus}`, menusColumns)
    handleSearchDatatable();
})