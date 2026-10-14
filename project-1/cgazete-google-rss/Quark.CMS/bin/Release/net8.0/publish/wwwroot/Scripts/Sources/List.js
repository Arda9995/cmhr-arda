$(document).ready(function () {
    var sourcesColumns = [
        {
            "data": "photoUrl", "name": "photoUrl", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [ '<img src=' + row.photoUrl + ' class="w-75px shadow-sm rounded border" />'
            ]); }
        },
        {
            "data": "name", "key": "name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.name, row.id]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Sources/GetAllPaged", `${localizer.newsSources}`, sourcesColumns, applyFilterData, 20, sourcesOptions)
    handleSearchDatatable();
})

var sourcesOptions = {
    "createdRow": generateChart
}

function applyFilterData(data) {

}

var applyFilter = () => {
    ReloadAjax();
}