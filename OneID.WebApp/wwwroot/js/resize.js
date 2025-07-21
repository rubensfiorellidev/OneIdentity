window.enableResize = (id) => {
    setTimeout(() => {
        const el = document.getElementById(id);
        if (!el) {
            console.warn("[Resize] Elemento não encontrado:", id);
            return;
        }

        el.style.position = 'relative';
        el.style.resize = 'both';
        el.style.overflow = 'auto';
        el.style.minWidth = '280px';
        el.style.minHeight = 'auto'; // deixa o conteúdo decidir
        el.style.maxWidth = '100%';
        el.style.maxHeight = '90vh';

        // Força layout reflow
        setTimeout(() => {
            window.dispatchEvent(new Event('resize'));
        }, 100);
    }, 150);
};
