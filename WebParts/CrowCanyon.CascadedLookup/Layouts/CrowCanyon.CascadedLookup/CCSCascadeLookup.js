var CascadedControls = new Array();

function AddCascadedControl(controlId, isAllowMultiple, allValuesOnEmpty, hFieldId, parentControlId, parentControlType, webUrl, lookupListName, linkedParentfield, lookupTargetField, viewWhereString, viewOrderString) {
    var found = false;

    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].controlId == controlId) {
                found = true;
            }
        }
    }

    if (!found) {
        this.ControlId = controlId;
        this.IsAllowMultiple = isAllowMultiple;
        this.AllValuesOnEmpty = allValuesOnEmpty;
        this.HFieldId = hFieldId;
        this.ParentControlId = parentControlId;
        this.ParentControlType = parentControlType;
        this.WebUrl = webUrl;
        this.LookupListName = lookupListName;
        this.LinkedParentfield = linkedParentfield;
        this.LookupTargetField = lookupTargetField;
        this.ViewWhereString = viewWhereString;
        this.ViewOrderString = viewOrderString;

        CascadedControls.push(this);
    }
}

function getQueryString(control) {

    var whereString = "";
    if (control.ParentControlId != '') {
        if (control.ParentControlType == 2) {
            var ctrlIds = control.ParentControlId.split(";#");
            if (ctrlIds != null && ctrlIds.length == 2) {
                var list = document.getElementById(ctrlIds[1]);
                if (list.options.length > 0) {
                    for (var i = 0; i < list.options.length; ++i) {
                        //alert(list.options[i].value);
                        if (whereString == "") {
                            whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">' + list.options[i].value + '</Value></Eq>';
                        }
                        else {
                            whereString = '<Or>' + whereString + '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">' + list.options[i].value + '</Value></Eq></Or>';
                        }
                    }
                }
                else {
                    if (control.AllValuesOnEmpty == false) {
                        whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">-1</Value></Eq>';
                    }
                }
            }
        }
        else {
            var list = document.getElementById(control.ParentControlId);
            if (control.ParentControlType == 1) {
                if (list.value != null && document.getElementById(list.optHid).value != "" && document.getElementById(list.optHid).value != "0") {
                    whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">' + document.getElementById(list.optHid).value + '</Value></Eq>';
                }
                else {
                    if (control.AllValuesOnEmpty == false) {
                        whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">-1</Value></Eq>';
                    }
                }
            }
            else {
                if (list.selectedIndex >= 0) {
                    if (list.options[list.selectedIndex].value != null && list.options[list.selectedIndex].value == "0") {
                        if (control.AllValuesOnEmpty == false) {
                            whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">-1</Value></Eq>';
                        }
                    }
                    else {
                        whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">' + list.options[list.selectedIndex].value + '</Value></Eq>';
                    }
                }
                else {
                    if (control.AllValuesOnEmpty == false) {
                        whereString = '<Eq><FieldRef LookupId="TRUE" Name="' + control.LinkedParentfield + '" /><Value Type="Counter">-1</Value></Eq>';
                    }
                }
            }
        }
    }

    if (control.ViewWhereString != null && control.ViewWhereString != "") {
        if (whereString != "") {
            whereString = '<And>' + whereString + control.ViewWhereString + '</And>';
        }
        else {
            whereString = control.ViewWhereString;
        }
    }

    if (whereString != "") {
        whereString = '<Where>' + whereString + '</Where>';
    }

    if (control.ViewOrderString != "") {
        whereString = whereString + control.ViewOrderString;
    }
    else {
        whereString = whereString + '<OrderBy><FieldRef Name="' + control.LookupTargetField + '" /></OrderBy>';
    }

    var result = '<?xml version="1.0" encoding="utf-8"?>' +
	'<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' +
	  '<soap12:Body>' +
		'<GetListItems xmlns="http://schemas.microsoft.com/sharepoint/soap/">' +
		'<listName>' + control.LookupListName + '</listName>' +
			'<query>' +
				'<Query>' +
				    whereString +
				'</Query>' +
			'</query>' +
			'<viewFields><ViewFields>' +
				'<FieldRef Name="' + control.LookupTargetField + '" /><FieldRef Name="ID" />' +
			'</ViewFields></viewFields>' +
		'</GetListItems>' +
	  '</soap12:Body>' +
	'</soap12:Envelope>'
    return result;
}


