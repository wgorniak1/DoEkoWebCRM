'use strict';

$(document).ready(function () {
    var app = new application();

    app.main();

});

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
            ajax: { url: this.url, dataSrc: "" },
            columns: [
                {
                    data: "projectId",
                    name: "projectId",
                    title: "Nr",
                    type: "number",
                    visible: true,
                    orderable: true
                },
                {
                    data: "shortDescription",
                    name: "shortDescription",
                    title: "Nazwa",
                    type: "string",
                    orderable: true
                },
                {
                    data: null,
                    name: "parentProjectId",
                    title: "Projekt Nadrzędny",
                    type: "string",
                    orderable: true,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            default:
                                return row.parentProjectId === null ? "" : row.parentProjectId;
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
                    name: "rseCount",
                    title: "Liczba źródeł",
                    type: "html-num",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            case 'display':
                            case 'filter':
                                //var content = '<input type="number" name="usableAreaMin" class="form-control input-sm wg-data wg-data-min" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
                                return '1';
                            default:
                                return data;
                        }
                    }
                },
                {
                    data: null,
                    name: "totalPower",
                    title: "Moc całkowita",
                    type: "html-num",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            case 'display':
                            case 'filter':
                                //var content = '<input type="number" name="usableAreaMin" class="form-control input-sm wg-data wg-data-min" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
                                return '1';
                            default:
                                return data;
                        }
                    }
                },
                {
                    data: null,
                    name: "totalPrice",
                    title: "Kwota",
                    type: "html-num",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        switch (type) {
                            case 'display':
                            case 'filter':
                                //var content = '<input type="number" name="usableAreaMin" class="form-control input-sm wg-data wg-data-min" value="' + data + '" step="0.1" min="0.0" max="99999.9">';
                                return '1';
                            default:
                                return data;
                        }
                    }
                },

            ],
            stateSave: true,
            paging: true,
            language: WgLanguage,
            ordering: true,
            order: [['0', 'asc']],
            processing: true,
            serverSide: false,
            //dom: "<'row wg-dt-padding'<'col-sm-12'B>>" +
            //"<'row'<'col-sm-12'tr>>" +
            //"<'row wg-dt-padding'<'col-sm-12'i>>",
            buttons: [
                //{
                //    text: '<span class="text-primary glyphicon glyphicon glyphicon-retweet"></span> Resetuj',
                //    titleAttr: 'Przywróć ustawienia domyślne',
                //    className: 'btn',
                //    name: 'btnReset',
                //    action: function (e, dt, node, config) {
                //        //
                //        dt.table().buttons().disable();
                //        $('div#TaxTable_processing').show();

                //        var modal = $("#ResetModal");
                //        modal.data("section", "tax");
                //        modal.modal('show');
                //    }
                //},
                {
                    text: '<span class="text-primary glyphicon glyphicon-refresh"></span> Odśwież',
                    titleAttr: 'Odśwież zawartość z bazy danych',
                    className: 'btn',
                    name: 'btnRefresh',
                    action: function (e, dt, node, config) {
                        dt.ajax.reload();
                        dt.rows().cells().invalidate().render();
                        dt.button('btnSave:name').remove();
                    }
                }
            ],
            select: 'single',
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

        alert('events');
    }
}

class DTOptions {

}
