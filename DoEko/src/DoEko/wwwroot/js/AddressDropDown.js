﻿$(function () {
    if ($("#Address_StateId").val() === '') {
        var DistrictDefaultValue = "<option value=''>Wybierz województwo</option>";
        $("#Address_DistrictId").html(DistrictDefaultValue).show;
    }

    if ($("#Address_DistrictId").val() === '') {
        var CommuneDefaultValue = "<option value=''>Wybierz powiat</option>";
        $("#Address_CommuneId").html(CommuneDefaultValue).show();
    }


    $("#Address_StateId").change(function () {
        var selectedItemValue = $(this).val();
        var ddlDistricts = $("#Address_DistrictId");
        var ddlCommunes = $("#Address_CommuneId");

        if (selectedItemValue === '') {
            ddlDistricts.html("<option value=''>Wybierz województwo</option>").show;
        }
        else
            $.ajax({
                cache: false,
                type: "GET",
                url: '/Addresses/GetDistrictsAJAX',
                data: { "id": selectedItemValue },
                success: function (data) {
                    ddlDistricts.html('');
                    ddlDistricts.append($('<option></option>').val('').html('Wybierz'));
                    $.each(data, function (id, option) {
                        ddlDistricts.append($('<option></option>').val(option.id).html(option.name));
                    });


                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Błąd przy aktualizacji listy powiatów!');
                }
            });
        ddlCommunes.html("<option value=''>Wybierz powiat</option>").show();
    });

    $("#Address_DistrictId").change(function () {
        var selectedItemValue = $(this).val();
        var selectedStateId = $("#Address_StateId").val();
        var ddlCommune = $("#Address_CommuneId");

        if (selectedItemValue === '') {
            ddlCommune.html("<option value=''>Wybierz Powiat</option>").show;
        }
        else
            $.ajax({
                cache: false,
                type: "GET",
                url: '/Addresses/GetCommunesAJAX',
                data: {
                    "stateId": selectedStateId,
                    "districtId": selectedItemValue
                },
                success: function (data) {
                    ddlCommune.html('');
                    ddlCommune.append($('<option></option>').val('').html('Wybierz'));

                    $.each(data, function (id, option) {
                        ddlCommune.append($('<option></option>').val(option.id).html(option.name));
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Błąd przy aktualizacji listy gmin!');
                }
            });

    });

    $("#Address_CommuneId").change(function () {
        var selectedItem = $(this).val();
        var communeType = $("#Address_CommuneType");

        $.ajax({
            cache: false,
            type: "GET",
            url: '/Addresses/GetCommuneTypeAJAX',
            data: { "communeId": selectedItem },
            success: function (data) {
                communeType.val(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Błąd przy zapisywaniu rodzaju gminy!');
            }
        });
    });

});