function UpdateMyChildControls(triggerObjectId) {

    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].ParentControlId != '') {
                if (CascadedControls[i].ParentControlId == triggerObjectId) {
                    var childControl = CascadedControls[i];
                    var wssSvc = new WssSvcCall();
                    wssSvc.soapQuery = getQueryString(childControl);
                    wssSvc.url = childControl.WebUrl + "/_vti_bin/lists.asmx";
                    wssSvc.returnFunctionName = function () { filterChildControl(childControl); } //Second arg is to indicate it is not at the time of Loading..But, at the time of change event occured.
                    wssSvc.Submit();
                }
            }
        }
    }
}

function UpdateMyControls(triggerObjectId) {

    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].ControlId != '') {
                if (CascadedControls[i].ControlId == triggerObjectId) {
                    var childControl = CascadedControls[i];
                    var wssSvc = new WssSvcCall();
                    wssSvc.soapQuery = getQueryString(childControl);
                    wssSvc.url = childControl.WebUrl + "/_vti_bin/lists.asmx";
                    wssSvc.returnFunctionName = function () { filterChildControl(childControl); } //Second arg is to indicate it is not at the time of Loading..But, at the time of change event occured.
                    wssSvc.Submit();
                }
            }
        }
    }
}

function WssSvcCall() {

    this.soapQuery = "";
    this.url = "";
    this.returnFunctionName = function () { return; };

    this.Submit = function () {
        http_request = false;
        if (window.XMLHttpRequest) {
            http_request = new XMLHttpRequest();
            if (http_request.overrideMimeType) {
                http_request.overrideMimeType('text/html');
            }
        } else if (window.ActiveXObject) {
            try {
                http_request = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                try {
                    http_request = new ActiveXObject("Microsoft.XMLHTTP");
                } catch (e) { }
            }
        }
        if (!http_request) {
            alert('Cannot create XMLHTTP instance');
            return false;
        }

        http_request.onreadystatechange = this.returnFunctionName;
        http_request.open('POST', this.url, false);
        http_request.setRequestHeader("Content-type", "application/soap+xml; charset=utf-8");
        http_request.setRequestHeader("Content-length", this.soapQuery.length);
        http_request.send(this.soapQuery);
    }
}

function parseXML(inputString) {
    if (window.DOMParser) {
        parser = new DOMParser();
        xmlDoc = parser.parseFromString(inputString, "text/xml");
    }
    else {
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = "false";
        xmlDoc.loadXML(inputString);
    }
    return xmlDoc;
}

