$(document).ready(function () {
    var usersColumns = [       
        {
            "data": "firstname", "name": "Firstname", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.firstname, row.lastname]); }
        },
        {
            "data": "email", "name": "Email", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.email]); }
        },
        {
            "data": "mobilePhone", "name": "MobilePhone", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.mobilePhone ?? ""]); }
        },
        {
            "data": "roleName", "name": "RoleName", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.roleName]); }
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
    CreateTable("#datatable", "/Users/GetAllPaged", `${localizer.users}`, usersColumns)
    handleSearchDatatable();
})