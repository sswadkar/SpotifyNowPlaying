namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    public class AlbumArtCommand : SpotifyCommandBase
    {
        public AlbumArtCommand()
            : base(displayName: "Now Playing", description: "Shows the current Spotify album art with the song title")
        {
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            SpotifyTileRenderer.RenderAlbumArtTile(this.Snapshot, imageSize);

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            var snapshot = this.Snapshot;
            if (snapshot.HasTrackData)
            {
                return this.GetScrollingLine(snapshot.Title, 12);
            }

            return $"{this.FitLine(snapshot.StatusTitle, 14)}{Environment.NewLine}{this.FitLine(snapshot.StatusDetail, 14)}";
        }

        protected override String GetRenderKey(SpotifySnapshot snapshot) =>
            $"{snapshot.ArtworkUrl}|{snapshot.TrackKey}|{snapshot.ProgressBucket}";

        protected override String GetMarqueeSource(SpotifySnapshot snapshot) => snapshot.HasTrackData ? snapshot.Title : null;

        protected override Int32? GetMarqueeWindow(SpotifySnapshot snapshot) => snapshot.HasTrackData ? 12 : null;
    }
}
