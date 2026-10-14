$(document).ready(function () {
    var userContractsColumns = [
        {
            "data": "title", "name": "Title", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id, row.title]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/UserContracts/GetAllPaged", `${localizer.userContracts}`, userContractsColumns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    
}

var applyFilter = () => {
    ReloadAjax();
}