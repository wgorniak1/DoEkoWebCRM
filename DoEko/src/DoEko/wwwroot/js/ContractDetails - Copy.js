/////////////////////////////////////////////////////////////
// Contract Details
// 1. DataTable with investments
// 2. 
/////////////////////////////////////////////////////////////

//---------------------------------------------------------//
$(document).ready(function () {
    var editMode = false;

    var table = $('#InvestmentListTable').DataTable({
        ajax: {
            url: "Investment/ListAjax",
            dataSrc: "data",
            rowId: "investmentId"
        },
        columns: [
            {
                responsivePriority: 1,
                data: 'fullName',
                name: 'fullName',
                title: 'Nazwisko i Imię',
                type: 'html',
                render: function (data, type, row, meta) {
                    switch (type) {
                        case "display":
                            return (data.length > 30) ? data.replace(' ', '<br/>') : data;
                        case "filter":
                            return data;
                        case "sort":
                            return data;
                    }
                }
            },
            {
                responsivePriority: 8,

                data: "pesel",
                name: "pesel",
                title: "Pesel",
            },
            {
                responsivePriority: 9,

                data: "idNumber",
                name: "idNumber",
                title: "Nr dowodu os.",
            },
            {
                responsivePriority: 10,

                data: "taxId",
                name: "taxId",
                title: "NIP",
            },
            {
                responsivePriority: 11,

                data: "birthDate",
                name: "birthDate",
                title: "Data urodz.",
                type: "date",
                render: function (data, type, row, meta) {
                    var bdt = new Date(Date.parse(data));
                    switch (type) {
                        case "display":
                            return (bdt > new Date('1900/01/01')) ? bdt.toLocaleDateString() : '';
                        case "filter":
                            return bdt;
                        case "sort":
                            return bdt;
                    }
                }
            },
            {
                responsivePriority: 6,

                data: "email",
                name: "email",
                title: "E-mail",
                type: "string",
                render: function (data, type, row, meta) {
                    switch (type) {
                        case "display":
                            return data ? '<a href="mailto:' + data + '" class="">' + data + '</a>' : '';
                        case "filter":
                            return data;
                        case "sort":
                            return data;
                    }

                },
            },
            {
                responsivePriority: 7,

                data: "phoneNumber",
                name: "phoneNumber",
                title: "Telefon",
                type: "num",
                render: function (data, type, row, meta) {
                    switch (type) {
                        case 'display':
                            return data ? '<a href="tel:' + data.replace(/\s+/g, '') + '">' + data + '</a>' : '';
                        case 'filter':
                            return data.replace(/\s+/g, '');
                        case 'sort':
                            return data.replace(/\s+/g, '');
                        default:
                    }
                }
            },
            {
                responsivePriority: 4,

                data: "address",
                data: "address",
                title: "Adres",
                type: "html",
                render: function (data, type, row, meta) {
                    switch (type) {
                        case "display":
                            return data.firstLine + '<br/>' + data.secondLine;
                        case "filter":
                            return data.secondLine + ',' + data.firstLine;
                        case "sort":
                            return data.secondLine + ',' + data.firstLine;
                    }
                }
            },
            {
                responsivePriority: 3,

                data: "dataProcessingConfirmation",
                name: "dataProcessingConfirmation",
                title: "Zgoda na przetw. danych",
                type: "string",
                render: function (data, type, row, meta) {
                    switch (type) {
                        case "display":
                            if (editMode) {
                                var content =
                                    '<input type="checkbox" id="dataProcessingConfirmation[' + row.businessPartnerId + ']" name="dataProcessingConfirmation" class="form-control checkbox person-confirmationchange" data-onstyle="success" data-size="small" data-toggle="toggle" data-on="Tak" data-off="Nie" value="' + data + '"';
                                content += !data ? '/>' : 'checked />';
                                return content;
                            }
                            else
                                return !data ? "Nie" : "Tak";
                        case "filter":
                            return !data ? "Nie" : "Tak";
                        case "sort":
                            return !data ? "Nie" : "Tak";
                    }

                }
            },
            {
                responsivePriority: 12,

                data: "investments",
                name: "investmentsCount",
                title: "Liczba inwestycji",
                type: "num",
                render: function (data, type, row, meta) {
                    return data.length;
                }
            },
            {
                responsivePriority: 5,

                data: "investments",
                name: "investments",
                title: "Inwestycje",
                type: "html",
                render: function (data, type, row, meta) {

                    var content = "";

                    switch (type) {
                        case "display":
                            data.forEach(function (investment, index, arr) {
                                content += '<tr><td>' +
                                    '<a href="/Investments/Details/' + investment.investmentId + '" target="_blank">' + investment.address.firstLine + '<br/>' + investment.address.secondLine + '</a ></td ></tr > ';
                            });
                            return '<table>' + content + '</table>';
                        case "filter":
                            data.forEach(function (investment, index, arr) {
                                content += investment.address.secondLine + ',' + investment.address.firstLine + ',';
                            });

                            return content;
                        case "sort":
                            data.forEach(function (investment, index, arr) {
                                content += investment.address.secondLine + ',' + investment.address.firstLine + ',';
                            });
                            return content;
                    }

                }
            },
            {
                responsivePriority: 2,
                data: null,
                name: "actions",
                title: "Akcje",
                orderable: false,
                searchable: false,
                type: 'html',
                width: "100px",
                autoWidth: false,
                visible: true,
                render: function (data, type, row, meta) {

                    var content = "";
                    content = '<div class="pull-right">';
                    content += '<button title="Edytuj dane" class="btn btn-default btn-sm person-edit" type="button">' +
                        '<span class="glyphicon glyphicon-pencil"></span>' +
                        '</button>';

                    if (data.investments.length === 0) {
                        content += '<button class="btn btn-default btn-sm person-delete" type="button">' +
                            '<span class="glyphicon glyphicon-trash"></span>' +
                            '</button>';
                    }
                    else {
                    }
                    content += '</div>';
                    return content;

                }
            }
        ],

        stateSave: true,
        pagingType: "full",
        language: {
            url: "/js/datatables-language-pl.json"
        },

        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"]],
        processing: true,
        dom: "<'row'<'col-sm-6'B><'col-sm-6'f>>" +
        "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
            {
                extend: 'copyHtml5',
                text: '<span class="glyphicon glyphicon-copy"></span>',
                className: 'btn text-primary',
                copySuccess: {
                    1: 'Skopiowano 1 rekord do schowka',
                    _: 'Skopiowano %d rekordów do schowka'
                },
                copyTitle: 'Kopiowanie do schowka',
                copyKeys: 'Naciśnij <i>ctrl</i> lub <i>\u2318</i> + <i>C</i> aby skopiować tabelę<br>do schowka.<br><br>aby anulować, kliknij ten komunikat lub naciśnij ESC.'
            },
            {
                extend: 'csvHtml5',
                text: '<span class="glyphicon glyphicon-download-alt" title="Export do CSV"></span>',
                className: 'btn text-primary',
                fieldSeparator: ';',
                charset: 'UTF-8'
            },
            {
                extend: 'colvis',
                text: '<span class="glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
                className: 'btn text-primary'
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-refresh" title="Odśwież zawartość"></span>',
                className: 'btn',
                action: function (e, dt, node, config) {

                    dt.ajax.reload();

                    dt.rows().cells().invalidate().render();
                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-check" title="Edytuj zgody..."></span>',
                className: 'btn',
                action: function (e, dt, node, config) {
                    editMode = !editMode;

                    dt.column('dataProcessingConfirmation').cells().invalidate().render();


                    dt.rows().every(function (rowIdx, tableLoop, rowLoop) {
                        if (dt.row(rowIdx).child.isShown()) {
                            $(dt.cell(rowIdx, 0).node()).click();
                            $(dt.cell(rowIdx, 0).node()).click();
                        }
                    });

                    $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');

                }
            },
        ],
        select: false,
        colReorder:
        {
            fixedColumnsRight: 1
        },
        //responsive: {
        //breakpoints: [
        //    { name: 'desktop', width: 3000 },
        //    //{ name: 'tablet', width: 1 },
        //   //{ name: 'fablet', width: 1024 },
        //  //  { name: 'phone', width: 768 }
        //],
        //details: {
        //    type: 'column',
        //    renderer: function (api, rowIdx, columns) {
        //        var data = $.map(columns, function (col, i) {
        //            return col.hidden ?
        //                '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
        //                '<td>' + col.title + ':' + '</td> ' +
        //                '<td>' + col.data + '</td>' +
        //                '</tr>' : '';
        //        }).join('');

        //        return data ?
        //            $('<table/>').append(data) :
        //            false;
        //    }
        //}
        //},
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
            $('div#BPPersonListTable_processing').addClass("wg-loader");

            //var context = $('div#BPPersonListTable_filter');
            //$('*', context).addClass('small');

            //context = $('div#BPPersonListTable_length');
            //$('label', context).addClass('small');

            //$('select', context).addClass('small');
            ////$('select', context).addClass('small btn btn-default btn-sm');
            //// $('select', context).removeClass('form-control');

            //context = $('div#BPPersonListTable_info');
            //context.addClass('small');
            //$('*', context).addClass('small');

            //context = $('div#BPPersonListTable_paginate');
            //$('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");


            $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');
        }
    });

    table.on('responsive-display', function (e, datatable, row, showHide, update) {
        $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');
    });


});


