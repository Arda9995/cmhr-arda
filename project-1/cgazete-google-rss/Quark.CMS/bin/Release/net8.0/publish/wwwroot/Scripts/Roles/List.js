$(document).ready(function () {
    var columns = [
        {
            "data": "id", "name": "id", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id]); }
        },
        {
            "data": "Name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.name]); }
        },
        {
            "data": "Description", "name": "Description", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.description]); }
        },
        {
            "data": "ActiveUserCount", "name": "ActiveUserCount", "autoWidth": true, "class": "text-center",
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.activeUserCount]); }
        },
        {
            "data": "PassiveUserCount", "name": "PassiveUserCount", "autoWidth": true, "class": "text-center",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.passiveUserCount]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Roles/GetAllPaged", `${localizer.roles}`, columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    var isEnabled = document.getElementById("IsEnabled");
    data.IsEnabled = isEnabled.checked;
}

var applyFilter = () => {
    ReloadAjax();
}