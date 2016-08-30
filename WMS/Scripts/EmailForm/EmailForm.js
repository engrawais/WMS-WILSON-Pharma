
$(document).ready(function () {
    $("#CDept").hide();
    $("#CSec").hide();
    $("#CCat").hide()
    $("#CLoc").hide();

    $('#CriteriaComp').change(function () {
        var test = $(this).val();
        if (test == 'C') {
            $("#CComp").show();
            $("#SecDept").show();
            $("#CLoc").hide();
        }
        if (test == 'L') {
            $("#CLoc").show();
            $("#CComp").hide();
            $("#CDept").hide();
            $("#CSec").hide();
            $("#SecDept").hide();
           
        }
    });
    $('#Criteria').change(function () {
        var test = $(this).val();
        if (test == 'S') {

            $("#CSec").show();
            $("#CDept").hide();
        }
        if (test == 'D') {
            $("#CSec").hide();
            $("#CDept").show();
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

        // loading section if  select department changed/////////////////////////
        ////////////////////////////////////////////////////////////


        //$('#DepartmentID').change(function () {
        //    $('#SectionID').empty();
        //    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
        //    var URL = '/EmailForm/GetDepartment';
        //    $.getJSON(URL + '/' + $('#DpartmentID').val(), function (data) {
        //        var items;
        //        $.each(data, function (i, state) {
        //            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
        //            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        //        });
        //        $('#SectionID').html(items);
        //    });

    });
});
