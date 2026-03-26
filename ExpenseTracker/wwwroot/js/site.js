// Expense Tracker JavaScript

// ── Dark / Light Mode ──
(function () {
    var savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
})();

document.addEventListener('DOMContentLoaded', function () {

    // ── Theme toggle ──
    var themeBtn  = document.querySelector('.theme-toggle-btn');
    var themeIcon = themeBtn ? themeBtn.querySelector('i') : null;

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        if (themeIcon) {
            themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }
    }

    // Initialise icon to match persisted theme
    var currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
    if (themeIcon) {
        themeIcon.className = currentTheme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }

    if (themeBtn) {
        themeBtn.addEventListener('click', function () {
            var next = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
            applyTheme(next);
        });
    }

    // ── Auto-dismiss alerts after 5 seconds ──
    document.querySelectorAll('.alert-dismissible').forEach(function (alert) {
        setTimeout(function () {
            bootstrap.Alert.getOrCreateInstance(alert).close();
        }, 5000);
    });

    // ── Default date inputs to today ──
    document.querySelectorAll('input[type="date"]').forEach(function (input) {
        if (!input.value) {
            input.value = new Date().toISOString().split('T')[0];
        }
    });

    // ── Format amount inputs on blur ──
    document.querySelectorAll('input[name="Amount"]').forEach(function (input) {
        input.addEventListener('blur', function () {
            var v = parseFloat(this.value);
            if (!isNaN(v)) this.value = v.toFixed(2);
        });
    });

    // ── Active sidebar link highlighting ──
    var currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar-nav .nav-link').forEach(function (link) {
        var href = link.getAttribute('href');
        if (!href) return;
        var linkPath = href.toLowerCase().split('?')[0];
        // exact match or starts-with match (for sub-pages)
        if (linkPath !== '/' && currentPath.startsWith(linkPath)) {
            link.classList.add('active');
        } else if (linkPath === '/' && currentPath === '/') {
            link.classList.add('active');
        }
    });

    // ── Sidebar toggle (mobile) ──
    var toggleBtn    = document.getElementById('sidebarToggle');
    var sidebar      = document.getElementById('sidebar');
    var overlay      = document.getElementById('sidebarOverlay');

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.toggle('sidebar-open');
            if (overlay) overlay.classList.toggle('active');
        });
    }

    if (overlay) {
        overlay.addEventListener('click', function () {
            if (sidebar) sidebar.classList.remove('sidebar-open');
            overlay.classList.remove('active');
        });
    }

    // Close sidebar when a link is clicked on mobile
    if (sidebar) {
        sidebar.querySelectorAll('.nav-link').forEach(function (link) {
            link.addEventListener('click', function () {
                if (window.innerWidth < 992) {
                    sidebar.classList.remove('sidebar-open');
                    if (overlay) overlay.classList.remove('active');
                }
            });
        });
    }

    // ── Bootstrap tooltips ──
    if (typeof bootstrap !== 'undefined') {
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
            new bootstrap.Tooltip(el);
        });
    }
});

// ── Real-time notification counter ──
(function () {
    var badge = document.getElementById('notif-badge');
    if (!badge) return;

    function updateNotifCount() {
        var el = document.getElementById('notif-badge');
        if (!el) return;
        fetch('/Notifications/GetUnreadCount')
            .then(function (r) { return r.ok ? r.json() : null; })
            .then(function (data) {
                if (!data) return;
                if (data.count > 0) {
                    el.textContent = data.count;
                    el.style.display = '';
                } else {
                    el.style.display = 'none';
                }
            })
            .catch(function (err) { console.error('Failed to fetch notification count:', err); });
    }

    updateNotifCount();
    setInterval(updateNotifCount, 30000);
})();

// Show loading state on form submit
function showLoadingSpinner(formElement) {
    var btn = formElement.querySelector('button[type="submit"]');
    if (btn) {
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Loading...';
        btn.disabled = true;
    }
}
