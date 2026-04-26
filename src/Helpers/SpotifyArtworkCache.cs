namespace Loupedeck.SpotifyNowPlaying
{
    using System;
    using System.Net.Http;

    internal static class SpotifyArtworkCache
    {
        private static readonly Object Sync = new();
        private static readonly HttpClient HttpClient = new();
        private static String _cachedUrl = String.Empty;
        private static Byte[] _cachedBytes;

        public static Byte[] GetArtworkBytes(String artworkUrl)
        {
            if (String.IsNullOrWhiteSpace(artworkUrl))
            {
                return null;
            }

            lock (Sync)
            {
                if (String.Equals(_cachedUrl, artworkUrl, StringComparison.Ordinal) && _cachedBytes != null)
                {
                    return _cachedBytes;
                }
            }

            try
            {
                var bytes = HttpClient.GetByteArrayAsync(artworkUrl).GetAwaiter().GetResult();
                lock (Sync)
                {
                    _cachedUrl = artworkUrl;
                    _cachedBytes = bytes;
                }

                return bytes;
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, $"Failed to fetch Spotify artwork: {artworkUrl}");
                return null;
            }
        }

        public static void Warm(String artworkUrl)
        {
            if (String.IsNullOrWhiteSpace(artworkUrl))
            {
                return;
            }

            lock (Sync)
            {
                if (String.Equals(_cachedUrl, artworkUrl, StringComparison.Ordinal) && _cachedBytes != null)
                {
                    return;
                }
            }

            _ = GetArtworkBytes(artworkUrl);
        }
    }
}
