function GetChildNodeByIndex(tbody, tagName, index) {
    var i = 0;
    for (var y = 0; y < tbody.childNodes.length; y++) {
        if (tbody.childNodes[y].nodeType == 1 && tbody.childNodes[y].tagName.toLowerCase() == tagName) {
            if (i == index)
                return tbody.childNodes[y];
            i++;
        }
    }
}

var iw_valuetxt;
function openValuePicker(listID, typeID, txtID, multiline) {
    var type = document.getElementById(typeID);
    iw_valuetxt = document.getElementById(txtID);
    if (type == null || iw_valuetxt == null)
        return;

    iw_url = "valuepicker.aspx?List=" + listID + "&Type=" + type.value + "&Value=" + escapeProperly(iw_valuetxt.value) + "&MultiLine=" + multiline;


    var popX = 450;
    var popY = 400;

    if (typeof (SP) == 'undefined') {//2007
        var frm = document.getElementById("iwcfframe");
        frm.src = iw_url;
        IWDBsm(document.getElementById("ol"), popX, popY);
    }
    else {
        if (iw_url.indexOf("?") > 0)
            iw_url += "&IsDlg=1";
        else
            iw_url += "?IsDlg=1";

        var options = {
            url: iw_url,
            width: popX,
            height: popY,
            title: 'Infowise Smart List Pro'
        }
        options.dialogReturnValueCallback = Function.createDelegate(null, IWSmActCloseCallback);
        SP.UI.ModalDialog.showModalDialog(options);

    }
}


function setValueType(v_defTypes, ddlID, txtID) {
    var ddl = document.getElementById(ddlID);
    if (ddl == null)
        return;
    var selectedValue = ddl.value;

    var selectedFieldType = "";
    for (i = 0; i < v_defTypes.length; i++) {
        if (v_defTypes[i].fieldName == selectedValue) {
            selectedFieldType = v_defTypes[i].fieldType;
            break;
        }
    }
    var txt = document.getElementById(txtID);
    txt.value = selectedFieldType;
}

function FilterOperators(v_defTypes, v_fieldTypes, fieldDDL, operatorsDDL, txtValue) {
    var fieldControl = document.getElementById(fieldDDL);
    var conditionControl = document.getElementById(operatorsDDL);
    if (fieldControl == null || conditionControl == null)
        return;

    var selectedValue = fieldControl.options[fieldControl.selectedIndex].value;
    var conditionValue = conditionControl.options[conditionControl.selectedIndex].value;

    var found = false;

    // searching for the right field type
    var selectedFieldType = "text";
    for (i = 0; i < v_defTypes.length; i++) {
        if (v_defTypes[i].fieldName == selectedValue) {
            selectedFieldType = v_defTypes[i].fieldType;
            break;
        }
    }

    if (selectedFieldType == "datetime")
        selectedFieldType = "number";

    for (i = 0; i < v_fieldTypes.length; i++) {
        var curFieldType = v_fieldTypes[i].name;


        if (curFieldType == selectedFieldType) {

            if (curFieldType == "yesno")
                document.getElementById(txtValue).style.display = "none";
            else
                document.getElementById(txtValue).style.display = "";

            conditionControl.options.length = v_fieldTypes[i].operators.length
            for (j = 0; j < v_fieldTypes[i].operators.length; j++) {
                conditionControl.options[j].value = v_fieldTypes[i].operators[j].operation;
                if (v_fieldTypes[i].operators[j].operation == conditionValue)
                    found = true;
                conditionControl.options[j].text = v_fieldTypes[i].operators[j].text;
            }
            break;
        }
    }

    if (found)
        conditionControl.value = conditionValue;
}
//sets optional section state
function ToggleSection(imgLink, hdnID, value) {
    var img = GetFirstChild(imgLink);
    var tr = GetParentByTagName(imgLink, "tr");

    if (tr.style.display == "none")//do not show sections with hidden header
        return;

    var section = GetSiblingByTagName(tr.nextSibling, "tr");
    var hdnEl = document.getElementById(hdnID);

    var hide = value;
    if (hide == null)
        hide = section.style.display == "";
    if (hide) {

        section.style.display = "none";
        img.src = "/_layouts/images/plus.gif";
        hdnEl.value = "hidden";
    }
    else {
        section.style.display = "";
        img.src = "/_layouts/images/minus.gif";
        hdnEl.value = "visible";
    }
}

