$(document).ready(function () { 
    var customerNotificationsColumns = [
        {
            "data": "createdDate", "name": "CreatedDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col81", [row.createdDate]); }
        },
        {
            "data": "senderName", "name": "SenderName", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col82", [row.senderName]); }
        },
        {
            "data": "title", "name": "Title", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col83", [row.title]); }
        },
        {
            "data": "message", "name": "Message", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col84", [row.message]); }
        },
        {
            "data": "imageUrl", "name": "ImageUrl", "class": "w-100px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col85", [row.imageUrl]); }
        },
        {
            "data": "isRead", "name": "IsRead", "class": "w-70px text-center",
            "render": function (data, type, row) { return GetTableHtml("Col86", [row.isRead ? "success" : "danger", row.isReadText]); }
        }
    ];
    CreateTable("#customersNotificationsDatatable", "/Members/GetNotificationsPaged/" + memberId, `${localizer.notifications}`, customerNotificationsColumns);

    var customerCreditCardsColumns = [
        {
            "data": "createdDate", "name": "CreatedDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col91", [row.createdDate]); }
        },
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col92", [row.name]); }
        },
        {
            "data": "cardHolderName", "CardHolderName": "Title", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col93", [row.cardHolderName]); }
        },
        {
            "data": "cardNo", "name": "CardNo", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col94", [row.cardNo]); }
        },
        {
            "data": "expiration", "name": "Expiration", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col95", [row.expirationMonth + "/" + row.expirationYear]); }
        }
    ];
    CreateTable("#customerCreditCardsDatatable", "/Members/GetCreditCardsPaged/" + memberId, "CreditCards", customerCreditCardsColumns);
})