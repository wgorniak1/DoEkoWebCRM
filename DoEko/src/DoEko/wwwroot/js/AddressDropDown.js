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