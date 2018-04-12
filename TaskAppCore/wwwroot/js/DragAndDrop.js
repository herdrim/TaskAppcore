// Działa tylko dla div-ów o id: div1, div2, div3, div4 oraz dla url /Home/Index

function allowDrop(ev) {
    if (ev.target.id == "div1" || ev.target.id == "div2" || ev.target.id == "div3" || ev.target.id == "div4")
        ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text/TaskId", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var divId = ev.target.id;
    var id = ev.dataTransfer.getData("text/TaskId");

    var taskData = {};
    taskData.TaskId = id;

    if (divId == "div1")
        taskData.IsToDo = true;
    else
        taskData.IsToDo = false;
    if (divId == "div2")
        taskData.IsInProgress = true;
    else
        taskData.IsInProgress = false;
    if (divId == "div3")
        taskData.IsTested = true;
    else
        taskData.IsTested = false;
    if (divId == "div4")
        taskData.IsFinished = true;
    else
        taskData.IsFinished = false;

    document.getElementById(divId).appendChild(document.getElementById(id))
    UpdateTask(taskData);
}

function UpdateTask(data) {
    $.ajax({
        type: "POST",
        url: '/Home/Index',
        data: { "json": JSON.stringify(data) },
        dataType: "json",
        success: function () {
            alert("Data has been updated successfully");
        },
        error: function () {
            alert("Error while updating data");
        }
    });
}