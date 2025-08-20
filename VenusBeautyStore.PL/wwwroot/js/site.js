// Navegar al hacer click en enlaces del offcanvas *después* de que se cierre
(function () {
    const menu = document.getElementById('vbMenu');
    if (!menu || !window.bootstrap) return;

    menu.querySelectorAll('a.nav-link').forEach(link => {
        link.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (!href || href === '#') return; // ignorar placeholders

            // Si el offcanvas está visible, primero ciérralo y luego navega
            if (menu.classList.contains('show')) {
                e.preventDefault();
                const oc = bootstrap.Offcanvas.getInstance(menu);
                menu.addEventListener('hidden.bs.offcanvas', function onHidden() {
                    menu.removeEventListener('hidden.bs.offcanvas', onHidden);

                    // Enlaces con hash (misma página)
                    if (href.startsWith('#') || href.includes('#')) {
                        // Si viene como /Home/Index#servicios dejamos el hash tal cual
                        window.location.href = href;
                        // scroll suave y compensación del header
                        const hash = href.includes('#') ? '#' + href.split('#')[1] : href;
                        const el = document.querySelector(hash);
                        if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    } else {
                        // Navegación normal a otra página
                        window.location.href = href;
                    }
                }, { once: true });
                if (oc) oc.hide();
            }
        });
    });
})();
