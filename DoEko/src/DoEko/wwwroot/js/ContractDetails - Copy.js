/////////////////////////////////////////////////////////////
// Contract Details
/////////////////////////////////////////////////////////////
//---------------------------------------------------------//
$(document).ready(function () {
    var neoenergetykaFile = $('input#Neoenergetyka', $('form#NeoenergetykaForm'));
    neoenergetykaFile.on('change', onNeoenergetykaFileChange);
    $('button.wg-btn-handle-neoenergetyka').on('click',
        function () {
            //reset existing value
            neoenergetykaFile.val('');
            neoenergetykaFile.focus().trigger('click');
        });
    //
    var PKOFile = $('input#FIFile', $('form#FIForm'));
    PKOFile.on('change', onPKOFileChange);
    $('button.wg-btn-handle-pko').on('click',
        function () {
            //reset existing value
            PKOFile.val('');
            PKOFile.focus().trigger('click');
        });
    //
    var INVFile = $('input#INFFile', $('form#INFForm'));
    INVFile.on('change', onInvFileChange);
    $('button.wg-btn-handle-pko').on('click',
        function () {
            //reset existing value
            INVFile.val('');
            INVFile.focus().trigger('click');
        });
});
//---------------------------------------------------------//
function onNeoenergetykaFileChange() {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");
        return;
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
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.fail(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');
        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });

}
//---------------------------------------------------------//
function onPKOFileChange() {
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
    var form = new FormData($('form#FIForm')[0]);

    var call = $.ajax({
        type: "POST",
        url: "/Payments/UploadPaymentsFile",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.fail(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');
        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });
    });
}
//---------------------------------------------------------//
function onInvFileChange() {
    const allowedTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-excel"];

    var file = $(this)[0].files[0];

    if (file === undefined) {
        WgTools.alert("Proszę wskazać plik do importu!", true, "E");
        return;
    }
    else if (!allowedTypes.includes(file.type)) {
        WgTools.alert("Niedozwolony format pliku!", true, "E");
        return;
    }
    //Post File:
    var form = new FormData($('form#INFForm')[0]);

    var call = $.ajax({
        type: "POST",
        url: "/Investments/UploadDataFromFile",
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () {
            $('#panel-group').addClass('wg-loader');
        }
    });

    call.done(function (data, success) {
        $('#panel-group').removeClass('wg-loader');
        WgTools.alert("Pomyślnie wczytano dane", true, "S");
    });
    call.fail(function (xhr, status, error) {
        $('#panel-group').removeClass('wg-loader');

        $.each(xhr.responseJSON, function (idx, data) {
            if (data.length > 0) {
                WgTools.alert(data, false, 'E');
            }
        });

    });
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
/////////////////////////////////////////////////////////////