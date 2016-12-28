//CLick on row to edit
function onInvestmentRowClicked() {
    var alink = $(this).children("td#InvestmentRowCol").children('div').children('a');

    window.location.href = alink.attr('href');
}

$('body').on('click', '.investmentrow', onInvestmentRowClicked);

//Pagination
function onDataTablePageChange() {
    var newPage = $(this).attr('data-table-paging-target');
    ajaxDataTableRefresh(newPage, 0, null);
}
$('body').on('click', '.data-table-paging-page', onDataTablePageChange);
//Pagination - size
function onDataTablePageSizeChange() {
    var newSize = $(this).val();
    var currentSize = $(this).attr("data-table-paging-size");
    if (newSize !== currentSize) {
        ajaxDataTableRefresh(0, newSize, null);
    }
}
$('body').on('change', '.data-table-paging-size', onDataTablePageSizeChange);
//Sorting
function onDatatableSortChange() {
    var newSortBy = $(this).attr('data-table-sortby');
    ajaxDataTableRefresh(0,0,newSortBy);
}
$('body').on('click', '.data-table-sort', onDatatableSortChange);

//Filtering
function onDataTableFilter() {
    ajaxDataTableRefresh(1, 0, null);
}

function onDataTableFilterReset() {
    //var form = $('data-table-filter-form');
    //form.trigger('reset');
    $("#CommuneId").val(''),
    $("#Status").val(''),
    //$("#ProjectId").val(),
    //$("#ContractId").val(),
    $("#City").val(''),
    $("#FreeText").val(''),

    ajaxDataTableRefresh(1, 0, null);
}
$('body').on('click', '.data-table-filter-submit', onDataTableFilter);
$('body').on('click', '.data-table-filter-reset', onDataTableFilterReset);
//DATA TABLE AJAX
function onAjaxDataTableRefreshCompleted(data, status) {
    $('.data-table-container').html(data);
}
function onAjaxDataTableRefreshFailed(xhr, status, error) {
    alert(error);
}
function ajaxDataTableRefresh(targetPage, targetPageSize, targetSort) {
    var paging = {};
    if (targetPage !== 0) {
        paging.CurrentNumber = targetPage;
    }
    else {
        paging.CurrentNumber = $("nav > ul.pagination > li.active > a").attr("data-table-paging-target");
    }

    if (targetPageSize !== 0) {
        paging.PageSize = targetPageSize;
    }
    else {
        paging.PageSize = $("#PageSize").val();
    }

    var sorting = {};
    if (targetSort !== null) {
        sorting.sortBy = targetSort;
    }
    else {
        sorting.sortBy = $(".data-table-container > table > thead").attr("data-table-sortby-current");
    }
    
    var concatenated = $("#CommuneId").val();

    var getDataTable = $.ajax({
        type: "GET",
        url: "/Investments/ListAjax",
        data: {
            "page": paging.CurrentNumber,
            "pageSize": paging.PageSize,
            "userId": $("#UserId").val(),
            "communeId": $("#CommuneId").val(),
            "status": $("#Status").val(),
            "projectId": $("#ProjectId").val(),
            "contractId": $("#ContractId").val(),
            "city": $("#City").val(),
            "freeSearch": $("#FreeText").val(),
            "filterByInspector": $("#FilterByInspector").val(),
            "sortBy": sorting.sortBy
        },
        dataType: 'html'
        //beforeSend: function () { ajaxLoader($('.data-table-container'), true) },
        //completed: function () { ajaxLoader($('.data-table-container'), false) }
        //contentType: 'application/json',
    });
    getDataTable.done(function (data, success) {
        onAjaxDataTableRefreshCompleted(data, success);
    });
    getDataTable.error(function (xhr, status, error) {
        onAjaxDataTableRefreshFailed(xhr, status, error);
    });
}

//function ajaxLoader(ref, show) {
  
//}