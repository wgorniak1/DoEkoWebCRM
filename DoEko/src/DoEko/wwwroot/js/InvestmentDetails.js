'use strict';
/////////////////////////////////////////////////////////////
class WgRSEEnum {

    constructor() {
        this.surveyType = ["CO", "CWU", "EE"];
        this.rseType = [["", "Pompa Ciepła Grunt.", "Kocioł na Pellet", "Pompa Ciepła Powietrzna"], ["", "", "", "Solary", "Pompa Ciepła"], ["", "", "", "", "", "Panele Fotowolt."]];
    }

    getText(type, rseType, single) {
        if (rseType === undefined)
            return this.surveyType[type];
        else if (single)
            return this.rseType[type][rseType]
        else
            return this.rseType[type][rseType] + ' (' + this.surveyType[type] + ')';
    }


    getId(typeText, rseTypeText) {
        var type = -1;
        var rseType = -1;

        for (var i = 0; i < this.surveyType.length; i++) {
            if (this.surveyType[i] === typeText) type = i;
        }

        if (rseTypeText === undefined) {
            return type;
        }

        if (type >= 0) {
            for (var j = 0; j < this.rseType[type].length; j++) {
                if (this.surveyType[type][i] === typeText) rseType = i;
            }
        }

        return rseType;

    }
}
class WgLocEnum {
    constructor() {
        this._array = ["Dach", "Grunt", "Elewacja"]
    }
    getText(value) {
        return this._array[value];
    }
    getValue(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }
}
class WgBdgEnum {
    constructor() {
        this._array = ["Gospodarczy", "Mieszkalny"]
    }
    getText(value) {
        return this._array[value];
    }
    getValue(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }
}
class WgEnuSrvCancelStatus {
    constructor() {
        this._array = ["Przed inspekcją", "Po inspekcji", "Instalacja niemożliwa", "Brak kontaktu z właścicielem", "Błąd - niezamawiane źródło"];
}
    getText(value) {
        return this._array[value];
    }
    getValue(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }
}
class WgEnuSrvStatus {
    constructor() {
        this._array = ["Nowa", "W trakcie realizacji", "W trakcie weryfikacji", "Do poprawy", "Zatwierdzona", "Anulowana"];
    }
    getText(value) {
        return this._array[value];
    }
    getValue(text) {
        for (var i = 0; i < this._array.length; i++) {
            if (this._array[i] === text)
                return i;
        }
    }
}

var WgEnums = {
    rse: new WgRSEEnum(), localization: new WgLocEnum(), building: new WgBdgEnum(),
    status: new WgEnuSrvStatus(), cancel: new WgEnuSrvCancelStatus()
};

$(document).ready(function () {
    var InvestmentId = $("#SurveyListTable").data('investmentid');
    var isAdmin = $("#SurveyListTable").data('admin');
    InitializeSurveyListTab(InvestmentId);
});

