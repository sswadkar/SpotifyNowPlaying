namespace Loupedeck.SpotifyNowPlaying
{
    using System;
    using System.Timers;

    public abstract class SpotifyCommandBase : PluginDynamicCommand
    {
        private String _lastRenderKey;
        private readonly Timer _marqueeTimer;
        private Int32 _marqueeOffset;

        protected SpotifyCommandBase(String displayName, String description)
            : base(displayName: displayName, description: description, groupName: "Spotify")
        {
            this._lastRenderKey = this.GetRenderKey(SpotifyStateCache.Current);
            this._marqueeTimer = new Timer(450);
            this._marqueeTimer.AutoReset = true;
            this._marqueeTimer.Elapsed += this.OnMarqueeTimerElapsed;
            this._marqueeTimer.Start();
            SpotifyStateCache.StateChanged += this.OnStateChanged;
        }

        protected SpotifySnapshot Snapshot => SpotifyStateCache.Current;

        protected override void RunCommand(String actionParameter)
        {
            PluginLog.Info($"{this.DisplayName} refresh requested");
            SpotifyStateCache.RefreshNow();
            this.ActionImageChanged();
        }

        protected String FitLine(String value, Int32 maxLength)
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

        protected String GetScrollingLine(String value, Int32 maxLength)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }

            var normalized = value.Trim().Replace(Environment.NewLine, " ").Replace(" ", "\u00A0");
            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            var restartPrefix = normalized[..Math.Min(3, normalized.Length)];
            var padded = $"{normalized}{new String('.', (maxLength * 4) + 24)}{restartPrefix}";
            if (this._marqueeOffset >= padded.Length)
            {
                this._marqueeOffset = 0;
            }

            var windowChars = new char[maxLength];
            for (var i = 0; i < maxLength; i++)
            {
                windowChars[i] = padded[(this._marqueeOffset + i) % padded.Length];
            }

            return new String(windowChars);
        }

        protected abstract String GetRenderKey(SpotifySnapshot snapshot);

        protected virtual String GetMarqueeSource(SpotifySnapshot snapshot) => null;

        protected virtual Int32? GetMarqueeWindow(SpotifySnapshot snapshot) => null;

        private void OnStateChanged(SpotifySnapshot snapshot)
        {
            var nextKey = this.GetRenderKey(snapshot);
            this._marqueeOffset = 0;
            if (String.Equals(this._lastRenderKey, nextKey, StringComparison.Ordinal))
            {
                return;
            }

            this._lastRenderKey = nextKey;
            this.ActionImageChanged();
        }

        private void OnMarqueeTimerElapsed(Object sender, ElapsedEventArgs e)
        {
            var snapshot = this.Snapshot;
            var source = this.GetMarqueeSource(snapshot);
            var window = this.GetMarqueeWindow(snapshot);
            if (String.IsNullOrWhiteSpace(source) || !window.HasValue)
            {
                return;
            }

            var normalized = source.Trim().Replace(Environment.NewLine, " ");
            if (normalized.Length <= window.Value)
            {
                return;
            }

            this._marqueeOffset++;
            this.ActionImageChanged();
        }
    }
}
