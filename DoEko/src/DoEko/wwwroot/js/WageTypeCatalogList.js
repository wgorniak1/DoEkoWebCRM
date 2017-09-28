//
$(document).ready(function () {
    var editMode = false;

    var table = $('#WTCTable').DataTable({
        ajax: {
            url: "/api/WageTypeDefinitions",
            dataSrc: "",
            rowId: "wageTypeDefinitionId"
        },
        columns: [
                    //{
                    //    responsivePriority: 1,
                    //    data: 'fullName',
                    //    name: 'fullName',
                    //    title: 'Nazwisko i Imię',
                    //    type: 'html',
                    //    render: function (data, type, row, meta) {
                    //        switch (type) {
                    //            case "display":
                    //                return data.length > 30 ? data.replace(' ', '<br/>') : data;
                    //            case "filter":
                    //                return data;
                    //            case "sort":
                    //                return data;
                    //        }
                    //    }
                    //},
            {
                data: "code",
                name: "code",
                title: "code"
            },
            {
                data: "shortDescription",
                name: "shortDescription",
                title: "shortDescription"
            },
            {
                data: "number",
                name: "number",
                title: "number"
            },
            {
                data: "unit",
                name: "unit",
                title: "unit"
            },
            {
                data: "rate",
                name: "rate",
                title: "rate"
            },
            {
                data: "amount",
                name: "amount",
                title: "amount"
            },
            {
                data: "currency",
                name: "currency",
                title: "currency"
            }
        ],

        stateSave: true,
        pagingType: "full",
        language: {
            url: "/js/datatables-language-pl.json"
        },

        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        //order: [[0, "asc"]],
        processing: true,
        dom: "<'row'<'col-sm-6'B><'col-sm-6'f>>" +
        "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
        //    {
        //        extend: 'copyHtml5',
        //        text: '<span class="glyphicon glyphicon-copy"></span>',
        //        className: 'btn text-primary',
        //        copySuccess: {
        //            1: 'Skopiowano 1 rekord do schowka',
        //            _: 'Skopiowano %d rekordów do schowka'
        //        },
        //        copyTitle: 'Kopiowanie do schowka',
        //        copyKeys: 'Naciśnij <i>ctrl</i> lub <i>\u2318</i> + <i>C</i> aby skopiować tabelę<br>do schowka.<br><br>aby anulować, kliknij ten komunikat lub naciśnij ESC.'
        //    },
        //    {
        //        extend: 'csvHtml5',
        //        text: '<span class="glyphicon glyphicon-download-alt" title="Export do CSV"></span>',
        //        className: 'btn text-primary',
        //        fieldSeparator: ';',
        //        charset: 'UTF-8'
        //    },
        //    {
        //        extend: 'colvis',
        //        text: '<span class="glyphicon glyphicon-th-list" title="Pokaż / Ukryj kolumny"></span>',
        //        className: 'btn text-primary'
        //    },
        //    {
        //        text: '<span class="text-primary glyphicon glyphicon-refresh" title="Odśwież zawartość"></span>',
        //        className: 'btn',
        //        action: function (e, dt, node, config) {
                    
        //            dt.ajax.reload();
                    
        //            dt.rows().cells().invalidate().render();
        //        }
        //    },
            {
                text: '<span class="glyphicon glyphicon-plus" title="Nowy Składnik Wynagrodzenia"></span> Nowy',
                className: 'btn',
                action: function (e, dt, node, config) {
                    
                    //editMode = !editMode;
                    //dt.column('dataProcessingConfirmation').cells().invalidate().render();
                    
                    //dt.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    //    if (dt.row(rowIdx).child.isShown()) {
                    //        $(dt.cell(rowIdx,0).node()).click();
                    //        $(dt.cell(rowIdx,0).node()).click();
                    //    }
                    //});
                    
                    //$('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');

                    $.get("WageTypeCatalog/Create").done(function (data) {
                        var popup     = null,
                            popupForm = null;

                        //insert html form
                        $("#ModalSection").html(data);

                        //prepare modal
                        popup = $("#WTCModal");

                        popup.modal({ backdrop: 'static', keyboard: true });

                        popup.one("hide.bs.modal", function () {
                            //force modal to hide
                            $('#WTCModal').modal('hide');
                            $('body').removeClass('modal-open');
                            $('.modal-backdrop').remove();
                            //clear modal html
                            $("#ModalSection").html("");
                        });

                        //prepare form inside modal
                        popupForm = $("form", popup);

                        popupForm.removeData("validator");
                        popupForm.removeData("unobtrusiveValidation");
                        $.validator.unobtrusive.parse(popupForm);
                        popupForm.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

                        //prepare form submit events
                        $(".wtc-btn-submit").on("click", function () {
                            var form = $("#WTCModal form");

                            if (form.length === 0) {
                                return;
                            }

                            form.validate({ lang: 'pl' });

                            if (form.valid()) {
                                //form.submit();
                                var data = {};
                                form.serializeArray().map(function (x) { data[x.name] = x.value; });
                                var id = $('[name="WageTypeDefinitionId"]', form);
                                var method = form.type || "POST";
                                var url = form.action || "api/WageTypeDefinitions";
                                var call = $.ajax({
                                    url: url,
                                    type: method,
                                    contentType: "application/json",
                                    data: data
                                });

                                call.done(WTCCreateCompleted());
                                call.fail(WTCCreateFailed());
                            }

                        });
                        //show modal
                        popupForm.modal("show");
                    });
                    
                }
            }
        ],
        //select: false,
        //colReorder: 
        //{
        //    fixedColumnsRight: 1
        //},
        ////responsive: {
        //    //breakpoints: [
        //    //    { name: 'desktop', width: 3000 },
        //    //    //{ name: 'tablet', width: 1 },
        //    //   //{ name: 'fablet', width: 1024 },
        //    //  //  { name: 'phone', width: 768 }
        //    //],
        //    //details: {
        //    //    type: 'column',
        //    //    renderer: function (api, rowIdx, columns) {
        //    //        var data = $.map(columns, function (col, i) {
        //    //            return col.hidden ?
        //    //                '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
        //    //                '<td>' + col.title + ':' + '</td> ' +
        //    //                '<td>' + col.data + '</td>' +
        //    //                '</tr>' : '';
        //    //        }).join('');
                    
        //    //        return data ?
        //    //            $('<table/>').append(data) :
        //    //            false;
        //    //    }
        //    //}
        ////},
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        }
        //drawCallback: function (settings, json) {
        //    $('div#BPPersonListTable_processing').addClass("wg-loader");

        //    //var context = $('div#BPPersonListTable_filter');
        //    //$('*', context).addClass('small');

        //    //context = $('div#BPPersonListTable_length');
        //    //$('label', context).addClass('small');

        //    //$('select', context).addClass('small');
        //    ////$('select', context).addClass('small btn btn-default btn-sm');
        //    //// $('select', context).removeClass('form-control');

        //    //context = $('div#BPPersonListTable_info');
        //    //context.addClass('small');
        //    //$('*', context).addClass('small');

        //    //context = $('div#BPPersonListTable_paginate');
        //    //$('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
        //    //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");


        //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');
        //}
    });

    //table.on('responsive-display', function (e, datatable, row, showHide, update) {
    //    $('input[name=dataProcessingConfirmation]').bootstrapToggle(editMode ? '' : 'destroy');  
    //});


});

