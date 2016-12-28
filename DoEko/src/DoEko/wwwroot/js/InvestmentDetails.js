///////////////////////////////////////////////////////////////////////////////
// SURVEY Reject MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Show modal
function onSurveyRejectModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    var surveyId = $(this).data('survey-id');
    var form = $("#SurveyRejectModalForm");
    var srv = $("#SurveyId", form);
    srv.val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvRejectModal"]', onSurveyRejectModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Close/Cancel modal
function onSurveyRejectModalCancelButtonClick() {
    var form = document.getElementById("SurveyRejectModalForm");
    form.reset();
}
$('body').on('click', 'button.srvrejectmodal-btn-cancel', onSurveyRejectModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveyRejectModalSubmitButtonClick() {
    ajaxSurveyRejectPost();
}
$('body').on('click', 'button.srvrejectmodal-btn-submit', onSurveyRejectModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveyRejectCompleted(data, status) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyRejectFailed(xhr, status, error) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();
    //
    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyRejectPost() {
    var form = $("#SurveyRejectModalForm");
    var srv = $("#SurveyId", form);

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/RejectAjax",
        data: {
            "surveyId": srv.val()
        }
    });

    call.done(function (data, success) {
        onAjaxSurveyRejectCompleted(data, success);
    });
    call.error(function (xhr, status, error) {
        onAjaxSurveyRejectFailed(xhr, status, error);
    });
}

///////////////////////////////////////////////////////////////////////////////
// SURVEY Approve MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Show modal
function onSurveyApprovalModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    var surveyId = $(this).data('survey-id');
    var form = $("#SurveyApprovalModalForm");
    var srv = $("#SurveyId", form);
    srv.val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvApprovalModal"]', onSurveyApprovalModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Close/Cancel modal
function onSurveyApprovalModalCancelButtonClick() {
    var form = document.getElementById("SurveyApprovalModalForm");
    form.reset();
}
$('body').on('click', 'button.srvapprovalmodal-btn-cancel', onSurveyApprovalModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveyApprovalModalSubmitButtonClick() {
    ajaxSurveyApprovePost();
}
$('body').on('click', 'button.srvapprovalmodal-btn-submit', onSurveyApprovalModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveyApproveCompleted(data, status) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyApproveFailed(xhr, status, error) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();
    //
    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyApprovePost() {
    var form = $("#SurveySubmitModalForm");
    var srv = $("#SurveyId", form);

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/ApproveAjax",
        data: {
            "surveyId": srv.val(),
            "submitInvestment": true
        }
    });

    call.done(function (data, success) {
        onAjaxSurveyApproveCompleted(data, success);
    });
    call.error(function (xhr, status, error) {
        onAjaxSurveyApproveFailed(xhr, status, error);
    });
}

///////////////////////////////////////////////////////////////////////////////
// SURVEY SUBMIT MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Show modal
function onSurveySubmitModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    var surveyId = $(this).data('survey-id');
    var form = $("#SurveySubmitModalForm");
    var srv = $("#SurveyId", form);
    srv.val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvSubmitModal"]', onSurveySubmitModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Close/Cancel modal
function onSurveySubmitModalCancelButtonClick() {
    var form = document.getElementById("SurveySubmitModalForm");
    form.reset();
}
$('body').on('click', 'button.srvsubmitmodal-btn-cancel', onSurveySubmitModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveySubmitModalSubmitButtonClick() {
  ajaxSurveySubmitPost();
}
$('body').on('click', 'button.srvsubmitmodal-btn-submit', onSurveySubmitModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveySubmitCompleted(data, status) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveySubmitFailed(xhr, status, error) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();
    //
    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveySubmitPost() {
    var form = $("#SurveySubmitModalForm");
    var srv = $("#SurveyId", form);

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/SubmitAjax",
        data: {
            "surveyId": srv.val(),
            "submitInvestment": true
        }
    });

    call.done(function (data, success) {
        onAjaxSurveySubmitCompleted(data, success);
    });
    call.error(function (xhr, status, error) {
        onAjaxSurveySubmitFailed(xhr, status, error);
    });
}


