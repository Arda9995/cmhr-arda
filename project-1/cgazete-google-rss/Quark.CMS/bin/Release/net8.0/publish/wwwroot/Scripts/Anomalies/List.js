$(document).ready(function () {
    var anomaliesColumns = [
        {
            "data": "dataURL", "name": "DataURL",
            "render": function (data, type, row) {
                return GetTableHtml("Col1", [row.dataURL, row.dataURL]);
            }
        },
        {
            "data": "dataType", "name": "DataType", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col2", [row.dataType]);
            }
        },
        {
            "data": "issue", "name": "Issue", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col3", [row.issue]);
            }
        },
        {
            "data": "createdDate", "name": "CreatedDate", "class": "w-200px",
            "render": function (data, type, row) {
                return GetTableHtml("Col4", [row.createdDate]);
            }
        },
        {
            "data": "isSolved", "name": "IsSolved", "class": "w-100px",
            "render": function (data, type, row) {
                return GetTableHtml("Col5", [manageBadge(row)]);
            }
        },
        {
            "data": "action", "name": "Action", "class": "w-50px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col6", [row.id]);
            }
        }
    ];
    var datatable = CreateTable("#datatable", "/Anomalies/GetAllPaged", `${localizer.anomalies}`, anomaliesColumns, applyFilterData, 20);
    handleSearchDatatable();
})

function applyFilterData(data) {
    var b = document.getElementById("dataType");
    data.SelectedDataTypeID = parseInt(b.options[b.selectedIndex].value);

    var f = document.getElementById("issueType");
    data.SelectedIssueTypeID = parseInt(f.options[f.selectedIndex].value);

    var f = document.getElementById("user");
    data.SelectedUserID = parseInt(f.options[f.selectedIndex].value);

    var OnlyShowUnResolved = document.getElementById("OnlyShowUnResolved");
    data.OnlyShowUnResolved = OnlyShowUnResolved.checked;
}

var applyFilter = () => {
    ReloadAjax();
}

function manageBadge(row){
    if(row.isSolved){
        return `<span class="badge badge-success">${localizer.solved}</span>`;
    }
    else {
        return `<span class="badge badge-danger">${localizer.unSolved}</span>`;
    }
}