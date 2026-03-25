(function () {
  'use strict';

  function ensureContainer() {
    var el = document.getElementById('sbToastContainer');
    if (!el) {
      el = document.createElement('div');
      el.id = 'sbToastContainer';
      el.className = 'sb-toast-container position-fixed top-0 end-0 p-3';
      el.setAttribute('aria-live', 'polite');
      el.setAttribute('aria-atomic', 'true');
      document.body.appendChild(el);
    }
    return el;
  }

  function ensureConfirmContainer() {
    var el = document.getElementById('sbConfirmContainer');
    if (!el) {
      el = document.createElement('div');
      el.id = 'sbConfirmContainer';
      el.className = 'sb-confirm-container position-fixed top-50 start-50 translate-middle p-3';
      el.setAttribute('aria-live', 'polite');
      el.setAttribute('aria-atomic', 'true');
      document.body.appendChild(el);
    }
    return el;
  }

  function ensureConfirmBackdrop() {
    var el = document.getElementById('sbConfirmBackdrop');
    if (!el) {
      el = document.createElement('div');
      el.id = 'sbConfirmBackdrop';
      el.className = 'sb-confirm-backdrop';
      document.body.appendChild(el);
    }
    return el;
  }

  function iconHtml(kind) {
    var map = {
      success: 'fa-circle-check',
      info: 'fa-circle-info',
      warning: 'fa-triangle-exclamation',
      error: 'fa-circle-xmark',
      confirm: 'fa-circle-question'
    };
    var cls = map[kind] || map.info;
    return '<i class="fas ' + cls + ' sb-toast-icon sb-toast-icon-' + kind + '"></i>';
  }

  function sanitize(text) {
    var div = document.createElement('div');
    div.textContent = String(text ?? '');
    return div.innerHTML;
  }

  function buildToast(options) {
    var kind = options.kind || 'info';
    var title = options.title || (kind === 'error' ? 'Error' : kind === 'warning' ? 'Warning' : kind === 'success' ? 'Success' : kind === 'confirm' ? 'Confirm' : 'Info');
    var message = options.message || '';
    var autohide = options.autohide !== false;
    var delay = typeof options.delay === 'number' ? options.delay : (kind === 'error' || kind === 'warning' ? 5000 : 3500);

    var wrapper = document.createElement('div');
    wrapper.className = 'toast sb-toast sb-toast-' + kind;
    wrapper.role = 'alert';
    wrapper.ariaLive = 'assertive';
    wrapper.ariaAtomic = 'true';
    wrapper.dataset.bsAutohide = autohide ? 'true' : 'false';
    wrapper.dataset.bsDelay = String(delay);

    var header = document.createElement('div');
    header.className = 'toast-header sb-toast-header';
    header.innerHTML = iconHtml(kind) + '<strong class="me-auto sb-toast-title">' + sanitize(title) + '</strong>' +
      '<button type="button" class="btn-close ms-2 mb-1" data-bs-dismiss="toast" aria-label="Close"></button>';

    var body = document.createElement('div');
    body.className = 'toast-body sb-toast-body';
    body.innerHTML = '<div class="sb-toast-message">' + sanitize(message) + '</div>';

    wrapper.appendChild(header);
    wrapper.appendChild(body);

    return { wrapper: wrapper, body: body };
  }

  function show(options) {
    var container = ensureContainer();
    var built = buildToast(options || {});
    container.appendChild(built.wrapper);

    var toast = bootstrap.Toast.getOrCreateInstance(built.wrapper);
    built.wrapper.addEventListener('hidden.bs.toast', function () {
      try { built.wrapper.remove(); } catch { }
    }, { once: true });
    toast.show();
    return toast;
  }

  function confirm(options) {
    return new Promise(function (resolve) {
      var opts = options || {};
      var variant = opts.variant || 'default'; // default | danger
      var confirmText = opts.confirmText || (variant === 'danger' ? 'Delete' : 'Yes');
      var cancelText = opts.cancelText || 'Cancel';

      var container = ensureConfirmContainer();
      var backdrop = ensureConfirmBackdrop();
      backdrop.classList.add('active');

      var built = buildToast({
        kind: 'confirm',
        title: opts.title || 'Confirm',
        message: opts.message || 'Are you sure?',
        autohide: false
      });

      if (variant === 'danger') built.wrapper.classList.add('sb-toast-confirm-danger');

      var actions = document.createElement('div');
      actions.className = 'sb-toast-actions mt-2 d-flex gap-2 justify-content-end';
      actions.innerHTML =
        '<button type="button" class="btn btn-sm btn-outline-secondary sb-toast-cancel">' + sanitize(cancelText) + '</button>' +
        '<button type="button" class="btn btn-sm ' + (variant === 'danger' ? 'btn-danger' : 'btn-primary') + ' sb-toast-ok">' + sanitize(confirmText) + '</button>';

      built.body.appendChild(actions);
      container.appendChild(built.wrapper);

      var toast = bootstrap.Toast.getOrCreateInstance(built.wrapper, { autohide: false });

      function cleanup(val) {
        built.wrapper.removeEventListener('hidden.bs.toast', onHidden);
        try { toast.hide(); } catch { }
        backdrop.classList.remove('active');
        resolve(val);
      }

      function onHidden() {
        try { built.wrapper.remove(); } catch { }
        backdrop.classList.remove('active');
      }

      built.wrapper.addEventListener('hidden.bs.toast', onHidden, { once: true });

      built.wrapper.querySelector('.sb-toast-cancel').addEventListener('click', function () { cleanup(false); });
      built.wrapper.querySelector('.sb-toast-ok').addEventListener('click', function () { cleanup(true); });
      built.wrapper.querySelector('.btn-close').addEventListener('click', function () { cleanup(false); });

      toast.show();
      setTimeout(function () {
        try { built.wrapper.querySelector('.sb-toast-ok').focus(); } catch { }
      }, 50);
    });
  }

  window.SBToast = {
    show: show,
    success: function (message, title) { return show({ kind: 'success', title: title, message: message }); },
    info: function (message, title) { return show({ kind: 'info', title: title, message: message }); },
    warning: function (message, title) { return show({ kind: 'warning', title: title, message: message }); },
    error: function (message, title) { return show({ kind: 'error', title: title, message: message, delay: 6000 }); },
    confirm: confirm
  };
})();
