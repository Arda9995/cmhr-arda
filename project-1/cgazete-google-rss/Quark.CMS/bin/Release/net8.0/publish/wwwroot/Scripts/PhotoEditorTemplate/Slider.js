var slider = document.querySelector("#opacity_slider");
var value = document.querySelector("#opacity_value");
var inputOpacity = document.querySelector("#Opacity");
preparePhoto()

noUiSlider.create(slider, {
    start: inputOpacity.value,
    range: {
        min: 0,
        max: 100
    },
});

slider.noUiSlider.on("update", function (values,handle) {
    value.textContent = Math.round(values[handle]);
});

slider.noUiSlider.on("change", function (values,handle) {
    inputOpacity.value = Math.round(values[handle]);
    preparePhoto()
});