///////////////////////////////////////////////////////////////////////////////
// SURVEY CREATE MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Show modal
function onSurveyCreateModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    onSurveyCreateSurveyTypeChange();
}
$('body').on('click', 'button[data-target="#SrvCreateModal"]', onSurveyCreateModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Close/Cancel modal
function onSurveyCreateModalCancelButtonClick() {
    var form = document.getElementById("SurveyCreateModalForm");
    form.reset();
}
$('body').on('click', 'button.srvcreatemodal-btn-cancel', onSurveyCreateModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveyCreateModalSubmitButtonClick() {
    var $form = $("form#SurveyCreateModalForm");

    $form.validate();

    if ($form.valid()) {
        ajaxSurveyCreatePost();
    }
}
$('body').on('click', 'button.srvcreatemodal-btn-submit', onSurveyCreateModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - Survey Type has changed, update RSE Type
function onSurveyCreateSurveyTypeChange() {
    var form = $("#SurveyCreateModalForm");
    var srvType = $("#SurveyType",form).val();
    var rseSection = $("#SurveyCreateModalFormRSEType", form);
    var rseEN = $("#RSETypeEN", form);
    var rseHW = $("#RSETypeHW", form);
    var rseCH = $("#RSETypeCH", form);

    rseEN.hide();
    rseCH.hide();
    rseHW.hide();

    switch (srvType) {
        case "0": rseCH.show();
            rseSection.show();
            break;
        case "1": rseHW.show();
            rseSection.show();
            break;
        case "2": rseEN.show();
            rseSection.show();
            break;
        default: rseSection.hide();
    }
}
$('body').on('change', '#SurveyType', onSurveyCreateSurveyTypeChange);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveyCreateCompleted(data, status) {
    var modal = $("#SurveyCreateModal");
    modal.modal('hide');

    var form = document.getElementById("SurveyCreateModalForm");
    form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyCreateFailed(xhr, status, error) {
    var modal = $("#SurveyCreateModal");
    modal.modal('hide');

    var form = document.getElementById("SurveyCreateModalForm");
    form.reset();
    //
    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyCreatePost() {
    var form = $("#SurveyCreateModalForm");

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/CreateAjax",
        data: form.serialize()
        //dataType: 'html'
        //beforeSend: function () { ajaxLoader($('.data-table-container'), true) },
        //completed: function () { ajaxLoader($('.data-table-container'), false) }
        //contentType: 'application/json',
    });

    call.done(function (data, success) {
        onAjaxSurveyCreateCompleted(data, success);
    });
    call.error(function (xhr, status, error) {
        onAjaxSurveyCreateFailed(xhr, status, error);
    });
}


///////////////////////////////////////////////////////////////////////////////
// SURVEY CANCEL MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCancel Modal -> Show modal
function onSurveyCancelModalShowButtonClick() {
    var surveyId = $(this).attr("data-survey-id");
    var form = $("#SurveyCancelModalForm");
    //update SurveyId on the form
    $('#SurveyId', form).val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvCancelModal"]', onSurveyCancelModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCancel Modal -> Close/Cancel modal
function onSurveyCancelModalCancelButtonClick() {
    var form = document.getElementById("SurveyCancelModalForm");
    form.reset();
}
$('body').on('click', 'button.srvcancelmodal-btn-cancel', onSurveyCancelModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCancel Modal -> Submit
function onSurveyCancelModalSubmitButtonClick() {
    var $form = $("form#SurveyCancelModalForm");

    $form.validate();

    if ($form.valid()) {
        ajaxSurveyCancelPost();
    }
}
$('body').on('click', 'button.srvcancelmodal-btn-submit', onSurveyCancelModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCancel Modal -> Form modifications
function onSurveyCancelNewSurveyChange(event) {

    var $form = $("form#SurveyCancelModalForm");
    var srvType = $("#ReplaceWithType", $form);

    //event.data = true / false;
    if (event.data) {
        //enable validation for new Survey Type / RSE Type
        srvType.attr('data-val', 'true');
        srvType.attr('data-val-required', 'Pole jest obowiązkowe');
    }
    else {
        //disable validation
        srvType.removeAttr('data-val');
        srvType.removeAttr('data-val-required');
    }

    $form.removeData("validator");
    $form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($form);

    $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

}
$('body').on('change', '.optionbutton-true', true, onSurveyCancelNewSurveyChange);
$('body').on('change', '.optionbutton-false', false, onSurveyCancelNewSurveyChange);

//---------------------------------------------------------------------------//
function onSurveyCancelReplaceWithChange() {
    var form = $("#SurveyCancelModalForm");
    var srvType = $(this).val();
    var rseSection = $("#SurveyCancelModalFormRSEType", form);
    var rseEN = $("#ReplaceWithRSEEN", form);
    var rseHW = $("#ReplaceWithRSEHW", form);
    var rseCH = $("#ReplaceWithRSECH", form);

    rseEN.hide();
    rseCH.hide();
    rseHW.hide();

    switch (srvType) {
        case "0": rseCH.show();
            rseSection.show();
            break;
        case "1": rseHW.show();
            rseSection.show();
            break;
        case "2": rseEN.show();
            rseSection.show();
            break;
        default: rseSection.hide();
    }
}
$('body').on('change', '#ReplaceWithType', onSurveyCancelReplaceWithChange);
//---------------------------------------------------------------------------//
//DATA TABLE AJAX
function onAjaxSurveyCancelCompleted(data, status) {
    var modal = $("#SurveyCancelModal");
    modal.modal('hide');

    var form = document.getElementById("SurveyCancelModalForm");
    form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyCancelFailed(xhr, status, error) {
    var modal = $("#SurveyCancelModal");
    modal.modal('hide');

    var form = document.getElementById("SurveyCancelModalForm");
    form.reset();

    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyCancelPost() {
    var form = $("#SurveyCancelModalForm");

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/CancelAjax",
        data: form.serialize()
        //dataType: 'html'
        //beforeSend: function () { ajaxLoader($('.data-table-container'), true) },
        //completed: function () { ajaxLoader($('.data-table-container'), false) }
        //contentType: 'application/json',
    });

    call.done(function (data, success) {
        onAjaxSurveyCancelCompleted(data, success);
    });
    call.error(function (xhr, status, error) {
        onAjaxSurveyCancelFailed(xhr, status, error);
    });
}
///////////////////////////////////////////////////////////////////////////////
// ADDITIONAL COTNROLS
///////////////////////////////////////////////////////////////////////////////
// Yes/No buttons - set related checkbox or/and show/hide related element
function optionBtnTrue() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    if (checkboxName) {
        $('input[name="' + checkboxName + '"]').val('True');
    }

    var elementToShowId = $(this).attr('data-linked-element-show');
    var elementToHideId = $(this).attr('data-linked-element-hide');

    if (elementToShowId) { $(elementToShowId).collapse('show'); }
    if (elementToHideId) { $(elementToHideId).collapse('hide'); }
}

function optionBtnFalse() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    if (checkboxName) {
        $('input[name="' + checkboxName + '"]').val('False');
    }

    var elementToShowId = $(this).attr('data-linked-element-show');
    var elementToHideId = $(this).attr('data-linked-element-hide');

    if (elementToShowId) { $(elementToShowId).collapse('show'); }
    if (elementToHideId) { $(elementToHideId).collapse('hide'); }
}

$('body').on('change', '.optionbutton-true', optionBtnTrue);
$('body').on('change', '.optionbutton-false', optionBtnFalse);
///////////////////////////////////////////////////////////////////////////////
