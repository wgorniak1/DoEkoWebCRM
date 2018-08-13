'use strict';

//plugin to add options to select
(function( $ ) {

    $.fn.addOptions = function(array, selected) {
        
        if (!array || Object.values(array[ 0 ]).length < 2) {
            return this;
        } else {

            return this.filter("select").each(function () {
                var select = this;
                //$(array).each(function () {
                //    var oPropValues = Object.values(this);
                //    $('<option/>', oPropValues[0] === selected ? { value: oPropValues[0], selected: true } : { value: oPropValues[0] })
                //        .text(oPropValues[ 1 ])
                //        .appendTo(select);
                //});

                array.forEach(function (item) {
                    var option = document.createElement('option');

                    option.setAttribute('value', item.value);

                    if (item.value === selected.toString()) {
                        option.setAttribute('selected', '');
                    }

                    option.appendChild(document.createTextNode(item.text));
                    select.appendChild(option);
                });
            });
        }
 
    };
 
}( jQuery ));

class mySelect {
    ///optArray = [{value: "",text:""},{value: "",text:""},...]
    constructor(
        optArray = [{ value: "", text: "" }],
        setEmpty = true,
        defaultOpt = -1,
        labelText = "",
        idText = "" ) {
        
        var select = document.createElement("select"),
            div = document.createElement('div'),
            option,
            i = 0,
            il = optArray.length;
        //
        div.className = "form-group form-inline";
        select.setAttribute("id", idText);
        select.className = "form-control input-sm";

        //empty option
        if (setEmpty === true) {
            option = document.createElement('option');
            option.setAttribute('value', '');
            option.appendChild(document.createTextNode(''));
            select.appendChild(option);
        }

        for (; i < il; i += 1) {
            option = document.createElement('option');
            option.setAttribute('value', optArray[i].value);
            option.appendChild(document.createTextNode(optArray[i].text));
            select.appendChild(option);
        }

        if (labelText.length > 0) {
            var label = document.createElement('label');
            label.setAttribute('for', idText);
            label.appendChild(document.createTextNode(labelText));
            label.appendChild(select);
            div.appendChild(label);
        }
        else {

            div.appendChild(select);
        }

        this.domNode = div;
    }
}

class WgRSEEnum {

    constructor() {
        this.surveyType = ["CO", "CWU", "EE"];
        this.rseType = [["", "Pompa Ciepła Grunt.", "Kocioł na Pellet", "Pompa Ciepła Powietrzna"],["", "", "", "Solary", "Pompa Ciepła"],["","","","","","Panele Fotowolt."]];
    }
    
    getText(type, rseType, single) {
        if (rseType === undefined)
            return this.surveyType[type];
        else if (single)
            return this.rseType[type][rseType];
        else
            return this.rseType[type][rseType] + ' (' + this.surveyType[type] + ')';
    }

    
    getId(typeText, rseTypeText) {
        var type = -1;
        var rseType = -1;

        for (var i = 0; i < this.surveyType.length; i++)
        {
            if (this.surveyType[i] === typeText) type = i;
        }

        if (rseTypeText === undefined) {
            return type;
        }

        if (type >= 0) {
            for (var j = 0; j < this.rseType[type].length; j++) {
                if (this.surveyType[type][i] === typeText) rseType = i;
            }
        }

        return rseType;
    }

    asKeyValueArray(withIds = false) {
        var keyValues = [];

        for (var i = 0; i < this.surveyType.length; i++) {
            var type = this.surveyType[i];

            this.rseType[i].forEach(function (value, index) {
                if (value.length > 0) {
                    keyValues.push(withIds === true ?
                        { value: i.toString().concat(index.toString()), text: value + ' (' + type + ')' } :
                        { value: value + ' (' + type + ')', text: value + ' (' + type + ')' } );
                }
            });            
        }

        return keyValues;
    }
}
class WgLocEnum{
    constructor() {
        this._array = ["Dach","Grunt","Elewacja"];
    }
    getText(value) {
        return this._array[value];
    }
    getValue(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }

    asKeyValueArray(withIds = false) {
        var keyValues = [];

        this._array.forEach(function (value, index) {
            if (value.length > 0) {
                keyValues.push(withIds === true ?
                    { value: index.toString(), text: value } :
                    { value: value,            text: value });
            }
        });

        return keyValues;
    }
}
class WgBdgEnum{
    constructor() {
        this._array = ["Gospodarczy", "Mieszkalny"];
    }
    Text(value) {
        return this._array[value];
    }
    Value(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }

    asKeyValueArray(withIds = false) {
        var keyValues = [];

        this._array.forEach(function (value, index) {
            if (value.length > 0) {
                keyValues.push(withIds === true ?
                    { value: index.toString(), text: value } :
                    { value: value, text: value });
            }
        });

        return keyValues;
    }

}

