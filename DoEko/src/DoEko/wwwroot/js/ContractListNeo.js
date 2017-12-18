//
$(document).ready(function () {

    var table = $('#ContractListTable').DataTable({
        ajax: {
            url: "api/v1/Contract/Neo",
            dataSrc: "",
            rowId: "contractId"
        },
        columns: [
                    {
                        responsivePriority: 1,
                        data: 'project',
                        name: 'project',
                        title: 'Projekt'
                    },
                    {
                        responsivePriority: 2,
                        data: "contractId",
                        name: "contractId",
                        title: "Nr"
                    },
                    {
                        responsivePriority: 3,
                        data: "number",
                        name: "number",
                        title: "Numer Umowy"
                    },
                    {
                        responsivePriority: 4,

                        data: "investmentNo",
                        name: "investmentNo",
                        title: "L. Inwestycji"
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
                            var id = row.contractId;

                            var content = "";

                            content = '<div class="pull-right">';
                            content += '<a class="btn btn-sm btn-default" href="/Contracts/Details/' + id + '">';
                            content += '<span class="glyphicon glyphicon-eye-open"></span>';
                            content += '</a>';
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
            $('div#ContractListTable_processing').addClass("wg-loader");

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
        }
    });

    //table.on('responsive-display', function (e, datatable, row, showHide, update) {
    //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');  
    //});


});