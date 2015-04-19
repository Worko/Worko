var userPic;
var _cuCookie = 'worko_stay_connected';
var _sli = null;
var _hasChanged = false;


/* Page loading */
$(window).load(function () {
    // Animate loader off screen
    $(".se-pre-con").fadeOut("slow");;
});

$(function () {
    /* Initialization */
    $('[data-tooltip="tooltip"]').tooltip()
    /* Initialization */

    /* Register */
    // Validate picture type
    $('#file').change(function () {
        var output = document.getElementById('output');
        var file = document.getElementById('file').value;
        if (file.match(/^.*?\.(jpeg|jpg|png|gif|bmp)$/)) {
            output.src = URL.createObjectURL(event.target.files[0]);
        }
        else {
            $('#imgErr').modal('show');
            //alert("(jpeg,jpg,png,gif,bmp) :אנא בחר קובץ מסוג תמונה");
            output.src = "";
            document.getElementById('file').value = "";
        }
    });

    $("#resetForm").click(function () {
        resetForm($("#register-form"));
    });
    /* Register */

    /* Login */
    $('#logoff').click(function () {
        eraseCookie(_cuCookie);
        window.location.href = $(this).data('href');
    });

    if (_sli && _sli.stay && _sli.id) {
        createCookie(_cuCookie, _sli.id, 365);
    }

    if (window.location.pathname.toLowerCase().search('login') != -1) {
        var cookie = readCookie(_cuCookie);
        if (cookie) {
            $.ajax({
                method: "POST",
                url: 'AutoLogin',
                //contentType: false,
                //dataType: "json",
                data: {
                    workerId: cookie
                },
                success: function (data) {
                    if (data.Message == 'success' && data.Url) {
                        window.location.href = data.Url;
                    }
                }
            });
        }
    }
    /* Login */
    
    /* Error messages */
    var err = $('#error');
    if (err) {
        err.modal({ show: true });
    }
    /* Error messages */

    /* Stations */
    $('#add-station').click(function () {
        $('#add-station-modal').modal('show');
    });

    $('.search #search-station').keyup(function () {
        searchTable($(this).val());
    });

    $('.search button').click(function () {
        searchTable($('.search #search-station').val());
    });

    /* Workers */
    $('#add-worker').click(function () {
        $('#add-worker-modal').modal('show');
    });

    $('.search #search-worker').keyup(function () {
        searchTable($(this).val());
    });

    $('.search button').click(function () {
        searchTable($('.search #search-worker').val());
    });

    $('#userFilePic').change(function () {
        var output = $('.changePic');
        var file = document.getElementById('userFilePic').value;
        if (file.match(/^.*?\.(jpeg|jpg|png|gif|bmp)$/)) {
            _workerDetails.pic = URL.createObjectURL(event.target.files[0]);
            output.attr('src', URL.createObjectURL(event.target.files[0]));
        }
        else {
            $('#imgErr').modal('show');
            //alert("(jpeg,jpg,png,gif,bmp) :אנא בחר קובץ מסוג תמונה");
            output.src = "";
            document.getElementById('file').value = "";
        }
    });

    /* Constrains Submission */
    $('#shifts-constrains .shift.clickable').click(function () {
        var v = $(this).find('i.fa');
        var hidden = $(this).find('.ischecked');
        var wrong = $('.wrong');

        wrong.addClass('hide');

        if (hidden.val().toLowerCase() == "false") {
            var checkCount = $('#shifts-constrains .shift .fa-times').length;

            if (checkCount < 2) {
                v.addClass('fa-times');
                hidden.val("True");
            }
            else {
                wrong.removeClass('hide');
            }
        }
        else {
            v.removeClass('fa-times');
            hidden.val("False");
        }
        
    });
});


/* ScheduleConstrains */

$('#schedule-constrains .shift.clickable').click(function () {
    var v = $(this).find('i.fa');
    var hidden = $(this).find('.ischecked');
    var url;

    if (v.hasClass('fa-times')) {
        v.removeClass('fa-times');
        hidden.val("False");
        url = '/ShiftsManager/RemoveSchduleConstrain';
    } else {
        v.addClass('fa-times');
        hidden.val("True");
        url = '/ShiftsManager/AddSchduleConstrain';
    }

    var wsid = $('#WSID').val();
    var day = $(this).data('day');
    var shiftTime = $(this).data('shift');
    var stationId = $(this).parents('tr').data('stationid');

    $.ajax({
        method: "POST",
        url: url,
        data: {
            wsid: wsid,
            day: day,
            shiftTime: shiftTime,
            stationId: stationId
        },
        success: function (data) {
        }
    });

});



