namespace Loupedeck.SpotifyNowPlaying
{
    using SkiaSharp;

    internal static class SpotifyVolumeRenderer
    {
        private static readonly SKColor Background = new(18, 18, 18);
        private static readonly SKColor Panel = new(26, 26, 26);
        private static readonly SKColor Accent = new(48, 232, 114);
        private static readonly SKColor Muted = new(205, 205, 205);

        public static BitmapImage Render(SpotifySnapshot snapshot, PluginImageSize imageSize)
        {
            var width = imageSize.GetButtonWidth();
            var height = imageSize.GetButtonHeight();

            using var builder = new ImageBuilder(width, height);
            builder.Clear(Background);
            builder.FillRectangle(14, 14, width - 28, height - 28, Panel);

            if (!snapshot.HasTrackData && snapshot.StatusDetail == "Closed")
            {
                builder.DrawHorizontallyCenteredText("Closed", 12, Muted, 46);
                return builder.ToBitmapImage();
            }

            builder.DrawHorizontallyCenteredText($"{snapshot.SoundVolume}%", 24, Accent, 40);
            builder.DrawHorizontallyCenteredText("Spotify Volume", 12, Muted, 70);
            return builder.ToBitmapImage();
        }
    }
}
