/////////////////////////////////////////////////////////////////////////////////
// NO UI SLIDER
/////////////////////////////////////////////////////////////////////////////////
let slider = document.querySelector("#slider");
let txtAutoSlideDelay = document.querySelector("#txtAutoSlideDelay");
let block_slider = document.getElementById("block_slider");
let block_catalog = document.getElementById("block_catalog");

noUiSlider.create(slider, {
    start: [10],
    connect: !0,
    range: {
        min: 0,
        max: 20
    }
});

slider.noUiSlider.on("update", function (values, handle) {
    lblAutoSlideDelay.innerHTML = Math.round(values[handle]);
    txtAutoSlideDelay.value = Math.round(values[handle]);
});

/////////////////////////////////////////////////////////////////////////////////
// RADIO BUTTONS
/////////////////////////////////////////////////////////////////////////////////

let autoplayDelay = $('#autoplayDelay');

window.onload = function (){
    if (blockTypeID === '2') {
        autoplayDelay.slideToggle('fast');
        block_catalog.checked = "checked";
    }
    else if(blockTypeID === '1'){
        block_slider.checked = "checked";
    }
}

$('input[type=radio]').change(function() {
    if (this.id === 'block_slider') {
        autoplayDelay.slideToggle('fast');
    }else if (this.id === 'block_catalog') {
        autoplayDelay.slideToggle('fast');
    }
});