// Assign investment to inspector
function onAssignButtonClicked() {
    var investmentId = $(this).attr('data-investmend-id');
    ajaxAssignInvestment(investmentId);
}

$('body').on('click', '.investment-assign', onAssignButtonClicked);

//AJAX 

function onAjaxInvestmentAssignCompleted(investmentId, status) {
    var row = $('tr[data-investment-id="' + investmentId.toString() + '"]');
    //hide button
    $('td > div > button#Assign', row).hide();
    //show disabled button
    $('td > div > a#Assigned', row).show();
}

function onAjaxInvestmentAssignFailed(xhr, status, error) {
    alert("Wystąpił problem z przypisaniem inspektora. Proszę odświeżyć stronę i powtórzyć działanie jeśli to nie pomoże, proszę skontaktowac się z administratorem.");
}

function ajaxAssignInvestment(investmentId, InspectorId) {
    var ajaxCall = $.ajax({
        type: "POST",
        url: "/Investments/AssignInspector",
        data: {
            "InspectorId": null,
            "InvestmentId": investmentId
        },
        dataType: 'html'
    });
    ajaxCall.done(function (data, success) {
        onAjaxInvestmentAssignCompleted(investmentId, success);
    });
    ajaxCall.error(function (xhr, status, error) {
        onAjaxInvestmentAssignFailed(xhr, status, error);
    });
}