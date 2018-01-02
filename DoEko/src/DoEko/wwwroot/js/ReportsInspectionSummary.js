$(document).ready(function () {
    var reports = [];

    var table = $('#InspectionSummaryTable').DataTable({
        ajax: {
            url: 'InspectionSummaryList',
            dataSrc: 'data',
            rowId: 'key',
            data: {
                singleDocuments: false,
                reportName: ""
            }
        },
        columns: [
                    {
                        data: 'reportName',
                        name: 'Raport',
                        type: 'html',
                        render: function (data, type, row, meta) {
                            if (reports.length > 0 && reports[reports.length - 1] !== data ||
                                reports.length === 0) {
                                reports.push(data);
                            }
                            return '<span class="glyphicon glyphicon-folder-close"></span> ' + data;
                        }
                    },
                    {
                        data: 'count',
                        name: "Liczba dokumentów"
                        //type: 'html',
                        //render: function (data, type, row, meta) {
                        //    return '<a href="' + row.url + '" class="wg-link-default"> ' + data + '</a>';
                        //}
                    },

                    //{
                    //    data: 'name',
                    //    name: "Dokument",
                    //    type: 'html',
                    //    render: function (data, type, row, meta) {
                    //        return '<a href="' + row.url + '" class="wg-link-default"> ' + data + '</a>';
                    //    }
                    //},
                    {
                        data: null,
                        name: "Akcje",
                        orderable: false,
                        searchable: false,
                        type: 'html',
                        render: function (data, type, row, meta) {
                            content  = '<div class="pull-right">';
                            content += '<button class="btn btn-default btn-sm report-delete" type="button" title="Usuń">' +
                                            '<span class="glyphicon glyphicon-trash"></span>' +
                                          '</button>';
                            content += '<button class="btn btn-default btn-sm report-download" type="button" title="Pobierz">' +
                                         '<span class="glyphicon glyphicon-download-alt"></span>' +
                                       '</button>';
                            content += '</div>';
                            return content;
                            }
                    }
        ],
        stateSave: true,
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"]],
        processing: true,
        dom: "<'row'<'col-sm-6'B><'col-sm-6'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row'<'col-sm-3'l><'col-sm-3'i><'col-sm-6'p>>",
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
        {
            text: '<span class="text-primary glyphicon glyphicon-refresh" title="Odśwież zawartość"></span>',
            className: 'btn-sm',
            action: function (e, dt, node, config) {
                dt.ajax.reload();
                dt.columns.adjust().draw();
            }
        },
        {
            text: '<span class="glyphicon glyphicon-plus" title="Utwórz nowy raport"></span>',
            className: 'btn-sm btn-primary',
            action: function (e, dt, node, config) {
                $(this).attr("disabled", "disabled");
                onReportCreate(dt);
                $(this).removeAttr("disabled");
            }
        }
        //{
        //    extend: 'pageLength',
        //    text: 'page len',
        //    className: 'btn-sm text-primary'
        //},
        //{
        //    extend: 'colvis',
        //    text: '<span class="glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
        //    className: 'btn-sm text-primary'
        //}
        ],
        select: false,
        //colReorder: {
        //    fixedColumnsRight: 1
        //},
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
            

            var context = $('div#InspectionSummaryTable_filter');
            $('*', context).addClass('small');

            context = $('div#InspectionSummaryTable_length');
            $('label', context).addClass('small');

            $('select', context).addClass('small');
            //$('select', context).addClass('small btn btn-default btn-sm');
            // $('select', context).removeClass('form-control');

            context = $('div#InspectionSummaryTable_info');
            context.addClass('small');
            $('*', context).addClass('small');

            context = $('div#InspectionSummaryTable_paginate');
            $('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");

            $('.parentrow').closest('tr').each(function () {
                var row = table.row(this);
                childrows = $(this).closest('tr').nextUntil('.parentrow');

                row.child(childrows).hide();
            });
        },
        createdRow: function( row, data, dataIndex ) {
            if ( data.name === "" && data.url === "" ) {
                $(row).addClass( 'parentrow' );
            }
        }
    });

});

