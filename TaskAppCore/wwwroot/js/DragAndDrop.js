// Działa tylko dla div-ów o id: div1, div2, div3, div4 oraz dla url /Home/Index

function allowDrop(ev) {
    if (ev.target.id === "div1" || ev.target.id === "div2" || ev.target.id === "div3" || ev.target.id === "div4")
        ev.preventDefault();
}

function drag(ev) {
    var child = ev.target.childNodes;
    ev.dataTransfer.setData("text/TaskId", child[1].id);
}

function drop(ev) {
    ev.preventDefault();
    var divId = ev.target.id;
    var id = ev.dataTransfer.getData("text/TaskId");

    var taskData = {};
    taskData.TaskId = id;

    if (divId === "div1")
        taskData.IsToDo = true;
    else
        taskData.IsToDo = false;
    if (divId === "div2")
        taskData.IsInProgress = true;
    else
        taskData.IsInProgress = false;
    if (divId === "div3")
        taskData.IsTested = true;
    else
        taskData.IsTested = false;
    if (divId === "div4")
        taskData.IsFinished = true;
    else
        taskData.IsFinished = false;

    document.getElementById(divId).appendChild(document.getElementById(id).parentElement);
    ChangeTaskStatus(taskData);
}

function ChangeTaskStatus(data) {
    $.ajax({
        type: "POST",
        url: '/Home/UpdateStatus',
        data: { "json": JSON.stringify(data) },
        dataType: "json",
        success: function (data) {
            if (!data.isValid) {
                alert(data.message);
                location.reload();
            }
        },
        error: function () {
            alert("Error while updating data");
        }
    });
}

function OnAssignClick(obj) {
    var divId = obj.parentNode.parentNode.id;
    var pElem = obj.parentNode;
    var taskId = obj.id;
    pElem.setAttribute("draggable", "true");
    pElem.setAttribute("ondragstart", "drag(event)");
    obj.setAttribute("class", "btn btn-danger");
    obj.setAttribute("onclick", "OnResignClick(this)");
    obj.innerHTML = "Resign";



    if (divId === "IsToDo")
        document.getElementById("div1").appendChild(pElem);
    else if (divId === "IsInProgress")
        document.getElementById("div2").appendChild(pElem);
    else if (divId === "IsTested")
        document.getElementById("div3").appendChild(pElem);
    else if (divId === "IsFinished")
        document.getElementById("div4").appendChild(pElem);

    AssignTaskToUser(taskId);

    document.getElementById(divId).removeChild(pElem);   
}

function AssignTaskToUser(taskId) {
    $.ajax({
        type: "POST",
        url: '/Home/Assign',
        data: { taskId: taskId },
        dataType: "json",
        success: function (data) {
            if (!data.isValid) {
                alert(data.message);
                location.reload();
            }
        },
        error: function () {
            alert("Error while updating data");
        }
    });
}



function OnResignClick(obj) {
    var divId = obj.parentNode.parentNode.id;
    var pElem = obj.parentNode;
    var taskId = obj.id;
    pElem.removeAttribute("draggable");
    pElem.removeAttribute("ondragstart");
    obj.setAttribute("class", "btn btn-success");
    obj.setAttribute("onclick", "OnAssignClick(this)");
    obj.innerHTML = "Asign";


    if (divId === "div1")
        document.getElementById("IsToDo").appendChild(pElem);
    else if (divId === "div2")
        document.getElementById("IsInProgress").appendChild(pElem);
    else if (divId === "div3")
        document.getElementById("IsTested").appendChild(pElem);
    else if (divId === "div4")
        document.getElementById("IsFinished").appendChild(pElem);

    CancelTask(taskId);

    document.getElementById(divId).removeChild(pElem);
}

function CancelTask(taskId) {
    $.ajax({
        type: "POST",
        url: '/Home/CancelTask',
        data: { taskId: taskId },
        dataType: "json",
        success: function (data) {
            if (!data.isValid) {
                alert(data.message);
                location.reload();
            }
        },
        error: function () {
            alert("Error while updating data");
        }
    });
}