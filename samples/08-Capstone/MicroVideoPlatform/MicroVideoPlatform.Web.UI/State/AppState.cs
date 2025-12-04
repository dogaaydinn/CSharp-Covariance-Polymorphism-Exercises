using Blazored.LocalStorage;

namespace MicroVideoPlatform.Web.UI.State;

/// <summary>
/// Global application state management.
/// Handles theme, user preferences, and UI state.
/// </summary>
public class AppState
{
    private readonly ILocalStorageService _localStorage;
    private bool _isDarkMode;
    private string? _currentUserId;

    public event Action? OnChange;

    public AppState(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Gets or sets dark mode state.
    /// </summary>
    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                _ = _localStorage.SetItemAsync("theme", value ? "dark" : "light");
                NotifyStateChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets current user ID.
    /// </summary>
    public string? CurrentUserId
    {
        get => _currentUserId;
        set
        {
            if (_currentUserId != value)
            {
                _currentUserId = value;
                _ = _localStorage.SetItemAsync("userId", value);
                NotifyStateChanged();
            }
        }
    }

    /// <summary>
    /// Gets whether user is authenticated.
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUserId);

    /// <summary>
    /// Initializes state from local storage.
    /// Call this on app startup.
    /// </summary>
    public async Task InitializeAsync()
    {
        var theme = await _localStorage.GetItemAsStringAsync("theme");
        _isDarkMode = theme == "dark";

        _currentUserId = await _localStorage.GetItemAsStringAsync("userId");

        NotifyStateChanged();
    }

    /// <summary>
    /// Toggles dark/light mode.
    /// </summary>
    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
    }

    /// <summary>
    /// Clears all application state.
    /// </summary>
    public async Task ClearAsync()
    {
        _currentUserId = null;
        _isDarkMode = false;
        await _localStorage.ClearAsync();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