$('body').on('change', 'input.person-confirmationchange', onConfirmationChange);

$('body').on('click', 'button.person-edit', function () { $(this).attr("disabled", "disabled"); onPersonEdit($('#BPPersonListTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled"); });
$('body').on('click', 'button.person-delete', function () { $(this).attr("disabled", "disabled"); onPersonDelete($('#BPPersonListTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled"); });

//---------------------------------------------------------//
$(document).ready(function () {
    var neoenergetykaFile = $('input#Neoenergetyka', $('form#NeoenergetykaForm'));
    neoenergetykaFile.on('change', onNeoenergetykaFileChange);
    $('button.wg-btn-handle-neoenergetyka').on('click',
        function () {
            //reset existing value
            neoenergetykaFile.val('');
            neoenergetykaFile.focus().trigger('click');
        });
    //
    var PKOFile = $('input#FIFile', $('form#FIForm'));
    PKOFile.on('change', onPKOFileChange);
    $('button.wg-btn-handle-pko').on('click',
        function () {
            //reset existing value
            PKOFile.val('');
            PKOFile.focus().trigger('click');
        });
    //
    var INVFile = $('input#INFFile', $('form#INFForm'));
    INVFile.on('change', onInvFileChange);
    $('button.wg-btn-handle-pko').on('click',
        function () {
            //reset existing value
            INVFile.val('');
            INVFile.focus().trigger('click');
        });
});
//---------------------------------------------------------//
function onNeoenergetykaFileChange() {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");
        return
    }
    else if (!allowedTypes.includes(file.type)) {
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
    //Post File:
    var form = new FormData($('form#NeoenergetykaForm')[0]);

    var call = $.ajax({
        type: "POST",
        url: "/Contracts/UploadNeoenergetykaResults",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.error(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');
        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });

}
//---------------------------------------------------------//
function onPKOFileChange() {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");
        return
    }
    else if (!allowedTypes.includes(file.type)) {
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
    //Post File:
    var form = new FormData($('form#FIForm')[0]);

    var call = $.ajax({
        type: "POST",
        url: "/Payments/UploadPaymentsFile",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.error(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');
        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });
}
//---------------------------------------------------------//
function onInvFileChange() {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");
        return
    }
    else if (!allowedTypes.includes(file.type)) {
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
    //Post File:
    var form = new FormData($('form#INFForm')[0]);

    var call = $.ajax({
        type: "POST",
        url: "/Investments/UploadDataFromFile",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.error(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');

        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });

    });
}
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
//---------------------------------------------------------//
/////////////////////////////////////////////////////////////