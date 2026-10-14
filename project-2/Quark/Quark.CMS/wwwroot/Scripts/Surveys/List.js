$(document).ready(function () {
    var surveysColumns = [
        {
            "data": "photoUrl", "name": "PhotoUrl", "orderable": false, "class": "align-top text-center",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.photoUrl, row.question]); }
        },
        {
            "data": "question", "name": "Question", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.question, row.id]); }
        },
        {
            "data": "totalAnswers", "name": "TotalAnswers", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.totalAnswers]); }
        },
        {
            "data": "createdDate", "name": "CreatedDate", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.createdDate]); }
        },
        {
            "data": "isEnabled", "name": "IsEnabled", "class": "",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isEnabled ? "success" : "danger", row.isEnabledText]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id]); }
        }
    ];
    CreateTable("#datatable", "/Surveys/GetAllPaged", `${localizer.surveys}`, surveysColumns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    
    
}

var applyFilter = () => {
    ReloadAjax();
}