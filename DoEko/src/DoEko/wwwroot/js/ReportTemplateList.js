//
$(document).ready(function () {
    var table = $('#ReportTemplateTable').DataTable({
        ajax: { 
            url: "List",
            dataSrc: "data",
            rowId: "key"
        },
        columns: [
                    {
                        data: "keyText",
                        name: "Szablon - Sekcja",
                    },
                    { 
                        data: null,
                        name: "kolumna 2",
                        type: 'html',
                        render: function (data, type, row, meta) {
                            if (data.name !== null) {
                                return '<a href="'+data.url+'" class="wg-link-default">'+data.name+'</a>';
                            }
                            else {
                                return '<span class="text-danger">Brak</span>';
                            }
                        }
                    },
                    {
                        data: null,
                        name: "Akcja",
                        orderable: false,
                        searchable: false,
                        type: 'html',
                        render: function (data, type, row, meta) {
                            var content = "";
                            if (data.name !== null) {

                                return '<button class="btn btn-default btn-sm template-delete" type="button">' +
                                            '<span class="glyphicon glyphicon-trash"></span>' +
                                          '</button>';
                            }
                            else {
                                return '<button class="btn btn-default btn-sm template-upload">' +
                                         '<span class="glyphicon glyphicon-plus"></span>' +
                                       '</button>';
                            }

                        }
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
        }
        ],
        select: false,
        colReorder: {
            fixedColumnsRight: 1
        },
        responsive: true,
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
        drawCallback: function (settings, json) {
            $('div#ReportTemplateTable_processing').addClass("wg-loader");

            var context = $('div#ReportTemplateTable_filter');
            $('*',context).addClass('small');

            context = $('div#ReportTemplateTable_length');
            $('label', context).addClass('small');
            
            $('select', context).addClass('small');
            //$('select', context).addClass('small btn btn-default btn-sm');
           // $('select', context).removeClass('form-control');

            context = $('div#ReportTemplateTable_info');
            context.addClass('small');
            $('*',context).addClass('small');

            context = $('div#ReportTemplateTable_paginate');
            $('ul > li > a', context).addClass('small').attr("style","padding: 5px 10px;");
            //$('ul > li > a', context).addClass('btn btn-sm small').attr("style", "border-radius: 0;");
        }
    });

});
///////////////////////////////////////////////////////////////////////////////
// Action buttons:
//  + Upload new template
//    description:
//      1. user clicked button
//      2. system takes record key that was set during table initialization
//      3. system and put record key into name and id of file input
//      4. system pass click on file input
//      5. user selects file or aborts action
//      6. system validates selected file 
//      7. system calls ajax to upload file
//      8. once uploaded system reloads table content
//  + Delete existing template
// all files have unique "template names". 
//
$('body').on('change', '.template-input', onTemplateInputChange);
$('body').on('click', '.template-upload', onTemplateAddClick);
$('body').on('click', '.template-delete', onTemplateDeleteClick);

