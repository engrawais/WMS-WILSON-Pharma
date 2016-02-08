//Index page samna ata ha pora page
$(document).ready(function () {

    //#trigers id
    //it hides id cpl


    // $("#CPL").hide();

    //var URL = '/WMS/LeaveSettings/CPLList';
    $('#buttonId').click(function () {
        $("#EmpDetails").show();
        var companyid = document.getElementById("CompanyIDJobCard").value;
        var empNo = document.getElementById("JobEmpNo").value;
        var URL = '/WMS/LvApp/GetEmpInfo';
        // var URL = '/Emp/GradeList';
        $.getJSON(URL + '/' + empNo + "w" + companyid, function (data) {
            var values = data.split('@');
            document.getElementById("EName").value = values[0];
            document.getElementById("EDesignation").value = values[1];
            document.getElementById("ESection").value = values[2];
            document.getElementById("EFName").value = values[6];
            document.getElementById("EDOB").value = values[7];
        });
    });


});