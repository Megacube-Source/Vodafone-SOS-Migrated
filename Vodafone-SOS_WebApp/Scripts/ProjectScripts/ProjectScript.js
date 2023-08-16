//This function will call the server method to fill dropdown 
function FnFillDropDown(DropdownId, ColumnName,TransactionId,FormType)
{

    $.ajax({
        data: { DropdownId: DropdownId, TransactionId: TransactionId, ColumnName: ColumnName, FormType: FormType },
        url: "/LClaims/GetDropDownValue",
        dataType: "json",
        type: "POST",
        success: function (data) {

            var markup = '<option value="">-- Select Here --</option>';
            for (var x = 0; x < data.length; x++) {
                if (data[x].LdvValue == data[x].SelectedValue) {
                    markup += '<option selected="selected" value="' + data[x].LdvValue + '">' + data[x].LdvValue + '</option>';
                }
                else {
                    markup += '<option value="' + data[x].LdvValue + '">' + data[x].LdvValue + '</option>';
                }
            }
            $("#" + ColumnName).html(markup).show();

        },
        error: function (reponse) {
            //  alert("error : " + reponse);
        }
    });
}

//This function will make dynamic forms in claims and Payees using companyspecific labels
function GenerateCompanySpecifiCForm(CompanySpecificArray,FormType,TransactionId)
{
    //Order the form field as per ordinal number of Companyspecificdata
 
    //console.log(CompanySpecificArray)
    /*Loop through company specific data and add divs on screen as per ordinal number of the field*/
    var ElementToBeAdded = '';
    for (var i = 0; i < CompanySpecificArray.length; ++i) {
        //Make a server call to get dropdown values to be displayed if dropdownId is present in that column
        if (CompanySpecificArray[i].LcscDropDownId) {
            var DropdownId = Number(CompanySpecificArray[i].LcscDropDownId);
            var ColumnName = CompanySpecificArray[i].LcscColumnName;
            FnFillDropDown(DropdownId, ColumnName, TransactionId, FormType);
        }
        //Add tooltip which is same as text box value as directed by JS
        if (document.getElementById(String(CompanySpecificArray[i].LcscColumnName))) {
            var TooltipValue = "";
            if ($('#'+String(CompanySpecificArray[i].LcscColumnName)).is("select")) {
                // the input field is  a dropdown
                //TooltipValue = $("#" + String(CompanySpecificArray[i].LcscColumnName)+" option:selected").text();
TooltipValue='';
            }
            else {
                TooltipValue = document.getElementById(String(CompanySpecificArray[i].LcscColumnName)).value;
            }
            document.getElementById(String(CompanySpecificArray[i].LcscColumnName)).title = TooltipValue;
        }
        else if (document.getElementById('Lp' + String(CompanySpecificArray[i].LcscColumnName))) {
            var TooltipValue = "";
            if ($('#Lp'+String(CompanySpecificArray[i].LcscColumnName)).is("select")) {
                // the input field is  a dropdown
                //TooltipValue = $("#Lp" + String(CompanySpecificArray[i].LcscColumnName)+" option:selected").text();
TooltipValue='';
            }
else  if ($('#Lp'+String(CompanySpecificArray[i].LcscColumnName)).is(":checkbox")) {
TooltipValue='';
}

            else {
                TooltipValue = document.getElementById('Lp'+String(CompanySpecificArray[i].LcscColumnName)).value;
            }
            document.getElementById('Lp' + String(CompanySpecificArray[i].LcscColumnName)).title = TooltipValue;//document.getElementById('Lp' + String(CompanySpecificArray[i].LcscColumnName)).value;
        }
        else if (document.getElementById('Lc' + String(CompanySpecificArray[i].LcscColumnName))) {
             var TooltipValue = "";
            if ($('#Lc'+String(CompanySpecificArray[i].LcscColumnName)).is("select")) {
                // the input field is  a dropdown
                //TooltipValue = $("#Lc" + String(CompanySpecificArray[i].LcscColumnName)+" option:selected").text();
TooltipValue='';
            }
else  if ($('#Lc'+String(CompanySpecificArray[i].LcscColumnName)).is(":checkbox")) {
TooltipValue='';
}
            else {
                TooltipValue = document.getElementById('Lc'+String(CompanySpecificArray[i].LcscColumnName)).value;
            }
            document.getElementById('Lc' + String(CompanySpecificArray[i].LcscColumnName)).title =TooltipValue; //document.getElementById('Lc' + String(CompanySpecificArray[i].LcscColumnName)).value;
        }

        //check if IsManadatory is true then dynamically add validation to those fields
        if (Boolean(CompanySpecificArray[i].LcscIsMandatory) == true) {

            
                if (String(CompanySpecificArray[i].LcscColumnName) == 'A01' || String(CompanySpecificArray[i].LcscColumnName) == 'A02' || String(CompanySpecificArray[i].LcscColumnName) == 'A03' || String(CompanySpecificArray[i].LcscColumnName) == 'A04' || String(CompanySpecificArray[i].LcscColumnName) == 'A05' || String(CompanySpecificArray[i].LcscColumnName) == 'A06' || String(CompanySpecificArray[i].LcscColumnName) == 'A07' || String(CompanySpecificArray[i].LcscColumnName) == 'A08' || String(CompanySpecificArray[i].LcscColumnName) == 'A09' || String(CompanySpecificArray[i].LcscColumnName) == 'A10') {
                    if (document.getElementById(CompanySpecificArray[i].LcscColumnName)) {
                        $('#' + String(CompanySpecificArray[i].LcscColumnName)).prop('required', true);
                    }
                }
                else {
                    if (FormType == 'Claims') {
                        if (document.getElementById('Lc' + CompanySpecificArray[i].LcscColumnName)) {
                            $('#Lc' + String(CompanySpecificArray[i].LcscColumnName)).prop('required', true);
                        }
                    }
                    else if (FormType == 'Payees')
                    {
                        
                        //Added By Sachin on 13/6/2018 for issue in 2.1 "Create/Edit payee screen, if Can Raise Claims and Create login are set as mandatory, system always ask these checkboxes to be checked"
                        if (document.getElementById('Lp' + CompanySpecificArray[i].LcscColumnName)) {
                            if (CompanySpecificArray[i].LcscColumnName != "CanRaiseClaims" && CompanySpecificArray[i].LcscColumnName != "CreateLogin") {
                                $("#Lbl" + CompanySpecificArray[i].LcscColumnName).addClass("required");
                            }
                            var IsWIAMEnabled = null;//SG Changes - Payee Email non mandatory when WIAM Enabled
                            if (document.getElementById('IsWIAMEnabled')) {
                                IsWIAMEnabled = document.getElementById('IsWIAMEnabled').value;
                            }
                            if (IsWIAMEnabled == 'yes' && (CompanySpecificArray[i].LcscColumnName == 'Email')) {
                                $("#LblEmail").removeClass("required");
                            }
                        }
                        
                        if (document.getElementById('Lp' + CompanySpecificArray[i].LcscColumnName)) {
                            if (CompanySpecificArray[i].LcscColumnName != "CanRaiseClaims" && CompanySpecificArray[i].LcscColumnName != "CreateLogin") {
                                $('#Lp' + String(CompanySpecificArray[i].LcscColumnName)).prop('required', true);
                            }
                        }
                    }
                }
         

            //Add red star on required columns
              
                    $("#Lbl" + CompanySpecificArray[i].LcscColumnName).addClass("required");
            
        }

        //replace custom label with Default Label
        if (CompanySpecificArray[i].LcscLabel && document.getElementById("Lbl" + CompanySpecificArray[i].LcscColumnName)) {
            document.getElementById("Lbl" + CompanySpecificArray[i].LcscColumnName).innerHTML = CompanySpecificArray[i].LcscLabel;
        }

        //Where(p => p.LcscColumnName == "PayeeId").Where(p => p.LcscDisplayOnForm == true)
        if (Boolean(CompanySpecificArray[i].LcscDisplayOnForm) == true) {
            if (document.getElementById('DIV' + CompanySpecificArray[i].LcscColumnName)) {
                ElementToBeAdded += document.getElementById('DIV' + CompanySpecificArray[i].LcscColumnName).outerHTML;
            }
        }
else
{
 if (document.getElementById('Hidden' + CompanySpecificArray[i].LcscColumnName)) {
                ElementToBeAdded += document.getElementById('Hidden' + CompanySpecificArray[i].LcscColumnName).outerHTML;
            }
}
/*else
{
 if (String(CompanySpecificArray[i].LcscColumnName) == 'A01' || String(CompanySpecificArray[i].LcscColumnName) == 'A02' || String(CompanySpecificArray[i].LcscColumnName) == 'A03' || String(CompanySpecificArray[i].LcscColumnName) == 'A04' || String(CompanySpecificArray[i].LcscColumnName) == 'A05' || String(CompanySpecificArray[i].LcscColumnName) == 'A06' || String(CompanySpecificArray[i].LcscColumnName) == 'A07' || String(CompanySpecificArray[i].LcscColumnName) == 'A08' || String(CompanySpecificArray[i].LcscColumnName) == 'A09' || String(CompanySpecificArray[i].LcscColumnName) == 'A10') {
ElementToBeAdded += ' <input type="hidden" id="'+CompanySpecificArray[i].LcscColumnName+'" name="'+CompanySpecificArray[i].LcscColumnName+'" > ';
}
else
{
ElementToBeAdded += ' <input type="hidden" id="Lp'+CompanySpecificArray[i].LcscColumnName+'" name="Lp'+CompanySpecificArray[i].LcscColumnName+'" > ';
}
}*/

    }
    /*Now remove the UnOrdered Div from the screen*/
    $('#UnOrderedForm').empty();
    /*Add Ordered Div on the Form*/
    $('#OrderedForm').html(ElementToBeAdded);

}


