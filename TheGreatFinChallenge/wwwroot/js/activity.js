////function setActivityId(e, id) {
////    var x = document.getElementById("editActivityId");
////    x.value = id;

////    var parent = e.parentElement;
////    var columns = parent.cells;
////    //console.log(columns);
////    var type = "Normal";
////    if (columns.length == 8) type = "All";

////    var body = document.getElementById("editActivityBody");
////    body.innerHTML = "";

////    for (i = 0; i < columns.length - 1; i++) {
////        var header = tableHeadersDictionairy[type][i];
////        console.log(i, header);
////        //console.log(columns[i]);
////        if (header != "Type" && header != "Name") {
////            var group = document.createElement("div");
////            group.classList += ["form-group"];

////            var label = document.createElement("label");
////            label.innerHTML = header;

////            var input = document.createElement("input");
////            input.classList += ["form-control"];
////            input.required = true;
////            input.name = header;
////            input.value = columns[i].innerHTML;

////            if (header == "Date") {
////                input.type = "datetime-local";
////                var date = columns[i].innerHTML.split(" ")[0].split("/");
////                var time = columns[i].innerHTML.split(" ")[1];
////                input.value = `${date[2]}-${date[1]}-${date[0]}T${time}`;

////            } else if (header == "Duration") {
////                input.type = "time";
////                input.step = 1;
////            } else if (header == "Calories" || header == "Distance") {
////                input.type = "number";
////                input.min = 1;
////                input.step = 0.01;
////            }

////            if (header == "Calories" || header == "Gear") input.required = false;

////            group.appendChild(label);
////            group.appendChild(input);

////            body.append(group);
////            //console.log(header);
////        }
////        console.log(body);
////    }


////    var x = document.getElementById("deleteActivityId");
////    x.value = id;
////    //console.log(e)
////}