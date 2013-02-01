//TODO: move all scripts to this one js file
document.getElementsByClassName = function (cl, elTag) {
    var retnode = [];
    var myclass = new RegExp('\\b' + cl + '\\b');
    if (elTag == null) elTag = '*';
    var elem = this.getElementsByTagName(elTag);
    for (var i = 0; i < elem.length; i++) {
        var classes = elem[i].className;
        if (myclass.test(classes)) retnode.push(elem[i]);
    }
    return retnode;
};

var g_currentModalDialog = null;
function SLFE_CustomActionHandler(el) {
    var data = g_ButtonUICommnads[window.g_CUIcommandProperties.SourceControlId].split('|');
    var continueAction = true;
    if (data[0] != "") {
        continueAction = confirm(data[0]);
    }

    if (continueAction == true) {
        window.location.href = data[1];
    }
}
function SLFE_CustomActionRedirect(url, isRedirect) {
    if (window.frameElement == null || isRedirect != true)
        window.location.href = url;
    else
        window.frameElement.ownerDocument.location.href = url;
}
function SLFE_ShowWebUrlHelp() {
    g_currentModalDialog = window.showModelessDialog('/_layouts/blank.htm', '', 'dialogHeight: 400px; dialogWidth: 500px;  edge: Raised; center: Yes; help: No; resizable: No; status: No;');
    setTimeout(SLFE_ShowWebUrlHelpHTML, 100);

}
function SLFE_ShowWebUrlHelpHTML() {
    if ((g_currentModalDialog.document != null) && (g_currentModalDialog.document.title == '')) {
        g_currentModalDialog.document.title = 'Define dynamic source sites';
        var el = document.getElementById('SLFEHiddenWebUrlHelp');
        g_currentModalDialog.document.body.innerHTML = el.innerHTML;
        return;
    }
    setTimeout(SLFE_ShowWebUrlHelpHTML, 100);
}
function SLFE_RenderCustomActionsToolbarAsString(customActionsToolBarHTML, parentWebPartWPQ) {
    var customActionsToolbarHtml = customActionsToolBarHTML;
    var parentControl = document.getElementById('WebPart' + parentWebPartWPQ);
    if (customActionsToolbarHtml != "" && parentControl) {
        var dv = document.createElement('div');
        dv.innerHTML = customActionsToolbarHtml;
        parentControl.insertBefore(dv, null);
    }
}

function SLFE_RenderCustomActionsToolbar(customActionsToolBarClientID) {
    var customActionsToolbar = document.getElementById(customActionsToolBarClientID);
    if (customActionsToolbar) {
        var toolbarTable = customActionsToolbar.parentNode;
        while (toolbarTable.tagName != 'TABLE' && toolbarTable.parentNode != null)
            toolbarTable = toolbarTable.parentNode;
        if (toolbarTable)
            toolbarTable.parentNode.insertBefore(customActionsToolbar, toolbarTable);
    }
}

function SLFE_MoveCustomActionsToolbar(customActionsToolBarClientID) {
    var customActionsToolbar = document.getElementById(customActionsToolBarClientID);
    if (customActionsToolbar) {
        var allTables = document.getElementsByTagName('TABLE');
        var toolbarTable = null;
        for (var i = 0; i < allTables.length; i++) {
            if (allTables.item(i).className == 'ms-toolbar') {
                toolbarTable = allTables.item(i);
                break;
            }
        }
        if (toolbarTable) {
            toolbarTable.parentNode.insertBefore(customActionsToolbar, toolbarTable);
            var row = document.getElementById('idAttachmentsRow');
            var table = document.getElementsByClassName('table-toModify');
            var clone = row.cloneNode(true);
            row.parentNode.removeChild(row);
            clone.id = 'idAttachmentsRow';
            table[0].getElementsByTagName('TBODY').item(0).appendChild(clone);
        }
    }
}
function SLFE_SethdnChangedControls(hdnChangedControlsClientID, fieldName, fieldValue) {
    var hdnCtrl = document.getElementById(hdnChangedControlsClientID);
    if (hdnCtrl == null) return;
    else hdnCtrl.value += ';' + fieldName + '|' + fieldValue;

    __doPostBack(g_SLFEUpdatePanelHelper, hdnChangedControlsClientID);
}

