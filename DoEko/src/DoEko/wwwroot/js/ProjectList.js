'use strict';

$(document).ready(function () {
    var app = new application();

    app.main();

});

class GetAllInput {
    constructor(skipCount, maxResultCount, order, columns) {
        this.skipCount = skipCount || 0;
        this.totalCount = maxResultCount || 2147483647;

        let sorting = $.map(order, (v, i) => { return columns[v.column].name + ' ' + v.dir.toUpperCase(); });

        this.sorting = sorting.join() || '';
    }
}


class TextIdDictionary extends Map {
    setAll(dataObjectArray) {

        super.clear();
        for (var i = 0; i < dataObjectArray.length; i++) {
            this.set(dataObjectArray[i].key, dataObjectArray[i].value);
        }
    }

    Id(text) {
        super.forEach(function (item) {
            if (item.value === text) {
                return item.key;
            }
        });
    }

    Text(id) {
        return super.get(id);
    }


}

class KeyValuePair {
    constructor(key, value) {
        this._key = key,
        this._value = value;
    }
    get Text() { return this._key; }
    get Key() { return this._key; }
    get Value() { return this._value; }
}

class WGENUMS {

    constructor() {
        this._url = '/api/v1/Projects/Dictionary';

        this._projectstatus = new TextIdDictionary();

        var self = this;

        $.ajax({
            async: false,
            type: 'GET',
            url: this._url + '/Status',
            success: function (data) {
                self._projectstatus.setAll(data);
            }
        });
    }
    get ProjectStatus() {

        return this._projectstatus;
    }
}


class application {
    constructor() {
        this._initialized = false;

        this.$table = $('#ProjectList');
        this.url = '/api/v1/Projects';

        this.enums = new WGENUMS();
    }

