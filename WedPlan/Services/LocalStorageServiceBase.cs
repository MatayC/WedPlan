using Blazored.LocalStorage;

namespace WedPlan.Services;

/// <summary>
/// Generische Basisklasse für Services, die eine Liste von Entitäten im
/// Browser-LocalStorage persistieren. Kapselt das Laden/Speichern und hält
/// die Daten zusätzlich in-memory vor.
///
/// Diese Basis ist bewusst einfach gehalten, damit die konkreten Services
/// später leicht auf eine echte Datenbank (EF Core / SQLite) umgestellt
/// werden können – die öffentliche API der abgeleiteten Services bleibt gleich.
/// </summary>
/// <typeparam name="T">Der Entitätstyp.</typeparam>
public abstract class LocalStorageServiceBase<T>
{
    private readonly ILocalStorageService _localStorage;

    /// <summary>Schlüssel, unter dem die Daten im LocalStorage abgelegt werden.</summary>
    protected abstract string StorageKey { get; }

    /// <summary>In-Memory-Cache der Entitäten.</summary>
    protected List<T> Items { get; private set; } = new();

    /// <summary>Gibt an, ob die Daten bereits aus dem LocalStorage geladen wurden.</summary>
    private bool _isLoaded;

    protected LocalStorageServiceBase(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Stellt sicher, dass die Daten aus dem LocalStorage geladen sind.
    /// Muss vor jedem Zugriff aufgerufen werden. Der LocalStorage-Zugriff ist
    /// erst nach dem ersten Render (interaktiv) verfügbar.
    /// </summary>
    protected async Task EnsureLoadedAsync()
    {
        if (_isLoaded)
        {
            return;
        }

        if (await _localStorage.ContainKeyAsync(StorageKey))
        {
            Items = await _localStorage.GetItemAsync<List<T>>(StorageKey) ?? new List<T>();
        }
        else
        {
            Items = new List<T>();
        }

        _isLoaded = true;
    }

    /// <summary>
    /// Persistiert den aktuellen In-Memory-Zustand in den LocalStorage.
    /// </summary>
    protected async Task SaveAsync()
    {
        await _localStorage.SetItemAsync(StorageKey, Items);
    }
}
