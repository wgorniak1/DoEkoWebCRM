    'use strict';

    class DropDown {

        constructor(keyId, type, parent) {
            this.type = type;
            this.$element = $('select[data-address-id="' + keyId + '"][data-address="' + type + '"]');
            this.parent = parent;
            
            if (parent !== null) {
                //parent's parent changed
                if (parent.parent !== null) {
                    parent.parent.$element.on('change', this._clear(this.$element));
                }
                //parent changed
                parent.$element.on('change', this._clear(this.$element));
                parent.$element.on('change', this._refresh(this));
            }
        }

        //Type of dropdowns
        static TYPE() {
            return Object.freeze({
                state: "state",
                district: "district",
                commune: "commune",
                communeType: "communeType"
            });
        }
        //api urls
        static API() {
            return Object.freeze({
                state: "/api/v1/address/states",
                district: "/api/v1/address/states/{stateId}/districts",
                commune: "/api/v1/address/states/{stateId}/districts/{districtId}/communes",
            });
        }

        get value() {
            return this.$element.val();
        }

        get api() {
            return '';
        }

        _clear(element) {
            return function () {
                element.val('');
                if (element.is('select')) {
                    element.html('');
                    element.append($('<option></option>').val('').html('Wybierz'));
                }
                else {
                    //do nothing
                }
            }
        }

        _refresh(self) {
            return function () {
                //
                if (self.parent.value !== "") {
                    var jsxhr = $.getJSON(self.api);
                    //
                    jsxhr.done(self._fillOptions(self.$element));
                    //
                    jsxhr.fail(self._handleException(self.type));
                }
            }
        }

        _fillOptions(select) {
            return function (data) {
                $.each(data, function (index, item) {
                    select.append($('<option></option>').val(item.id).html(item.text));
                })
            }
        }

        _handleException(type) {
            return function (error) {
                var selectType = '';
                switch (type) {
                    case "state":    selectType = 'województw';
                    case "district": selectType = 'powiatów';
                    case "commune":  selectType = 'gmin';
                    default:         selectType = '';
                }
                var message = "Wystąpił problem podczas pobierania listy " + selectType;
                alert(message);
                console.log(message,error);
            }
        }
    }

    class DropDownState extends DropDown {

        get api() {
            return DropDown.API().state;
        }

        constructor(keyId) {
            super(keyId, DropDown.TYPE().state, null);
        }
    }

    class DropDownDistrict extends DropDown {
        get api() {
            var value = DropDown.API()
                .district
                .replace("{stateId}", this.parent.value);
            return value;
        }

        constructor(keyId, state) {
            super(keyId, DropDown.TYPE().district,state);
        }
    }
    class DropDownCommune extends DropDown {
        get api() {
            var value = DropDown.API()
                .commune
                .replace("{stateId}", this.parent.parent.value)
                .replace("{districtId}", this.parent.value);
            return value;
        }

        constructor(keyId, district) {
            super(keyId, DropDown.TYPE().commune, district);
            //
            this.$communeType = $('input[data-address-id="' + keyId + '"][data-address="' + DropDown.TYPE().communeType + '"]');

            //clear communeType when any of the parent drop downs has changed
            this.parent.parent.$element.on('change', this._clear(this.$communeType));
            this.parent.$element.on('change', this._clear(this.$communeType));
            this.$element.on('change', this._clear(this.$communeType));
            //update communeType when any commune drop down changed
            this.$element.on('change', this._refreshType(this));

        }

        _refreshType(self) {
            return function () {
                var value = self.value % 10;
                self.$communeType.val(value);
            }
        }

    }

    class Address {
        constructor(keyId) {
            this.state = new DropDownState(keyId);
            this.district = new DropDownDistrict(keyId, this.state);
            this.commune = new DropDownCommune(keyId, this.district);
        }

        static createColletion() {

            var uniqueObjects = {};

            $('[data-address-id]').map(function (index, item) {
                    var value = item.dataset.addressId;
                    uniqueObjects[value + '::' + typeof value] = value;
            });

            return Object.keys(uniqueObjects).map(function (v) {
                return uniqueObjects[v];
            }).map(function (item) {
                return new Address(item);
            });
        }
    }

var addrCollection = Address.createColletion();

//data-address='state' data-address-id='1'
//data-address='district' data-address-id='1'
//data-address='commune' data-address-id='1'
//data-address='communeType' data-address-id='1'


