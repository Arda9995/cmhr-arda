/// V1.2.3
/// Update Time: 06.10.2023
/// Author: Murat YÄ±lmaz

var documentTitle = 'Export Data';

function GetDefaultDataTableOptions(page = 20) {
    var defaultDataTableOptions = {
        "language": { "url": datatablesLanguageFile },
        "pageLength": page,
        "lengthMenu": [[page, 50, 100], [page, 50, 100]],

    };

    return defaultDataTableOptions;
}

function dtXssHandler(data) {
    if (data !== null) {
        if (typeof data === 'object') {
            for (var i = 0; i < Object.entries(data).length; i++) {
                var propName = Object.entries(data)[i][0]
                var propValue = Object.entries(data)[i][1];
                data[propName] = dtXssHandler(propValue);
            }
        }
        else if (Array.isArray(data)) {
            for (var i = 0; i < data.length; i++) {
                data[i] = dtXssHandler(data[i])
            }
        }
        else if (typeof data === 'string') {
            data = data.replace(/[\u00A0-\u9999<>\&]/g, function (i) {
                return '&#' + i.charCodeAt(0) + ';';
            });
        }
    }

    return data;
}

function CreateTable(tableName, url, exportedDocumentName, columns, filterData = {}, page = 20, options = {}) {
    documentTitle = exportedDocumentName;
    
    var datatableOptions = {
        "order": [],
        "searchDelay": 2000,
        "processing": true,
        "serverSide": true,
        "filter": true,
        ...options
    }
    
    let datatable = $(tableName).DataTable({
        ...GetDefaultDataTableOptions(page),
        ...datatableOptions,
        ajax: {
            url: url,
            data: filterData,
            type: "POST",
            "dataSrc": function (response) {
                response = dtXssHandler(response);

                return response.data;
            },
            "error": function (xhr, textStatus) {
                $(tableName + '_processing').hide();
                console.error('DataTable ajax error', url, xhr.status, textStatus);
                alert('Sunucu zamanında yanıt vermedi. Lütfen birkaç saniye sonra tekrar deneyin.');
            },
        },
        "columnDefs": [
            {
                "targets": [0],
                "visible": true,
                "searchable": false
            }
        ],
        "columns": columns
    });
    datatable.on('draw', function () {
        var buttons = $(".dt-buttons > .btn-secondary")
        buttons.addClass("btn-light-primary");
        buttons.removeClass("btn-secondary");
        $('[data-bs-toggle="tooltip"]').tooltip();

        // Metronic datatable styling
        $(tableName).addClass("table align-middle table-row-dashed fs-6 gy-5 dataTable no-footer");
        $(tableName + " thead tr").addClass("text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0");
        $(tableName + " tbody").addClass("fw-semibold text-gray-600");
        $(tableName + " tbody a:not(.btn):not(.noclass)").addClass("text-gray-800 text-hover-primary fs-5 fw-bold");
        KTMenu.createInstances();
    });
    $('.dropdown-toggle').dropdown();
    //exportButtons();

    return datatable;
}

var initDatatable = function () {
    dt = $("#kt_datatable_example_1").DataTable({
        searchDelay: 500,
        processing: true,
        serverSide: true,
        order: [[5, 'desc']],
        stateSave: true,
        select: {
            style: 'multi',
            selector: 'td:first-child input[type="checkbox"]',
            className: 'row-selected'
        },
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                render: function (data) {
                    return `
                            <div class="form-check form-check-sm form-check-custom form-check-solid">
                                <input class="form-check-input" type="checkbox" value="${data}" />
                            </div>`;
                }
            },
            {
                targets: 4,
                render: function (data, type, row) {
                    return `<img src="${hostUrl}media/svg/card-logos/${row.CreditCardType}.svg" class="w-35px me-3" alt="${row.CreditCardType}">` + data;
                }
            },
            {
                targets: -1,
                data: null,
                orderable: false,
                className: 'text-end',
                render: function (data, type, row) {
                    return;
                },
            },
        ],
        // Add data-filter attribute
        createdRow: function (row, data, dataIndex) {
            $(row).find('td:eq(4)').attr('data-filter', data.CreditCardType);
        }
    });
    table = dt.$;

    // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw
    dt.on('draw', function () {
        initToggleToolbar();
        toggleToolbars();
        handleDeleteRows();
        KTMenu.createInstances();
    });
    
    dt.on('reload', function() { console.log("reload")})
}


function ReloadAjax(specifiedDatatable) {
    if (specifiedDatatable) {
        specifiedDatatable.ajax.reload();
    } else {
        datatable.ajax.reload();
    }
}

function GetTableHtml(id, valueList = []) {
    let html = document.getElementById(id).innerHTML;
    for (var i = 0; i < valueList.length; i++) {
        html = html.replace("{" + i + "}", valueList[i])
        html = html.replace("{" + i + "}", valueList[i])
    }
    return html;
}

var handleSearchDatatable = (dt = datatable, selector = '[data-kt-filter="datatable-search"]') => {
    const filterSearch = document.querySelector(selector);
    filterSearch.addEventListener('keyup', function (e) {
        dt.search(e.target.value).draw();
    });
}

var exportButtons = () => {
    var buttons = new $.fn.dataTable.Buttons(datatable, {
        buttons: [
            {
                extend: 'copyHtml5',
                title: documentTitle
            },
            {
                extend: 'excelHtml5',
                title: documentTitle
            },
            {
                extend: 'csvHtml5',
                title: documentTitle
            },
            {
                extend: 'pdfHtml5',
                title: documentTitle
            }
        ]
    }).container().appendTo($('#datatable_export_button'));

    // Hook dropdown menu click event to datatable export buttons
    const exportButtons = document.querySelectorAll('#datatable_export_button_menu [data-kt-export]');
    exportButtons.forEach(exportButton => {
        exportButton.addEventListener('click', e => {
            e.preventDefault();

            // Get clicked export value
            const exportValue = e.target.getAttribute('data-kt-export');
            const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

            // Trigger click event on hidden datatable export buttons
            target.click();
        });
    });
};