$(document).ready(function () {
    var columns = [       
        {
            "data": "Date", "name": "Date", "class": "w-70px",
            "render": function (data, type, row) { return GetTableHtml("Col1", [row.date]); }
        },
        {
            "data": "Title", "name": "Title", "class": "w-170px", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.title, row.id]); }
        },
        {
            "data": "AgencyName", "name": "AgencyName", "class": "w-70px", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.agencyName]); }
        },
        {
            "data": "CategoryName", "name": "CategoryName", "class": "w-70px", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.categoryName]); }
        },
        {
            "data": "IsActive", "name": "IsActive", "class": "w-70px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.isActive ? "success" : "danger", row.isActiveText]); }
        }
    ];
    CreateTable("#datatable", "/AgencyNews/GetAllPaged", "AgencyNews", columns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    var startDate = document.getElementById("StartDate");
    data.startDate = startDate.value;

    var endDate = document.getElementById("EndDate");
    data.endDate = endDate.value;

    var g = document.getElementById("category");
    data.SelectedCategoryID = parseInt(g.options[g.selectedIndex].value);

    var s = document.getElementById("agency");
    data.SelectedAgencyID = parseInt(s.options[s.selectedIndex].value);

    var isActive = document.getElementById("IsActive");
    data.IsActive = isActive.checked;
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


$(document).on("click", ".news-detail", function () {
    var id = $(this).data("id");

    $.ajax({
        url: "/AgencyNews/Detail/" + id,
        type: "GET",
        success: function (res) {
            debugger;
            if (!res.isSuccess) {
                alert(res.message || "Haber bulunamadı.");
                return;
            }

            var data = res.data;

            $("#newsTitle").text(data.title);
            $("#newsAgency").text(data.agencyName);
            $("#newsCategory").text(data.categoryName);
            $("#newsType").text(data.type);
            $("#newsDate").text(data.date);
            $("#newsSubtitle").text(data.subtitle);
            $("#newsContent").html(data.content);

            $("#btnHaberlestir").attr("href", "/News/Create/1?agencyNewsID=" + data.id);

            $("#newsDetailModal").modal("show");
        },
        error: function () {
            alert("Haber detayları yüklenirken hata oluştu.");
        }
    });
});