//---------------------------------------------------------------------------//
function onTemplateAddClick(event) {
    //1. set template id
    var table = $('#ReportTemplateTable').DataTable();
    var data = table.row($(this).parents('tr')).data();
    var input = $("form#TemplateUpload > input[type='file']");

    input.attr('id', data.key);
    input.attr('name', data.key);

    //2. click on input
    input.click();
}
//---------------------------------------------------------------------------//
function onTemplateInputChange() {
    var allowedTypes = ["application/msword",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.template"];
    var input = $(this);
    var file = input[0].files[0];

    if (file !== undefined && !allowedTypes.includes(file.type)) {
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
       
    //each row is named with template section name
    var sectionName = input.attr('name');

    //upload: "Templates / Surveysummary / Title / filename
    UploadTemplate(sectionName, file);
}

//---------------------------------------------------------------------------//
function UploadTemplate(sectionName, docfile) {

    var form = new FormData();
    var templateName = "InspectionSummary";

    form.append("templateName", templateName);
    form.append(sectionName, docfile);
    
    var call = $.ajax({
        url: "Create",
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {            
            $('#ReportTemplateTable_processing').show();
        }
    });

    call.done(function (data, success) {
        //notification popup
        WgTools.alert("Pomyślnie wczytano plik szablonu", true, "S");
        //refresh table content
        var table = $('#ReportTemplateTable').DataTable();
        table.ajax.reload(null, false);
    });
    call.error(function (xhr, status, error) {
        //Notification popup
        WgTools.alert(error, true, 'E');
    });

}

//function onUploadTemplateCompleted(data, status, sectionName, file) {
//    var targetUrl = data;
//    var fileName = targetUrl.split('/').reverse()[0];
//
//    var row = $('tr[name="' + sectionName + '"]').first();
//    var columnLink = $('td[data-column="TemplateLink"]', row).first();
//    var columnManage = $('td[data-column="TemplateManage"]', row).first();
//
//    // create document link
//    var link = document.createElement('a');
//    link.setAttribute('href', targetUrl);
//    link.setAttribute('class','wg-link-nounderline template-link');
//    
//    var linkIcon = document.createElement("span");
//    linkIcon.setAttribute('class', 'glyphicon glyphicon-link');
//    var linkText = document.createTextNode(fileName);
//    
//    link.appendChild(linkIcon);
//    link.appendChild(linkText);
//        
//    // replace place holder with link
//    columnLink.html('');
//    columnLink.append(link);
//    
//
//    // create button to delete
//    var delButton = document.createElement('button');
//    delButton.setAttribute('class', 'btn btn-default btn-sm template-delete');
//    delButton.setAttribute('type', 'button');
//    var delButtonIcon = document.createElement('span');
//    delButtonIcon.setAttribute('class', 'glyphicon glyphicon-trash')
//
//    delButton.appendChild(delButtonIcon);
//
//    // replace upload button with delete button
//    columnManage.children().first().html('');
//    columnManage.children().first().append(delButton);
//
//    //notification popup
//    WgTools.alert("Pomyślnie wczytano plik szablonu", true, "S");
//}
//
//function onUploadTemplateFailed(xhr, status, error) {
//    WgTools.alert(error, true, 'E');
//}
///////////////////////////////////////////////////////////////////////////////
//---------------------------------------------------------------------------//
function onTemplateDeleteClick() {
    //1. set template id
    var table = $('#ReportTemplateTable').DataTable();
    var data = table.row($(this).parents('tr')).data();

    //2. confirm decision
    var modal = $('#TemplateDeleteModal').modal("show");

    //3. if confirmed
    $('body').one('click', '.template-delete-confirmed', data.key, DeleteTemplate);
}

function DeleteTemplate(event) {

    var templateName = "InspectionSummary";
    var templateSection = event.data;

    var form = new FormData();

    form.append("templateName", templateName);
    form.append("templateSection", templateSection);

    var call = $.ajax({
        url: "Delete",
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        data: form
    });

    call.done(function (data, success) {
        //notification popup
        WgTools.alert("Usunięto szablon", true, "S");

        //refresh table content
        var table = $('#ReportTemplateTable').DataTable();
        table.ajax.reload(null, false);

    });
    call.error(function (xhr, status, error) {
        //Notification popup
        WgTools.alert(error, true, 'E');
    });
}

//function onDeleteTemplateCompleted(data, status, name) {
//    var a = $('a[name="' + name + '"]');
//    // image link
//    a.removeAttr('href');
//    a.removeAttr('target');
//    // image
//    a.children('img').first().attr('src', '');
//    a.children('img').first().attr('alt', 'Dodaj zdjęcie');
//    // image delete button
//    a.siblings('button.photo-delete').first().hide();
//    a.siblings('form').first().children('input[type="file"]').first().val('');
//
//    //<button class="btn btn-default btn-sm template-upload">
//    //    <span class="glyphicon glyphicon-plus"></span>
//    //</button>
//    //<form action="" method="post" enctype="multipart/form-data" hidden>
//    //    <input type="file" id="@item.Key.ToString()" name="@item.Key.ToString()" class="template-input" accept="application/msword">
//    //    <input type="submit" value="Upload">
//    //</form>
//
//
//}
//
//function onDeleteTemplateFailed(xhr, status, error) {
//    WgTools.alert(error, true, 'E');
//}
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
//function turnLoader(turnOn) {
//    //var a = $('a[name="' + imageName + '"]');
//    // image
//    //var img = a.children('img').first();
//
//    var ajaxLoaderDiv = $('#AjaxLoader');
//
//    if (turnOn) {
//        ajaxLoaderDiv.addClass('wg-loader');
//        //img.removeClass('wg-image-placeholder');
//        //img.addClass('wg-loader');
//    }
//    else {
//        ajaxLoaderDiv.removeClass('wg-loader');
//        //img.removeClass('wg-loader');
//        //img.addClass('wg-image-placeholder');
//    }
//
//}
