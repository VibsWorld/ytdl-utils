# YtdlOn24Downloader

A simple Windows WPF UI for [yt-dlp](https://github.com/yt-dlp/yt-dlp) tailored for downloading long event recordings from the **On24** platform (and any other yt-dlp-compatible stream).

## Why this exists

Downloading 4–5 hour event streams over a slow or unstable connection is painful from the command line. This app lets you paste the event URL, pick your `cookies.txt`, toggle proven robustness flags, and launch yt-dlp in a visible CMD window so you can monitor progress.

## Requirements

- Windows 10 or 11
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet) (or SDK to build)
- [yt-dlp](https://github.com/yt-dlp/yt-dlp/releases) installed and available on your `PATH` (or browse to the `.exe` in the app)

## Build

```bash
dotnet build YtdlOn24Downloader.sln
```

To publish a self-contained single-file executable:

```bash
dotnet publish YtdlOn24Downloader/YtdlOn24Downloader.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

## Usage

1. **Paste the full event URL** into the *Event / Stream URL* box.
2. **Browse** for your `cookies.txt` file.
3. (Optional) **Browse** for `yt-dlp.exe` if it is not on your system `PATH`.
4. The **Format String** defaults to `bestvideo+bestaudio/best`. Change it if needed.
5. Toggle **Robustness Options** checkboxes. All are enabled by default for slow/unstable connections.
6. Review the **Generated Command** preview.
7. Press **Execute**. A new CMD window opens running yt-dlp. The window stays open after finish (`/k`) so you can scroll back and inspect logs.

## Robustness Options Explained

| Flag | Why it helps on slow/unstable connections |
|---|---|
| `--retries infinite` | Never gives up when a fatal network error occurs. |
| `--fragment-retries infinite` | Retries individual DASH/HLS fragments forever — critical for long streams. |
| `--file-access-retries 10` | Tolerates transient file-system locks. |
| `--continue` | Resumes partially downloaded files if the transfer is interrupted. |
| `--part` | Writes data to a `.part` file first so an incomplete write never corrupts the final output. |
| `--skip-unavailable-fragments` | Skips missing stream segments instead of killing the entire 5-hour job. |
| `--hls-use-mpegts` | Uses MPEG-TS container for HLS, which is more resilient to interruption than MP4. |
| `--socket-timeout 30` | Waits 30 seconds before giving up on a stalled socket. |
| `--throttled-rate 100K` | If speed drops below ~100 KB/s, yt-dlp re-extracts the stream to work around server-side throttling. |
| `--no-abort-on-error` | Continues instead of stopping the entire run when a single error occurs. |
| `--concurrent-fragments 3` | Downloads 3 fragments in parallel for better throughput without overwhelming slow bandwidth. |

All options are exposed as individual checkboxes so you can fine-tune them per download.

## Settings Persistence

The app remembers across sessions:
- Last used URL, cookies path, and yt-dlp path
- Format string
- State of every robustness checkbox

Settings are stored in Windows user-local application data (`%LOCALAPPDATA%`).

## License

MIT — use at your own risk. Respect the terms of service of the platforms you download from.
