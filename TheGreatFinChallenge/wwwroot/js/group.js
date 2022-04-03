function checkDataFormat(i) {
    console.log(i)
    var els = document.getElementsByClassName("groupPanel")[i].getElementsByClassName("lineChartHeaders");
    for (i = 0; i < els.length; i++) {
        var el = els[i]
        if (el.classList.contains("d-flex") && i == 3) {
            _departmentDateCallback = true;
            break;
        } else if (el.classList.contains("d-flex")) {
            _departmentDateCallback = false;
            break;
        };
    }
}

function changeDataOfDepartment(i, n, parent) {
    console.log(parent);
    var elements = $(`#${parent} .lineChartHeaders`);

    elements[i].classList.remove("d-flex");
    elements[i].classList.add("d-none");

    elements[n].classList.remove("d-none");
    elements[n].classList.add("d-flex");

    var departmentId = parent.split("_")[1];
    const myLineChart = chartsVariable[`lineChart_${departmentId}`];
    var maxNum = Math.max.apply(null, lineChartData[departmentId][n]) + 1
    var minNum = Math.min(maxNum, 5);

    myLineChart.data.datasets[0].data = lineChartData[departmentId][n];
    myLineChart.options.scales["yAxes"][0].ticks.maxTicksLimit = minNum;

    _departmentDateCallback = false;
    label = "Activities"
    if (n == 1) label = "Calories";
    else if (n == 2) label = "Distance (km)";
    else if (n == 3) {
        label = "Duration";
        _departmentDateCallback = true;
    }

    myLineChart.data.datasets[0].label = label;
    myLineChart.update()
}

function changeDataOfUserRanking(i, n) {
    var elements = $(`.UserRankingChartHeaders`);
    elements[i].classList.remove("d-flex");
    elements[i].classList.add("d-none");
    elements[n].classList.remove("d-none");
    elements[n].classList.add("d-flex");

    console.log(i, n)
    document.getElementById(`userRankingTables_${i}`).classList.add('d-none');
    document.getElementById(`userRankingTables_${n}`).classList.remove('d-none');

    _userDateCallback = false;
    if (n == 3) _userDateCallback = true;

    const myLineChart = chartsVariable[`userRankChart`];
    myLineChart.data.datasets = userRankingChartData[n];
    myLineChart.update()
}

function changeDataOfGroupRanking(i, n) {
    var elements = $(`.GroupRankingChartHeaders`);
    elements[i].classList.remove("d-flex");
    elements[i].classList.add("d-none");
    elements[n].classList.remove("d-none");
    elements[n].classList.add("d-flex");

    console.log(i, n)
    document.getElementById(`groupRankingTables_${i}`).classList.add('d-none');
    document.getElementById(`groupRankingTables_${n}`).classList.remove('d-none');
    _groupDateCallback = false;
    if (n == 3) _groupDateCallback = true;

    const myLineChart = chartsVariable[`groupRankChart`];
    myLineChart.data.datasets = groupRankingChartData[n];
    myLineChart.update()
}
