(function () {
    'use strict';

    function buildOptions($select, items, selectedValue, options) {
        var includeEmpty = options && options.includeEmpty;
        var emptyLabel = (options && options.emptyLabel) || 'Select';
        var useNameAsValue = options && options.useNameAsValue;

        $select.empty();
        if (includeEmpty) {
            $select.append('<option value="">' + emptyLabel + '</option>');
        }

        (items || []).forEach(function (item) {
            var id = item.id != null ? item.id : item.Id;
            var name = item.name || item.Name || '';
            var code = item.code || item.Code || '';
            var value = useNameAsValue ? name : id;
            var label = name;
            if (code) {
                label = name + ' (' + code + ')';
            }
            $select.append('<option value="' + String(value) + '">' + $('<span>').text(label).html() + '</option>');
        });

        if (selectedValue != null && String(selectedValue) !== '') {
            $select.val(String(selectedValue));
        }
    }

    function loadCategoryTypes(selectId, selectedValue, options) {
        var $select = $(selectId);
        return $.get('/Security/CategoryTypes/GetCategoryTypeOptions', function (response) {
            var items = response && response.data ? response.data : [];
            buildOptions($select, items, selectedValue, options);
        });
    }

    function loadCategoriesByType(selectId, typeName, selectedValue, options) {
        var $select = $(selectId);
        var url = '/Security/Categories/GetCategoryOptions?typeName=' + encodeURIComponent(typeName || '');
        return $.get(url, function (response) {
            var items = response && response.data ? response.data : [];
            if ((!items || items.length === 0) && options && options.fallback) {
                buildOptions($select, options.fallback, selectedValue, options);
            } else {
                buildOptions($select, items, selectedValue, options);
            }
        });
    }

    window.SBCategoryDropdowns = {
        loadCategoryTypes: loadCategoryTypes,
        loadCategoriesByType: loadCategoriesByType
    };
})();