//This function is defined to get file names uploaded in claims
function FnGetUploadedFileName(Files) {
   // var Files = document.getElementById("File1")
    var FileHtml = ''
    for (var i = 0; i < Files.files.length; ++i) {
            FileHtml = FileHtml + '<li>' + Files.files.item(i).name + '</li>'
    }
    document.getElementById('AttachedFiles').innerHTML = '<p>Recent Attachments (Add description and click Save to add attachment)</p><ul>' + FileHtml + '</ul>'

}

function FnGetUploadedCommon(Files, id, Type, Code) {

    var files = Files.files;
    //var myID = 3; //uncomment this to make sure the ajax URL works
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }
            var seturl = '';
            if (Type == 'LSupportTickets') {
                var seturl = '';
                seturl = '/LSupportTickets/UploadHomeReport?id=' + id + '&Type=' + Type;
            } else if (Type == 'LPayees')
            {
                seturl = '/LPayees/UploadAutoAttachment?id=' + id + '&PayeeCode=' + Code + '&Type=' + Type  ;
            }
            else if (Type == 'LUsers') {
                seturl = '/LUsers/UploadAutoAttachment?id=' + id + '&EmailID=' + Code + '&Type=' + Type;
            }
            else if (Type == 'LClaims') {
                seturl = '/LClaims/UploadAutoAttachment?id=' + id + '&PayeeCode=' + Code + '&Type=' + Type;
            }
            else if (Type == 'LCalc') {
                seturl = '/LCalc/UploadAutoAttachment?id=' + id ;
            }
            $.ajax({
                type: "POST",
                url: seturl,
                contentType: false,
                processData: false,
                data: data,
                async: false,
                success: function (result) {
 
                    var newValue = result.replace('"', '');
                    newValue = newValue.replace('"', '');

                    var nameArr = newValue.split(',');
                    for (var i = 0; i < nameArr.length; i++) {
                        var dataArr = nameArr[i].split(':');
                        if (Type == 'LSupportTickets') {
                            document.getElementById('AttachedFiles').innerHTML = document.getElementById('AttachedFiles').innerHTML + '<span><b>' + dataArr[0] + '</b>&emsp; &emsp;<a href="/GenericGrid/DeleteSupportingDocument?id=' + dataArr[1] + '&EntityId=' + id + '&EntityType=LSupportTickets&FormType=' + Type + '"><i class="glyphicon glyphicon-remove" style="color:red;"></i></a></span><br />';
                        }
                        else if (Type == 'LUsers') {
                            document.getElementById('AttachedFiles').innerHTML = document.getElementById('AttachedFiles').innerHTML + '<span><b>' + dataArr[0] + '</b>&emsp; &emsp;<a href="/GenericGrid/DeleteSupportingDocument?id=' + dataArr[1] + '&EntityId=' + id + '&EntityType=LUsers&FormType=' + Type + '"><i class="glyphicon glyphicon-remove" style="color:red;"></i></a></span><br />';
                        }
                        else if (Type == 'LClaims') {
                            document.getElementById('AttachedFiles').innerHTML = document.getElementById('AttachedFiles').innerHTML + '<span><b>' + dataArr[0] + '</b>&emsp; &emsp;<a href="/GenericGrid/DeleteSupportingDocument?id=' + dataArr[1] + '&EntityId=' + id + '&EntityType=LClaims&FormType=' + Type + '"><i class="glyphicon glyphicon-remove" style="color:red;"></i></a></span><br />';
                        }
                        else if (Type == 'LPayees') {
                            document.getElementById('AttachedFiles').innerHTML = document.getElementById('AttachedFiles').innerHTML + '<span><b>' + dataArr[0] + '</b>&emsp; &emsp;<a href="/GenericGrid/DeleteSupportingDocument?id=' + dataArr[1] + '&EntityId=' + id + '&EntityType=LPayees&FormType=' + Type + '"><i class="glyphicon glyphicon-remove" style="color:red;"></i></a></span><br />';
                        }
                        else if (Type == 'LCalc') {
                            document.getElementById('AttachedFiles').innerHTML = document.getElementById('AttachedFiles').innerHTML + '<span><b>' + dataArr[0] + '</b>&emsp;</span><br />';
                        }
                        //document.getElementById('AttachedFiles').innerHTML = '<p>Recent Attachments (Add description and click Save to add attachment)</p><ul>' + FileHtml + '</ul>'

                    }
                    console.log(result);
                    //location.reload();
                    
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    location.reload();
                }
            });
           
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
}
var isChanged = false;
function FnClickFormButtons(ActionName, TransactionId) {
    isChanged = false;//To skip check 
    switch (ActionName) {
        case "Approve":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=Approve&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "SendToAnalyst":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=SendToAnalyst&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "Dashboard":
            window.location.href = '/GenericGrid/DownloadDashboardFile?TransactionId=' + TransactionId;
            break;
        case "Download":
            window.location.href = '/GenericGrid/DownloadFile?TransactionId=' + TransactionId;
            break;
        case "Withdraw":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=Withdraw&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "Prelim":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=Prelim&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "UnPrelim":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=UnPrelim&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "Reject":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=Reject&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "SelfAssign":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=SelfAssign&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "SendToRequester":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=SendToRequester&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "ReClaim":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=ReClaim&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "VerifyPayee":
            window.location.href = '/GenericGrid/UpdateBaseTableWfStatus?ActionName=VerifyPayee&TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "AssignTo":
            window.location.href = '/GenericGrid/AssignTo?TransactionId=' + TransactionId + '&Comments=' + GlobalComment
            break;
        case "WashClaim":
        case "Edit":
            window.location.href = '/GenericGrid/Edit?TransactionId=' + TransactionId + '&WFConfigId=0'
            break;

    }

}

//This function is used to store comments typed on any of the text area in every WF tab
var GlobalComment = '';
function FnAttachComments(Comment) {
    GlobalComment = Comment;

}