//class KeyValuePair {
//    constructor(key,value) {
//        this._id = key;
//        this._value = value;
//    }

//    get id() { return this._id }
//    get value() { return this._value }
//}

//class SimpleEnum {
//    constructor(array = []) {
//        this._array = array;
//    }

//    Text(id = 0) {
//        return (this._array.length > 0) ? this._array[id] : null;
//    }

//    Value(text = "") {
//        return this._array.indexOf(text);
//    }

//    get KeyValuePairs() {
//        var kvp = new ();
//    }
//}

//const WG_RANGE_MIN = (double)0;
//const 

var WgEnums = { rse: new WgRSEEnum(), localization: new WgLocEnum(), building: new WgBdgEnum() };

var WgTaxTableEdit = true;
var WgNetTableEdit = true;


$(document).ready(function () {
    var projectId = $("#TaxTable").data('projectid');
    InitializeTaxTab(projectId);

    InitializeNetTab(projectId);

    $('#TabsContainer ul li a').click(function () {
        var link = $(this).attr('href');
        var id = $('table', $(link)).attr('id');
        var table = $('#' + id).dataTable().api();

        table.fixedHeader.adjust();
        //new $.fn.dataTable.FixedHeader(table, {
        //    headerOffset: $('#NavBarMain').outerHeight()
        //});

    });
    //table.on('responsive-display', function (e, datatable, row, showHide, update) {
    //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');  
    //});
});


