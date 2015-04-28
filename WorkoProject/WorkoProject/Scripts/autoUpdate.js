

$(document).ready(function () {
    autoUpdate(updateRequestsAlert);
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

function updateRequestsAlert() {
    $.ajax({
        method: "POST",
        url: '/Requests/GetUnreadWorkersRequests',
        data: {},
        dataType: "json",
        success: function (data) {
            var dot = $('.updateDot-alerts');
            var rows = $('.navbar-top-links .dropdown-alerts');
            rows.empty();

            if (data.requests.length > 0) {
                // show the red dot
                dot.css("display", "block");
                
                for (var i=0; i < data.requests.length; i++)
                {
                    var request = data.requests[i];
                    var worker = $.grep(data.workers, function (e) {
                        return e.IdNumber === request.WorkerId;
                    })[0];

                    var date = new Date(request.Date.match(/\d+/)[0] *1);
                    date = date.toLocaleDateString().replace(/\./g, "/");
                    var txt = "<li><a href=\"/Requests/WorkersRequestsList\"><div><span class=\"pull-left text-muted small\">" + date + "</span><i class=\"fa fa-comment fa-fw\"></i> בקשה חדשה<p><span class=\"request-from pull-right text-muted large\">מאת: <span class=\"request-from-name text-muted large\">" + worker.FirstName + ' ' + worker.LastName + "</span></span></p></div></a></li><li class=\"divider\"></li>";
                    rows.append(txt);
                }
                var txt = "<li><a class=\"text-center all-requests\" href=\"/Requests/WorkersRequestsList\"><strong>צפה בכל ההתראות</strong><i class=\"fa fa-angle-left\"></i></a></li>";
                rows.append(txt);
            }

            else
            {
                dot.css("display", "none");
                var txt = "<li><div class=\"no-alerts\">אין התראות חדשות.</div></li>";
                rows.append(txt);
            }

        }
    });
}