function InitializeSurveyListTab(investmentId) {
    var ajaxUrl = "/api/v1/Survey?investmentId=" + investmentId;

    var table = $('#SurveyListTable').DataTable({
        ajax: {
            url: ajaxUrl,
            dataSrc: "",
            rowId: "surveyId"
        },
        columns: [
                    {
                        data: null,
                        name: 'rseType',
                        title: 'Typ OZE',
                        //type: 'html',
                        render: function (data, type, row, meta) {
                            return WgEnums.rse.getText(data.type, data.rseType);
                        }
                    },
                    {
                        data: "status",
                        name: "status",
                        title: "Status inspekcji",
                        type: "html",
                        render: function (data, type, row, meta) {
                            return '<span title="' + ((data === 5) ? WgEnums.cancel.getText(row.cancelType) : '' ) + '">' + WgEnums.status.getText(data) + '</span>';
                        }
                    },
                    {
                        data: "changedAt",
                        name: "changedAt",
                        title: "Ost.Zmiana",
                        type: "datetime",
                        render: function (data, type, row, meta) {
                            var date = new Date(data);
                            switch (type) {
                                case 'display':
                                    return date.toLocaleString();
                                default:
                                    return date;
                            }
                        }

                    },
                    {
                        data: "resultCalculation.completed",
                        name: "completed",
                        title: "Zakończono obliczenia",
                        render: function (data, type, row, meta) {
                            return (data === true) ? "Tak" : "Nie";
                            }
                    },
                    {
                        data: "resultCalculation.rseNetPrice",
                        name: "netPrice",
                        title: "Cena Netto",
                        type: "num-fmt",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    return parseFloat(data).toFixed(2) + ' zł';
                                default:
                                    return parseFloat(data).toFixed(2);
                            }
                        }
                    },
                    {
                        data: "resultCalculation.rseTax",
                        name: "taxPrice",
                        title: "VAT",
                        type: "num-fmt",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    return parseFloat(data).toFixed(2) + ' zł';
                                default:
                                    return parseFloat(data).toFixed(2);
                            }
                        }
                    },
                    {
                        data: "resultCalculation.rseGrossPrice",
                        name: "grossPrice",
                        title: "Cena Brutto",
                        type: "num-fmt",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    return parseFloat(data).toFixed(2) + ' zł';
                                default:
                                    return parseFloat(data).toFixed(2);
                            }
                        }
                    },
                    {
                        data: "resultCalculation.rseOwnerContrib",
                        name: "ownerContrib",
                        title: "Udział własny",
                        type: "num-fmt",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    return parseFloat(data).toFixed(2) + ' zł';
                                default:
                                    return parseFloat(data).toFixed(2);
                            }
                        }
                    },
                    {
                        data: "resultCalculation.finalRSEPower",
                        name: "FinalPower",
                        title: "Dobrana Moc",
                        type: "html-num-fmt",
                        render: function (data, type, row, meta) {
                            switch (type) {
                                case 'display':
                                    switch (row.type.toString() + row.rseType.toString()) {
                                        //case '01'://CO.PCG
                                        //case '02'://CO.K
                                        //case '03'://CO.PCP
                                        case '13'://HW.SOL
                                            return '<span title="' + row.finalSolConfig + '">' + parseFloat(data).toFixed(2) + ' KW ' + '</span>';
                                        //case '14'://HW.PCP
                                            return '<span title="' + row.finalPVConfig + '">' + parseFloat(data).toFixed(2) + ' KW ' + '</span>';
                                        case '25'://EE.PV
                                        default:
                                            return '<span>' + parseFloat(data).toFixed(2) + ' KW</span>';
                                    }
                                default:
                                    return data;
                            }
                        }
                    },
                    {
                        data: null,
                        className: 'all',
                        name: "actions",
                        title: "Akcje",
                        orderable: false,
                        searchable: false,
                        type: 'html',
                        autoWidth: false,
                        visible: true,
                        render: function (data, type, row, meta) {
                            var content = "";
                            var isAdmin = $("#SurveyListTable").data('admin');
                            var linkClass = "btn btn-default btn-sm";

                            if (row.investment.status === 2) //in review
                            {
                                linkClass = linkClass + " disabled";
                            }

                            content = '<div class="pull-right">';

                            if (isAdmin === true) {
                                content += '<button class="' + linkClass + '"';
                                content += ' data-toggle="modal"';
                                content += ' data-target="#SrvMoveModal"';
                                content += ' data-survey-id="' + row.surveyId + '"';
                                content += ' type="button">';
                                content += '<span class="glyphicon glyphicon-transfer"></span>';
                                content += '</button>';
                            }

                            switch (row.status) {
                                case 0://new
                                    content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                    content += ' class="btn btn-default btn-sm">';
                                    content += '<span class="glyphicon glyphicon-eye-open"></span>';
                                    content += '</a>';
                                    content += '<button class="' + linkClass + '"';
                                    content += ' data-toggle="modal"';
                                    content += ' data-target="#SrvCancelModal"';
                                    content += ' data-survey-id="' + row.surveyId + '"';
                                    content += ' type="button">';
                                    content += '<span class="glyphicon glyphicon-ban-circle"></span>';
                                    content += '</button>';

                                    break;
                                case 1://draft
                                    content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                    content += ' class="' + linkClass + '" title="Edytuj ankietę">';
                                    content += ' <span class="glyphicon glyphicon-eye-open"></span>';
                                    content += '</a>';
                                    content += '<button class="' + linkClass + '"';
                                    content += ' data-toggle="modal"';
                                    content += ' data-target="#SrvCancelModal"';
                                    content += ' data-survey-id="' + row.surveyId + '"';
                                    content += ' type="button"';
                                    content += ' title="Anuluj ankietę">';
                                    content += '<span class="glyphicon glyphicon-ban-circle"></span>';
                                    content += '</button>';
                                    content += '<button class="' + linkClass + '"';
                                    content += ' data-toggle="modal"';
                                    content += ' data-target="#SrvSubmitModal"';
                                    content += ' data-survey-id="' + row.surveyId + '"';
                                    content += ' type="button"';
                                    content += ' title="Zakończ wprowadzanie ';
                                    content += '(Wyślij do akceptacji)">';
                                    content += '<span class="glyphicon glyphicon-send"></span>';
                                    content += '</button>';

                                    break;
                                case 2://submitted
                                    if (isAdmin)
                                    {
                                        content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                        content += ' class="' + linkClass + '">';
                                        content += '<span class="glyphicon glyphicon-eye-open"></span>';
                                        content += '</a>';
                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvCancelModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Anuluj ankietę">';
                                        content += '<span class="glyphicon glyphicon-ban-circle"></span>';
                                        content += '</button>';

                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvApprovalModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Zatwierdź ankietę">';
                                        content += '<span class="glyphicon glyphicon-ok-circle"></span>';
                                        content += '</button>';
                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvRejectModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Odrzuć ankietę">';
                                        content += '<span class="glyphicon glyphicon-remove-circle"></span>';
                                        content += '</button>';
                                    }

                                    break;
                                case 3://rejected
                                    content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                    content += ' class="' + linkClass + '">';
                                    content += '<span class="glyphicon glyphicon-eye-open"></span>';
                                    content += '</a>';
                                    if (isAdmin)
                                    {
                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvCancelModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Anuluj Ankietę">';
                                        content += '<span class="glyphicon glyphicon-ban-circle"></span>';
                                        content += '</button>';
                                    }
                                    break;
                                case 4://approved
                                    if (isAdmin)
                                    {
                                        content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                        content += ' class="' + linkClass + '" title="Edytuj ankietę">';
                                        content += ' <span class="glyphicon glyphicon-eye-open"></span>';
                                        content += '</a>';
                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvCancelModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Anuluj Ankietę">';
                                        content += '<span class="glyphicon glyphicon-ban-circle"></span>';
                                        content += '</button>';
                                    }
                                    break;
                                case 5://cancelled
                                    if (isAdmin)
                                    {
                                        content += '<a href="/Surveys/Maintain/' + row.surveyId + '"';
                                        content += ' class="' + linkClass + '" title="Edytuj ankietę">';
                                        content += '<span class="glyphicon glyphicon-eye-open"></span>';
                                        content += '</a>';
                                        content += '<button class="' + linkClass + '"';
                                        content += ' data-toggle="modal"';
                                        content += ' data-target="#SrvRevertModal"';
                                        content += ' data-survey-id="' + row.surveyId + '"';
                                        content += ' type="button"';
                                        content += ' title="Przywróć ankietę">';
                                        content += '<span class="glyphicon glyphicon-repeat"></span>';
                                        content += '</button>';
                                    }
                                    break;
                                }

                            content += '</div>';

                            return content;
                        }
                    }
        ],

        stateSave: true,
        pagingType: "full",
        language: WgLanguage,
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Wszystkie"]],
        order: [[0, "asc"], [1, 'asc'], [2, 'asc'], [3, 'asc'], [4, 'asc']],
        processing: true,
        dom: "<'row wg-dt-padding'<'col-sm-6'B><'col-sm-6'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row wg-dt-padding'<'col-sm-4'l><'col-sm-4'i><'col-sm-4'p>>",
        buttons: [
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

        ],
        //select: 'single',
        //responsive: {
        //    details: {
        //    }
        //},
        fixedHeader: {
            headerOffset: $('#NavBarMain').outerHeight()
        },
    });
}
///////////////////////////////////////////////////////////////////////////////
// SURVEY Move MODAL CONTROLS
//---------------------------------------------------------------------------//
$('body').on('click', '.btn[data-target]', function (event) {
    if ($(this).hasClass('disabled'))
    {
        event.stopPropagation();
    }
});
//---------------------------------------------------------------------------//
// SurveyMove Modal -> Show modal
function onSurveyMoveModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    var surveyId = $(this).data('survey-id');
    var form = $("#SurveyMoveModalForm");
    var srv = $("#SurveyId", form);
    srv.val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvMoveModal"]', onSurveyMoveModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyMove Modal -> Close/Cancel modal
