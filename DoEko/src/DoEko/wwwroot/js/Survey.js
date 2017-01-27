//
//$(document).ready
//   (
//       function () {
//           $('input[type="number"]').each
//           (
//               function (index) {
//                   var value = $(this).val();
//                   $(this).val(value.replace(',','.'));
//               });
//       });
//$(document).change
//   (
//       function () {
//           $('input[type="number"]').each
//           (
//               function (index) {
//                   var value = $(this).val();
//                   $(this).val(value.replace(',', '.'));
//               });
//       });

/////////////////////////////////////////////////////////////
//on Building General info - Insulation Type change to other
/////////////////////////////////////////////////////////////
function onCHInsTypeChange() {
    var type = $(".insulationtype").val();
    if (type === '3') {
        $(".insulationtype-other").show();
    }
    else {
        $(".insulationtype-other").hide();
    }
}

$('body').on('change', '.insulationtype', onCHInsTypeChange);
/////////////////////////////////////////////////////////////
//on Central Heating source change
function onCHTypeChange() {
    var type = $(".centralheating-type").val();
    if (type === '1') {
        $(".centralheating-other").hide();
        $(".centralheating-fuel").hide();
    }
    else if (type === '9'){
        $(".centralheating-other").show();
        $(".centralheating-fuel").show();
    }
    else {
        $(".centralheating-other").hide();
        $(".centralheating-fuel").show();
    }
}
$('body').on('change', '.centralheating-type', onCHTypeChange);
/////////////////////////////////////////////////////////////

//Navigation steps
var stepNo;

function ajaxGetSectionNo() {
    var getSect = $.ajax({
        type: "GET",
        url: "/Surveys/MaintainStepAjax",
        data: {
            stepNo: stepNo,
            surveyId: $("#MaintainedSurveyId").val()
        },
        dataType: "html"
    });
    getSect.done(function (data, success) {
        onAjaxGetSuccess(data, success);
    });
    getSect.error(function (xhr, status, error) {
        onAjaxGetFailure(xhr, status, error);
    });
}
function onNavigationBarClick() {
    stepNo = $(this).attr('data-step-number');

    ajaxPostCurrentSection(onAjaxPostError, ajaxGetSectionNo);
}
$('body').on('click', '.navigation-bar-link', onNavigationBarClick);
//Wall area
function onWallSizeChange() {
    var form = $("#Dynamic form");
    var width = parseFloat($("#Width", form).val());
    var height = parseFloat($("#Height", form).val());

    var area = width * height;

    $("#UsableArea").val(area.toFixed(2));

}
$('body').on('change', '.wallarea', onWallSizeChange);
//Boiler room volume
function onBoilerRoomSizeChange() {
    var form = $("#Dynamic form");
    var width = parseFloat($("#Width", form).val());
    var height = parseFloat($("#Height", form).val());
    var length = parseFloat($("#Length", form).val());
    
    var volume =  width * height * length;

    $("#Volume").val(volume.toFixed(2));
}
$('body').on('change', '.boilerroomvolume', onBoilerRoomSizeChange);

//<script type="text/javascript" title="PlannedInstallation">
function plndInstallChanged(event) {

    var value = $(this).val();
    if (value === 1) {
        $(".plannedinstallationpurpose").hide();
    }
    else {
        $(".plannedinstallationpurpose").show();
    }

}

$('body').on('change', '#Localization', plndInstallChanged);

//<script type="text/javascript" title="SectionsAjaxPost">
function SectionPostFailure(xhr, status, error) {
    WgTools.alert(error, true, 'E');
}

