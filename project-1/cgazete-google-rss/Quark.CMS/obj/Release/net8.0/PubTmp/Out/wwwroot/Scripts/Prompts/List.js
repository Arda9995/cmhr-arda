$(document).ready(function () {
    var promptsColumns = [
        {
            "data": "name", "name": "Name", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col2", [row.id, row.name]); }
        },
        {
            "data": "promptType", "name": "PromptType", "autoWidth": true,
            "render": function (data, type, row) { return GetTableHtml("Col3", [row.promptType]); }
        },
        {
            "data": "action", "name": "Action", "class": "w-100px text-center", "orderable": false,
            "render": function (data, type, row) { return GetTableHtml("Col12", [row.id, row.id]); }
        }
    ];
    CreateTable("#datatable", "/Prompts/GetAllPaged", `${localizer.prompts}`, promptsColumns, applyFilterData)
    handleSearchDatatable();
})

function applyFilterData(data) {
    var b = document.getElementById("promptType");
    data.SelectedPromptTypeID = parseInt(b.options[b.selectedIndex].value);
}

var applyFilter = () => {
    ReloadAjax();
}