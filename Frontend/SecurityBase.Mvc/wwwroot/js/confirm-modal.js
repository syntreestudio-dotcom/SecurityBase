/* Shared confirmation dialog + modal dirty guard */
(function () {
    'use strict';

    function getOverlay() {
        return document.getElementById('sbConfirmOverlay');
    }

    function setOverlayHidden(hidden) {
        var overlay = getOverlay();
        if (!overlay) return;
        overlay.hidden = !!hidden;
        document.body.classList.toggle('sb-confirm-open', !hidden);
    }

    function setDialogContent(title, message, okText, cancelText, tone) {
        var titleEl = document.getElementById('sbConfirmTitle');
        var messageEl = document.getElementById('sbConfirmMessage');
        if (titleEl) titleEl.textContent = title || 'Confirm';
        if (messageEl) messageEl.textContent = message || 'Are you sure?';

        var okBtn = document.querySelector('[data-sb-confirm-ok]');
        var cancelBtn = document.querySelector('[data-sb-confirm-cancel]');

        if (okBtn) okBtn.textContent = okText || 'OK';
        if (cancelBtn) cancelBtn.textContent = cancelText || 'Cancel';

        if (okBtn) {
            okBtn.classList.toggle('sb-confirm-btn-danger', (tone || 'danger') === 'danger');
            okBtn.classList.toggle('sb-confirm-btn-secondary', (tone || 'danger') === 'secondary');
        }
    }

    function once(el, eventName, handler) {
        var wrapped = function (e) {
            el.removeEventListener(eventName, wrapped);
            handler(e);
        };
        el.addEventListener(eventName, wrapped);
    }

    function trapEscape(onCancel) {
        function onKeydown(e) {
            if (e.key === 'Escape') {
                e.preventDefault();
                onCancel();
            }
        }
        document.addEventListener('keydown', onKeydown);
        return function () { document.removeEventListener('keydown', onKeydown); };
    }

    window.SBConfirm = {
        open: function (opts) {
            opts = opts || {};
            return new Promise(function (resolve) {
                var overlay = getOverlay();
                if (!overlay) {
                    // Fallback if partial wasn't rendered for some reason.
                    resolve(window.confirm(opts.message || 'Are you sure?'));
                    return;
                }

                setDialogContent(opts.title, opts.message, opts.okText, opts.cancelText, opts.tone);
                setOverlayHidden(false);

                var cleanupEscape = trapEscape(function () { resolve(false); setOverlayHidden(true); });

                var okBtn = overlay.querySelector('[data-sb-confirm-ok]');
                var cancelBtns = overlay.querySelectorAll('[data-sb-confirm-cancel]');

                if (okBtn) {
                    once(okBtn, 'click', function () {
                        cleanupEscape();
                        resolve(true);
                        setOverlayHidden(true);
                    });
                }

                if (cancelBtns && cancelBtns.length) {
                    Array.prototype.forEach.call(cancelBtns, function (btn) {
                        once(btn, 'click', function () {
                            cleanupEscape();
                            resolve(false);
                            setOverlayHidden(true);
                        });
                    });
                }
            });
        }
    };

    function snapshotForm(formEl) {
        if (!formEl) return '';
        var parts = [];
        var fields = formEl.querySelectorAll('input, select, textarea');
        Array.prototype.forEach.call(fields, function (field) {
            if (!field || !field.name) return;
            if (field.disabled) return;

            var type = (field.type || '').toLowerCase();
            if (type === 'button' || type === 'submit' || type === 'reset' || type === 'file') return;

            if (type === 'checkbox') {
                parts.push(field.name + '=' + (field.checked ? '1' : '0'));
                return;
            }

            if (type === 'radio') {
                if (field.checked) parts.push(field.name + '=' + String(field.value || ''));
                return;
            }

            parts.push(field.name + '=' + String(field.value || ''));
        });
        parts.sort();
        return parts.join('&');
    }

    function getBsModal(modalEl) {
        if (!modalEl) return null;
        if (window.bootstrap && window.bootstrap.Modal && window.bootstrap.Modal.getOrCreateInstance) {
            return window.bootstrap.Modal.getOrCreateInstance(modalEl);
        }
        return null;
    }

    var guardStateById = {};

    window.SBModalDirtyGuard = {
        attach: function (cfg) {
            cfg = cfg || {};
            var modalId = cfg.modalId;
            var formId = cfg.formId;
            if (!modalId || !formId) return;

            var modalEl = document.getElementById(modalId);
            var formEl = document.getElementById(formId);
            if (!modalEl || !formEl) return;

            if (guardStateById[modalId] && guardStateById[modalId].attached) return;

            var state = {
                attached: true,
                modalEl: modalEl,
                formEl: formEl,
                cleanSnapshot: '',
                allowNextHide: false
            };
            guardStateById[modalId] = state;

            modalEl.addEventListener('shown.bs.modal', function () {
                state.cleanSnapshot = snapshotForm(formEl);
                state.allowNextHide = false;
            });

            function isDirty() {
                if (!state.cleanSnapshot) {
                    state.cleanSnapshot = snapshotForm(formEl);
                    return false;
                }
                return snapshotForm(formEl) !== state.cleanSnapshot;
            }

            function hideModalSafely() {
                state.allowNextHide = true;
                var bs = getBsModal(modalEl);
                if (bs) bs.hide();
                else modalEl.classList.remove('show');
            }

            // Custom close buttons (X / Cancel) use our guard explicitly.
            var closeButtons = modalEl.querySelectorAll('[data-sb-modal-close]');
            if (closeButtons && closeButtons.length) {
                Array.prototype.forEach.call(closeButtons, function (btn) {
                    btn.addEventListener('click', function (ev) {
                        if (ev && ev.preventDefault) ev.preventDefault();
                        if (!isDirty()) {
                            hideModalSafely();
                            return;
                        }

                        window.SBConfirm.open({
                            title: 'Discard changes?',
                            message: 'You have unsaved changes. Do you want to discard them and close?',
                            okText: 'Discard',
                            cancelText: 'Keep editing',
                            tone: 'danger'
                        }).then(function (ok) {
                            if (!ok) return;
                            hideModalSafely();
                        });
                    });
                });
            }

            modalEl.addEventListener('hide.bs.modal', function (e) {
                if (state.allowNextHide) return;

                if (!isDirty()) return;

                e.preventDefault();

                window.SBConfirm.open({
                    title: 'Discard changes?',
                    message: 'You have unsaved changes. Do you want to discard them and close?',
                    okText: 'Discard',
                    cancelText: 'Keep editing',
                    tone: 'danger'
                }).then(function (ok) {
                    if (!ok) return;
                    hideModalSafely();
                });
            });
        },

        allowCloseOnce: function (modalId) {
            var s = guardStateById[modalId];
            if (!s) return;
            s.allowNextHide = true;
        },

        markClean: function (modalId) {
            var s = guardStateById[modalId];
            if (!s) return;
            s.cleanSnapshot = snapshotForm(s.formEl);
        }
    };
})();