/* functions */
/* search tables */
function searchTable(key) {
    var table = $('.search-table');

    if (key != null && key != "") {
        table.find("tbody tr").addClass('hide');
        var rows = table[0].children[1].children;
        var matchW = new RegExp(key, 'gi');
        $(rows).each(function () {
            var cols = $(this).find('.field');
            for (var i = 0; i < cols.length; i++) {
                if ($(this).hasClass('hide') && cols[i].innerText.search(matchW) > -1) {
                    $(this).removeClass('hide');
                }
            }
        });
    } else {
        table.find("tbody tr").removeClass('hide');
    }
}

/* workers*/
var _workerDetails = {};
$(document).on('click', '.changePic', function () {
    $('#userFilePic').click();
});


$(document).on('click', '#workers-list .edit', function () {
    var wFname, wLname, wPhone, wEmail, msg = null;

    // if clicking on other 'edit' button, close the open editing row and update the station.
    // open the clicked row to edit
    var editingRow = $('.editing');
    var parent = $(this).parents('tr');

    if (editingRow.length > 0 && editingRow[0] != parent[0]) {
        msg = updateWorker(editingRow);
    }
    

    if (parent.hasClass('editing')) {
        msg = updateWorker(parent);
    } else {
        // convert row to edit mode
        parent.addClass('editing');
        userPic = parent.find('.worker-pic img').attr('src');
        _workerDetails.pic = userPic;
        wFname = parent.find('.worker-fname').text();
        wLname = parent.find('.worker-lname').text();
        wPhone = parent.find('.worker-phone').text();
        wEmail = parent.find('.worker-email').text();

        parent.find('.worker-pic img').addClass('changePic');
        parent.find('.worker-fname').html('<input id="wFname" type="text" class="form-control" placeholder="הזן שם פרטי" value="' + wFname + '" />');
        parent.find('.worker-lname').html('<input id="wLname" type="text" class="form-control" placeholder="הזן שם משפחה" value="' + wLname + '" />');
        parent.find('.worker-phone').html('<input id="wPhone" type="text" class="form-control" placeholder="הזן מספר טלפון" value="' + wPhone + '" />');
        parent.find('.worker-email').html('<input id="wEmail" type="text" class="form-control" placeholder="הזן כתובת מייל" value="' + wEmail + '" />');

        parent.find('.edit').html('אישור');

        _workerDetails.fname = wFname;
        _workerDetails.lname = wLname;
        _workerDetails.phone = wPhone;
        _workerDetails.email = wEmail;
    }

    if (msg != null) {
        // problem while trying to update the station
        // TODO: display message
    }
});

function updateWorker(worker) {
    var wFame, wLname, wPhone, wEmail, msg = null, wId;

    var data = new FormData();
    var files;
    try {
        files = $("#userFilePic").get(0).files;

        // Add the uploaded image content to the form data collection
        if (files && files.length > 0) {
            data.append("userPic", files[0]);
        }
    }
    catch (e) { }
    
    wId = worker.data('id');
    wFname = worker.find('#wFname').val();
    wLname = worker.find('#wLname').val();
    wPhone = worker.find('#wPhone').val();
    wEmail = worker.find('#wEmail').val();

    data.append("id", String(wId));
    data.append("fname", wFname);
    data.append("lname", wLname);
    data.append("phone", wPhone);
    data.append("email", wEmail);

    if (_workerDetails.pic != userPic || _workerDetails.fname != wFname || _workerDetails.lname != wLname || _workerDetails.phone != wPhone || _workerDetails.email != wEmail) {
        $.ajax({
            method: "POST",
            url: 'UpdateWorker',
            contentType: false,
            processData: false,
            dataType: "json",
            data: data,
            success: function (data) {
                $('body').append(data.html);
                $('#update-worker-modal').modal('show');
            }
        });
    }

    worker.find('.worker-pic img').removeClass('changePic');
    worker.find('.worker-fname').html(wFname);
    worker.find('.worker-lname').html(wLname);
    worker.find('.worker-phone').html(wPhone);
    worker.find('.worker-email').html(wEmail);
    worker.find('.edit').html('ערוך');
    worker.removeClass('editing');

    return msg;
}

function addWorkerSuccess(data) {
    var message = $(this).find('.modal-footer .message');
    message.addClass(data.state);
    message.html(data.message);
    if (data.state != 'error') {
        _hasChanged = true;
    }
}


$(document).on('click', '#workers-list .delete', function () {
    var workerToRemove = $(this).parents('tr');
    var workerId = workerToRemove.data('id');

    $.ajax({
        method: "POST",
        url: 'DeleteWorker',
        data: {
            workerId: workerId.toString()
        },
        success: function (data) {
            workerToRemove.remove();
            $('body').append(data.html);
            $('#delete-worker-modal').modal('show');
            
        }
    });
});


/* /workers*/




/* stations */
var _stationDetails = { };


function addStationSuccess(data) {
    var message = $(this).find('.modal-footer .message');
    message.addClass(data.state);
    message.html(data.message);
    if (data.state != 'error') {
        _hasChanged = true;
    }
}

