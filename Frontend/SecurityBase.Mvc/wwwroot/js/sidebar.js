// Sidebar/menu behavior (active state, collapse persistence, submenu toggles)
(function () {
    'use strict';

    function getSidebar() { return document.getElementById('sidebar'); }
    function getOverlay() { return document.getElementById('sidebar-overlay'); }

    function isMobile() { return window.innerWidth <= 1024; }

    function normalizePath(path) {
        if (!path) return '/';
        var p = String(path).split('?')[0].split('#')[0];
        if (p.length > 1 && p.endsWith('/')) p = p.slice(0, -1);
        return p;
    }

    function getHrefPath(anchor) {
        if (!anchor) return null;
        var href = anchor.getAttribute('href');
        if (!href || href === '#' || href === 'javascript:void(0)') return null;
        try {
            return normalizePath(new URL(anchor.href, window.location.origin).pathname);
        } catch {
            return normalizePath(href);
        }
    }

    function clearMenuActive() {
        document.querySelectorAll('#sidebar .menu-item.active').forEach(function (el) {
            el.classList.remove('active');
        });
        document.querySelectorAll('#sidebar .submenu.show').forEach(function (el) {
            el.classList.remove('show');
        });
        document.querySelectorAll('#sidebar .menu-item.expanded').forEach(function (el) {
            el.classList.remove('expanded');
        });
    }

    function expandParentIfNeeded(menuItem) {
        if (!menuItem) return;
        var parentMenuId = menuItem.getAttribute('data-parent');
        if (!parentMenuId) return;

        var parentSubmenu = document.getElementById(parentMenuId);
        var parentMenuItem = document.querySelector('[data-menu-id="' + parentMenuId + '"]');
        if (parentSubmenu && parentMenuItem) {
            parentSubmenu.classList.add('show');
            parentMenuItem.classList.add('expanded');
            parentMenuItem.classList.add('active');
        }
    }

    function setActiveMenu() {
        var currentPath = normalizePath(window.location.pathname);
        var menuItems = Array.prototype.slice.call(document.querySelectorAll('#sidebar a.menu-item'));

        clearMenuActive();

        var foundExactMatch = false;
        menuItems.forEach(function (item) {
            var hrefPath = getHrefPath(item);
            if (!hrefPath) return;

            if (currentPath === hrefPath) {
                item.classList.add('active');
                foundExactMatch = true;
                expandParentIfNeeded(item);
            }
        });

        if (foundExactMatch) return;

        // Fallback for /Controller/Create, /Controller/Edit/{id}, /Controller/Details/{id}
        var parts = currentPath.split('/').filter(function (p) { return !!p; });
        if (parts.length >= 1) {
            var controller = parts[0];
            var action = parts.length >= 2 ? parts[1] : 'Index';
            var actionLower = String(action || '').toLowerCase();

            if (actionLower === 'create' || actionLower === 'edit' || actionLower === 'details') {
                var indexPath = normalizePath('/' + controller + '/Index');
                menuItems.forEach(function (item) {
                    var hrefPath = getHrefPath(item);
                    if (hrefPath && hrefPath === indexPath) {
                        item.classList.add('active');
                        expandParentIfNeeded(item);
                    }
                });
            }
        }
    }

    function persistCollapsedState(collapsed) {
        try {
            localStorage.setItem('sb_sidebar_collapsed', collapsed ? '1' : '0');
        } catch { }
    }

    function restoreCollapsedState() {
        if (isMobile()) return;
        var sidebar = getSidebar();
        if (!sidebar) return;
        try {
            var v = localStorage.getItem('sb_sidebar_collapsed');
            if (v === '1') sidebar.classList.add('collapsed');
        } catch { }
    }

    window.toggleSubMenu = function (menuId) {
        var submenu = document.getElementById(menuId);
        var parentMenu = document.querySelector('[data-menu-id="' + menuId + '"]');

        if (!submenu || !parentMenu) return;

        var isExpanded = submenu.classList.contains('show');

        document.querySelectorAll('#sidebar .submenu.show').forEach(function (sm) {
            if (sm.id === menuId) return;
            sm.classList.remove('show');
            var pm = document.querySelector('[data-menu-id="' + sm.id + '"]');
            if (pm) pm.classList.remove('expanded');
        });

        if (isExpanded) {
            submenu.classList.remove('show');
            parentMenu.classList.remove('expanded');
        } else {
            submenu.classList.add('show');
            parentMenu.classList.add('expanded');
        }
    };

    window.toggleSidebar = function () {
        var sidebar = getSidebar();
        var overlay = getOverlay();
        if (!sidebar) return;

        if (isMobile()) {
            sidebar.classList.toggle('mobile-open');
            if (overlay) overlay.classList.toggle('active');
        } else {
            sidebar.classList.toggle('collapsed');
            persistCollapsedState(sidebar.classList.contains('collapsed'));
        }
    };

    window.closeSidebar = function () {
        var sidebar = getSidebar();
        var overlay = getOverlay();
        if (!sidebar) return;

        if (isMobile()) {
            sidebar.classList.remove('mobile-open');
            if (overlay) overlay.classList.remove('active');
        }
    };

    function wireCloseOnNavigation() {
        var sidebar = getSidebar();
        if (!sidebar) return;
        sidebar.addEventListener('click', function (e) {
            var target = e.target;
            if (!target) return;
            var a = target.closest ? target.closest('a.menu-item') : null;
            if (!a) return;
            var href = a.getAttribute('href');
            if (!href || href === '#' || href === 'javascript:void(0)') return;
            if (isMobile()) window.closeSidebar();
        });
    }

    window.addEventListener('resize', function () {
        // On breakpoint changes: ensure mobile overlay is cleaned up.
        if (!isMobile()) {
            var overlay = getOverlay();
            if (overlay) overlay.classList.remove('active');
            var sidebar = getSidebar();
            if (sidebar) sidebar.classList.remove('mobile-open');
            restoreCollapsedState();
        }
    });

    document.addEventListener('DOMContentLoaded', function () {
        restoreCollapsedState();
        setActiveMenu();
        wireCloseOnNavigation();
    });
})();

