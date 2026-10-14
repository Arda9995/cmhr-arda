$(document).ready(function () {
    var columns = [
        {
            "data": "name", "name": "Name", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id, row.name]); }
        },
        {
            "data": "description", "name": "Description", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.description]); }
        },
        {
            "data": "planReferenceCode", "name": "PlanReferenceCode", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id, row.productReferenceCode]); }
        },
        {
            "data": "createdDate", "name": "CreatedDate", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id, row.createdDate]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        }
    ];
    CreateTable("#paymentProductDatatable", "/PaymentManagement/GetAllProductsPaged", "Products", columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {

}

var applyFilter = () => {
    ReloadAjax();
}