function filterChildControl(control) {
    if (http_request.readyState == 4) {
        if (http_request.status == 200) {
            xmlResult = parseXML(http_request.responseText);
            var resultNodes = xmlResult.getElementsByTagName('z:row');
            var elementsByStar = false;
            try {
                if (resultNodes.length == 0) {
                    resultNodes = xmlResult.getElementsByTagName('*');
                    if (resultNodes.length > 0) {
                        elementsByStar = true;
                    }
                }
            }
            catch (er) { }

            var LeftboxCtrl = "";
            var RightboxCtrl = "";
            var DropdwonCtrl = "";
            var selValue = "0";

            if (control.IsAllowMultiple) {
                var ctrlIds = control.ControlId.split(";#");
                if (ctrlIds != null && ctrlIds.length == 2) {
                    LeftboxCtrl = document.getElementById(ctrlIds[0]);
                    RightboxCtrl = document.getElementById(ctrlIds[1]);

                    var length = LeftboxCtrl.options.length;
                    for (var i = length - 1; i >= 0; i--) {
                        LeftboxCtrl.options[i] = null;
                    }
                }
            }
            else {
                DropdwonCtrl = document.getElementById(control.ControlId);
                var length = DropdwonCtrl.options.length;
                if (length > 0) {
                    if (DropdwonCtrl.selectedIndex >= 0) {
                        selValue = DropdwonCtrl.options[DropdwonCtrl.selectedIndex].value;
                    }

                    for (var i = length - 1; i >= 0; i--) {
                        if (DropdwonCtrl.options[i].value != "0") {
                            DropdwonCtrl.options[i] = null;
                        }
                    }
                }
            }
            
            for (var i = 0; i < resultNodes.length; i++) {
                if ((elementsByStar && resultNodes[i].nodeName == 'z:row') || !elementsByStar) {
                    var targetFieldValue = attributeValue(resultNodes[i], "ows_" + control.LookupTargetField);

                    var targetFieldID = attributeValue(resultNodes[i], "ows_ID");

                    if (targetFieldValue.length > 0 && targetFieldID.length > 0) {
                        if (control.IsAllowMultiple) {

                            var opt = document.createElement("option");
                            LeftboxCtrl.options.add(opt, null);
                            // Assign text and value to Option object
                            opt.text = targetFieldValue;
                            opt.value = targetFieldID;
                        }
                        else {
                            DropdwonCtrl.options[DropdwonCtrl.options.length] = new Option(targetFieldValue, targetFieldID, false, false);
                        }
                        //optionsCount = optionsCount + 1;
                    }
                }
            }

            if (control.IsAllowMultiple) {
                var rightlen = RightboxCtrl.options.length;
                for (var i = rightlen - 1; i >= 0; i--) {
                    var deleteitem = true;
                    var leftlen = LeftboxCtrl.options.length;
                    for (var j = leftlen - 1; j >= 0; j--) {
                        if (RightboxCtrl.options[i].value == LeftboxCtrl.options[j].value) {
                            deleteitem = false;
                            LeftboxCtrl.options[j] = null;
                        }
                    }

                    if (deleteitem) {
                        RightboxCtrl.options[i] = null;
                    }
                }

                SetValueFromListBox(control.HFieldId, RightboxCtrl.id);
            }
            else {
                var len = DropdwonCtrl.options.length;
                for (var i = 0; i < len; i++) {
                    if (DropdwonCtrl.options[i].value == selValue) {
                        DropdwonCtrl.selectedIndex = i;
                        break;
                    }
                }

                SetValueFromDropDown(control.HFieldId, DropdwonCtrl.id);
            }


            //update Child control
            UpdateMyChildControls(control.ControlId);
        }
        else {
            alert('There was a problem with the request.');
        }
    }
}

function attributeValue(node, attributeName) {
    var attributesCollection = node.attributes;
    for (atv = 0; atv < attributesCollection.length; atv++) {
        if (attributesCollection[atv].name == attributeName) return attributesCollection[atv].value
    }
    return "";
}

function Listbox_MoveAcross(sourceID, destID) {
    var src = document.getElementById(sourceID);
    var dest = document.getElementById(destID);

    if (src.selectedIndex == -1) {
        return;
    }

    dest.selectedIndex = -1;

    for (var count = 0; count < src.options.length; count++) {

        if (src.options[count].selected == true) {
            var option = src.options[count];

            var newOption = document.createElement("option");
            newOption.value = option.value;
            newOption.text = option.text;
            newOption.selected = true;
            try {
                dest.add(newOption, null); //Standard
                src.remove(count, null);
            } catch (error) {
                dest.add(newOption); // IE only
                src.remove(count);
            }
            count--;
        }
    }
}

function SetValueFromDropDown(hfieldId, ControlId) {
    var hfield = document.getElementById(hfieldId);
    var dropDownCtrl = document.getElementById(ControlId);
    
    if (hfield == null) {
        return;
    }

    hfield.value = "";
    
    if (dropDownCtrl != null) {
        if (dropDownCtrl.selectedIndex == -1) {
            hfield.value = "";
        }
        else {
            if (dropDownCtrl.options[dropDownCtrl.selectedIndex].value == "0") {
                hfield.value = "";
            }
            else {
                hfield.value = dropDownCtrl.options[dropDownCtrl.selectedIndex].value + ";#" + dropDownCtrl.options[dropDownCtrl.selectedIndex].text;
            }

        }
    }
}

function SetValueFromListBox(hfieldId, ControlId) {
    var hfield = document.getElementById(hfieldId);
    var listboxCtrl = document.getElementById(ControlId);
    
    if (hfield == null) {
        return;
    }

    hfield.value = "";
    
    if (listboxCtrl != null) {
        if (listboxCtrl.options.length > 0) {
            for (var i = 0; i < listboxCtrl.options.length; i++) {
                if (hfield.value == "") {
                    hfield.value = listboxCtrl.options[i].value + ";#" + listboxCtrl.options[i].text;
                }
                else {
                    hfield.value = hfield.value + ";#" + listboxCtrl.options[i].value + ";#" + listboxCtrl.options[i].text;
                }
            }
        }
    }
    else {
        hfield.value = "";
    }
}

