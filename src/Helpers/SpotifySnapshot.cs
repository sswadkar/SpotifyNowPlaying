namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    public sealed class SpotifySnapshot
    {
        public static SpotifySnapshot State(String title, String detail) =>
            new()
            {
                StatusTitle = title ?? "Spotify",
                StatusDetail = detail ?? String.Empty,
            };

        public String Title { get; init; } = String.Empty;

        public String Artist { get; init; } = String.Empty;

        public String ArtworkUrl { get; init; } = String.Empty;

        public Double DurationSeconds { get; init; }

        public Double PositionSeconds { get; init; }

        public Boolean IsPlaying { get; init; }

        public String StatusTitle { get; init; } = "Spotify";

        public String StatusDetail { get; init; } = "Closed";

        public Boolean HasTrackData => !String.IsNullOrWhiteSpace(this.Title);

        public Double ProgressRatio =>
            this.DurationSeconds > 0
                ? Math.Clamp(this.PositionSeconds / this.DurationSeconds, 0.0, 1.0)
                : 0.0;

        public String TrackKey =>
            this.HasTrackData
                ? $"{this.Title}|{this.Artist}|{this.ArtworkUrl}"
                : $"{this.StatusTitle}|{this.StatusDetail}";

        public Int32 ProgressBucket => (Int32)Math.Round(this.PositionSeconds);

        public String DurationText => this.FormatSeconds(this.DurationSeconds);

        public String PositionText => this.FormatSeconds(this.PositionSeconds);

        private String FormatSeconds(Double value)
        {
            var duration = TimeSpan.FromSeconds(Math.Max(0, value));
            return duration.TotalHours >= 1
                ? duration.ToString(@"h\:mm\:ss")
                : duration.ToString(@"m\:ss");
        }
    }
}
