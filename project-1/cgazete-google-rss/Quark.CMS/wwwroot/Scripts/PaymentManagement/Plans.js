$(document).ready(function () {
    var columns = [
        {
            "data": "name", "name": "Name", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id, row.name]); }
        },
        {
            "data": "activeSubscriptionCount", "name": "ActiveSubscriptionCount", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col99", [row.id, row.activeSubscriptionCount]); }
        },
        {
            "data": "price", "name": "Price", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.price]); }
        },
        {
            "data": "paymentInterval", "name": "PaymentInterval", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id, row.paymentInterval]); }
        },
        {
            "data": "planReferenceCode", "name": "PlanReferenceCode", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id, row.planReferenceCode]); }
        },
        {
            "data": "productReferenceCode", "name": "ProductReferenceCode", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.id, row.productReferenceCode]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.isEnabled ? "success" : "danger", row.isEnabledText, row.isPopular ? "success" : "", row.isPopularText]); }
        },
        {
            "data": "id", "name": "ID", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col7", [row.id]); }
        }
    ];
    CreateTable("#paymentPlanDatatable", "/PaymentManagement/GetAllPlansPaged", "Plans", columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
   
}

var applyFilter = () => {
   ReloadAjax();
}


