window.enableResize = (id) => {
    const el = document.getElementById(id);
    if (!el) {
        console.warn("Elemento não encontrado:", id);
        return;
    }

    el.style.position = 'relative';
    el.style.resize = 'both';
    el.style.overflow = 'auto';
    el.style.minWidth = '200px';
    el.style.minHeight = '150px';
    el.style.maxWidth = '100%';
    el.style.maxHeight = '90vh';
};