function WTCCreateFailed(xhr, status, error) {
    WgTools.alert(error, true, 'E');
}

function WTCCreateCompleted() {
    WgTools.alert('Pomyślnie zapisano dane sekcji.', true, 'S');
}
//$('body').on('change', 'input.person-confirmationchange', onConfirmationChange);
//$('body').on('click', 'button.person-edit', function () { $(this).attr("disabled", "disabled"); onPersonEdit($('#BPPersonListTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled"); });
//$('body').on('click', 'button.person-delete', function () { $(this).attr("disabled", "disabled"); onPersonDelete($('#BPPersonListTable').DataTable().row($(this).parents('tr'))); $(this).removeAttr("disabled"); });


//function onConfirmationChange() {
//    var newValue = $(this).prop('checked');
//    var srcData = $('#BPPersonListTable').DataTable().row(this.closest('tr[role="row"]')).data();

    
//    var form = new FormData();
//    form.append("id", srcData.businessPartnerId);
//    form.append("allowed", newValue);

//    var call = $.ajax({
//        url: "DataProcessingAllowed",
//        type: "POST",
//        cache: false,
//        contentType: false,
//        processData: false,
//        data: form
//        //beforeSend: function () {
//        //    $('#ReportTemplateTable_processing').show();
//        //}
//    });

