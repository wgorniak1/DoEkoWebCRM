//
$(document).ready(function () {
    var editMode = false;

    var table = $('#BPPersonListTable').DataTable({
        ajax: {
            url: "List",
            dataSrc: "data",
            rowId: "businessPartnerId"
        },
        columns: [
                    {
                        data: "fullName",
                        name: "fullName",
                        title: "Nazwisko i Imię",
                    },
                    {
                        data: "pesel",
                        name: "pesel",
                        title: "Pesel",
                    },
                    {
                        data: "idNumber",
                        name: "idNumber",
                        title: "Nr dowodu os.",
                    },
                    {
                        data: "taxId",
                        name: "taxId",
                        title: "NIP",
                    },
                    {
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
                        data: "phoneNumber",
                        name: "phoneNumber",
                        title: "Telefon",
                        type: "html-num",
                        render: function (data, type, row, meta) {
                            return data ? '<a href="tel:' + data.replace(/\s+/g, '') + '">' + data + '</a>' : '';
                        }
                    },
                    {
                        data: "address",
                        data: "address",
                        title: "Adres",
                        type: "string",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case "display":
                                    return data.firstLine + '<br/>' + data.secondLine;
                                case "filter":
                                    return data.singleLine;
                                case "sort":
                                    return data.singleLine;
                            }
                        }
                    },
                    {
                        data: "dataProcessingConfirmation",
                        name: "dataProcessingConfirmation",
                        title: "Zgoda na przetw. danych",
                        type: "string",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case "display":
                                    if (editMode) {
                                        var content = 
                                        '<input type="checkbox" id="dataProcessingConfirmation['+row.businessPartnerId+']" name="dataProcessingConfirmation" class="form-control checkbox confirmationchange" data-toggle="toggle" data-on="Tak" data-off="Nie" value="' + data + '"';
                                        content += !data ? '/>' : 'checked />';
                                        return content;
                                    }
                                    else 
                                    return !data ? "Nie" : "Tak";
                                case "filter":
                                    return !data ? "Nie" : "Tak";
                                case "sort":
                                    return !data ? "Nie" : "Tak";                            }
                            
                        }
                    },
                    {
                        data: "investments",
                        name: "investmentsCount",
                        title: "Liczba inwestycji",
                        type: "num",
                        render: function (data, type, row, meta) {
                            return data.length;
                        }
                    },
                    {
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
                                            '<a href="/Investments/Details/' + investment.investmentId + '" target="_blank">' + investment.address.singleLine + '</a ></td ></tr > ';
                                    });
                                    return '<table>' + content + '</table>';
                                case "filter":
                                    data.forEach(function (investment, index, arr) {
                                        content += investment.address.singleLine + ';';
                                    });

                                    return content;
                                case "sort":
                                    data.forEach(function (investment, index, arr) {
                                        content += investment.address.singleLine + ';';
                                    });
                                    return bdt;
                            }

                        }
                    }
                    //{
                    //    data: null,
                    //    name: "Akcja",
                    //    orderable: false,
                    //    searchable: false,
                    //    type: 'html',
                    //    render: function (data, type, row, meta) {
                    //        var content = "";
                    //        if (data.name !== null) {

                    //            return '<button class="btn btn-default btn-sm template-delete" type="button">' +
                    //                        '<span class="glyphicon glyphicon-trash"></span>' +
                    //                      '</button>';
                    //        }
                    //        else {
                    //            return '<button class="btn btn-default btn-sm template-upload">' +
                    //                     '<span class="glyphicon glyphicon-plus"></span>' +
                    //                   '</button>';
                    //        }

                    //    }
                    //}
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
                    dt.rows().invalidate().render();

                   // $('input[name=GrossNetFundsType]').bootstrapToggle( (editMode) ? '' : 'destroy');
                }
            },
            {
                text: '<span class="text-primary glyphicon glyphicon-pencil" title="Edytuj zgody..."></span>',
                className: 'btn-sm',
                action: function (e, dt, node, config) {
                    editMode = !editMode;
                    //dt.ajax.reload();
                    dt.column('dataProcessingConfirmation').cells().invalidate().render();
                    //dt.draw('full-reset');

                    dt.rows().every(function (rowIdx, tableLoop, rowLoop) {
                        if (dt.row(rowIdx).child.isShown()) {
                            $(dt.cell(rowIdx,0).node()).click();
                            $(dt.cell(rowIdx,0).node()).click();
                        }
                    });
                    
                    $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');

                }
            },
        ],
        select: true,
        colReorder: true,
        //{
        //    fixedColumnsRight: 1
        //},
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

            var context = $('div#BPPersonListTable_filter');
            $('*', context).addClass('small');

            context = $('div#BPPersonListTable_length');
            $('label', context).addClass('small');

            $('select', context).addClass('small');
            //$('select', context).addClass('small btn btn-default btn-sm');
            // $('select', context).removeClass('form-control');

            context = $('div#BPPersonListTable_info');
            context.addClass('small');
            $('*', context).addClass('small');

            context = $('div#BPPersonListTable_paginate');
            $('ul > li > a', context).addClass('small').attr("style", "padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");


            $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');
        }
    });

    table.on('responsive-display', function (e, datatable, row, showHide, update) {
        $('input[name=dataProcessingConfirmation]').bootstrapToggle((editMode) ? '' : 'destroy');  
    });


});


$('body').on('change', 'input.confirmationchange', onConfirmationChange);

function onConfirmationChange() {
    var newValue = $(this).prop('checked');
    var rowIndex = $(this).closest('tr').index();
    var srcData = $('#BPPersonListTable').dataTable().api().row(rowIndex).data();

    
    var form = new FormData();
    form.append("id", srcData.businessPartnerId);
    form.append("allowed", newValue);

    var call = $.ajax({
        url: "DataProcessingAllowed",
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        data: form
        //beforeSend: function () {
        //    $('#ReportTemplateTable_processing').show();
        //}
    });

    call.done(function (data, success) {
        //notification popup
        WgTools.alert("Zapisano zmiany", true, "S");

        srcData.dataProcessingConfirmation = newValue;

    });
    call.error(function (xhr, status, error) {
        //Notification popup
        WgTools.alert(error, true, 'E');
    });

}


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
//    call.error(function (xhr, status, error) {
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
//    call.error(function (xhr, status, error) {
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
