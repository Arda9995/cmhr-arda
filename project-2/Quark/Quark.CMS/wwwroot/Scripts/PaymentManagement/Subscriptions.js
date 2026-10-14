$(document).ready(function () {
    var columns = [
        {
            "data": "memberName", "name": "MemberName", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.id, row.memberName, row.memberID]); }
        },
        {
            "data": "subscriptionReferenceCode", "name": "SubscriptionReferenceCode", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.subscriptionReferenceCode]); }
        },
        {
            "data": "price", "name": "Price", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id, row.price]); }
        },
        {
            "data": "subscriptionStartDate", "name": "SubscriptionStartDate", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.id, row.subscriptionStartDate]); }
        },
        {
            "data": "subscriptionEndDate", "name": "SubscriptionEndDate", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.id, row.subscriptionEndDate]); }
        },
        {
            "data": "subscriptionPlanReferenceCode", "name": "SubscriptionPlanReferenceCode", "class": "w-70px text-center","orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.id, row.subscriptionPlanReferenceCode]); }
        },
        {
            "data": "subscriptionStatus", "name": "SubscriptionStatus", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col7", [row.subscriptionStatuClassName, row.subscriptionStatus]); }
        },
        {
            "data": "memberID", "name": "MemberID", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col8", [row.memberID]); }
        }
    ];
    CreateTable("#subscriptionsDatatable", "/PaymentManagement/GetAllSubscriptionsPaged", "Subscriptions", columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {

}

var applyFilter = () => {
    ReloadAjax();
}