// happened after closing the modal
$('#add-station-modal').on('hidden.bs.modal', function () {
    if (_hasChanged) {
        $.ajax({
            method: "POST",
            url: 'Stations/GetStations',
            dataType: "json",
            data: {},
            success: function (data) {
                $('#Stations-list .panel-body').html(data.html);
                $('.message').text('');
                _hasChanged = false;
            }
        });
    }

    resetForm($('#add-station-modal form'));
})

$('#worker-station-modal').on('hidden.bs.modal', function () {
    resetForm($('#worker-station-modal form'));
});

$(document).on('hidden.bs.modal', '#update-station-modal', function () {
    $('#update-station-modal').remove();
});

$(document).on('click', '#Stations-list .edit', function () {
    var sName, sDesc, sStatus, sStatusVal, msg = null;
    
    // if clicking on other 'edit' button, close the open editing row and update the station.
    // open the clicked row to edit
    var editingRow = $('.editing');
    var parent = $(this).parents('tr');

    if (editingRow.length > 0 && editingRow[0] != parent[0]) {
        msg = updateStation(editingRow);
    }
    
    if (parent.hasClass('editing')) {
        msg = updateStation(parent);
    } else {
        // convert row to edit mode
        parent.addClass('editing');
        sName = parent.find('.station-name').text();
        sDesc = parent.find('.station-desc').text();
        sStatus = parent.find('.station-status').text();

        parent.find('.station-name').html('<input id="sName" type="text" class="form-control" placeholder="הזן שם עמדה" value="' + sName + '" />');
        parent.find('.station-desc').html('<textarea  id="sDesc" class="form-control" placeholder="הזן תיאור עמדה" rows="2">' + sDesc + '</textarea>');
        
        options = [ { text: 'עמדה פעילה', value: "1" },
                    { text: 'עמדה לא פעילה', value: "2" },
                    { text: 'בטיפול', value: "3" }];
        
        parent.find('.station-status').html(getDropDown(options, 'sStatus', 'form-control', sStatus));
        parent.find('.edit').html('אישור');
        $('.link-worker, .related-workers').addClass('disabled');

        _stationDetails.name = sName;
        _stationDetails.desc = sDesc;
        _stationDetails.status = sStatus;
    }

    if (msg != null) {
        // problem while trying to update the station
        // TODO: display message
    }
});


$(document).on('click', '#Stations-list .related-workers', function () {
    var stationID = $(this).parents('tr').data('id');

    $('#related-worker-station-modal').attr('data-sid', stationID);
    $('#related-worker-station-modal .error-message').html('');

    $('#related-worker-station-modal #stationName').html($(this).parents('tr').find('.station-name').html());

    $.ajax({
        method: "POST",
        url: 'Stations/GetRelatedWorkers',
        dataType: "json",
        data: {
            stationID: stationID
        },
        success: function (data) {
            var rows = $('#related-worker-station-modal table tbody');
            rows.empty();
            for (var i = 0; i < data.workers.length; i++) {
                var worker = data.workers[i];
                var t1 = "<tr data-id=" + worker.IdNumber + ">";
                var t2 = "<td class=\"worker-pic\"><img src=\"" + (worker.Picture == "" ? "Content/img/default_user.png" : worker.Picture) + "\" alt\=\"\" width=\"30\" height=\"30\" /></td>";
                var t3 = "<td class=\"worker-fname field\">" + worker.FirstName + "</td>";
                var t4 = "<td class=\"worker-lname field\">" + worker.LastName + "</td>";
                var t5 = "<td class=\"worker-id field\">" + worker.IdNumber + "</td>";
                var t6 = "</tr>";

                var txt = t1 + t2 + t3 + t4 + t5 + t6;
                rows.append(txt);
            }
        }
    });

    $('#related-worker-station-modal').modal('show');
});

$(document).on('click', '#Stations-list .link-worker', function () {
    var stationID = $(this).parents('tr').data('id');
    $('#worker-station-modal').attr('data-sid', stationID);

    $('#worker-station-modal .error-message').html('');
    $('#stationName').html($(this).parents('tr').find('.station-name').html());

    $.ajax({
        method: "POST",
        url: 'Stations/GetWorkersByStationId',
        dataType: "json",
        data: {
            stationID: stationID
        },
        success: function (data) {
            var rows = $('#worker-station-modal table tbody tr');
            rows.find('.linkWorker').removeClass('disabled');
            rows.find('.unlinkWorker').addClass('disabled');

            rows.each(function () {
                var row = $(this);
                for (var i = 0; i < data.workers.length; i++) {
                    if (row.data('id') == data.workers[i]) {
                        row.find('.linkWorker').addClass('disabled');
                        row.find('.unlinkWorker').removeClass('disabled');
                    }
                }
            });
        }
    });

    $('#worker-station-modal').modal('show');
});