    main() {
        if (this._initialized) {
            return;
        }

        var self = this;

        //initialize table properties
        this.$table.DataTable({
            ajax: function (data, callback, settings) {
                var tableData = {
                    draw: 0,
                    recordsTotal: 0,
                    recordsFiltered: 0,
                    data: []
                };
                
                var calculateSums = this.api().columns('.summary').visible()[0];
                let searchName = $('#name').val();
                let searchStatus = $('#status').val();

                let input = new GetAllInput(data.start, data.length, data.order, data.columns);
                input.search = searchName;
                input.status = searchStatus;

                $.get("api/v1/Projects",input)
                    .done(function (result) {
                        var requests = [];

                        if (calculateSums) {
                            for (var i = 0; i < result.items.length; i++) {
                                let item = result.items[i];
                                let url = 'api/v1/Projects/' + item.projectId;

                                requests.push(
                                    $.get({ url: url + '/totalrse', itemNo: i })
                                    .done(function (value) {
                                        let item = result.items[this.itemNo];
                                        item.totalRse = value;
                                    }));


                                requests.push(
                                    $.get({ url: url + '/totalpower', itemNo: i })
                                    .done(function (value) {
                                        let item = result.items[this.itemNo];
                                        item.totalPower = value;
                                    }));

                                requests.push(
                                    $.get({ url: url + '/totalcost', itemNo: i })
                                    .done(function (value) {
                                        let item = result.items[this.itemNo];
                                        item.totalPrice = value;
                                    }));
                            };
                        }

                        //resolve all ajax calls
                        $.when.apply($,requests).then(function() {

                            tableData.draw = data.draw;
                            tableData.recordsTotal = result.totalCount;
                            tableData.recordsFiltered = result.filteredCount;
                            tableData.data = result.items;
                            callback(tableData);
                        });


                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        tableData.draw = data.draw;
                        tableData.recordsTotal = 0;
                        tableData.recordsFiltered = 0;
                        tableData.data = [];
                        callback(tableData);
                    });
            },
            columns: [
                {
                    data: "projectId",
                    name: "ProjectId",
                    title: "Nr",
                    type: "number",
                    visible: true,
                    orderable: true
                },
                {
                    data: "shortDescription",
                    name: "ShortDescription",
                    title: "Nazwa",
                    type: "html",
                    orderable: true,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            case 'display':
                                let a =
                                       '<a href="/Projects/Details/' + row.projectId + '" class="">' +
                                            row.shortDescription +
                                        '</a>';
                                return a;
                            default:
                                return row.shortDescription;
                        }
                    }
                },
                {
                    data: null,
                    name: "parentProjectId",
                    title: "Projekt Nadrzędny",
                    type: "html",
                    orderable: true,
                    render: function (data, type, row, meta) {
                        if (row.parentProjectId === null) {
                            return '';
                        }

                        switch (type) {
                            case 'display':

                                let a =
                                    '<a href="/Projects/Details/' + row.parentProjectId + '" class="">' +
                                        row.parentProject.shortDescription +
                                    '</a>';
                                return a;
                                
                            default:
                                return row.parentProject.shortDescription;
                        }
                    }
                },
                {
                    data: "status",
                    name: "status",
                    title: "Status",
                    orderable: true,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            case 'display':
                            case 'filter':
                                return self.enums.ProjectStatus.Text(data);
                            default:
                                return data;
                            //return data;
                        }

                    }
                },
                {
                    data: null,
                    name: "totalRSE",
                    title: "Liczba źródeł",
                    type: "html-num",
                    orderable: false,
                    className: "summary",
                    render: function (data, type, row, meta) {
                        if (row.totalRse === undefined) {
                            return '';
                        }
                        data = row.totalRse;
                        switch (type) {
                            case 'display':
                                return data;
                            default:
                                return data;
                        }
                    }
                },
                {
                    data: null,
                    name: "TotalPower",
                    title: "Moc całkowita",
                    type: "html-num",
                    orderable: false,
                    className: "summary",
                    render: function (data, type, row, meta) {
                        if (row.totalPower === undefined) {
                            return '';
                        }
                        data = row.totalPower;
                        switch (type) {
                            case 'display':
                                if (data > 1000) {
                                    let power = (data / 1000).toFixed(2);
                                    return '<span>' + power.toString() + ' MW' + '</span>';
                                } else {
                                    let power = data.toFixed(2);
                                    return '<span>' + power.toString() + ' kW' + '</span>';
                                }
                            default:
                                return data;
                        }
                    }
                },
                {
                    data: null,
                    name: "TotalPrice",
                    title: "Kwota",
                    type: "html-num",
                    className: "summary",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (row.totalPrice === undefined) {
                            return '';
                        }
                        data = row.totalPrice;
                        switch (type) {
                            case 'display':
                                if (data > 1000000) {
                                    let cost = (data / 1000000).toFixed(2);
                                    return '<span>' + cost + ' mln PLN' + '</span>';
                                } else if (data > 1000) {
                                    let cost = (data / 1000000).toFixed(2);
                                    return '<span>' + cost + ' tys. PLN' + '</span>';

                                } else {
                                    let cost = data.toFixed(2);
                                    return '<span>' + cost + ' tys. PLN' + '</span>';
                                }
                            default:
                                return data;
                        }
                    }
                },
                {
                    data: null,
                    name: "Actions",
                    title: "Akcje",
                    type: "html",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        
                        let edit = '<a href="/Projects/Edit/' + row.projectId + '" class="btn btn-sm btn-primary"><span class="small glyphicon glyphicon-pencil"></span></a>';
                        let del = '<a href="/Projects/Delete/' + row.projectId + '" class="btn btn-sm btn-danger"><span class="small glyphicon glyphicon-trash"></span></a>';

                        return edit + '|' + del;
                    }
                }
            ],
            stateSave: true,
            paging: true,
            pageLength: 8,
            lengthChange: true,
            language: WgLanguage,
            ordering: true,
            order: [['0', 'asc']],
            processing: true,
            serverSide: true,
            dom: //"<'row'<'col-sm-6'B><'col-sm-6'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-6'i><'col-sm-6'p>>",

            //buttons: [
            //    //{
            //    //    text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Resetuj',
            //    //    titleAttr: 'Przywróć ustawienia domyślne',
            //    //    className: 'btn',
            //    //    name: 'btnReset',
            //    //    action: function (e, dt, node, config) {
            //    //        //
            //    //        dt.table().buttons().disable();
            //    //        $('div#TaxTable_processing').show();

            //    //        var modal = $("#ResetModal");
            //    //        modal.data("section", "tax");
            //    //        modal.modal('show');
            //    //    }
            //    //},
            //    {
            //        text: '<span class="text-primary glyphicon glyphicon-refresh"></span> Odśwież',
            //        titleAttr: 'Odśwież zawartość z bazy danych',
            //        className: 'btn',
            //        name: 'btnRefresh',
            //        action: function (e, dt, node, config) {
            //            dt.ajax.reload();
            //            dt.rows().cells().invalidate().render();
            //            //dt.button('btnSave:name').remove();
            //        }
            //    },
            //    {
            //        text: 'Wł./Wył. Sumy',
            //        titleAttr: 'Włącz/wyłącz statystyki',
            //        className: 'btn btn-sm',
            //        name: 'btnToggleSums',
            //        action: function (e, dt, node, config) {
            //            let state = dt.columns('.summary').visible();
            //            dt.columns('.summary').visible(!state[0]);

            //            dt.columns.adjust().draw(false);
            //        }
            //    }
            //],
            //select: 'single',
            responsive: true,
            fixedHeader: {
                headerOffset: $('#NavBarMain').outerHeight()
            }
            //drawCallback: function (settings) {
            //    $('#TaxTable thead tr th')
            //        .each(function (i) {
            //        });
            //    var dt = this.api();
            //    var indexes = dt.rows().indexes();
            //    $('#TaxTable tbody tr').each(function (i) {
            //        var currentRow = dt.row($(this));
            //        var currentRowIndex = indexes.indexOf(currentRow.index());
            //        var prevRow = dt.rows(currentRowIndex - 1).data();
            //        var nextRow = dt.rows(currentRowIndex + 1).data();
            //    });
            
         });

        //set event handlers
        this.attachEvents();
    }

    attachEvents() {
        if (this._initialized) {
            return;
        }
        //FILTER
        $('#projectList-filter').click((e) => {
            e.preventDefault();
            $('button',$('#toolbar')).attr("disabled", true);

            let dt = this.$table.DataTable();

            dt.ajax.reload();
            dt.rows().cells().invalidate().render();

            $('button', $('#toolbar')).removeAttr("disabled");
        });
        //REFRESH
        $('#projectList-refresh').click((e) => {
            e.preventDefault();
            $('button', $('#toolbar')).attr("disabled", true);

            let dt = this.$table.DataTable();

            dt.ajax.reload();
            dt.rows().cells().invalidate().render();

            $('button', $('#toolbar')).removeAttr("disabled");
        });
        //TURN ON/OFF SUMMARY
        $('#projectList-summary').click((e) => {
            e.preventDefault();
            $('button', $('#toolbar')).attr("disabled", true);

            let dt = this.$table.DataTable();

            let state = dt.columns('.summary').visible()[0];
            dt.columns('.summary').visible(!state);

            dt.columns.adjust().draw(false);


            $('button', $('#toolbar')).removeAttr("disabled");
        });
    }
}

class DTOptions {

}
