
$(document).ready(function () {
    var cat = $('#HasCat').val();
    if (cat == 'False') {
        $("#CCat").hide();
    }
    if (cat == 'true') {
        $("#CCat").show();
    }
    var loc = $('#HasLoc').val();
    if (loc == 'False') {
        $("#CLoc").hide();
    }
    if (loc == 'true') {
        $("#CLoc").show();
    }

    var cri = $('#Criteria').val();
    if (cri == 'D') {
        $("#CDept").show();
        $("#CSec").hide();
    }
    if (cri == 'S') {
        $("#CSec").show();
        $("#CDept").hide();
    }


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
    var comp = $('#CompanyID').val();
    $.getJSON(URL + '/' + comp, function (data) {
        var selectedItemID = document.getElementById("selectedDeptHidden").value;
        var items;
        $.each(data, function (i, state) {
            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#DepartmentID').html(items);
    });

    $('#SectionID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetSection';
    $.getJSON(URL + '/' + comp, function (data) {
        var selectedItemID = document.getElementById("selectedSecHidden").value;
        var items;
        $.each(data, function (i, state) {
            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
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
$(window).load(function () {
    // run code
});