function onResetModalCancel() {

    //
    var modal = $("#ResetModal");
    var section = modal.data("section");
    modal.data("section", "");

    //
    if (section === "tax") {
        var table = $('#TaxTable').DataTable();

        var loading = $('div#TaxTable_processing');
        loading.hide();
    }
    else {
        table = $('#NetTable').DataTable();

        var loading = $('div#NetTable_processing');
        loading.hide();

    }

    table.buttons().enable();
}
function onResetModalSubmit() {

    //
    var modal = $("#ResetModal");
    var section = modal.data("section");
    modal.data("section", "");
    modal.modal('hide');

    var $table,
        table,
        ajaxUrl;

    if (section === "tax") {
        $table = $("#TaxTable");
        table = $table.DataTable();
        ajaxUrl = "/api/v1/RSERules/Tax?projectId=" + $table.data('projectid');

    }
    else if (section === "net") {
        $table = $("#NetTable");
        table = $table.DataTable();
        ajaxUrl = "/api/v1/RSERules/Price?projectId=" + $table.data('projectid');
    }
    else {
        //
        WgTools.alert("Coś poszło nie tak. Proszę skontaktować się z administratorem strony.", false, 'E');
        return;
    }

    //Call API
    var call = $.ajax({
        url: ajaxUrl,
        type: 'DELETE',
        data: {},
        contentType: 'application/json'
    });

    call.done(function (result) {
        table.buttons().enable();
        table.buttons(['btnDivide:name', ['btnJoin:name']]).disable();
        table.ajax.reload();
        table.rows().cells().invalidate().render();
        WgTools.alert("Pomyślnie przywrócono wartości początkowe.", true, 'S');

        var selectRSEType      = $("#searchRSEType").val("");
        var selectLocalization = $("#searchLocalization").val("");
        var selectBuilding = $("#searchBuilding").val("");
        table.column(0).search('').draw();
        table.column(1).search('').draw();
        table.column(2).search('').draw();


    });

    call.fail(function (result) {
        WgTools.alert("Coś poszło nie tak. Proszę skontaktować się z administratorem strony.", false, 'E');
    });
    
}
function onTableDataChanged(event) {

    //update data from form
    var dt = event.data.dt;
    var fieldName  = $(this).attr("name");
    var fieldValue = $(this).val();

    var row = dt.row($(this).parents('tr'));
    var rowData = row.data();

    switch (fieldName) {
        case "usableAreaMin":
            rowData.usableAreaMin = parseFloat(fieldValue);
            break;
        case "usableAreaMax":
            
            rowData.usableAreaMax = fieldValue === '99999.99' ? Number.MAX_VALUE : parseFloat(fieldValue);
            break;
        case "vat":
            rowData.vat = parseInt(fieldValue);
            break;
        case "unit":
            rowData.unit = parseInt(fieldValue);

            dt.rows(function (index, data, node) {
                    return data.surveyType === rowData.surveyType &&
                        data.rseType === rowData.rseType ? true : false;
                })
                .every(function (index, tableLoop, rowLoop) {
                    this.data().unit = parseInt(fieldValue);
                    this.invalidate();
                })
                .draw();

            break;
        case 'numberMin':
            rowData.numberMin = parseFloat(fieldValue);
            break;
        case 'numberMax':
            rowData.numberMax = fieldValue === '99999.99' ? Number.MAX_VALUE : parseFloat(fieldValue);
            break;
        case 'netPrice':
            rowData.netPrice = parseFloat(fieldValue);
            break;
        case 'multiply':
            rowData.multiply = (fieldValue === "on") ? true : false;
            break;
        default:
            break;
    }

    //dt.draw();

    ////everything has been done 
    //event.stopPropagation();


    if (dt.buttons('btnSave:name').length === 0) {
        
        //add save button since content has been changed
        dt.button().add(dt.buttons().length, {
            name: "btnSave",
            text: '<span class="text-primary glyphicon glyphicon glyphicon-save"></span> Zapisz',
            titleAttr: 'Zapamiętaj wprowadzone zmiany',
            className: 'btn',
            action: function (e, dt, node, config) {
                var remove = false;
                //
                dt.buttons().disable();

                //
                var data = dt.rows().data().toArray();
                var method;
                var projectId = $(dt.table().node()).data('projectid');

                if (data[0].projectId == '1') {

                    for (var i = 0; i < data.length; i++) {
                        data[i].ProjectId = projectId;
                    }
                    remove = true;
                    method = 'POST';
                }
                else {

                    method = 'PUT';

                }
                var ajaxUrl;
                var payload;
                if (dt.table().node().id == 'TaxTable') {

                    ajaxUrl = '/api/v1/RSERules/Tax';

                    payload = {
                        ProjectId: projectId,
                        TaxRules: data
                    };

                }
                else {
                    ajaxUrl = '/api/v1/RSERules/Price';
                    payload = {
                        ProjectId: projectId,
                        PriceRules: data
                    };

                }

                //Call API

                var call = $.ajax({
                    url: ajaxUrl,
                    type: method,
                    data: JSON.stringify(payload),
                    dataType: "json",
                    contentType: 'application/json'
                });

                call.always(function (jqXHR, textStatus, errorThrown) {
                    dt.buttons().enable();
                    dt.buttons(['btnDivide:name', 'btnJoin:name']).disable();
                });

                call.done(function (result) {
                    dt.ajax.reload();
                    dt.rows().cells().invalidate().render();
                    WgTools.alert("Pomyślnie zapisano ustawienia.", true, 'S');
                    dt.button('btnSave:name').remove();


                });

                call.fail(function (jqXHR, textStatus, errorThrown) {
                    if (remove) {
                        for (var i = 0; i < data.length; i++) {
                            data[i].ProjectId = '1';
                        }
                    }
                    switch (jqXHR.status) {
                        case 400:

                            var modelState = jqXHR.responseJSON;
                            var errorMessages = [];

                            errorMessages.push("Błąd walidacji danych:");

                            for (var field in modelState) {

                                modelState[field].forEach(function (v, i) {
                                    if ($.inArray(v, errorMessages) === -1)
                                        errorMessages.push(v);
                                });

                                var rowNumber = parseInt(field.match(/\d+/));

                                if (rowNumber !== NaN) {
                                    //rowNumber += 1;
                                    var $row = $(dt.row(rowNumber).node());
                                    $row.addClass('wg-bg-danger');
                                    //$('input, select, label', $row).addClass('bg-danger');
                                }
                            }

                            WgTools.alert(errorMessages, false, 'E');        
                            break;
                        case 401:
                            WgTools.alert("Brak połączenia z serwerem. Proszę odświeżyć stronę", false, 'E');
                            break;

                        default:
                            WgTools.alert("Coś poszło nie tak. Proszę skontaktować się z administratorem strony.", false, 'E');
                            break;
                    }

                });
            }
        });

    }
}
function onTableDataMaxChanged(event) {

    var dt = event.data.dt;
    var row = dt.row($(this).parents('tr'));

    //var min = $(this).parents('tr').next('tr').children('td').find('input.wg-area-min');
    //    min.val($(this).val());
    //});
}



