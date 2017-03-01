//
$(document).ready(function () {
    var table = $('#InspectorWorkTable').DataTable({
        ajax: {
            url: "InspectorWork",
            dataSrc: "data",
            rowId: "inspectorId"
        },
        columns: [
                    {
                        data: "inspectorName",
                        name: "Inspektor"
                    },
                    {
                        data: "projectDescr",
                        name: "Projekt"
                    },
                    {
                        data: "contractNo",
                        name: "Nr Umowy"
                    },
                    {
                        data: "period",
                        name: "Okres"
                    },
                    {
                        data: "surveyCount",
                        name: "rse_count"
                    }
        ],
        stateSave: true,
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
                className: 'btn-sm text-primary'
            },
        {
            extend: 'csvHtml5',
            text: '<span class="glyphicon glyphicon-download-alt" title="Export do CSV"></span>',
            className: 'btn-sm text-primary'
        },
        //{
        //    extend: 'pageLength',
        //    text: 'page len',
        //    className: 'btn-sm text-primary'
        //},
        {
            extend: 'colvis',
            text: '<span class="glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
            className: 'btn-sm text-primary'
        },
        {
            text: '<span class="text-primary glyphicon glyphicon-refresh" title="Odśwież zawartość"></span>',
            className: 'btn-sm',
            action: function (e, dt, node, config) {
                dt.ajax.reload();
                dt.columns.adjust().draw();
            }
        }
        ],
        select: false,
        colReorder: { },
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
            $('div#InspectorWorkTable_processing').addClass("wg-loader");

            var context = $('div#InspectorWorkTable_filter');
            $('*', context).addClass('small');

            context = $('div#InspectorWorkTable_length');
            $('label', context).addClass('small');

            $('select', context).addClass('small');
            //$('select', context).addClass('small btn btn-default btn-sm');
            // $('select', context).removeClass('form-control');

            context = $('div#InspectorWorkTable_info');
            context.addClass('small');
            $('*', context).addClass('small');

            context = $('div#InspectorWorkTable_paginate');
            $('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");
        },

        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            $(api.table().column('rse_count:name').footer()).html(
              'Suma:' + api.column('rse_count:name', { page: 'current' }).data().sum()
            );
        }
    });

});