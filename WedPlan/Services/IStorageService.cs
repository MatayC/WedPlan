namespace WedPlan.Services;

/// <summary>Identifiziert einen der Bild-Plätze einer Hochzeit.</summary>
public enum ImageSlot
{
    /// <summary>Großes Titelbild (Hero-Hintergrund auf dem Dashboard).</summary>
    Cover,
    /// <summary>Erstes dezentes Dashboard-Bild.</summary>
    Gallery1,
    /// <summary>Zweites dezentes Dashboard-Bild.</summary>
    Gallery2
}

/// <summary>
/// Kapselt das Hochladen und Löschen von Bildern (Titel-/Dashboard-Bilder) in Supabase Storage.
/// Jeder <see cref="ImageSlot"/> wird unabhängig verwaltet.
/// </summary>
public interface IStorageService
{
    /// <summary>Maximale erlaubte Dateigröße für Bilder in Bytes (5 MB).</summary>
    long MaxCoverImageBytes { get; }

    /// <summary>Erlaubte MIME-Typen für Bilder.</summary>
    IReadOnlyList<string> AllowedImageContentTypes { get; }

    /// <summary>
    /// Lädt ein Bild für den angegebenen Slot einer Hochzeit hoch und liefert die öffentliche URL.
    /// Ein bereits vorhandenes Bild desselben Slots wird ersetzt.
    /// </summary>
    Task<(bool Success, string? Url, string? Error)> UploadImageAsync(
        Guid weddingId, ImageSlot slot, byte[] content, string contentType, string fileExtension);

    /// <summary>Entfernt das Bild des angegebenen Slots (falls vorhanden).</summary>
    Task DeleteImageAsync(Guid weddingId, ImageSlot slot);
}
