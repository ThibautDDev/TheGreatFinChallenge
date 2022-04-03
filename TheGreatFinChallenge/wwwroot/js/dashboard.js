function changeFitnessCard1(i, n) {
    // This function will display a certain card with information on press.
    var x = document.getElementsByClassName("fitnessCards1");    //Get all elements with fitnessCards as className
    var htmlCur = x[i]
    var htmlNext = x[n]

    // Change displaysettings between current and next card
    htmlNext.classList.remove("d-none")
    htmlCur.classList.add("d-none")
}

function changeFitnessCard2(i, n) {
    // This function will display a certain card with information on press.
    var x = document.getElementsByClassName("fitnessCards2");    //Get all elements with fitnessCards as className
    var htmlCur = x[i]
    var htmlNext = x[n]

    // Change displaysettings between current and next card
    htmlNext.classList.remove("d-none")
    htmlCur.classList.add("d-none")
}
//}

//function changeRankingCard(i, n) {
//    // This function will display a certain card with information on press.
//    var x = document.getElementsByClassName("rankingCards");    //Get all elements with rankingCards as className
//    var htmlCur = x[i]
//    var htmlNext = x[n]

//    // Change displaysettings between current and next card
//    htmlNext.classList.remove("d-none")
//    htmlCur.classList.add("d-none")
//}


function changeData(i, n) {
    // This function will display a certain chart with information on press.
    var x = document.getElementsByClassName("lineChartHeaders");    //Get all headers with lineChartHeaders as className
    var htmlCur = x[i]
    var htmlNext = x[n]

    // Change displaysettings between current and next card
    htmlNext.classList.remove("d-none")
    htmlNext.classList.add("d-flex")

    htmlCur.classList.remove("d-flex")
    htmlCur.classList.add("d-none")
    //console.log(i, n)
    //console.log(window[`data${name}`])

    dsplName = disciplines[i]
    //console.log(dsplName)

    // Update chart data with smooth transition and dynamic data based on the n
    var maxNum = Math.max.apply(null, lineChartData[dsplName]) + 1
    var minNum = Math.min(maxNum, 5);
    //console.log(maxNum, minNum)
    myLineChart.data.datasets[0].data = lineChartData[dsplName];
    myLineChart.options.scales["yAxes"][0].ticks.maxTicksLimit = minNum
    myLineChart.update()
}


//function changeData(i, n, name) {
//    // This function will display a certain chart with information on press.
//    var x = document.getElementsByClassName("lineChartHeaders");    //Get all headers with lineChartHeaders as className
//    var htmlCur = x[i-1]
//    var htmlNext = x[n-1]

//    // Change displaysettings between current and next card
//    htmlNext.classList.remove("d-none")
//    htmlNext.classList.add("d-flex")

//    htmlCur.classList.remove("d-flex")
//    htmlCur.classList.add("d-none")
//    //console.log(i, n)
//    //console.log(window[`data${name}`])

//    // Update chart data with smooth transition and dynamic data based on the n
//    myLineChart.data.datasets[0].data = window[`data${name}`]
//    myLineChart.options.scales["yAxes"][0].ticks.maxTicksLimit = Math.max.apply(null, window[`data${name}`]) + 1
//    myLineChart.update()
//}