function SLFE_SetDefaults(clientDefaults, clientSetLookupMatch) {
    if (clientDefaults != '' && clientDefaults != null) {
        var dfltPairs = clientDefaults.split(';');
        for (i = 0; i < dfltPairs.length; i++) {
            if (dfltPairs[i] == null || dfltPairs[i] == '') continue;
            var DefaultValueSet = false;

            var id = dfltPairs[i].split('|')[0];
            var val = dfltPairs[i].split('|')[1].toLowerCase();

            var inputCtrls = document.getElementsByTagName('input');
            for (j = 0; j < inputCtrls.length; j++) {
                if (inputCtrls[j].id.indexOf(id) != -1) {
                    var label = inputCtrls[j].parentElement.getElementsByTagName('label')[0].innerText;
                    if (label.toLowerCase() == val)
                        inputCtrls[j].checked = true;
                    else
                        inputCtrls[j].checked = null;
                }
            }
            if (DefaultValueSet) continue; //dont check select controls...

            var selectCtrls = document.getElementsByTagName('select');
            for (j = 0; j < selectCtrls.length; j++) {
                if (selectCtrls[j].id.indexOf(id) != -1) {
                    var options = selectCtrls[j].getElementsByTagName('option');
                    for (k = 0; k < options.length; k++) {
                        if (options[k].text.toLowerCase() == val)
                            options[k].selected = '1';
                        else
                            options[k].selected = null;
                    }
                }
            }
        }
    }
    try {
        if (clientSetLookupMatch != '' && clientSetLookupMatch != null) {
            var dfltPairs = clientSetLookupMatch.split(';');
            for (i = 0; i < dfltPairs.length; i++) {
                if (dfltPairs[i] == null || dfltPairs[i] == '') continue;
                var id = dfltPairs[i];
                var inputCtrls = document.getElementsByTagName('input');
                for (j = 0; j < inputCtrls.length; j++) {
                    if (inputCtrls[j].id.indexOf(id) != -1) {
                        var ctrl = inputCtrls[j];
                        var str = ctrl.value;
                        EnsureSelectElement(ctrl, ctrl.opt);
                    }
                }
            }
        }
    }
    catch (e) {
    }
}
function SLFE_SetTabMouseOver(el, elClass) {
    el.setAttribute('regcssclass', el.className);
    el.className = el.className + " " + elClass;
}
function SLFE_SetTabMouseOut(el) {
    var regcssclass = el.getAttribute('regcssclass');
    el.className = regcssclass;
}

//Key = tab name, value = tab control id
var SLFE_TabToElementIDHash = {};

