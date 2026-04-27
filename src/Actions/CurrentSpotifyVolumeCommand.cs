namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    public class CurrentSpotifyVolumeCommand : SpotifyCommandBase
    {
        public CurrentSpotifyVolumeCommand()
            : base(displayName: "Current Spotify Volume", description: "Shows the current Spotify volume")
        {
            this.IsWidget = true;
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            SpotifyVolumeRenderer.Render(this.Snapshot, imageSize);

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => " ";

        protected override String GetRenderKey(SpotifySnapshot snapshot) =>
            $"{snapshot.SoundVolume}|{snapshot.StatusTitle}|{snapshot.StatusDetail}";
    }
}
