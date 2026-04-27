# Spotify Now Playing

Logi Options+ plugin for MX Creative Console that shows Spotify album art, the current song title, and the current Spotify volume.

## Features

- `Now Playing` action under the `Spotify` group
- `Volume` action under the `Spotify` group
- Reads Spotify on macOS through `osascript`
- Uses album art for the key image
- Scrolls long song titles in the caption area

## Build

```bash
dotnet build
```

## Install

Building creates a Logi plugin link at:

`~/Library/Application Support/Logi/LogiPluginService/Plugins/SpotifyNowPlaying.link`

Then in Logi Options+:

1. Open your MX Creative Keypad
2. Go to `Installed Plugins`
3. Add `Spotify Now Playing`
4. Assign `Now Playing`
