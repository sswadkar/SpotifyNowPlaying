namespace Loupedeck.SpotifyNowPlaying
{
    using System;
    using System.Timers;

    internal static class SpotifyStateCache
    {
        private static readonly Object Sync = new();
        private static readonly Timer RefreshTimer = new(250) { AutoReset = true };
        private static SpotifySnapshot _current = SpotifySnapshot.State("Spotify", "Loading");
        private static Boolean _started;

        static SpotifyStateCache()
        {
            RefreshTimer.Elapsed += (_, _) => RefreshCore();
        }

        public static event Action<SpotifySnapshot> StateChanged;

        public static SpotifySnapshot Current
        {
            get
            {
                lock (Sync)
                {
                    return _current;
                }
            }
        }

        public static void Start()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            RefreshCore();
            RefreshTimer.Start();
        }

        public static void Stop() => RefreshTimer.Stop();

        public static void RefreshNow() => RefreshCore();

        private static void RefreshCore()
        {
            SpotifySnapshot snapshot;
            try
            {
                snapshot = SpotifyAppleScriptReader.GetCurrentTrack();
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Failed to refresh Spotify state");
                snapshot = SpotifySnapshot.State("Spotify", "Error");
            }

            SpotifyArtworkCache.Warm(snapshot.ArtworkUrl);

            String previousKey;
            String nextKey = CreateSnapshotKey(snapshot);

            lock (Sync)
            {
                previousKey = CreateSnapshotKey(_current);
                _current = snapshot;
            }

            if (!String.Equals(previousKey, nextKey, StringComparison.Ordinal))
            {
                StateChanged?.Invoke(snapshot);
            }
        }

        private static String CreateSnapshotKey(SpotifySnapshot snapshot) =>
            $"{snapshot.TrackKey}|{snapshot.ProgressBucket}|{snapshot.DurationSeconds:0}|{snapshot.SoundVolume}|{snapshot.StatusTitle}|{snapshot.StatusDetail}|{snapshot.IsPlaying}";
    }
}
