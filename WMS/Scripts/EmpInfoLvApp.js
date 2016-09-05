//Index page samna ata ha pora page
$(document).ready(function () {

    //#trigers id
    //it hides id cpl


    // $("#CPL").hide();

    //var URL = '/WMS/LeaveSettings/CPLList';
    $('#buttonId').click(function () {

        var companyid = document.getElementById("CompanyID").value;
        var empNo = document.getElementById("EmpNo").value;
        //var URL = '/WMS/LvApp/GetEmpInfo';
        var URL = '/LvApp/GetEmpInfo';
        $.getJSON(URL + '/' + empNo + "w" + companyid, function (data) {
            var values = data.split('@');
            document.getElementById("EName").value = values[0];
            document.getElementById("EDesignation").value = values[1];
            document.getElementById("ESection").value = values[2];
            document.getElementById("ECL").value = values[3];
            document.getElementById("EAL").value = values[4];
            document.getElementById("ESL").value = values[5];
            document.getElementById("ECPL").value = values[6];
            document.getElementById("EFName").value = values[7];
            document.getElementById("EDOB").value = values[8];
        });
    });
    $('#BbuttonId').click(function () {

        var companyid = document.getElementById("CompanyID").value;
        var empNo = document.getElementById("BEmpNo").value;
        //var URL = '/WMS/LvApp/GetEmpInfo';
        var URL = '/LvApp/GetEmpInfo';
        $.getJSON(URL + '/' + empNo + "w" + companyid, function (data) {
            var values = data.split('@');
            document.getElementById("BName").value = values[0];
            document.getElementById("BDesignation").value = values[1];
            document.getElementById("BSection").value = values[2];
            document.getElementById("ECL").value = values[3];
            document.getElementById("EAL").value = values[4];
            document.getElementById("ESL").value = values[5];
            document.getElementById("ECPL").value = values[6];
            document.getElementById("BFName").value = values[7];
            document.getElementById("BDOB").value = values[8];
        });
    });

});