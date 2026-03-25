/* Lightweight form validation utilities (client-side) */
(function () {
    'use strict';

    function qs(root, selector) {
        return (root || document).querySelector(selector);
    }

    function qsa(root, selector) {
        return Array.prototype.slice.call((root || document).querySelectorAll(selector));
    }

    function closestFieldContainer(el) {
        if (!el) return null;
        return el.closest('.input-with-icon') || el.closest('.form-group') || el.parentElement;
    }

    function clearErrors(formEl) {
        if (!formEl) return;
        qsa(formEl, '.is-invalid').forEach(function (el) { el.classList.remove('is-invalid'); });
        qsa(formEl, '.sb-invalid-feedback').forEach(function (el) { el.remove(); });
    }

    function setError(inputEl, message) {
        if (!inputEl) return;
        inputEl.classList.add('is-invalid');

        var container = closestFieldContainer(inputEl) || inputEl;
        var feedback = document.createElement('div');
        feedback.className = 'sb-invalid-feedback';
        feedback.textContent = message || 'Invalid value.';

        // Put feedback after the input or container.
        if (container === inputEl) {
            inputEl.insertAdjacentElement('afterend', feedback);
        } else {
            container.insertAdjacentElement('afterend', feedback);
        }
    }

    function isEmpty(value) {
        return value == null || String(value).trim() === '';
    }

    function isEmail(value) {
        if (isEmpty(value)) return false;
        // Simple, practical email check
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(value).trim());
    }

    function isInt(value) {
        if (isEmpty(value)) return false;
        return /^-?\d+$/.test(String(value).trim());
    }

    function parseIntSafe(value) {
        var n = parseInt(String(value), 10);
        return isNaN(n) ? null : n;
    }

    function passwordStrength(pwd) {
        pwd = String(pwd || '');
        var hasLower = /[a-z]/.test(pwd);
        var hasUpper = /[A-Z]/.test(pwd);
        var hasDigit = /\d/.test(pwd);
        var hasSpecial = /[^A-Za-z0-9]/.test(pwd);
        var hasSpace = /\s/.test(pwd);
        var len = pwd.length;

        var score = 0;
        if (len >= 8) score++;
        if (hasLower && hasUpper) score++;
        if (hasDigit) score++;
        if (hasSpecial) score++;
        if (len >= 12) score++;

        // normalize to 0..4
        score = Math.max(0, Math.min(4, score - (hasSpace ? 2 : 0)));

        var ok = len >= 8 && hasLower && hasUpper && hasDigit && hasSpecial && !hasSpace;
        return {
            ok: ok,
            score: score,
            rules: {
                len: len >= 8,
                lower: hasLower,
                upper: hasUpper,
                digit: hasDigit,
                special: hasSpecial,
                space: !hasSpace
            }
        };
    }

    function updatePasswordMeter(inputEl, barEl, textEl) {
        if (!inputEl || !barEl) return;
        var s = passwordStrength(inputEl.value || '');
        var pct = [0, 25, 50, 75, 100][s.score] || 0;
        barEl.style.width = pct + '%';
        barEl.dataset.score = String(s.score);

        if (textEl) {
            if (isEmpty(inputEl.value)) {
                textEl.textContent = 'Use 8+ chars with upper/lower/number/symbol (no spaces).';
            } else if (s.ok) {
                textEl.textContent = 'Strong password.';
            } else {
                textEl.textContent = 'Password must be 8+ with upper, lower, number, symbol (no spaces).';
            }
        }
    }

    function focusFirstInvalid(formEl) {
        if (!formEl) return;
        var first = qs(formEl, '.is-invalid');
        if (first && typeof first.focus === 'function') first.focus();
    }

    function validateUserForm() {
        var formEl = document.getElementById('userForm');
        if (!formEl) return true;

        clearErrors(formEl);

        var userIdEl = document.getElementById('userId');
        var usernameEl = document.getElementById('username');
        var emailEl = document.getElementById('email');
        var passwordEl = document.getElementById('passwordHash');
        var passwordGroup = document.getElementById('passwordGroup');

        var isCreate = !userIdEl || String(userIdEl.value || '0') === '0';

        // Username
        var username = (usernameEl && usernameEl.value) ? String(usernameEl.value).trim() : '';
        if (isEmpty(username)) {
            setError(usernameEl, 'Username is required.');
        } else if (username.length < 3 || username.length > 30) {
            setError(usernameEl, 'Username must be 3–30 characters.');
        } else if (!/^[a-zA-Z0-9._-]+$/.test(username)) {
            setError(usernameEl, 'Use letters, numbers, dot, underscore or hyphen only.');
        }

        // Email
        var email = (emailEl && emailEl.value) ? String(emailEl.value).trim() : '';
        if (isEmpty(email)) {
            setError(emailEl, 'Email is required.');
        } else if (!isEmail(email)) {
            setError(emailEl, 'Enter a valid email address.');
        }

        // Password (required in create; optional in edit but if entered must be strong)
        var passwordVisible = !!(passwordGroup && passwordGroup.offsetParent !== null);
        var passwordVal = (passwordEl && passwordEl.value) ? String(passwordEl.value) : '';
        if (isCreate && passwordVisible) {
            if (isEmpty(passwordVal)) {
                setError(passwordEl, 'Password is required for new users.');
            } else {
                var st = passwordStrength(passwordVal);
                if (!st.ok) setError(passwordEl, 'Password is too weak.');
            }
        } else if (!isEmpty(passwordVal)) {
            var st2 = passwordStrength(passwordVal);
            if (!st2.ok) setError(passwordEl, 'Password is too weak.');
        }

        var ok = qsa(formEl, '.is-invalid').length === 0;
        if (!ok) focusFirstInvalid(formEl);
        return ok;
    }

    function validateRoleForm() {
        var formEl = document.getElementById('roleForm');
        if (!formEl) return true;
        clearErrors(formEl);

        var roleNameEl = document.getElementById('roleName');
        var descEl = document.getElementById('description');

        var roleName = (roleNameEl && roleNameEl.value) ? String(roleNameEl.value).trim() : '';
        if (isEmpty(roleName)) {
            setError(roleNameEl, 'Role name is required.');
        } else if (roleName.length < 2 || roleName.length > 60) {
            setError(roleNameEl, 'Role name must be 2–60 characters.');
        }

        var desc = (descEl && descEl.value) ? String(descEl.value).trim() : '';
        if (!isEmpty(desc) && desc.length > 250) {
            setError(descEl, 'Description must be 250 characters or less.');
        }

        var ok = qsa(formEl, '.is-invalid').length === 0;
        if (!ok) focusFirstInvalid(formEl);
        return ok;
    }

    function validateMenuForm() {
        var formEl = document.getElementById('menuForm');
        if (!formEl) return true;
        clearErrors(formEl);

        var menuNameEl = document.getElementById('menuName');
        var routeEl = document.getElementById('route');
        var iconEl = document.getElementById('icon');
        var displayOrderEl = document.getElementById('displayOrder');
        var parentMenuIdEl = document.getElementById('parentMenuId');
        var menuTypeEl = document.getElementById('menuType');

        var menuName = (menuNameEl && menuNameEl.value) ? String(menuNameEl.value).trim() : '';
        if (isEmpty(menuName)) {
            setError(menuNameEl, 'Menu name is required.');
        } else if (menuName.length < 2 || menuName.length > 60) {
            setError(menuNameEl, 'Menu name must be 2–60 characters.');
        }

        var route = (routeEl && routeEl.value) ? String(routeEl.value).trim() : '';
        if (!isEmpty(route) && route[0] !== '/') {
            setError(routeEl, 'Route should start with “/” (example: /Users/Index).');
        } else if (!isEmpty(route) && route.length > 120) {
            setError(routeEl, 'Route is too long (max 120).');
        }

        var icon = (iconEl && iconEl.value) ? String(iconEl.value).trim() : '';
        if (!isEmpty(icon) && icon.length > 80) {
            setError(iconEl, 'Icon class is too long (max 80).');
        }

        var displayOrderRaw = displayOrderEl ? displayOrderEl.value : '';
        if (!isEmpty(displayOrderRaw)) {
            if (!isInt(displayOrderRaw)) {
                setError(displayOrderEl, 'Display order must be an integer.');
            } else {
                var n = parseIntSafe(displayOrderRaw);
                if (n == null || n < 0 || n > 9999) setError(displayOrderEl, 'Display order must be between 0 and 9999.');
            }
        }

        var menuType = (menuTypeEl && menuTypeEl.value) ? String(menuTypeEl.value) : 'Parent';
        var parentRaw = parentMenuIdEl ? parentMenuIdEl.value : '';
        if (menuType === 'Child') {
            if (isEmpty(parentRaw)) {
                setError(parentMenuIdEl, 'Parent menu is required for child menus.');
            } else if (!isInt(parentRaw)) {
                setError(parentMenuIdEl, 'Parent menu selection is invalid.');
            }
        } else if (!isEmpty(parentRaw)) {
            // If user manually entered, still validate integer (should normally be empty for Parent)
            if (!isInt(parentRaw)) setError(parentMenuIdEl, 'Parent menu selection is invalid.');
        }

        var ok = qsa(formEl, '.is-invalid').length === 0;
        if (!ok) focusFirstInvalid(formEl);
        return ok;
    }

    window.SBFormValidation = {
        clearErrors: clearErrors,
        validateUserForm: validateUserForm,
        validateRoleForm: validateRoleForm,
        validateMenuForm: validateMenuForm,
        passwordStrength: passwordStrength,
        updatePasswordMeter: updatePasswordMeter
    };
})();
