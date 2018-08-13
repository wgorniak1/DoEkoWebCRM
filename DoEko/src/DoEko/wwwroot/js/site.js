$(function () {
    "use strict";
    $('.wg-panel-header-button').click(function (e) { event.stopPropagation(); });
});

//my tools           

var WgTools = new alertClass();

function alertClass() {
    //alert method
    var count = 0;
    this.createLink = function (url,fileName) {
        var link = document.createElement('a');
        link.href = url;
        if (link.download !== undefined) link.download = fileName;

        return link;
    }

    this.alert = function (msgText, fade, msgType) {
        var alertContainer = document.getElementById('dynamicAlertBox'),
            alertBox = document.createElement('div'),
            boxCloseBtn = document.createElement('button'),
            closeBtnIcon = document.createElement('span'),
            alertMessageContainer = document.createElement('div');

        count += 1;


        closeBtnIcon.setAttribute('aria-hidden', 'true');
        closeBtnIcon.innerHTML = "&times;";

        boxCloseBtn.setAttribute('type','button');
        boxCloseBtn.setAttribute('class','close');
        boxCloseBtn.setAttribute('data-dismiss','alert');
        boxCloseBtn.setAttribute('aria-label','Close');
        boxCloseBtn.appendChild(closeBtnIcon);
        //
        if (msgText.constructor === Array) {
            for (var i = 0; i < msgText.length; i++) {
                var pnode = document.createElement('span');
                pnode.appendChild(document.createTextNode(msgText[i]));
                alertMessageContainer.appendChild(pnode);
                alertMessageContainer.appendChild(document.createElement('br'));
            }
        } else {
            var pnode = document.createElement('span');
            pnode.appendChild(document.createTextNode(msgText));
            alertMessageContainer.appendChild(pnode);
            alertMessageContainer.appendChild(document.createElement('br'));
        }
            
        switch (msgType) {
            case 'E': alertBox.setAttribute('class', 'alert alert-danger alert-dismissable');
                        break;
            case 'W': alertBox.setAttribute('class', 'alert alert-warning alert-dismissable');
                        break;
            case 'S': alertBox.setAttribute('class', 'alert alert-success alert-dismissable');
                        break;
            default:  alertBox.setAttribute('class', 'alert alert-info alert-dismissable');
                        break;
        }

        var idAlertBox = 'AlertBoxNo' + count;

        alertBox.setAttribute('role','alert');
        alertBox.setAttribute('id',idAlertBox);           
        alertBox.appendChild(boxCloseBtn);
        alertBox.appendChild(alertMessageContainer);

        alertContainer.appendChild(alertBox);

        if (fade === true) {
            $('#' + idAlertBox).fadeOut(6000,'swing');
        }
    };
}
/////
//function NavBarStepList(stepListId, prevBtnId, nextBtnId) {
//    "use strict";

//    //Navigation bar
//    this.navBar = new function () {
//        this.$ref = $('#' + stepListId);
//        // set Current Step
//        this.setCurrentStep = function (stepNo) {
//            this.$ref.attr("data-step-current", stepNo);
//        };
//        // Navigation bar - Previous Step before the one currently selected
//        this.previousStep = new function () {
//            this.$ref = $('#' + stepListId).children("li.active").first().prev("li");

//            if (this.$ref.length > 0) {
//                this.from = parseInt(this.$ref.attr("data-step-from"), 10);
//                this.to = parseInt(this.$ref.attr("data-step-to"), 10);
//            }
//            else {
//                this.from = NaN;
//                this.to = NaN;
//            }
//        };
//        // Navigation bar - Previous Step before the one currently selected
//        this.nextStep = new function () {
//            this.$ref = $('#' + stepListId).children("li.active").first().next("li");

//            if (this.$ref.length > 0) {
//                this.from = parseInt(this.$ref.attr("data-step-from"), 10);
//                this.to = parseInt(this.$ref.attr("data-step-to"), 10);
//            }
//            else {
//                this.from = NaN;
//                this.to = NaN;
//            }
//        };
//        // Navigation bar - Currently selected Step          
//        this.currentStep = new function () {
//            this.$ref = $('#' + stepListId).children("li.active").first();
//            this.number = parseInt($('#' + stepListId).attr("data-step-current"), 10);
//            this.from = parseInt(this.$ref.attr("data-step-from"), 10);
//            this.to = parseInt(this.$ref.attr("data-step-to"), 10);
//        };
//        //
//        this.isLastStep = function (stepNo) {
//            var CheckNextStep = false;

//            this.$ref.children('li').each(function() {

//                if (CheckNextStep) {
//                    //StepNo is from previous step, if we're here, this means that there is another step
//                    CheckNextStep = false;
//                    return false;
//                }
//                var from = parseInt($(this).attr('data-step-from'));
//                var to = parseInt($(this).attr('data-step-to'));

//                if (from <= stepNo && stepNo < to) {
//                    //there is still one substep in the current step
//                    return false;
//                }
//                if (from <= stepNo && stepNo === to) {
//                    CheckNextStep = true;
//                }
//            });