function createListItem1(controlId, txtNewEntryid) {

    var txtNewentry = document.getElementById(txtNewEntryid);

    if (txtNewentry.value == '') {
        return;
    }

    this.oControl = null;
    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].ControlId == controlId) {
                this.oControl = CascadedControls[i];
                break;
            }
        }
    }

    if (this.oControl != null) {
        var siteUrl = "/sites/MySiteCollection";
        var clientContext = new SP.ClientContext(siteUrl);
        //var oList = clientContext.get_web().get_lists().getByTitle(oControl.LookupListName);
//        var oSite = clientContext.get_site();
        var oWebObj = clientContext.get_site().openWeb(oControl.WebUrl);  //oSite.openWeb(oControl.WebUrl);
        var oList = oWebObj.get_lists().getByTitle(oControl.LookupListName);

        var itemCreateInfo = new SP.ListItemCreationInformation();
        this.oListItem = oList.addItem(itemCreateInfo);
        this.otxtValue = txtNewentry.value;

        oListItem.set_item(oControl.LookupTargetField, txtNewentry.value);

        // set Parent Field Value
        if (oControl.ParentControlId != '') {
            var parentValue = GetParentValue(oControl);
            if (parentValue != null) {
                oListItem.set_item(oControl.LinkedParentfield, parentValue);
            }
        }
        oListItem.update();

        clientContext.load(oListItem);

        clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
    }
}

function onQuerySucceeded() {

    alert('Item created: ' + oListItem.get_id());

    //Add value in control
    if (oControl.ParentControlId != '') {
        var parentValue = GetParentValue(oControl);
        if (parentValue != null && parent != "") {
            //add value
            AddItem(oControl, oListItem.get_id(), otxtValue);
        }
        else{
            if(oControl.AllValuesOnEmpty){
                //add value
                AddItem(oControl, oListItem.get_id(), otxtValue);
            }
        }
     }
     else{
        //add value
         AddItem(oControl, oListItem.get_id(), otxtValue);
     }
}
        
function onQueryFailed(sender, args) {

    alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
}


function GetParentValue(control) {

    if (control.ParentControlType == 2) {
        var ctrlIds = control.ParentControlId.split(";#");
        if (ctrlIds != null && ctrlIds.length == 2) {
            var list = document.getElementById(ctrlIds[1]);
            if (list.options.length > 0) {
                for (var i = 0; i < list.options.length; ++i) {
                    if (list.options[i].selected && list.options[i].value != "0") {
                        return list.options[i].value;
                    }
                }

                return list.options[0].value;
            }
        }
    }
    else {
        var list = document.getElementById(control.ParentControlId);
        if (control.ParentControlType == 1) {
            if (list.value != null && list.value != "") {
                return document.getElementById(list.optHid).value;
            }
        }
        else {
            if (list.selectedIndex >= 0) {
                if (list.options[list.selectedIndex].value != null && list.options[list.selectedIndex].value != "0") {
                    return list.options[list.selectedIndex].value;
                }
            }
        }
    }

    return null;
}

function AddItem(control, cvalue, cText) {
    if (control.IsAllowMultiple) {
        var ctrlIds = control.ControlId.split(";#");
        if (ctrlIds != null && ctrlIds.length == 2) {
            var RightboxCtrl = document.getElementById(ctrlIds[1]);

            var opt = document.createElement("option");
            RightboxCtrl.options.add(opt, null);

            // Assign text and value to Option object
            opt.text = cText;
            opt.value = cvalue;

            SetValueFromListBox(control.HFieldId, RightboxCtrl.id);
        }
    }
    else {
        DropdwonCtrl = document.getElementById(control.ControlId);

        DropdwonCtrl.options[DropdwonCtrl.options.length] = new Option(cText, cvalue, false, false);

        DropdwonCtrl.selectedIndex = DropdwonCtrl.options.length;

        SetValueFromDropDown(control.HFieldId, DropdwonCtrl.id);
    }

    UpdateMyChildControls(control.ControlId);
}

