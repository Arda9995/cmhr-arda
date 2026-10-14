$(document).ready(function () {
    var newsColumns = [
        {
            "data": "photoUrl", "name": "Photo", "orderable": false, "class": "",
            "render": function (data, type, row) {
                return GetTableHtml("Col2", [row.photoUrl, row.title]);
            }
        },
        {
            "data": "title", "name": "Title", "autoWidth": true, "class": "",
            "render": function (data, type, row) {
                return GetTableHtml("Col3", [manageLockBadge(row)]);
            }
        },
        {
            "data": "publishedDate", "name": "PublishedDate", "class": "w-150px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col6", [row.publishedDate]);
            }
        },
        {
            "data": "views", "name": "Views", "class": "w-150px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col7", [row.id, row.views]);
            }
        },
        {
            "data": "status", "name": "Status", "class": "w-75px text-center",
            "render": function (data, type, row) {
                return GetTableHtml("Col8", [ManageBadgeColor(row)]);
            }
        },
        {
            "data": "isLockedByCurrentUser",
            "name": "IsLockedByCurrentUser",
            "class": "w-50px text-center",
            "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col12", [manageLockOrUpdate(row)]);
            }
        }
    ];
    var datatable = CreateTable("#datatable", "/News/GetAllPaged", `${localizer.news}`, newsColumns, applyFilterData, 20, newsOptions);
})

var newsOptions = {
    "createdRow": generateChart
}

function applyFilterData(data) {
    var b = document.getElementById("author");
    data.SelectedAuthorID = parseInt(b.options[b.selectedIndex].value);

    var f = document.getElementById("status");
    data.SelectedStatusID = parseInt(f.options[f.selectedIndex].value);

    var g = document.getElementById("category");
    data.SelectedCategoryID = parseInt(g.options[g.selectedIndex].value);

    var h = document.getElementById("type");
    data.SelectedTypeID = parseInt(h.options[h.selectedIndex].value);

    var s = document.getElementById("source");
    data.SelectedSourceID = parseInt(s.options[s.selectedIndex].value);

    var u = document.getElementById("user");
    data.SelectedUserID = parseInt(u.options[u.selectedIndex].value);

    data.CreatedDate = document.getElementById("createdDate").value;

    data.PublishDate = document.getElementById("publishDate").value;

    data.SearchValue = document.getElementById("searchValue").value;

    var j = document.getElementById("range");
    data.SelectedViewRange = parseInt(j.options[j.selectedIndex].value);

}

var applyFilter = () => {
    ReloadAjax();
}

function BuildLockIcon(id) {
    return '<div class="menu-item px-3"><a href="/News/Details/' + id + '" class="menu-link px-3 noclass">' + localizer.showView + '</a></div>';
}

function BuildMenuButton() {
    return '<button class="btn btn-sm btn-icon btn-bg-light btn-active-color-primary" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end"><i class="ki-solid ki-dots-horizontal fs-2x me-1"></i></button>';
}

function BuildUpdateMenuItem(id) {
    return '<div class="menu-item px-3"><a href="/News/Update/' + id + '" class="menu-link px-3 noclass">' + localizer.update + '</a></div>';
}

function BuildDetailsMenuItem(id) {
    return '<div class="menu-item px-3"><a href="/News/Details/' + id + '" class="menu-link px-3 noclass">' + localizer.viewNews + '</a></div>';
}

function BuildIntegrationsNewsMenuItem(id) {
    return '<div class="menu-item px-3"><a href="/NewsIntegration/Create?newsId=' + id + '" class="menu-link px-3 noclass">' + "Haber Merkezine Aktar" + '</a></div>';
}


//function BuildPublishMenuItem(id) {
//    return '<div class="menu-item px-3"><a onclick="publish(' + id + ')" class="menu-link px-3 noclass">'+ localizer.publish +'</a></div>';
//}

//function BuildUnpublishMenuItem(id) {
//    return '<div class="menu-item px-3"><a onclick="unpublish(' + id + ')" class="menu-link px-3 noclass">'+ localizer.unPublish +'</a></div>';
//}

//function BuildScheduleMenuItem(id) {
//    return '<div class="menu-item px-3"><a onclick="schedule(' + id + ')" class="menu-link px-3 noclass">'+ localizer.schedule +'</a></div>';
//}

//function BuildUnScheduleMenuItem(id) {
//    return '<div class="menu-item px-3"><a onclick="unschedule(' + id + ')" class="menu-link px-3 noclass">'+ localizer.toDraft +'</a></div>';
//}

//function BuildReScheduleMenuItem(id) {
//    return '<div class="menu-item px-3"><a onclick="schedule(' + id + ')" class="menu-link px-3 noclass">'+ localizer.reSchedule +'</a></div>';
//}

//function publish(id) {
//    Confirm(`${localizer.confirmPublishMsg}`, '/News/PublishNow', id, ReloadAjax, ReloadAjax);
//}

//function schedule(id) {
//    SelectDate(`${localizer.selectDateMsg}`, '/News/PublishScheduled/', id, ReloadAjax, ReloadAjax);
//}

//function unpublish(id) {
//    Confirm(`${localizer.confirmUnPublishMsg}`, '/News/UnPublish', id, ReloadAjax, ReloadAjax);
//}

//function unschedule(id) {
//    Confirm(`${localizer.confirmUnScheduleMsg}`, '/News/UnSchedule', id, ReloadAjax, ReloadAjax);
//}

