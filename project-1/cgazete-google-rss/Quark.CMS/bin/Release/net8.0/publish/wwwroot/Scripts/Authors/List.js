var dataTable;
var colName = '';
var orderDirection = '';

$(document).ready(function () {
    var authorsColumns = [
        {
            "data": "photo", "name": "Photo", "autoWidth": true, "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [BuildAuthorAvatarHTML(row.photo, row.nameInitials,row.name,row.id)]); }
        },
        {
            "data": "firstname", "name": "Firstname", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id, row.name]); }
        },
        {
            "data": "email", "name": "Email", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.email]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    dataTable = CreateTable("#datatable", "/Authors/GetAllPaged", `${localizer.authors}`, authorsColumns)
    handleSearchDatatable();

    dataTable.on('order', function (e) {
        if (dataTable.order().length > 0) {
            var colIndex = dataTable.order()[0][0]

            colName = authorsColumns[colIndex].name;
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
    window.location.href = "/Authors/DownloadExcelExport?sortColumn=" + colName + "&sortDirection=" + orderDirection + '&SearchValue=' + filters.searchValue;
}

function BuildAuthorAvatarHTML(photo, nameInitials, name, id) {

    if (photo == null || photo == '') {
        return `<div class="symbol symbol-35px symbol-md-50px"><a href="/Authors/Details/${id}"> <div class="symbol symbol-50px symbol-circle"><span class="symbol-label bg-light-success text-success fs-5 fw-bolder">${nameInitials}</span></a></div></div>`;
    }
    else {
        return `<div class="symbol symbol-circle symbol-50px overflow-hidden me-3 avatar-parent"><a href="/Authors/Details/${id}"> <div class="symbol-label"> <img src="${photo}" alt="${name}" class="w-100"></div></a></div>`;
    }
}