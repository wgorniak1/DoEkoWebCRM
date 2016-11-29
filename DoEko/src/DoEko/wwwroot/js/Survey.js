//<script type="text/javascript" title="PlannedInstallation">
function plndInstallChanged(event) {

    var value = $(this).val();
    if (value == 1) {
        $(".plannedinstallationpurpose").hide();
    }
    else {
        $(".plannedinstallationpurpose").show();
    }

};

$('body').on('change', '#Localization', plndInstallChanged);



//<script type="text/javascript" title="SectionsAjaxPost">
function SectionPostFailure(xhr, status, error) {
    //set error message
    $("#AjaxErrorMsg").html(error);
    //display message box
    //$("#AjaxErrorDiv").alert();
    $("#AjaxErrorDiv").fadeToggle(function () {
        $("#AjaxErrorDiv").fadeToggle(3000);
    });
    alert("error");
};

function SectionPostSuccess() {

};

//<script type="text/javascript" title="on Div Dynamic Ajax refresh">
function onAjaxGetSuccess(data, status) {

    $('#Dynamic').html(data);

    var $form = $("#Dynamic form");
    $form.removeData("validator");
    $form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($form);

    var section = $("#Dynamic form").attr('id');

    if (section === 'InvestmentGeneralInfo') {
        $("#stepPrev").attr('disabled', 'disabled');
    }
    else {
        $("#stepPrev").removeAttr('disabled');
    }

    if (section === 'SurveyPhoto') {
        $("#stepNext").attr('disabled', 'disabled');
        $("#stepLast").removeClass('hidden');
    }
    else {
        $("#stepNext").removeAttr('disabled');
        $("#stepLast").addClass('hidden');
    }

}

function onAjaxGetFailure(xhr, status, error) {
    alert(error);
}

//<script type="text/javascript" title="OwnerAddRemove">
function addOwner() {
    // post current div
    var form = $("#Dynamic form");
    form.validate();

    if (form.valid()) {

        var url = form.attr("action"),
                  method = form.attr('data-ajax-method'),
                  ajaxTrue = form.attr('data-ajax');

        //1st
        var postCurrent =
        $.ajax({
            type: method || "POST",
            url: url,
            data: form.serialize()
        });

        //chain call     
        $.when(postCurrent).done(function () {
            // Get owner section again without id
            var getNew =
            $.ajax({
                url: "/InvestmentOwners/CreatePersonAjax",
                data: { investmentId: $("#InvestmentId").val() },
                type: "GET",
                dataType: "html"
            });

            getNew.done(function (data, success) {
                onAjaxGetSuccess(data, success)
            });

            getNew.error(function (xhr, status, error) {
                onAjaxGetFailure(xhr, status, error)
            });
        });
    }
}
function removeOwner() {


}

$('body').on('click', '.addowner', addOwner);
$('body').on('click', '.remowner', removeOwner);


$(".remownerconfirmed").click(function () {
    // current div no post
    // ajax to delete current owner
    // ajax to re read next / last owner
});

//<script type="text/javascript" title="StepFormNavigation">
function NavigationPrev() {
    var form = $("#Dynamic form");

    form.validate();

    if (form.valid()) {

        var url = form.attr("action"),
            method = form.attr('data-ajax-method'),
            ajaxTrue = form.attr('data-ajax');

        //1st
        var postCurrent = 
        $.ajax({
            type: method || "POST",
            url: url,
            data: form.serialize()
        });

        //chain call                        
        $.when(postCurrent).done(function () {
            var getPrev = 
            $.ajax({
                type: "GET",
                url: "/Surveys/MaintainPreviousStepAjax",
                data: {
                    currentStep: $('#Dynamic form').attr('id'),
                    surveyId: $("#MaintainedSurveyId").val()
                },
                dataType: "html"
            });

            getPrev.done(function (data, success) {
                onAjaxGetSuccess(data, success)
            });

            getPrev.error(function (xhr, status, error) {
                onAjaxGetFailure(xhr, status, error)
            });

        });
    };
};

