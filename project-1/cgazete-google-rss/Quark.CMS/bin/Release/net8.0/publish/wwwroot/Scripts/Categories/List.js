var onlyEnableds = true;
var currentID = 0;
var currentIDHasChildren = false;

$("#divCategoriesTree").jstree({
    "core": {
        'data': {
            'url': function (node) {
                if(node.id === "#") {
                    return '/Categories/LoadTree?onlyEnableds=' + onlyEnableds 
                }else{
                    return '/Categories/LoadTree?onlyEnableds=' + onlyEnableds + '&parent=' + node.id
                }
            },

            'data': function (node) { return { 'parent': node.id }; }
        }
    },
    "plugins": ["state", "types"]
});

function ShowOnlyEnableds() {
    onlyEnableds = true;
    RefreshData();
}

function ShowAll() {
    onlyEnableds = false;
    RefreshData();
}

function RefreshData() {
    $("#divCategoriesTree").jstree().refresh();
}

$('#divCategoriesTree').on("changed.jstree", function (e, data) {
    currentID = 0;
    if (data.selected.length > 0) {
        currentIDHasChildren = data.node.children.length > 0;
        currentID = data.selected;
        $("#spanSelectedCategories").text(data.instance.get_node(data.selected[0]).text);
    } else {
        $("#spanSelectedCategories").text("--");
    }
    //GetMenuDetail();
});

// -------------------------------------- PRODUCT PROPERTIES LIST

function ChangeStatus(isEnabled) {
    if (currentID == 0) {
        ShowError(`${localizer.selectCategoryErrorMsg}`);
        return;
    }

    Swal.fire({
        title: `${localizer.changeCategoryStatus}`,
        text: `${localizer.changeCategoryStatusConfirmation}`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `${localizer.yes}`,
        cancelButtonText: `${localizer.no}`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Categories/ChangeStatus?id=' + currentID + '&isEnabled=' + isEnabled,
                success: function (data) {
                    RefreshData();
                },
                error: function (xhr, textStatus, exceptionThrown) { ShowError(JSON.parse(xhr.responseText)); }
            });
        }
    });
}

function Delete() {
    if (currentID == 0) {
        ShowError(`${localizer.selectCategoryErrorMsg}`);
        return;
    }

    Swal.fire({
        title: `${localizer.delete}`,
        text: `${localizer.deleteCategoryConfirmation}`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `${localizer.yes}`,
        cancelButtonText: `${localizer.no}`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/Categories/Delete?id=' + currentID,
                success: function (data) {
                    if (data.isSuccess == false) {
                        ShowError(data.message);
                    }
                    RefreshData();
                },
                error: function (xhr, textStatus, exceptionThrown) { ShowError(JSON.parse(xhr.responseText)); }
            });
        }
    });
}

function GotoAdd() {

    if (currentID === 0) {
        window.location.href = "/Categories/Create";
    }

    window.location.href = "/Categories/Create?id=" + currentID;
}
function GotoUpdate() {
    if (currentID === 0) {
        ShowError(`${localizer.selectCategoryErrorMsg}`);
        return;
    }

    window.location.href = "/Categories/Update?id=" + currentID;
}

function GotoDetails(){
    if (currentID === 0) {
        ShowError(`${localizer.selectCategoryErrorMsg}`);
        return;
    }

    window.location.href = "/Categories/Details?id=" + currentID;
}

var removePhoto = function (id) {
    document.getElementById(id).value = '';
}