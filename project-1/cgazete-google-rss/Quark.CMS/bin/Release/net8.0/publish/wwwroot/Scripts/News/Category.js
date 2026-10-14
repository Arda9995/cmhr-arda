var blockUI = new KTBlockUI(document.querySelector('#categoryCard'));
var onlyEnableds = true;

function formatCategoryIDs(categoryIDs) {
    return categoryIDs ? categoryIDs
        .split('-')
        .map(id => id.replace(/^\||\|$/g, ''))
        .filter(id => id.trim() !== "") : [];
}

function getLastCategoryId(idString) {
    const trimmedString = idString.replace(/^\||\|$/g, '');
    const idParts = trimmedString.split('|');
    return idParts[idParts.length - 1];
}

var selectedMainCategoryIDs = formatCategoryIDs($("#mainCategoryIdMap").val());
var selectedSecondaryCategoryIDs = formatCategoryIDs($("#secondaryCategoryIDs").val());


function initializeTree(treeSelector, selectedIDs, saveFunction, allowMultiple = true) {
    $(treeSelector).jstree({
        "core": {
            'data': {
                'url': node => node.id === "#"
                    ? `/Categories/LoadTree?onlyEnableds=${onlyEnableds}`
                    : `/Categories/LoadTree?onlyEnableds=${onlyEnableds}&parent=${node.id}`,
                'data': node => ({ 'parent': node.id })
            },
        },
        "plugins": allowMultiple ? ["checkbox"] : [],
        "checkbox": {
            "three_state": false,
        },
        "multiple": allowMultiple
    }).on("ready.jstree", function () {
        if (selectedIDs && selectedIDs.length > 0) {
            selectedIDs.forEach(id => {
                if (id.includes('|')) {
                    let idParts = id.split('|').filter(Boolean);
                    openParentsAndSelectChild(treeSelector, idParts.slice(0, -1), idParts[idParts.length - 1]);
                } else {
                    $(treeSelector).jstree("select_node", id);
                }
            });

            if (treeSelector === "#divMainCategoriesTree" && selectedIDs.length === 1) {
                $("#mainCategoryId").val(getLastCategoryId(selectedIDs[0]));
            }

            setTimeout(() => saveFunction(), 500);
        }
    }).on("select_node.jstree", function (e, data) {
        let parent = data.node.parent;
        while (parent !== "#") {
            $(treeSelector).jstree("select_node", parent);
            parent = $(treeSelector).jstree("get_node", parent).parent;
        }
    }).on("deselect_node.jstree", function (e, data) {
        let children = data.node.children_d;
        children.forEach(child => {
            $(treeSelector).jstree("deselect_node", child);
        });
    });


}

function openParentsAndSelectChild(treeSelector, parents, child, index = 0) {
    if (index < parents.length) {
        $(treeSelector).jstree("open_node", parents[index], () => openParentsAndSelectChild(treeSelector, parents, child, index + 1));
    } else {
        $(treeSelector).jstree("select_node", child);
    }
}

function saveCategories(treeSelector, spanSelector, inputSelector, type) {
    var selectedNodes = $(treeSelector).jstree("get_selected", true);
    var selectedCategories = [];
    var selectedCategoryNames = [];

    if (selectedNodes.length > 0) {
        blockUI.block();

        if (type == "1") {
            let node = selectedNodes[0];
            if (node.children.length === 0) {
                let hierarchy = [];
                let originalNodeText = node.text;

                while (node) {
                    hierarchy.unshift(node.id);
                    node = $(treeSelector).jstree("get_node", node.parent);
                    if (node.id === "#") break;
                }

                selectedCategories.push(`|${hierarchy.join("|")}|`);
                selectedCategoryNames.push(originalNodeText);

                var updatedMainCategoryId = getLastCategoryId(selectedCategories[0]);
                $("#mainCategoryId").val(updatedMainCategoryId);
                $("#mainCategoryIdMap").val(selectedCategories);
            }
        } else {
            selectedNodes.forEach(node => {
                selectedCategoryNames.push(node.text);
                if (node.children.length === 0) {
                    let hierarchy = [];
                    while (node) {
                        hierarchy.unshift(node.id);
                        node = $(treeSelector).jstree("get_node", node.parent);
                        if (node.id === "#") break;
                    }

                    selectedCategories.push(`|${hierarchy.join("|")}|`);

                    $("#SecondaryCategoryIDMaps").val(selectedCategories);
                }
            });

            $(inputSelector).val(selectedCategories.join("-"));
        }

        $(spanSelector).text(selectedCategoryNames.length > 0 ? selectedCategoryNames.join(", ") : "--");
        blockUI.release();
    } else {
        $(spanSelector).text("--");
        $(inputSelector).val("");
    }
}


function saveCategoriesWithHierarchy(treeSelector, spanSelector, inputSelector) {
    var selectedNodes = $(treeSelector).jstree("get_selected", true);
    var selectedCategories = [];
    var selectedCategoryNames = [];

    if (selectedNodes.length > 0) {
        blockUI.block();

        selectedNodes.forEach(node => {
            let hierarchy = [];
            let originalNodeText = node.text;

            while (node) {
                hierarchy.unshift(node.id);
                node = $(treeSelector).jstree("get_node", node.parent);
                if (node.id === "#") break;
            }

            selectedCategories.push(`|${hierarchy.join("|")}|`);
            selectedCategoryNames.push(originalNodeText);

            let parentNode = $(treeSelector).jstree("get_node", node.parent);
            while (parentNode && parentNode.id !== "#") {
                selectedCategories.push(`|${parentNode.id}|`);
                selectedCategoryNames.push(parentNode.text);
                parentNode = $(treeSelector).jstree("get_node", parentNode.parent);
            }
        });

        $(spanSelector).text(selectedCategoryNames.join(", "));
        $(inputSelector).val(selectedCategories.join("-"));
        blockUI.release();
    } else {
        $(spanSelector).text("--");
        $(inputSelector).val("");
    }
}


$(document).ready(function () {
    $("#newsFormCreate").on('submit', function (e) {
        var mainCategoryIdForValidation = $("#mainCategoryId").val();
        var mainCategoryIdMapForValidation = $("#mainCategoryIdMap").val();

        setInterval(function () {
            var mainCategoryIdForValidation = $("#mainCategoryId").val();
            var mainCategoryIdMapForValidation = $("#mainCategoryIdMap").val();

            if (!mainCategoryIdForValidation || !mainCategoryIdMapForValidation) {
                $("#mainCategoryValidation").text(localizer.pleaseSelectMainCategory);
            } else {
                $("#mainCategoryValidation").text("");
            }
        }, 500);

        if (!mainCategoryIdForValidation || !mainCategoryIdMapForValidation) {
            e.preventDefault();
            $("#mainCategoryValidation").text(localizer.pleaseSelectMainCategory);
        }
    });
});



initializeTree("#divMainCategoriesTree", selectedMainCategoryIDs, () => saveCategories("#divMainCategoriesTree", "#spanSelectedMainCategory", "#mainCategoryIdMap", "1"), false);
initializeTree("#divSecondaryCategoriesTree", selectedSecondaryCategoryIDs, () => saveCategories("#divSecondaryCategoriesTree", "#spanSelectedSecondaryCategories", "#secondaryCategoryIDs", "2"), true);