function NavigationNext() {
    var form = $("#Dynamic form");

    form.validate();

    if (form.valid()) {

        //$form.submit();

        var url = form.attr("action"),
                   method = form.attr('data-ajax-method'),
                   ajaxTrue = form.attr('data-ajax');

        //1st
        var postCurrent =
        $.ajax({
            type: method || "POST",
            url: url,
            data: form.serialize()
        });

        //chain call     
        $.when(postCurrent).done(function () {
            var getNext =
            $.ajax({
                type: "GET",
                url: "/Surveys/MaintainNextStepAjax",
                data: {
                    currentStep: $('#Dynamic form').attr('id'),
                    surveyId: $("#MaintainedSurveyId").val()
                },
                dataType: "html"
            });
            
            getNext.done(function (data, success) {
                onAjaxGetSuccess(data, success)
            });

            getNext.error(function (xhr, status, error) {
                onAjaxGetFailure(xhr, status, error)
            });

        });


    };
};

function NavigationLast() {
    var $form = $("#Dynamic form");
    $form.validate();
    if ($form.valid()) {
        $form.submit();
    };

};

$('body').on('click', '#stepNext', NavigationNext);
$('body').on('click', '#stepPrev', NavigationPrev);
$('body').on('click', '#stepLast', NavigationLast);

//<script type="text/javascript" title="Checkboxes driven by option buttons">
function optionBtnTrue() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    $('input[name="' + checkboxName + '"]').val('True');

    var elementToShowId = $(this).attr('data-link-show-element');
    var elementToHideId = $(this).attr('data-link-hide-element');
                
    if (elementToShowId != "") $(elementToShowId).collapse('show');
    if (elementToHideId != "") $(elementToHideId).collapse('hide');
};

function optionBtnFalse() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    $('input[name="' + checkboxName + '"]').val('False');

    var elementToShowId = $(this).attr('data-link-show-element');
    var elementToHideId = $(this).attr('data-link-hide-element');

    if (elementToShowId != "") $(elementToShowId).collapse('show');
    if (elementToHideId != "") $(elementToHideId).collapse('hide');
};
$('body').on('change', '.optionbutton-true', optionBtnTrue);
$('body').on('change', '.optionbutton-false', optionBtnFalse);

///////////////////////////////////////////////////////////////////////////////
//<script type="text/javascript" title="Zdjecia">
$(document).on('change', ':file', function () {
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, ''),
        name = input.attr('name');
    input.trigger('fileselect', [numFiles, label, name]);
});

$(document).on('fileselect', ':file',function (event, numFiles, label, name) {
    $('#' + name).val(label);
});
///////////////////////////////////////////////////////////////////////////////
//<script>
//$(function () {
//    $("#stepLast").click(function () {
//        $("#mainForm").valid();
//        $("#mainForm").submit();

//    })
//    $("#stepNext").click(function () {
//        $("#mainForm").valid();
//        var StepList = new NavBarStepList("stepList", "stepPrev", "stepNext");
//        StepList.goNext();
//    });

//    $("#stepPrev").click(function () {
//        $("#mainForm").valid();

//        var StepList = new NavBarStepList("stepList", "stepPrev", "stepNext");
//        StepList.goPrev();
//    });

//    $("#stepList li a").click(function () {
//        var targetStep = parseInt($(this).parent("li").attr('data-step-from'), 10);

//        var stepList = new NavBarStepList("stepList", "stepPrev", "stepNext");

//        stepList.navBar.setCurrentStep(targetStep);
//        if (stepList.navBar.isFirstStep(targetStep)) {
//            stepList.navButtons.previous.disable();
//        }
//        else {
//            stepList.navButtons.previous.enable();
//        }


//        if (stepList.navBar.isLastStep(targetStep)) {
//            stepList.navButtons.next.disable();
//            $("#stepLast").removeClass("hidden");
//        }
//        else {
//            stepList.navButtons.next.enable();
//            $("#stepLast").addClass("hidden");
//        }

//    });
//});