//            return CheckNextStep;
//        };
//        //
//        this.isFirstStep = function (stepNo) {
//            var result = false;

//            this.$ref.children("li").each(function () {
//                var from = parseInt($(this).attr('data-step-from'));
//                var to = parseInt($(this).attr('data-step-to'));

//                if (from < stepNo && stepNo <= to) {
//                    return false;
//                }

//                if (from === stepNo && stepNo <= to) {
//                    if ($(this).prev("li").length === 0) {
//                        result = true;                                                        
//                    }
//                    return false;
//                }
//            });
//            return result;
//        };

//    };

//    //Navigation buttons
//    this.navButtons = new function () {
//        this.previous = new function () {
//            this.$ref = $('#' + prevBtnId);
//            this.disable = function () {
//                this.$ref.attr('disabled', 'disabled');
//            };
//            this.enable = function () {
//                this.$ref.removeAttr('disabled');
//            };
//        };
//        this.next = new function () {
//            this.$ref = $('#' + nextBtnId);
//            this.disable = function () {
//                this.$ref.attr('disabled', 'disabled');
//            };
//            this.enable = function () {
//                this.$ref.removeAttr('disabled');
//            };
//        };
//    };
    
//    //Tabbed Content divs
//    this.navContent = function (TargetStepNo) {
//        const tabDivId = "#Step_";
//        this.$currentTab = $(tabDivId + navBar.currentStep.number);
//        this.$otherTab = $(tabDivId + TargetStepNo);
//    };

//    this.tabDiv = function (id) {
//        const tabDivId = "#Step_";
//        var DivId = tabDivId + id;
//        return $(DivId);
//    };

//    this.goNext = function () {
//        var targetNumber = 0;
//        var updateNavBar = false;
//        var isLastStep = false;

//        if (this.navBar.currentStep.number < this.navBar.currentStep.to) {
//            targetNumber = this.navBar.currentStep.number + 1;
//        }
//        else {
//            updateNavBar = true;
//            targetNumber = this.navBar.nextStep.from;
//        }
//        //Update div with target content
//        var $currentTab = this.tabDiv(this.navBar.currentStep.number);
//        $currentTab.removeClass('in active');

//        var $targetTabDiv = this.tabDiv(targetNumber);
//        $targetTabDiv.addClass('in active');

//        //update selection marker on navigation bar
//        if (updateNavBar) {
//            this.navBar.currentStep.$ref.removeClass("active");
//            this.navBar.nextStep.$ref.addClass("active");
//        }

//        //manage navigation buttons
//        if (this.navBar.isLastStep(targetNumber)) {
//            this.navButtons.next.disable();
//            $("#stepLast").removeClass('hidden');
//        }
//        this.navButtons.previous.enable();

//        //update currentStep number
//        this.navBar.setCurrentStep(targetNumber);

//    };

//    this.goPrev = function () {
//        var targetNumber = 0;
//        var updateNavBar = false;
//        var isLastStep = false;

//        if (this.navBar.currentStep.number > this.navBar.currentStep.from)
//        {
//            targetNumber = this.navBar.currentStep.number - 1;
//        }
//        else
//        {
//            updateNavBar = true;
//            targetNumber = this.navBar.previousStep.to;
//        }

//        //Switch active div (tab content)
//        this.tabDiv(this.navBar.currentStep.number).removeClass('in active');
//        this.tabDiv(targetNumber).addClass('in active');

//        //update selection marker on navigation bar
//        if (updateNavBar) {
//            this.navBar.currentStep.$ref.removeClass("active");
//            this.navBar.previousStep.$ref.addClass("active");
//        }

//        //manage navigation buttons
//        if (this.navBar.isFirstStep(targetNumber)) {
//            this.navButtons.previous.disable();
//        }
//        this.navButtons.next.enable();
//        $("#stepLast").addClass('hidden');

//        //update currentStep number
//        this.navBar.setCurrentStep(targetNumber);

//    };

//}
$(document).ready(function () {

    $('input[data-val-length-max]').each(function (index) {
        $(this).attr('maxlength', $(this).attr('data-val-length-max'));
    });
});

    var WgLanguage = {
        //"processing": "<i class=\"fa fa-spinner fa-spin fa-3x fa-fw\"></i><span class=\"sr-only\">Loading...</span> ",
        "processing": "<span class='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span>",
        "search": "Znajdź:",
        "lengthMenu": "_MENU_",
        "info": "Wiersze od _START_ do _END_ z _TOTAL_ łącznie",
        "infoEmpty": "Pozycji 0 z 0 dostępnych",
        "infoFiltered": "(filtr. z _MAX_ wszystkich pozycji)",
        "infoPostFix": "",
        "loadingRecords": "",
        "zeroRecords": "Nie znaleziono pasujących pozycji",
        "emptyTable": "Brak danych",
        "paginate": {
            "first": "Pierwsza",
            "previous": "Poprz.",
            "next": "Nast.",
            "last": "Ostatnia"
        },
        "aria": {
            "sortAscending": ": aktywuj, by posortować kolumnę rosnąco",
            "sortDescending": ": aktywuj, by posortować kolumnę malejąco"
        }
    }