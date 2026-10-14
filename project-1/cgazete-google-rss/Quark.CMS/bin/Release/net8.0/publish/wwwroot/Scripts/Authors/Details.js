$(document).ready(function () {
    var newsColumns = [
        {
            "data": "photoUrl", "name": "Photo", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col2", [ '<img src=' + row.photoUrl + ' class="w-75px shadow-sm rounded border" />'
            ]); }
        },
        {
            "data": "title", "name": "Title", "autoWidth" : true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.id, row.title]); }
        },
        {
            "data": "category", "name": "Category", "class": "w-100px",
            "render": function (data, type, row) { return GetTableHtml("Col4", [row.category]); }
        },
        {
            "data": "type", "name": "Type", "class": "w-100px",
            "render": function (data, type, row) { return GetTableHtml("Col5", [row.type]); }
        },
        {
            "data": "publishedDate", "name": "PublishedDate", "class": "w-150px",
            "render": function (data, type, row) { return GetTableHtml("Col6", [row.publishedDate]); }
        },
        {
            "data": "id", "name": "IsImpressionIncreasing", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col7", [row.id]); }
        },
        {
            "data": "status", "name": "Status", "class": "w-100px",
            "render": function (data, type, row) { return GetTableHtml("Col8", [ManageBadgeColor(row)]); }
        },
        {
            "data": "isLockedAuthorized", "name": "IsLockedAuthorized", "class": "w-50px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [manageLockOrUpdate(row)]); }
        }
    ];
    var datatable = CreateTable("#datatable", "/Authors/GetAllNewsPaged/", `${localizer.news}`, newsColumns, applyFilterData, 20, newsOptions);
    handleSearchDatatable();
})

var newsOptions = {
    "createdRow": generateChart
}

function applyFilterData(data) {
    data.ID = document.getElementById("authorId").value;
}

var applyFilter = () => {
    ReloadAjax();
}

function manageLockOrUpdate(row) {
    if (row.isLockedAuthorized) {
        return '<i class="ki-duotone ki-lock-2 fs-2">' +
            '<span class="path1"></span>' +
            '<span class="path2"></span>' +
            '<span class="path3"></span>' +
            '<span class="path4"></span>' +
            '<span class="path5"></span>' +
            '</i>';
    } else {
        return '<a href="/News/Update/' + row.id + '" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1" data-bs-toggle="tooltip" data-bs-placement="top" title="'+ localizer.update +'">' +
            '<i class="ki-duotone ki-pencil fs-2">' +
            '<i class="path1"></i>' +
            '<i class="path2"></i>' +
            '</i>' +
            '</a>';
    }
}

function generateChart(row, data, index) {
    //#region Geçici çözüm
    if (data.status != "Yay&#305;nda") {
        return;
    }
    //#endregion
    var increasingColor = "#00d97e";
    var decreasingColor = "#ff3d71";
    var element = row.querySelector("#impression_chart_" + data.id);
    var chartOptions = {
        series: [{
            name: "Impressions",
            data: data.impressions
        }],
        chart: {
            height: "100px",
            width: "150px",
            type: 'line',
            toolbar: {
                show: false // this will hide the toolbar
            },
            zoom: {
                enabled: false // this will disable zooming
            }
        },
        legend: {
            show: false
        },
        dataLabels: {
            enabled: false
        },
        grid: {
            show: false
        },
        stroke: {
            curve: 'smooth',
            show: true,
            width: 3,
            colors: [data.isImpressionIncreasing ? increasingColor : decreasingColor]
        },
        xaxis: {
            categories: data.impressionDates,
            labels: {
                show: false // this will hide the x-axis labels
            },
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            }
        },
        yaxis: {
            labels: {
                show: false // this will hide the y-axis labels
            }
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            enabled: false // this will disable tooltips
        },
        noData: {
            text: ` `,
            align: "center",
            verticalAlign: "middle",
        },
    }
    let chart = new ApexCharts(element, chartOptions);
    chart.render();
}

function ManageBadgeColor(row){

    if (row.statusId === NewsStatusEnum.Draft) {
        return '<span class="badge badge-primary">'+row.status+'</span>';
    } else if (row.statusId === NewsStatusEnum.Published) {
        return '<span class="badge badge-success">'+row.status+'</span>';
    } else if (row.statusId === NewsStatusEnum.Unpublished) {
        return '<span class="badge badge-danger">'+row.status+'</span>';
    } else if (row.statusId === NewsStatusEnum.Scheduled) {
        return '<span class="badge badge-info">'+row.status+'</span>';
    }

    return null;
}