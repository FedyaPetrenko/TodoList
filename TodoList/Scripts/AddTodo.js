$(document).ready(

    function () {
        $.ajax({
            url: "/ToDoTasks/BuildToDoTasksTable",
            success: function(result) {
                $("#tableDiv").html(result);
            }
        });
    })