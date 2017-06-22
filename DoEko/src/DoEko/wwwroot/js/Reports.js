
//
function onAjaxContractsGetFailed(xhr, status, error) {
    alert(error);
}
//
function onAjaxContractsGetCompleted(data, success) {
    var contracts = document.getElementById("ContractId");

    //clear current options
    while (contracts.options.length) {
        contracts.remove(0);
    }

    //add empty option
    var opt = new Option("", "");
    contracts.options.add(opt);

    //fill with new data
    $.each(data, function (key, item) {

        var opt = new Option(item.text, item.value);
        contracts.options.add(opt);
    });

    //
    updateExtractLink();
}
//
function updateExtractLink() {
    var paramProject = "projectId=" + $("#ProjectId").val().toString();
    var paramContract = "contractId=" + $("#ContractId").val().toString();

    var url = "/Reports/SurveyToCSV" + "?" + paramProject + "&" + paramContract;

    $(".data-link").attr("href", url);
}

////
function onProjectChange() {

    var newId = $(this).val();
    var currentId = $(this).attr("data-project");

    if (newId !== currentId) {
        //
        $(this).attr("data-project", newId);

        //
        var getContracts = $.ajax({
            type: "GET",
            url: "/Contracts/GetContractsAjax",
            data: {
                "projectId": newId
            },
            dataType: 'json',
            contentType: 'application/json'
        });

        getContracts.done(function (data, success) {
            onAjaxContractsGetCompleted(data, success);
        });
        getContracts.fail(function (xhr, status, error) {
            onAjaxContractsGetFailed(xhr, status, error);
        });
    }
}

//
function onContractChange() {
    updateExtractLink();
}

//
$('body').on('change', '.data-project', onProjectChange);
$('body').on('change', '.data-contract', onContractChange);
