//
$(document).ready(function () {
    var table = $('#UserListTable').DataTable({
        ajax: {
            url: "List",
            dataSrc: "data"
        },
        rowId: "id",
        columns: [
                    {
                        data: "null",
                        name: "fullName",
                        title: "Nazwisko i Imię",
                        render: function (data, type, row, meta) {
                            return row.lastName + ' ' + row.firstName;
                        },
                        searchable: true,
                        orderable: true
                    },
                    {
                        data: "userName",
                        name: "userName",
                        title: "Login",
                        searchable: true,
                        orderable: true
                    },
                    {
                        data: "email",
                        name: "email",
                        title: "E-mail",
                        type: "html",
                        render: function (data, type, row, meta) {
                            return '<a href="mailto:' + data + '" class="">' + data + '</a>';
                        },
                        searchable: true,
                        orderable: true,
                        visible: false
                    },

                    {
                        data: "emailConfirmed",
                        name: "emailConfirmed",
                        title: "e-mail zweryfikowano",
                        render: function (data, type, row, meta) {
                            return data === true ?
                                '<span class="glyphicon glyphicon-ok"></span>' :
                                '<span class="glyphicon glyphicon-remove"></span>';
                        },
                        searchable: false,
                        orderable: true,
                        visible: false
                    },
                    {
                        data: "phoneNumber",
                        name: "phoneNumber",
                        title: "Telefon",
                        searchable: true,
                        visible: false
                    },


                    {
                        data: "_roles",
                        name: "role",
                        title: "Rola",
                        render: function (data, type, row, meta) {
                            var roles = data;
                            var content = "";
                            roles.forEach(function (currentValue, index, arr) {
                                content += currentValue.name;
                            });

                            return content;
                        },
                        searchable: false,
                        orderable: true,
                        visible: true
                    },
                    {
                        data: "accessFailedCount",
                        name: "accessFailedCount",
                        title: "Nieudanych prób logowania",
                        searchable: true,
                        orderable: true,
                        visible: false

                    },
                    //{
                    //    data: "lockoutEnabled",
                    //    name: "lockoutEnabled",
                    //    title: "Blokada",
                    //    render: function (data, type, row, meta) {
                    //        return data === true ?
                    //            '<span class="glyphicon glyphicon-ok"></span>' :
                    //            '<span class="glyphicon glyphicon-remove"></span>';
                    //    },
                    //    searchable: false,
                    //    orderable: false,
                    //    visible: true

                    //},
                    {
                        data: "lockoutEnd",
                        name: "lockoutEnd",
                        title: "Zablokowane do",
                        searchable: false,
                        orderable: true,
                        visible: false
                    },
                    {
                        data: null,
                        name: "actions",
                        type: 'html',
                        render: function (data, type, row, meta) {
                            var currentDate = new Date();
                            var lockDate = row.lockoutEnd === "" ? currentDate : new Date(Date.parse(row.lockoutEnd));

                            var content = "";
                            content = '<div class="pull-right">';
                            content += '<button title="Edytuj dane" class="btn btn-default btn-sm user-edit" type="button">' +
                                            '<span class="glyphicon glyphicon-pencil"></span>' +
                                        '</button>';
                            if ( lockDate <= currentDate && row.lockoutEnabled === true) {
                                content += '<button title="Zablokuj konto" class="btn btn-default btn-sm user-lock" type="button">' +
                                                '<i class="fa fa-lock"></i>' +
                                           '</button>';
                            }
                            else if (lockDate <= currentDate && row.lockoutEnabled === false) {
                                content += '<button title="Zablokuj konto" class="btn btn-default btn-sm disabled" type="button" disabled>' +
                                    '<i class="fa fa-lock"></i>' +
                                    '</button>';
                            }
                            else {
                                content += '<button title="Odblokuj konto" class="btn btn-default btn-sm user-unlock" type="button">' +
                                           '<i class="fa fa-unlock"></i>' +
                                           '</button>';
                            }
                            content += '<button class="btn btn-default btn-sm user-delete" type="button">' +
                                            '<span class="glyphicon glyphicon-trash"></span>' +
                                       '</button>';
                            content += '</div>';
                            return content;
                        },
                        width: "100px",
                        autoWidth: false,
                        searchable: false,
                        orderable: false,
                        visible: true
                    }
        ],
        stateSave: true,
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
        //{
        //    extend: 'pageLength',
        //    text: 'page len',
        //    className: 'btn-sm text-primary'
        //},
        {
            extend: 'colvis',
            text: '<span class="text-primary glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
            className: 'btn-sm'
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
            text: '<span class="glyphicon glyphicon-plus" title="Dodaj nowego użytkownika"></span>',
            className: 'btn-sm btn-primary',
            action: function (e, dt, node, config) {
                $(this).attr("disabled", "disabled");
                onUserCreate(dt);
                $(this).removeAttr("disabled");
            }
        }
        ],
        select: false,
        colReorder: {
            fixedColumnsRight: 1
        },
        responsive: true,
        columnDefs: [
          { responsivePriority: 1, targets: 0 },
          { responsivePriority: 400, targets: 1 },
          { responsivePriority: 500, targets: 2 },
          { responsivePriority: 600, targets: 3 },
          { responsivePriority: 700, targets: 4 },
          { responsivePriority: 800, targets: 5 },
          { responsivePriority: 900, targets: 6 },
          { responsivePriority: 900, targets: 7 },
          { responsivePriority: 1, targets: 8 }
        ],
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
            

            var context = $('div#UserListTable_filter');
            $('*', context).addClass('small');

            context = $('div#UserListTable_length');
            $('label', context).addClass('small');

            $('select', context).addClass('small');
            //$('select', context).addClass('small btn btn-default btn-sm');
            // $('select', context).removeClass('form-control');

            context = $('div#UserListTable_info');
            context.addClass('small');
            $('*', context).addClass('small');

            context = $('div#UserListTable_paginate');
            $('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");
        }
    });

});
///////////////////////////////////////////////////////////////////////////////
$('body').on('click', 'button.user-edit', function () { $(this).attr("disabled", "disabled"); onUserEdit($('#UserListTable').DataTable().row($(this).parents('tr')).id()); $(this).removeAttr("disabled"); });
$('body').on('click', 'button.user-lock', function () { $(this).attr("disabled", "disabled"); onUserLock($('#UserListTable').DataTable().row($(this).parents('tr')).id()); $(this).removeAttr("disabled"); });
$('body').on('click', 'button.user-unlock', function () { $(this).attr("disabled", "disabled"); onUserUnlock($('#UserListTable').DataTable().row($(this).parents('tr')).id()); $(this).removeAttr("disabled"); });
$('body').on('click', 'button.user-delete', function () { $(this).attr("disabled", "disabled"); onUserDelete($('#UserListTable').DataTable().row($(this).parents('tr')).id()); $(this).removeAttr("disabled"); });
///////////////////////////////////////////////////////////////////////////////
//---------------------------------------------------------------------------//
function onUserCreate(dt) {
    //get modal with form
    var getModal = $.ajax({
        type: "GET",
        url: "Create",
        dataType: "html"
    });

    getModal.done(function (data, success) {
        //insert new modal after user delete modal
        $('#UserDeleteModal').after(data);

        var context = $("#MaintainUserModal");
        var $form = $("form", context);

        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);

        $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

        context.modal('show');

        $('body').on('click', 'button.user-create-cancel', onUserCreateCancel);
        $('body').on('click', 'button.user-create-submit', onUserCreateSubmit);

    });
    getModal.fail(function (xhr, status, error) {
        //onAjaxGetFailure(xhr, status, error);
        WgTools.alert(error, false, "E");
    });
}
function onUserEdit(userId) {

    //get modal with form
    var getModal = $.ajax({
        type: "GET",
        url: "Edit",
        data: { "userId": userId },
        dataType: "html"
    });

    getModal.done(function (data, success) {
        //insert new modal after user delete modal
        $('#UserDeleteModal').after(data);

        var context = $("#MaintainUserModal");
        var $form = $("form", context);

        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);

        $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

        context.modal('show');

        $form.on('click', 'button.user-edit-cancel', onUserEditCancel);
        $form.on('click', 'button.user-edit-submit', onUserEditSubmit);

    });

    getModal.fail(function (xhr, status, error) {
        WgTools.alert(error, false, "E");
    });

    $(this).bind();
}
function onUserDelete(userId) {
    $('body').one('click', 'button.user-delete-submit', { userId: userId } , onUserDeleteSubmit);
    $('#UserDeleteModal').modal('show');
}
function onUserLock(userId) {
    //get modal with form
    var token = $('input[name="__RequestVerificationToken"]').val();

    var deleteAction = $.ajax({
        type: "POST",
        url: "Lock",
        data: {
            Id: userId,
            __RequestVerificationToken: token
        }
    });

    deleteAction.done(function () {
        //Post was successful

        //close modal
        //DestroyModal("#MaintainUserModal");
        //refresh table
        $('#UserListTable').DataTable().ajax.reload();
        //popup message
        WgTools.alert("Konto użytkownika zostało zablokowane.", true, 'S');
    });

    deleteAction.fail(function (xhr, textStatus, error) {

        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });
}
function onUserUnlock(userId) {
    //get modal with form
    var token = $('input[name="__RequestVerificationToken"]').val();

    var deleteAction = $.ajax({
        type: "POST",
        url: "UnLock",
        data: {
            Id: userId,
            __RequestVerificationToken: token
        }
    });

    deleteAction.done(function () {
        //Post was successful

        //close modal
        //DestroyModal("#MaintainUserModal");
        //refresh table
        $('#UserListTable').DataTable().ajax.reload();
        //popup message
        WgTools.alert("Konto użytkownika zostało odblokowane.", true, 'S');
    });

    deleteAction.fail(function (xhr, textStatus, error) {

        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });
}
//---------------------------------------------------------------------------//
// User Edit \ Cancel | Submit
function onUserEditCancel() {
    DestroyModal("#MaintainUserModal");
}
function onUserEditSubmit() {
    //var context = $("#MaintainUserModal");
    var $form = $("form", $("#MaintainUserModal"));

    var $fields = $(':input:not([type=hidden],[hidden],[readonly])', $form);
    var $submit = $('button.user-edit-submit');
    var submitTitle = $submit.text();

    $form.validate({ lang: 'pl' });

    if ($form.valid()) {
        //Prepare form data
        var formData = $form.serialize();

        //disable editing
        $fields.each(function () {
            $(this).attr('disabled', 'disabled');
        });

        $submit.text("Trwa zapisywanie...");

        //var url = $form.attr("action"),
        //    method = $form.attr('data-ajax-method'),
        //    ajaxTrue = $form.attr('data-ajax');

        //Post Data
        var submit = $.ajax({
            type: $form.attr('data-ajax-method') || "POST",
            url: $form.attr("action"),
            data: formData
        });

        submit.fail(function (error) {
            //Process errors
            var $ul = $('div.validation-summary-valid.text-danger > ul');

            $ul.empty();

            $.each(error.responseJSON, function (idx, data) {
                for (var i = 0; i < data.length; i++) {
                    $ul.append('<li>' + data[i] + '</li>');
                }

                //this is when modelstate.toList()
                //var errors = data.value.errors;
                //if (errors.length > 0) {
                //    for (var i = 0; i < errors.length; i++) {
                //        $ul.append('<li>' + errors[i].errorMessage + '</li>');
                //    }
                //}
            });

            //unlock form to fix errors
            $fields.each(function () {
                $(this).removeAttr('disabled');
            });
            $submit.text(submitTitle);
        });

        submit.done(function () {
            //Post was successful

            //close modal
            DestroyModal("#MaintainUserModal");
            //refresh table
            $('#UserListTable').DataTable().ajax.reload();
            //popup message
            WgTools.alert("Pomyślnie zapisano dane konta użytkownika", true, 'S');
        });
    }
}
//---------------------------------------------------------------------------//
// User Create \ Cancel | Submit
function onUserCreateCancel() {
    DestroyModal("#MaintainUserModal");
}
function onUserCreateSubmit() {
    var $form = $("form", $("#MaintainUserModal"));
    var $fields = $(':input:not([type=hidden],[hidden],[readonly])', $form);
    var $submit = $('button.user-create-submit');
    var submitTitle = $submit.text();

    $form.validate({ lang: 'pl' });

    if ($form.valid()) {
        //Prepare form data
        var formData = $form.serialize();

        //disable editing
        $fields.each(function () {
            $(this).attr('disabled', 'disabled');
        });

        $submit.text("Trwa zapisywanie...");

        //Post Data
        var submit = $.ajax({
            type: $form.attr('data-ajax-method') || "POST",
            url: $form.attr("action"),
            data: formData
        });

        submit.fail(function (error) {
            //Process errors
            var $ul = $('div.validation-summary-valid.text-danger > ul');

            $ul.empty();

            $.each(error.responseJSON, function (idx, data) {
                for (var i = 0; i < data.length; i++) {
                    $ul.append('<li>' + data[i] + '</li>');
                }

                //this is when modelstate.toList()
                //var errors = data.value.errors;
                //if (errors.length > 0) {
                //    for (var i = 0; i < errors.length; i++) {
                //        $ul.append('<li>' + errors[i].errorMessage + '</li>');
                //    }
                //}
            });

            //unlock form to fix errors
            $fields.each(function () {
                $(this).removeAttr('disabled');
            });
            $submit.text(submitTitle);
        });


        submit.done(function () {
            //Post was successful

            //close modal
            DestroyModal("#MaintainUserModal");
            //refresh table
            $('#UserListTable').DataTable().ajax.reload();
            //popup message
            WgTools.alert("Pomyślnie utworzono nowe konto użytkownika", true, 'S');
        });
    }
}
//---------------------------------------------------------------------------//
function onUserDeleteCancel() {

}
function onUserDeleteSubmit(event) {
    //get modal with form
    var token = $('input[name="__RequestVerificationToken"]').val();

    var deleteAction = $.ajax({
        type: "POST",
        url: "Delete",
        data: {
            Id: event.data.userId,
            __RequestVerificationToken: token
        }
    });

    deleteAction.done(function () {
        //Post was successful

        //close modal
        //DestroyModal("#MaintainUserModal");
        //refresh table
        $('#UserListTable').DataTable().ajax.reload();
        //popup message
        WgTools.alert("Konto użytkownika zostało usunięte.", true, 'S');
    });

    deleteAction.fail(function (xhr, textStatus, error) {
        
        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }

        });
    });
}

//---------------------------------------------------------------------------//
//---------------------------------------------------------------------------//

//---------------------------------------------------------------------------//
//Common functions for all actions
function DestroyModal(modalId) {
    var context = $(modalId);
    context.one('hidden.bs.modal', function () { context.remove(); });
    context.modal('hide');
}