$('body').on('click', 'button.report-delete', function () { $(this).attr("disabled", "disabled"); onReportDelete($('#InspectionSummaryTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled"); });
$('body').on('click', 'button.report-download', function () { $(this).attr("disabled", "disabled"); onReportDownload($('#InspectionSummaryTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled");});

function onReportDownload(row){
    var report = row.data().reportName;

    //get modal with form
    var getModal = $.ajax({
        type: "GET",
        url: "InspectionSummaryList",
        data: { singleDocuments: true,
                reportName: report
               },
        dataType: "json"
    });
    getModal.done(function (data, success) {
        data.data.forEach(function (row) {
            window.open(row.url, '_blank');
            //var link = document.createElement('a');
            //link.href = row.url;
            //link.style.display = 'none';
            //document.body.appendChild(link);
            //link.click();
            //delete link;
        });
    });
    getModal.fail(function (xhr, status, error) {
        WgTools.alert(error, false, "E");
    });
}

function onReportCreate(dt) {
    //get modal with form
    var getModal = $.ajax({
        type: "GET",
        url: "InspectionSummaryCreateAjax",
        dataType: "html"
    });

    getModal.done(function (data, success) {
        //insert new modal after user delete modal
        $('#ReportDeleteModal').after(data);

        var context = $("#ReportCreateModal");
        var $form = $("form", context);

        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);

        $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

        context.modal('show');

        $('body').on('click', 'button.report-create-cancel', onReportCreateCancel);
        $('body').on('click', 'button.report-create-submit', onReportCreateSubmit);

        $('body').on('change', 'select.data-project', onProjectChange);
        //$('body').on('change', 'button.report-create-submit', onContractChange);


    });
    getModal.fail(function (xhr, status, error) {
        //onAjaxGetFailure(xhr, status, error);
        WgTools.alert(error, false, "E");
    });
}

function onReportCreateCancel() {
    DestroyModal("#ReportCreateModal");
}
function onReportCreateSubmit() {
    var $form = $("form", $("#ReportCreateModal"));
    var $fields = $(':input:not([type=hidden],[hidden],[readonly])', $form);
    var $submit = $('button.report-create-submit');
    var submitTitle = $submit.text();

    $form.validate({ lang: 'pl' });

    if ($form.valid()) {
        //Prepare form data
        var formData = $form.serialize();

        //disable editing
        $fields.each(function () {
            $(this).attr('disabled', 'disabled');
        });

        $submit.text("Trwa generowanie...");

        //Post Data
        var submit = $.ajax({
            type: $form.attr('data-ajax-method') || "POST",
            url: $form.attr("action"),
            data: formData,
            timeout: 600000
        });

        submit.fail(function (error) {
            WgTools.alert(error, false, 'E');
            //Process errors
            //var $ul = $('div.validation-summary-valid.text-danger > ul');

            //$ul.empty();

            //$.each(error.responseJSON, function (idx, data) {
            //    for (var i = 0; i < data.length; i++) {
            //        $ul.append('<li>' + data[i] + '</li>');
            //    }

            //    //this is when modelstate.toList()
            //    //var errors = data.value.errors;
            //    //if (errors.length > 0) {
            //    //    for (var i = 0; i < errors.length; i++) {
            //    //        $ul.append('<li>' + errors[i].errorMessage + '</li>');
            //    //    }
            //    //}
            //});

            //unlock form to fix errors
            $fields.each(function () {
                $(this).removeAttr('disabled');
            });
            $submit.text(submitTitle);
        });


        submit.done(function () {
            //Post was successful

            //close modal
            DestroyModal("#ReportCreateModal");
            //refresh table
            $('#InspectionSummaryTable').DataTable().ajax.reload();
            //popup message
            WgTools.alert("Pomyślnie wygenerowano nowy raport", true, 'S');
        });
    }
}
function onProjectChange() {

    var newId = $(this).val();
    var currentId = $(this).attr("data-project");

    if (newId !== currentId) {
        //
        $(this).attr("data-project", newId);

        //
        var getContracts = $.ajax({
            type: "GET",
            url: "/Contracts/GetContractsAjax",
            data: {
                "projectId": newId
            },
            dataType: 'json',
            contentType: 'application/json'
        });

        getContracts.done(function (data, success) {
            var contracts = document.getElementById("ContractId");

            //clear current options
            while (contracts.options.length) {
                contracts.remove(0);
            }

            //add empty option
            var opt = new Option("", "");
            contracts.options.add(opt);

            //fill with new data
            $.each(data, function (key, item) {

                var opt = new Option(item.text, item.value);
                contracts.options.add(opt);
            });

        });
        getContracts.fail(function (xhr, status, error) {
            WgTools.alert(error, true, 'E');
        });
    }
}
//---------------------------------------------------------------------------//
//Common functions for all actions
function DestroyModal(modalId) {
    var context = $(modalId);
    context.one('hidden.bs.modal', function () { context.remove(); });
    context.modal('hide');
}