//$('body').on('change', '.address-state', function () { trigger('address:state:change', $(this).val, $(this).data('address-key')); });
//$('body').on('change', '.address-district', function () { trigger('address:district:change', $(this).val, $(this).data('address-key')); });
//$('body').on('change', '.address-commune', function () { trigger('address:commune:change', $(this).val, $(this).data('address-key')); });

//function onStateChange() {
//    var addressKey = $(this).data("address-key");

//    var selectedItemValue = $(this).val();
//    var ddlDistricts = $('.address-district[data-address-key="' + addressKey + '"]');
//    var ddlCommunes = $('.address-commune[data-address-key="' + addressKey + '"]');

//    if (selectedItemValue === '') {
//        ddlDistricts.html("<option value=''>Wybierz województwo</option>").show;
//    }
//    else
//        $.ajax({
//            cache: false,
//            type: "GET",
//            url: '/Addresses/GetDistrictsAJAX',
//            data: { "id": selectedItemValue },
//            success: function (data) {
//                ddlDistricts.html('');
//                ddlDistricts.append($('<option></option>').val('').html('Wybierz'));
//                $.each(data, function (id, option) {
//                    ddlDistricts.append($('<option></option>').val(option.id).html(option.name));
//                });


//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                alert('Błąd przy aktualizacji listy powiatów!');
//            }
//        });
//    ddlCommunes.html("<option value=''>Wybierz powiat</option>").show();

//}

//$(function () {

//    $(".address-state").change(function () {
//        var addressKey = $(this).data("address-key");

//        var selectedItemValue = $(this).val();
//        var ddlDistricts = $('.address-district[data-address-key="' + addressKey + '"]');
//        var ddlCommunes = $('.address-commune[data-address-key="' + addressKey + '"]');

//        if (selectedItemValue === '') {
//            ddlDistricts.html("<option value=''>Wybierz województwo</option>").show;
//        }
//        else
//            $.ajax({
//                cache: false,
//                type: "GET",
//                url: '/Addresses/GetDistrictsAJAX',
//                data: { "id": selectedItemValue },
//                success: function (data) {
//                    ddlDistricts.html('');
//                    ddlDistricts.append($('<option></option>').val('').html('Wybierz'));
//                    $.each(data, function (id, option) {
//                        ddlDistricts.append($('<option></option>').val(option.id).html(option.name));
//                    });


//                },
//                error: function (xhr, ajaxOptions, thrownError) {
//                    alert('Błąd przy aktualizacji listy powiatów!');
//                }
//            });
//        ddlCommunes.html("<option value=''>Wybierz powiat</option>").show();
//    });

//    $(".address-district").change(function () {
//        var addressKey = $(this).attr("data-address-key");
        
//        var selectedItemValue = $(this).val();
//        var selectedStateId = $('.address-state[data-address-key="' + addressKey + '"]').val();
//        var ddlCommune = $('.address-commune[data-address-key="' + addressKey + '"]');

//        if (selectedItemValue === '') {
//            ddlCommune.html("<option value=''>Wybierz Powiat</option>").show;
//        }
//        else
//            $.ajax({
//                cache: false,
//                type: "GET",
//                url: '/Addresses/GetCommunesAJAX',
//                data: {
//                    "stateId": selectedStateId,
//                    "districtId": selectedItemValue
//                },
//                success: function (data) {
//                    ddlCommune.html('');
//                    ddlCommune.append($('<option></option>').val('').html('Wybierz'));

//                    $.each(data, function (id, option) {
//                        ddlCommune.append($('<option></option>').val(option.id).html(option.name));
//                    });
//                },
//                error: function (xhr, ajaxOptions, thrownError) {
//                    alert('Błąd przy aktualizacji listy gmin!');
//                }
//            });

//    });

//    $(".address-commune").change(function () {
//        var addressKey = $(this).attr("data-address-key");
//        var selectedItem = $(this).val();
//        var communeType = $('.address-communetype[data-address-key="' + addressKey + '"]');

//        $.ajax({
//            cache: false,
//            type: "GET",
//            url: '/Addresses/GetCommuneTypeAJAX',
//            data: { "communeId": selectedItem },
//            success: function (data) {
//                communeType.val(data);
//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                alert('Błąd przy zapisywaniu rodzaju gminy!');
//            }
//        });
//    });

//});