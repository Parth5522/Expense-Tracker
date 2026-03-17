// Expense Tracker JavaScript

document.addEventListener('DOMContentLoaded', function () {

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

// Show loading state on form submit
function showLoadingSpinner(formElement) {
    var btn = formElement.querySelector('button[type="submit"]');
    if (btn) {
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Loading...';
        btn.disabled = true;
    }
}