function SectionPostSuccess() {
    WgTools.alert('Pomyślnie zapisano dane sekcji.', true, 'S');
}
//*******************************************************************
//Updates Section DIV with new form that is returned from server
//Function is called whenever AJAX GET NEXT / PREV section is called
function onAjaxGetSuccess(data, status) {

    $('#Dynamic').html(data);

    var $form = $("#Dynamic form");

    $form.removeData("validator");
    $form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($form);

    $form.data("validator").settings.ignore = ".data-val-ignore, :hidden, :disabled";

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
    WgTools.alert(error, true, 'E');
}
//Executes AJAX GET PREV section
function ajaxGetPrevSection() {
    var getPrev = $.ajax({
        type: "GET",
        url: "/Surveys/MaintainPreviousStepAjax",
        data: {
            currentStep: $('#Dynamic form').attr('id'),
            surveyId: $("#MaintainedSurveyId").val()
        },
        dataType: "html"
    });
    getPrev.done(function (data, success) {
        onAjaxGetSuccess(data, success);
    });
    getPrev.error(function (xhr, status, error) {
        onAjaxGetFailure(xhr, status, error);
    });
}
// Executes AJAX GET NEXT section
function ajaxGetNextSection() {
    var getNext = $.ajax({
        type: "GET",
        url: "/Surveys/MaintainNextStepAjax",
        data: {
            currentStep: $('#Dynamic form').attr('id'),
            surveyId: $("#MaintainedSurveyId").val()
        },
        dataType: "html"
    });
    getNext.done(function (data, success) {
        onAjaxGetSuccess(data, success);
    });
    getNext.error(function (xhr, status, error) {
        onAjaxGetFailure(xhr, status, error);
    });
}
//*******************************************************************
//Updates Errors on form when AJAX POST returns errors
function onAjaxPostError(xhr, status, error){
    //Prepare fields and error messages
    var keys = Object.keys(xhr.responseJSON);
    var values = keys.map(function (k) { return xhr.responseJSON[k]; });
    var i = 0;
    var context = $("form", $("#Dynamic"));

    //update form to display error messages
    keys.forEach(function (key) {
        var inputField = $('input[name="' + key + '"], select[name="' + key + '"]', context);

        inputField.removeClass('valid');
        inputField.addClass('input-validation-error');
        inputField.attr('aria-invalid', true);

        var msgContainer = $('span[data-valmsg-for="' + key + '"]', context);
                
        var element = document.createElement("span");
        element.setAttribute('id', key.replace('.', '_') + '-error');
        element.setAttribute('class', '');
        
        var errorText = document.createTextNode(values[i]);

        element.appendChild(errorText);

        msgContainer.addClass('field-validation-error');
        msgContainer.removeClass ('field-validation-valid');
        msgContainer.html(element);

        i++;
    });
}
//Execute AJAX POST CURRENT SECTION
function ajaxPostCurrentSection(onError, onSuccess) {

    var form = $("#Dynamic form");

    form.validate({
        lang: 'pl'
    });

    if (form.valid()) {

        var url = form.attr("action"),
            method = form.attr('data-ajax-method'),
            ajaxTrue = form.attr('data-ajax');

        //1st
        var postCurrent = $.ajax({
            type: method || "POST",
            url: url,
            data: form.serialize()
        });

        postCurrent.error(onError);
        postCurrent.success(onSuccess);
        postCurrent.success(function () { WgTools.alert("Pomyślnie zapisano dane sekcji", true, 'S'); });
        //$.when(postCurrent);
    }
}
//*******************************************************************

//*******************************************************************
//Button handlers: Investment Owner | Add New Owner
function btnAddOwner() {
    //1. Post current section and then if success, create new owner
    ajaxPostCurrentSection(onAjaxPostError, function () {

        //1. Message informing
        WgTools.alert('Pomyślnie zapisano dane sekcji', true, 'S');

        var getNew = $.ajax({
            url: "/InvestmentOwners/CreatePersonAjax",
            data: { investmentId: $("#InvestmentId").val() },
            type: "GET",
            dataType: "html"
        });
        getNew.done(function (data, success) {
            onAjaxGetSuccess(data, success);
        });

        getNew.error(function (xhr, status, error) {
            onAjaxGetFailure(xhr, status, error);
        });
    });
}

function removeOwner() {
    $("#DeleteOwnerModal").modal('hide');
    $("div.modal-backdrop").remove();
    
    // ajax to delete current owner
    // ajax to re read next / last owner
    //1. Post current div
    //var form = $("#Dynamic form");
    var investmentId = $("#InvestmentId").val(),
    ownerId = $("#Owner_BusinessPartnerId").val(),
    // id=@("InvestmentOwnerData_" + Model.OwnerNumber.ToString() + '_' + Model.OwnerTotal.ToString())
    formId = $("#Dynamic form").attr("id").split("_");
    var OwnerNo = formId[1];
    var OwnerTotal = formId[2];
    if (ownerId !== "00000000-0000-0000-0000-000000000000") {
        //1st
        var deleteCurrent = $.ajax({
            type: "POST",
            url: "/InvestmentOwners/DeletePersonAjax",
            data: { investmentId: investmentId, ownerId: ownerId } 
        });

        //chain call     
        deleteCurrent.done(ajaxGetPrevSection);
    }
    else {
        ajaxGetPrevSection();
    }
}

$('body').on('click', '.addowner', btnAddOwner);
$('body').on('click', '.deleteownersubmit', removeOwner);


