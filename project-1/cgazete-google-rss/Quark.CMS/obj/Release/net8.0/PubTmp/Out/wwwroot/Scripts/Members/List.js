$(document).ready(function () {
    var columns = [
        {
            "data": "id", "name": "id", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id]); }
        },
        {
            "data": "firstname", "name": "Firstname", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.firstname, row.lastname]); }
        },
        {
            "data": "email", "name": "Email", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.email ?? ""]); }
        },
        {
            "data": "mobilePhone", "name": "MobilePhone", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.mobilePhone ?? ""]); }
        },
        {
            "data": "createdDate", "name": "CreatedDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.createdDate]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Members/GetAllPaged", `${localizer.members}`, columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    var startDate = document.getElementById("StartDate");
    data.startDate = startDate.value;

    var endDate = document.getElementById("EndDate");
    data.endDate = endDate.value;

    var isEnabled = document.getElementById("IsEnabled");
    data.IsEnabled = isEnabled.checked;

    var isDeleted = document.getElementById("IsDeleted");
    data.IsDeleted = isDeleted.checked;

}

var applyFilter = () => {
    ReloadAjax();
}

$("#StartDate").daterangepicker({
    singleDatePicker: true,
    showDropdowns: true,
    autoApply: true,
    locale: {
        format: 'DD.MM.yyyy'
    }
});
$("#EndDate").daterangepicker({
    singleDatePicker: true,
    showDropdowns: true,
    autoApply: true,
    locale: {
        format: 'DD.MM.yyyy'
    }
});