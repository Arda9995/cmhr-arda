var googleTrendsBlockUI = null;

$(document).ready(function () {

    var newsColumns = [
        {
            "data": "photoUrl", "name": "Photo", "orderable": false, "class": "align-top",
            "render": function (data, type, row) {
                return GetTableHtml("Col2", [row.photoUrl, row.title]);
            }
        },
        {
            "data": "title", "name": "Title", "autoWidth": true, "class": "align-top", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col3", [manageLockBadge(row)]);
            }
        },
        {
            "data": "publishedDate", "name": "PublishedDate", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col6", [row.publishedDate]);
            }
        },
        {
            "data": "viewCount", "name": "ViewCount", "class": "w-150px",
            "render": function (data, type, row) {
                return GetTableHtml("Col9", [row.viewCount]);
            }
        },
        {
            "data": "status", "name": "Status", "class": "w-75px",
            "render": function (data, type, row) {
                return GetTableHtml("Col10", [ManageBadgeColor(row)]);
            }
        },
        {
            "data": "isLockedByCurrentUser", "name": "IsLockedByCurrentUser", "class": "w-50px text-center", "orderable": false,
            "render": function (data, type, row) {
                return GetTableHtml("Col12", [manageLockOrUpdate(row)]);
            }
        }
    ];
    var datatable = CreateTable("#datatable", "/Home/GetAllNewsPaged", `${localizer.news}`, newsColumns, applyFilterData, 20, newsOptions);
    handleSearchDatatable();
})

var newsOptions = {
    "createdRow": generateChart
}

function applyFilterData(data) {

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

function BuildPublishMenuItem(id) {
    return '<div class="menu-item px-3"><a onclick="publish(' + id + ')" class="menu-link px-3 noclass">' + localizer.publish + '</a></div>';
}

function BuildUnpublishMenuItem(id) {
    return '<div class="menu-item px-3"><a onclick="unpublish(' + id + ')" class="menu-link px-3 noclass">' + localizer.unPublish + '</a></div>';
}

function BuildScheduleMenuItem(id) {
    return '<div class="menu-item px-3"><a onclick="schedule(' + id + ')" class="menu-link px-3 noclass">' + localizer.schedule + '</a></div>';
}

function BuildUnScheduleMenuItem(id) {
    return '<div class="menu-item px-3"><a onclick="unschedule(' + id + ')" class="menu-link px-3 noclass">' + localizer.toDraft + '</a></div>';
}

function BuildReScheduleMenuItem(id) {
    return '<div class="menu-item px-3"><a onclick="schedule(' + id + ')" class="menu-link px-3 noclass">' + localizer.reSchedule + '</a></div>';
}

function publish(id) {
    Confirm(`${localizer.confirmPublishMsg}`, '/News/PublishNow', id, ReloadAjax, ReloadAjax);
}

function schedule(id) {
    SelectDate(`${localizer.selectDateMsg}`, '/News/PublishScheduled/', id, ReloadAjax, ReloadAjax);
}

function unpublish(id) {
    Confirm(`${localizer.confirmUnPublishMsg}`, '/News/UnPublish', id, ReloadAjax, ReloadAjax);
}

function unschedule(id) {
    Confirm(`${localizer.confirmUnScheduleMsg}`, '/News/UnSchedule', id, ReloadAjax, ReloadAjax);
}

function manageLockOrUpdate(row) {
    if (row.isLockedByCurrentUser) {
        return;
    } else {
        let html = BuildMenuButton();
        html += '<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-7 w-200px py-4" data-kt-menu="true">';
        if (row.statusId === NewsStatusEnum.Draft) {
            html += BuildUpdateMenuItem(row.id);
            html += BuildPublishMenuItem(row.id);
            html += BuildScheduleMenuItem(row.id);
        } else if (row.statusId === NewsStatusEnum.Published) {
            html += BuildUpdateMenuItem(row.id)
            html += BuildUnpublishMenuItem(row.id)
        } else if (row.statusId === NewsStatusEnum.Unpublished) {
            html += BuildUpdateMenuItem(row.id)
            html += BuildPublishMenuItem(row.id)
        } else if (row.statusId === NewsStatusEnum.Scheduled) {
            html += BuildUpdateMenuItem(row.id)
            html += BuildPublishMenuItem(row.id)
            html += BuildReScheduleMenuItem(row.id)
            html += BuildUnScheduleMenuItem(row.id)
        }
        html += '</div>';
        return html;
    }
}

function manageLockBadge(row) {
    if (row.isLockedByCurrentUser) {
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
            '<div class="d-flex">' +
            '<a href="/News/Details/' + row.id + '" target="_blank">' + row.title + '</a>' +
            '</div>' +
            '<div class="d-flex mt-2">' +
            '<div class="badge badge-light-primary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer["Category"]">' + row.category + '</div>' +
            '<div class="badge badge-light-success fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer[" NewsType"]">' + row.type + '</div>' +
            '<div class="badge badge-light-info fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.relatedNews + row.relatedNewsCount + '</div>' +
            '<div class="badge badge-light-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.tags + ': ' + row.tagsCount + '</div>' +
            '</div>';
    } else {
        return '<div class="d-flex">' +
            '<a href="/News/Details/' + row.id + '" target="_blank">' + row.title + '</a>' +
            '</div>' +
            '<div class="d-flex mt-2">' +
            '<div class="badge badge-light-primary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer["Category"]">' + row.category + '</div>' +
            '<div class="badge badge-light-success fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top" title="@_localizer[" NewsType"]">' + row.type + '</div>' +
            '<div class="badge badge-light-info fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.relatedNews + row.relatedNewsCount + '</div>' +
            '<div class="badge badge-light-secondary fw-bold me-2" data-bs-toggle="tooltip" data-bs-custom-class="tooltip-inverse" data-bs-placement="top">' + localizer.tags + ': ' + row.tagsCount + '</div>' +
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
        }
    }
    let chart = new ApexCharts(element, chartOptions);
    chart.render();
}

