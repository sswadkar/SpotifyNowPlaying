namespace Loupedeck.SpotifyNowPlaying
{
    using System;
    using System.Diagnostics;
    using System.Text.Json;

    internal static class SpotifyAppleScriptReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private const String Script = """
            var spotify = Application("/Applications/Spotify.app");
            try {
                if (!spotify.running()) {
                    JSON.stringify({ statusTitle: "Spotify", statusDetail: "Closed" });
                } else {
                    var state = spotify.playerState ? String(spotify.playerState()) : "";
                    var track = spotify.currentTrack();
                    if (state === "stopped") {
                        JSON.stringify({ statusTitle: "Spotify", statusDetail: "Stopped" });
                    } else {
                        JSON.stringify({
                            title: track ? String(track.name() || "") : "",
                            artist: track ? String(track.artist() || "") : "",
                            artworkUrl: track && track.artworkUrl ? String(track.artworkUrl() || "") : "",
                            durationSeconds: track && track.duration ? Number(track.duration() || 0) : 0,
                            positionSeconds: spotify.playerPosition ? Number(spotify.playerPosition() || 0) : 0,
                            isPlaying: state === "playing",
                            statusTitle: "Spotify",
                            statusDetail: state === "paused" ? "Paused" : "Playing"
                        });
                    }
                }
            } catch (e) {
                JSON.stringify({
                    statusTitle: String(e).toLowerCase().indexOf("authorize") >= 0 ? "Grant" : "Spotify",
                    statusDetail: String(e).toLowerCase().indexOf("authorize") >= 0 ? "Spotify Access" : String(e)
                });
            }
            """;

        public static SpotifySnapshot GetCurrentTrack()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/osascript",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            startInfo.ArgumentList.Add("-l");
            startInfo.ArgumentList.Add("JavaScript");
            startInfo.ArgumentList.Add("-e");
            startInfo.ArgumentList.Add(Script);

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return SpotifySnapshot.State("Spotify", "Script failed");
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            var error = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                PluginLog.Warning($"osascript exited with code {process.ExitCode}: {error}");
                return SpotifySnapshot.State("Spotify", "Script error");
            }

            if (String.IsNullOrWhiteSpace(output))
            {
                return SpotifySnapshot.State("Spotify", "Unavailable");
            }

            try
            {
                var payload = JsonSerializer.Deserialize<SpotifyScriptPayload>(output, JsonOptions);
                if (payload == null)
                {
                    return SpotifySnapshot.State("Spotify", "Unavailable");
                }

                if (!String.IsNullOrWhiteSpace(payload.Title))
                {
                    return new SpotifySnapshot
                    {
                        Title = payload.Title,
                        Artist = payload.Artist ?? String.Empty,
                        ArtworkUrl = payload.ArtworkUrl ?? String.Empty,
                        DurationSeconds = Math.Max(0, payload.DurationSeconds),
                        PositionSeconds = Math.Max(0, payload.PositionSeconds),
                        IsPlaying = payload.IsPlaying,
                        StatusTitle = payload.StatusTitle ?? "Spotify",
                        StatusDetail = payload.StatusDetail ?? (payload.IsPlaying ? "Playing" : "Paused"),
                    };
                }

                return SpotifySnapshot.State(
                    payload.StatusTitle ?? "Spotify",
                    payload.StatusDetail ?? "Unavailable");
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, $"Failed to parse Spotify script output: {output}");
                return SpotifySnapshot.State("Spotify", "Parse error");
            }
        }

        private sealed class SpotifyScriptPayload
        {
            public String Title { get; set; }

            public String Artist { get; set; }

            public String ArtworkUrl { get; set; }

            public Double DurationSeconds { get; set; }

            public Double PositionSeconds { get; set; }

            public Boolean IsPlaying { get; set; }

            public String StatusTitle { get; set; }

            public String StatusDetail { get; set; }
        }
    }
}