var g_selectedTab = "";
function SLFE_SelectTab(tabName) {
    var requestResultTabsInfoHidden = $get(g_RequestResultTabsInfoHidden);

    var data = new SLFE_QuerystringFormated(requestResultTabsInfoHidden.value);
    if (tabName == null || tabName == "")
        tabName = data.get("currenttab");
    else tabName = unescape(tabName).replace(/\+/g, " ");

    try {
        if (SLFE_TabHideEmpty) {
            for (var i = 1; i < data.length; i++) {
                //get tab name
                var tabname = data.paramsnames[i];
                //get tab fields
                var tabfields = data.get(tabname, "");

                var selID = SLFE_TabToElementIDHash[tabname];
                if (selID != undefined) {
                    el = document.getElementById(selID);
                    if (el != null) {
                        //if fields empty - hide tab
                        if (tabfields == null || tabfields == "")
                            el.style.display = "none";
                        else
                            el.style.display = "";
                    }
                }
            }
        }
    } catch (e) { }

    if (tabName != "dummy") {

        if (tabName == "")
            tabName = g_selectedTab;
        var el = null;
        if (g_selectedTab != "") {
            var selID = SLFE_TabToElementIDHash[g_selectedTab];
            el = document.getElementById(selID);
            el.className = el.getAttribute('realclass');
        }
        g_selectedTab = tabName;
        var selID = SLFE_TabToElementIDHash[g_selectedTab];
        el = document.getElementById(selID);
        if (el != null) {
            if (el.className.indexOf("ms-cui-") > -1) {
                el.setAttribute('regcssclass', el.className);
                el.className += ' ms-cui-tt-s';
            }
            else {
                el.className = el.getAttribute('realclass') + ' ms-topnavselected';
                el.setAttribute('regcssclass', el.className);
            }
        }
    }
    else {
        g_selectedTab = tabName;
    }

    if (typeof (allFieldsArray) != "undefined" && allFieldsArray != null) {
        for (var i = 0; i < allFieldsArray.length; i++) {
            document.getElementById(allFieldsArray[i]).style.display = 'none';
        }
    }

    var table = null;
    var tabRowIndex = 0;
    if (g_selectedTab != "") {
        var hdnCurrentTab = $get(hdnCurrentTabFieldID);
        var isTabContainsAttachmentField = false;
        hdnCurrentTab.value = g_selectedTab;

        var selectedTabArray = data.get(g_selectedTab).split("|");

        for (var i = 0; i < selectedTabArray.length; i++) {
            if (selectedTabArray[i] != "") {
                if (selectedTabArray[i] == 'Attachments~Show') isTabContainsAttachmentField = true;

                if (document.getElementById(selectedTabArray[i]) != null) {
                    var theTr = document.getElementById(selectedTabArray[i]);
                    if (table == null) { //Added && theTr !=null
                        do {
                            table = table == null ? theTr.parentNode : table.parentNode;

                        } while (table.tagName.toLowerCase() != "table");
                        for (var rI = 0; rI < table.rows.length; rI++) {
                            if (table.rows[rI].id == "TabsControl") {
                                tabRowIndex = rI;
                                break;
                            }
                        }

                    }
                    var from = theTr.rowIndex;
                    var to = i + tabRowIndex + 1;
                    try {
                        if (table.rows[from] != null && table.rows[to] != null) {
                            try {
                                table.moveRow(from, to);
                            }
                            catch (e) {
                                var tbody = table.tBodies[0];
                                var trFrom = tbody.rows[from];
                                tbody.removeChild(trFrom);
                                var trTo = tbody.rows[to];
                                tbody.insertBefore(trFrom, trTo);
                            }
                        }
                    } catch (x) { }
                    theTr.style.display = '';
                }
            }

        }

        // Attachments show/hide
        if (!isTabContainsAttachmentField) {
            var elmAttachmentRow = document.getElementById("idAttachmentsRow")
            if (elmAttachmentRow != null) elmAttachmentRow.style.display = 'none';
        }
        else {
            var elm = document.getElementById("idAttachmentsTable");
            var elmAttachmentRow = document.getElementById("idAttachmentsRow")
            if ((elm != null && elm.rows.length > 0) && (elmAttachmentRow != null))
                elmAttachmentRow.style.display = '';
        }

    }
    else {
        for (var ii = 0; ii < data.length; ii++) {
            if (data.getvalueat(ii) == "")
                continue;
            var tabArray = data.getvalueat(ii).split("|");

            for (var i = 0; i < tabArray.length; i++) {
                if (tabArray[i] != "") {
                    var theTr = document.getElementById(tabArray[i]);
                    if (theTr != null)
                        theTr.style.display = '';
                }
            }
        }
    }

    //for 2010 - resize dialog when changing tabs
    //debugger;
    try {
        if (window.frameElement != null) {
            window.frameElement.autoSize();
            //issue #1811
            var bodyHeight = $(top.document.body).height();
            var dialogMaxHeight = bodyHeight - 60;
            //if dialog is too high
            if ($(window.frameElement).height() > dialogMaxHeight) {
                //move dialog to top
                //top.document.getElementsByClassName("ms-dlgContent")[0].style.top = "8px";
                //top.document.getElementsByClassName("ms-dlgContent")[0].previousSibling.style.top = "8px";
                $(".ms-dlgContent", top.document)[0].style.top = "8px";
                $(".ms-dlgContent", top.document)[0].previousSibling.style.top = "8px";
                $("#s4-workspace")[0].style.height = dialogMaxHeight - 200 + "px";

                //resize all dialog divs/frames/whatnot
                window.frameElement.style.height = dialogMaxHeight + "px";
                $(".ms-dlgContent", top.document)[0].previousSibling.style.height = dialogMaxHeight + "px";
                $(".ms-dlgContent", top.document)[0].style.height = dialogMaxHeight + 31 + "px";
                $(".ms-dlgBorder", top.document)[0].style.height = dialogMaxHeight + 32 + "px";
            }
            //end issue #1811 fix
        }
    }
    catch (e) { }

    //move and resize update progress panel
    try { moveToParentTd(); }
    catch (e) { }
}
function SLFE_SelectNext(currentTab) {
    var tabCtrl = document.getElementById('SLFE_TabControl');
    if (tabCtrl != null) {
        var tabs = tabCtrl.getElementsByTagName('a');
        for (var i = 0; i < tabs.length; i++) {
            if (tabs[i].innerHTML == currentTab) {
                if (i < tabs.length - 1)
                    SLFE_SelectTab(tabs[i + 1].innerHTML);
                else
                    SLFE_SelectTab(tabs[i].innerHTML);
                return;
            }
        }
    }
}

