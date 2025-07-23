window.tokenRefresh = async function () {
    try {
        const response = await fetch('/v1/auth/refresh-token', {
            method: 'POST',
            credentials: 'include'
        });

        return response.ok;
    } catch (err) {
        console.error('Erro no tokenRefresh:', err);
        return false;
    }
};
