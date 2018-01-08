'use strict';

class WgRSEEnum {

    constructor() {
        this.surveyType = ["CO", "CWU", "EE"];
        this.rseType = [["", "Pompa Ciepła Grunt.", "Kocioł na Pellet", "Pompa Ciepła Powietrzna"],["", "", "", "Solary", "Pompa Ciepła"],["","","","","","Panele Fotowolt."]];
    }
    
    getText(type, rseType, single) {
        if (rseType === undefined)
            return this.surveyType[type];
        else if (single)
            return this.rseType[type][rseType]
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
        this._array = ["Dach","Grunt","Elewacja"]
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
        this._array = ["Gospodarczy", "Mieszkalny"]
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
        table.ajax.reload();
        table.rows().cells().invalidate().render();
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
                            var maxValue = (data === Number.MAX_VALUE) ? '99999.99' : data;

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
                            var value = data + " %";

                            switch (type) {
                                case 'display':
                                    var content =
                                        '<select class="form-control input-sm">' +
                                           '<option value="0" ' + ((data === 0) ? 'selected' : '') + '>0 %</option>' +
                                           '<option value="5" ' + ((data === 5) ? 'selected' : '') + '>5 %</option>' +
                                           '<option value="8" ' + ((data === 8) ? 'selected' : '') + '>8 %</option>' +
                                           '<option value="23" ' + ((data === 23) ? 'selected' : '') + '>23 %</option>' +
                                        '</select>';
                                    return WgTaxTableEdit === true ? content : value;
                                default:
                                    return value;
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
        },
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
                                    var content =
                                        '<select class="form-control input-sm" name="unit" >' +
                                           '<option value="1" ' + ((data === 1) ? 'selected' : '') + '>Moc ostateczna</option>' +
                                           '<option value="2" ' + ((data === 2) ? 'selected' : '') + '>Liczba zestawów</option>' +
                                        '</select>';
                                    return WgNetTableEdit === true ? content : data;
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
                                data = '999999.99';

                            switch (type) {
                                case 'display':
                                    var content = '<input type="number" name="numberMax" class="form-control input-sm" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
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
                                    content = '<div class="input-group">'; 
                                    content += '<span class="input-group-addon">zł</span>';
                                    content += '<input type="number" name="netPrice" min="0" max="99999" step="0.01" class="form-control input-sm" value="'+ data +'"/>';
                                    content += '</div>';
                                    return (WgNetTableEdit === true) ? content : data;
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
                                    content += (data === true) ? ' checked />' : '/>';
                                    return WgNetTableEdit === true ? content : data;
                                default:
                                    return data;
                            }
                        }


                    },
        ],
        stateSave: true,
        pagingType: "full",
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"], [1, "asc"], [2, "asc"], [3, "asc"]],
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
                                curRow.invalidate();
                                newRow.invalidate();
                                curRow.draw();
                                newRow.draw();
                            }
                        }
        ],
        select: 'single',
        responsive:  true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings) {
            var table = $("#NetTable").dataTable().api();

            $('input[data-toggle="toggle"]').bootstrapToggle();

        }
    });

    $('#NetTable tbody')
        .on('change', 'input[name="numberMax"]', function () {
            var table = $('#NetTable').dataTable().api();

            var curRow = table.row($(this).parent('td').parent('tr'));
            var curRowData = curRow.data();
            var index = curRow.index();

            var nextRow = table.row(index + 1);
            nextRow.data().numberMin = $(this).val();
            nextRow.invalidate();
            nextRow.draw();
            //var rows = table.rows().data().filter(function (data, index) {
            //    if (data.surveyType == curRowData.surveyType &&
            //        data.rseType == curRowData.rseType &&
            //        data.numberMax > curRowData.numberMax)
            //        return true;
            //    else
            //        return false;
            //});

            //if (rows.length > 0)
            //    rows[0].numberMin = $(this).val();

            //$('input', table.cell(newRow.index(),'numberMin:name').node()).val($(this).val());
        });


}