var newsDraggable = null;
var selectedNewsItems = [];
var min = 0;
var max = 0;
var leftNewsContainerBlockUI = null;
var rightNewsContainerBlockUI = null;
var pagesDropdownBlockUI = null;
var blocksDropdownBlockUI = null;
var tooltipTriggerList = null;
var currentPage = 1;

$(document).ready(function () {
    var leftNewsContainer = document.querySelector('#newsList');
    leftNewsContainerBlockUI = new KTBlockUI(leftNewsContainer);

    var rightNewsContainer = document.querySelector('#selectedNews');
    rightNewsContainerBlockUI = new KTBlockUI(rightNewsContainer);

    var pagesDropdown = document.querySelector('#pagesDropdown');
    pagesDropdownBlockUI = new KTBlockUI(pagesDropdown);

    var blocksDropdown = document.querySelector('#blocksDropdown');
    blocksDropdownBlockUI = new KTBlockUI(blocksDropdown);

    getNewsPaged(1, 10);
    getPages();
    getBlocks();
    toggleChooseBlockHint();

    // Scroll sırasında mirror elementlerini temizle
    $('.sticky-column').on('scroll', function() {
        cleanupDraggableMirrors();
    });
})

// Mirror elementlerini temizleyen fonksiyon
function cleanupDraggableMirrors() {
    $('.draggable-mirror').remove();
    $('.draggable-source--is-dragging').removeClass('draggable-source--is-dragging');
}

// Kaydet butonu
function saveBlock() {
    var selectedBlock = $('#blocksList').val();
    if(selectedBlock == null || selectedBlock == -1){
        return;
    }
    if(selectedNewsItems.length == 0) {
        return;
    }
    if(selectedNewsItems.length < min || selectedNewsItems.length > max){
        $('#minMaxValidationErrorBox').removeClass('d-none').addClass('d-flex');
        return;
    }
    setSelectedNewsByOrder();
    $.ajax({
        url: '/Blocks/SaveBlockNews',
        method: 'POST',
        data: {
            blockId: selectedBlock,
            selectedNewsItems: selectedNewsItems
        },
        success: function (response) {
            if (response.isSuccess) {
                toastr.success(`${localizer.blockManageSuccessMsg}`);
            } else {
                toastr.error(`${localizer.blockManageErrorMsg}`);
            }
        }
    })

}

// selectedNewsItems listesini ekrandaki ordera göre sort eder.
function setSelectedNewsByOrder() {
    var selectedNewsDiv = document.querySelector('#selectedNews');
    var newsItems = Array.from(selectedNewsDiv.querySelectorAll('.newsItem'));
    selectedNewsItems = [];

    newsItems.forEach((item, index) => {
        selectedNewsItems.push({
            newsId: item.querySelector('.newsIds').value,
            order: index + 1
        });
        let orderElement = item.querySelector('.newsOrder');
        if (orderElement) {
            orderElement.textContent = index + 1;
        }
    });
}

// ekrandaki min ve max değerlerini doldurur
function setMinMaxValues() {
    $.ajax({
        url: '/Blocks/GetMinMaxValues',
        method: 'GET',
        data: {
            blockId: $('#blocksList').val()
        },
        success: function (response) {
            if(response.isSuccess){
                $('.minValue').html(response.data.min);
                $('.maxValue').html(response.data.max);
                min = response.data.min;
                max = response.data.max;
            }else{
                $('.minValue').html(0);
                $('.maxValue').html(0);
                min = 0;
                max = 0;
            }
        }
    })
}

// Sayfalar dropdownını doldurur
function getPages(){
    $('#minMaxValidationErrorBox').removeClass('d-flex').addClass('d-none');
    pagesDropdownBlockUI.block();
    $.ajax({
        url: '/Pages/GetAllSelectList',
        method: 'GET',
        success: function (response) {
            let selectlist = document.querySelector('#pagesList');
            response.data.map(x => $(selectlist).append(`<option value="${x.value}">${x.text}</option>`));
            pagesDropdownBlockUI.release();
        }
    })
}

// Bloklar dropdownını doldurur
function getBlocks() {
    $('#minMaxValidationErrorBox').removeClass('d-flex').addClass('d-none');
    blocksDropdownBlockUI.block();
    $.ajax({
        url: '/Blocks/GetAllSelectList?selectedPageId=' + $('#pagesList').val(),
        method: 'GET',
        success: function (response) {
            let selectlist = document.querySelector('#blocksList');
            $(selectlist).empty();
            response.data.map(x => $(selectlist).append(`<option value="${x.value}">${x.text}</option>`));
            blocksDropdownBlockUI.release();
        }
    })
}

// Soldaki haberler listesini paginated olarak getirir
function getNewsPaged(page, pageSize) {
    var searchValue = document.querySelector('#search-value')?.value;
    currentPage = page;
    leftNewsContainerBlockUI.block();
    $.ajax({
        url: '/Blocks/GetNewsPaged',
        method: 'POST',
        data: {
            searchValue: searchValue,
            page: page,
            pageSize: pageSize,
            selectedNewsItems: selectedNewsItems
        },
        success: function (response) {
            var newsList = document.querySelector('#newsList');
            newsList.innerHTML = response;
            leftNewsContainerBlockUI.release();
            NewsDraggable.init();

            let popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
            let popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
                return new bootstrap.Popover(popoverTriggerEl)
            });
        }
    })
}