function InitializeTaxTab(projectId) {

    var ajaxUrl = "/api/v1/RSERules/Tax?projectId=" + projectId + "&getDefaults=True";

    var table = $('#TaxTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: ""
        },
        columnDefs: [{
            className: 'text-right',
            targets: [3, 4, 5]
        }],
        columns: [
            {
                data: null,
                name: 'primaryKey',
                title: 'Klucz',
                type: 'string',
                visible: false,
                orderable: true,
                render: function (data, type, row, meta) {
                    var key =
                        data.surveyType.toString() +
                        data.rseType.toString() +
                        data.installationLocalization.toString() +
                        data.buildingPurpose.toString() +
                        (data.usableAreaMin * 100).toString();

                    return key;
                }
            },
            {
                data: null,
                name: "rseType",
                title: 'Typ OZE',
                type: 'html',
                orderable: false,
                render: function (data, type, row, meta) {
                    switch (type) {
                        //case 'display':
                        //    return WgEnums.rse.getText(data.surveyType, data.rseType);
                        //case 'filter':
                        default:
                            return WgEnums.rse.getText(data.surveyType, data.rseType);
                            //return data.surveyType.toString() + data.rseType.toString();
                    }
                }
            },
            {
                data: "installationLocalization",
                name: "installationLocalization",
                title: "Lokalizacja",
                orderable: false,
                render: function (data, type, row, meta) {
                    switch (type) {
                        //case 'display':
                        //case 'filter':
                        //    return WgEnums.localization.getText(data);
                        default:
                            return WgEnums.localization.getText(data);
                            //return data;
                    }
                }
            },
            {
                data: "buildingPurpose",
                name: "buildingPurpose",
                title: "Przezn. Budynku",
                orderable: false,
                render: function (data, type, row, meta) {
                    switch (type) {
                        //case 'display':
                        //case 'filter':
                        //    return WgEnums.building.Text(data);
                        default:
                            return WgEnums.building.Text(data);
                            //return data;
                    }

                }
            },
            {
                data: "usableAreaMin",
                name: "usableAreaMin",
                title: "Powierzchnia Od",
                type: "html-num",
                orderable: false,
                render: function (data, type, row, meta) {
                    switch (type) {
                        case 'display':
                            var content = '<input type="number" name="usableAreaMin" class="form-control input-sm wg-data wg-data-min" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
                            return WgTaxTableEdit === true ? content : data;
                        default:
                            return data;
                    }
                }
            },
            {
                data: "usableAreaMax",
                name: "usableAreaMax",
                title: "Powierzchnia Do",
                type: "html-num",
                orderable: false,
                render: function (data, type, row, meta) {
                    switch (type) {
                        case 'display':
                            var currentValue, minValue, maxValue, status;

                            if (data === Number.MAX_VALUE) {
                                currentValue = '99999.99';
                                minValue = '99999.99';
                                maxValue = '99999.99';
                                status = false;
                            }
                            else {
                                currentValue = data;
                                minValue = (row.usableAreaMin + 1).toString();
                                maxValue = '99999.99';
                                status = true;
                            }

                            var content =
                                '<input type="number" ' +
                                'class="form-control input-sm wg-data wg-data-max" ' +
                                'name="usableAreaMax" ' +
                                'value = "' + currentValue + '" ' +
                                'step = "0.1" ' +
                                'min = "' + minValue + '" ' +
                                'max = "' + maxValue + '" ' +
                                ((status === false) ? 'disabled> ' : '> ');


                            return WgTaxTableEdit === true ? content : currentValue;
                        //var currentValue = data === Number.MAX_VALUE ? '99999.99' : data;
                        //return currentValue;
                        default:
                            return (data === Number.MAX_VALUE) ? '99999.99' : data;
                    }
                }
            },
            {
                data: "vat",
                name: "vat",
                title: "VAT",
                type: "html-num-fmt",
                orderable: false,
                render: function (data, type, row, meta) {

                    switch (type) {
                        default:
                            var opt = [{ value: "0", text: "0 %" }, { value: "5", text: "5 %" }, { value: "8", text: "8 %" }, { value: "23", text: "23 %" }];
                            return $('<select>', { name: "vat", class: "form-control input-sm wg-data wg-data-tax", disabled: !WgTaxTableEdit })
                                .addOptions(opt, data)
                                .prop('outerHTML');

                            return data;
                    }

                }
            }
        ],
        stateSave: false,
        paging: false,
        language: WgLanguage,
        ordering: true,
        order: [['0', 'asc']],
        processing: true,
        serverSide: false,
        dom: "<'row wg-dt-padding'<'col-sm-12'B>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row wg-dt-padding'<'col-sm-12'i>>",
        buttons: [
            {
                text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Resetuj',
                titleAttr: 'Przywróć ustawienia domyślne',
                className: 'btn',
                name: 'btnReset',
                action: function (e, dt, node, config) {
                    //
                    dt.table().buttons().disable();
                    $('div#TaxTable_processing').show();

                    var modal = $("#ResetModal");
                    modal.data("section", "tax");
                    modal.modal('show');
                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-refresh"></span> Odśwież',
                titleAttr: 'Odśwież zawartość z bazy danych',
                className: 'btn',
                name: 'btnRefresh',
                action: function (e, dt, node, config) {
                    dt.ajax.reload();
                    dt.rows().cells().invalidate().render();
                    dt.button('btnSave:name').remove();
                }
            },
            {
                extend: 'selectedSingle',
                text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Podziel',
                titleAttr: 'Podziel przedział powierzchni',
                className: 'btn',
                name: 'btnDivide',
                action: function (e, dt, node, config) {

                    var currentRow = dt.row({ selected: true, page: 'current' });

                    if (currentRow === undefined)
                        return;

                    var rowData = currentRow.data();

                    //new Max for currentRow
                    var newUsableAreaMax = rowData.usableAreaMax === Number.MAX_VALUE ? '99999.99' : rowData.usableAreaMax;
                    newUsableAreaMax = newUsableAreaMax - (newUsableAreaMax - rowData.usableAreaMin) / 2;


                    //add new row
                    var nodeNew = table.row
                        .add({
                            project: null,
                            projectId: rowData.projectId,
                            surveyType: rowData.surveyType,
                            rseType: rowData.rseType,
                            installationLocalization: rowData.installationLocalization,
                            buildingPurpose: rowData.buildingPurpose,
                            usableAreaMin: newUsableAreaMax,
                            usableAreaMax: rowData.usableAreaMax,
                            vat: rowData.vat
                        })
                        .invalidate()
                        .draw()
                        .node();

                    //update max in current row
                    rowData.usableAreaMax = newUsableAreaMax;

                    var nodeCurrent = currentRow
                        .invalidate()
                        .draw()
                        .node();

                    $('input.wg-data', nodeCurrent).trigger('change');
                }
            },
            {
                extend: 'selectedSingle',
                text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Usuń',
                titleAttr: 'Usuń przedział powierzchni',
                name: 'btnJoin',
                className: 'btn',
                action: function (e, dt, node, config) {

                    var currentRow = dt.row({ selected: true, page: 'current' });

                    if (currentRow === undefined)
                        return;

                    var rowData = currentRow.data();

                    //if (rowData.usableAreaMax === Number.MAX_VALUE ) {
                    //    return;
                    //}

                    $('input.wg-data', currentRow.node() ).trigger('change');

                    currentRow.remove().draw();

                }
            }

        ],
        select: 'single',
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings) {
            $('#TaxTable thead tr th')
                .each(function (i) {
            });

            var dt = this.api();
            var indexes = dt.rows().indexes();

            $('#TaxTable tbody tr').each(function (i) {
                var currentRow = dt.row($(this));
                var currentRowIndex = indexes.indexOf(currentRow.index());

                var prevRow = dt.rows(currentRowIndex - 1).data();
                var nextRow = dt.rows(currentRowIndex + 1).data();

            });
        }
    });

    //activate save button upon data change
    $('#TaxTable').on('change', 'input.wg-data, select.wg-data', { dt: $("#TaxTable").dataTable().api() }, onTableDataChanged);

    //add dropdown filters 
    $('#TaxTable thead tr th').each(function (i) {

        var title = $(this).text() + ": ";
        switch (i) {
            case 0:
                //new select node
                var mySel1 = new mySelect(WgEnums.rse.asKeyValueArray(false), true, -1, title, "searchRSEType");
                $(this).html(mySel1.domNode);
                break;
            case 1:
                var mySel1 = new mySelect(WgEnums.localization.asKeyValueArray(false), true, -1, title,"searchLocalization");
                $(this).html(mySel1.domNode);
                break;
            case 2:
                var mySel1 = new mySelect(WgEnums.building.asKeyValueArray(false), true, -1, title,"searchBuilding");
                $(this).html(mySel1.domNode);

                break;
            default:
                var div = document.createElement('div');
                div.className = "form-group form-inline";
                var label = document.createElement('p');
                label.appendChild(document.createTextNode(title));
                label.className = "form-control-static";
                div.appendChild(label);
                
                $(this).html(div);
        }
        

        $('select', this).on('change', function () {
            if (table.column(i+1).search() !== this.value) {
                table
                    .column(i+1)
                    .search(this.value)
                    .draw();
            }
        });
    });

    //control min-max
    $('#TaxTable').on('change', 'input.wg-data-max', { dt: $("#TaxTable").dataTable().api() }, onTableDataMaxChanged);


    $('body').on('click', 'button.resetmodal-btn-cancel', onResetModalCancel);
    $('body').on('click', 'button.resetmodal-btn-submit', onResetModalSubmit);

    $('#TaxTable tbody').on('click', 'input, select, label', table, function (event) {
        //when clicking on input or select or label (toggle checkbox)
        //don't select the row
        event.stopPropagation();

        //if ($(this).hasClass('toggle-off') || $(this).hasClass('toggle-on') && $(this).hasClass('active')) {
        //    var val = ($(this).prop('checked') === 'true');

        //    $(this).bootstrapToggle(val === true ? 'off' : 'on');
        //}
    });

}