function onSurveyMoveModalCancelButtonClick() {
    var form = document.getElementById("SurveyMoveModalForm");
    form.reset();
}
$('body').on('click', 'button.srvmovemodal-btn-cancel', onSurveyMoveModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveyMoveModalSubmitButtonClick() {
    ajaxSurveyMovePost();
}
$('body').on('click', 'button.srvmovemodal-btn-submit', onSurveyMoveModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveyMoveCompleted(data, status) {
    WgTools.alert(data, true, 'S');
    setTimeout(window.location.reload(), 4000);
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyMoveFailed(xhr, status, error) {
    WgTools.alert(error, false, 'E');
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyMovePost() {
    var form = $("#SurveyMoveModalForm");
    var srv = $("#SurveyId", form);
    var inv = $("#InvestmentId", form);
    var call = $.ajax({
        type: "POST",
        url: "/Surveys/Move",
        data: form.serialize()
    });

    call.done(function (data, success) {
        onAjaxSurveyMoveCompleted(data, success);
    });
    call.fail(function (xhr, status, error) {
        onAjaxSurveyMoveFailed(xhr, status, error);
    });
}





///
function onInvestmentDetailsSave() {
    //use WebApi()
    var $form = $("#InvestmentDetailsForm");
    var invId = $("#InvestmentId", $form);

    $form.validate({
        lang: 'pl'
    });

    if (!$form.valid())
        return;
    var url = "/Investments/PutInvestment/" + invId.val(); 
    var formData = $form.serialize();

    var call = $.ajax({
        url: url,
        type: "POST",
        data: formData //{ "id": invId.val() },
    });

    call.fail(function () { WgTools.alert("Wystąpił problem podczas zapisu", false, 'E');  });
    call.done(function () { onInvestmentDetailsCancel(); });
    call.done(function () { WgTools.alert("Pomyślnie zapisano dane sekcji", true, 'S'); });

}
function onInvestmentDetailsCancel() {
    var form = $("#InvestmentDetailsForm");
    var invId = $("#InvestmentId", form);

    var call = $.ajax({
        type: "GET",
        url: "/Investments/DetailsAjax",
        data: { "id": invId.val() },
        dataType: "html"
    });

    call.done(function (data, success) {
        $('#details .panel-body').html(data);

        $('button.investment-details-edit').removeAttr('disabled');
        
    });
    call.fail(function (xhr, status, error) {
        
    });
}
function onInvestmentDetailsEdit(event) {
    if ($(this).hasClass('disabled')) return;

    var investmentId = $(this).attr('data-investmentid');

    var call = $.ajax({
        type: "GET",
        url: "/Investments/EditAjax",
        data: { "id": investmentId },
        dataType: "html"
    });

    call.done(function (data, success) {
        $('#details .panel-body').html(data);

        var $form = $("#InvestmentDetailsForm");

        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);
        $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

        $('button.investment-details-edit').attr('disabled', 'disabled');
    });

    call.fail(function (xhr, status, error) {
        //
    });
}
$('button.investment-details-edit').on('click', onInvestmentDetailsEdit);
$('body').on('click', 'form#InvestmentDetailsForm button.submit', onInvestmentDetailsSave);
$('body').on('click', 'form#InvestmentDetailsForm button.cancel', onInvestmentDetailsCancel);

///

///////////////////////////////////////////////////////////////////////////////
// SURVEY Revert MODAL CONTROLS

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Show modal
function onSurveyRevertModalShowButtonClick() {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('show');
    var surveyId = $(this).data('survey-id');
    var form = $("#SurveyRevertModalForm");
    var srv = $("#SurveyId", form);
    srv.val(surveyId);
}
$('body').on('click', 'button[data-target="#SrvRevertModal"]', onSurveyRevertModalShowButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Close/Cancel modal
function onSurveyRevertModalCancelButtonClick() {
    var form = document.getElementById("SurveyRevertModalForm");
    form.reset();
}
$('body').on('click', 'button.srvrevertmodal-btn-cancel', onSurveyRevertModalCancelButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal -> Submit
function onSurveyRevertModalSubmitButtonClick() {
    ajaxSurveyRevertPost();
}
$('body').on('click', 'button.srvrevertmodal-btn-submit', onSurveyRevertModalSubmitButtonClick);

//---------------------------------------------------------------------------//
// SurveyCreate Modal - after Ajax is completed
function onAjaxSurveyRevertCompleted(data, status) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();

    window.location.reload();
}
//---------------------------------------------------------------------------//
//
function onAjaxSurveyRevertFailed(xhr, status, error) {
    //var modal = $("#SurveyCreateModal");
    //modal.modal('hide');

    //var form = document.getElementById("SurveyCreateModalForm");
    //form.reset();
    //
    alert(error);
}
//---------------------------------------------------------------------------//
//
function ajaxSurveyRevertPost() {
    var form = $("#SurveyRevertModalForm");
    var srv = $("#SurveyId", form);

    var call = $.ajax({
        type: "POST",
        url: "/Surveys/RevertAjax",
        data: {
            "surveyId": srv.val()
        }
    });

    call.done(function (data, success) {
        onAjaxSurveyRevertCompleted(data, success);
    });
    call.fail(function (xhr, status, error) {
        onAjaxSurveyRevertFailed(xhr, status, error);
    });
}









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
    call.fail(function (xhr, status, error) {
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
    var form = $("#SurveyApprovalModalForm");
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
    call.fail(function (xhr, status, error) {
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
    call.fail(function (xhr, status, error) {
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
    call.fail(function (xhr, status, error) {
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
    call.fail(function (xhr, status, error) {
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
