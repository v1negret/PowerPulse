using Blazored.LocalStorage;

namespace PowerPulse.Services;

public class ThemeService
{
    private readonly ILocalStorageService _localStorage;
    private bool _isDarkTheme;
    public event Action OnThemeChanged;

    public ThemeService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        private set
        {
            _isDarkTheme = value;
            OnThemeChanged?.Invoke();
        }
    }

    public async Task InitializeAsync()
    {
        var savedTheme = await _localStorage.GetItemAsync<string>("theme");
        IsDarkTheme = savedTheme == "dark";
    }

    public async Task ToggleThemeAsync()
    {
        IsDarkTheme = !IsDarkTheme;
        await _localStorage.SetItemAsync("theme", IsDarkTheme ? "dark" : "light");
        OnThemeChanged?.Invoke();
    }
}