function SLFE_SelectPrev(currentTab) {
    var tabCtrl = document.getElementById('SLFE_TabControl');
    if (tabCtrl != null) {
        var tabs = tabCtrl.getElementsByTagName('a');
        for (var i = 0; i < tabs.length; i++) {
            if (tabs[i].innerHTML == currentTab) {
                if (i > 0)
                    SLFE_SelectTab(tabs[i - 1].innerHTML);
                else
                    SLFE_SelectTab(tabs[i].innerHTML);
                return;
            }
        }
    }
}

function SLFE_HideAttachments() {
    SLFE_DisableAttachments();
    var elem = document.getElementById('idAttachmentsRow');
    if (elem != null)
        elem.style.display = 'none';
}

function SLFE_DisableAttachments() {
    var tbl = document.getElementById('idAttachmentsTable');
    if (tbl == null)
        return;

    var rows = tbl.getElementsByTagName('tr');
    for (i = 0; i < rows.length; i++) {
        var tds = rows[i].getElementsByTagName('td');
        if (tds.length > 1)
            tds[1].style.display = 'none';
    }

    var elems = document.getElementsByTagName('a');
    for (i = 0; i < elems.length; i++) {
        if (elems[i].id.match('Attach_LinkText') != null) {
            elems[i].parentElement.parentElement.parentElement.parentElement.parentElement.style.display = 'none';
            elems[i].href = '#';
            elems[i].onclick = null;
            break;
        }
    }
}

function TextBoxPriotiryKeyPress(elm) {
    try {
        var charCode = getEventKeyCode();

        if (charCode == "13") {
            UpdateOrder(elm);
            return false;
        }
        else {
            elm.hasChanged = true;
            return isNumberKey(charCode);
        }
    }
    catch (e) {
        alert(e.message);
    }
}
function getEventKeyCode() {
    if (window.event === undefined) {
        return e.which;
    }
    else {
        return event.keyCode;
    }
}
function isNumberKey(charCode) {
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    else
        return true;
}