//    call.done(function (data, success) {
//        //notification popup
//        WgTools.alert("Zapisano zmiany", true, "S");

//        srcData.dataProcessingConfirmation = newValue;

//    });
//    call.fail(function (xhr, status, error) {
//        //Notification popup
//        WgTools.alert(error, true, 'E');
//    });

//}

//function onPersonDelete(row) {
//    $('body').one('click', 'button.person-delete-submit', { row: row }, onPersonDeleteSubmit);
//    $('#PersonDeleteModal').modal('show');
//}

//function onPersonEdit() {

//}

//function onPersonDeleteCancel() {

//}

//function onPersonDeleteSubmit(event) {
//    var token = $('input[name="__RequestVerificationToken"]').val();

//    var deleteAction = $.ajax({
//        type: "POST",
//        url: "Delete",
//        data: {
//            id: event.data.row.data().businessPartnerId,
//            __RequestVerificationToken: token
//        }
//    });

//    deleteAction.done(function () {
//        //Post was successful

//        //close modal
//        //DestroyModal("#MaintainUserModal");
//        //refresh table
//        event.data.row
//            .remove()
//            .draw('full-hold');

//        //popup message
//        WgTools.alert("Dane właściciela zostały usunięte.", true, 'S');
//    });

//    deleteAction.fail(function (xhr, textStatus, error) {

//        $.each(xhr.responseJSON, function (idx, data) {
//            if (data.length > 0) {
//                WgTools.alert(data, false, 'E');
//            }

//        });
//    });
//}

//function onPersonEditCancel() {

//}

//function onPersonEditSubmit() {

//}


/////////////////////////////////////////////////////////////////////////////////
//// Action buttons:
////  + Upload new template
////    description:
////      1. user clicked button
////      2. system takes record key that was set during table initialization
////      3. system and put record key into name and id of file input
////      4. system pass click on file input
////      5. user selects file or aborts action
////      6. system validates selected file 
////      7. system calls ajax to upload file
////      8. once uploaded system reloads table content
////  + Delete existing template
//// all files have unique "template names". 
////
//$('body').on('change', '.template-input', onTemplateInputChange);
//$('body').on('click', '.template-upload', onTemplateAddClick);
//$('body').on('click', '.template-delete', onTemplateDeleteClick);

////---------------------------------------------------------------------------//
//function onTemplateAddClick(event) {
//    //1. set template id
//    var table = $('#ReportTemplateTable').DataTable();
//    var data = table.row($(this).parents('tr')).data();
//    var input = $("form#TemplateUpload > input[type='file']");

//    input.attr('id', data.key);
//    input.attr('name', data.key);

//    //2. click on input
//    input.click();
//}
////---------------------------------------------------------------------------//
//function onTemplateInputChange() {
//    var allowedTypes = ["application/msword",
//                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                        "application/vnd.openxmlformats-officedocument.wordprocessingml.template"];
//    var input = $(this);
//    var file = input[0].files[0];

//    if (file !== undefined && !allowedTypes.includes(file.type)) {
//        WgTools.alert("Niedozwolony format pliku!", true, "E");
//        return;
//    }

//    //each row is named with template section name
//    var sectionName = input.attr('name');

//    //upload: "Templates / Surveysummary / Title / filename
//    UploadTemplate(sectionName, file);
//}

////---------------------------------------------------------------------------//
//function UploadTemplate(sectionName, docfile) {

//    var form = new FormData();
//    var templateName = "InspectionSummary";

//    form.append("templateName", templateName);
//    form.append(sectionName, docfile);

//    var call = $.ajax({
//        url: "Create",
//        type: "POST",
//        cache: false,
//        contentType: false,
//        processData: false,
//        data: form,
//        beforeSend: function () {
//            $('#ReportTemplateTable_processing').show();
//        }
//    });

//    call.done(function (data, success) {
//        //notification popup
//        WgTools.alert("Pomyślnie wczytano plik szablonu", true, "S");
//        //refresh table content
//        var table = $('#ReportTemplateTable').DataTable();
//        table.ajax.reload(null, false);
//    });
//    call.fail(function (xhr, status, error) {
//        //Notification popup
//        WgTools.alert(error, true, 'E');
//    });

//}

