$(document).ready(function () {
    var columns = [
        {
            "data": "userID", "name": "UserID", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.userName]); }
        },
        {
            "data": "logType", "name": "LogType", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [BuildStr(row)]); }
        },
        {
            "data": "createdDate", "name": "CreatedDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.createdDate]); }
        },
        {
            "data": "IsSuccess", "name": "isSuccess", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isSuccess ? "success" : "danger", row.isSuccessText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-50px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col6", [row.id]);
            }
        }
    ];
    CreateTable("#datatable", "/SystemLogs/GetAllPaged", `${localizer.loginLogs}`, columns, applyFilterData, 20)
    handleSearchDatatable();
})

function BuildStr(row){
    if (row.objectID == null){
        return row.logType
    }else {
        return '<a target="_blank" href="' + row.url + '">'+ row.logType +'</a>'
    }
}

function applyFilterData(data) {
    var b = document.getElementById("type");
    data.SelectedTypeID = parseInt(b.options[b.selectedIndex].value);
}

var applyFilter = () => {
    ReloadAjax();
}