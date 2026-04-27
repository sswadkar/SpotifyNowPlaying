namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    internal static class SpotifyTileRenderer
    {
        private static readonly BitmapColor Background = new(18, 18, 18);
        private static readonly BitmapColor Secondary = new(168, 168, 168);
        private static readonly BitmapColor Primary = new(255, 255, 255);
        private static readonly BitmapColor SpotifyGreen = new(29, 185, 84);
        private static readonly BitmapColor SpotifyGreenMuted = new(48, 232, 114);
        private static readonly BitmapColor Overlay = new(0, 0, 0, 160);
        private static readonly BitmapColor ProgressBack = new(60, 60, 60);
        private static readonly BitmapColor SoftPanel = new(26, 26, 26);
        private static readonly BitmapColor SoftPanel2 = new(35, 35, 35);

        public static BitmapImage RenderAlbumArtTile(SpotifySnapshot snapshot, PluginImageSize imageSize)
        {
            var artBytes = SpotifyArtworkCache.GetArtworkBytes(snapshot.ArtworkUrl);
            if (artBytes != null)
            {
                using var artBuilder = new BitmapBuilder(imageSize);
                artBuilder.Clear(Background);
                artBuilder.DrawImage(artBytes, 0, 0, artBuilder.Width, artBuilder.Height, default);
                return artBuilder.ToImage();
            }

            using var builder = new BitmapBuilder(imageSize);
            builder.Clear(Background);

            var width = builder.Width;
            var height = builder.Height;
            builder.FillRectangle(8, 8, width - 16, height - 16, SoftPanel);
            builder.FillRectangle(8, 8, width - 16, 4, SpotifyGreen);
            builder.DrawText("Spotify", 12, 24, width - 24, 14, Secondary, 11, 12, 0, null);
            builder.DrawText(
                snapshot.HasTrackData ? Clamp(snapshot.Title, 18) : Clamp(snapshot.StatusTitle, 18),
                12,
                38,
                width - 24,
                18,
                Primary,
                16,
                18,
                0,
                null);
            builder.DrawText(
                snapshot.HasTrackData ? Clamp(snapshot.Artist, 18) : Clamp(snapshot.StatusDetail, 18),
                12,
                58,
                width - 24,
                14,
                Secondary,
                11,
                12,
                0,
                null);
            return builder.ToImage();
        }

        public static BitmapImage RenderProgressTile(SpotifySnapshot snapshot, PluginImageSize imageSize)
        {
            using var builder = new BitmapBuilder(imageSize);
            builder.Clear(Background);

            var width = builder.Width;
            var height = builder.Height;
            if (!snapshot.HasTrackData)
            {
                builder.FillRectangle(14, 18, width - 28, height - 36, SoftPanel);
                builder.FillRectangle(14, 18, width - 28, 3, SpotifyGreen);
                builder.DrawText(snapshot.StatusDetail, 18, 36, width - 36, 14, Secondary, 11, 12, 0, null);
                return builder.ToImage();
            }

            builder.FillRectangle(14, 14, width - 28, height - 28, SoftPanel);
            builder.FillRectangle(14, 14, width - 28, 3, SpotifyGreen);
            builder.DrawText(snapshot.PositionText, 18, 22, width - 36, 18, Primary, 18, 19, 0, null);
            builder.DrawText(snapshot.DurationText, 18, 39, width - 36, 11, Secondary, 10, 10, 0, null);
            builder.FillRectangle(18, height - 23, width - 36, 6, SoftPanel2);
            builder.FillRectangle(18, height - 23, Math.Max(2, (Int32)Math.Round((width - 36) * snapshot.ProgressRatio)), 6, SpotifyGreen);
            return builder.ToImage();
        }

        public static BitmapImage RenderVolumeTile(SpotifySnapshot snapshot, PluginImageSize imageSize)
        {
            using var builder = new BitmapBuilder(imageSize);
            builder.Clear(Background);

            var width = builder.Width;
            var height = builder.Height;
            builder.FillRectangle(14, 14, width - 28, height - 28, SoftPanel);

            if (!snapshot.HasTrackData && String.Equals(snapshot.StatusDetail, "Closed", StringComparison.OrdinalIgnoreCase))
            {
                builder.DrawText("Closed", 14, 46, width - 28, 14, Secondary, 12, 12, 0, null);
                return builder.ToImage();
            }

            builder.DrawText($"{snapshot.SoundVolume}%", 10, 38, width - 20, 24, SpotifyGreenMuted, 28, 28, 0, null);
            return builder.ToImage();
        }

        private static String Clamp(String value, Int32 maxLength)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }

            var normalized = value.Trim().Replace(Environment.NewLine, " ");
            return normalized.Length <= maxLength
                ? normalized
                : $"{normalized[..(maxLength - 1)]}…";
        }
    }
}
