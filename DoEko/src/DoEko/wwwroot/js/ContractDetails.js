/////////////////////////////////////////////////////////////
// Contract Details
/////////////////////////////////////////////////////////////
//---------------------------------------------------------//
$(document).ready(function () {
    var neoenergetykaFile = $('input#Neoenergetyka', $('form#NeoenergetykaForm'));

    neoenergetykaFile.on('change', onNeoenergetykaFileChange);

    $('button.wg-btn-handle-neoenergetyka').on('click',
        function () {
            neoenergetykaFile.focus().trigger('click');
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
            //$('#ReportTemplateTable_processing').show();
        }
    });

    call.done(function (data, success) {
        //notification popup
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
        //refresh table content
        //var table = $('#ReportTemplateTable').DataTable();
        //table.ajax.reload(null, false);
    });
    call.error(function (xhr, status, error) {
        //Notification popup
        WgTools.alert(error, true, 'E');
    });

}
//---------------------------------------------------------//
function onNeoenergetykaPostSuccess() {
    WgTools.alert('Pomyślnie zapisano dane sekcji.', true, 'S');
}
//---------------------------------------------------------//
function onNeoenergetykaPostFailure(xhr, error, status) {
    WgTools.alert(error, true, 'E');
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
//---------------------------------------------------------//
/////////////////////////////////////////////////////////////