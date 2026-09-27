/*
 * Kun.svg template binder + helpers for D-1 / varga / chalit / special-lagna charts.
 * Geometry stays in Content/Kun.svg; only existing IDs are populated.
 */
(function (root) {
    'use strict';

    var templatePromise;
    var PLANET_IDS = {
        sun: 's', surya: 's', sur: 's',
        moon: 'c', chandra: 'c', cha: 'c', mon: 'c',
        mars: 'm', mangal: 'm', mangla: 'm', mgl: 'm', mar: 'm',
        mercury: 'b', budha: 'b', budh: 'b', bud: 'b', mer: 'b',
        jupiter: 'g', guru: 'g', gur: 'g', jup: 'g',
        venus: 'sk', sukra: 'sk', shukra: 'sk', suk: 'sk', ven: 'sk',
        saturn: 'sn', sani: 'sn', shani: 'sn', san: 'sn', sat: 'sn',
        rahu: 'r', rah: 'r',
        ketu: 'k', ket: 'k'
    };
    var PLANET_LABELS = {
        s: 'SUR', c: 'CHA', m: 'MGL', b: 'BUD', g: 'GUR',
        sk: 'SUK', sn: 'SAN', r: 'RAH', k: 'KET'
    };

    function read(obj, lower, upper, fallback) {
        if (obj && obj[lower] !== undefined && obj[lower] !== null) return obj[lower];
        if (obj && obj[upper] !== undefined && obj[upper] !== null) return obj[upper];
        return fallback;
    }

    function normalizeName(value) {
        return String(value || '').toLowerCase().replace(/[^a-z]/g, '');
    }

    function planetPrefix(name) {
        return PLANET_IDS[normalizeName(name)] || null;
    }

    function findById(svg, id) {
        return svg.querySelector('[id="' + id + '"]') || null;
    }

    /** Kun.svg uses bare <text id="…"> nodes — set textContent directly. */
    function setExistingText(svg, id, value) {
        var node = findById(svg, id);
        if (!node) return false;
        var text = value == null ? '' : String(value);
        if (node.tagName && node.tagName.toLowerCase() === 'text') {
            while (node.firstChild) node.removeChild(node.firstChild);
            node.appendChild(svg.ownerDocument.createTextNode(text));
            return true;
        }
        var textNodes = node.getElementsByTagName('text');
        if (textNodes.length > 0) {
            textNodes[0].textContent = text;
            for (var i = 1; i < textNodes.length; i++) textNodes[i].textContent = '';
        } else {
            node.textContent = text;
        }
        return true;
    }

    function clearExistingText(svg, id) {
        setExistingText(svg, id, '');
    }

    function appendPlanetLabel(svg, id, label) {
        var node = findById(svg, id);
        if (!node) return;
        var cur = (node.textContent || '').trim();
        var next = cur ? (cur + ' ' + label) : label;
        setExistingText(svg, id, next);
    }

    function loadTemplate(templateUrl) {
        var url = templateUrl || '/Content/Kun.svg';
        if (!templatePromise || templatePromise._url !== url) {
            templatePromise = fetch(url, { cache: 'no-store' })
                .then(function (response) {
                    if (!response.ok) throw new Error('Kun.svg could not be loaded.');
                    return response.text();
                });
            templatePromise._url = url;
        }
        return templatePromise;
    }

    function normalizeSvgSize(svg) {
        // Force readable size regardless of host CSS / missing viewBox
        svg.setAttribute('viewBox', '0 0 400 400');
        svg.setAttribute('preserveAspectRatio', 'xMidYMid meet');
        svg.setAttribute('width', '100%');
        svg.setAttribute('height', '100%');
        svg.style.display = 'block';
        svg.style.maxWidth = '100%';
        svg.style.width = '100%';
        svg.style.height = 'auto';
        svg.style.aspectRatio = '1 / 1';
    }

    function bindTemplate(templateText, houses, positions) {
        var parser = new DOMParser();
        var documentNode = parser.parseFromString(templateText, 'image/svg+xml');
        var svg = documentNode.documentElement;
        if (!svg || svg.nodeName.toLowerCase() === 'parsererror' || svg.querySelector('parsererror')) {
            throw new Error('Kun.svg parse failed.');
        }
        normalizeSvgSize(svg);

        for (var house = 1; house <= 12; house++) {
            var houseData = houses && houses[house - 1] ? houses[house - 1] : {};
            var rashi = read(houseData, 'rashi', 'Rashi', '');
            setExistingText(svg, 'h' + house, rashi === '' ? '' : String(rashi));
        }

        var prefixes = ['s', 'c', 'm', 'b', 'g', 'sk', 'sn', 'r', 'k'];
        for (var p = 0; p < prefixes.length; p++) {
            for (var h = 1; h <= 12; h++) clearExistingText(svg, prefixes[p] + h);
        }
        clearExistingText(svg, 'LAG');

        (positions || []).forEach(function (planet) {
            var houseNo = Number(read(planet, 'house', 'House', 0));
            if (houseNo < 1 || houseNo > 12) return;
            var prefix = planetPrefix(read(planet, 'name', 'Name', ''));
            if (!prefix) return;
            // Stack multiple grahas in the same bhava (VB6 style: SUK SUR KET)
            appendPlanetLabel(svg, prefix + houseNo, PLANET_LABELS[prefix]);
        });

        return new XMLSerializer().serializeToString(svg);
    }

    function housesFromLagnaSign(lagnaSign) {
        var lag = Number(lagnaSign) || 1;
        if (lag < 1 || lag > 12) lag = 1;
        var houses = [];
        for (var h = 1; h <= 12; h++) {
            houses.push({ houseNumber: h, rashi: ((lag - 1 + h - 1) % 12) + 1 });
        }
        return houses;
    }

    function positionsFromSignPlanets(planets, lagnaSign) {
        var lag = Number(lagnaSign) || 1;
        if (lag < 1 || lag > 12) lag = 1;
        var out = [];
        (planets || []).forEach(function (p) {
            var name = p.planetName || p.name || p.code || '';
            if (/lagna|asc|^lag$/i.test(String(name))) return;
            var sign = Number(p.sign || p.rashi || 0);
            var house = Number(p.houseFromLagna || p.house || p.chalitHouse || 0);
            if ((!house || house < 1) && sign >= 1 && sign <= 12) {
                house = ((sign - lag + 12) % 12) + 1;
            }
            if (house >= 1 && house <= 12) out.push({ name: name, house: house });
        });
        return out;
    }

    root.generateKundliSVG = function (houses, options) {
        options = options || {};
        return loadTemplate(options.templateUrl).then(function (templateText) {
            return bindTemplate(templateText, houses || [], options.planetPositions || []);
        });
    };

    root.renderNorthIndianChart = function (hostEl, title, lagnaSign, planets, options) {
        if (!hostEl) return Promise.resolve();
        options = options || {};
        var houses = housesFromLagnaSign(lagnaSign);
        var positions = positionsFromSignPlanets(planets, lagnaSign);
        hostEl.innerHTML = '<div class="ni-chart-card"><div class="ni-chart-title">' +
            (title || '') + '</div><div class="ni-chart-svg ni-chart-loading">Loading chart…</div></div>';
        return root.generateKundliSVG(houses, {
            templateUrl: options.templateUrl,
            planetPositions: positions
        }).then(function (svg) {
            hostEl.innerHTML =
                '<div class="ni-chart-card">' +
                '<div class="ni-chart-title">' + (title || '') + '</div>' +
                '<div class="ni-chart-svg">' + svg + '</div></div>';
        }).catch(function (err) {
            hostEl.innerHTML = '<div class="text-danger small">Chart: ' +
                (err && err.message ? err.message : err) + '</div>';
        });
    };

    root.kundliChartHelpers = {
        housesFromLagnaSign: housesFromLagnaSign,
        positionsFromSignPlanets: positionsFromSignPlanets
    };
}(window));