////function onUploadTemplateCompleted(data, status, sectionName, file) {
////    var targetUrl = data;
////    var fileName = targetUrl.split('/').reverse()[0];
////
////    var row = $('tr[name="' + sectionName + '"]').first();
////    var columnLink = $('td[data-column="TemplateLink"]', row).first();
////    var columnManage = $('td[data-column="TemplateManage"]', row).first();
////
////    // create document link
////    var link = document.createElement('a');
////    link.setAttribute('href', targetUrl);
////    link.setAttribute('class','wg-link-nounderline template-link');
////    
////    var linkIcon = document.createElement("span");
////    linkIcon.setAttribute('class', 'glyphicon glyphicon-link');
////    var linkText = document.createTextNode(fileName);
////    
////    link.appendChild(linkIcon);
////    link.appendChild(linkText);
////        
////    // replace place holder with link
////    columnLink.html('');
////    columnLink.append(link);
////    
////
////    // create button to delete
////    var delButton = document.createElement('button');
////    delButton.setAttribute('class', 'btn btn-default btn-sm template-delete');
////    delButton.setAttribute('type', 'button');
////    var delButtonIcon = document.createElement('span');
////    delButtonIcon.setAttribute('class', 'glyphicon glyphicon-trash')
////
////    delButton.appendChild(delButtonIcon);
////
////    // replace upload button with delete button
////    columnManage.children().first().html('');
////    columnManage.children().first().append(delButton);
////
////    //notification popup
////    WgTools.alert("Pomyślnie wczytano plik szablonu", true, "S");
////}
////
////function onUploadTemplateFailed(xhr, status, error) {
////    WgTools.alert(error, true, 'E');
////}
/////////////////////////////////////////////////////////////////////////////////
////---------------------------------------------------------------------------//
//function onTemplateDeleteClick() {
//    //1. set template id
//    var table = $('#ReportTemplateTable').DataTable();
//    var data = table.row($(this).parents('tr')).data();

//    //2. confirm decision
//    var modal = $('#TemplateDeleteModal').modal("show");

//    //3. if confirmed
//    $('body').one('click', '.template-delete-confirmed', data.key, DeleteTemplate);
//}

//function DeleteTemplate(event) {

//    var templateName = "InspectionSummary";
//    var templateSection = event.data;

//    var form = new FormData();

//    form.append("templateName", templateName);
//    form.append("templateSection", templateSection);

//    var call = $.ajax({
//        url: "Delete",
//        type: "POST",
//        cache: false,
//        contentType: false,
//        processData: false,
//        data: form
//    });

//    call.done(function (data, success) {
//        //notification popup
//        WgTools.alert("Usunięto szablon", true, "S");

//        //refresh table content
//        var table = $('#ReportTemplateTable').DataTable();
//        table.ajax.reload(null, false);

//    });
//    call.fail(function (xhr, status, error) {
//        //Notification popup
//        WgTools.alert(error, true, 'E');
//    });
//}

////function onDeleteTemplateCompleted(data, status, name) {
////    var a = $('a[name="' + name + '"]');
////    // image link
////    a.removeAttr('href');
////    a.removeAttr('target');
////    // image
////    a.children('img').first().attr('src', '');
////    a.children('img').first().attr('alt', 'Dodaj zdjęcie');
////    // image delete button
////    a.siblings('button.photo-delete').first().hide();
////    a.siblings('form').first().children('input[type="file"]').first().val('');
////
////    //<button class="btn btn-default btn-sm template-upload">
////    //    <span class="glyphicon glyphicon-plus"></span>
////    //</button>
////    //<form action="" method="post" enctype="multipart/form-data" hidden>
////    //    <input type="file" id="@item.Key.ToString()" name="@item.Key.ToString()" class="template-input" accept="application/msword">
////    //    <input type="submit" value="Upload">
////    //</form>
////
////
////}
////
////function onDeleteTemplateFailed(xhr, status, error) {
////    WgTools.alert(error, true, 'E');
////}
/////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
////function turnLoader(turnOn) {
////    //var a = $('a[name="' + imageName + '"]');
////    // image
////    //var img = a.children('img').first();
////
////    var ajaxLoaderDiv = $('#AjaxLoader');
////
////    if (turnOn) {
////        ajaxLoaderDiv.addClass('wg-loader');
////        //img.removeClass('wg-image-placeholder');
////        //img.addClass('wg-loader');
////    }
////    else {
////        ajaxLoaderDiv.removeClass('wg-loader');
////        //img.removeClass('wg-loader');
////        //img.addClass('wg-image-placeholder');
////    }
////
////}
