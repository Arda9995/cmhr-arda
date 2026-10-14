function EnableSurvey(id){
    Confirm("Bu anketi aktifleştirmek şuanda aktif olan diğer anketi pasif hale getirir. Bunu yapmak istediğinize emin misiniz?", 
        "/Surveys/EnableSurvey",
        id,
        ReloadFunc,
        ReloadFunc);
}

function RefreshAnswers(id){
    $.ajax({
        url: `/Surveys/RefreshAnswers/${id}`,
        type: "POST",
        success: function(response) {
            window.location.reload();
        }
    })
}

function ReloadFunc(){
    location.reload();
}

google.load('visualization', '1', {
    packages: ['corechart', 'bar', 'line']
});

google.setOnLoadCallback(function () {
    var hiddenValue = document.getElementById("options-dictionary").value;
    var surveyData = JSON.parse(hiddenValue);

    var dataArray = [['Survey', 'Survey']];
    var optionColors = {};

    function getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
    
    Object.entries(surveyData).forEach(([key, value]) => {
        var randomColor = getRandomColor();
        optionColors[key] = randomColor; 
        dataArray.push([key, parseInt(value)]);
    });
    
    var data = google.visualization.arrayToDataTable(dataArray);

    var options = {
        colors: Object.values(optionColors), 
        legend: { position: "none" },
        chartArea: { left: 0, top: 0, right: 0, bottom: 0 }
    };

    var chart = new google.visualization.PieChart(document.getElementById('kt_docs_google_chart_pie'));
    chart.draw(data, options);
    
    document.querySelectorAll('.bullet').forEach(bullet => {
        var optionKey = bullet.getAttribute('data-option');
        if (optionColors[optionKey]) {
            bullet.style.backgroundColor = optionColors[optionKey];
        }
    });

    window.onresize = function () {
        chart.draw(data, options);
    };
});
