/* ============================================================
   NIRMALA ASTROLOGY - Main JavaScript
   Handles: Preloader, Navbar, AJAX Place Search, Form Validation,
   Toast Notifications, Stars Animation, Counter Animation, Modal
   ============================================================ */

(function ($) {
    'use strict';

    // ==================== PRELOADER ====================
    $(window).on('load', function () {
        setTimeout(function () {
            $('#preloader').addClass('hidden');
        }, 800);
    });

    // Fallback: hide preloader after 3 seconds even if load event doesn't fire
    setTimeout(function () {
        $('#preloader').addClass('hidden');
    }, 3000);

    // ==================== INIT ON DOM READY ====================
    $(document).ready(function () {

        // Initialize AOS (Animate on Scroll)
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-in-out',
                once: true,
                offset: 80
            });
        }

        initNavbar();
        initStarField();
        initCounterAnimation();
        initPlaceSearch();
        initFormHandlers();
        initAddPlaceModal();
        initGlobalLoading();
        initPdfDownloadRefresh();
        initSmoothScroll();
        setTodayDate();

    });

    // ==================== NAVBAR ====================
    function initNavbar() {
        // Add scrolled class on scroll
        $(window).on('scroll', function () {
            if ($(this).scrollTop() > 50) {
                $('#mainNavbar').addClass('scrolled');
            } else {
                $('#mainNavbar').removeClass('scrolled');
            }
        });

        // Close mobile navbar on link click
        $('.navbar-nav .nav-link').on('click', function () {
            if ($('.navbar-collapse').hasClass('show')) {
                $('.navbar-toggler').click();
            }
        });
    }

    // ==================== STAR FIELD ANIMATION ====================
    function initStarField() {
        var heroParticles = document.getElementById('heroParticles');
        if (!heroParticles) return;

        // Create stars
        for (var i = 0; i < 60; i++) {
            var star = document.createElement('div');
            star.className = 'star';
            var size = Math.random() * 3 + 1;
            star.style.width = size + 'px';
            star.style.height = size + 'px';
            star.style.left = Math.random() * 100 + '%';
            star.style.top = Math.random() * 100 + '%';
            star.style.setProperty('--duration', (Math.random() * 4 + 2) + 's');
            star.style.setProperty('--max-opacity', (Math.random() * 0.7 + 0.3).toString());
            star.style.animationDelay = (Math.random() * 4) + 's';
            heroParticles.appendChild(star);
        }

        // Add floating planets
        var planets = [
            { class: 'hero-planet hero-planet-1', size: 120, top: 15, right: 10 },
            { class: 'hero-planet hero-planet-2', size: 60, bottom: 25, left: 8 },
            { class: 'hero-planet hero-planet-3', size: 80, top: 60, right: 20 }
        ];
        planets.forEach(function (p) {
            var el = document.createElement('div');
            el.className = p.class;
            heroParticles.appendChild(el);
        });
    }

    // ==================== COUNTER ANIMATION ====================
    function initCounterAnimation() {
        var counters = document.querySelectorAll('.counter');
        if (counters.length === 0) return;

        var options = { threshold: 0.5 };
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    var counter = entry.target;
                    var target = parseInt(counter.getAttribute('data-target'));
                    animateCounter(counter, target);
                    observer.unobserve(counter);
                }
            });
        }, options);

        counters.forEach(function (counter) {
            observer.observe(counter);
        });
    }

    function animateCounter(element, target) {
        var current = 0;
        var increment = target / 80;
        var step = function () {
            current += increment;
            if (current < target) {
                element.textContent = formatNumber(Math.floor(current));
                requestAnimationFrame(step);
            } else {
                element.textContent = formatNumber(target) + '+';
            }
        };
        requestAnimationFrame(step);
    }

    function formatNumber(num) {
        if (num >= 1000) {
            return (num / 1000).toFixed(num >= 10000 ? 0 : 1) + 'K';
        }
        return num.toString();
    }

    // ==================== PLACE SEARCH (AJAX) ====================
    var searchTimeout = null;
    var activePlaceRequest = null;
    var placeSearchCache = {};
    var loaderDepth = 0;

    function initPlaceSearch() {
        var $input = $('#placeSearch');
        var $dropdown = $('#placeDropdown');
        var $placeId = $('#placeId');
        var $placeSelect = $('#placeSelect');

        $placeSelect.on('change', function () {
            var $option = $(this).find('option:selected');
            if (!this.value) {
                $placeId.val('');
                $input.val('');
                clearAutoFields();
                return;
            }
            $placeId.val(this.value);
            $input.val($option.data('place-name') || $option.text().trim());
            $('#latitude').val($option.data('latitude') || '');
            $('#longitude').val($option.data('longitude') || '');
            $('#timezone').val($option.data('timezone') || '5.30');
            if ($('#smtCorr').length) $('#smtCorr').val($option.data('smt-corr') || '');
        });

        if (!$input.length) return;

        // Search on typing (debounced, min 2 chars)
        $input.on('input', function () {
            var keyword = $(this).val().trim();

            // Clear if input is empty
            if (keyword.length === 0) {
                $dropdown.removeClass('show').html('');
                $placeId.val('');
                clearAutoFields();
                return;
            }

            // Require minimum 2 characters
            if (keyword.length < 2) {
                $dropdown.removeClass('show').html('');
                return;
            }

            // Debounce: wait 300ms after last keystroke
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(function () {
                searchPlaces(keyword);
            }, 220);
        });

        // Hide dropdown when clicking outside
        $(document).on('click', function (e) {
            if (!$(e.target).closest('#placeSearch, #placeDropdown').length) {
                $dropdown.removeClass('show');
            }
        });

        // Keyboard navigation
        $input.on('keydown', function (e) {
            var $items = $dropdown.find('.place-item');
            var $active = $items.filter('.active');
            var index = $items.index($active);

            if (e.keyCode === 40) { // Down
                e.preventDefault();
                if ($active.length) {
                    $active.removeClass('active');
                    $items.eq((index + 1) % $items.length).addClass('active');
                } else {
                    $items.first().addClass('active');
                }
            } else if (e.keyCode === 38) { // Up
                e.preventDefault();
                if ($active.length) {
                    $active.removeClass('active');
                    var prev = index - 1 < 0 ? $items.length - 1 : index - 1;
                    $items.eq(prev).addClass('active');
                }
            } else if (e.keyCode === 13) { // Enter
                e.preventDefault();
                if ($active.length) {
                    selectPlace($active);
                }
            } else if (e.keyCode === 27) { // Escape
                $dropdown.removeClass('show');
            }
        });
    }

    function searchPlaces(keyword) {
        var $dropdown = $('#placeDropdown');
        var $input = $('#placeSearch');
        keyword = $.trim(keyword);
        if (keyword.length < 2) return;

        if (placeSearchCache[keyword]) {
            renderPlaceResults(placeSearchCache[keyword], $dropdown);
            return;
        }
        if (activePlaceRequest) activePlaceRequest.abort();

        // Show loading
        $dropdown.html('<div class="place-item text-center"><i class="fas fa-spinner fa-spin text-gold"></i><span class="text-light-muted ms-2">Searching...</span></div>').addClass('show');

        activePlaceRequest = $.ajax({
            url: '/Home/SearchPlace',
            type: 'GET',
            data: { keyword: keyword },
            dataType: 'json',
            global: false,
            success: function (data) {
                placeSearchCache[keyword] = data || [];
                renderPlaceResults(placeSearchCache[keyword], $dropdown);
            },
            error: function (xhr, status) {
                if (status !== 'abort') $dropdown.html('<div class="place-item text-center text-danger">Error searching places. Please try again.</div>').addClass('show');
            },
            complete: function () { activePlaceRequest = null; }
        });
    }

    function renderPlaceResults(data, $dropdown) {
        if (data && data.length > 0) {
            var html = '';
            data.forEach(function (place) {
                html += '<div class="place-item" data-place-id="' + place.placeId + '" data-latitude="' + place.latitude + '" data-longitude="' + place.longitude + '" data-timezone="' + place.timezone + '" data-smt-corr="' + (place.smtCorr || '') + '">' +
                    '<div class="place-item-name"><i class="fas fa-map-marker-alt text-gold me-2"></i>' + place.placeName + '</div>' +
                    '<div class="place-item-detail">' + (place.countryName || '') + '</div></div>';
            });
            $dropdown.html(html).addClass('show');
            $dropdown.find('.place-item').on('click', function () { selectPlace($(this)); });
        } else {
            $dropdown.html('<div class="place-item text-center text-light-muted">No places found. <a href="#" data-bs-toggle="modal" data-bs-target="#addPlaceModal" class="text-gold">Add new place</a></div>').addClass('show');
        }
    }

    function selectPlace($item) {
        var placeName = $item.find('.place-item-name').text().trim();
        var placeId = $item.data('place-id');
        var latitude = $item.data('latitude');
        var longitude = $item.data('longitude');
        var timezone = $item.data('timezone');
        var smtCorr = $item.data('smt-corr');

        $('#placeSearch').val(placeName);
        $('#placeId').val(placeId);
        $('#PlaceName').val(placeName);
        $('#CountryName').val($item.find('.place-item-detail').text().trim());
        $('#latitude').val(latitude);
        $('#longitude').val(longitude);
        $('#timezone').val(timezone);
        if ($('#smtCorr').length) { $('#smtCorr').val(smtCorr); }
        $('#placeDropdown').removeClass('show');
    }

    function clearAutoFields() {
        $('#latitude').val('');
        $('#longitude').val('');
        $('#timezone').val('');
    }

    // ==================== FORM HANDLERS ====================
    function initFormHandlers() {
        // Clear form button
        $('#clearFormBtn').on('click', function () {
            $('#kundliForm')[0].reset();
            $('#placeId').val('');
            clearAutoFields();
            $('.field-validation-error').text('');
            $('.validation-summary-errors').remove();
        });

        // Generate Kundli button -> Calculate and redirect to Result page
        $('#generateKundliBtn').on('click', function (e) {
            e.preventDefault();
            // Validate required fields
            var name = $('input[name="Customer.Name"]').val().trim();
            var dob = $('input[name="Customer.DOB"]').val();
            var birthTime = $('input[name="Customer.BirthTime"]').val();
            var placeId = $('#placeId').val();

            if (!name) { showToast('Please enter your name.', 'error'); $('input[name="Customer.Name"]').focus(); return; }
            if (!dob) { showToast('Please select your date of birth.', 'error'); $('input[name="Customer.DOB"]').focus(); return; }
            if (!birthTime) { showToast('Please select your time of birth.', 'error'); $('input[name="Customer.BirthTime"]').focus(); return; }
            if (!placeId) { showToast('Please select your birth place.', 'error'); $('#placeSearch').focus(); return; }

            // Parse birth details
            var dobDate = new Date(dob);
            var day = dobDate.getDate();
            var month = dobDate.getMonth() + 1;
            var year = dobDate.getFullYear();
            var timeParts = birthTime.split(':');
            var hours = parseInt(timeParts[0]) || 0;
            var minutes = parseInt(timeParts[1]) || 0;
            var longitude = parseFloat($('#longitude').val()) || 0;
            var latitude = parseFloat($('#latitude').val()) || 0;
            var timezone = parseFloat($('#timezone').val());
            if (isNaN(timezone)) timezone = 5.30; // atlas HH.MM packed (FNDG → 5h30m)

            showSpinner('Calculating Kundli…');
            $('#generateKundliBtn').prop('disabled', true).attr('aria-busy', 'true');

            // Store birth data for the Result page
            var birthData = {
                name: name,
                gender: $('input[name="Gender"]:checked').val() || 'Male',
                day: day,
                month: month,
                year: year,
                hours: hours,
                minutes: minutes,
                longitude: longitude,
                latitude: latitude,
                timezone: timezone,
                place: $('#placeSearch').val(),
                reportChoice: 'DETAILED'
            };
            localStorage.setItem('kundliBirthData', JSON.stringify(birthData));

            // Persist on the server before redirecting so every report page can reuse the same birth data.
            var token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: $('#kundliForm').data('session-url') || '/api/birth-data/session',
                type: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    Name: birthData.name,
                    Gender: birthData.gender,
                    Day: birthData.day,
                    Month: birthData.month,
                    Year: birthData.year,
                    Hours: birthData.hours,
                    Minutes: birthData.minutes,
                    Latitude: birthData.latitude,
                    Longitude: birthData.longitude,
                    Timezone: birthData.timezone,
                    PlaceId: placeId,
                    Place: birthData.place,
                    ReportChoice: birthData.reportChoice
                },
                success: function (response) {
                    if (response && response.success) window.location.href = $('#kundliForm').data('result-url') || '/Home/KundliResult';
                    else showToast((response && response.message) || 'Unable to preserve birth data.', 'error');
                },
                error: function (xhr) {
                    hideSpinner();
                    $('#generateKundliBtn').prop('disabled', false).removeAttr('aria-busy');
                    var message = 'Unable to preserve birth data on the server.';
                    if (xhr && xhr.responseJSON && xhr.responseJSON.message) message = xhr.responseJSON.message;
                    else if (xhr && xhr.responseText) {
                        try { var body = JSON.parse(xhr.responseText); if (body.message) message = body.message; } catch (ignore) { }
                    }
                    showToast(message, 'error');
                }
            });
        });

        // Save New Data button (AJAX submit)
        $('#saveNewDataBtn').on('click', function () {
            var $form = $('#kundliForm');
            var formData = $form.serialize();

            // Client-side validation
            var name = $('input[name="Name"]').val().trim();
            var dob = $('input[name="DOB"]').val();
            var birthTime = $('input[name="BirthTime"]').val();
            var placeId = $('#placeId').val();

            if (!name) { showToast('Please enter your name.', 'error'); return; }
            if (!dob) { showToast('Please select your date of birth.', 'error'); return; }
            if (!birthTime) { showToast('Please select your time of birth.', 'error'); return; }
            if (!placeId) { showToast('Please select your birth place.', 'error'); return; }

            showSpinner();
            $.ajax({
                url: '/Home/SaveCustomer',
                type: 'POST',
                data: formData,
                dataType: 'json',
                success: function (response) {
                    hideSpinner();
                    if (response.success) {
                        showToast(response.message, 'success');
                        // Optionally reset form
                        $('#kundliForm')[0].reset();
                        $('#placeId').val('');
                        clearAutoFields();
                    } else {
                        showToast(response.message || 'An error occurred.', 'error');
                        if (response.errors && response.errors.length > 0) {
                            response.errors.forEach(function (err) {
                                showToast(err, 'error');
                            });
                        }
                    }
                },
                error: function () {
                    hideSpinner();
                    showToast('An error occurred while saving. Please try again.', 'error');
                }
            });
        });
    }

    // ==================== ADD PLACE MODAL ====================
    function initAddPlaceModal() {
        // Load countries when modal is shown
        $('#addPlaceModal').on('shown.bs.modal', function () {
            var $select = $('#countrySelect');
            if ($select.find('option').length <= 1) {
                $select.html('<option value="">-- Loading --</option>');
                $.ajax({
                    url: '/Home/GetCountries',
                    type: 'GET',
                    dataType: 'json',
                    success: function (data) {
                        var html = '<option value="">-- Select Country --</option>';
                        data.forEach(function (c) {
                            html += '<option value="' + c.countryId + '">' + c.countryName + '</option>';
                        });
                        $select.html(html);
                    },
                    error: function () {
                        $select.html('<option value="">-- Error loading --</option>');
                    }
                });
            }
        });

        // Handle "Add New Place" link inside dropdown
        $(document).on('click', '#placeDropdown a[data-bs-toggle="modal"]', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('#placeDropdown').removeClass('show');
            var modal = new bootstrap.Modal(document.getElementById('addPlaceModal'));
            modal.show();
        });

        // Save Place button
        $('#savePlaceBtn').on('click', function () {
            var $form = $('#addPlaceForm');
            var formData = $form.serialize();

            // Basic validation
            var placeName = $('input[name="PlaceName"]').val().trim();
            var countryId = $('select[name="CountryID"]').val();
            var lat = $('input[name="Latitude"]').val().trim();
            var lng = $('input[name="Longitude"]').val().trim();
            var tz = $('input[name="TimeZone"]').val().trim();

            if (!placeName || !countryId || !lat || !lng || !tz) {
                showToast('All required fields must be filled.', 'error');
                return;
            }

            showSpinner();
            $.ajax({
                url: '/Home/AddPlace',
                type: 'POST',
                data: formData,
                dataType: 'json',
                success: function (response) {
                    hideSpinner();
                    if (response.success) {
                        showToast(response.message, 'success');
                        // Close modal and reset form
                        var modal = bootstrap.Modal.getInstance(document.getElementById('addPlaceModal'));
                        if (modal) modal.hide();
                        $form[0].reset();
                        // Refresh place search if there's a keyword
                        var keyword = $('#placeSearch').val().trim();
                        if (keyword.length >= 2) {
                            searchPlaces(keyword);
                        }
                    } else {
                        showToast(response.message || 'Failed to add place.', 'error');
                        if (response.errors && response.errors.length > 0) {
                            response.errors.forEach(function (err) {
                                showToast(err, 'error');
                            });
                        }
                    }
                },
                error: function () {
                    hideSpinner();
                    showToast('Error saving place. Please try again.', 'error');
                }
            });
        });
    }

    // ==================== GLOBAL LOADING BEHAVIOR ====================
    function initGlobalLoading() {
        $(document).on('click', 'a[href]', function (e) {
            var $a = $(this);
            var href = $a.attr('href') || '';
            if (e.isDefaultPrevented() || href === '#' || href.indexOf('#') === 0 || href.indexOf('javascript:') === 0 || $a.attr('target') === '_blank' || $a.attr('download') !== undefined || $a.hasClass('dropdown-toggle')) return;
            if (href.indexOf('tel:') === 0 || href.indexOf('mailto:') === 0) return;
            if ($a.data('no-spinner') || $a.hasClass('js-pdf-download')) return;
            showSpinner('Loading…');
        });
        $(document).on('submit', 'form', function () {
            var $f = $(this);
            if ($f.valid && !$f.valid()) return;
            if ($f.data('no-spinner') || $f.hasClass('js-pdf-download-form')) return;
            showSpinner('Submitting…');
        });
    }

    // PDF attachment download: show loader briefly, download via hidden iframe, hide loader, refresh.
    function initPdfDownloadRefresh() {
        $(document).on('click', 'a.js-pdf-download', function (e) {
            e.preventDefault();
            var href = $(this).attr('href');
            if (!href) return;
            var refresh = refreshFlag($(this));
            downloadPdfThenRefresh(href, refresh);
        });
        $(document).on('submit', 'form.js-pdf-download-form', function (e) {
            var $f = $(this);
            var submitter = e.originalEvent && e.originalEvent.submitter;
            var mode = submitter ? $(submitter).val() : $f.find('button[type=submit][clicked=true]').val();
            if (!mode) {
                // fallback: which button was last clicked
                mode = $f.data('last-mode') || 'download';
            }
            if (mode === 'preview') {
                // allow normal navigation for preview
                showSpinner('Loading preview…');
                return true;
            }
            e.preventDefault();
            var action = $f.attr('action') || window.location.href;
            var params = $f.serialize() + '&mode=download';
            var url = action + (action.indexOf('?') >= 0 ? '&' : '?') + params;
            // Prefer POST via temporary form targeting hidden iframe so AntiForgery works
            postPdfDownload($f, refreshFlag($f));
        });
        $(document).on('click', 'form.js-pdf-download-form button[type=submit]', function () {
            $(this).closest('form').data('last-mode', $(this).val());
        });
    }

    function refreshFlag($el) {
        var v = $el.data('pdf-refresh');
        return v === 1 || v === '1' || v === true;
    }

    function postPdfDownload($form, refresh) {
        showSpinner('Exporting PDF…');
        var iframe = document.getElementById('pdfDownloadFrame');
        if (!iframe) {
            iframe = document.createElement('iframe');
            iframe.id = 'pdfDownloadFrame';
            iframe.name = 'pdfDownloadFrame';
            iframe.style.display = 'none';
            iframe.setAttribute('aria-hidden', 'true');
            document.body.appendChild(iframe);
        }
        var $clone = $form.clone();
        $clone.attr('target', 'pdfDownloadFrame');
        $clone.find('button[name=mode]').remove();
        $clone.append('<input type="hidden" name="mode" value="download" />');
        $clone.css({ position: 'absolute', left: '-9999px' });
        $('body').append($clone);
        $clone.trigger('submit');
        setTimeout(function () {
            $clone.remove();
            forceHideSpinner();
            if (refresh) window.location.reload();
        }, 1800);
    }

    var pdfPrefetch = {};

    function savePdfBlob(file) {
        var a = document.createElement('a');
        var href = URL.createObjectURL(file.blob);
        a.href = href;
        a.download = file.name || 'Kundli.pdf';
        document.body.appendChild(a);
        a.click();
        a.remove();
        setTimeout(function () { URL.revokeObjectURL(href); }, 2000);
    }

    function fetchPdf(url) {
        return fetch(url, { credentials: 'same-origin' }).then(function (r) {
            if (!r.ok) throw new Error('pdf');
            var name = 'Kundli.pdf';
            var cd = r.headers.get('Content-Disposition') || '';
            var m = /filename=\"?([^\";]+)\"?/i.exec(cd);
            if (m && m[1]) name = m[1];
            return r.blob().then(function (blob) { return { blob: blob, name: name }; });
        });
    }

    window.prefetchPdf = function (url) {
        if (!url) return null;
        if (!pdfPrefetch[url]) {
            pdfPrefetch[url] = fetchPdf(url).then(function (file) {
                var status = document.getElementById('pdfExportStatus');
                if (status) status.textContent = 'Report is ready. Click Export PDF to save it.';
                return file;
            }).catch(function (err) {
                delete pdfPrefetch[url];
                throw err;
            });
        }
        return pdfPrefetch[url];
    };

    window.downloadPdfThenRefresh = function (url) {
        showSpinner('Exporting PDF…');
        var pending = (window.prefetchPdf && window.prefetchPdf(url)) || fetchPdf(url);
        pending.then(function (file) {
            savePdfBlob(file);
        }).catch(function () {
            window.location.href = url;
        }).then(function () {
            forceHideSpinner();
        });
    };

    // ==================== SMOOTH SCROLL ====================
    function initSmoothScroll() {
        $('a[href^="#"]').on('click', function (e) {
            var target = $(this.getAttribute('href'));
            if (target.length) {
                e.preventDefault();
                $('html, body').animate({
                    scrollTop: target.offset().top - 70
                }, 600, 'swing');
            }
        });
    }

    // ==================== SET TODAY DATE ====================
    function setTodayDate() {
        var $el = $('#todayDate');
        if ($el.length) {
            var now = new Date();
            var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            $el.text(now.toLocaleDateString('en-IN', options));
        }
    }

    // ==================== TOAST NOTIFICATION ====================
    window.showToast = function (message, type) {
        type = type || 'success';
        var iconClass = type === 'success' ? 'fas fa-check-circle text-success' : 'fas fa-exclamation-circle text-danger';
        var bgClass = type === 'success' ? 'toast-success' : 'toast-error';

        var toastHtml = '<div class="toast ' + bgClass + ' show" role="alert">' +
            '<div class="toast-header">' +
            '<i class="' + iconClass + ' me-2"></i>' +
            '<strong class="me-auto text-light">' + (type === 'success' ? 'Success' : 'Error') + '</strong>' +
            '<button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>' +
            '</div>' +
            '<div class="toast-body text-light">' + message + '</div>' +
            '</div>';

        $('#toastContainer').append(toastHtml);

        // Auto remove after 4 seconds
        setTimeout(function () {
            $('#toastContainer .toast').last().fadeOut(300, function () {
                $(this).remove();
            });
        }, 4000);
    };

    // ==================== LOADING SPINNER ====================
    window.showSpinner = function (message) {
        loaderDepth++;
        var $spinner = $('#globalSpinner');
        if ($spinner.length === 0) {
            $('body').append('<div id="globalSpinner" class="spinner-overlay" role="status" aria-live="polite"><div class="spinner-panel"><div class="spinner-circle"></div><div class="spinner-message"></div></div></div>');
            $spinner = $('#globalSpinner');
        }
        $spinner.stop(true, true).show().find('.spinner-message').text(message || 'Loading…');
    };

    window.hideSpinner = function () {
        loaderDepth = Math.max(0, loaderDepth - 1);
        if (loaderDepth > 0) return;
        $('#globalSpinner').stop(true, true).fadeOut(180, function () { $(this).remove(); });
    };

    window.forceHideSpinner = function () {
        loaderDepth = 0;
        $('#globalSpinner').stop(true, true).fadeOut(180, function () { $(this).remove(); });
    };

})(jQuery);