function getCreatQueryString(controlId, txtNewEntryid) {
    var txtNewentry = document.getElementById(txtNewEntryid);

    if (txtNewentry.value == '') {
        return;
    }

    this.oControl = null;
    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].ControlId == controlId) {
                this.oControl = CascadedControls[i];
                break;
            }
        }
    }

    var soap = '<?xml version="1.0" encoding="utf-8"?>';
    //soap += '<soap:Envelope xmlns:xsi="\http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd="\http://www.w3.org/2001/XMLSchema\" xmlns:soap="\http://schemas.xmlsoap.org/soap/envelope/\">';
    //soap += '<soap:Body>';
    soap += '<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">';
    soap += '<soap12:Body>';

    soap += '<UpdateListItems xmlns="http://schemas.microsoft.com/sharepoint/soap/">';
    soap += '<listName>' + oControl.LookupListName + '</listName>';
    soap += '<updates><Batch OnError="Continue">';
    soap += '<Method ID="1" Cmd="New">';

    soap += '<Field Name="' + oControl.LookupTargetField + '">' + txtNewentry.value + '</Field>';

    // set Parent Field Value
    if (oControl.ParentControlId != '') {
        var parentValue = GetParentValue(oControl);
        if (parentValue != null) {
            soap += '<Field Name="' + oControl.LinkedParentfield + '">' + parentValue + '</Field>';
        }
    }

    soap += ' </Method></Batch></updates>'
    soap += '</UpdateListItems>';
    //</soap:Body></soap:Envelope>';
    soap += '</soap12:Body>';
    soap += '</soap12:Envelope>';

    return soap;
}


function createListItem(controlId, txtNewEntryid) {

    this.oControl = null;
    if (CascadedControls.length > 0) {
        for (var i = 0; i < CascadedControls.length; i++) {
            if (CascadedControls[i].ControlId == controlId) {
                this.oControl = CascadedControls[i];
                break;
            }
        }
    }

    if (this.oControl != null) {
        var wssSvc = new WssSvcCall();
        wssSvc.soapQuery = getCreatQueryString(controlId, txtNewEntryid);
        wssSvc.url = oControl.WebUrl + "/_vti_bin/lists.asmx";
        wssSvc.returnFunctionName = function () { UpdateControlValue(oControl); } //Second arg is to indicate it is not at the time of Loading..But, at the time of change event occured.
        wssSvc.Submit();
    }
}

function UpdateControlValue(control) {
    
    if (http_request.readyState == 4) {
        if (http_request.status == 200) {
            xmlResult = parseXML(http_request.responseText);
            var resultNodes = xmlResult.getElementsByTagName('z:row');
            var elementsByStar = false;
            try {
                if (resultNodes.length == 0) {
                    resultNodes = xmlResult.getElementsByTagName('*');
                    if (resultNodes.length > 0) {
                        elementsByStar = true;
                    }
                }
            }
            catch (er) { }

            for (var i = 0; i < resultNodes.length; i++) {
                if ((elementsByStar && resultNodes[i].nodeName == 'z:row') || !elementsByStar) {
                    var targetFieldValue = attributeValue(resultNodes[i], "ows_" + control.LookupTargetField);

                    var targetFieldID = attributeValue(resultNodes[i], "ows_ID");

                    if (targetFieldValue.length > 0 && targetFieldID.length > 0) {
                        if (control.IsAllowMultiple) {
                            var ctrlIds = control.ControlId.split(";#");
                            if (ctrlIds != null && ctrlIds.length == 2) {
                                var RightboxCtrl = document.getElementById(ctrlIds[1]);

                                var opt = document.createElement("option");
                                RightboxCtrl.options.add(opt, null);

                                // Assign text and value to Option object
                                opt.text = targetFieldValue;
                                opt.value = targetFieldID;

                                SetValueFromListBox(control.HFieldId, RightboxCtrl.id);
                            }
                        }
                        else {
                            DropdwonCtrl = document.getElementById(control.ControlId);
                            DropdwonCtrl.options[DropdwonCtrl.options.length] = new Option(targetFieldValue, targetFieldID, false, false);
                            DropdwonCtrl.selectedIndex = (DropdwonCtrl.options.length - 1);

                            SetValueFromDropDown(control.HFieldId, DropdwonCtrl.id);
                        }
                    }
                }
            }

            //update Child control
            UpdateMyChildControls(control.ControlId);
        }
        else {
            alert('There was a problem with the request.');
        }
    }
}