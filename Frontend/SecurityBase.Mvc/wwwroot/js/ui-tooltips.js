// Bootstrap tooltip initialization (for info icons and other tooltip triggers)
(function () {
    'use strict';

    function initTooltips(root) {
        if (!window.bootstrap || !window.bootstrap.Tooltip) return;
        (root || document).querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
            // Avoid double init
            if (el._sbTooltipInitialized) return;
            el._sbTooltipInitialized = true;
            new bootstrap.Tooltip(el, { trigger: 'hover focus', container: 'body' });
        });
    }

    document.addEventListener('DOMContentLoaded', function () { initTooltips(document); });

    // Re-init when a modal is shown (in case any content is injected later)
    document.addEventListener('shown.bs.modal', function (e) { initTooltips(e.target); });
})();

