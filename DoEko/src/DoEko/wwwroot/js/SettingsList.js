//
$(document).ready(function () {
    var table = $('#ConstantsListTable').DataTable({
        ajax: {
            url: "api/v1/controlparameter",
            dataSrc: ""
        },
        rowId: "id",
        columns: [
            {
                data: "name",
                name: "Name",
                title: "Nazwa",
                render: function (data, type, row, meta) {
                    return data;;
                },
                searchable: true,
                orderable: true
            },
            {
                data: "description",
                name: "Description",
                title: "Opis",
                searchable: true,
                orderable: true
            },
            {
                data: "value",
                name: "Value",
                title: "Wartość",
                type: "string",
                render: function (data, type, row, meta) {
                    return data;
                },
                searchable: true,
                orderable: true,
                visible: true
            },
        ],
        stateSave: false,
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"]],
        processing: true,
        dom: "<'row'<'col-sm-6'B><'col-sm-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
            {
                extend: 'copyHtml5',
                text: '<span class="text-primary glyphicon glyphicon-copy"></span>',
                className: 'btn-sm text-primary'
            },
            {
                extend: 'csvHtml5',
                text: '<span class="text-primary glyphicon glyphicon-download-alt" title="Export do CSV"></span>',
                className: 'btn-sm text-primary'
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-pencil" title="Edytuj"></span>',
                className: 'btn',
                action: function (e, dt, node, config) {

                }
            },
            {
                text: '<span class="text-primary glyphicon" title="Zapisz"></span>',
                className: 'btn disabled',
                action: function (e, dt, node, config) {

                }
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
                text: '<span class="glyphicon glyphicon-plus" title="Dodaj"></span>',
                className: 'btn-sm btn-primary',
                action: function (e, dt, node, config) {
                    $(this).attr("disabled", "disabled");
                    onEntryCreate(dt);
                    $(this).removeAttr("disabled");
                }
            }
        ],
        select: false,
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
        }
    });

});

function onEntryCreate(dt) {

}