////////////////////////////////////////////////////////////////////
// SECTION BUILDING - ROOF
////////////////////////////////////////////////////////////////////
// ON ADD NEW ROOF PLANE
function onAddRoof() {
    //1. Post current section and then if success, create new owner
    ajaxPostCurrentSection(onAjaxPostError, function () {
        //1. Message informing
        WgTools.alert('Pomyślnie zapisano dane sekcji', true, 'S');

        var getNew = $.ajax({
            url: "/Surveys/CreateRoofPLaneAjax",
            data: { surveyId: $("#SurveyId").val() },
            type: "GET",
            dataType: "html"
        });
        getNew.done(function (data, success) {
            onAjaxGetSuccess(data, success);
        });

        getNew.error(function (xhr, status, error) {
            onAjaxGetFailure(xhr, status, error);
        });
    });
}
////////////////////////////////////////////////////////////////////
// ON DELETE ROOF PLANE
function onRemoveRoof() {
    $("#DeleteRoofModal").modal('hide');
    $("div.modal-backdrop").remove();

    // ajax to delete current roof
    // ajax to re read next / last roof
    var surveyId = $("#SurveyId").val();
    var roofId = $("#Plane_RoofPlaneId").val();
    // id=@("SurveyRoofPlane_" + Model.RoofNumber.ToString() + '_' + Model.RoofTotal.ToString())
    var formId = $("#Dynamic form").attr("id").split("_");
    var RoofNo = formId[1];
    var RoofTotal = formId[2];

    if (roofId !== "00000000-0000-0000-0000-000000000000") {

        var deleteCurrent = $.ajax({
            type: "POST",
            url: "/Surveys/DeleteRoofPlaneAjax",
            data: { surveyId: surveyId, roofPlaneId: roofId }
        });

        //chain call     
        deleteCurrent.done(ajaxGetPrevSection);
    }
    else {
        ajaxGetPrevSection();
    }
}
////////////////////////////////////////////////////////////////////
$('body').on('click', '.addroof', onAddRoof);
$('body').on('click', '.deleteroofsubmit', onRemoveRoof);
////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////
function onRoofTypeChange() {
    var newType = $(this).val();
    var imgName = "roof_0p.jpg";
    switch (newType) {
        case "0":
            imgName = "roof_0p.jpg";
            break;
        case "1":
            imgName = "roof_1p.png";
            break;
        case "2":
            imgName = "roof_2p.png";
            break;
        case "3":
            imgName = "roof_4p.png";
            break;
        default:
            break;
    }
    var d = new Date();    
    $("img.roof-thumbnail").attr('src', '/images/' + imgName + "?" + d.getTime());

}
$('body').on('change', '.roof-type', onRoofTypeChange);
////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////
function NavigationPrev() {
    var section = $("#Dynamic form").attr('id');

    if (section === 'SurveyPhoto') {
        ajaxGetPrevSection();
    } else {
        ajaxPostCurrentSection(onAjaxPostError, ajaxGetPrevSection);
    }
}
function NavigationNext() {
    ajaxPostCurrentSection(onAjaxPostError, ajaxGetNextSection);
}
function NavigationLast() {
    var $form = $("#Dynamic form").first();
    $form.validate({
        lang: 'pl'
    });
    if ($form.valid()) {
        $form.submit();
    }
}

$('body').on('click', '#stepNext', NavigationNext);
$('body').on('click', '#stepPrev', NavigationPrev);
$('body').on('click', '#stepLast', NavigationLast);

//<script type="text/javascript" title="Checkboxes driven by option buttons">
function optionBtnTrue() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    $('input[name="' + checkboxName + '"]').val('True');

    var elementToShowId = $(this).attr('data-link-show-element');
    var elementToHideId = $(this).attr('data-link-hide-element');
                
    if (elementToShowId !== "") $(elementToShowId).collapse('show');
    if (elementToHideId !== "") $(elementToHideId).collapse('hide');
}

function optionBtnFalse() {
    var checkboxName = $(this).attr('data-linked-checkbox');
    $('input[name="' + checkboxName + '"]').val('False');

    var elementToShowId = $(this).attr('data-link-show-element');
    var elementToHideId = $(this).attr('data-link-hide-element');

    if (elementToShowId !== "") $(elementToShowId).collapse('show');
    if (elementToHideId !== "") $(elementToHideId).collapse('hide');
}

$('body').on('change', '.optionbutton-true', optionBtnTrue);
$('body').on('change', '.optionbutton-false', optionBtnFalse);

///////////////////////////////////////////////////////////////////////////////
//<script type="text/javascript" title="Zdjecia">
//$(document).on('change', ':file', function () {
//    var input = $(this),
//        numFiles = input.get(0).files ? input.get(0).files.length : 1,
//        label = input.val().replace(/\\/g, '/').replace(/.*\//, ''),
//        name = input.attr('name');
//    input.trigger('fileselect', [numFiles, label, name]);
//});