function GetFirstChild(parentNode) {
    for (var i = 0; i < parentNode.childNodes.length; i++)
        if (parentNode.childNodes[i].nodeType == 1)
            return parentNode.childNodes[i];
}

function GetSiblingByTagName(el, tagName) {
    if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
        return el;

    if (el.nextSibling == null)
        return null;

    return GetSiblingByTagName(el.nextSibling, tagName);
}

function GetParentByTagName(el, tagName) {
    if (el.nodeType == 1 && el.tagName.toLowerCase() == tagName.toLowerCase())
        return el;
    if (el.parentNode == null)
        return null;
    return GetParentByTagName(el.parentNode, tagName);
}

function SetToggleSection(linkID, hdnID) {
    var hdnEl = document.getElementById(hdnID);
    var hide = null;
    if (hdnEl.value != "")
        hide = hdnEl.value != "visible";
    if (hide != null) {
        var imgLink = document.getElementById(linkID);
        ToggleSection(imgLink, hdnID, hide);
    }
}

function IWDBpageWidth() {
    return window.top.innerWidth != null ? window.top.innerWidth : window.top.document.documentElement && window.top.document.documentElement.clientWidth ? window.top.document.documentElement.clientWidth : window.top.document.body != null ? window.top.document.body.clientWidth : null;
}

function IWDBpageHeight() {
    return window.top.innerHeight != null ? window.top.innerHeight : window.top.document.documentElement && window.top.document.documentElement.clientHeight ? window.top.document.documentElement.clientHeight : window.top.document.body != null ? window.top.document.body.clientHeight : null;
}

function IWDBpageWinHeight(win) {
    return win.document.scrollHeight != null ? win.document.scrollHeight : win.document.body.scrollHeight
}

function IWDBpageWinWidth(win) {
    return win.document.scrollWidth != null ? win.document.scrollWidth : win.document.body.scrollWidth
}

function IWDBposLeft() {
    return typeof window.top.pageXOffset != 'undefined' ? window.top.pageXOffset : window.top.document.documentElement && window.top.document.documentElement.scrollLeft ? window.top.document.documentElement.scrollLeft : window.top.document.body.scrollLeft ? window.top.document.body.scrollLeft : 0;
}

function IWDBposTop() {
    return typeof window.top.pageYOffset != 'undefined' ? window.top.pageYOffset : window.top.document.documentElement && window.top.document.documentElement.scrollTop ? window.top.document.documentElement.scrollTop : window.top.document.body.scrollTop ? window.top.document.body.scrollTop : 0;
}

//Adjusts modal dialog BG scrol position
function IWDBscrollFix() {
    var obols = document.getElementsByName('ol');
    for (var i = 0; i < obols.length; i++) {
        obols[i].style.top = IWDBposTop() + 'px';
        obols[i].style.left = IWDBposLeft() + 'px';
    }
}

//Sets modal dialog BG full screen
function IWDBsizeFix() {
    var obols = document.getElementsByName('ol');
    for (var i = 0; i < obols.length; i++) {
        obols[i].style.height = IWDBpageHeight() + 'px';
        obols[i].style.width = IWDBpageWidth() + 'px';
    }
}

//Shows modal dialog for SharePoint 2007
function IWDBsm(obol, wd, ht) {
    var b = '';
    var p = 'px';

    obol.style.height = IWDBpageHeight() + p;
    obol.style.width = IWDBpageWidth() + p;
    obol.style.top = IWDBposTop() + p;
    obol.style.left = IWDBposLeft() + p;
    obol.style.display = b;

    IWPositionPopup(obol.firstChild, wd, ht);


    return false;
}

