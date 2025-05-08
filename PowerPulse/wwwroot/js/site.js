// Переключение темы
window.toggleTheme = () => {
    console.log('Theme set to:', localStorage.getItem('theme'));
    const currentTheme = localStorage.getItem('theme') || 'light';
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    localStorage.setItem('theme', newTheme);
    return setTheme(); // Возвращаем новое состояние темы
};

// Функция для получения текущей темы
window.getTheme = () => {
    console.log('Theme set to:', localStorage.getItem('theme'));
    return localStorage.getItem('theme') || 'light';
};

// Функция для установки темы
function setTheme() {
    const theme = localStorage.getItem('theme') || 'light';
    console.log('Applying theme:', theme);
    if (theme === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
}

// Принудительное обновление темы
window.updateTheme = () => {
    setTheme();
};

window.downloadFile = (fileName, mimeType, base64String) => {
    const linkSource = `data:${mimeType};base64,${base64String}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
};