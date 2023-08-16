/// <reference path="../jqxgrid.js" />
 function RowDoubleClick()
 {
     $.get('../jqxgrid.js');
//This method is called when we double click on the grid rows
$('#jqxgridOpen').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridOpen').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridOpen').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
$('#jqxgridReExamine').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridReExamine').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridReExamine').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
$('#jqxgridWithdrawn').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridWithdrawn').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridWithdrawn').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
$('#jqxgridRejected').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridRejected').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridRejected').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
$('#jqxgridApproved').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridApproved').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridApproved').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
$('#jqxgridPaid').on('rowdoubleclick', function (event) {
    var selectedrowindex = $('#jqxgridPaid').jqxGrid('selectedrowindex');
    var xx = $('#jqxgridPaid').jqxGrid("getrowid", selectedrowindex);
    if (xx) {
        window.location.href = "/LClaims/Edit/" + xx + "?FormType=disabled";
    }
});
}