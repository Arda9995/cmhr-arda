let slider = document.querySelector("#slider");
let txtAutoSlideDelay = document.querySelector("#txtAutoSlideDelay");

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

$('input[type=radio]').change(function() {
    if (this.id === 'block_slider') {
        autoplayDelay.slideToggle('fast');
    }else if (this.id === 'block_catalog') {
        autoplayDelay.slideToggle('fast');
    }
});