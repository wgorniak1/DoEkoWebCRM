//
function getAntiforgeryToken() {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    return token;
}
//
function onContractDeleteSuccess() {
    $("#AjaxErrorMsg").html('<span>Umowa została usunięta</span>');
    //display message box
    //$("#AjaxErrorDiv").alert();
    $("#AjaxErrorDiv").show(300,function(){ $("#AjaxErrorDiv").hide(6000);});
}
//
function onContractDeleteFail(xhr, status, error) {
    //set error message
    $("#AjaxErrorMsg").html(error);
    //display message box
    //$("#AjaxErrorDiv").alert();
    $("#AjaxErrorDiv").removeClass("hidden");
    $("#AjaxErrorDiv").show();
}
//
function onContractDelete(event) {
    //mark entry for deletion -> needed to pass contract id via ajax
    $(this).addClass('contract-delete-marked');
}
//
function onContractDeleteCancel(event) {
    //clear deletion mark
    $(".contract-delete-marked").removeClass('contract-delete-marked');
}
//
function onContractDeleteSubmit() {
    var deleteRequest = $.ajax({
        type: "POST",
        url: "/Contracts/Delete",
        data: {
            __RequestVerificationToken: getAntiforgeryToken(),
            id: $(".contract-delete-marked").attr("data-contract-id")
        }
    });

    deleteRequest.success(function(data,success){
        onContractDeleteSuccess(data, success)
    });
    deleteRequest.fail(function (xhr, status, error) {
        onContractDeleteFail(xhr, status, error)
    });
}
//
$('body').on('click', '.contract-delete', onContractDelete);
$('body').on('click', '.contract-delete-cancel', onContractDeleteCancel);
$('body').on('click', '.contract-delete-submit', onContractDeleteSubmit);