//Positions modal dialog center screen
function IWPositionPopup(obbx, wd, ht) {
    var p = 'px';
    var tp = IWDBposTop() + ((IWDBpageHeight() - ht) / 2) - 12;
    var lt = IWDBposLeft() + ((IWDBpageWidth() - wd) / 2) - 12;

    obbx.style.top = (tp < 0 ? 0 : tp) + p;
    obbx.style.left = (lt < 0 ? 0 : lt) + p;
    obbx.style.width = wd + p;
    obbx.style.height = ht + p;
    obbx.style.display = '';
}

//Hides modal dialog
function IWhm(obol) {
    var n = 'none';
    obol.style.display = n;
    document.onkeypress = '';
}

//Positions New form dialog
function IWinitmb() {
    window.top.onscroll = IWDBscrollFix;
    window.top.onresize = IWDBsizeFix;
}

//Attaches event listener to position the New modal dialog
if (window.addEventListener)
    window.addEventListener('load', IWinitmb, false);
else if (window.attachEvent)
    window.attachEvent('onload', IWinitmb);


var iw_url = "";

//Returns last child element, cross-browser
function IWGetLastChild(parent) {
    var children = parent.childNodes;
    if (children.length == 0)
        return null;

    for (var i = children.length - 1; i >= 0; i--) {
        if (children[i].nodeType == 1)
            return children[i];
    }
    return null;
}

//Returns first child element, cross-browser
function IWGetFirstChild(parent) {
    var children = parent.childNodes;
    if (children.length == 0)
        return null;

    for (var i = 0; i < children.length; i++) {
        if (children[i].nodeType == 1)
            return children[i];
    }
    return null;
}


var iwatf_refresh = null; //holds ID of refresh button
//Close new form callback for SharePoint 2010 client object model
function IWSmActCloseCallback(result, newValue) {
    if (result == SP.UI.DialogResult.OK)
        iw_valuetxt.value = newValue;
}

//Check status of New frame
function IWSmActCheckFrame(event, frm, frmWindow, forceClose, newValue) {
    if (frm == null) {
        frm = top.document.getElementById("iwcfframe");
        frmWindow = frm.contentWindow;
    }
    else if (frmWindow == null)
        frmWindow = frm.contentWindow;

    var gears = "/_layouts/images/GEARS_AN.GIF";
    if (frmWindow.location.href.indexOf(gears) > 0)//not loaded
        return;

    var pageUrl = iw_url;
    if (pageUrl.indexOf("?") > 0)
        pageUrl = pageUrl.substring(0, pageUrl.indexOf("?"));
    if (forceClose || frmWindow.location.href.indexOf(pageUrl) < 0) {//closing
        IWhm(frm.parentNode.parentNode.parentNode.parentNode);
        frm.src = gears;
        if(typeof(newValue) != "undefined" && newValue != null)
            iw_valuetxt.value = newValue;
    }
    else {//just opened, adjust position
        var ph = IWDBpageHeight();
        var ht = IWDBpageWinHeight(frmWindow);
        var wt = IWDBpageWinWidth(frmWindow);
        if (ht > ph - 40)
            ht = ph - 40;
        frm.height = ht;
        frm.parentNode.parentNode.parentNode.style.height = ht + 10;
        frm.width = wt;
        frm.parentNode.parentNode.parentNode.style.width = wt + 10;
        IWPositionPopup(frm.parentNode, wt, ht);
    }
}

function ToggleConditions(checkboxID, clientID) {
    var checkBox = document.getElementById(checkboxID);
    var element = document.getElementById(clientID);

    if (checkBox == null || element == null)
        return;

    if (checkBox.checked)
        element.style.display = "block";
    else
        element.style.display = "none";
}



