'use strict';

//plugin to add options to select
(function( $ ) {

    $.fn.addOptions = function(array, selected) {
        
        if (!array || Object.values(array[ 0 ]).length < 2) {
            return this;
        } else {

            return this.filter("select").each(function () {
                var select = this;
                $(array).each(function () {
                    var oPropValues = Object.values(this);
                    $('<option/>', oPropValues[0] === selected ? { value: oPropValues[0], selected: true } : { value: oPropValues[0] })
                        .text(oPropValues[ 1 ])
                        .appendTo(select);
                });
            });
        }
 
    };
 
}( jQuery ));

class mySelect {

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
}
class WgBdgEnum{
    constructor() {
        this._array = ["Gospodarczy", "Mieszkalny"];
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
}

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
        
        new $.fn.dataTable.FixedHeader(table, {
            headerOffset: $('#NavBarMain').outerHeight()
        });

    });
    //table.on('responsive-display', function (e, datatable, row, showHide, update) {
    //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');  
    //});
});

function InitializeTaxTab(projectId) {
    var ajaxUrl = "/api/v1/RSERules/Tax?projectId=" + projectId;

    var table = $('#TaxTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: ""
        },
        columnDefs: [{
            className: 'text-right',
            targets: [3,4,5]
        }],
        columns: [
                    {
                        data: null,
                        name: 'rseType',
                        title: 'Typ OZE',
                        type: 'html',
                        render: function(data, type, row, meta){
                            return WgEnums.rse.getText(data.surveyType, data.rseType);
                        }
                    },
                    {
                        data: "installationLocalization",
                        name: "installationLocalization",
                        title: "Lokalizacja",
                        render: function (data, type, row, meta) {
                            return WgEnums.localization.getText(data);
                        }
                    },
                    {
                        data: "buildingPurpose",
                        name: "buildingPurpose",
                        title: "Przezn. Budynku",
                        render: function (data, type, row, meta) {
                            return WgEnums.building.getText(data);
                        }
                    },
                    {
                        data: "usableAreaMin",
                        name: "usableAreaMin",
                        title: "Powierzchnia Od",
                        type: "html-num",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content = '<input type="number" class="form-control input-sm wg-area-min" value="' + data + '" step="0.1" min="0.0" max="99999.9" disabled>';
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
                        render: function (data, type, row, meta) {
                            var maxValue = data === Number.MAX_VALUE ? '99999.99' : data;

                            switch (type) {
                                case 'display':
                                    var content = '<input type="number" class="form-control input-sm wg-area-max" value="' + maxValue + '" step="0.1" min="0.0" max="99999.9">';
                                    return WgTaxTableEdit === true ? content : maxValue;
                                default:
                                    return maxValue;
                            }
                        }
                    },
                    {
                        data: "vat",
                        name: "vat",
                        title: "VAT",
                        type: "html-num-fmt",
                        render: function (data, type, row, meta) {

                            switch (type) {
                                case 'display':

                                    var opt = [{ val: "0", text: "0 %" }, { val: "5", text: "5 %" }, { val: "8", text: "8 %" }, { val: "23", text: "23 %" }];
                                    return $('<select>', { name: "vat", class: "form-control input-sm", disabled: !WgTaxTableEdit })
                                        .addOptions(opt, data)
                                        .prop('outerHTML');

                                default:
                                    return data;
                            }

                        }
                    }
                    //{
                    //    data: null,
                    //    className: 'all',
                    //    name: "actions",
                    //    title: "Akcje",
                    //    orderable: false,
                    //    searchable: false,
                    //    type: 'html',
                    //    autoWidth: false,
                    //    visible: true,
                    //    render: function (data, type, row, meta) {
                    //        var content = "";

                    //        content = '<div class="pull-right">';
                    //        content += '<button class="btn btn-sm btn-default" href="/Investment/Details/' + '">';
                    //        content += '<span class="glyphicon glyphicon-eye-open"></span>';
                    //        content += '</a>';
                    //        content += '</div>';

                    //        return content;
                    //    }
                    //}
        ],

        stateSave: false,
        pagingType: "full",
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"], [1, 'asc'], [2, 'asc'], [3, 'asc'], [4, 'asc']],
        processing: true,
        dom: "<'row wg-dt-padding'<'col-sm-6'B><'col-sm-6'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row wg-dt-padding'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
                        {
                            text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Przywróć',
                            titleAttr: 'Przywróć ustawienia domyślne',
                            className: 'btn',
                            action: function (e, dt, node, config) {
                                //
                                dt.table().buttons().disable();
                                var loading = $('div#TaxTable_processing');
                                loading.show();

                                //
                                //missing confirmation popup
                                //

                                //call
                                var call = $.ajax({
                                    url: ajaxUrl,
                                    type: 'DELETE',
                                    data: {},
                                    contentType: 'application/json'
                                });

                                call.done(function (result) {
                                    dt.table().buttons().enable();
                                    dt.ajax.reload();
                                    dt.rows().cells().invalidate().render();
                                    WgTools.alert("Pomyślnie przywrócono wartości początkowe.", false, 'S');
                                });
                                call.fail(function (result) {
                                    WgTools.alert("Coś poszło nie tak. Proszę skontaktować się z administratorem strony.", false, 'E');
                                });
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon-refresh"></span>',
                            titleAttr: 'Odśwież zawartość',
                            className: 'btn',
                            action: function (e, dt, node, config) {

                                dt.ajax.reload();
                                dt.rows().cells().invalidate().render();
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Podziel',
                            titleAttr: 'Podziel przedział powierzchni',
                            className: 'btn',
                            action: function () {
                                var table = $("#TaxTable").dataTable().api();

                                var currentRow = table.eq(0).row({ selected: true, page: 'current' });

                                if (currentRow === undefined)
                                    return;

                                var rowData = currentRow.data();
                                                                                                    
                                //new Max for currentRow
                                var newUsableAreaMax = rowData.usableAreaMax - (rowData.usableAreaMax - rowData.usableAreaMin) / 2;


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

                                //var nc = $(nodeCurrent);
                                //var nn = $()

                            }
                        }
        ],
        select: 'single',
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        }
    });

    $('body').on('change', 'input.wg-area-max', function () {
        var min = $(this).parents('tr').next('tr').children('td').find('input.wg-area-min');
        min.val($(this).val());
    });
}