function manageLockOrUpdate(row) {
    let html = BuildMenuButton();
    html += '<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-7 w-200px py-4" data-kt-menu="true">';
    html += BuildDetailsMenuItem(row.id);
    html += BuildIntegrationsNewsMenuItem(row.id);
    if (row.isLockedByCurrentUser) {
        if (row.hasCreateUpdateRight) {
            html += BuildUpdateMenuItem(row.id);
            html += '</div>';
            return html;
        }
    } else if (row.lockedBy != 0 && !row.isLockedByCurrentUser) {

        //if (row.statusId === NewsStatusEnum.Draft && row.hasPublishRight) {
        //    html += BuildPublishMenuItem(row.id);
        //    html += BuildScheduleMenuItem(row.id);
        //} else if (row.statusId === NewsStatusEnum.Published && row.hasUnPublishRight) {
        //    html += BuildUnpublishMenuItem(row.id)
        //} else if (row.statusId === NewsStatusEnum.Unpublished && row.hasPublishRight) {
        //    html += BuildPublishMenuItem(row.id)
        //} else if (row.statusId === NewsStatusEnum.Scheduled && row.hasPublishRight) {
        //    html += BuildPublishMenuItem(row.id)
        //    html += BuildReScheduleMenuItem(row.id)
        //    html += BuildUnScheduleMenuItem(row.id)
        //}
        html += '</div>';
        return html;
    } else {
        if (row.hasCreateUpdateRight) {
            html += BuildUpdateMenuItem(row.id);
            html += '</div>';
            return html;
        }
    }
}


function manageLockBadge(row) {
    if (row.isLockedByCurrentUser) {
        return '<div class="badge badge-info mb-2 p-2">' +
            '<i class="ki-duotone ki-lock-2 text-light me-2">' +
            '<span class="path1"></span>' +
            '<span class="path2"></span>' +
            '<span class="path3"></span>' +
            '<span class="path4"></span>' +
            '<span class="path5"></span>' +
            '</i>' +
            localizer.newsLockedByCurrentUser +
            '</div>' +
            '<div class="d-flex">' +
            '<a href="/News/Details/' + row.id + '" target="_blank">' + row.title + '</a>' +
            '</div>' +
            '<div class="d-flex mt-2">' +
            '<div class="badge badge-light-warning fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="ID">' + 'ID : ' + row.id + '</div>' +
            '<div class="badge badge-light-primary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer["Category"]">' + row.category + '</div>' +
            '<div class="badge badge-light-success fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer[" NewsType"]">' + row.type + '</div>' +
            '<div class="badge badge-light-info fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.relatedNews + row.relatedNewsCount + '</div>' +
            '<div class="badge badge-light-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.tags + ': ' + row.tagsCount + '</div>' +
            '<div class="badge badge-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top"> Editör : ' + row.createdByName + '</div>' +
            '</div>';
    } else if (row.lockedBy != 0 && !row.isLockedByCurrentUser) {
        return '<div class="badge badge-danger mb-2 p-2">' +
            '<i class="ki-duotone ki-lock-2 text-light me-2">' +
            '<span class="path1"></span>' +
            '<span class="path2"></span>' +
            '<span class="path3"></span>' +
            '<span class="path4"></span>' +
            '<span class="path5"></span>' +
            '</i>' +
            localizer.newsLocked +
            '</div>' +
            '<div class="d-flex"> ' +
            '<a href="/News/Details/' + row.id + '" target="_blank">' + row.title + '</a>' +
            '</div>' +
            '<div class="d-flex mt-2">' +
            '<div class="badge badge-light-warning fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="ID">' + 'ID : ' + row.id + '</div>' +
            '<div class="badge badge-light-primary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer["Category"]">' + row.category + '</div>' +
            '<div class="badge badge-light-success fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer[" NewsType"]">' + row.type + '</div>' +
            '<div class="badge badge-light-info fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.relatedNews + row.relatedNewsCount + '</div>' +
            '<div class="badge badge-light-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.tags + ': ' + row.tagsCount + '</div>' +
            '<div class="badge badge-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top"> Editör : ' + row.createdByName + '</div>' +
            '</div>';
    } else {
        return '<div class="d-flex"> ' +
            '<a href="/News/Details/' + row.id + '" target="_blank">' + row.title + '</a>' +
            '</div>' +
            '<div class="d-flex mt-2">' +
            '<div class="badge badge-light-warning fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="ID">' + 'ID : ' + row.id + '</div>' +
            '<div class="badge badge-light-primary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer["Category"]">' + row.category + '</div>' +
            '<div class="badge badge-light-success fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer[" NewsType"]">' + row.type + '</div>' +
            '<div class="badge badge-light-info fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.relatedNews + row.relatedNewsCount + '</div>' +
            '<div class="badge badge-light-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.tags + ': ' + row.tagsCount + '</div>' +
            '<div class="badge badge-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top"> Editör : ' + row.createdByName + '</div>' +
            '</div>';
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
            name: localizer.impressions,
            data: data.impressions
        }],
        chart: {
            height: "60px",
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
            categories: formatImpressionDates(data.impressionDates),
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
            enabled: true // this will disable tooltips
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

function formatImpressionDates(dates) {
    return dates.map(dateString => {
        const date = new Date(dateString);
        const options = {
            day: '2-digit',
            month: 'long',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        return date.toLocaleDateString('tr-TR', options);
    });
}

function ManageBadgeColor(row) {

    if (row.statusId === NewsStatusEnum.Draft) {
        var badge = '<span class="badge badge-primary">' + row.status + '</span>';
        if (row.isPublished) {
            badge = '<span class="badge badge-warning"> Yayinda </span>';
        }
        return badge;
    } else if (row.statusId === NewsStatusEnum.Published) {
        return '<span class="badge badge-success">' + row.status + '</span>';
    } else if (row.statusId === NewsStatusEnum.Unpublished) {
        return '<span class="badge badge-danger">' + row.status + '</span>';
    } else if (row.statusId === NewsStatusEnum.Scheduled) {
        return '<span class="badge badge-info">' + row.status + '</span>';
    }

    return null;
}