function ManageBadgeColor(row) {

    if (row.statusId === NewsStatusEnum.Draft) {
        return '<span class="badge badge-primary">' + row.status + '</span>';
    } else if (row.statusId === NewsStatusEnum.Published) {
        return '<span class="badge badge-success">' + row.status + '</span>';
    } else if (row.statusId === NewsStatusEnum.Unpublished) {
        return '<span class="badge badge-danger">' + row.status + '</span>';
    } else if (row.statusId === NewsStatusEnum.Scheduled) {
        return '<span class="badge badge-info">' + row.status + '</span>';
    }

    return null;
}

function GetTrends() {
    googleTrendsBlockUI.block();
    $.ajax({
        url: '/Home/GetTrends/',  // The controller action URL
        type: 'GET',
        data: {
            category: $('#trendsCategory').val()
        },
        success: function (response) {
            // Insert the returned HTML into the #trendsContent div
            $('#trendsContent').html(response);
            $('[data-bs-toggle="tooltip"]').tooltip();
            googleTrendsBlockUI.release();
        },
        error: function () {
            alert('Failed to load trending news');
        }
    });
}
function GenericGetRequest(url) {

    return $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            return data;
        },
        error: function (xhr, status, error) {
            return;
        }
    });
}

$(document).ready(function () {

    GenericGetRequest('/Home/GetDailyNewsInformation/')
        .done(function (response) {
            $("#taggedNews").text(response.taggedNewsCount);
            $("#withoutTaggedNews").text(response.withoutTaggedNewsCount);
            $("#relatedNews").text(response.relatedNewsCount);
            $("#withoutRelatedNews").text(response.withoutRelatedNewsCount);
        })
        .fail(function () {
        });


    GenericGetRequest('/Home/GetTodayNewsDetails/')
        .done(function (response) {
            $("#todaysNewsImage").attr("src", response.newsImageURL);
            $("#todayNewsURL").attr("href", "/News/Details/" + response.newsID);
            $("#todayNewsTitle").text(response.newsTitle.truncate());
            $("#todayTagCount").text(response.tagCount);
            $("#todayNewsCategoryName").text(response.categoryName);
            $("#todayNewsSourceName").text(response.newsSourceName);
            $("#todayNewsTypeName").text(response.newsTypeName);
        })
        .fail(function () {
        });

    GenericGetRequest('/Home/GetMostReadedNewsDetails/')
        .done(function (response) {
            $("#mostReadedNewsImpression").text(response.impression);
            $("#mostReadedNewsImage").attr("src", response.newsImageURL);
            $("#mostReadedNewsURL").attr("href", "/News/Details/" + response.newsID);
            $("#mostReadedNewsTitle").text(response.newsTitle.truncate());
            $("#mostReadedTagCount").text(response.tagCount);
            $("#mostReadedNewsCategoryName").text(response.categoryName);
            $("#mostReadedNewsSourceName").text(response.newsSourceName);
            $("#mostReadedNewsTypeName").text(response.newsTypeName);
        })
        .fail(function () {
        });


    GenericGetRequest('/Home/GetUnpublishedNewsCount/')
        .done(function (response) {
            $("#unpublishedNewsCount").text(response);
        })
        .fail(function () {
        });


    // Sabit renkler dizisi
    const colors = ['primary', 'danger', 'success', 'info', 'warning', 'dark', 'secondary'];

    GenericGetRequest('/Home/GetNewsTypeDetails/')
        .done(function (response) {
            const totalNewsCount = response.totalNewsCount;
            $(".card-title .text-muted").text(`${totalNewsCount} ${localizer.newsCount}`);

            const newsTypeItems = response.newsTypeItems;
            const cardBody = $(".newsTypesAndCounts");

            let colorIndex = 0;

            newsTypeItems.forEach(function (item) {
                const color = colors[colorIndex % colors.length];

                const newsTypeItemHTML = `
            <div class="d-flex flex-stack">
                <div class="symbol symbol-40px me-4">
                    <div class="symbol-label fs-2 fw-semibold bg-${color} text-inverse-${color}">${item.newsTypeName.charAt(0)}</div>
                </div>
                <div class="d-flex align-items-center flex-row-fluid flex-wrap">
                    <div class="flex-grow-1 me-2">
                        <a href="#" class="text-gray-800 text-hover-primary fs-6 fw-bold">${item.newsTypeName}</a>
                        <span class="text-muted fw-semibold d-block fs-7">${item.newsCount} Haber</span>
                    </div>
                    <a href="/News/Create/${item.newsTypeID}" class="btn btn-sm btn-icon btn-bg-light btn-active-color-primary w-30px h-30px">
                        <i class="ki-duotone ki-arrow-right fs-2"><span class="path1"></span><span class="path2"></span></i>
                    </a>
                </div>
            </div>
            <div class="separator separator-dashed my-4"></div>
            `;
                cardBody.append(newsTypeItemHTML);
                colorIndex++;
            });
        })
        .fail(function () {
        });


    GenericGetRequest('/Home/GetLastPublishedNews/')
        .done(function (response) {
            const cardBody = $("#lastPublishedNewsContainer");
            const lastPublishedNewsItem = `<div class="card card-flush h-xl-100">
            <div class="card-body py-9">
                <div class="row gx-9 h-100">
                    <div class="col-sm-6 mb-10 mb-sm-0">
                        <div class="bgi-no-repeat bgi-position-center bgi-size-cover card-rounded min-h-400px min-h-sm-100 h-100" style="background-size: 100% 100%;background-image:url('${response.newsImageURL}')">
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="d-flex flex-column h-100">
                            <div class="mb-7">
                                <div class="d-flex flex-stack mb-6">
                                    <div class="flex-shrink-0 me-5">
                                        <span class="text-gray-500 fs-7 fw-bold me-2 d-block lh-1 pb-1">${localizer.lastPublishedNews}</span>
                                        <span class="text-gray-800 fs-1 fw-bold">${response.newsTitle.truncate(15)}</span>
                                    </div>
                                    <span class="badge badge-light-primary flex-shrink-0 align-self-center py-3 px-4 fs-7">${localizer.published}</span>
                                </div>
                                <div class="d-flex align-items-center flex-wrap d-grid gap-2 lastpublished-authorsection">
                            
                                </div>
                            </div>
                            <div class="mb-6">
                                <span class="fw-semibold text-gray-600 fs-6 mb-8 d-block">
                                    ${response.newsSubtitle.truncate(100)}
                                </span>
                                <div class="d-flex">
                                    <div class="border border-gray-300 border-dashed rounded min-w-100px w-100 py-2 px-4 me-6 mb-3">
                                        <span class="fs-6 text-gray-700 fw-bold">${response.publishDate}</span>
                                        <div class="fw-semibold text-gray-500">${localizer.publishDate}</div>
                                    </div>
                                </div>
                            </div>
                            <div class="d-flex flex-stack mt-auto bd-highlight">
                                <a href="/News/Details/${response.newsID}" class="d-flex align-items-center text-primary opacity-75-hover fs-6 fw-semibold">
                                    ${localizer.viewNews}

                                    <i class="ki-duotone ki-exit-right-corner fs-4 ms-1"><span class="path1"></span><span class="path2"></span></i>
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;

            cardBody.append(lastPublishedNewsItem);
            if (response.authorID != null) {
                const lastPublishedAuthorSection = $(".lastpublished-authorsection");

                var authorHTML = `        <div class="d-flex align-items-center me-5 me-xl-13">
                                        <div class="symbol symbol-30px symbol-circle me-3">
                                            <img src="${response.authorImageURL}" class="" alt="">
                                        </div>
                                        <div class="m-0">
                                            <span class="fw-semibold text-gray-500 d-block fs-8">${localizer.author}</span>
                                            <a href="/Authors/Details/${response.authorID}" class="fw-bold text-gray-800 text-hover-primary fs-7">${response.authorName}</a>
                                        </div>
                                    </div>
                                    <div class="d-flex align-items-center">
                                    </div>`;
                lastPublishedAuthorSection.append(authorHTML);
            }
        })
        .fail(function () {
        });

    GenericGetRequest('/Home/GetNewsChartDataByCategories/')
        .done(function (response) {
            const categories = response.categories;
            const newsCountsByCategory = response.newsCountsByCategory;

            var element = document.querySelector("#kt_charts_widget_18_chart");
            var aspectRatio = 4 / 3;

            var chartWidth = Math.min(window.innerWidth * 0.9, 820);
            var chartHeight = chartWidth / aspectRatio;

            window.addEventListener('resize', function () {
                var chartWidth = Math.min(window.innerWidth * 0.9, 820);
                var chartHeight = chartWidth / aspectRatio;

                chart.updateOptions({
                    chart: {
                        height: chartHeight + "px",
                        width: chartWidth + "px"
                    }
                });
            });

            var chartOptions = {
                series: [{
                    name: localizer.newsCount,
                    data: newsCountsByCategory
                }],
                chart: {
                    height: chartHeight + "px",
                    width: chartWidth + "px",
                    stacked: true,
                    type: 'bar',
                    toolbar: {
                        show: true
                    },
                    zoom: {
                        enabled: true
                    }
                },
                legend: {
                    show: false
                },
                dataLabels: {
                    enabled: false
                },
                grid: {
                    show: true
                },

                stroke: {
                    curve: 'smooth',
                    show: true,
                    width: 4,
                },
                xaxis: {
                    categories: categories,
                    labels: {
                        show: true
                    },
                    axisBorder: {
                        show: true
                    },
                    axisTicks: {
                        show: true
                    }
                },
                yaxis: {
                    labels: {
                        show: true
                    }
                },
                fill: {
                    opacity: 1
                },
                tooltip: {
                    enabled: true
                },
                noData: {
                    text: ` `,
                    align: "center",
                    verticalAlign: "middle",
                },
                plotOptions: {
                    bar: {
                        borderRadius: 5,
                        distributed: true
                    }
                }
            }
            let chart = new ApexCharts(element, chartOptions);
            chart.render();

        })
        .fail(function (error) {
        });

    GenericGetRequest('/Home/GetNewsChartDataByDailyNews/')
        .done(function (response) {
            const categories = response.newsCreateDates.toTurkishDates();
            console.log(categories)
            const newsCountsByCategory = response.newsCounts;

            var element = document.querySelector("#kt_charts_widget_36");
            var aspectRatio = 800 / 300;

            var chartWidth = Math.min(window.innerWidth * 0.9, 700);
            var chartHeight = chartWidth / aspectRatio;

            window.addEventListener('resize', function () {
                var chartWidth = Math.min(window.innerWidth * 0.9, 700);
                var chartHeight = chartWidth / aspectRatio;

                chart.updateOptions({
                    chart: {
                        height: chartHeight + "px",
                        width: chartWidth + "px"
                    }
                });
            });

            var chartOptions = {
                series: [{
                    name: localizer.newsCount,
                    data: newsCountsByCategory
                }],
                chart: {
                    height: chartHeight + "px",
                    width: chartWidth + "px",
                    type: 'line',
                    toolbar: {
                        show: true
                    },
                    zoom: {
                        enabled: true
                    }
                },
                legend: {
                    show: false
                },
                dataLabels: {
                    enabled: false
                },
                grid: {
                    show: true
                },

                stroke: {
                    show: true,
                    width: 4,
                },
                xaxis: {
                    categories: categories,
                    labels: {
                        show: true,
                        style: {
                            fontSize: '12px',
                            fontWeight: 'bold',   // 👈 kalın yapar
                            fontFamily: 'Arial'   // 👈 özel font istersen
                        }
                    },
                    axisBorder: {
                        show: true
                    },
                    axisTicks: {
                        show: false
                    }
                },
                yaxis: {
                    labels: {
                        show: true,
                        style: {
                            fontSize: '14px',
                            fontWeight: 'bold',   // 👈 kalın yapar
                            fontFamily: 'Arial'   // 👈 özel font istersen
                        }
                    }
                },
                fill: {
                    opacity: 1
                },
                tooltip: {
                    enabled: true,
                },
                noData: {
                    text: ` `,
                    align: "center",
                    verticalAlign: "middle",
                }
            }
            let chart = new ApexCharts(element, chartOptions);
            chart.render();

        })
        .fail(function () {
        });
});

String.prototype.truncate = function (length = 20) {
    if (this.length > length) {
        return this.substring(0, length) + '...';
    }
    return this;
};


//$(function () {
//    $('#dateRangePicker').daterangepicker({
//        opens: 'left',
//        startDate: moment().subtract(30, 'days'),
//        endDate: moment(),
//        locale: {
//            format: 'DD MMM YYYY',
//            applyLabel: 'Filtrele',
//            cancelLabel: 'İptal'
//        }
//    }, function (start, end) {
//        // Callback when a date range is selected
//        $('#dateRangePicker').val(start.format('DD MMM YYYY') + ' - ' + end.format('DD MMM YYYY'));
//        updateChartData(start, end); // Update chart data based on the selected range
//    });

//    // Set initial date range text
//    $('#dateRangePicker').val(moment().subtract(30, 'days').format('DD MMM YYYY') + ' - ' + moment().format('DD MMM YYYY'));
//});

function formatTurkishDate(dateStr) {
    const [day, month, year] = dateStr.split(".");
    const date = new Date(year, month - 1, day);

    return new Intl.DateTimeFormat("tr-TR", {
        day: "numeric",
        month: "short",
        year: "numeric"
    }).format(date);
}

// String extension
if (!String.prototype.toTurkishDate) {
    Object.defineProperty(String.prototype, "toTurkishDate", {
        value: function () {
            return formatTurkishDate(this);
        },
        writable: false
    });
}

// Array extension
if (!Array.prototype.toTurkishDates) {
    Object.defineProperty(Array.prototype, "toTurkishDates", {
        value: function () {
            return this.map(d => formatTurkishDate(d));
        },
        writable: false
    });
}
