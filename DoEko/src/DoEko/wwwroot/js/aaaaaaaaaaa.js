$(document).ready(function () {
    application.init();
});

$(document).ajaxSuccess(function (a, b, c, d) {
    if (d && d.action && d.action.Code == 100) location.reload();
    application.uiCheckLabels();
});

var values = [0, 1];
var minPrice;
var maxPrice;
var application = {
    init: function () {
        this.loadImages();
        this.events();
        this.uiCheckLabels();
    },
    loadImages: function () {
        var productImages = $('#main-section .product-item, #main-section .cart-item, #main-section .add-to-cart-popup').find('img');
        var sliderImage = $('.slider .product-image').find('img');
        var blogImage = $('.blog').find('img');

        $.each(productImages, function (index, element) {
            $(element).attr('src', $(element).data('src'));
        });

        if (sliderImage.length > 0) {
            // $.each(sliderImage, function(index, element) {
            //   (index === 0) ? $(element).attr('src', $(element).data('src')) : $(element).attr('src', '');
            // });
            $.each(sliderImage, function (index, element) {
                $(element).attr('src', $(element).data('src'));
            });
        }

        if (blogImage.length > 0) {
            $.each(blogImage, function (index, element) {
                $(element).attr('src', $(element).data('src'));
            });
        }
    },
    openCloseDropdown: function (e) {
        var menuButton = $(e.currentTarget);
        var dropdownMenu = menuButton.find('.dropdown-menu');

        if ($('.dropdown-menu').not('.hidden').length == 0 || menuButton.hasClass('open')) {
            dropdownMenu.toggleClass('hidden');
            this.uiMenuButtonAnimation(menuButton);
        } else {
            $('.dropdown-menu').toggleClass('hidden');
            this.uiMenuButtonAnimation($('.open'));
            this.uiMenuButtonAnimation(menuButton);
        }
        var width = window.innerWidth;
        var height = $(window).height();
        if (width <= 768) {
            $('.dropdown-menu').not('.hidden').css('min-height', height - 45);
        } else {
            // $('.dropdown-menu').not('.hidden').css('min-height', height - 80);
            // $('.dropdown-menu').not('.hidden').css('min-height', 80);
        }
    },
    closeDropdown: function (e) {
        if (window.innerWidth > 768 && window.innerWidth <= 1024 && $(e.currentTarget).hasClass('search-tab-close-icon')) {
            $(e.currentTarget).parent().parent().addClass('hidden');
            $(e.currentTarget).parent().parent().parent().removeClass('open');
        }
        if (window.innerWidth > 1024) {
            $('.search-section').removeClass('adv-search-opened');
        }
    },
    closeDropdownFromAdvanced: function (e) {
        if (window.innerWidth > 768 && window.innerWidth <= 1024 && $(e.currentTarget).hasClass('search-tab-close-icon')) {
            $(e.currentTarget).parent().parent().parent().parent().parent().addClass('hidden');
            $(e.currentTarget).parent().parent().parent().parent().parent().parent().removeClass('open');
        }
        if (window.innerWidth > 1024) {
            $('.search-section').removeClass('adv-search-opened');
        }
    },
    openRecentlySearched: function (e) {
        var height = $(e.currentTarget).parent().height();
        $(e.currentTarget).parent().parent().css('min-height', height);
        $(e.currentTarget).next().next().next().removeClass('hidden');
        $(e.currentTarget).addClass('active');
    },
    openRecentlySearchedFromAdvanced: function (e) {
        var height = $(e.currentTarget).parent().parent().parent().parent().prev().height();
        $(e.currentTarget).parent().parent().parent().parent().parent().css('min-height', height);
        $(e.currentTarget).parent().parent().parent().parent().prev().children().eq(4).removeClass('hidden');
        $(e.currentTarget).parent().parent().parent().parent().prev().children().eq(1).addClass('active');
    },
    closeRecentlySearched: function (e) {
        $(e.currentTarget).parent().parent().css('min-height', 80);
        $(e.currentTarget).parent().find('.recently-searched').addClass('hidden');
        $(e.currentTarget).parent().find('.recently-searched-icon').removeClass('active');
    },
    showStandardAccountForm: function (e) {
        $('#registration-company-form').addClass('hidden');
        $('#registration-standard-form').removeClass('hidden');
    },
    showCompanyAccountForm: function (e) {
        $('#registration-standard-form').addClass('hidden');
        $('#registration-company-form').removeClass('hidden');
    },
    showStandardAccountFormPopup: function (e) {
        $('#registration-company-form-popup').addClass('hidden');
        $('#registration-standard-form-popup').removeClass('hidden');
    },
    showCompanyAccountFormPopup: function (e) {
        $('#registration-standard-form-popup').addClass('hidden');
        $('#registration-company-form-popup').removeClass('hidden');
    },
    registerStandardAccount: function (e) {
        var data = $('#registration-standard-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                var confirmationMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Description + '</p>';
                application.createMessage(confirmationMessage, null, '');
                // window.location.replace('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    registerCompanyAccount: function (e) {
        var data = $('#registration-company-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                var confirmationMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Description + '</p>';
                application.createMessage(confirmationMessage, null, '');
                // window.location.replace('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    registerStandardAccountPopup: function (e) {
        var data = $('#registration-standard-form-popup').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                var confirmationMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Description + '</p>';
                application.createMessage(confirmationMessage, null, 'reload');
                // window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    registerCompanyAccountPopup: function (e) {
        var data = $('#registration-company-form-popup').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                var confirmationMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Description + '</p>';
                application.createMessage(confirmationMessage, null, 'reload');
                // window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    login: function (e) {
        var data = $('#login-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.replace('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    loginPopup: function (e) {
        var data = $('#login-form-popup').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    recoverPass: function (e) {
        var data = $('#reminder-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage(result.action.Description);
                $('#reminder-form').html(result.action.Description);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    resetPass: function (e) {
        var data = $('#password-reset-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage(result.action.Description);
                $('#password-reset-form').html(result.action.Description);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    askAboutProduct: function (e) {
        var data = $('#ask-about-product-form').serializeArray();
        var productId = $('.add-to-cart-popup input[name="productId"]').val();
        data.push({ name: "productId", value: productId });
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    recommendProduct: function (e) {
        var data = $('#recommend-product-form').serializeArray();
        var productId = $('.add-to-cart-popup input[name="productId"]').val();
        data.push({ name: "productId", value: productId });
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    notifyAboutAvailability: function (e) {
        var data = $('#notify-about-availability-form').serializeArray();
        var productId = $('.add-to-cart-popup input[name="productId"]').val();
        data.push({ name: "productId", value: productId });
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    askForPrice: function (e) {
        var data = $('#ask-for-price-form').serializeArray();
        var productId = $('.add-to-cart-popup input[name="productId"]').val();
        data.push({ name: "productId", value: productId });
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    askForPriceInList: function (e) {
        var data = $(e.currentTarget).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    recalculate: function () {
        var data = {
            __csrf: __CSRF,
            __action: 'Cart/Recalculate',
            __template: 'partials/cart/cart-template.html'
        };
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
                application.createMessage($('input[name="changes-info"]').val());
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    logout: function (e) {
        var data = {
            __csrf: __CSRF,
            __action: 'Customer/Logout'
        };
        $.post(null, data, function (result) {
            if (result.action.Result) {
                var pageId;
                $.get(location.href, { __collection: 'page' }, function (result) {
                    pageId = result.collection.PageId;
                }).done(function () {
                    if (pageId == 6) {
                        window.location.replace('');
                    } else {
                        location.reload();
                    }
                });
            } else {
                if (result.action.Validation != null) {
                    errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                } else {
                    errorMessage = '<div class="title">' + result.action.Message + '</div>';
                }
                application.createMessage(errorMessage);
            }
        });
    },
    showAdvancedSearch: function () {
        $('.advanced-filters').removeClass('hidden');
        $('.search-form').addClass('hidden');
        if ($('#SearchAdvancedForm').index() == -1) {
            $.get('', { __template: 'partials/common/search-advanced.html' }, function (result) {
                $('.advanced-filters').html(result.template);
                $('.mCustomScrollbar').mCustomScrollbar();
                if (window.innerWidth > 768) {
                    application.uiMakeUndercategoriesVisibleInAdvancedSearchFilters();
                }
                application.uiSetSwitchNameWidthInHeader();
                application.events();
            });
        } else {
            if (window.innerWidth > 768) {
                application.uiMakeUndercategoriesVisibleInAdvancedSearchFilters();
            }
        }
    },
    hideAdvancedSearch: function () {
        $('.advanced-filters').addClass('hidden');
        values = [0, 1];
        $('.search-form').removeClass('hidden');
    },
    openCloseCategories: function (e) {
        if ($(e.currentTarget).hasClass('fa') || $(e.currentTarget).hasClass('heading-container')) {
            var undercategory = $(e.currentTarget).parent().find('.undercategories');
        } else {
            var undercategory = $(e.currentTarget).find('.undercategories');
        }

        undercategory.toggleClass('hidden');

    },
    openCloseUndercategories: function (e) {
        var underundercategory = $(e.target).parent().find('.underundercategories');

        underundercategory.toggleClass('hidden');
    },
    showPopup: function (e) {
        var parent = $(e.target).closest('div');

        parent.find('.popup-dialog').eq(0).removeClass('hidden');
    },
    showPopupNext: function (e) {
        var parent = $(e.currentTarget).parent().next();

        parent.removeClass('hidden');
    },
    showPopupNextClosest: function (e) {
        var parent = $(e.currentTarget).next();

        parent.removeClass('hidden');
    },
    showPopupWithForm: function (e) {
        var template = $(e.currentTarget).data('template');
        var popupClass = $(e.currentTarget).data('popup-class');
        if ($(popupClass).children().index() == -1) {
            $.get(null, { __template: template }, function (result) {
                $(popupClass).removeClass('hidden').html(result.template);
            });
        } else {
            $(popupClass).removeClass('hidden');
        }
    },
    showPopupComplaint: function (e) {
        $(e.currentTarget).parent().find('.complaint-popup').removeClass('hidden');
    },
    showPopupReturn: function (e) {
        $(e.currentTarget).parent().find('.return-popup').removeClass('hidden');
    },
    showPopupComplaintDetails: function (e) {
        var complaintIndex = $(e.currentTarget).index();
        var divToReplace = $(e.currentTarget).find('.complaint-details').eq(0);
        var inputs = $(e.currentTarget).find('> form input');
        var complaintId = inputs.eq(0).val();
        var complaintString = inputs.eq(0).attr('name');
        var template = inputs.eq(1).val();
        var collection = inputs.eq(2).val();
        var url = location.pathname + '?' + complaintString + '=' + complaintId;
        $.get(url, { __template: template, __collection: collection }, function (result) {
            divToReplace.replaceWith(result.template);
            $('body').find('.complaint-details-popup').eq(complaintIndex).removeClass('hidden');
            application.loadImages();
        });
    },
    toggleScrollToTopButton: function () {
        if ($(window).scrollTop() > $(window).height()) {
            $('.scroll-to-top').show(300);
        } else {
            $('.scroll-to-top').hide(300);
        }
    },
    hidePopup: function (e) {
        var parent = $(e.target).closest('.popup-dialog');

        parent.addClass('hidden');
    },
    createMessage: function (message, delay, url) {

        var currentDelay = 1500;
        if (delay != null && delay != undefined) {
            currentDelay = delay;
        }

        var container = '<div class="message-popup-background"><div class="message-popup"><span>'
            + message + '</span></div></div>';

        $('.message-popup-background').remove();
        $('body').append(container);
        $('.message-popup-background').delay(currentDelay).fadeOut(function () {
            if (url != null && url != undefined) {
                if (url == 'reload') {
                    window.location.reload();
                } else {
                    window.location.replace(url);
                }
            }
        });
    },
    newsletterSubscribe: function (e) {
        var data = $('#NewsletterSubscribeForm').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                $('.newsletter').remove();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                    application.uiCheckLabels();
                }
            }
        });
    },
    newsletterUnsubscribe: function (e) {
        var data = $('#unsubscribe-newsletter-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.hidePopup(e);
                application.uiPreventScrolling();
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                    application.uiCheckLabels();
                }
            }
        });
    },
    showProductDetailsPopup: function (e) {
        if ($('#main-section').find('.comparer').index() >= 0) {
            var productUrl = $(e.currentTarget).prev().val();
        } else {
            var productUrl = $(e.currentTarget).prev().attr("href");
        }

        var url = location.hostname + productUrl;
        $.post(url, { __collection: 'product-details', __template: 'partials/product/product-popup.html' }, function (result) {
            console.log(result);
            $('.product-popup').replaceWith(result.template);
            application.addClipAndBatchToCart(e);
            application.showPopup(e);
            application.loadImages();
            application.uiCheckLabels();
            application.uiPreventScrolling();
        });
    },
    addClipAndBatchToCart: function (e) {
        if (typeof e == 'undefined') {
            var det = $('#AddToCartForm');
        } else {
            var det = $(e.currentTarget).next().find('#AddToCartForm');
        }
        var sp = det.find('#supplyId');
        var sup = sp.data('supplies'); //pobieram obiekt z #supplyId
        if (sup) {
            var s = sup.Supplies;
        }
        var fDrop = det.find('.attributes-select'); //pobieram wszystkie kontenery dla poziomów

        // jeśli fantom
        if (sp.attr('data-clip') == 1) {
            var parentId = det.find('#supplyId').data('parent-id');
            var result = null;

            // rekursywnie przeszukuje drzewo w poszukiwaniu ojca
            for (var el in s) {
                if (result == null) {
                    var element = s[el];
                    var result = application.searchForSupplyId(element, parentId);
                    if (result != null) {
                        var elIndex = el;
                    }
                }
            }

            // jeśli nie ma ojca...
            if (result == null) {
                var length = s.length;

                // ... ale są dzieci
                if (length > 0) {
                    fDrop.each(function () {
                        application.SetSupl($(this), s);
                        s = $(this).children('[data-selected = "true"]').data('el').Supplies; // to s zostanie w funkcji SetSupl przypisane jako data-el dla każdego spana z wartością atrybutu
                        lvl++;
                    });
                    $('[data-selected = "true"]').trigger('click');
                } else {
                    fDrop.each(function () {
                        $(this).prev().addClass('hidden');
                    });
                }

            } else {
                var valId = result.Key.split(',');
                var lvl = 0;
                //dla wszystkich tych kontenerów wykonuje funkcje i podmieniam s na kolejny poziom fantomów
                fDrop.each(function () {
                    application.SetSuplFirst($(this), s, lvl, valId);
                    s = $(this).children('[data-selected = "true"]').data('el').Supplies; // to s zostanie w funkcji SetSupl przypisane jako data-el dla każdego spana z wartością atrybutu
                    lvl++;
                });
            }

        } else {
            //dla wszystkich tych kontenerów wykonuje funkcje i podmieniam s na kolejny poziom fantomów
            fDrop.each(function () {
                application.SetSupl($(this), s);
                s = $(this).children('[data-selected = "true"]').data('el').Supplies; // to s zostanie w funkcji SetSupl przypisane jako data-el dla każdego spana z wartością atrybutu
                lvl++;
            });
            $('[data-selected = "true"]').trigger('click');
        }
    },
    SetSuplFirst: function (drop, s, lvl, valId) {
        var det = drop.parent().parent();
        var fDrop = det.find('.attributes-select'); //pobieram wszystkie kontenery dla poziomów

        drop.empty();
        for (var el in s) {
            drop.append($('<span class="button-option"></span>').data('value', s[el].ValueId).text(s[el].Value).data('el', s[el]));
        }
        drop.append($('<input type="hidden" name="attributeId"/>').val(s[el].ValueId));

        var buttons = drop.find('.button-option');
        buttons.each(function () {
            if ($(this).data('value') == valId[lvl]) {
                $(this).addClass('active').attr('data-selected', 'true');
            }
        });

        var spLast = fDrop.last().find('[data-selected = "true"]').data('el');
        if (spLast && spLast.SupplyId) { //sprawdzam, kiedy jest to wybrany element z ostatniego poziomu, żeby do #supplyId wpisać id fantomu
            det.find('#supplyId').val(spLast.SupplyId);
            if (spLast && spLast.StockLevel) {
                det.find('#supplyId').attr('data-stock-control', spLast.StockLevel.Control);
                det.find('#supplyId').attr('data-stock-quantity-control', spLast.StockLevel.QuantityControl);
                det.find('#supplyId').attr('data-stock-value', spLast.StockLevel.Value);
                if (spLast.StockLevel.Value == 1) {
                    $('.button-plus-add-product').addClass('max');
                } else {
                    $('.button-plus-add-product').removeClass('max');
                }
            }
            if (spLast && spLast.Product) {
                $('.add-to-cart-popup input[name="productId"]').val(spLast.SupplyId);
                var t = $('.attributes-select').eq(0);
                if (spLast && spLast.Product.AttributesPolyvalent[0]) {
                    var attributes = spLast.Product.AttributesPolyvalent;
                    for (var i = 0; i < attributes.length; i++) {
                        t.parent().append('<div class="attr-poly"><input name="attributeId" type="hidden" value="' + attributes[i].Values[0].ValueId + '"/><h3>' + attributes[i].Name + '</h3></div>');
                        var values = attributes[i].Values;
                        for (var j = 0; j < values.length; j++) {
                            $('.attr-poly').append(' <input class="button-option value' + j + ' attributeId" type="button" value="' + values[j].Value + '"/><input type="hidden" value="' + values[j].ValueId + '"/>');
                            if (values[j].ValueId == attributes[i].Values[0].ValueId) {
                                $('.value' + j).addClass('active');
                            } else {
                                $('.value' + j).removeClass('active');
                            }
                        }
                    }
                } else {
                    if ($('.attr-poly').index() >= 0) {
                        $('.attr-poly').remove();
                    }
                }
                if (spLast && spLast.Product.AttributesEditable[0]) {
                    for (var i = 0; i < spLast.Product.AttributesEditable.length; i++) {
                        t.parent().append('<div class="attr-ed"><input class="attributeEditable' + i + '" name="attributeEditable" type="text" value="" /><label>' + spLast.Product.AttributesEditable[i].Name + '</label></div>');
                    }
                } else {
                    if ($('.attr-ed').index() >= 0) {
                        $('.attr-ed').remove();
                    }
                }
            }
        }
    },
    searchForSupplyId: function (element, matchingSupplyId) {
        if (element.SupplyId == matchingSupplyId) {
            return element;
        } else if (element.Supplies != null) {
            var i;
            var result = null;
            for (i = 0; result == null && i < element.Supplies.length; i++) {
                result = application.searchForSupplyId(element.Supplies[i], matchingSupplyId);
            }
            return result;
        }
        return null;
    },
    SetSupl: function (drop, s) {
        var det = drop.parent().parent();
        var fDrop = det.find('.attributes-select'); //pobieram wszystkie kontenery dla poziomów
        drop.empty();
        //dla wszystkich wartości dla danego poziomu cechy przypisuję w data-el jsona z danym poziomem cechy
        for (var el in s) {
            drop.append($('<span class="button-option"></span>').data('value', s[el].ValueId).text(s[el].Value).data('el', s[el]));
        }
        drop.append($('<input type="hidden" name="attributeId"/>').val(s[el].ValueId));
        drop.children().first().attr('data-selected', 'true');

        var spLast = fDrop.last().find('[data-selected = "true"]').data('el');
        if (spLast && spLast.SupplyId) { //sprawdzam, kiedy jest to wybrany element z ostatniego poziomu, żeby do #supplyId wpisać id fantomu
            det.find('#supplyId').val(spLast.SupplyId);
            if (spLast && spLast.StockLevel) {
                det.find('#supplyId').attr('data-stock-control', spLast.StockLevel.Control);
                det.find('#supplyId').attr('data-stock-img-url', spLast.StockLevel.ImageUrl);
                det.find('#supplyId').attr('data-stock-quantity-control', spLast.StockLevel.QuantityControl);
                det.find('#supplyId').attr('data-stock-text', spLast.StockLevel.Text);
                det.find('#supplyId').attr('data-stock-type', spLast.StockLevel.Type);
                det.find('#supplyId').attr('data-stock-value', spLast.StockLevel.Value);
                if ($('.quantity-field').val() > spLast.StockLevel.Value) {
                    $('.quantity-field').val(spLast.StockLevel.Value);
                }
                if ($('.quantity-field').val() == spLast.StockLevel.Value) {
                    $('.button-plus-add-product').addClass('max');
                } else {
                    $('.button-plus-add-product').removeClass('max');
                }
            }
            if (spLast && spLast.Product) {
                $('.add-to-cart-popup input[name="productId"]').val(spLast.SupplyId);
                det.find('#supplyId').attr('data-imageId', spLast.Product.ImageId);
                det.find('#supplyId').attr('data-images', spLast.Product.Images);
                det.find('#supplyId').attr('data-price', spLast.Product.Price);
                det.find('#supplyId').attr('data-old-price', spLast.Product.PreviousPrice);
                det.find('#supplyId').attr('data-points', spLast.Product.Points);
                det.find('#supplyId').attr('data-points-price', spLast.Product.PointsPrice);
                det.find('#supplyId').attr('data-id', spLast.Product.Id);
                det.find('#supplyId').attr('data-code', spLast.Product.Code);
                det.find('#supplyId').attr('data-vat', spLast.Product.VAT);
                det.find('#supplyId').attr('data-weight', spLast.Product.Weight);
                det.find('#supplyId').attr('data-symbol', spLast.Product.Symbol);
                det.find('#supplyId').attr('data-upc', spLast.Product.UPC);
                if (spLast.Product.Availability.Date != null) {
                    var date = new Date(spLast.Product.Availability.Date);
                    date = date.toLocaleDateString();
                    if (date.length == 9) {
                        date = '0' + date;
                    }
                    date = date.split('.');
                    var day = date[0];
                    var month = date[1];
                    var year = date[2];
                    date = '(' + year + '-' + month + '-' + day + ')';
                    det.find('#supplyId').attr('data-availability-date', date);
                } else {
                    det.find('#supplyId').attr('data-availability-date', spLast.Product.Availability.Date);
                }
                det.find('#supplyId').attr('data-availability-img-url', spLast.Product.Availability.ImageUrl);
                det.find('#supplyId').attr('data-availability-status', spLast.Product.Availability.Status);
                det.find('#supplyId').attr('data-availability-text', spLast.Product.Availability.Text);
                det.find('#supplyId').attr('data-availability-type', spLast.Product.Availability.Type);
                det.find('#supplyId').attr('data-ask-for-price', spLast.Product.AskForPrice);
                var t = $('.attributes-select').eq(0);
                if (spLast && spLast.Product.AttributesPolyvalent[0]) {
                    $('.attr-poly').remove();
                    var attributes = spLast.Product.AttributesPolyvalent;
                    for (var i = 0; i < attributes.length; i++) {
                        t.parent().append('<div class="attr-poly"><input name="attributeId" type="hidden" value="' + attributes[i].Values[0].ValueId + '"/><h3>' + attributes[i].Name + '</h3></div>');
                        var values = attributes[i].Values;
                        for (var j = 0; j < values.length; j++) {
                            $('.attr-poly').append(' <input class="button-option value' + j + ' attributeId" type="button" value="' + values[j].Value + '"/><input type="hidden" value="' + values[j].ValueId + '"/>');
                            if (values[j].ValueId == attributes[i].Values[0].ValueId) {
                                $('.value' + j).addClass('active');
                            } else {
                                $('.value' + j).removeClass('active');
                            }
                        }
                    }
                } else {
                    if ($('.attr-poly').index() >= 0) {
                        $('.attr-poly').remove();
                    }
                }
                if (spLast && spLast.Product.AttributesEditable[0]) {
                    $('input[name="attributeEditable"]').remove();
                    for (var i = 0; i < spLast.Product.AttributesEditable.length; i++) {
                        t.parent().append('<div class="attr-ed"><input class="attributeEditable' + i + '" name="attributeEditable" type="text" value="" /><label>' + spLast.Product.AttributesEditable[i].Name + '</label></div>');
                    }
                } else {
                    if ($('.attr-ed').index() >= 0) {
                        $('.attr-ed').remove();
                    }
                }
            }
        }
        $('[data-selected = "true"]').addClass('active');
    },
    setSupplyId: function (e) {
        var det = $(e.currentTarget).parent().parent().parent();
        var span = $(e.currentTarget);
        var t = $(e.currentTarget).parent();

        t.children('[data-selected = "true"]').removeAttr('data-selected').removeClass('active');
        span.attr('data-selected', 'true');

        var s = span.data('el');

        if (s && s.SupplyId) {
            det.find('#supplyId').val(s.SupplyId);
            if (s && s.StockLevel) {
                det.find('#supplyId').attr('data-stock-control', s.StockLevel.Control);
                det.find('#supplyId').attr('data-stock-img-url', s.StockLevel.ImageUrl);
                det.find('#supplyId').attr('data-stock-quantity-control', s.StockLevel.QuantityControl);
                det.find('#supplyId').attr('data-stock-text', s.StockLevel.Text);
                det.find('#supplyId').attr('data-stock-type', s.StockLevel.Type);
                det.find('#supplyId').attr('data-stock-value', s.StockLevel.Value);
                if ($('.quantity-field').val() > s.StockLevel.Value) {
                    $('.quantity-field').val(s.StockLevel.Value);
                }
                if ($('.quantity-field').val() == s.StockLevel.Value) {
                    $('.button-plus-add-product').addClass('max');
                } else {
                    $('.button-plus-add-product').removeClass('max');
                }
            }
            if (s && s.Product) {
                $('.add-to-cart-popup input[name="productId"]').val(s.SupplyId);
                det.find('#supplyId').attr('data-imageId', s.Product.ImageId);
                det.find('#supplyId').attr('data-images', s.Product.Images);
                det.find('#supplyId').attr('data-price', s.Product.Price);
                det.find('#supplyId').attr('data-old-price', s.Product.PreviousPrice);
                det.find('#supplyId').attr('data-points', s.Product.Points);
                det.find('#supplyId').attr('data-points-price', s.Product.PointsPrice);
                det.find('#supplyId').attr('data-id', s.Product.Id);
                det.find('#supplyId').attr('data-code', s.Product.Code);
                det.find('#supplyId').attr('data-vat', s.Product.VAT);
                det.find('#supplyId').attr('data-weight', s.Product.Weight);
                det.find('#supplyId').attr('data-symbol', s.Product.Symbol);
                det.find('#supplyId').attr('data-upc', s.Product.UPC);
                if (s.Product.Availability.Date != null) {
                    var date = new Date(s.Product.Availability.Date);
                    date = date.toLocaleDateString();
                    if (date.length == 9) {
                        date = '0' + date;
                    }
                    date = date.split('.');
                    var day = date[0];
                    var month = date[1];
                    var year = date[2];
                    date = '(' + year + '-' + month + '-' + day + ')';
                    det.find('#supplyId').attr('data-availability-date', date);
                } else {
                    det.find('#supplyId').attr('data-availability-date', s.Product.Availability.Date);
                }
                det.find('#supplyId').attr('data-availability-img-url', s.Product.Availability.ImageUrl);
                det.find('#supplyId').attr('data-availability-status', s.Product.Availability.Status);
                det.find('#supplyId').attr('data-availability-text', s.Product.Availability.Text);
                det.find('#supplyId').attr('data-availability-type', s.Product.Availability.Type);
                det.find('#supplyId').attr('data-ask-for-price', s.Product.AskForPrice);
                if (s.Product.AttributesPolyvalent[0]) {
                    $('.attr-poly').remove();
                    var attributes = s.Product.AttributesPolyvalent;
                    for (var i = 0; i < attributes.length; i++) {
                        t.parent().append('<div class="attr-poly"><input name="attributeId" type="hidden" value="' + attributes[i].Values[0].ValueId + '"/><h3>' + attributes[i].Name + '</h3></div>');
                        var values = attributes[i].Values;
                        for (var j = 0; j < values.length; j++) {
                            $('.attr-poly').append(' <input class="button-option value' + j + ' attributeId" type="button" value="' + values[j].Value + '"/><input type="hidden" value="' + values[j].ValueId + '"/>');
                            if (values[j].ValueId == attributes[i].Values[0].ValueId) {
                                $('.value' + j).addClass('active');
                            } else {
                                $('.value' + j).removeClass('active');
                            }
                        }
                    }
                } else {
                    if ($('.attr-poly').index() >= 0) {
                        $('.attr-poly').remove();
                    }
                }
                if (s && s.Product.AttributesEditable[0]) {
                    $('input[name="attributeEditable"]').remove();
                    for (var i = 0; i < s.Product.AttributesEditable.length; i++) {
                        t.parent().append('<div class="attr-ed"><input class="attributeEditable' + i + '" name="attributeEditable" type="text" value="" /><label>' + s.Product.AttributesEditable[i].Name + '</label></div>');
                    }
                } else {
                    if ($('.attr-ed').index() >= 0) {
                        $('.attr-ed').remove();
                    }
                }
            }
        } else {
            t.nextAll('div').each(function () {
                if ($(this).index() != -1) {
                    application.SetSupl($(this), s.Supplies);
                    s = $(this).find('[data-selected="true"]').data('el');
                }
            });
        }
    },
    setUnitId: function (e) {
        var unitId = $(e.currentTarget).next().val();
        $(e.currentTarget).parent().parent().find('input[name="unitId"]').val(unitId);
    },
    setAttributeId: function (e) {
        var attributeId = $(e.currentTarget).next().val();
        $(e.currentTarget).parent().find('input[name="attributeId"]').val(attributeId);
    },
    addToCart: function (e) {
        e.preventDefault();
        var productId = $(e.currentTarget).data('product-id');
        $('input[name="productId"]').val(productId);
        var length = parseInt($(e.currentTarget).parent().parent().find('.counter').text());
        // for (i=0; i<length; i++){
        //   var value = $( '.attributeEditable' + i + '.used' ).val();
        //   $(e.currentTarget).parent().parent().append('<input name="attributeEditable" type="hidden" value="' + value + '" />');
        // }

        var data = $(e.currentTarget).parent().parent().serializeArray();
        console.dir(data);

        $.post(null, data, function (result) {
            console.log(result.action);
            if (result.action.Result) {
                application.uiIncrementProductsInCart();
                application.uiPreventScrolling();
                $('input[name="attributeEditable"]').remove();
                if ($('section.product-details').index() >= 0) {
                    $('.instant-show .modal-box').html(result.template);
                    application.addClipAndBatchToCart();
                    application.loadImages();
                }
                var rememberedChoice = application.getCookie('rememberChoice');
                if (rememberedChoice == 'goToCart') {
                    var url = $('input[name="main-url"]').val() + $('input[name="go-to-cart"]').val();
                    window.location.replace(url);
                    application.createMessage('<div class="title">' + result.action.Message + '</div>');
                } else if (rememberedChoice == 'stayOnPage') {
                    application.createMessage('<div class="title">' + result.action.Message + '</div>');
                    application.uiPreventScrolling();
                } else {
                    if ($('input[name="after-adding-to-cart"]').val() == 1) {
                        var url = $('input[name="main-url"]').val() + $('input[name="go-to-cart"]').val();
                        window.location.replace(url);
                        application.createMessage('<div class="title">' + result.action.Message + '</div>');
                    } else if ($('input[name="after-adding-to-cart"]').val() == 3) {
                        $('.after-adding-to-cart-popup').removeClass('hidden');
                        application.uiSetSwitchNameWidthInAfterAddingToCartPopup();
                    } else {
                        application.createMessage('<div class="title">' + result.action.Message + '</div>');
                        application.uiPreventScrolling();
                    }
                }
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else if (result.action.Description != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + ' ' + result.action.Description;
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage, 3000);
                    // $( 'input[name="attributeEditable"]' ).remove();
                }
            }
        });
    },
    removeFromCart: function (e) {
        var product = $(e.target).closest('.cart-item');
        var itemNumber = product.data('item-number');
        var totalCost = $('#total-cost').text();
        var productPrice = product.find('.product-price').text();
        var data = {
            __collection: 'customer.Cart.Value',
            __csrf: __CSRF,
            __template: 'partials/cart/cart-template.html',
            __action: 'cart/PositionDelete',
            no: itemNumber
        };

        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
                application.uiDecrementProductsInCart();
                application.uiDecrementTotalCost(totalCost, productPrice);
                product.remove();
            }
        });
    },
    recalculateProductsValue: function (e) {
        var parent = $(e.target).closest('.cart-item');
        var itemNumber = parent.data('item-number');
        var itemQuantity = parseFloat(parent.find('.quantity-field').val());
        var data = {
            __collection: 'customer.Cart.Value',
            __template: 'partials/cart/cart-template.html',
            __csrf: __CSRF,
            __action: 'cart/QuantityChange',
            no: itemNumber,
            quantity: itemQuantity
        };
        var errorMessage;

        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else if (result.action.Description != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + ' ' + result.action.Description;
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                    $('.shopping-cart').replaceWith(result.template);
                    application.loadImages();
                }
            }
        });
    },
    orderNextStep: function (e) {
        if ($('#delivery-address-data').index() >= 0) {
            var allEdited = true;
            $('#delivery-address-data input[required]').each(function () {
                if (allEdited === false) {
                    return;
                } else {
                    if ($(this).val() == '') {
                        allEdited = false;
                    }
                }
            });
            if (allEdited === false) {
                var errorMessage = $(e.currentTarget).next().next().val();
                application.createMessage(errorMessage, 3000);
                return;
            } else if ($('input[name="invoice"]').hasClass('company') && $('input[name="tin"]').val() == '') {
                var message = $('input[name="company"]').attr('data-info');
                application.createMessage(message, 2000);
            } else {
                var data = $('#delivery-address-data').serializeArray();
                $.post(null, data, function (result) {
                    if (result.action.Result) {
                        var container = $('body');
                        var data = {
                            __template: 'partials/cart/cart-template.html',
                            __csrf: __CSRF,
                            __action: 'Order/StepNext'
                        };
                        $.post(null, data, function (result) {
                            if (result.action.Result) {
                                $('.shopping-cart').replaceWith(result.template);
                                if ($('body').find('#invoice-address-data').index() >= 0) {
                                    application.uiSetSwitchNameWidthInInvoice();
                                }
                                application.loadImages();
                                application.uiSetSwitchNameWidthInSummaryCheckboxes();
                                application.uiScrollToTop(container);
                                application.uiCheckLabels();
                            } else {
                                if (result.action.Code != 100) {
                                    if (result.action.Validation != null) {
                                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                                    } else {
                                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                                    }
                                    application.createMessage(errorMessage, 3000);
                                }
                            }
                        });
                    } else {
                        if (result.action.Code != 100) {
                            if (result.action.Validation != null) {
                                errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                            } else {
                                errorMessage = '<div class="title">' + result.action.Message + '</div>';
                            }
                            application.createMessage(errorMessage, 3000);
                            return;
                        }
                    }
                });
            }
        } else {
            var container = $('body');
            var data = {
                __template: 'partials/cart/cart-template.html',
                __csrf: __CSRF,
                __action: 'Order/StepNext'
            };
            $.post(null, data, function (result) {
                if (result.action.Result) {
                    if ((result.template).indexOf('name="sel-del-met" value="' + 1 + '"') !== -1 || (result.template).indexOf('name="sel-del-met" value="' + 2 + '"') !== -1) {
                        application.orderPrevStep();
                    } else {
                        $('.shopping-cart').replaceWith(result.template);
                        if ($('body').find('#invoice-address-data').index() >= 0) {
                            application.uiSetSwitchNameWidthInInvoice();
                        }
                        application.loadImages();
                        application.uiSetSwitchNameWidthInSummaryCheckboxes();
                        application.uiScrollToTop(container);
                        application.uiCheckLabels();
                    }
                } else {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        }
                        application.createMessage(errorMessage);
                    }
                }
            });
        }
    },
    orderPrevStep: function (e) {
        var container = $('.shopping-cart');
        var data = {
            __template: 'partials/cart/cart-template.html',
            __csrf: __CSRF,
            __action: 'Order/StepPrev'
        };
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').html(result.template);
                if ($('body').find('#invoice-address-data').index() >= 0) {
                    application.uiSetSwitchNameWidthInInvoice();
                }
                application.loadImages();
                application.uiScrollToTop(container);
                application.uiCheckLabels();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    saveDeliveryAddress: function () {
        var allEdited = true;
        $('#delivery-address-data input[required]').each(function () {
            if ($(this).val() == '') {
                allEdited = false;
            }
        });
        if (allEdited == true) {
            var data = $('#delivery-address-data').serializeArray();
            $.post(null, data, function (result) {
                if (!result.action.Result) {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        }
                        application.createMessage(errorMessage, 3000);
                        $('input[name="error"]').val('true');
                    }
                }
            });
        }
    },
    selectDeliveryCountry: function (e) {
        var countryCode = $('.select-country').val();
        var data = {
            __template: 'partials/cart/cart-template.html',
            __csrf: __CSRF,
            __action: 'Order/DeliveryCountryChange',
            countryCode: countryCode
        };
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
                application.uiCheckLabels();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage, 3000);
                }
            }
        });
    },
    selectCountryRegistration: function (e) {
        var countryCode = $('.select-country-registration').val();
        $('input[name="countryCode"]').val(countryCode);
        var data = {
            __csrf: __CSRF,
            __action: 'Order/DeliveryCountryChange',
            countryCode: countryCode
        };
        $.post(null, data, function () { });
    },
    changeDeliveryCountry: function (e) {
        var countryCode = $('#change-country').val();
        var data = {
            __template: 'partials/cart/cart-template.html',
            __csrf: __CSRF,
            __action: 'Order/DeliveryCountryChange',
            countryCode: countryCode
        };
        $.post(null, data, function (result) {
            if (!result.action.Result) {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage, 3000);
                }
            }
        });
    },
    showPaymentOptions: function (e) {
        $('.payment').removeClass('hidden');
        $('.delivery-method-payments').addClass('hidden');
        $('.delivery-method-payments').removeClass('chosen');
        $('.delivery-method-payments input').removeClass('chosen');
        $('.delivery-method-payments input').prop('checked', false);
        var i = $(e.currentTarget).prev().val();
        $('.delivery-method-payment' + i).removeClass('hidden');
        $('.delivery-method-payment' + i).addClass('chosen');

        if ($(e.currentTarget).hasClass('many')) {
            var index = $(e.currentTarget).attr('data-many');
            $('.delivery-method-payments.many' + index).removeClass('hidden');
        }

        var paymentId = $('.delivery-method-payment' + i).find('input').eq(0).val();
        $('.delivery-method-payment' + i).find('input').eq(0).addClass('chosen');
        $('.delivery-method-payment' + i).find('input').eq(0).prop('checked', true);


        $('#DeliveryMethodsAndPayment input[name="id"]').val(paymentId);
        var data = $('#DeliveryMethodsAndPayment').serializeArray();
        $.post(null, data, function (result) {
            $('.shopping-cart').replaceWith(result.template);
            var selectedDeliveryValue = $('body').find('input[name="selected-delivery-value"]').val();
            $('body').find('input[name="delivery"][checked] + label .option-price').text(selectedDeliveryValue);
            application.loadImages();
            application.uiCheckLabels();
        });
    },
    setPayment: function (e) {
        $(e.target).parent().siblings().find('input').removeClass('chosen');
        $(e.target).addClass('chosen');
        var paymentId = $(e.target).val();
        $('#DeliveryMethodsAndPayment input[name="id"]').val(paymentId);
        var data = $('#DeliveryMethodsAndPayment').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                var selectedDeliveryValue = $('body').find('input[name="selected-delivery-value"]').val();
                $('body').find('input[name="delivery"][checked] + label .option-price').text(selectedDeliveryValue);
                application.loadImages();
                application.uiCheckLabels();
            }
        });
    },
    addNote: function (e) {
        var container = $('body');
        var note = $('textarea[name="note"]').val();
        var data = {
            __template: 'partials/cart/cart-template.html',
            __csrf: __CSRF,
            __action: 'Order/NoteAdd',
            note: note
        };
        $.post(null, data, function (result) {
            if (!result.action.Result) {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    toggleInvoice: function (e) {
        if (!$('input[name="invoice"]').hasClass('company')) {
            var container = $('body');
            if ($('input[name="invoice"]').prop('checked')) {
                var invoice = true;
            }
            else {
                var invoice = '';
            }
            var data = {
                __template: 'partials/cart/cart-template.html',
                __csrf: __CSRF,
                __action: 'Order/InvoiceChange',
                invoice: invoice
            };
            $.post(null, data, function (result) {
                if (!result.action.Result) {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            var length = result.action.Validation.length;
                            var errors = '';
                            for (var i = 0; i < length; i++) {
                                errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                            }
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                            application.createMessage(errorMessage, length * 1000);
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                            application.createMessage(errorMessage);
                        }
                    }
                }
            });
        }
    },
    saveInvoiceAddress: function (e) {
        var countryCode = $('.select-country.in-invoice').val();
        $(e.currentTarget).prev().prev().find('input[name="countryCode"]').val(countryCode);
        var data = $('#invoice-address-data').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    printOrder: function (e) {
        window.print();
    },
    placeOrder: function (e) {
        $.get('', { __collection: 'cart' }, function (res) {
            var quantity = res.collection.Products.length;
            var products = [];
            for (i = 0; i < quantity; i++) {
                var weight = res.collection.Products[i].Weight;
                var status = res.collection.Products[i].Status.Text;
                var product = [weight, status];
                products.push(product);
            }
            var container = $('body');
            var data = $('#order-summary-tos').serializeArray();
            $.post(null, data, function (result) {
                if (result.action.Result) {
                    var cart = result.collection;
                    $('.shopping-cart').replaceWith(result.template);
                    for (i = 0; i < quantity; i++) {
                        $('.thx .cart-items .cart-item').eq(i).find('.product-weight .attribute-value').prepend(products[i][0] + ' ');
                        $('.thx .cart-items .cart-item').eq(i).find('.product-availability .attribute-value').text(products[i][1]);
                    }
                    application.loadImages();
                    application.uiScrollToTop(container);
                    application.uiCheckLabels();
                    application.uiSetCartItemsHeight();
                } else {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            var length = result.action.Validation.length;
                            var errors = '';
                            for (var i = 0; i < length; i++) {
                                errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                            }
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                            application.createMessage(errorMessage, length * 4000);
                        } else if (result.action.Code == 1013) {
                            application.recalculate();
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                            application.createMessage(errorMessage);
                        }
                    }
                }
            });
        });
    },
    showMap: function (e) {
        var latitude = e.target.getAttribute('data-latitude');
        var longitude = e.target.getAttribute('data-longitude');
        var shopAddress = e.target.getAttribute('data-address');
        var shopName = e.target.getAttribute('data-company-name');
        var contentString = '<div id="content"><div id="siteNotice">'
            + '<p>' + shopName + '</p><p>' + shopAddress + '</p></div></div>';

        if ($(e.target).hasClass('show-on-the-map')) {
            var mapContainer = document.getElementById('map-container');
        } else {
            var mapNo = $(e.target).parent().next().find('input[name="mapNo"]').val();
            var mapId = 'map-container' + mapNo;
            var mapContainer = document.getElementById(mapId);
        }

        var mapOptions = {
            center: new google.maps.LatLng(latitude, longitude),
            zoom: 16,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoomControl: true,
            mapTypeControl: false,
            streetViewControl: false,
            rotateControl: false
        };
        var map = new google.maps.Map(mapContainer, mapOptions);
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(latitude, longitude),
            map: map,
            title: shopAddress,
            icon: 'https://static.comarch.com/files/img/google-marker.png'
        });
        var infoWindow = new google.maps.InfoWindow({
            content: contentString
        });

        marker.addListener('click', function () {
            infoWindow.open(map, marker);
        });

        infoWindow.open(map, marker);
    },
    openCloseLabelDropdown: function (e) {
        var dropdownElement = $(e.currentTarget).data('for');

        if (dropdownElement) {
            var labelArrow = $(e.currentTarget).find('> span');

            var windowWidth = window.innerWidth;
            if (windowWidth >= 768 && $('.client-profile').index() >= 0) {
                if ($('.' + dropdownElement).hasClass('hidden')) {
                    $('.customer-details').addClass('hidden');
                    $('.account-settings').addClass('hidden');
                    $('.orders-details').addClass('hidden');
                    $('.loyalty').addClass('hidden');
                    $('.complaints-details').addClass('hidden');
                    $('.wish-list').addClass('hidden');
                    $('.reviews-details').addClass('hidden');
                    $('.account-history').addClass('hidden');
                    $('.customer-details').prev().removeClass('active');
                    $('.account-settings').prev().removeClass('active');
                    $('.orders-details').prev().removeClass('active');
                    $('.loyalty').prev().removeClass('active');
                    $('.complaints-details').prev().removeClass('active');
                    $('.wish-list').prev().removeClass('active');
                    $('.reviews-details').prev().removeClass('active');
                    $('.account-history').prev().removeClass('active');
                    $('.' + dropdownElement).removeClass('hidden');
                    $('.' + dropdownElement).prev().addClass('active');
                    var height = $('.' + dropdownElement).height() + 60;
                    if (height < 486) {
                        height = 486;
                    }
                    if (dropdownElement == 'customer-details' || dropdownElement == 'account-settings') {
                        $('.' + dropdownElement).parent().parent().height(height);
                    }
                }
            } else {
                $('.' + dropdownElement).toggleClass('hidden');
                this.uiChangeArrow(labelArrow);
            }
            if (dropdownElement == 'customer-details') {
                application.uiEqualizeAddressSize();
            }
        }
    },
    incrementValue: function (e) {
        e.preventDefault();
        var fieldName = $(e.target).attr('data-field');
        var parent = $('body');
        var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').eq(0).val(), 10);

        if (currentVal < 99999) {
            if ($('input[name="stock-value"]').index() >= 0 && $('input[name="stock-quantity-control"]').val() == 'true' && !$(e.currentTarget).parent().hasClass('ask')) {
                var max = parseInt($('.stock-value .value').eq(0).text(), 10);
                if (!isNaN(currentVal)) {
                    if (currentVal < max) {
                        parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
                        if ($('input[name="last-good-quantity"]').index() >= 0) {
                            $('input[name="last-good-quantity"]').val(currentVal + 1)
                        }
                        if (currentVal + 1 == max) {
                            $('.quantity-field').next().addClass('max');
                        }
                        if ($(e.target).prev().prev().hasClass('min')) {
                            $('.quantity-field').prev().removeClass('min');
                        }
                    } else {
                        if (max == 0) {
                            parent.find('input[name=' + fieldName + ']').val(0);
                            if ($('input[name="last-good-quantity"]').index() >= 0) {
                                $('input[name="last-good-quantity"]').val(0)
                            }
                            $('.quantity-field').prev().addClass('min');
                        } else if (max == 1) {
                            parent.find('input[name=' + fieldName + ']').val(1);
                            if ($('input[name="last-good-quantity"]').index() >= 0) {
                                $('input[name="last-good-quantity"]').val(1)
                            }
                            $('.quantity-field').prev().addClass('min');
                        }
                    }
                } else {
                    parent.find('input[name=' + fieldName + ']').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                    $('.quantity-field').prev().addClass('min');
                }
            } else if ($('.complaints').index() >= 0) {
                var parent = $(e.currentTarget).parent();
                var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').eq(0).val(), 10);
                var max = parseInt(parent.find('.max-value .value').eq(0).text(), 10);
                if (!isNaN(currentVal)) {
                    if (currentVal < max) {
                        parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
                        if ($('input[name="last-good-quantity"]').index() >= 0) {
                            $('input[name="last-good-quantity"]').val(currentVal + 1)
                        }
                        if (currentVal + 1 == max) {
                            parent.find('.quantity-field').next().addClass('max');
                        }
                        if ($(e.target).prev().prev().hasClass('min')) {
                            parent.find('.quantity-field').prev().removeClass('min');
                        }
                    }
                } else {
                    parent.find('input[name=' + fieldName + ']').val(max);
                    parent.find('.quantity-field').prev().removeClass('min');
                    parent.find('.quantity-field').next().addClass('max');
                }
            } else if ($(e.currentTarget).parent().hasClass('ask') || $('.start').index() >= 0) {
                var parent = $(e.currentTarget).parent();
                var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').eq(0).val(), 10);
                if (!isNaN(currentVal)) {
                    parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(currentVal + 1)
                    }
                    $(e.currentTarget).prev().prev().removeClass('min');
                } else {
                    parent.find('input[name=' + fieldName + ']').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                    $(e.currentTarget).prev().prev().addClass('min');
                }
            } else {
                if (!isNaN(currentVal)) {
                    parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(currentVal + 1)
                    }
                    $('.quantity-field').prev().removeClass('min');
                } else {
                    parent.find('input[name=' + fieldName + ']').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                    $('.quantity-field').prev().addClass('min');
                }
            }
        }
    },
    decrementValue: function (e) {
        e.preventDefault();
        var fieldName = $(e.target).attr('data-field');
        if ($('.complaints').index() >= 0 || $(e.currentTarget).parent().hasClass('ask') || $('.start').index() >= 0) {
            var parent = $(e.target).parent();
            var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').eq(0).val(), 10);
            if (!isNaN(currentVal) && currentVal > 1) {
                parent.find('input[name=' + fieldName + ']').val(currentVal - 1);
                if ($('input[name="last-good-quantity"]').index() >= 0) {
                    $('input[name="last-good-quantity"]').val(currentVal - 1)
                }
                if ($(e.target).next().next().hasClass('max')) {
                    $(e.target).next().next().removeClass('max');
                }
                if (currentVal == 2) {
                    $(e.target).addClass('min');
                }
            }
        } else {
            var parent = $('body');
            var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').eq(0).val(), 10);
            if (!isNaN(currentVal) && currentVal > 1) {
                parent.find('input[name=' + fieldName + ']').val(currentVal - 1);
                if ($('input[name="last-good-quantity"]').index() >= 0) {
                    $('input[name="last-good-quantity"]').val(currentVal - 1)
                }
                if ($(e.target).next().next().hasClass('max')) {
                    $('.quantity-field').next().removeClass('max');
                }
                if (currentVal == 2) {
                    $('.quantity-field').prev().addClass('min');
                }
            } else {
                if ($('input[name="stock-value"]').index() >= 0 && $('input[name="stock-quantity-control"]').val() == 'true') {
                    var max = parseInt($('.stock-value .value').eq(0).text(), 10);
                    if (max == 0) {
                        parent.find('input[name=' + fieldName + ']').val(0);
                        if ($('input[name="last-good-quantity"]').index() >= 0) {
                            $('input[name="last-good-quantity"]').val(0)
                        }
                    } else {
                        parent.find('input[name=' + fieldName + ']').val(1);
                        if ($('input[name="last-good-quantity"]').index() >= 0) {
                            $('input[name="last-good-quantity"]').val(1)
                        }
                    }
                } else {
                    parent.find('input[name=' + fieldName + ']').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                }
                $('.quantity-field').prev().addClass('min');
            }
        }
    },
    validateQuantity: function (e) {
        if ($('input[name="stock-value"]').index() >= 0 && !$(e.currentTarget).parent().hasClass('ask')) {
            if ($('input[name="stock-quantity-control"]').val() == 'true') {
                var max = parseInt($('.stock-value .value').eq(0).text(), 10);
                if ($(e.currentTarget).val() >= max) {
                    $('.quantity-field').val(max);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(max)
                    }
                    $('.quantity-field').next().addClass('max');
                    if (max == 0 || max == 1) {
                        $('.quantity-field').prev().addClass('min');
                    } else {
                        $('.quantity-field').prev().removeClass('min');
                    }
                } else if ($(e.currentTarget).val() == '' || $(e.currentTarget).val() == 0 || $(e.currentTarget).val() == 1) {
                    $('.quantity-field').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                    $('.quantity-field').prev().addClass('min');
                    if (max == 1) {
                        $('.quantity-field').next().addClass('max');
                    } else {
                        $('.quantity-field').next().removeClass('max');
                    }
                } else {
                    $('.quantity-field').val($(e.currentTarget).val());
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val($(e.currentTarget).val())
                    }
                    $('.quantity-field').next().removeClass('max');
                    if (max == 0 || max == 1) {
                        $('.quantity-field').prev().addClass('min');
                    } else {
                        $('.quantity-field').prev().removeClass('min');
                    }
                }
            } else {
                if ($(e.currentTarget).val() < 1) {
                    $('.quantity-field').val(1);
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val(1)
                    }
                    $('.quantity-field').prev().addClass('min');
                    $('.quantity-field').next().removeClass('max');
                } else {
                    $('.quantity-field').val($(e.currentTarget).val());
                    if ($('input[name="last-good-quantity"]').index() >= 0) {
                        $('input[name="last-good-quantity"]').val($(e.currentTarget).val())
                    }
                    $('.quantity-field').prev().removeClass('min');
                    $('.quantity-field').next().removeClass('max');
                }
            }
        } else if ($('.complaints').index() >= 0) {
            var max = parseInt($(e.currentTarget).parent().find('.max-value .value').eq(0).text(), 10);
            if ($(e.currentTarget).val() >= max || $(e.currentTarget).val() == '' || $(e.currentTarget).val() < 2) {
                $(e.currentTarget).val(max);
                if ($('input[name="last-good-quantity"]').index() >= 0) {
                    $('input[name="last-good-quantity"]').val(max)
                }
                $(e.currentTarget).next().addClass('max');
                $(e.currentTarget).prev().removeClass('min');
            } else {
                $(e.currentTarget).next().removeClass('max');
                $(e.currentTarget).prev().removeClass('min');
            }
        } else if ($(e.currentTarget).parent().hasClass('ask')) {
            if ($(e.currentTarget).val() < 1) {
                $(e.currentTarget).val(1);
                if ($('input[name="last-good-quantity"]').index() >= 0) {
                    $('input[name="last-good-quantity"]').val(1)
                }
                $(e.currentTarget).prev().addClass('min');
                $(e.currentTarget).next().removeClass('max');
            } else {
                $(e.currentTarget).prev().removeClass('min');
                $(e.currentTarget).next().removeClass('max');
            }
        } else {
            if ($('input[name="last-good-quantity"]').index() >= 0) {
                $('input[name="last-good-quantity"]').val($(e.currentTarget).val())
            }
        }
    },
    validateQuantitySymbols: function (e) {
        var quantity = $(e.currentTarget).val();
        if (quantity == 0 || quantity > 99999 || quantity == '' || quantity.indexOf('-') !== -1 || quantity.indexOf('+') !== -1 || quantity.indexOf('.') !== -1 || quantity.indexOf(',') !== -1) {
            $(e.currentTarget).val($('input[name="last-good-quantity"]').val());
        }
    },
    enableDisableButton: function (e) {
        e.preventDefault();
        var buttonGroup = $(e.target).closest('div').find('.button-option');

        $.each(buttonGroup, function (index, value) {
            $(value).removeClass('active');
        });

        $(e.target).addClass('active');
    },
    changeValues: function (e) {
        e.preventDefault();

        if ($(e.currentTarget).hasClass('unitId')) {
            // recalculate and reload prices
            var ratio = Number($(e.currentTarget).next().attr('data-ratio').replace(',', '.'));
            var basicRatio = Number($('input[name="basic-unit-ratio"]').val().replace(',', '.'));
            var unit = $(e.currentTarget).val();
            var price = $(e.currentTarget).parent().find('input[name="initial-price"]').val();
            price = Number(price.replace(',', '.')) / basicRatio * ratio;
            price = price.toPrice();
            if ($('.product-popup .old-price').index() >= 0) {
                var oldPrice = $(e.currentTarget).parent().find('input[name="initial-old-price"]').val();
                oldPrice = Number(oldPrice.replace(',', '.')) / basicRatio * ratio;
                oldPrice = oldPrice.toPrice();
                if (window.innerWidth > 480) {
                    $(e.currentTarget).parent().parent().parent().find('.red-price .value').text(price);
                    $(e.currentTarget).parent().parent().parent().find('.old-price .value').text(oldPrice);
                    $(e.currentTarget).parent().parent().parent().find('.red-price .unit').text(unit);
                    $(e.currentTarget).parent().parent().parent().find('.old-price .unit').text(unit);
                }

            } else if ($('.product-popup .price').index() >= 0) {
                $(e.currentTarget).parent().parent().parent().find('.price .value').text(price);
                $(e.currentTarget).parent().parent().parent().find('.price .unit').text(unit);
            }

            // if($('.product-popup .unit-price').index() >= 0){
            // 	$(e.currentTarget).parent().parent().parent().find('.unit-price .value').text(price);
            // }

            // recalculate stock lvl
            if ($('#supplyId').index() >= 0) {
                var control = $('#supplyId').attr('data-stock-control');
            } else {
                var control = $('input[name="stock-control"]').val();
            }
            if (control == "true") {
                if ($('#supplyId').index() >= 0) {
                    var value = $('#supplyId').attr('data-stock-value');
                } else {
                    var value = $('input[name="stock-value"]').val();
                }
                var unitRatio = Number($('.unitId.active').next().attr('data-ratio').replace(',', '.'));
                var calculatedVal = value / unitRatio * basicRatio;
                calculatedVal = Math.floor(calculatedVal);

                $('.stock-value .value').text(calculatedVal);
                $('.stock-value .unit-name').text(unit);
                $('.quantity-field').attr('max', calculatedVal);
                $('.quantity-field').trigger('change');
            }
        } else if ($(e.currentTarget).hasClass('attributeId')) {
            // do nothing
        } else {
            // phantoms
            if ($('#supplyId').attr('data-clip') == 1) {
                // reload images in slider

                if ($('#supplyId').attr('data-imageId') != -1) {

                    var supplyImageId = $('#supplyId').attr('data-imageId');
                    var oldImageId = $('.sample').attr('data-oldImageId');

                    if (supplyImageId != oldImageId) {
                        $('.sample').attr('data-oldImageId', supplyImageId);
                        if ($('.slider .product-image img').index() != -1) {
                            var imageData = $('.slider .product-image img');
                            var url = imageData.attr('src').split('/');
                        } else {
                            var imageData = $('.sample');
                            var url = imageData.attr('data-src').split('/');
                        }

                        $('.slider-images').slick('unslick');
                        $('.slider-images').html('');

                        var format = url[2].split('.')[1];
                        var images = $('#supplyId').attr('data-images').split(',');
                        if (images == '') {
                            images = $('input[name="parent-images"]').attr('data-parent-images').split(',');
                            images.splice(-1, 1);
                        }
                        var length = images.length;

                        if ($('#supplyId').val() == $('.sample').attr('data-productId')) {
                            $.get('', { __collection: 'product-details.Product.Images' }, function (res) {
                                var imagesObj = res.collection;
                                for (var i = 0; i < length; i++) {
                                    var src = url[0] + '/' + images[i] + '/.' + format;
                                    $('.slider-images').append('<figure class="product-image"><img alt="' + imagesObj[i].Alt + '" src="' + src + '"></figure>');
                                }
                                application.slickSliderProductDetails();
                            });
                        } else {
                            var alt = imageData.attr('alt');
                            for (var i = 0; i < length; i++) {
                                var src = url[0] + '/' + images[i] + '/.' + format;
                                $('.slider-images').append('<figure class="product-image"><img alt="' + alt + '" src="' + src + '"></figure>');
                            }
                            application.slickSliderProductDetails();
                        }
                    }
                } else {
                    $('.slider-images').html('');
                    $('.slider-images').append('<figure class="product-image"><i class="fa fa-picture-o" aria-hidden="true"></i></figure>');
                    $('.sample').attr('data-oldImageId', -1);
                }

                // reload prices
                if ($('.desktop-price').hasClass('prices')) {
                    if ($('.unitId.active').index() >= 0) {
                        var ratio = Number($('.unitId.active').next().attr('data-ratio').replace(',', '.'));
                    } else {
                        var ratio = Number($('input[name="basic-ratio"]').val().replace(',', '.'));
                    }
                    if ($('.unit-price').index() >= 0) {
                        var unitRatio = Number($('input[name="unit-price-ratio"]').val().replace(',', '.'));
                    }
                }

                var ask = $('#supplyId').attr('data-ask-for-price');
                var login = $('input[name="login-to-see-prices"]').val();

                if (ask == "false" && login == "false" && $('#supplyId').attr('data-price') != '') {
                    var price = Number($('#supplyId').attr('data-price').replace(',', '.')) * ratio;
                    var oldPrice = Number($('#supplyId').attr('data-old-price').replace(',', '.')) * ratio;
                    var unitPrice = Number($('#supplyId').attr('data-price').replace(',', '.')) * unitRatio;

                    // toPrice filter in JS
                    Number.prototype.format = function (n, x, s, c) {
                        var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
                            num = this.toFixed(Math.max(0, ~~n));
                        return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
                    };

                    Number.prototype.toPrice = function () {
                        return this.format(__decPlaces, 3, __decThoSep, __decSep);
                    }

                    price = price.toPrice();
                    oldPrice = oldPrice.toPrice();
                    unitPrice = unitPrice.toPrice();

                    if (oldPrice <= price && $('.red-price').index() >= 0) {
                        $('.red-price').addClass('price').removeClass('red-price');
                        $('.old-price').addClass('hidden');
                    }
                    if (oldPrice > price && $('.red-price').index() == -1) {
                        $('.price').addClass('red-price').removeClass('price');
                        $('.old-price').removeClass('hidden');
                    }

                    $('.price .value').text(price);
                    $('.red-price .value').text(price);
                    $('.old-price .value').text(oldPrice);
                    $('.unit-price .value').text(unitPrice);
                    if ($('input[name="initial-price"]').index() >= 0) {
                        $('input[name="initial-price"]').val(price);
                        $('input[name="initial-old-price"]').val(oldPrice);
                    }

                    var points = $('#supplyId').attr('data-points');
                    var pointsPrice = $('#supplyId').attr('data-points-price');

                    if (points != null) {
                        $('.loyalty-info .amount').text(points);
                        $('.loyalty-info').removeClass('hidden');
                    } else {
                        $('.loyalty-info').addClass('hidden');
                    }

                    if (pointsPrice != null) {
                        $('.points-price .amount').text(pointsPrice);
                        $('.points-price').removeClass('hidden');
                    } else {
                        $('.points-price').addClass('hidden');
                    }

                }

                // reload availability
                var date = $('#supplyId').attr('data-availability-date');
                var imgUrl = $('#supplyId').attr('data-availability-img-url');
                var status = $('#supplyId').attr('data-availability-status');
                var text = $('#supplyId').attr('data-availability-text');
                var type = $('#supplyId').attr('data-availability-type');

                $('.availability-container .value').text(text);
                if (date == null) {
                    $('.availability-container .date').addClass('hidden');
                } else {
                    $('.availability-container .date').removeClass('hidden');
                    $('.availability-container .date').text(date);
                }
                $('.availability-container img').attr('src', imgUrl);
                $('.availability-container img').attr('alt', text);

                if (type == 2 || text == '') {
                    $('.availability-container .value').addClass('hidden');
                } else {
                    $('.availability-container .value').removeClass('hidden');
                }
                if (type == 3 || imgUrl == '') {
                    $('.availability-container img').addClass('hidden');
                } else {
                    $('.availability-container img').removeClass('hidden');
                }
                if (status == 3) {
                    $('.add-to-cart').parent().addClass('hidden');
                    $('.open-popup-notify-about-availability').parent().removeClass('hidden');
                } else {
                    $('.add-to-cart').parent().removeClass('hidden')
                    $('.open-popup-notify-about-availability').parent().addClass('hidden');
                }

                // reload product attributes
                var ul = $('.product-attributes ul');
                var translations = ul.prev();

                var id = $('#supplyId').attr('data-id');
                var code = $('#supplyId').attr('data-code');
                var vat = $('#supplyId').attr('data-vat');
                var weight = $('#supplyId').attr('data-weight');
                var symbol = $('#supplyId').attr('data-symbol');
                var upc = $('#supplyId').attr('data-upc');

                var vatTrans = translations.data('vat');
                var weightTrans = translations.data('weight');
                var symbolTrans = translations.data('symbol');
                var upcTrans = translations.data('upc');

                $('.id-value').text(id);
                $('.code-value').text(code);

                if (vat == null && ul.find('.vat-value').index() >= 0) {
                    $('.vat-value').parent().remove();
                } else if (vat != null) {
                    if (ul.find('.vat-value').index() >= 0) {
                        $('.vat-value').text(vat + '%');
                    } else {
                        ul.append('<li><span>' + vatTrans + '</span> <span class="vat-value">' + vat + '%</span></li>');
                    }
                }

                if (weight == '' && ul.find('.weight-value').index() >= 0) {
                    $('.weight-value').parent().remove();
                } else if (weight != '') {
                    if (ul.find('.weight-value').index() >= 0) {
                        $('.weight-value').text(weight + 'kg');
                    } else {
                        ul.append('<li><span>' + weightTrans + '</span> <span class="weight-value">' + weight + 'kg</span></li>');
                    }
                }

                if (symbol == '' && ul.find('.symbol-value').index() >= 0) {
                    $('.symbol-value').parent().remove();
                } else if (symbol != '') {
                    if (ul.find('.symbol-value').index() >= 0) {
                        $('.symbol-value').text(symbol);
                    } else {
                        ul.append('<li><span>' + symbolTrans + '</span> <span class="symbol-value">' + symbol + '</span></li>');
                    }
                }

                if (upc == '' && ul.find('.upc-value').index() >= 0) {
                    $('.upc-value').parent().remove();
                } else if (upc != '') {
                    if (ul.find('.upc-value').index() >= 0) {
                        $('.upc-value').text(upc);
                    } else {
                        ul.append('<li><span>' + upcTrans + '</span> <span class="upc-value">' + upc + '</span></li>');
                    }
                }
            }
            // batch
            else if ($('#supplyId').attr('data-batch') == 1) {
                var price = $('input[name="price"]').val();
                var status = $('input[name="status"]').val();
                var ask = $('input[name="ask-for-price"]').val();
            }

            // clip(phantoms) and batch
            // stock level
            var control = $('#supplyId').attr('data-stock-control');
            var imgUrl = $('#supplyId').attr('data-stock-img-url');
            var qControl = $('#supplyId').attr('data-stock-quantity-control');
            var text = $('#supplyId').attr('data-stock-text');
            var type = $('#supplyId').attr('data-stock-type');
            var value = $('#supplyId').attr('data-stock-value');
            if ($('input[name="basic-unit-ratio"]').index() >= 0) {
                var basicRatio = Number($('input[name="basic-unit-ratio"]').val().replace(',', '.'));
            } else {
                var basicRatio = 1;
            }
            if ($('.unitId.active').index() >= 0) {
                var unitRatio = Number($('.unitId.active').next().attr('data-ratio').replace(',', '.'));
            } else {
                var unitRatio = 1;
            }
            var calculatedVal = value / unitRatio * basicRatio;
            calculatedVal = Math.floor(calculatedVal);

            $('.stock-container .text').text(text);
            $('.stock-container img').attr('src', imgUrl);
            $('.stock-container img').attr('alt', text);
            $('.stock-value .value').text(calculatedVal);
            $('.quantity-field').attr('max', calculatedVal);
            $('.quantity-field').trigger('change');


            if (control == "false") {
                $('.stock-container').addClass('hidden');
                $('.stock-value').addClass('hidden');
            } else {
                $('.stock-container').removeClass('hidden');
                $('.stock-value').removeClass('hidden');
                if (type == 2 || text == '') {
                    $('.stock-container .text').addClass('hidden');
                } else {
                    $('.stock-container .text').removeClass('hidden');
                }
                if (type == 3 || imgUrl == '') {
                    $('.stock-container img').addClass('hidden');
                } else {
                    $('.stock-container img').removeClass('hidden');
                }
                if ((qControl == true && value == 0) || status == 3) {
                    $('.add-to-cart').parent().addClass('hidden');
                    $('.open-popup-notify-about-availability').parent().removeClass('hidden');
                } else {
                    $('.add-to-cart').parent().removeClass('hidden')
                    $('.open-popup-notify-about-availability').parent().addClass('hidden');
                }
            }

            // reload "ask for price"
            if (ask == "true") {
                $('.desktop-price').addClass('hidden');
                $('.add-to-cart').parent().addClass('hidden')
                $('.open-popup-ask-for-price').parent().removeClass('hidden');
            } else {
                $('.desktop-price').removeClass('hidden');
                $('.open-popup-ask-for-price').parent().addClass('hidden');
            }

            // reload "login-to-see-prices"
            var hide = $('input[name="login-to-see-prices"]').val();
            if (hide == "true") {
                $('.add-to-cart').parent().addClass('hidden');
                $('.desktop-price').addClass('hidden');
                $('.open-popup-ask-for-price').parent().addClass('hidden');
                $('.open-popup-notify-about-availability').parent().addClass('hidden');
            }
        }
    },
    updateCustomerData: function (e) {
        var country = $('#change-customer-data').find('.select-country').val();
        $('#change-customer-data').find('input[name="countryCode"]').val(country);
        var data = $('#change-customer-data').serializeArray();
        if ($('.address.active .content div:first-child').text() != '') {
            $.post(null, data, function (result) {
                if (result.action.Result) {
                    message = '<div class="title">' + result.action.Message + '</div>';
                    application.createMessage(message);
                    application.hidePopup(e);
                    application.uiPreventScrolling();
                    window.location.reload();
                } else {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            var length = result.action.Validation.length;
                            var errors = '';
                            for (var i = 0; i < length; i++) {
                                errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                            }
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                            application.createMessage(errorMessage, length * 1000);
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                            application.createMessage(errorMessage);
                        }
                    }
                }
            });
        } else {
            var ajaxCall1 = $.post(null, data);
            var counter = $('.address.active .delivery-address-update-button').data('counter');
            $('#delivery-address-update-' + counter).find('input[name="countryCode"]').val(country);
            var inputs = $('#delivery-address-update-' + counter + ' input:not([type="hidden"])');
            inputs.each(function () {
                if ($(this).val() == '') {
                    var name = $(this).attr('name');
                    var value = $('#change-customer-data').find('input[name="' + name + '"]').val();
                    $(this).val(value);
                }
            });
            var data = $('#delivery-address-update-' + counter).serializeArray();
            var ajaxCall2 = $.post(null, data);
            $.when(ajaxCall1, ajaxCall2).done(function (result1, result2) {
                if (result1[0].action.Result) {
                    message = result1[0].action.Message;
                    application.createMessage(message);
                    window.location.reload();
                } else {
                    if (result1[0].action.Validation != null) {
                        var length = result1[0].action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result1[0].action.Validation[i].Error + '</p>';
                        }
                        errorMessage = result1[0].action.Message + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = result1[0].action.Message;
                        application.createMessage(errorMessage);
                    }
                }
            });
        }
    },
    updateCustomerEmail: function (e) {
        var data = $('#change-customer-email').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                message = '<div class="title">' + result.action.Message + '</div>';
                application.createMessage(message);
                application.hidePopup(e);
                application.uiPreventScrolling();
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    updateCustomerPassword: function (e) {
        var data = $('#change-customer-password').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                message = '<div class="title">' + result.action.Message + '</div>';
                application.createMessage(message);
                application.hidePopup(e);
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    addToComparer: function (e) {
        var data = $('#comparison-tool-add-form').serializeArray();
        var url = data[3].value;
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.replace(url);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    deleteFromComparer: function (e) {
        var data = $(e.target).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.reload();
                if ($('.forms-container').children().eq(1).index() == -1) {
                    window.location.replace('');
                }
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    setTab: function (e) {
        var div = $(e.currentTarget);
        var divName = div.attr('data-for');
        if (div.hasClass('redirect')) {
            var tab = 'executed';
            var template = 'partials/customer/orders-details.html';
            var divToShow = div.next();
            sessionStorage.removeItem('ordersPageQuery');
            sessionStorage.removeItem('ordersPageNumber');
        } else if (divName == 'account-history') {
            var tab = divName;
            var template = 'partials/customer/account-history.html';
            var divToShow = div.next();
        } else if (divName == 'orders-details') {
            var tab = 'under-execution';
            var template = 'partials/customer/orders-details.html';
            var divToShow = div.next();
            sessionStorage.removeItem('ordersPageQuery');
            sessionStorage.removeItem('ordersPageNumber');
        } else if (divName == 'loyalty') {
            var tab = 'loyalty';
            var template = 'partials/customer/loyalty.html';
            var divToShow = div.next();
            sessionStorage.removeItem('loyaltyPageQuery');
            sessionStorage.removeItem('loyaltyPageNumber');
        } else if (divName == 'complaints-details') {
            var tab = 'complaint-ue';
            var template = 'partials/customer/complaints-details.html';
            var divToShow = div.next();
        } else if (divName == 'wish-list') {
            var tab = 'wish-list';
            var template = 'partials/customer/wish-list.html';
            var divToShow = div.next();
        } else if (divName == 'reviews-details') {
            var tab = 'to-comment';
            var template = 'partials/customer/reviews-details.html';
            var divToShow = div.next();
        } else {
            if (div.hasClass('executed-tab')) {
                var tab = 'executed';
                var template = 'partials/customer/orders-details.html';
                var divToShow = div.parent();
                sessionStorage.removeItem('ordersPageQuery');
                sessionStorage.removeItem('ordersPageNumber');
            } else if (div.hasClass('under-execution-tab')) {
                var tab = 'under-execution';
                var template = 'partials/customer/orders-details.html';
                var divToShow = div.parent();
                sessionStorage.removeItem('ordersPageQuery');
                sessionStorage.removeItem('ordersPageNumber');
            } else if (div.hasClass('complaint-e-tab')) {
                var tab = 'complaint-e';
                var template = 'partials/customer/complaints-details.html';
                var divToShow = div.parent().parent();
            } else if (div.hasClass('complaint-ue-tab')) {
                var tab = 'complaint-ue';
                var template = 'partials/customer/complaints-details.html';
                var divToShow = div.parent().parent();
            } else if (div.hasClass('to-comment-tab')) {
                var tab = 'to-comment';
                var template = 'partials/customer/reviews-details.html';
                var divToShow = div.parent().parent();
            } else if (div.hasClass('commented-tab')) {
                var tab = 'commented';
                var template = 'partials/customer/reviews-details.html';
                var divToShow = div.parent().parent();
            }
        }
        if (divToShow.hasClass('hidden') || div.hasClass('inactive-tab') || div.hasClass('redirect')) {
            div.removeClass('redirect');
            $.get(null, { tab: tab, __template: template }, function (result) {
                divToShow.replaceWith(result.template);
                application.loadImages();

                var windowWidth = window.innerWidth;
                if (windowWidth >= 768) {
                    var cl = divToShow.attr('class');
                    var divToShowNew = $('body').find('.' + cl);
                    var height = divToShowNew.height() + 100;
                    if (cl == 'account-history') {
                        divToShowNew.parent().height(height);
                    } else {
                        divToShowNew.parent().parent().height(height);
                    }
                }

            });
        }
    },
    searchInCollectionPoints: function (e) {
        $('#CollectionPoints .collection-point-container').removeClass('hidden');
        $('#CollectionPoints .collection-point-container').removeClass('no-border');
        $.get('', { __collection: 'cart.SelectedDelivery.CollectionPoints' }, function (res) {
            var phrase = $(e.currentTarget).prev().val().toLowerCase();
            var collectionPoints = res.collection;
            var cpCount = collectionPoints.length;
            for (i = 0; i < cpCount; i++) {
                var data = collectionPoints[i].Address;
                data = data.toLowerCase();
                var id = collectionPoints[i].Id;
                if (data.indexOf(phrase) == -1) {
                    $(e.currentTarget).parent().next().find('.collection-point-container').eq(i).addClass('hidden');
                } else {
                    var j = i;
                }
            }
            $(e.currentTarget).parent().next().find('.collection-point-container').eq(j).addClass('no-border');
        });
    },
    showOrderDetailsPopup: function (e) {
        var divToReplace = $(e.currentTarget).parent();
        var data = $(e.currentTarget).find('> form').serializeArray();
        var pageQuery = sessionStorage.getItem('ordersPageQuery');
        var pageNumber = sessionStorage.getItem('ordersPageNumber');
        if (pageQuery != null) {
            data.push({ name: pageQuery, value: pageNumber });
        }
        $.get(null, data, function (result) {
            divToReplace.replaceWith(result.template);
            var orderIndex = $(e.currentTarget).index();
            var quantity = result.collection.Order.Products.length;
            var urlArr = [];
            for (i = 0; i < quantity; i++) {
                var urlItem = [];
                var url = result.collection.Order.Products[i].Url;
                var urlIndex = i;
                urlItem.push(url, urlIndex);
                urlArr.push(urlItem);
            }
            $.each(urlArr, function () {
                var url = $(this)[0];
                var index = $(this)[1];
                $.get(url, { index: index, __collection: 'product-details|page' }, function (res) {
                    var orderIndex = $(e.currentTarget).index();
                    var index = parseInt(res.collection['page'].GET.index);
                    var weight = res.collection['product-details'].Product.Weight;
                    var status = res.collection['product-details'].Product.Availability.Text;
                    if (divToReplace.hasClass('executed')) {
                        $('.executed .order').eq(orderIndex).find('.cart-item').eq(index).find('.product-weight .attribute-value').prepend(weight + ' ');
                        $('.executed .order').eq(orderIndex).find('.cart-item').eq(index).find('.product-availability .attribute-value').text(status);
                    } else {
                        $('.under-execution .order').eq(orderIndex).find('.cart-item').eq(index).find('.product-weight .attribute-value').prepend(weight + ' ');
                        $('.under-execution .order').eq(orderIndex).find('.cart-item').eq(index).find('.product-availability .attribute-value').text(status);
                    }
                });
            });
            application.loadImages();
            application.uiPreventScrolling();
            if (divToReplace.hasClass('executed')) {
                $('.executed .order').eq(orderIndex).find('> .popup-dialog:not(.set-elements)').removeClass('hidden');
            } else {
                $('.under-execution .order').eq(orderIndex).find('.popup-dialog:not(.set-elements)').removeClass('hidden');
            }
            application.uiSetCartItemsHeight();
        });
    },
    changePage: function (e) {
        if ($('section.product-details').index() == -1) {
            var divToReplace = $(e.currentTarget).parent().parent().parent();
            var data = $(e.currentTarget).find('form').serializeArray();

            if (divToReplace.hasClass('executed') || divToReplace.hasClass('under-execution') || divToReplace.hasClass('loyalty-history')) {
                var pageInput = $(e.currentTarget).find('form input').eq(0);
                var pageQuery = pageInput.attr('name');
                var pageNumber = pageInput.attr('value');
                if (divToReplace.hasClass('loyalty-history')) {
                    sessionStorage.setItem('loyaltyPageQuery', pageQuery);
                    sessionStorage.setItem('loyaltyPageNumber', pageNumber);
                } else {
                    sessionStorage.setItem('ordersPageQuery', pageQuery);
                    sessionStorage.setItem('ordersPageNumber', pageNumber);
                }
            }

            $.get(null, data, function (result) {
                if (divToReplace.hasClass('account-history')) {
                    var classToFind = '.account-history';
                } else if (divToReplace.hasClass('executed')) {
                    var classToFind = '.executed';
                } else if (divToReplace.hasClass('under-execution')) {
                    var classToFind = '.under-execution';
                } else if (divToReplace.hasClass('loyalty-history')) {
                    var classToFind = '.loyalty-history';
                } else if (divToReplace.hasClass('complaint-e')) {
                    var classToFind = '.complaint-e';
                } else if (divToReplace.hasClass('complaint-ue')) {
                    var classToFind = '.complaint-ue';
                } else if (divToReplace.hasClass('to-comment')) {
                    var classToFind = '.to-comment';
                } else if (divToReplace.hasClass('commented')) {
                    var classToFind = '.commented';
                }
                divToReplace.replaceWith(result.template);
                application.loadImages();

                if (window.innerWidth <= 768) {
                    if (classToFind == '.account-history') {
                        $('html,body').animate({ scrollTop: $(classToFind).prev().offset().top }, 'slow');
                    } else if (classToFind == '.executed') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().prev().offset().top }, 'slow');
                    } else if (classToFind == '.under-execution') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().prev().offset().top }, 'slow');
                    } else if (classToFind == '.loyalty-history') {
                        $('html,body').animate({ scrollTop: $(classToFind).parent().parent().offset().top }, 'slow');
                    } else if (classToFind == '.complaint-e') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().parent().prev().offset().top }, 'slow');
                    } else if (classToFind == '.complaint-ue') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().parent().prev().offset().top }, 'slow');
                    } else if (classToFind == '.to-comment') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().parent().prev().offset().top }, 'slow');
                    } else if (classToFind == '.commented') {
                        $('html,body').animate({ scrollTop: $(classToFind).eq(0).parent().parent().prev().offset().top }, 'slow');
                    }
                } else {
                    $('html,body').animate({ scrollTop: $('#main-section').offset().top - 81 }, 'slow');
                }
            });
        }
    },
    setFixedHorizontal: function () {
        var comparerSectionWidth = $('section.comparer').width();
        var mainSectionWidth = $('#main-section').width();
        var windowWidth = window.innerWidth;
        var a = $('#header-section').outerHeight();
        var b = $('.comparer .page-header').outerHeight();
        var c = $('.comparer .forms-container').outerHeight();
        var d = $('.comparer .products .product-img');
        var len = d.length;
        var imgHeight = 0;
        for (i = 0; i < len; i++) {
            var newH = d.eq(i).outerHeight();
            if (newH > imgHeight) {
                imgHeight = newH;
            }
        }
        d = imgHeight;
        if (windowWidth <= 768) {
            var totalHeight = a + b + c + d + 10;
        } else {
            var totalHeight = a + b + c + d + 10 + 20;
        }
        if (totalHeight < $(window).scrollTop()) {
            $('.comparer .fixed-header').removeClass('hidden');
            if (windowWidth <= 768) {
                $('.comparer .fixed-header').css({
                    'top': $(window).scrollTop()
                });
            } else {
                $('.comparer .fixed-header').css({
                    'top': $(window).scrollTop() - a - 20
                });
            }
        } else {
            $('.comparer .fixed-header').addClass('hidden');
        }
        if (comparerSectionWidth > mainSectionWidth) {
            $('#header-section').css({
                'position': 'relative',
                'left': $(window).scrollLeft()
            });
            $('#footer-section').css({
                'position': 'relative',
                'left': $(window).scrollLeft()
            });
            $('.comparer .attribute-name span').css({
                'position': 'relative',
                'left': $(window).scrollLeft()
            });
        }
    },
    setHeightsInComparer: function () {
        // Set product-attribute-value height
        $('.products-attribute-container').each(function () {
            var height = $(this).children().eq(0).height();
            $(this).children().each(function () {
                var newHeight = $(this).height();
                if (newHeight > height) {
                    height = newHeight;
                }
            });
            var oldHeight = $(this).children().eq(0).height();
            $(this).children().each(function () {
                $(this).css('height', height);
            });
        });

        // Fix products-attribute-container height in comparer
        $('.comparer .products-attribute-container').each(function () {
            var height = $(this).height();
            $(this).css('height', height);
        });
    },
    // sliderNext: function(e) {
    //   var images = $(e.currentTarget).prev().prev().find('img');
    //   var nav = $(e.currentTarget).next().find('span');
    //   var quantity = images.length;
    //   for (i=0; i<quantity; i++){
    //     if (images.eq(i).attr('src') != ""){
    //       var newImage = images.eq(i+1);
    //       var data = newImage.attr('data-src');
    //       var index = i+1;
    //     }
    //   }
    //   newImage.attr('src', data);
    //   newImage.siblings().attr('src', '');
    //   application.uiSliderHideAlt();
    //   application.uiToggleArrows(index, quantity);
    // },
    // sliderPrev: function(e) {
    //   var images = $(e.currentTarget).prev().find('img');
    //   var nav = $(e.currentTarget).next().next().find('span');
    //   var quantity = images.length;
    //   for (i=0; i<quantity; i++){
    //     if (images.eq(i).attr('src') != ""){
    //       var newImage = images.eq(i-1);
    //       var data = newImage.attr('data-src');
    //       var index = i-1;
    //     }
    //   }
    //   newImage.attr('src', data);
    //   newImage.siblings().attr('src', '');
    //   application.uiSliderHideAlt();
    //   application.uiToggleArrows(index, quantity);
    // },
    // sliderChoose: function(e) {
    //   var index = $(e.currentTarget).index();
    //   var images = $(e.currentTarget).parent().prev().prev().prev().children();
    //   var quantity = images.length - 1;
    //   var image = images.eq(index);
    //   var data = image.attr('data-src');
    //   image.attr('src', data);
    //   image.siblings().attr('src', '');
    //   application.uiSliderHideAlt();
    //   application.uiToggleArrows(index, quantity);
    // },
    setMessages: function () {

    },
    setCookie: function (cname, cvalueAll, cvalueNew, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        if (cvalueAll == "") {
            var cvalueAll = cvalueNew;
        } else {
            var cvalueAll = cvalueAll + ' | ' + cvalueNew;
        }

        document.cookie = cname + "=" + cvalueAll + "; " + expires + "; path=/";
    },
    getCookie: function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    },
    listPhrases: function () {
        var searchedPhrases = application.getCookie("searchedPhrases");
        var ul = $('#header-section .recently-searched ul');
        if (searchedPhrases != "") {
            ul.removeClass('empty');
            var arr = searchedPhrases.split(' | ');
            var length = arr.length;
            ul.html('');
            for (i = 0; i < length; i++) {
                ul.prepend('<li class="previous-phrase"><span>' + arr[i] + '</span><span class="fa fa-arrow-left"></span></li>');
            }
        }
    },
    addPhrase: function (e) {
        var searchedPhrases = application.getCookie("searchedPhrases");
        var newPhrase = $(e.currentTarget).prev().val();
        application.setCookie("searchedPhrases", searchedPhrases, newPhrase, 365);
    },
    searchPhrase: function (e) {
        var searchedPhrases = application.getCookie("searchedPhrases");
        var phrase = $(e.currentTarget).eq(0).text();
        application.setCookie("searchedPhrases", searchedPhrases, phrase, 365);
        var url = $(e.currentTarget).parent().prev().val();
        url = url + '?search=' + phrase;
        location.assign(url);
    },
    rememberChoice: function (e) {
        var parent = $(e.currentTarget).parent();
        if ($(e.currentTarget).hasClass('primary-action-button')) {
            parent = parent.parent();
        }
        if (parent.find('input[name="remember-choice"]').prop('checked')) {
            if ($(e.currentTarget).hasClass('primary-action-button')) {
                application.setCookie('rememberChoice', '', 'goToCart', 365);
            } else {
                application.setCookie('rememberChoice', '', 'stayOnPage', 365);
            }
        }
    },
    showUndercategories: function (e) {
        var index = $(e.currentTarget).parent().index();
        if ($(e.currentTarget).siblings().index() != -1) {
            $('.header-categories').addClass('undercategories-open');
            // $(e.currentTarget).addClass('active');
            $('.header-undercategories .undercategories').eq(index).removeClass('hidden');
        }
    },
    hideUndercategories: function (e) {
        var index = $(e.currentTarget).parent().index();
        if ($(e.currentTarget).siblings().index() != -1) {
            $('.header-undercategories .undercategories').eq(index).addClass('hidden');
            $('.header-categories').removeClass('undercategories-open');
        }
    },
    keepUndercategoriesVisible: function (e) {
        var index = $(e.currentTarget).index();
        $('.header-categories').addClass('undercategories-open');
        $('.header-categories .categories li').eq(index).find('a').addClass('active');
        $(e.currentTarget).removeClass('hidden');
    },
    dontKeepUndercategoriesVisible: function (e) {
        $(e.currentTarget).addClass('hidden');
        $('.header-categories a').removeClass('active');
        $('.header-categories').removeClass('undercategories-open');
    },
    slickSliderRelated: function () {
        $('.related-products').slick({
            infinite: false,
            slidesToShow: 5,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 1440,
                    settings: {
                        slidesToShow: 4,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    },
    slickSliderBanners: function () {
        var windowWidth = window.innerWidth;
        var width = (windowWidth + 17) / 5;
        var widthDesktop = windowWidth / 5;
        var string = width + 'px';
        var stringDesktop = widthDesktop + 'px';
        $('.banner:not(.solo)').slick({
            centerMode: true,
            centerPadding: stringDesktop,
            autoplay: true,
            autoplaySpeed: 5000,
            dots: true,
            slidesToShow: 1,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 768,
                    settings: {
                        centerPadding: string
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        centerMode: false,
                        arrows: false
                    }
                }
            ]
        });
        var settingsConfig = $('.banner:not(.solo)').data('slick');
        if (settingsConfig && settingsConfig.speed) {
            $('.banner:not(.solo) .img').css({ "transition-duration": settingsConfig.speed + "ms" });
        }
    },
    slickSliderManufacturers: function () {
        $('.manufacturers').slick({
            infinite: false,
            slidesToShow: 5,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 1440,
                    settings: {
                        slidesToShow: 4,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    },
    slickSliderProductDetails: function () {
        $('.slider-images').slick({
            infinite: false,
            slidesToShow: 1,
            slidesToScroll: 1,
            dots: true
        });
    },
    triggerSearchAdvanced: function (e) {
        e.preventDefault();
        if (window.innerWidth > 1024) {
            $('.search-section').addClass('adv-search-opened');
            $('#header-section .search-advanced-icon').eq(0).trigger('click');
        } else if ($('#header-section .search').hasClass('open') && $('#header-section .advanced-filters').hasClass('hidden')) {
            $('#header-section .search-advanced-icon').eq(0).trigger('click');
        } else if (!$('#header-section .search').hasClass('open')) {
            $('#header-section .search').trigger('click');
            $('#header-section .search-advanced-icon').eq(0).trigger('click');
        }
    },
    addDiscount: function () {
        var data = $('#DiscountCodeForm').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
                application.uiCheckLabels();
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    deleteDiscount: function () {
        var data = $('#coupon-delete').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.shopping-cart').replaceWith(result.template);
                application.loadImages();
                application.uiCheckLabels();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    changeReviewAuthorName: function (e) {
        var data = $('#change-review-author-name').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.review-author-name').text($('input[name="author"]').val());
                message = '<div class="title">' + result.action.Message + '</div>';
                application.createMessage(message);
                application.hidePopup(e);
                application.uiPreventScrolling();
                application.loadImages();
                application.uiCheckLabels();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    addReview: function () {
        var data = $('#add-review-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage(result.action.Description);
                $('input[name="author"]').val('');
                $('textarea[name="comment"]').val('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    addReviewInProfile: function (e) {
        var data = $(e.currentTarget).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage(result.action.Description);
                $('input[name="author"]').val('');
                $('textarea[name="comment"]').val('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    sendContactForm: function (e) {
        var data = $('#contact-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                if (window.innerWidth <= 768) {
                    application.hidePopup(e);
                } else {
                    application.uiPreventScrolling();
                }
                $('input[name="subject"]').val('');
                $('textarea[name="message"]').val('');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    acceptOrder: function (e) {
        var parent = $(e.currentTarget).parents('.under-execution');
        var data = $('#accept-order-form').serializeArray();
        var pageQuery = sessionStorage.getItem('ordersPageQuery');
        var pageNumber = sessionStorage.getItem('ordersPageNumber');
        if (pageQuery != null) {
            data.push({ name: pageQuery, value: pageNumber });
        }
        if (parent.index() != -1) {
            var url = location.href + '?tab=under-execution&' + pageQuery + '=' + pageNumber;
        } else {
            var url = null;
        }
        $.post(url, data, function (result) {
            if (result.action.Result) {
                if (parent.index() != -1) {
                    $('.under-execution').eq(1).replaceWith(result.template);
                }
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    restorePayment: function (e) {
        var data = $('#restore-payment-form').serializeArray();
        $.post('', data, function (result) {
            if (result.action.Result) {
                window.location.replace(result.action.Redirect302);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    cancelOrder: function (e) {
        var parent = $(e.currentTarget).parents('.under-execution');
        var data = $('#cancel-order-form').serializeArray();
        var message = $('#cancel-order-button').attr('data-info');
        var pageQuery = sessionStorage.getItem('ordersPageQuery');
        var pageNumber = sessionStorage.getItem('ordersPageNumber');
        if (pageQuery != null) {
            data.push({ name: pageQuery, value: pageNumber });
        }
        if (parent.index() != -1) {
            var url = location.href + '?tab=under-execution&' + pageQuery + '=' + pageNumber;
        } else {
            var url = null;
        }
        $.post(url, data, function (result) {
            if (result.action.Result) {
                application.createMessage(message);
                if (parent.index() != -1) {
                    $('.under-execution').eq(1).replaceWith(result.template);
                }
                application.uiPreventScrolling();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    showLanguages: function (e) {
        $('#change-language-form').removeClass('hidden');
        $('.currency').addClass('lang-active');
    },
    hideLanguages: function (e) {
        $(e.currentTarget).parent().parent().addClass('hidden');
        $('.currency').removeClass('lang-active');
    },
    changeLanguage: function (e) {
        $('input[name="languageId"]').val($(e.currentTarget).attr('data-id'));
        var data = $('#change-language-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.replace(result.action.Redirect302);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    showCurrency: function (e) {
        $(e.currentTarget).next().removeClass('hidden');
        $(e.currentTarget).parent().addClass('active');
    },
    hideCurrency: function (e) {
        $(e.currentTarget).parent().parent().addClass('hidden');
        $(e.currentTarget).parent().parent().parent().removeClass('active');
    },
    changeCurrency: function (e) {
        $('input[name="currency"]').val($(e.currentTarget).attr('data-code'));
        var data = $('#change-currency-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    search: function () {
        if ($('#search-form [name="search"]').val() != '') {
            var data = $('#search-form').serializeArray();
            $.post(null, data, function (result) {
                if (result.action.Result) {
                    window.location.replace(result.action.Redirect302);
                } else {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        }
                        application.createMessage(errorMessage);
                    }
                }
            });
        }
    },
    searchAdvanced: function () {
        var canSearch = false;
        if ($('input[name="search"]').eq(1).val() != '') {
            canSearch = true;
        }

        $('.advanced-search-filters input[type="number"]').each(function () {
            if ($(this).val() != '') {
                canSearch = true;
            }
        });

        $('.advanced-search-filters input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') != false && $(this).attr('name') != 'fields') {
                canSearch = true;
            }
        });

        if (canSearch) {
            var data = $('#SearchAdvancedForm').serializeArray();
            $.post(null, data, function (result) {
                if (result.action.Result) {
                    var minPrice = $('#SearchAdvancedForm input[name="$minPrice"]').val();
                    var maxPrice = $('#SearchAdvancedForm input[name="$maxPrice"]').val();
                    if (minPrice > 0 || maxPrice > 0) {
                        var url = (result.action.Redirect302).split('?');
                        var query = url[1];
                        if (minPrice > 0) {
                            query = query + '&seaF=' + minPrice;
                        }
                        if (maxPrice > 0) {
                            query = query + '&seaT=' + maxPrice;
                        }
                        url = url[0] + '?' + query;
                        window.location.replace(url);
                    } else {
                        window.location.replace(result.action.Redirect302);
                    }
                } else {
                    if (result.action.Code != 100) {
                        if (result.action.Validation != null) {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                        } else {
                            errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        }
                        application.createMessage(errorMessage);
                    }
                }
            });
        }
    },
    resetSearchAdvanced: function () {
        $('input[name="search"]').val('');
        $('.advanced-search-filters input[type="number"]').val('');
        $('.advanced-search-filters input[type="checkbox"]').prop('checked', false);
        $('.advanced-search-filters li').removeClass('open');
    },
    toggleTIN: function () {
        $('input[name="tin"]').toggleClass('hidden');
    },
    closeConfigMessage: function (e) {
        var name = $(e.currentTarget).attr('id');
        var days = $(e.currentTarget).attr('data-days');
        application.setCookie(name, '', 1, days);
        if ($(e.currentTarget).hasClass('popup')) {
            if ($(e.currentTarget).parent().parent().next().hasClass('hidden')) {
                $(e.currentTarget).parent().parent().next().removeClass('hidden');
            }
            $(e.currentTarget).parent().parent().remove();
        } else {
            $(e.currentTarget).parent().remove();
        }
    },
    deliveryAddressAdd: function (e) {
        var data = $('#delivery-address-add').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    deliveryAddressDelete: function (e) {
        var counter = $(e.currentTarget).attr('data-counter');
        var data = $('#delivery-address-delete-' + counter).serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                $(e.currentTarget).parent().parent().parent().remove();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    deliveryAddressUpdate: function (e) {
        var counter = $(e.currentTarget).attr('data-counter');
        var country = $('#delivery-address-update-' + counter).find('.select-country').val();
        $('#delivery-address-update-' + counter).find('input[name="countryCode"]').val(country);
        var data = $('#delivery-address-update-' + counter).serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    deliveryAddressLabelUpdate: function (e) {
        var counter = $(e.currentTarget).attr('data-counter');
        $('#delivery-address-update-' + counter).find('input[name="default"]').prop('checked', true);
        var data = $('#delivery-address-update-' + counter).serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
                window.location.reload();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        var length = result.action.Validation.length;
                        var errors = '';
                        for (var i = 0; i < length; i++) {
                            errors = errors + '<p>' + result.action.Validation[i].Error + '</p>';
                        }
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + errors;
                        application.createMessage(errorMessage, length * 1000);
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                        application.createMessage(errorMessage);
                    }
                }
            }
        });
    },
    toggleMoreLessText: function (e) {
        if ($(e.currentTarget).hasClass('more')) {
            $(e.currentTarget).parent().addClass('hidden');
            $(e.currentTarget).parent().next().removeClass('hidden');
        } else if ($(e.currentTarget).hasClass('less')) {
            $(e.currentTarget).parent().addClass('hidden');
            $(e.currentTarget).parent().prev().removeClass('hidden');
        }
    },
    redirectToExecutedOrders: function () {
        $('.order-details-container > .set-tab').addClass('redirect');
        $('.order-details-container > .set-tab').trigger('click');
        $('body').find('.order-details-container .orders-details > .inactive-tab.set-tab').trigger('click');
    },
    addComplaint: function (e) {
        var data = $(e.currentTarget).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.hidePopup(e);
                application.createMessage($('input[name="complaintSuccess"]').val(), 6000);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    addReturn: function (e) {
        var data = $(e.currentTarget).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.hidePopup(e);
                application.createMessage($('input[name="returnSuccess"]').val(), 6000);
                var complaintsButtons = $(e.currentTarget).parent();
                var quantity = parseInt($(e.currentTarget).find('input[name="quantity"]').val(), 10);
                application.updateMax(complaintsButtons, quantity);
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    updateMax: function (complaintsButtons, quantity) {
        if (complaintsButtons.find('.max-value .value').index() >= 0) {
            var max = parseInt(complaintsButtons.find('.max-value .value').eq(0).text(), 10) - quantity;
            if (max > 1) {
                complaintsButtons.find('.max-value .value').text(max);
                complaintsButtons.find('input[name="quantity"]').val(max).attr('max', max);
                complaintsButtons.find('input[name="quantity"]').prev().removeClass('min');
                complaintsButtons.find('input[name="quantity"]').next().addClass('max');
            } else if (max == 1) {
                complaintsButtons.find('.quantity-container').remove();
                complaintsButtons.find('form').append("<input type='hidden' value='1' name='quantity'/>");
            } else {
                complaintsButtons.prev().addClass('no-buttons');
                complaintsButtons.remove();
            }
        } else {
            complaintsButtons.prev().addClass('no-buttons');
            complaintsButtons.remove();
        }
    },
    cancelComplaint: function (e) {
        var data = $(e.currentTarget).parent().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                $('.complaint-ue').eq(0).replaceWith(result.template);
                application.createMessage($('input[name="complaint-info"]').val());
                application.loadImages();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    addToWishList: function () {
        var data = $('#add-to-wish-list-form').serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage('<div class="title">' + result.action.Message + '</div>');
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    deleteFromWishList: function (e) {
        var data = $(e.currentTarget).next().serializeArray();
        $.post(null, data, function (result) {
            if (result.action.Result) {
                application.createMessage($('input[name="deleted-from-wish-list"]').val());
                $('.wish-list').replaceWith(result.template);
                application.loadImages();
            } else {
                if (result.action.Code != 100) {
                    if (result.action.Validation != null) {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>' + '<p>' + result.action.Validation[0].Error + '</p>';
                    } else {
                        errorMessage = '<div class="title">' + result.action.Message + '</div>';
                    }
                    application.createMessage(errorMessage);
                }
            }
        });
    },
    resetFilters: function () {
        var url = window.location.href;
        var home = url.split('?')[0];
        //  var queriesString = url.split('?')[1];
        //  var queries = queriesString.split('&');
        //  for (var i=0; i<queries.length; i++) {
        // 	var pair = queries[i].split("=");
        // 	if(pair[0] == 'ftrLst'){
        // 		var index = i;
        // 	}
        // }
        // queries.splice(index, 1);
        // queriesString = '';
        // for (var i=0; i<queries.length; i++) {
        // 	queriesString = queriesString + queries[i] + '&';
        // }
        // queriesString = queriesString.slice(0, -1);
        // if(queriesString == ''){
        // 	url = home;
        // } else {
        // 	url = home + '?' + queriesString;
        // }
        window.location.replace(home);
    },
    showMoreInfo: function (e) {
        $(e.currentTarget).prev().removeClass('short');
        $(e.currentTarget).next().removeClass('hidden');
        $(e.currentTarget).addClass('hidden');
    },
    showLessInfo: function (e) {
        $(e.currentTarget).prev().prev().addClass('short');
        $(e.currentTarget).prev().removeClass('hidden');
        $(e.currentTarget).addClass('hidden');
    },
    showOrderDetailsPopupInLoyaltyHistory: function (e) {
        var template = 'partials/customer/order-details.html';
        var orderId = $(e.currentTarget).data('order-id');
        $.get(null, { __template: template, orderId: orderId, __collection: 'translations.Crt_OrderDetails|customer-profile.Order.Products' }, function (result) {
            var popup = '<div class="popup-dialog remove-popup"><div class="modal-box no-padding"><span class="order-details-title">'
                + result.collection['translations.Crt_OrderDetails'] + '</span><span class="fa fa-close hide-order-details-popup remove-popup"></span><div>' + result.template + '</div></div></div>';
            $('#main-section').append(popup);
            var orderIndex = $(e.currentTarget).index();
            var quantity = result.collection['customer-profile.Order.Products'].length;
            var urlArr = [];
            for (i = 0; i < quantity; i++) {
                var urlItem = [];
                var url = result.collection['customer-profile.Order.Products'][i].Url;
                var urlIndex = i;
                urlItem.push(url, urlIndex);
                urlArr.push(urlItem);
            }
            $.each(urlArr, function () {
                var url = $(this)[0];
                var index = $(this)[1];
                $.get(url, { index: index, __collection: 'product-details.Product|page' }, function (res) {
                    var orderIndex = $(e.currentTarget).index();
                    var index = parseInt(res.collection['page'].GET.index);
                    var weight = res.collection['product-details.Product'].Weight;
                    var status = res.collection['product-details.Product'].Availability.Text;
                    $('.popup-dialog.remove-popup').find('.cart-item').eq(index).find('.product-weight .attribute-value').prepend(weight + ' ');
                    $('.popup-dialog.remove-popup').find('.cart-item').eq(index).find('.product-availability .attribute-value').text(status);
                });
            });
            application.uiPreventScrolling();
        }).done(function () {
            application.loadImages();
        });
    },
    removePopup: function (e) {
        if ($(e.currentTarget).hasClass('hide-order-details-popup')) {
            $(e.currentTarget).parent().parent().remove();
        } else {
            $(e.currentTarget).remove();
        }
    },
    uiMenuButtonAnimation: function (button) {
        button.toggleClass('open');
    },
    uiScrollToTop: function (container) {
        $('html, body').animate({ scrollTop: container.offset().top });
    },
    uiChangeArrow: function (element) {
        if (element.hasClass('fa-angle-down')) {
            element.removeClass('fa-angle-down').addClass('fa-angle-up');
        } else {
            element.removeClass('fa-angle-up').addClass('fa-angle-down');
        }
    },
    uiIncrementProductsInCart: function () {
        var itemsCount = $('#header-section .cart-content .cart-amount').text();
        var newValue = parseInt(itemsCount, 10) + 1;

        $('#header-section .cart-content .cart-amount').text(newValue);
    },
    uiDecrementProductsInCart: function () {
        var itemsCount = $('#header-section .cart-content .cart-amount').text();
        var newValue = parseInt(itemsCount, 10) - 1;

        $('#header-section .cart-content .cart-amount').text(newValue);
    },
    uiDecrementTotalCost: function (totalCost, productPrice) {
        var total = parseFloat(totalCost.replace(',', '.')).toFixed(2);
        var product = parseFloat(productPrice.replace(',', '.')).toFixed(2);

        $('#total-cost').text((total - product).toFixed(2));
    },
    uiMoveLabel: function (e) {
        $(e.target).val() ? $(e.target).addClass('used') : $(e.target).removeClass('used');
    },
    uiCheckLabels: function () {
        var inputs = $('body').find('input');

        $.each(inputs, function (index, input) {
            if ($(input).val()) {
                $(input).addClass('used');
            }
        });
    },
    uiPreventScrolling: function () {
        $('body').toggleClass('modal-open');

        if ($('body').hasClass('modal-open') && window.innerWidth != window.outerWidth) {

            $('body').addClass('scroll-space');

        } else {
            $('body').removeClass('scroll-space');
        }
    },
    // uiFindActiveImage: function () {
    //   var images = $('.slider .product-image img');
    //   var quantity = images.length;
    //   for (i=0; i<quantity; i++){
    //     if (images.eq(i).attr('src') != ""){
    //       var index = i;
    //     }
    //   }
    //   application.uiToggleArrows(index, quantity);
    // },
    // uiToggleArrows: function(index, quantity) {
    //   var nav = $('.slider .slider-nav span');
    //   var leftArrow = $('.slider .fa-angle-left');
    //   var rightArrow = $('.slider .fa-angle-right');
    //   nav.removeClass('active');
    //   nav.eq(index).addClass('active');
    //   if(quantity <= 1){
    //     leftArrow.addClass('hidden');
    //     rightArrow.addClass('hidden');
    //   } else if(index == 0){
    //     leftArrow.addClass('hidden');
    //     rightArrow.removeClass('hidden');
    //   } else if(index+1 == quantity){
    //     leftArrow.removeClass('hidden');
    //     rightArrow.addClass('hidden');
    //   } else {
    //     leftArrow.removeClass('hidden');
    //     rightArrow.removeClass('hidden');
    //   }
    // },
    // uiSliderHideAlt: function(){
    //   $('.slider .product-image img').each(function(){
    //     if($(this).attr('src') == ""){
    //       $(this).addClass('hidden');
    //     } else {
    //       $(this).removeClass('hidden');
    //     }
    //   });
    // },
    uiSetSwitchNameWidthInSummaryCheckboxes: function () {
        if ($('#main-section').find('.summary-checkboxes').index() != -1) {
            $('#main-section').find('.summary-checkboxes .switch-name').each(function () {
                var width = $(this).parent().parent().width();
                $(this).css('width', width - 72);
            });
        }
    },
    uiSetSwitchNameWidthInInvoice: function () {
        if ($('#main-section').find('#invoice-address-data').index() != -1) {
            var width = $('#main-section').find('#invoice-address-data .switch-name').parent().parent().width();
            $('#main-section').find('#invoice-address-data .switch-name').css('width', width - 72);
        }
    },
    uiSetSwitchNameWidthInHeader: function () {
        $('#header-section').find('.switch-name').each(function () {
            var width = window.innerWidth;
            if (width <= 768) {
                var width = $(window).width();
                var left = -1 * (width - 82 - 17) - 10;
                $(this).css('width', width - 82 - 17);
                $(this).css('left', left);
            } else {
                var width = $(this).parent().parent().width() - 52 - 20 - 30;
                var left = -1 * (width) - 20;
                $(this).css('width', width);
                $(this).css('left', left);
            }
        });
    },
    uiSetSwitchNameWidthInNewsletter: function () {
        $('#footer-section').find('.newsletter .switch-name').each(function () {
            var width = $('#NewsletterSubscribeForm').width();
            $(this).css('width', width - 75);
        });
        $('#main-section').find('.contact-section .switch-name').each(function () {
            var windowWidth = window.innerWidth;
            var width = $('.middle-container .modal-box').width();
            if (windowWidth <= 480) {
                $(this).css('width', windowWidth - 82 - 2);
            } else if (windowWidth <= 768) {
                $(this).css('width', (windowWidth - 30) / 2 - 62 - 4);
            } else {
                // $(this).css('width', (width - 60)/2 - 62);
                $(this).css('width', (width - 70));
            }
        });
        $('#main-section').find('.registration-section .switch-name').each(function () {
            var width = window.innerWidth;
            if (width <= 480) {
                $(this).css('width', width - 102);
            } else if (width <= 768) {
                $(this).css('width', 355);
            } else {
                $(this).css('width', 375);
            }
        });
    },
    uiSetSwitchNameWidthInAfterAddingToCartPopup: function () {
        var switchLabelElement = $('.after-adding-to-cart-popup .switch-name');
        if (window.innerWidth <= 768) {
            var width = $(switchLabelElement).parent().parent().width() - 52 - 10;
        } else {
            var width = $(switchLabelElement).parent().parent().width() - 52 - 20;
        }
        $(switchLabelElement).css('width', width);
    },
    uiMakeFiltersLiHigher: function () {
        if (window.innerWidth > 768) {
            var names = $('.advanced-search-filters .switch-name');
            names.each(function () {
                if ($(this).text().length > 15) {
                    $(this).parent().parent().addClass('long');
                }
            });

        }
    },
    uiMakeUndercategoriesVisibleInFilters: function () {
        if (window.innerWidth > 768) {
            var undercategories = $('.filters .undercategories')
            undercategories.each(function () {
                $(this).removeClass('hidden');
                $(this).parent().addClass('open active');
                $(this).parent().find('.fa').addClass('hidden');
            });

        }
    },
    uiMakeUndercategoriesVisibleInAdvancedSearchFilters: function () {
        if (window.innerWidth > 768) {
            var undercategories = $('.advanced-search-filters .undercategories')
            undercategories.each(function () {
                $(this).removeClass('hidden');
                $(this).parent().addClass('open active');
                $(this).parent().find('.fa').addClass('hidden');
            });

        }
    },
    uiSetContainerWidthInCategories: function () {
        if (window.innerWidth > 768) {
            if ($('#main-section').find('section.categories .undercategories-menu').index() >= 0) {
                var windowWidth = window.innerWidth;
                var menuWidth = $('.undercategories-menu').width() + 40;
                var width = windowWidth - menuWidth - 20 - 17;
                $('section.categories .category-products').width(width);
            } else {
                $('section.categories .category-products').css('display', 'block');
            }
        }
    },
    uiSetImgInBlog: function () {
        var posts = $('.post-img');
        posts.each(function () {
            var url = $(this).next().val();
            $(this).css('background', 'url(' + url + ') no-repeat center');
        });
    },
    uiShowAddToCart: function () {
        $('.product-data .button-container .popup-dialog.instant-show').removeClass('hidden');
    },
    uiSetFixedHeader: function () {
        if (window.innerWidth > 768) {
            if ($(window).scrollTop() > 40) {
                $('.header-nav').addClass('fixed');
            } else {
                $('.header-nav').removeClass('fixed');
            }
        }
    },
    uiSetClientProfileHeight: function () {
        if (window.innerWidth > 768) {
            var height = $('.customer-details').height() + 60;
            if (height < 548) {
                height = 548;
            }
            $('.client-profile').height(height);
        }
    },
    uiEqualizeAddressSize: function () {
        var bestWidth = 0;
        var bestHeight = 0;
        $('.customer-details .address').each(function () {
            var width = $(this).width();
            if (width > bestWidth) {
                bestWidth = width;
            }
            var height = $(this).height();
            if (height > bestHeight) {
                bestHeight = height;
            }
        });

        $('.customer-details .address').width(bestWidth);
        $('.customer-details .address').height(bestHeight);
        $('.show-add-delivery-address').width(bestWidth - 18);
    },
    uiToggleCustomCheckbox: function (e) {
        if ($(e.currentTarget).prev().prop('checked')) {
            $(e.currentTarget).prev().prop('checked', false);
        } else {
            $(e.currentTarget).prev().prop('checked', true);
        }
    },
    uiSetCartItemsHeight: function () {
        var bestHeight = 0;
        $('.cart-item .product-description').each(function () {
            var height = $(this).height();
            if (height > bestHeight) {
                bestHeight = height;
            }
        });
        if (bestHeight < 22) {
            $('.cart-item .product-description').addClass('hidden');
        } else {
            $('.cart-item').each(function () {
                if (!$(this).parent().hasClass('popup-content')) {
                    if ($(this).children().hasClass('product-details')) {
                        $(this).find('> .product-details > .product-description').height(bestHeight);
                    } else {
                        $(this).find('> a > .product-details > .product-description').height(bestHeight);
                    }
                }
            });
        }
    },
    uiShowShortProductInfo: function () {
        if (window.innerWidth > 480 && $('.slider .product-description').index() >= 0) {
            var height = $('.slider .product-description').height();
            if (height > 200 && $('.slider .product-description').hasClass('dont-show')) {
                $('.slider .product-description').addClass('short');
                $('.slider .more-button').removeClass('hidden');
            } else if (height > 200) {
                $('.slider .less-button').removeClass('hidden');
            }
        }
    },
    uiShowShortCategoryInfo: function () {
        if ($('.category-info .text').index() >= 0) {
            var height = $('.category-info .text').height();
            if (height > 42 && $('.category-info .text').hasClass('dont-show')) {
                $('.category-info .text').addClass('short');
                $('.category-info .more-button').removeClass('hidden');
            } else if (height > 42) {
                $('.category-info .less-button').removeClass('hidden');
            }
        }
    },
    uiSetFocusOnInput: function (e) {
        $(e.currentTarget).prev().focus();
    },
    uiHideMessagePopup: function () {
        $('.message-popup-background').remove();
    },
    uiSetCategoryImgHeight: function () {
        var height = $('.category-img').width() / 4;
        $('.category-img').height(height);
    },
    uiSetBannerHeight: function () {
        var height = $('.banner .banner-img').eq(0).width() / 2;
        $('.banner-img').height(height);
        $('.banner > button').height(height);
    },
    uiSetCategoryTileHeight: function () {
        var height = $('.category-tile-container').eq(0).width() / 4;
        $('.category-tile-container').height(height);
    },
    uiEqualizeBlogPosts: function () {
        var topHeight = 0;
        $('.post').each(function () {
            var height = $(this).height();
            if (height > topHeight) {
                topHeight = height;
            }
        });
        $('.post').height(topHeight);
    },
    uiSetFilterButtonsFixed: function () {
        var buttonPosition = $('#btn_flt').offset().top;
        var filtersEnd = $('.filters .switches > li:last-child').offset().top + $('.filters .switches > li:last-child').height();
        if (filtersEnd > window.innerHeight - 122) {
            $('#btn_flt').addClass('fixed');
            $('#reset-filters').addClass('fixed');
            $('#footer-section').css('margin-top', 122);
            var buttonPosition = $('#btn_flt').offset().top;
        }
        if (buttonPosition < filtersEnd) {
            $('#btn_flt').addClass('fixed');
            $('#reset-filters').addClass('fixed');
            $('#footer-section').css('margin-top', 122);
            // if($('#btn_flt').hasClass('fixed') && buttonPosition > filtersEnd){
            // $('#btn_flt').removeClass('fixed');
            // 	$('#reset-filters').removeClass('fixed');
            // 	$('#footer-section').css('margin-top', 0);
            // }
        } else {
            $('#btn_flt').removeClass('fixed');
            $('#reset-filters').removeClass('fixed');
            $('#footer-section').css('margin-top', 0);

        }
    },
    events: function () {
        var self = this;

        $('#header-section').on('click', '.dropdown-menu', function (e) {
            e.stopPropagation();
        });

        $('#header-section').on('click', '.nav-links', function (e) {
            self.openCloseDropdown(e);
            self.uiPreventScrolling();
        });

        $('#header-section').on('click', '.logout', function (e) {
            self.logout(e);
        });

        $('#header-section').on('click', '.search', function (e) {
            var width = window.innerWidth;
            self.openCloseDropdown(e);
            self.listPhrases();
            if (width <= 768) {
                self.uiPreventScrolling();
                $(e.currentTarget).find('.recently-searched').removeClass('hidden');
            }
        });

        $('#header-section').on('click', '.search-tab-close-icon:not(.advanced)', function (e) {
            self.closeDropdown(e);
            self.closeRecentlySearched(e);
        });

        $('#header-section').on('click', '.recently-searched-icon:not(.advanced):not(.active)', function (e) {
            self.openRecentlySearched(e);
        });

        $('#header-section').on('click', '.recently-searched-icon:not(.advanced).active', function (e) {
            self.closeRecentlySearched(e);
        });

        $('#header-section').on('click', '.search-advanced-icon:not(.advanced)', function (e) {
            self.showAdvancedSearch();
            self.closeRecentlySearched(e);
        });

        $('#header-section').on('click', '.search-tab-close-icon.advanced', function (e) {
            self.hideAdvancedSearch();
            self.closeDropdownFromAdvanced(e);
            self.closeRecentlySearched(e);
        });

        $('#header-section').on('click', '.recently-searched-icon.advanced', function (e) {
            self.hideAdvancedSearch();
            self.openRecentlySearchedFromAdvanced(e);
        });

        $('#header-section').on('click', '.search-advanced-icon.advanced', function (e) {
            self.hideAdvancedSearch();
            self.closeRecentlySearched(e);
        });

        $('#header-section .search').on('click', '.search-advanced-button', function () {
            self.showAdvancedSearch();
        });

        $('#header-section .search').on('click', '.hide-search-advanced', function () {
            self.hideAdvancedSearch();
        });

        $('#header-section .main-nav').on('click', '.category-span', function (e) {
            var button = $(e.target).closest('li');
            self.openCloseCategories(e);
            self.uiMenuButtonAnimation(button);
        });

        $('#header-section .main-nav').on('click', '.undercategory-span', function (e) {
            var button = $(e.target).closest('li');
            self.openCloseUndercategories(e);
            self.uiMenuButtonAnimation(button);
        });

        $('#header-section .switches').on('click', 'span', function (e) {
            var button = $(e.target).closest('li');
            self.openCloseCategories(e);
            if (!$(e.currentTarget).hasClass('search-adv-button')) {
                self.uiMenuButtonAnimation(button);
            }
        });

        $('#header-section #search-form').on('click', '.search-button', function (e) {
            self.addPhrase(e);
            self.search();
        });

        $('#header-section #SearchAdvancedForm').on('click', '.search-button', function (e) {
            self.addPhrase(e);
            self.searchAdvanced();
        });

        $('#header-section #SearchAdvancedForm').on('click', '.search-adv-button', function (e) {
            $('#header-section #SearchAdvancedForm .search-button').trigger('click');
        });

        $('#header-section #SearchAdvancedForm').on('click', '.reset-adv-button', function (e) {
            self.resetSearchAdvanced();
        });

        $('#header-section .recently-searched').on('click', 'li', function (e) {
            self.searchPhrase(e);
        });

        $('#header-section .header-categories').on('mouseenter', 'a', function (e) {
            self.showUndercategories(e);
        });

        $('#header-section .header-categories').on('mouseleave', 'a', function (e) {
            self.hideUndercategories(e);
        });

        $('#header-section .header-undercategories').on('mouseenter', '.undercategories', function (e) {
            self.keepUndercategoriesVisible(e);
        });

        $('#header-section .header-undercategories').on('mouseleave', '.undercategories', function (e) {
            self.dontKeepUndercategoriesVisible(e);
        });

        $('#header-section').on('click', '.current-lang', function (e) {
            self.showLanguages(e);
        });

        $('#header-section #change-language-form').on('click', 'li.active', function (e) {
            e.stopPropagation();
            self.hideLanguages(e);
        });

        $('#header-section #change-language-form').on('click', 'li:not(.active)', function (e) {
            self.changeLanguage(e);
        });

        $('#header-section').on('click', '.current-currency', function (e) {
            self.showCurrency(e);
        });

        $('#header-section #change-currency-form').on('click', 'li.active', function (e) {
            self.hideCurrency(e);
        });

        $('#header-section #change-currency-form').on('click', 'li:not(.active)', function (e) {
            self.changeCurrency(e);
        });

        $('#main-section .filters .switches').on('click', '.heading-container', function (e) {
            var button = $(e.currentTarget).parent();
            self.openCloseCategories(e);
            self.uiMenuButtonAnimation(button);
            $(e.currentTarget).parent().toggleClass('active');
            $(e.currentTarget).find('> .fa').toggleClass('fa-minus');
            $(e.currentTarget).find('> .fa').toggleClass('fa-plus');
            self.uiSetFilterButtonsFixed();
        });

        $('#main-section').on('click', '.open-popup', function (e) {
            self.showPopup(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.open-popup.product-details', function (e) {
            application.addClipAndBatchToCart(e);
        });

        $('#main-section').on('click', '.open-popup-next', function (e) {
            self.showPopupNext(e);
            self.uiPreventScrolling();
            self.uiSetSwitchNameWidthInNewsletter();
        });

        $('#main-section').on('click', '.open-popup-next-closest', function (e) {
            self.showPopupNextClosest(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.open-popup-in-popup', function (e) {
            e.stopPropagation();
            self.showPopup(e);
        });
        $('#main-section').on('click', '.open-popup-switch', function (e) {
            if (e.target.tagName === "SPAN") {
                var checked = $('input[name="invoice"]').prop('checked');
                if (!checked) {
                    self.showPopup(e);
                    self.uiPreventScrolling();
                } else {
                    if ($('input[name="company-or-not"]').val() == 'company') {
                        $('input[name="invoice"]').prop('checked', true);
                    }
                }
            }
        });

        $('#main-section').on('click', '.open-popup-with-form', function (e) {
            self.showPopupWithForm(e);
            self.uiPreventScrolling();
        });


        $('#main-section').on('click', '.open-popup-ask-for-price:not(.in-list)', function (e) {
            self.showPopupWithForm(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.open-popup-ask-for-price.in-list', function (e) {
            self.showPopupNext(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.open-popup-complaint', function (e) {
            self.showPopupComplaint(e);
            // self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.open-popup-return', function (e) {
            self.showPopupReturn(e);
            // self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.show-complaint-details-popup', function (e) {
            self.showPopupComplaintDetails(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '#registration-standard-form .inactive-tab', function (e) {
            self.showCompanyAccountForm(e);
        });

        $('#main-section').on('click', '#registration-company-form .inactive-tab', function (e) {
            self.showStandardAccountForm(e);
        });

        $('#main-section').on('click', '#registration-standard-form-popup .inactive-tab', function (e) {
            self.showCompanyAccountFormPopup(e);
        });

        $('#main-section').on('click', '#registration-company-form-popup .inactive-tab', function (e) {
            self.showStandardAccountFormPopup(e);
        });

        $('#main-section').on('click', '#register-standard-account', function (e) {
            self.registerStandardAccount(e);
        });

        $('#main-section').on('click', '#register-company-account', function (e) {
            self.registerCompanyAccount(e);
        });

        $('#main-section').on('click', '#register-standard-account-popup', function (e) {
            self.registerStandardAccountPopup(e);
        });

        $('#main-section').on('click', '#register-company-account-popup', function (e) {
            self.registerCompanyAccountPopup(e);
        });

        $('#main-section').on('click', '#login', function (e) {
            self.login(e);
        });

        $('#main-section').on('click', '.login', function (e) {
            self.loginPopup(e);
        });

        $('#main-section').on('click', '#send-pass', function (e) {
            self.recoverPass(e);
        });

        $('#main-section').on('click', '#reset-pass', function (e) {
            self.resetPass(e);
        });

        $('#main-section').on('click', '#ask-about-product-button', function (e) {
            self.askAboutProduct(e);
        });

        $('#main-section').on('click', '#recommend-product-button', function (e) {
            self.recommendProduct(e);
        });

        $('#main-section').on('click', '#notify-about-availability-button', function (e) {
            self.notifyAboutAvailability(e);
        });

        $('#main-section').on('click', '#ask-for-price-button', function (e) {
            self.askForPrice(e);
        });

        $('#main-section').on('click', '.ask-for-price-button', function (e) {
            self.askForPriceInList(e);
        });

        $('#main-section').on('click', '#show-map', function (e) {
            self.showMap(e);
        });

        $('#main-section').on('click', '.show-map', function (e) {
            self.showMap(e);
        });

        $('#main-section').on('click', '.close-button', function (e) {
            self.hidePopup(e);
            self.uiPreventScrolling();
            $('input[name="invoice"]').prop('checked', false);
            if ($('#invoice-address-data').index() >= 0) {
                self.toggleInvoice(e);
            }
        });

        $('#main-section').on('click', '.popup-dialog:not(.recalculate)', function (e) {
            var width = window.innerWidth;
            if (width > 768 && e.target === this && !$(this).hasClass('contact-form-popup')) {
                self.hidePopup(e);
                self.uiPreventScrolling();
                $('input[name="invoice"]').prop('checked', false);
                if ($('#invoice-address-data').index() >= 0) {
                    self.toggleInvoice(e);
                }
            }
        });

        $('#main-section').on('click', '.popup-dialog.recalculate', function (e) {
            var width = window.innerWidth;
            if (width > 768 && e.target === this && !$(this).hasClass('contact-form-popup')) {
                self.hidePopup(e);
            }
        });

        $('#main-section').on('click', '.close-button-in-popup', function (e) {
            e.stopPropagation();
            self.hidePopup(e);
        });

        $('#main-section').on('click', '.close', function (e) {
            self.hidePopup(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.set-tab', function (e) {
            self.setTab(e);
        });

        $('#main-section').on('click', '.label', function (e) {
            self.openCloseLabelDropdown(e);
        });

        $('#main-section').on('click', '.button-plus', function (e) {
            self.incrementValue(e);
            self.recalculateProductsValue(e);
        });

        $('#main-section').on('click', '.button-minus', function (e) {
            self.decrementValue(e);
            self.recalculateProductsValue(e);
        });

        $('#main-section').on('click', '.button-plus-add-product', function (e) {
            self.incrementValue(e);
        });

        $('#main-section').on('click', '.button-minus-add-product', function (e) {
            self.decrementValue(e);
        });

        $('#main-section').on('change', '.quantity-field', function (e) {
            self.validateQuantitySymbols(e);
        });

        $('#main-section').on('change', '.quantity-field[max]', function (e) {
            self.validateQuantity(e);
        });

        $('#main-section').on('click', '.add-to-cart-open', function (e) {
            self.showProductDetailsPopup(e);
        });

        $('#main-section').on('click', '#AddToCartForm .attributes-select span', function (e) {
            self.setSupplyId(e);
        });

        $('#main-section').on('click', '.button-option.unitId', function (e) {
            self.setUnitId(e);
        });

        $('#main-section').on('click', '#AddToCartForm .button-option.attributeId', function (e) {
            self.setAttributeId(e);
        });

        $('#main-section').on('click', '#remove-points', function (e) {
            self.recalculate();
        });

        $('#main-section').on('click', '.button-option', function (e) {
            self.enableDisableButton(e);
            self.changeValues(e);
        });

        $('#main-section').on('click', '.add-to-cart', function (e) {
            self.addToCart(e);
        });

        $('#main-section').on('click', '.remove-from-cart', function (e) {
            e.preventDefault();
            self.removeFromCart(e);
        });

        $('#main-section').on('change', '.shopping-cart .quantity-field', function (e) {
            self.recalculateProductsValue(e);
        });

        $('#main-section').on('blur', 'input, textarea', function (e) {
            self.uiMoveLabel(e);
        });

        $('#main-section').on('click', '.order-prev-step', function (e) {
            self.orderPrevStep(e);
        });

        $('#main-section').on('click', '.order-next-step', function (e) {
            self.orderNextStep(e);
        });

        $('#main-section').on('change', '.select-country:not(.select-country-registration):not(.in-invoice)', function (e) {
            self.selectDeliveryCountry(e);
        })

        $('#main-section').on('change', '.select-country-registration', function (e) {
            self.selectCountryRegistration(e);
        });

        $('#main-section').on('click', '#delivery-country-change-button', function (e) {
            self.changeDeliveryCountry(e);
        });

        $('#main-section').on('click', '.delivery-method-container label', function (e) {
            self.showPaymentOptions(e);
        });

        $('#main-section').on('click', '.payment-container input', function (e) {
            self.setPayment(e);
        });

        $('#main-section').on('change', '#delivery-address-data input', function () {
            self.saveDeliveryAddress();
        });

        $('#main-section').on('change', 'textarea[name="note"]', function (e) {
            self.addNote(e);
        });

        $('#main-section').on('change', 'input[name="invoice"]', function (e) {
            if ($('#invoice-address-data').index() >= 0) {
                self.toggleInvoice(e);
            }
        });

        $('#main-section').on('click', '#save-invoice-address', function (e) {
            self.saveInvoiceAddress(e);
        });

        $('#main-section').on('click', '#place-order', function (e) {
            self.placeOrder(e);
        });

        $('#main-section').on('click', '#print-order', function (e) {
            self.printOrder(e);
        });

        $('#main-section').on('click', '#edit-customer-data', function (e) {
            self.updateCustomerData(e);
        });

        $('#main-section').on('click', '#edit-customer-email', function (e) {
            self.updateCustomerEmail(e);
        });

        $('#main-section').on('click', '#edit-customer-password', function (e) {
            self.updateCustomerPassword(e);
        });

        $('#main-section').on('click', '#comparison-tool-add-button', function (e) {
            self.addToComparer(e);
        });

        $('#main-section').on('click', '.comparison-tool-delete-button', function (e) {
            self.deleteFromComparer(e);
        });

        $('#main-section').on('click', '#search-collection-point-form .search-button', function (e) {
            self.searchInCollectionPoints(e);
        });

        $('#main-section').on('click', '.show-order-details-popup', function (e) {
            e.stopPropagation();
            self.showOrderDetailsPopup(e);
        });

        $('#main-section').on('click', '.hide-order-details-popup', function (e) {
            e.stopPropagation();
            self.hidePopup(e);
            self.uiPreventScrolling();
        });

        $('#main-section').on('click', '.orders-details .popup-dialog', function (e) {
            e.stopPropagation();
        });

        $('#main-section').on('click', '.complaints-details .popup-dialog', function (e) {
            e.stopPropagation();
        });

        $('#main-section').on('click', '.pagination li', function (e) {
            self.changePage(e);
        });

        // $('#main-section').on('click', '.slider .fa-angle-right', function(e) {
        //   self.sliderNext(e);
        // });

        // $('#main-section').on('click', '.slider .fa-angle-left', function(e) {
        //   self.sliderPrev(e);
        // });

        // $('#main-section').on('click', '.slider .slider-nav span', function(e) {
        //   self.sliderChoose(e);
        // });

        $('#main-section').on('click', '#DiscountCodeForm span', function () {
            self.addDiscount();
        });

        $('#main-section').on('click', '#coupon-delete span', function () {
            self.deleteDiscount();
        });

        $('#main-section').on('click', '#edit-review-author-name', function (e) {
            self.changeReviewAuthorName(e);
        });

        $('#main-section').on('click', '.add-review', function () {
            self.addReview();
        });

        $('#main-section').on('click', '.add-review-in-profile', function (e) {
            self.addReviewInProfile(e);
        });

        $('#main-section').on('click', '.contact-section .send-form', function (e) {
            e.stopPropagation(e);
            self.sendContactForm(e);
        });

        $('#main-section').on('click', '#accept-order-button', function (e) {
            self.acceptOrder(e);
        });

        $('#main-section').on('click', '#restore-payment-button', function (e) {
            self.restorePayment(e);
        });

        $('#main-section').on('click', '#cancel-order-button', function (e) {
            self.cancelOrder(e);
        });

        $('#main-section').on('click', '#unsubscribe-newsletter-button', function (e) {
            self.newsletterUnsubscribe(e);
        });

        $('#main-section').on('click', '.choose-collection-point', function () {
            application.createMessage($('input[name="choose-collection-point"]').val());
        });

        $('#main-section').on('click', '#delivery-address-add-button', function (e) {
            self.deliveryAddressAdd(e);
        });

        $('#main-section').on('click', '.delivery-address-update-button', function (e) {
            self.deliveryAddressUpdate(e);
        });

        $('#main-section').on('click', 'input[name="delivery-address"] + label', function (e) {
            self.deliveryAddressLabelUpdate(e);
        });

        $('#main-section').on('click', '.delivery-address-delete-button', function (e) {
            self.deliveryAddressDelete(e);
        });

        $('#main-section').on('click', '.custom-checkbox label', function (e) {
            self.uiToggleCustomCheckbox(e);
        });

        $('#main-section').on('click', '.category-info .more-less', function (e) {
            self.toggleMoreLessText(e);
        });

        $('#main-section').on('click', '.redirect-to-executed-orders-button', function () {
            self.redirectToExecutedOrders();
        });

        $('#main-section').on('click', '.add-complaint-button', function (e) {
            self.addComplaint(e);
        });

        $('#main-section').on('click', '.add-return-button', function (e) {
            self.addReturn(e);
        });

        $('#main-section').on('click', '.cancel-complaint-button', function (e) {
            self.cancelComplaint(e);
        });

        $('#main-section').on('click', '.add-to-wish-list', function (e) {
            self.addToWishList(e);
        });

        $('#main-section').on('click', '.delete-from-wish-list', function (e) {
            self.deleteFromWishList(e);
        });

        $('#main-section').on('click', '#reset-filters', function () {
            self.resetFilters();
        });

        $('#main-section').on('click', '.more-button', function (e) {
            self.showMoreInfo(e);
        });

        $('#main-section').on('click', '.less-button', function (e) {
            self.showLessInfo(e);
        });

        $('#main-section').on('click', '.new-review-in-profile .rating .fa', function (e) {
            var index = $(this).index();
            $(e.currentTarget).parent().parent().find('input[name="rating"]').val(index + 1);
            $(this).addClass('fa-star');
            $(this).removeClass('fa-star-o');
            $(this).prevAll().addClass('fa-star');
            $(this).prevAll().removeClass('fa-star-o');
            $(this).nextAll().addClass('fa-star-o');
            $(this).nextAll().removeClass('fa-star');
        });

        $('#main-section').on('click', '.remember-choice', function (e) {
            self.rememberChoice(e);
        });

        $('#main-section').on('click', '.loyalty-history-item.from-order', function (e) {
            self.showOrderDetailsPopupInLoyaltyHistory(e);
        });

        $('#main-section').on('click', '.remove-popup', function (e) {
            self.removePopup(e);
        });

        $('#footer-section').on('click', '.close-button', function (e) {
            self.hidePopup(e);
            self.uiPreventScrolling();
        });

        $('#footer-section').on('click', '.popup-dialog', function (e) {
            var width = window.innerWidth;
            if (width > 768 && e.target === this) {
                self.hidePopup(e);
                // self.uiPreventScrolling();
            }
        });

        $('#footer-section').on('click', '#newsletter-button', function (e) {
            self.newsletterSubscribe(e);
        });

        $('#footer-section').on('click', '.advanced-search-link', function (e) {
            self.triggerSearchAdvanced(e);
        });

        $('#footer-section').on('click', '.scroll-to-top', function () {
            application.uiScrollToTop($('#header-section'));
        });

        $('body').on('click', '.config-messages .close-button-config-messages', function (e) {
            application.closeConfigMessage(e);
        });

        $('body').on('click', '.config-messages .popup', function (e) {
            application.closeConfigMessage(e);
        });

        $('body').on('click', 'label:not(.switch)', function (e) {
            application.uiSetFocusOnInput(e);
        });

        $('body').on('click', '.message-popup-background', function () {
            application.uiHideMessagePopup();
        });

        $('body').on('blur', 'input', function () {
            application.uiCheckLabels();
        });

        application.uiMakeFiltersLiHigher();

        application.uiSetSwitchNameWidthInNewsletter();
        $(window).on('resize', function () {
            self.uiSetSwitchNameWidthInNewsletter();
        });


        var oldWW = window.innerWidth;
        if (oldWW > 768 && oldWW < 1025) {
            var oldR = 1;
        } else if (oldWW >= 1025) {
            var oldR = 2;
        }
        $(window).on('resize', function () {
            var newWW = window.innerWidth;
            if (newWW > 768 && newWW < 1025) {
                var newR = 1;
            } else if (newWW >= 1025) {
                var newR = 2;
            }
            if (newR > oldR) {
                $('#header-section .nav-section .header-nav nav .search:not(.open)').trigger('click');
                oldWW = newWW;
                oldR = newR;
            } else if (newR < oldR) {
                $('#header-section .nav-section .header-nav nav .search.open').trigger('click');
                oldWW = newWW;
                oldR = newR;
            }
        });

        $(window).on('scroll', function () {
            application.toggleScrollToTopButton();
            application.uiSetFixedHeader();
        });

        if (window.innerWidth > 1024) {
            $('#header-section .search').trigger('click');
        }

        if ($('#main-section').find('.product-details').index() >= 0 ||
            $('#main-section').find('.main-page-products').index() >= 0 ||
            $('#main-section').find('.category-products').index() >= 0 ||
            $('#main-section').find('.comparer').index() >= 0) {
            application.addClipAndBatchToCart();
        }

        if ($('#main-section').find('input[name="is404"]').index() >= 0) {
            $('.advanced-search-link').trigger('click');
            var message = $('input[name="is404"]').val();
            application.createMessage(message);
        }

        if ($('#main-section').find('.category-img').index() >= 0) {
            application.uiSetCategoryImgHeight();
            $(window).on('resize', function () {
                application.uiSetCategoryImgHeight();
            });
        }

        if ($('#main-section').find('.category-tile-container').index() >= 0) {
            application.uiSetCategoryTileHeight();
            $(window).on('resize', function () {
                application.uiSetCategoryTileHeight();
            });
        }

        if ($('#main-section').find('.banner').index() >= 0) {
            application.slickSliderBanners();
            application.uiSetBannerHeight();
            var oldWidth = window.innerWidth;
            if (oldWidth < 481) {
                var oldResolution = 1;
            } else if (oldWidth > 768) {
                var oldResolution = 3;
            } else {
                var oldResolution = 2;
            }
            $(window).on('resize', function () {
                var newWidth = window.innerWidth;
                if (newWidth < 481) {
                    var newResolution = 1;
                } else if (newWidth > 768) {
                    var newResolution = 3;
                } else {
                    var newResolution = 2;
                }
                if (newResolution != oldResolution) {
                    window.location.reload();
                    oldWidth = newWidth;
                    oldResolution = newResolution;
                } else {
                    $('.banner').slick('unslick');
                    application.slickSliderBanners();
                    application.uiSetBannerHeight();
                }
            });
        }

        if ($('#main-section').find('.manufacturers').index() >= 0) {
            application.slickSliderManufacturers();
        }

        if ($('#main-section').find('.comparer').index() >= 0) {
            application.setHeightsInComparer();
            application.setFixedHorizontal();
            $(window).on('scroll', function () {
                self.setFixedHorizontal();
            });
        }

        if ($('#main-section').find('section.product-details').index() >= 0) {
            application.uiShowAddToCart();
            application.slickSliderProductDetails();
            application.slickSliderRelated();
            application.uiShowShortProductInfo();
            var hash = window.location.hash;
            if (hash == '#opinions') {
                $('[data-for="product-reviews"]').trigger('click');
                if (window.innerWidth > 768) {
                    $('html,body').scrollTop($('.current-reviews').offset().top - 81);
                } else {
                    $('html,body').scrollTop($('.current-reviews').offset().top);
                }
            }
        }

        if ($('#main-section').find('.category-info').index() >= 0) {
            application.uiShowShortCategoryInfo();
        }

        if ($('#main-section').find('.summary-checkboxes').index() >= 0) {
            application.uiSetSwitchNameWidthInSummaryCheckboxes();
            $(window).on('resize', function () {
                self.uiSetSwitchNameWidthInSummaryCheckboxes();
            });
        }

        if ($('#main-section').find('#invoice-address-data').index() >= 0) {
            application.uiSetSwitchNameWidthInInvoice();
            $(window).on('resize', function () {
                self.uiSetSwitchNameWidthInInvoice();
            });
        }

        if ($('#main-section').find('input[name="changes"]').index() >= 0) {
            application.recalculate();
        }

        if ($('#main-section').find('.left-menu').index() >= 0 && window.innerWidth > 768) {
            $('.left-menu .sorting').removeClass('hidden');
            $('.left-menu .filters').removeClass('hidden');
            application.uiSetFilterButtonsFixed();
            $(window).on('scroll', function () {
                application.uiSetFilterButtonsFixed();
            });
        }

        if ($('#main-section').find('.contact-section').index() >= 0 && window.innerWidth > 768) {
            $('.middle-container button:not(.send-form)').trigger('click');
            $('.right-container button').trigger('click');
            application.uiSetSwitchNameWidthInNewsletter();
            $('body').removeClass('modal-open');
            $('body').removeClass('scroll-space');
        }

        if ($('#main-section').find('section.blog').index() >= 0) {
            application.uiSetImgInBlog();
            if (window.innerWidth > 768) {
                application.uiEqualizeBlogPosts();
            }
        }

        if ($('#main-section').find('.client-profile').index() >= 0) {
            application.uiEqualizeAddressSize();
            application.uiSetClientProfileHeight();
        }
        if ($('#main-section').find('.cart-item').index() >= 0) {
            application.uiSetCartItemsHeight();
            $(document).ajaxSuccess(function () {
                application.uiSetCartItemsHeight();
            });
        }

        // Toggle sorting types in category and undercategory page (mobile)
        $('.page-header .sorting').click(function () {
            $('.sorting-types').toggleClass('hidden');
            $('.sorting span').toggleClass('hidden');
            $('.page-header .sorting').toggleClass('active');
            if ($(this).prev().hasClass('active')) {
                $(this).prev().removeClass('active');
                $('.filters').addClass('hidden');
                $(this).prev().find('span').toggleClass('hidden');
            }
        });

        // Toggle sorting types in category and undercategory page (desktop)
        $('.left-menu .sorting').click(function () {
            $('.sorting-types').toggleClass('hidden');
            $('.left-menu .sorting').toggleClass('active');
            if ($(this).find('.fa-plus').index() >= 0) {
                $(this).find('.fa-plus').addClass('fa-minus').removeClass('fa-plus');
            } else if ($(this).find('.fa-minus').index() >= 0) {
                $(this).find('.fa-minus').addClass('fa-plus').removeClass('fa-minus');
            }
        });

        // Toggle filters in category and undercategory page
        $('.page-header .search').click(function () {
            $('.filters').toggleClass('hidden');
            $('.page-header .search span').toggleClass('hidden');
            $('.page-header .search').toggleClass('active');
            self.uiMakeUndercategoriesVisibleInFilters();
            if ($('.sorting').hasClass('active')) {
                $('.sorting').removeClass('active');
                $('.sorting-types').addClass('hidden');
                $('.sorting span').toggleClass('hidden');
            }
        });

        // Sort products
        var list = $('body');

        function parseQuery(qstr) {
            var query = {}, a, i, b;
            if (qstr.length <= 1) return {};
            a = qstr.substr(1).split('&');
            for (i = 0; i < a.length; i++) { b = a[i].split('='); query[decodeURIComponent(b[0])] = decodeURIComponent(b[1] || ''); }
            return query;
        }

        list.on('click', '.sorting-types ul>li', function (e) {
            var sortItem = $(this);
            var qs = parseQuery(location.search);
            if (qs.pageId) delete qs.pageId;
            $.get('', { __collection: 'products.SortingQueryGET' }, function (res) {
                qs[res.collection] = sortItem.data('value');
                LoadList(location.pathname + '?' + $.param(qs));
            });
        });

        function LoadList(href, skip) {
            location.href = href;
        }

        // filters
        $.pushObj = function (t, o) { var p; for (p in o) t.push({ name: p, value: o[p] }); }
        list.on('click', '.filters #btn_flt', function (e) {

            e.preventDefault();
            var regex = new RegExp(/^\s*\d+\s*$/);

            var isNumsValid = true;

            $('input[name="$maxPrice"], input[name="$minPrice"]').each(function () {
                var val = $(this).val();

                console.log(typeof val);

                var isMatch = regex.test(val);
                console.log(val);

                if (val != '' && !isMatch) {
                    isNumsValid = false;
                }

            });

            if (isNumsValid) {
                var d = $('.filters .switches input').serializeArray();

                if (d.length == 2 && !d[0].value && !d[1].value) {
                    var qs = parseQuery(location.search);
                    if (qs.ftrLst) delete qs.ftrLst;
                    LoadList(location.pathname + '?' + $.param(qs));
                    return;
                };
                $.pushObj(d, { __action: 'get/SearchFilters', __csrf: __CSRF });
                var p = $.param(d, true);
                console.log(p);
                $.get(location.pathname, p, function (d) {

                    if (d.action.Object) {
                        var qs = parseQuery(location.search);
                        if (qs.pageId) delete qs.pageId;
                        if (list.data('searchid')) qs.__searchResultId = list.data('searchid');
                        qs.ftrLst = d.action.Object;
                        LoadList(location.pathname + '?' + $.param(qs));
                    }
                });
            } else {
                $('#numsValidator').show();
            }
        });

        // toPrice filter in JS
        Number.prototype.format = function (n, x, s, c) {
            var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
                num = this.toFixed(Math.max(0, ~~n));
            return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
        };

        Number.prototype.toPrice = function () {
            return this.format(__decPlaces, 3, ' ', __decSep);
        }

        // Adding Review
        $('.new-review .rating .fa').click(function () {
            var index = $(this).index();
            $('#add-review-form input[name="rating"]').val(index + 1);
            $(this).addClass('fa-star');
            $(this).removeClass('fa-star-o');
            $(this).prevAll().addClass('fa-star');
            $(this).prevAll().removeClass('fa-star-o');
            $(this).nextAll().addClass('fa-star-o');
            $(this).nextAll().removeClass('fa-star');
        });

        // Show more related items in Product Page
        $('.product-related.solo-item .show-more').click(function () {
            $('.product-related.solo-item').addClass('hidden');
            $('.product-related.multi-item').removeClass('hidden');
        });

        // Show less related items in Product Page
        $('.product-related.multi-item .show-less').click(function () {
            $('.product-related.multi-item').addClass('hidden');
            $('.product-related.solo-item').removeClass('hidden');
        });

        //when form includes textarea: disabled enter submiting when non-submit input is focused. Only submit buttons or inputs should submitting a form by click or enter.
        $('body').on('keydown', 'form input:not([type="submit"])', function (e) {
            if (e.keyCode == 13) {
                var textarea = $(this).parents('form').find('textarea');
                if (textarea.length > 0) {
                    e.preventDefault();
                }
            }
        });

        $('body').on('keydown', '#NewsletterSubscribeForm input', function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
            }
        });

    }
};