function InitializeNetTab(projectId) {
    var ajaxUrl = "/api/v1/RSERules/Price?projectId=" + projectId;
    var table = $('#NetTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: ""
        },
        columns: [
                    {
                        data: null,
                        name: 'rseType',
                        title: 'Typ OZE',
                        type: 'html',
                        render: function (data, type, row, meta) {
                            return WgEnums.rse.getText(data.surveyType, data.rseType);
                        }
                    },
                    {
                        data: "unit",
                        name: "unit",
                        title: "Jednostka",
                        type: 'html',
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':

                                    var opt = [{ val: "1", text: "Moc ostateczna" }, { val: "2", text: "Liczba zestawów" }];

                                    var a = $('<select />', { name: 'unit', class: 'form-control input-sm unit', disabled: !WgNetTableEdit })
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
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content = '<input type="number" name="numberMin" class="form-control input-sm" value="' + data + '" step="0.1" min="0.0" max="99999.9" disabled>';
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
                        render: function (data, type, row, meta) {
                            if (data === Number.MAX_VALUE)
                                data = '99999.99';

                            switch (type) {
                                case 'display':
                                    var step = row.unit === 1 ? 0.1 : 1;
                                    var content = '<input type="number" style="width: 100%; min-width: 30px; max-width: 100px;" name="numberMax" class="form-control input-sm number-max" value="' + data + '" step="' + step + '" min="0.0" max="99999.9">';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }
                    },
                    {
                        data: "netPrice",
                        name: "netPrice",
                        title: "Cena Netto",
                        type: "html-num",
                        render: function(data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content;
                                   // content = '<div class="input-group">'; 
                                   // content += '<span class="input-group-addon">zł</span>';
                                    content = '<input type="number" name="netPrice" min="0" max="99999" step="1" class="form-control input-sm" value="'+ data +'"/>';
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
                        type: "html",
                        render: function(data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    var content;
                                    content = '<input name="multiply"  class="form-control input-sm checkbox"  data-toggle="toggle" data-on="Tak" data-off="Nie" type="checkbox"';
                                    content += data === true ? ' checked />' : '/>';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }


                    }
        ],
        stateSave: true,
        pagingType: "full",
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"], [1, "asc"], [2, "asc"], [3, "asc"], [4, "asc"], [5, "asc"] ],
        processing: true,
        dom: "<'row wg-dt-padding'<'col-sm-6'B><'col-sm-6'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row wg-dt-padding'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
                        {
                            text: 'draw',
                            action: function (e, dt, node, config) {
                                dt.draw();
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Przywróć',
                            titleAttr: 'Przywróć ustawienia domyślne',
                            className: 'btn',
                            action: function (e, dt, node, config) {
                                //
                                dt.table().buttons().disable();
                                var loading = $('div#NetTable_processing');
                                loading.show();

                                //
                                //missing confirmation popup
                                //

                                //call
                                var call = $.ajax({
                                    url: ajaxUrl,
                                    type: 'DELETE',
                                    data: {},
                                    contentType: 'application/json'
                                });

                                call.done(function (result) {
                                    dt.table().buttons().enable();
                                    dt.ajax.reload();
                                    dt.rows().cells().invalidate().render();
                                    WgTools.alert("Pomyślnie przywrócono wartości początkowe.", false, 'S');
                                });
                                call.fail(function (result) {
                                    WgTools.alert("Coś poszło nie tak. Proszę skontaktować się z administratorem strony.", false, 'E');
                                });
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon-refresh"></span>',
                            titleAttr: 'Odśwież zawartość',
                            className: 'btn',
                            action: function (e, dt, node, config) {

                                dt.ajax.reload();
                                dt.rows().cells().invalidate().render();
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-full"></span> Podziel',
                            titleAttr: 'Podziel przedział wartości',
                            className: 'btn',
                            action: function (e, dt, node, config) {
                                

                                var curRow = dt.row({ selected: true, page: 'current' });
                                var index = curRow.index();
                                

                                //calculate new value                        
                                var curRowData = curRow.data();
                                var numberDivisor = curRowData.numberMax - (curRowData.numberMax - curRowData.numberMin) / 2;
                                
                                // add new row
                                var newRow = dt.row.add({
                                    project: null,
                                    projectId: curRowData.projectId,
                                    surveyType: curRowData.surveyType,
                                    rseType: curRowData.rseType,
                                    unit: curRowData.unit,
                                    numberMin: numberDivisor,
                                    numberMax: curRowData.numberMax,
                                    netPrice: curRowData.netPrice,
                                    multiply: curRowData.multiply
                                });


                                //update value in current row
                                curRowData.numberMax = numberDivisor;

                                //redraw table so that new DOM elements are present
                                newRow.invalidate();
                                curRow.invalidate().draw();
                            }
                        },
                        {
                            text: '<span class="text-primary glyphicon glyphicon glyphicon-resize-small"></span> Usuń',
                            titleAttr: 'Usuń przedział wartości',
                            className: 'btn',
                            action: function (e, dt, node, config) {

                                var curRow = dt.row({ selected: true, page: 'current' });
                                if  (curRow === undefined){
                                    return;
                                }
                                var curRowData = curRow.data();
                                //                                
                                if (curRowData.numberMin === 0)
                                    return;
                                
                                var prevRows = dt.rows(function (index, data, node) {
                                    if (data.surveyType === curRowData.surveyType &&
                                        data.rseType === curRowData.rseType &&
                                        data.numberMax < curRowData.numberMax)
                                        return true;
                                    else
                                        return false;
                                });
                                if (prevRows.length > 0) {
                                    prevRows.data()[prevRows.count() - 1].numberMax = curRowData.numberMax;
                                    prevRows.invalidate();
                                }

                                curRow.remove().draw();

                            }
                        }

        ],
        select: 'single',
        responsive:  true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings) {
            var tableBody = $("#NetTable tbody");
            var table = $("#NetTable").dataTable().api();

            //activate toggles
            $('input[data-toggle="toggle"]',tableBody).bootstrapToggle();
            

            for (var i = 0; i < table.rows().count(); i++) {
                var curRow = table.row(i);
                var curRowData = curRow.data();

                var nextRowData = table.row(function (index, data, node) {
                    if (data.surveyType === curRowData.surveyType &&
                        data.rseType === curRowData.rseType &&
                        data.numberMax > curRowData.numberMax)
                        return true;
                    else
                        return false;
                }).data();
                if (nextRowData !== undefined) {
                    var maxValue = nextRowData.numberMax >= Number.MAX_VALUE ? 99999.98 : Number.parseFloat(nextRowData.numberMax) - 0.1;
                    $('input[name="numberMax"]', $(curRow.node())).attr('max', maxValue);
                }

                var prevRowData = table.rows(function (index, data, node) {
                    if (data.surveyType === curRowData.surveyType &&
                        data.rseType === curRowData.rseType &&
                        data.numberMax < curRowData.numberMax)
                        return true;
                    else
                        return false;
                }).data();
                if (prevRowData.length > 0) {
                    $('input[name="numberMax"]', $(curRow.node())).attr('min', Number.parseFloat(prevRowData[prevRowData.length - 1].numberMax) + 0.1);
                }
            }

        }
    });

    //
    // one unit type for each RSE type
    //
    $('#NetTable tbody').on('change', '.unit', table, function (event) {
        var dt = event.data;
        var value = $(this).val();
        var thisRowData = dt.row($(this).parent()).data();

        dt
            .rows(function (index, data, node) {
                return data.surveyType === thisRowData.surveyType &&
                        data.rseType === thisRowData.rseType ? true : false;
            })
            .every(function (index, tableLoop, rowLoop) {
                this.data().unit = parseInt(value);
                this.invalidate();
            })
            .draw();

        dt.draw();
        
        //everything has been done 
        event.stopPropagation();
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

    $('#NetTable tbody').on('click', 'input, select, label',table,function (event) {
        //when clicking on input or select or label (toggle checkbox)
        //don't select the row
        event.stopPropagation();
    });
}