// Sağdaki haberler listesini blockId'ye göre getirir
function getNewsByBlockId() {
    rightNewsContainerBlockUI.block();
    $.ajax({
        url: '/Blocks/GetNewsByBlockId?blockId=' + $('#blocksList').val(),
        method: 'GET',
        success: function (response) {
            rightNewsContainerBlockUI.release();
            var selectedNewsList = document.querySelector('#selectedNews');
            if (response != "") {
                selectedNewsList.innerHTML += response;
                NewsDraggable.init();
                var parser = new DOMParser();
                var doc = parser.parseFromString(response, "text/html");
                Array.from(doc.querySelectorAll('.newsIds')).map((item, index) => {
                    selectedNewsItems.push({ newsId: item.value, order: index + 1 })
                })
                toggleHowToAddNewsHint();
                getNewsPaged(1, 10)

                // Add remove buttons to news items
                addRemoveButtons();
            }
        }
    })
}

// Haber öğelerine X butonu ekler
function addRemoveButtons() {
    const newsItems = document.querySelectorAll('#selectedNews .newsItem');
    newsItems.forEach(item => {
        if (!item.querySelector('.remove-news-btn')) {
            const removeBtn = document.createElement('button');
            removeBtn.className = 'btn btn-icon btn-sm btn-light-danger remove-news-btn position-absolute top-0 end-0 m-2';
            removeBtn.innerHTML = '<i class="ki-duotone ki-cross-circle fs-2"><span class="path1"></span><span class="path2"></span></i>';
            removeBtn.onclick = function() {
                const newsId = item.querySelector('.newsIds').value;
                selectedNewsItems = selectedNewsItems.filter(item => item.newsId !== newsId);
                item.remove();
                setSelectedNewsByOrder();
                toggleHowToAddNewsHint();
                getNewsPaged(currentPage, 10);
                $('#minMaxValidationErrorBox').removeClass('d-flex').addClass('d-none');
            };
            item.style.position = 'relative';
            item.appendChild(removeBtn);
        }
    });
}

// Draggable'ı initialize eder
var NewsDraggable = {
    // Drag işlemi sonlandığında çalışır
    onDragStop: function (e) {
        var isRight = e.data.sourceContainer.classList.contains('draggable-right');
        if (!isRight) {
            setSelectedNewsByOrder();
        }
        toggleHowToAddNewsHint();
        getNewsPaged(currentPage, 10);
        $('#minMaxValidationErrorBox').removeClass('d-flex').addClass('d-none');
        
        // Drag işlemi bittiğinde mirror elementlerini temizle
        cleanupDraggableMirrors();
    },

    // Obje, draggable bir container üzerine taşındığında çalışır
    onDragOver: function(e) {
        var newsId = e.data.source.querySelector('.newsIds').value;
        var isLeft = e.data.overContainer.classList.contains('draggable-left');
        if(isLeft){
            selectedNewsItems = selectedNewsItems.filter(item => item.newsId !== newsId);
        }
    },

    // Drag işlemi başladığında çalışır.
    onDragStart: function(e) {
        let selectedBlock = $('#blocksList').val();
        if(selectedBlock == null || selectedBlock == -1){
            e.cancel();
            return;
        }

        // Önceki mirror elementlerini temizle
        cleanupDraggableMirrors();
    },

    init: function () {
        let self = this;
        !function () {
            if(newsDraggable != null){
                newsDraggable.destroy();
            }
            var containers = document.querySelectorAll(".custom-draggable-zone");
            if (0 === containers.length) return !1;
            newsDraggable = new Draggable.Sortable(containers, {
                draggable: ".draggable",
                handle: ".draggable .draggable-handle",
                mirror: {
                    appendTo: "body",
                    constrainDimensions: !0,
                    xAxis: false,
                    yAxis: true
                }
            })
                .on('drag:stop', self.onDragStop)
                .on('drag:over:container', self.onDragOver)
                .on('drag:start', self.onDragStart)
                .on('mirror:destroy', function() {
                    cleanupDraggableMirrors();
                });
        }()
    }
};

// Sayfa veya blok seçimi değiştiğinde yapılan seçimleri sıfırlar.
function resetSelections() {
    $('#selectedNews > .newsItem').remove();
    selectedNewsItems = [];
    getNewsPaged(currentPage, 10);
    toggleHowToAddNewsHint();
    toggleChooseBlockHint();
    $('.minValue').html(0);
    $('.maxValue').html(0);
    $('#minMaxValidationErrorBox').removeClass('d-flex').addClass('d-none');
}

// Sıfırla butonu
function resetChanges() {
    window.location.reload();
}

// ChooseBlockHint'i toggle eder
function toggleChooseBlockHint(){
    let selectedBlock = $('#blocksList').val();
    let selectedPage = $('#pagesList').val();
    if((selectedBlock == null || selectedBlock == -1) || (selectedPage == null || selectedPage == -1)){
        $('#chooseBlockHint').addClass('d-flex').removeClass('d-none');
    } else {
        $('#chooseBlockHint').addClass('d-none').removeClass('d-flex');
    }
}

// HowToAddNewsHint'i toggle eder
function toggleHowToAddNewsHint(){
    var selectedBlock = $('#blocksList').val();
    let selectedPage = $('#pagesList').val();

    if((selectedBlock != null && selectedBlock > -1) && (selectedPage != null && selectedPage > -1) && selectedNewsItems.length == 0){
        $('#howToAddNewsHint').addClass('d-flex').removeClass('d-none');
    } else {
        $('#howToAddNewsHint').addClass('d-none').removeClass('d-flex');
    }
}