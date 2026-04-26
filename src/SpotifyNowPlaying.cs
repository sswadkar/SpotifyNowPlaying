namespace Loupedeck.SpotifyNowPlaying
{
    using System;

    public class SpotifyNowPlaying : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public SpotifyNowPlaying()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            SpotifyStateCache.Start();
        }

        public override void Unload()
        {
            SpotifyStateCache.Stop();
        }
    }
}