//$(document).on('fileselect', ':file',function (event, numFiles, label, name) {
//    $('#' + name).val(label);
//});
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
///////////////////////////////////////////////////////////////////////////////
// SECTION PHOTO
//
// all pictures are numbered from 0 to 9:
// Picture0 and Picture5 are stored on investment level, the rest is stored on survey level
//
$('body').on('change', '.photo-input', onPhotoInputChanged);
$('body').on('click', '.photo-link', onPhotoLinkClick);
$('body').on('click', '.photo-delete', onPhotoDeleteClick);

//---------------------------------------------------------------------------//
function onPhotoLinkClick(event) {
    var link = $(this);
    // <a><img/></a><form><input></form>
    if (!link.attr('href')) {
        //
        event.stopPropagation();
        link.siblings('form').children('input[type="file"]').first().click();
    }
}
//---------------------------------------------------------------------------//
function onPhotoInputChanged() {
    const investmentlevel = ( "Picture0", "Picture5" );
    var input = $(this);
    var type = "";
    var guid = "";
    if (input.val() !== undefined) {
        //1. get file name 
        if (input.attr('name') === "Picture0" || 
            input.attr('name') === "Picture5") {
            type = "Investment";
            guid = $("#Dynamic form input[name='InvestmentId']").val();
        }
        else {
            type = "Survey";
            guid = $("#Dynamic form input[name='SurveyId']").val();
        }

        //upload
        UploadPhoto(type, guid, input.attr('name'), input[0].files[0]);
    }
}
//---------------------------------------------------------------------------//
function onPhotoDeleteClick() {
    var imageUrl = $(this).siblings('a').first().children('img').first().attr('src');
    var urlParts = imageUrl.split('/');
    //
    urlParts.reverse();

    if (urlParts.length >= 4) {
        var filename = urlParts[0];
        var name = urlParts[1];
        var guid = urlParts[2];
        var type = urlParts[3];
        DeletePhoto(type, guid, name, filename);
    }
}
//---------------------------------------------------------------------------//
function UploadPhoto(type, guid, imageName, image) {

    var form = new FormData();

    form.append("type", type);
    form.append("guid", guid);
    form.append(imageName, image);
    
    var call = $.ajax({
        url: "/Files/UploadPhoto",
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        data: form,
        beforeSend: function () { turnPhotoLoader(imageName, true);}
    });


    call.done(function (data, success) {
        onUploadPhotoCompleted(data, success, imageName);
        turnPhotoLoader(imageName, false);
    });
    call.error(function (xhr, status, error) {
        onUploadPhotoFailed(xhr, status, error);
    });

}
function turnPhotoLoader(imageName, turnOn) {
    var a = $('a[name="' + imageName + '"]');
    // image
    var img = a.children('img').first();

    if (turnOn) {
        img.removeClass('wg-image-placeholder');
        img.addClass('wg-loader');
    }
    else {
        img.removeClass('wg-loader');
        img.addClass('wg-image-placeholder');
    }

}

function onUploadPhotoCompleted(data, status, imageName) {
    var targetUrl = data;
    var a = $('a[name="' + imageName + '"]');
    // image link
    a.attr('href', targetUrl);
    a.attr('target', '_blank');
    // image
    a.children('img').first().attr('src', targetUrl);
    // image delete button
    a.siblings('button.photo-delete').first().removeAttr('hidden');
    a.siblings('button.photo-delete').first().show();
}
function onUploadPhotoFailed(xhr, status, error) {
    WgTools.alert(error, true, 'E');
}

//---------------------------------------------------------------------------//
function DeletePhoto(type, guid, name, filename) {
    var form = new FormData();

    form.append("type", type);
    form.append("guid", guid);
    form.append("pictureId", name);
    form.append("fileName", filename);

    var call = $.ajax({
        url: "/Files/DeletePhoto",
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        data: form
    });

    call.done(function (data, success) {
        onDeletePhotoCompleted(data, success, name);
    });
    call.error(function (xhr, status, error) {
        onDeletePhotoFailed(xhr, status, error);
    });
}

function onDeletePhotoCompleted(data, status, name) {
    var a = $('a[name="' + name + '"]');
    // image link
    a.removeAttr('href');
    a.removeAttr('target');
    // image
    a.children('img').first().attr('src', '');
    a.children('img').first().attr('alt', 'Dodaj zdjęcie');
    // image delete button
    a.siblings('button.photo-delete').first().hide();
    a.siblings('form').first().children('input[type="file"]').first().val('');
}
function onDeletePhotoFailed(xhr, status, error) {
    WgTools.alert(error, true, 'E');
}

//---------------------------------------------------------------------------//

///////////////////////////////////////////////////////////////////////////////
