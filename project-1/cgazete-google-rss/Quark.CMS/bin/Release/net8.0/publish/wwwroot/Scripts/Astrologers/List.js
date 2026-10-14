var dataTable;
var colName = '';
var orderDirection = '';

$(document).ready(function () {
    var astrologersColumns = [
        {
            "data": "id", "name": "ID", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col1", [row.id]);
            }
        },
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col2", [row.name, row.id]);
            }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col12", [row.id, row.id]);
            }
        }

    ];
    dataTable = CreateTable("#datatable", "/Astrologers/GetAllPaged", `${localizer.astrologers}`, astrologersColumns)
    handleSearchDatatable();

    dataTable.on('order', function (e) {
        if (dataTable.order().length > 0) {
            var colIndex = dataTable.order()[0][0]

            colName = pagesColumns[colIndex].name;
            orderDirection = dataTable.order()[0][1]
        }
    });
})
function applyFilterData(data) {
    data.searchValue = document.querySelector('#search-value').value;
}