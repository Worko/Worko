

$(document).ready(function () {
   // autoUpdate(updateScheduleConstrains);
});


var id = null;

function autoUpdate(func) {
    if (_autoUpdateRun == true) {
        $.ajaxSetup({ cache: false }); // This part addresses an IE bug.  without it, IE will only load the first number and will never refresh
        func();
        id = setTimeout(function () {
            autoUpdate(func);
        }, 3000); // the "3000" here refers to the time to refresh the div.  it is in milliseconds. 
    }
    else {
        clearTimeout(id);
    }
}

function updateScheduleConstrains() {
    var wsid = $('#WSID').val();

    $.ajax({
        method: "POST",
        url: '/ShiftsManager/GetScheduleConstrains',
        data: {
            wsid: parseInt(wsid)
        },
        dataType: "json",
        success: function (data) {
            $('#schedule-constrains .panel-body').html(data.html);
        }
    });
}