let userAgent = navigator.userAgent;
console.log(userAgent)
if (userAgent.match(/firefox|fxios/i) || userAgent.match(/opr\//i) || userAgent.match(/edg/i)){
    console.log("Browser not supported yet. This app was developed for chrome/firefox browsers")
    $("#modalWarningBrowser").modal({ keyboard: true })
}

else if ($(document).width() < 992 || $(document).height() < 500) {
    console.log("Screenformat not supported yet. This app was developed for laptop/desktop environments")
    $("#modalWarningScreenFormat").modal({ keyboard: true })
}

