var dataTable;
var colName = '';
var orderDirection = '';

$(document).ready(function () {
    var newsPhotoTypesColumns = [
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
            "data": "key", "name": "Key", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col3", [row.key]);
            }
        },
        {
            "data": "width", "name": "Width", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col4", [row.width, row.height]);
            }
        },
        {
            "data": "isRequired", "name": "IsRequired", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col5", [row.isRequired ? "success" : "danger", row.isRequiredText]);
            }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col6", [row.isEnabled ? "success" : "danger", row.isEnabledText]);
            }
        },
        {
            "data": "order", "name": "Order", "autoWidth": true,
            "render": function (data, type, row) {
                return GetTableHtml("Col7", [row.order]);
            }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col12", [row.id, row.id]);
            }
        }
        
    ];
    dataTable = CreateTable("#datatable", "/NewsPhotoTypes/GetAllPaged", `${localizer.newsPhotosSettings}`, newsPhotoTypesColumns)
    handleSearchDatatable();

    dataTable.on('order', function (e) {
        if (dataTable.order().length > 0) {
            var colIndex = dataTable.order()[0][0]

            colName = newsPhotoTypesColumns[colIndex].name;
            orderDirection = dataTable.order()[0][1]
        }
    });
})
function applyFilterData(data) {
    data.searchValue = document.querySelector('#search-value').value;
}

function ApplyFiltersToExcel() {
    var filters = {}
    applyFilterData(filters);
    window.location.href = "/NewsPhotoTypes/DownloadExcelExport?sortColumn=" + colName + "&sortDirection=" + orderDirection + '&SearchValue=' + filters.searchValue;
}