/* Link worker to station */
$(document).on('click', '.linkWorker', function () {
    var btn = $(this);
    var workerID = btn.parents('tr').data('id');
    var stationID = $('#worker-station-modal').attr('data-sid');

    $.ajax({
        method: "POST",
        url: 'Stations/LinkWorkerToStation',
        dataType: "json",
        data: {
            workerID: workerID,
            stationID: stationID
        },
        success: function (data) {
            if (data.msg == 'success')
            {
                btn.parents('tr').find('.unlinkWorker').removeClass('disabled');
                btn.addClass('disabled');
            }
            else
            {
                $('#worker-station-modal .error-message').html('אירעה שגיאה');
            }
        }
    });
});

$(document).on('click', '.unlinkWorker', function () {
    var btn = $(this);
    var workerID = btn.parents('tr').data('id');
    var stationID = $('#worker-station-modal').attr('data-sid');


    $.ajax({
        method: "POST",
        url: 'Stations/UnLinkWorkerToStation',
        dataType: "json",
        data: {
            workerID: workerID,
            stationID: stationID
        },
        success: function (data) {
            if (data.msg == 'success') {
                btn.parents('tr').find('.linkWorker').removeClass('disabled');
                btn.addClass('disabled');
            }
            else {
                $('#worker-station-modal .error-message').html('אירעה שגיאה');
            }
        }
    });
});



function updateStation(station) {
    var sName, sDesc, sStatus, sStatusVal, msg = null, sId;

    sId = station.data('id');
    sName = station.find('#sName').val();
    sDesc = station.find('#sDesc').val();
    sStatus = station.find('#sStatus').find('option:selected').text();
    sStatusVal = station.find('#sStatus').val();

    if (_stationDetails.name != sName || _stationDetails.desc != sDesc || _stationDetails.status != sStatus) {
        $.ajax({
            method: "POST",
            url: 'Stations/UpdateStation',
            dataType: "json",
            data: {
                id: parseInt(sId),
                name: sName,
                description: sDesc,
                status: parseInt(sStatusVal)
            },
            success: function (data) {
                $('body').append(data.html);
                $('#update-station-modal').modal('show');
            }
        });
    }

    station.find('.station-name').html(sName);
    station.find('.station-desc').html(sDesc);
    station.find('.station-status').html(sStatus);
    station.find('.edit').html('ערוך');
    station.removeClass('editing');
    $('.link-worker, .related-workers').removeClass('disabled');

    return msg;
}


$(document).on('click', '#Stations-list .delete', function () {
    var stationToRemove = $(this).parents('tr');
    var stationId = stationToRemove.data('id');

    $.ajax({
        method: "POST",
        url: 'Stations/DeleteStation',
        data: {
            stationId: stationId.toString()
        },
        success: function (data) {
            stationToRemove.remove();
            $('body').append(data.html);
            $('#delete-station-modal').modal('show');

        }
    });
});
/* stations */



/* General */

// create drop down list dynamicly
// options example: [{value:= "1", text: "option1"}, ...]
function getDropDown(options, id, css, selected) {
    var html = '<select id="' + id + '" class="'+ css + '">';
    
    for (var i = 0; i < options.length; i++) {
        html += '<option value="' + options[i].value + '"';
        if (options[i].text == selected) {
            html += ' selected';
        }
        html += '>' + options[i].text + '</option>';
    }
    html += '</select>';
    return html;
}

function resetForm(form) {
    form[0].reset();
    form.find('.input-validation-error').removeClass('input-validation-error');
    form.find('.field-validation-valid').html('');
}

/* General */


/* functions */


/* Add worker modal */
/* 
Orginal Page: http://thecodeplayer.com/walkthrough/jquery-multi-step-form-with-progress-bar 

*/
//jQuery time
var current_fs, next_fs, previous_fs; //fieldsets
var left, opacity, scale; //fieldset properties which we will animate
var animating; //flag to prevent quick multi-click glitches

$(".next").click(function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    next_fs = $(this).parent().next();

    //activate next step on progressbar using the index of next_fs
    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

    //show the next fieldset
    next_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)
            left = (now * 50) + "%";
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'transform': 'scale(' + scale + ')' });
            next_fs.css({ 'left': left, 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});

$(".previous").click(function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    previous_fs = $(this).parent().prev();

    //de-activate current step on progressbar
    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    //show the previous fieldset
    previous_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale previous_fs from 80% to 100%
            scale = 0.8 + (1 - now) * 0.2;
            //2. take current_fs to the right(50%) - from 0%
            left = ((1 - now) * 50) + "%";
            //3. increase opacity of previous_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'left': left });
            previous_fs.css({ 'transform': 'scale(' + scale + ')', 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});
