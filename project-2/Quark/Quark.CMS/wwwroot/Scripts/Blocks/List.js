$(document).ready(function () {
    var blocksColumns = [
        {
            "data": "Name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.name]); }
        },
        {
            "data": "pageName", "name": "PageName", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.pageName]); }
        },
        {
            "data": "BlockType", "name": "BlockType", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.blockType]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Blocks/GetAllPaged", `${localizer.blocks}`, blocksColumns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    var b = document.getElementById("page");
    data.SelectedPageID = parseInt(b.options[b.selectedIndex].value);
}

var applyFilter = () => {
    ReloadAjax();
}