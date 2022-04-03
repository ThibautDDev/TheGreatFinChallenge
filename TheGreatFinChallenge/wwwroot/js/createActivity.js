var currentTab = 0; // Current tab is set to be the first tab (0)
var typeActivity = "";
var timeInMinutes = 0;
showTab(currentTab); // Display the current tab

function showTab(n) {
    // This function will display the specified tab of the form ...
    var x = document.getElementsByClassName("tab");
    x[n].style.display = "block";
    // ... and fix the Previous/Next buttons:
    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }
    if (n == (x.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Submit";
    } else {
        document.getElementById("nextBtn").innerHTML = "Next";
    }
    // ... and run a function that displays the correct step indicator:
    fixStepIndicator(n)
}

function nextPrev(n) {
    // This function will figure out which tab to display
    var x = document.getElementsByClassName("tab");
    // Exit the function if any field in the current tab is invalid:
    if (n == 1 && !validateForm()) return false;
    // Hide the current tab:
    x[currentTab].style.display = "none";
    // Increase or decrease the current tab by 1:
    currentTab = currentTab + n;
    // if you have reached the end of the form... :
    if (currentTab >= x.length) {
        //...the form gets submitted:
        document.getElementById("regForm").submit();
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}

function validateForm() {
    // This function deals with validation of the form fields
    var x, y, i, valid = true;
    x = document.getElementsByClassName("tab");
    y = x[currentTab].getElementsByTagName("input");
    radioBtns = false
    radioChecked = false;
    // A loop that checks every input field in the current tab:
    for (i = 0; i < y.length; i++) {
        if (y[i].type == "radio") {
            radioBtns = true;
            if (y[i].checked) {
                typeActivity = y[i].value;
                radioChecked = true;
            }
        }
        // If a field is empty...
        if (y[i].value == "" && y[i].id != "gear" && y[i].id != "calories") {
            // add an "invalid" class to the field:
            y[i].className += " is-invalid";
            // and set the current valid status to false:
            valid = false;
        } else {
            y[i].className = "form-control";
        }

        if (y[i].id == "totalTime") {
            var time = y[i].value.split(":")
            timeInMinutes = (parseInt(time[0]) * 60) + parseInt(time[1]) + (parseInt(time[2]) / 60)
            console.log(time, timeInMinutes)
        }

        console.log(y[i])
        //if (y[i].id == "calories" && y[i].value == "") {
        //    if (typeActivity == "Cycling") MET = 12;
        //    else if (typeActivity == "Running") MET = 10;
        //    else if (typeActivity == "Hiking") MET = 5;
        //    else MET = 5.8;

        //    var calories = ((MET * 65 * 3.5) / 200) * timeInMinutes
        //    y[i].value = Math.round(calories)
        //    console.log(typeActivity, timeInMinutes, calories)
        //    valid = false;
        //}
    }
    if (radioBtns && !radioChecked) {
        valid = false;
    }


    // If the valid status is true, mark the step as finished and valid:
    if (valid) {
        document.getElementsByClassName("step")[currentTab].className += " finish";
    }
    return valid; // return the valid status
}

function calculateCalories() {
    var typeActivity = "Cycling";
    if (document.getElementById("running").checked) typeActivity = "Running"
    if (document.getElementById("hiking").checked) typeActivity = "Hiking"
    if (document.getElementById("swimming").checked) typeActivity = "Swimming"

    var time = document.getElementById("totalTime").value.split(":")
    var hours = (time[0] == "NaN" || time[0] == undefined) ? "0" : time[0];
    var minutes = (time[1] == "NaN" || time[1] == undefined) ? "0" : time[1];
    var seconds = (time[2] == "NaN" || time[2] == undefined) ? "0" : time[2];
    console.log(hours, minutes, seconds)
    var timeInMinutes = (parseInt(hours) * 60) + parseInt(minutes) + (parseInt(seconds) / 60)


    if (typeActivity == "Cycling") MET = 8;
    else if (typeActivity == "Running") MET = 9;
    else if (typeActivity == "Hiking") MET = 3;
    else MET = 6;

    var calories = ((MET * 65 * 3.5) / 200) * timeInMinutes
    document.getElementById("calories").value = Math.round(calories)
    console.log(typeActivity, timeInMinutes, calories)
}


function fixStepIndicator(n) {
    // This function removes the "active" class of all steps...
    var i, x = document.getElementsByClassName("step");
    for (i = 0; i < x.length; i++) {
        x[i].className = x[i].className.replace(" active", "");
    }
    //... and adds the "active" class to the current step:
    x[n].className += " active";
}