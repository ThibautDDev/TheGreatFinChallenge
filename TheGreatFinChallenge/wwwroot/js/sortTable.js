function isNumeric(str) {
    str = str.replace(',', '.')
    if (typeof str != "string") return false // we only process strings!  
    return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
        !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
}

function isDate(str) {
    return str.includes("/") && str.includes(":")
}

function sortTable(n, objectName) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById(objectName)
    switching = true;
    // Set the sorting direction to ascending:
    dir = "asc";
    /* Make a loop that will continue until
    no switching has been done: */
    while (switching) {
        // Start by saying: no switching is done:
        switching = false;
        rows = table.rows;
        /* Loop through all table rows (except the
        first, which contains table headers): */
        for (i = 1; i < (rows.length - 1); i++) {
            // Start by saying there should be no switching:
            shouldSwitch = false;
            /* Get the two elements you want to compare,
            one from current row and one from the next: */
            x = rows[i].getElementsByTagName("TD")[n];
            y = rows[i + 1].getElementsByTagName("TD")[n];
            /* Check if the two rows should switch place,
            based on the direction, asc or desc: */
            if (dir == "asc") {
                if (isNumeric(x.innerHTML) && isNumeric(y.innerHTML)) {
                    console.log(parseInt(x.innerHTML), parseInt(y.innerHTML))
                    if (parseInt(x.innerHTML) > parseInt(y.innerHTML)) {
                        // If so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }

                } else if (isDate(x.innerHTML) || isDate(y.innerHTML)) {
                    var a = x.innerHTML.split(" - ")
                    var b = y.innerHTML.split(" - ")

                    var date1 = new Date(a[0].split("/")[2], a[0].split("/")[1], a[0].split("/")[0], a[1].split(":")[0], a[1].split(":")[1])
                    var date2 = new Date(b[0].split("/")[2], b[0].split("/")[1], b[0].split("/")[0], b[1].split(":")[0], b[1].split(":")[1])
                    if (date1 > date2) {
                        // If so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }

                } else if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == "desc") {
                if (isNumeric(x.innerHTML) && isNumeric(y.innerHTML)) {
                    if (parseInt(x.innerHTML) < parseInt(y.innerHTML)) {
                        // If so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }

                } else if (isDate(x.innerHTML) || isDate(y.innerHTML)) {
                    var a = x.innerHTML.split(" - ")
                    var b = y.innerHTML.split(" - ")

                    var date1 = new Date(a[0].split("/")[2], a[0].split("/")[1], a[0].split("/")[0], a[1].split(":")[0], a[1].split(":")[1])
                    var date2 = new Date(b[0].split("/")[2], b[0].split("/")[1], b[0].split("/")[0], b[1].split(":")[0], b[1].split(":")[1])
                    if (date1 < date2) {
                        // If so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }

                }  else if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            /* If a switch has been marked, make the switch
            and mark that a switch has been done: */
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            // Each time a switch is done, increase this count by 1:
            switchcount++;
        } else {
            /* If no switching has been done AND the direction is "asc",
            set the direction to "desc" and run the while loop again. */
            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}