function SetFocuseToTextBox(uniqueId) {
    var spans = document.getElementsByTagName("span");

    for (var i = 0; i < spans.length; i++) {
        if (spans[i].name != "keyColumnSpans")
            continue;
        try {
            if (spans[i].firstChild.value == uniqueId) {
                spans[i].lastChild.focus();
                spans[i].lastChild.select();
                return;
            }
        }
        catch (e)
        { }
    }
}
function fireEvent(element, event) {
    try {
        if (document.createEventObject) {
            // dispatch for IE
            try {
                eval("element." + event + "();");
            }
            catch (e) {
                element.fireEvent("on" + event);
            }
        }
        else {
            // dispatch for firefox + others
            var isMouseEvent = (event == "click");
            if (isMouseEvent) {
                var evt = document.createEvent("MouseEvents");
                evt.initMouseEvent(event, true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
                element.dispatchEvent(evt);
            }
            else {
                var evt2 = document.createEvent("HTMLEvents");
                evt2.initEvent(event, true, true); // event type,bubbling,cancelable
                element.dispatchEvent(evt2);
            }
        }
    }
    catch (e) { }
}

function InvokeAction(btnId, hdnId, val, promptMsg) {
    if ((window.event === undefined) == false) {
        event.cancelBubble = true;
        event.returnValue = false;
    }
    //if (confirm("Are you sure you want to delete this condition?"))
    if (promptMsg != null && promptMsg != "")
        if (!confirm(promptMsg))
            return;

    var btn = document.getElementById(btnId);
    var hdn = document.getElementById(hdnId);
    //hdn.text = val;
    hdn.value = val;
    fireEvent(btn, "click");
}

function autoscroll() {
    var lists = document.getElementsByTagName('select');

    for (i = 0; i < lists.length; i++) {
        var lst = lists[i];
        if ((lst.multiple == true) && (lst.length > 0)) {
            var index = -1;
            for (var k = 0; k < lst.length; k++)
                if (lst.options[k].selected == true) {
                    index = k;
                    break;
                }
            if (index == -1)
                continue;

            if (lst.options[lst.length - 1].selected == true) {
                lst.options[lst.length - 1].selected = false;
                lst.options[lst.length - 1].selected = true;
            }
            else {
                lst.options[lst.length - 1].selected = true;
                lst.options[lst.length - 1].selected = false;
            }

            lst.options[index].selected = false;
            lst.options[index].selected = true;
        }
    }
}

function SLFE_OnClientResponseEnded(sender, args) {
    SLFE_SelectTab(g_selectedTab);
}


function SLFE_QuerystringFormated(qs) {
    this.params = {};
    this.paramsvalues = {};
    this.paramsnames = {};
    qs = qs.replace(/\+/g, ' ');
    var args = qs.split('&');

    for (var i = 0; i < args.length; i++) {
        var pair = args[i].split('=');
        var name = decodeURIComponent(pair[0]);

        var value = (pair.length == 2)
			? decodeURIComponent(pair[1])
			: name;

        this.params[name] = value;
        this.paramsvalues[i] = value;
        this.paramsnames[i] = name;
    }
    this.length = args.length;
}

SLFE_QuerystringFormated.prototype.get = function (key, default_) {
    if (default_ == null) default_ = "";
    var value = this.params[key];
    return (value != null) ? value : default_;
}
SLFE_QuerystringFormated.prototype.length = function () {
    return this.length;
}
SLFE_QuerystringFormated.prototype.getvalueat = function (index, default_) {
    if (default_ == null) default_ = "";
    var value = this.paramsvalues[index];
    return (value != null) ? value : default_;
}
SLFE_QuerystringFormated.prototype.getnameat = function (index, default_) {
    if (default_ == null) default_ = "";
    var value = this.paramsnames[index];
    return (value != null) ? value : default_;
}
SLFE_QuerystringFormated.prototype.contains = function (key) {
    var value = this.params[key];
    return (value != null);
}