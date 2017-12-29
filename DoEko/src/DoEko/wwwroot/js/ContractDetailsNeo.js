//
$(document).ready(function () {
    var ajaxUrl = "/api/v1/Investments/Neo?contractId=" + $("#ContractId").val();
    var table = $('#InvestmentListTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: "",
            rowId: "investmentId"
        },
        columns: [
                    {
                        data: 'address',
                        name: 'address',
                        title: 'Adres'
                    },
                    {
                        data: "plotNumber",
                        name: "plotNumber",
                        title: "Nr działki"
                    },
                    {
                        data: "status",
                        name: "status",
                        title: "Status"
                    },
                    {
                        className: "none",
                        data: "inspectionStatus",
                        name: "inspectionStatus",
                        title: "Status inspekcji"
                    },

                    {
                        data: "calculate",
                        name: "calculate",
                        title: "Do weryfikacji"
                    },
                    {
                        className: "none",
                        data: "surveys",
                        name: "surveys",
                        title: "Źródła",
                        render: function (data, type, row, meta) {
                            // Order, search and type get the original data
                            if (type !== 'display') {
                                return data;
                            }

                            var arrayLength = data.length;
                            var content = "";

                            if (arrayLength > 0) {
                                content = '<div class="row">';
                                content += '<div class="col-sm-3"><span class="control-label">Typ OZE</span></div>';
                                content += '<div class="col-sm-4">Status</div>';
                                content += '<div class="col-sm-3">Moc</div>';
                                content += '<div class="col-sm-2">Policzone</div>';
                                content += '</div>';
                                for (var i = 0; i < arrayLength; i++) {

                                    var power = data[i].finalPower;

                                    content += '<div class="row">';
                                    content += '<div class="col-sm-3">' + data[i].rseType + '</div>';
                                    var status = data[i].status;
                                    status = status.length > 10 ? status.substring(0, 9) + '...' : status;
                                    content += '<div class="col-sm-4" title="' + data[i].status + '">' + status + '</div>';
                                    content += '<div class="col-sm-3">' + power.toFixed(2) + ' kW </div>';
                                    content += '<div class="col-sm-2">' + (data[i].isCompleted === true ? 'Tak' : 'Nie') + '</div>';
                                    content += '</div>';
                                }

                            }
                            
                            return content;
                        }
                    },
                    {
                        data: null,
                        className: 'all',
                        name: "actions",
                        title: "Akcje",
                        orderable: false,
                        searchable: false,
                        type: 'html',
                        width: "100px",
                        autoWidth: false,
                        visible: true,
                        render: function (data, type, row, meta) {
                            var id = row.investmentId;

                            var content = "";

                            content = '<div class="pull-right">';
                            content += '<a class="btn btn-sm btn-default" href="/Investment/Details/' + id + '">';
                            content += '<span class="glyphicon glyphicon-eye-open"></span>';
                            content += '</a>';
                            content += '</div>';

                            return content;
                        }
                    }
        ],

        stateSave: false,
        pagingType: "full",
        language: WgLanguage,

        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"]],
        processing: true,
        dom: "<'row wg-dt-padding'<'col-sm-6'B><'col-sm-6'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row wg-dt-padding'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
            //{
            //    extend: 'copyHtml5',
            //    text: '<span class="glyphicon glyphicon-copy"></span>',
            //    className: 'btn text-primary',
            //    copySuccess: {
            //        1: 'Skopiowano 1 rekord do schowka',
            //        _: 'Skopiowano %d rekordów do schowka'
            //    },
            //    copyTitle: 'Kopiowanie do schowka',
            //    copyKeys: 'Naciśnij <i>ctrl</i> lub <i>\u2318</i> + <i>C</i> aby skopiować tabelę<br>do schowka.<br><br>aby anulować, kliknij ten komunikat lub naciśnij ESC.'
            //},
            //{
            //    extend: 'csvHtml5',
            //    text: '<span class="glyphicon glyphicon-download-alt" title="Export do CSV"></span>',
            //    className: 'btn text-primary',
            //    fieldSeparator: ';',
            //    charset: 'UTF-8'
            //},
            {
                extend: 'colvis',
                text: '<span class="glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
                className: 'btn text-primary'
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
                text: '<span class="text-primary glyphicon glyphicon-download"></span> Pobierz',
                titleAttr: 'Pobierz do weryfikacji kolejne 50 inwestycji',
                className: 'btn',
                action: function (e, dt, node, config) {

                    dt.table().buttons().disable();

                    var $loading = $('div#InvestmentListTable_processing');
                    $loading.show();

                    var url = "/api/v1/Survey/Neo?contractId=" + $("#ContractId").val();
                    var ajax = new $.get(url);

                    ajax.fail(function (result) {
                        $loading.hide();
                        dt.table().buttons().enable();
                        WgTools.alert(result.responseJSON.error, false, 'E');
                    });

                    ajax.done(function (result) {

                        var link = WgTools.createLink(result.url);
                        document.body.appendChild(link);
                        link.click();

                        $loading.hide();
                        dt.table().buttons().enable();
                        WgTools.alert('Pomyślnie wygenerowano plik', true, 'S');
                    });

                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-upload"></span> Zapisz',
                titleAttr: 'Zapisz wyniki (dobory)',
                className: 'btn',
                action: function (e, dt, node, config) {

                    dt.table().buttons().disable();
                    var $loading = $('div#InvestmentListTable_processing');
                    $loading.show();


                    var neoenergetykaFile = $('input#Neoenergetyka', $('form#NeoenergetykaForm'));

                    neoenergetykaFile.one('change', { dt: dt } , onNeoenergetykaFileChange);

                    neoenergetykaFile.focus().trigger('click');            
                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-filter"></span>Filtruj',
                titleAttr: 'Pokaż tylko do wykonania',
                className: 'btn',
                action: function (e, dt, node, config) {

                    var currentSearch = dt.table().column('calculate:name')
                    .search();

                    if (currentSearch != '') {
                        //clear filter
                        dt.table()
                        .column('calculate:name')
                        .search('')
                        .draw();

                        node.html(node.html().replace("Pokaż wszystko", "Filtruj"));

                    }
                    else {
                        //set filter
                        dt.table()
                        .column('calculate:name')
                        .search('Tak').draw();

                        node.html(node.html().replace("Filtruj", "Pokaż wszystko"));
                    }
                    //if (node.hasClass('filtered')) {
                    //    node.removeClass('filtered');
                    //}
                    //else {
                    //    node.addClass('filtered');
                    //}

                }
            },

        ],
        select: false,
        colReorder: 
        {
            fixedColumnsRight: 1
        },
        responsive: {
            //breakpoints: [
            //    { name: 'desktop', width: 3000 },
            //    //{ name: 'tablet', width: 1 },
            //   //{ name: 'fablet', width: 1024 },
            //  //  { name: 'phone', width: 768 }
            //],
            details: {
            //    type: 'column',
                renderer: function (api, rowIdx, columns) {
                    var data = $.map(columns, function (col, i) {
                        return col.hidden ? 
                       // '<tr><td>' + 
                            '<div class="row" data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                            '<div class="col-sm-2">' + col.title + ':' + '</div> ' +
                            '<div class="col-sm-10">' + col.data + '</div>' +
                            '</div>' //+
                        //'</td></tr>'
                            : '';

                        //'<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                        //    '<td>' + col.title + ':' + '</td> ' +
                        //    '<td>' + col.data + '</td>' +
                        //'</tr>' : '';
                    }).join('');

                    return data ?
                        data : 
                        //$('<table/>').append(data) :
                        false;
                }
            }
        },
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
       // drawCallback: function (settings, json) {
            //$('div#InvestmentListTable_processing').addClass("wg-loader");

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


            //$('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');
        //}
    });

    //table.on('responsive-display', function (e, datatable, row, showHide, update) {
    //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');  
    //});
});
//---------------------------------------------------------//
function onNeoenergetykaFileChange(event) {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");

        event.data.dt.table().buttons().enable();
        var $loading = $('div#InvestmentListTable_processing');
        $loading.hide();
        return;
    }
    else if (!allowedTypes.includes(file.type)) {
        $(this).val('');
        event.data.dt.table().buttons().enable();
        var $loading = $('div#InvestmentListTable_processing');
        $loading.hide();
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
    //Post File:
    var form = new FormData($('form#NeoenergetykaForm')[0]);
    //clear input value and 
    $(this).val('');

    var call = $.ajax({
        type: "POST",
        url: "/Contracts/UploadNeoenergetykaResults",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            
        }
    });

    call.done(function (data, success) {
        //notification popup
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
        //refresh table content
        event.data.dt.table().buttons().enable();
        event.data.dt.ajax.reload();
        event.data.dt.rows().cells().invalidate().render();
    });
    call.fail(function (xhr, status, error) {
        //Notification popup
        //xhr.responsejson np. "neoenergetyka" : ["Nie odnaleziono pliku"]
        //
        event.data.dt.table().buttons().enable();
        var $loading = $('div#InvestmentListTable_processing');
        $loading.hide();

        json = xhr.responseJSON;
        if (json.neoenergetyka !== null) {
            WgTools.alert(json.neoenergetyka, false, 'E');
        }
        else 
            WgTools.alert(json, false, 'E');
    });

}