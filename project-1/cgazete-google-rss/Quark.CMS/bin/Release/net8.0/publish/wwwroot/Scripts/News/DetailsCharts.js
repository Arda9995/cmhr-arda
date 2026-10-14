$(document).ready(function () {
    var analyticsCard = document.querySelector('#analytics_card');
    analyticsCardBlockUI = new KTBlockUI(analyticsCard);
    
    
});

$('#analytics_card').on('show.bs.collapse', function (){
    analyticsCardBlockUI.block();

    $.ajax({
        url: "/News/GetDetailsImpressions",
        type: 'GET',
        data : {id : newsID},
        success: function(response) {
            const labels_minute = response.data.labelsMinute;
            const min_data = response.data.minData || [];
            const labels_hour = response.data.labelsHour;
            const hour_data = response.data.hourData || [];
            const labels_day = response.data.labelsDay;
            const day_data = response.data.dayData || [];


            var element1 = document.getElementById('kt_apexcharts_1');
            var element2 = document.getElementById('kt_apexcharts_2');
            var element3 = document.getElementById('kt_apexcharts_3');

            var height = parseInt(KTUtil.css(element1, 'height'));
            var labelColor = "#000000";
            var borderColor = "#F5F2F2";
            var baseColor = "#1498F4";
            var lightColor = "#7BC1F2";

            function getChartOptions(labels, data, name) {
                return {
                    series: [{
                        name: name,
                        data: data
                    }],
                    chart: {
                        fontFamily: 'inherit',
                        type: 'area',
                        height: height,
                        toolbar: {
                            show: false
                        }
                    },
                    dataLabels: {
                        enabled: false
                    },
                    xaxis: {
                        categories: labels,
                        axisBorder: {
                            show: false,
                        },
                        axisTicks: {
                            show: false
                        },
                        labels: {
                            style: {
                                colors: labelColor,
                                fontSize: '12px'
                            }
                        }
                    },
                    yaxis: {
                        labels: {
                            style: {
                                colors: labelColor,
                                fontSize: '12px'
                            }
                        }
                    },
                    tooltip: {
                        style: {
                            fontSize: '12px'
                        }
                    },
                    colors: [baseColor, lightColor],
                    grid: {
                        borderColor: borderColor,
                        strokeDashArray: 4,
                        yaxis: {
                            lines: {
                                show: true
                            }
                        }
                    },
                    noData: {
                        text: `${localizer.noDataTxt}`,
                        align: "center",
                        verticalAlign: "middle",
                    },
                };
            }

            var options_1 = getChartOptions(labels_minute, min_data, `${localizer.minutelyViews}`);
            var options_2 = getChartOptions(labels_hour, hour_data, `${localizer.hourlyViews}`);
            var options_3 = getChartOptions(labels_day, day_data, `${localizer.dailyViews}`);

            var chart1 = new ApexCharts(element1, options_1);
            var chart2 = new ApexCharts(element2, options_2);
            var chart3 = new ApexCharts(element3, options_3);
            chart1.render();
            chart2.render();
            chart3.render();

            analyticsCardBlockUI.release();
        }
    });
})