function InitializeNetTab(projectId) {
    var ajaxUrl = "/api/v1/RSERules/Price?projectId=" + projectId + "&getDefaults=True";
    var table = $('#NetTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: ""
        },
        columns: [
                    {
                        data: null,
                        name: 'primaryKey',
                        title: 'Klucz',
                        type: 'string',
                        visible: false,
                        orderable: true,
                        render: function (data, type, row, meta) {
                            var number = (data.numberMin * 100).toFixed(0);
                            var leadingzeros = ('000000000' + number).substr(-10);
                            var key =
                                data.surveyType.toString() +
                                data.rseType.toString() +
                                data.unit.toString() +
                                leadingzeros;
                            return key;
                        }
                    },
                    {
                        data: null,
                        name: 'rseType',
                        title: 'Typ OZE',
                        type: 'html',
                        orderable: false,
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':

                                    //var div = document.createElement('div');
                                    //div.className = "form-group form-inline";
                                    //var label = document.createElement('p');
                                    //label.appendChild(document.createTextNode(WgEnums.rse.getText(data.surveyType, data.rseType)));
                                    //label.className = "form-control-static";
                                    //div.appendChild(label);
                            
                                    //return div.outerHTML();

                                    return '<div class="form-group form-inline">' +
                                        '<p class="form-control-static">' + WgEnums.rse.getText(data.surveyType, data.rseType) +
                                        '</p></div>'

                                default:
                                    return WgEnums.rse.getText(data.surveyType, data.rseType);
                            }
                        }
                    },
                    {
                        data: "unit",
                        name: "unit",
                        title: "Jednostka",
                        type: 'html',
                        orderable: false,
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':

                                    var opt = [{ value: "1", text: "Moc ostateczna" }, { value: "2", text: "Liczba zestawów" }];

                                    var a = $('<select />', { name: 'unit', class: 'form-control input-sm wg-data wg-data-unit', disabled: !WgNetTableEdit })
                                            .addOptions(opt, data + '')
                                            .addClass(WgNetTableEdit === true ? '' : 'disabled');

                                    return a.prop('outerHTML');


                                default:
                                    return data;
                            }
                        }

                    },
                    {
                        data: "numberMin",
                        name: "numberMin",
                        title: "Od",
                        type: "html-num",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content = '<input type="number" name="numberMin" class="form-control input-sm wg-data wg-data-min" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }

                    },
                    {
                        data: "numberMax",
                        name: "numberMax",
                        title: "Do",
                        type: "html-num",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            var currentValue = (data === Number.MAX_VALUE) ? '99999.99' : data;

                            switch (type) {
                                case 'display':
                                    var step = row.unit === 1 ? 0.1 : 1;
                                    //var content = '<input type="number" style="width: 100%; min-width: 30px; max-width: 100px;" name="numberMax" class="form-control input-sm number-max" value="' + data + '" step="' + step + '" min="0.0" max="99999.9">';
                                    var content = '<input type="number" name="numberMax" class="form-control input-sm wg-data wg-data-max" value="' + currentValue + '" step="' + step + '" min="0.0" max="99999.9">';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return currentValue;
                            }
                        }
                    },
                    {
                        data: "netPrice",
                        name: "netPrice",
                        title: "Cena Netto",
                        orderable: false,
                        type: "html-num",
                        render: function(data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content;
                                   // content = '<div class="input-group">'; 
                                   // content += '<span class="input-group-addon">zł</span>';
                                    content = '<input type="number" name="netPrice" min="0" max="99999" step="1" class="form-control input-sm wg-data wg-data-price" value="'+ data +'"/>';
                                   // content += '</div>';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }
                    },
                    {
                        data: "multiply",
                        name: "multiply",
                        title: "cena jednostkowa",
                        orderable: false,
                        type: "html",
                        render: function(data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content = '<div class="form-group form-inline">' +
                                                        '<input name="multiply"  class="form-control input-sm checkbox wg-data wg-data-multiply"  data-toggle="toggle" data-on="Tak" data-off="Nie" type="checkbox"';
                                    content += data === true ? ' checked />' : ' />';
                                    content += '</div>';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }


                    }
        ],
        stateSave: false,
        paging: false,
        serverSide: false,
        language: WgLanguage,
        ordering: true,
        order: [[0, "asc"]],
        processing: true,
        dom: "<'row wg-dt-padding'<'col-sm-12'B>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row wg-dt-padding'<'col-sm-12'i>>",
        buttons: [
            {
                text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Resetuj',
                titleAttr: 'Przywróć ustawienia domyślne',
                className: 'btn',
                name: 'btnReset',
                action: function (e, dt, node, config) {
                    //
                    dt.table().buttons().disable();
                    $('div#NetTable_processing').show();

                    var modal = $("#ResetModal");
                    modal.data("section", "net");
                    modal.modal('show');

                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-refresh"></span> Odśwież',
                titleAttr: 'Odśwież zawartość z bazy danych',
                className: 'btn',
                name: 'btnRefresh',
                action: function (e, dt, node, config) {

                    $('div#NetTable_processing').show();

                    dt.ajax.reload();
                    dt.rows().cells().invalidate().render();
                    dt.button('btnSave:name').remove();

                    WgTools.alert("Odświeżanie zakończone", true, 'S');
                }
            },
            {
                extend: 'selectedSingle',
                text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Podziel',
                titleAttr: 'Podziel przedział',
                className: 'btn',
                name: 'btnDivide',
                action: function (e, dt, node, config) {

                    var currentRow = dt.row({ selected: true, page: 'current' });

                    if (currentRow === undefined)
                        return;

                    var rowData = currentRow.data();

                    //new Max for currentRow
                    var newNumberMax = rowData.numberMax === Number.MAX_VALUE ? '99999.99' : rowData.numberMax;
                    newNumberMax = newNumberMax - (newNumberMax - rowData.numberMin) / 2;

                    //add new row
                    var nodeNew = table.row
                        .add({
                            project: null,
                            projectId: rowData.projectId,
                            surveyType: rowData.surveyType,
                            rseType: rowData.rseType,
                            unit: rowData.unit,
                            numberMin: newNumberMax,
                            numberMax: rowData.numberMax,
                            netPrice: rowData.netPrice,
                            multiply: rowData.multiply
                        })
                        .invalidate()
                        .draw()
                        .node();

                    //update max in current row
                    rowData.numberMax = newNumberMax;

                    var nodeCurrent = currentRow
                        .invalidate()
                        .draw()
                        .node();

                    $('input.wg-data', nodeCurrent).trigger('change');
                }
            },
            {
                extend: 'selectedSingle',
                text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Usuń',
                titleAttr: 'Usuń przedział',
                name: 'btnJoin',
                className: 'btn',
                action: function (e, dt, node, config) {

                        var currentRow = dt.row({ selected: true, page: 'current' });

                        if (currentRow === undefined)
                            return;

                        var rowData = currentRow.data();

                        //if (rowData.usableAreaMax === Number.MAX_VALUE ) {
                        //    return;
                        //}

                        $('input.wg-data', currentRow.node()).trigger('change');

                        currentRow.remove().draw();

                }
            },

                        //{
                        //    text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Podziel',
                        //    titleAttr: 'Podziel przedział wartości',
                        //    className: 'btn',
                        //    action: function (e, dt, node, config) {
                                

                        //        var curRow = dt.row({ selected: true, page: 'current' });
                        //        var index = curRow.index();
                                

                        //        //calculate new value                        
                        //        var curRowData = curRow.data();
                        //        var numberDivisor = curRowData.numberMax - (curRowData.numberMax - curRowData.numberMin) / 2;
                                
                        //        // add new row
                        //        var newRow = dt.row.add({
                        //            project: null,
                        //            projectId: curRowData.projectId,
                        //            surveyType: curRowData.surveyType,
                        //            rseType: curRowData.rseType,
                        //            unit: curRowData.unit,
                        //            numberMin: numberDivisor,
                        //            numberMax: curRowData.numberMax,
                        //            netPrice: curRowData.netPrice,
                        //            multiply: curRowData.multiply
                        //        });


                        //        //update value in current row
                        //        curRowData.numberMax = numberDivisor;

                        //        //redraw table so that new DOM elements are present
                        //        newRow.invalidate();
                        //        curRow.invalidate().draw();
                        //    }
                        //},
                        //{
                        //    text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-small"></span> Usuń',
                        //    titleAttr: 'Usuń przedział wartości',
                        //    className: 'btn',
                        //    action: function (e, dt, node, config) {

                        //        var curRow = dt.row({ selected: true, page: 'current' });
                        //        if  (curRow === undefined){
                        //            return;
                        //        }
                        //        var curRowData = curRow.data();
                        //        //                                
                        //        if (curRowData.numberMin === 0)
                        //            return;
                                
                        //        var prevRows = dt.rows(function (index, data, node) {
                        //            if (data.surveyType === curRowData.surveyType &&
                        //                data.rseType === curRowData.rseType &&
                        //                data.numberMax < curRowData.numberMax)
                        //                return true;
                        //            else
                        //                return false;
                        //        });
                        //        if (prevRows.length > 0) {
                        //            prevRows.data()[prevRows.count() - 1].numberMax = curRowData.numberMax;
                        //            prevRows.invalidate();
                        //        }

                        //        curRow.remove().draw();

                        //    }
                        //}

        ],
        select: 'single',
        responsive:  true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings) {
            var dt = this.api();

            //
            $('#NetTable thead tr th')
                .each(function (i) {
                });
            //
            var indexes = dt.rows().indexes();
            $('#NetTable tbody tr').each(function (i) {
                var currentRow = dt.row($(this));
                var currentRowIndex = indexes.indexOf(currentRow.index());

                var prevRow = dt.rows(currentRowIndex - 1).data();
                var nextRow = dt.rows(currentRowIndex + 1).data();
            });

            //
            var tableBody = $("#NetTable tbody");
            //var table = $("#NetTable").dataTable().api();

            //activate toggles
            $('input[data-toggle="toggle"]',tableBody).bootstrapToggle();
            

            //for (var i = 0; i < dt.rows().count(); i++) {
            //    var curRow = table.row(i);
            //    var curRowData = curRow.data();

            //    var nextRowData = table.row(function (index, data, node) {
            //        if (data.surveyType === curRowData.surveyType &&
            //            data.rseType === curRowData.rseType &&
            //            data.numberMax > curRowData.numberMax)
            //            return true;
            //        else
            //            return false;
            //    }).data();
            //    if (nextRowData !== undefined) {
            //        var maxValue = nextRowData.numberMax >= Number.MAX_VALUE ? 99999.98 : Number.parseFloat(nextRowData.numberMax) - 0.1;
            //        $('input[name="numberMax"]', $(curRow.node())).attr('max', maxValue);
            //    }

            //    var prevRowData = table.rows(function (index, data, node) {
            //        if (data.surveyType === curRowData.surveyType &&
            //            data.rseType === curRowData.rseType &&
            //            data.numberMax < curRowData.numberMax)
            //            return true;
            //        else
            //            return false;
            //    }).data();
            //    if (prevRowData.length > 0) {
            //        $('input[name="numberMax"]', $(curRow.node())).attr('min', Number.parseFloat(prevRowData[prevRowData.length - 1].numberMax) + 0.1);
            //    }
            //}

        }
    });



    //activate save button upon data change
    $('#NetTable').on('change', 'input.wg-data, select.wg-data', { dt: $("#NetTable").dataTable().api() }, onTableDataChanged);

    //add dropdown filters 
    $('#NetTable thead tr th').each(function (i) {

        var title = $(this).text() + ": ";
        switch (i) {
            case 0:
                //new select node
                var mySel1 = new mySelect(WgEnums.rse.asKeyValueArray(false), true, -1, title, "searchRSETypeNet");
                $(this).html(mySel1.domNode);
                break;
            default:
                var div = document.createElement('div');
                div.className = "form-group form-inline";
                var label = document.createElement('p');
                label.appendChild(document.createTextNode(title));
                label.className = "form-control-static";
                div.appendChild(label);

                $(this).html(div);
        }


        $('select', this).on('change', function () {
            if (table.column(i + 1).search() !== this.value) {
                table
                    .column(i + 1)
                    .search(this.value)
                    .draw();
            }
        });
    });


    //
    // one unit type for each RSE type
    //
    $('#NetTable tbody').on('change', '.wg-data-unit', table, function (event) {
    });

    //
    // 
    //
    class inputNumber {
        constructor(node) {
            this.value = parseFloat(node.val());
            this.max = parseFloat(node.attr('max'));
            this.min = parseFloat(node.attr('min'));
        }

        isValid() {
            return this.value <= this.max && this.value >= this.min ? true : false;
        }
        get value() {
            return this.value >= this.max ? this.max : this.value <= this.min ? this.min : this.value;
        }
    }

    $('#NetTable tbody').on('change', '.number-max',table,function (event) {
                        
        var n = $(this);
        var newValue = parseFloat(n.val());
        var allowedRange = { min: parseFloat(n.attr('min')), max: parseFloat(n.attr('max')) };

        if (newValue < allowedRange.min) {
            n.val(allowedRange.min);
        }
        else if (newValue > allowedRange.max) {
            n.val(allowedRange.max);
        }

        var curRow = event.data.row($(this).parent());
        var curRowData = curRow.data();

        if (curRowData.numberMax === newValue)
            return;

        var nextRow = event.data.row(function (index,data, node) {
            if (data.surveyType === curRowData.surveyType &&
                data.rseType === curRowData.rseType &&
                data.numberMax > curRowData.numberMax)
                return true;
            else
                return false;
        });

        curRowData.numberMax = newValue;

        if (nextRow.length > 0) {
            nextRow.data().numberMin = newValue;
            nextRow.invalidate();
            nextRow.draw(false);
        }


        event.stopPropagation();
    });

    $('#NetTable tbody').on('click', 'input, select',table,function (event) {
        //when clicking on input or select or label (toggle checkbox)
        //don't select the row
        event.stopPropagation();

        //if ($(this).hasClass('toggle-off') || $(this).hasClass('toggle-on') && $(this).hasClass('active')) {
        //    var val = ($(this).prop('checked') === 'true');

        //    $(this).bootstrapToggle(val === true ? 'off' : 'on');
        //}
    });
}