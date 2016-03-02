
$(document).ready(function () {

    $("#CDept").hide(); 
    $("#CLoc").hide();
    $("#CCat").hide();
    $('#Criteria').change(function () {
        var test = $(this).val();
        if (test == 'D') {
            $("#CDept").show();
            $("#CSec").hide();
        }
        if (test == 'S') {
            $("#CSec").show();
            $("#CDept").hide();
        }
    });

    $('#HasCat').change(function () {
        var test = $(this).val();
        if (test == 'False') {
            $("#CCat").hide();
        }
        if (test == 'true') {
            $("#CCat").show();
        }
    });

    $('#HasLoc').change(function () {
        var test = $(this).val();
        if (test == 'False') {
            $("#CLoc").hide();
        }
        if (test == 'true') {
            $("#CLoc").show();
        }
    });

    $('#DepartmentID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetDepartment';
    $.getJSON(URL + '/2', function (data) {
        var items;
        $.each(data, function (i, state) {
            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#DepartmentID').html(items);
    });

    $('#SectionID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetSection';
    $.getJSON(URL + '/2', function (data) {
        var items;
        $.each(data, function (i, state) {
            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#SectionID').html(items);
    });


    $('#CompanyID').change(function () {
        $('#DepartmentID').empty();
        //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
        var URL = '/EmailForm/GetDepartment';
        $.getJSON(URL + '/' + $('#CompanyID').val(), function (data) {
            var items;
            $.each(data, function (i, state) {
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
                // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
            });
            $('#DepartmentID').html(items);
        });
    });

    $('#CompanyID').change(function () {
        $('#SectionID').empty();
        //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
        var URL = '/EmailForm/GetSection';
        $.getJSON(URL + '/' + $('#CompanyID').val(), function (data) {
            var items;
            $.each(data, function (i, state) {
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
                // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
            });
            $('#SectionID').html(items);
        });
    });


});
