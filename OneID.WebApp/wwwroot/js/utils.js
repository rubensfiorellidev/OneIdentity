window.OneID = {
    copyToClipboard: (text) => navigator.clipboard.writeText(text),
    setCookie: (name, value, minutes) => {
        const expires = new Date(Date.now() + minutes * 60000).toUTCString();
        document.cookie = `${name}=${value}; expires=${expires}; path=/; Secure; SameSite=Strict`;
    }
};

window.dashboardState = {
    saveExpandState: function (isExpanded) {
        localStorage.setItem("dashboard_expand", isExpanded);
    },
    getExpandState: function () {
        return localStorage.getItem("dashboard_expand") === "true";
    }
};

document.addEventListener('click', function () {
    if (window.DotNet) {
        DotNet.invokeMethodAsync('OneID.WebApp', 'CloseLauncher');
    }
});
