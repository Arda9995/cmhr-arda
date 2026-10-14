var onlyEnableds = true;
var currentID = 0;
var currentIDHasChildren = false;
var DetailsID =parseInt(window.location.pathname.split("/")[3]);

$("#divMenusTree").jstree({
    "core": {
        'data': {
            'url': function (node) {
                if(node.id === "#") {
                    return '/Menus/LoadTree?onlyEnableds=' + onlyEnableds + '&parent=' + window.location.pathname.split("/")[3] 
                }else{
                    return '/Menus/LoadTree?onlyEnableds=' + onlyEnableds + '&parent=' + node.id 
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
    $("#divMenusTree").jstree().refresh();
}

$('#divMenusTree').on("changed.jstree", function (e, data) {
    currentID = 0;
    if (data.selected.length > 0) {
        currentIDHasChildren = data.node.children.length > 0;
        currentID = data.selected;
        $("#spanSelectedMenus").text(data.instance.get_node(data.selected[0]).text);
    } else {
        $("#spanSelectedMenus").text("--");
    }
    //GetMenuDetail();
});

// -------------------------------------- PRODUCT PROPERTIES LIST

function ChangeStatus(isEnabled) {
    if (currentID == 0) {
        ShowError(`${localizer.selectMenuErrorMsg}`);
        return;
    }

    Swal.fire({
        title: `${localizer.changeMenuStatus}`,
        text: `${localizer.changeMenuStatusConfirmation}`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `${localizer.yes}`,
        cancelButtonText: `${localizer.no}`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Menus/ChangeStatus?id=' + currentID + '&isEnabled=' + isEnabled,
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
        ShowError(`${localizer.selectMenuErrorMsg}`);
        return;
    }

    Swal.fire({
        title: `${localizer.delete}`,
        text: `${localizer.deleteMenuConfirmation}`,
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
                url: '/Menus/Delete?id=' + currentID,
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
    
    if (currentID == 0) {
        ShowError(`${localizer.selectMenuErrorMsg}`);
        return;
    }
    
    window.location.href = "/Menus/Create?currentId=" + currentID + "&menuId=" + menuId;
}

function GotoAddMain() {
    
    window.location.href = "/Menus/Create?menuId=" + menuId;
}
function GotoUpdate() {
    if (currentID == 0) {
        ShowError(`${localizer.selectMenuErrorMsg}`);
        return;
    }

    window.location.href = "/Menus/Update?currentId=" + currentID + "&menuId=" + menuId;
}

var removePhoto = function (id) {
    document.getElementById(id).value = '';
}