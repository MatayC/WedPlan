using Supabase.Storage;
using FileOptions = Supabase.Storage.FileOptions;

namespace WedPlan.Services;

/// <summary>
/// Supabase-Storage-Implementierung von <see cref="IStorageService"/>.
/// Titelbilder werden im öffentlichen Bucket "wedding-covers" abgelegt,
/// Dateiname pro Hochzeit, sodass ein erneuter Upload das alte Bild ersetzt.
/// </summary>
public class StorageService : IStorageService
{
    private const string Bucket = "wedding-covers";

    private readonly SupabaseClientProvider _provider;

    public StorageService(SupabaseClientProvider provider)
    {
        _provider = provider;
    }

    public long MaxCoverImageBytes => 5 * 1024 * 1024;

    public IReadOnlyList<string> AllowedImageContentTypes { get; } = new[]
    {
        "image/jpeg", "image/png", "image/webp"
    };

    /// <summary>Liefert das Dateinamen-Präfix eines Bild-Slots.</summary>
    private static string SlotPrefix(ImageSlot slot) => slot switch
    {
        ImageSlot.Cover => "cover",
        ImageSlot.Gallery1 => "gallery1",
        ImageSlot.Gallery2 => "gallery2",
        _ => "cover"
    };

    public async Task<(bool Success, string? Url, string? Error)> UploadImageAsync(
        Guid weddingId, ImageSlot slot, byte[] content, string contentType, string fileExtension)
    {
        try
        {
            if (content.Length == 0)
            {
                return (false, null, "Die Datei ist leer.");
            }

            if (content.Length > MaxCoverImageBytes)
            {
                return (false, null, "Das Bild ist zu groß (max. 5 MB).");
            }

            if (!AllowedImageContentTypes.Contains(contentType))
            {
                return (false, null, "Nur JPG-, PNG- oder WebP-Bilder sind erlaubt.");
            }

            var client = await _provider.GetClientAsync();
            var storage = client.Storage.From(Bucket);

            // Fester Slot-Präfix pro Hochzeit + Cache-Buster über Zeitstempel im Dateinamen.
            var prefix = SlotPrefix(slot);
            var ext = string.IsNullOrWhiteSpace(fileExtension) ? "jpg" : fileExtension.TrimStart('.');
            var path = $"{weddingId}/{prefix}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.{ext}";

            // Nur das alte Bild desselben Slots entfernen, damit die anderen Slots erhalten bleiben.
            await DeleteImageAsync(weddingId, slot);

            await storage.Upload(content, path, new FileOptions
            {
                ContentType = contentType,
                Upsert = true,
                CacheControl = "3600"
            });

            var url = storage.GetPublicUrl(path);
            return (true, url, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task DeleteImageAsync(Guid weddingId, ImageSlot slot)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var storage = client.Storage.From(Bucket);

            var prefix = SlotPrefix(slot) + "_";
            var existing = await storage.List($"{weddingId}");
            if (existing is { Count: > 0 })
            {
                var paths = existing
                    .Where(f => !string.IsNullOrEmpty(f.Name) && f.Name!.StartsWith(prefix, StringComparison.Ordinal))
                    .Select(f => $"{weddingId}/{f.Name}")
                    .ToList();

                if (paths.Count > 0)
                {
                    await storage.Remove(paths);
                }
            }
        }
        catch
        {
            // Löschen ist "best effort" – Fehler hier dürfen den Upload nicht blockieren.
        }
    }
}
