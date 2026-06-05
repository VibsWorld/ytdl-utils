using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using YtdlOn24Downloader.Properties;

namespace YtdlOn24Downloader
{
    public partial class MainWindow : Window
    {
        // Defensive paste sanitization: strip CR/LF that some apps smuggle into clipboard data, plus Trim.
        private static string Normalize(string? s) => s?.Trim().Replace("\r", "").Replace("\n", "") ?? string.Empty;

        private readonly List<RobustnessOption> _options = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BuildOptionsList();
        }

        private void BuildOptionsList()
        {
            _options.Add(new RobustnessOption(ChkRetryInfinite,         "--retries infinite",           () => Settings.Default.RetryInfinite,           v => Settings.Default.RetryInfinite = v));
            _options.Add(new RobustnessOption(ChkFragmentRetryInfinite, "--fragment-retries infinite",  () => Settings.Default.FragmentRetryInfinite,   v => Settings.Default.FragmentRetryInfinite = v));
            _options.Add(new RobustnessOption(ChkFileAccessRetries,     "--file-access-retries 10",     () => Settings.Default.FileAccessRetries,       v => Settings.Default.FileAccessRetries = v));
            _options.Add(new RobustnessOption(ChkContinuePartial,       "--continue",                   () => Settings.Default.ContinuePartial,         v => Settings.Default.ContinuePartial = v));
            _options.Add(new RobustnessOption(ChkPartFile,              "--part",                       () => Settings.Default.PartFile,                v => Settings.Default.PartFile = v));
            _options.Add(new RobustnessOption(ChkSkipUnavailableFragments, "--skip-unavailable-fragments", () => Settings.Default.SkipUnavailableFragments, v => Settings.Default.SkipUnavailableFragments = v));
            _options.Add(new RobustnessOption(ChkHlsUseMpegts,          "--hls-use-mpegts",             () => Settings.Default.HlsUseMpegts,            v => Settings.Default.HlsUseMpegts = v));
            _options.Add(new RobustnessOption(ChkSocketTimeout,         "--socket-timeout 30",          () => Settings.Default.SocketTimeout,           v => Settings.Default.SocketTimeout = v));
            _options.Add(new RobustnessOption(ChkThrottledRate,         "--throttled-rate 100K",        () => Settings.Default.ThrottledRate,           v => Settings.Default.ThrottledRate = v));
            _options.Add(new RobustnessOption(ChkNoAbortOnError,        "--no-abort-on-error",          () => Settings.Default.NoAbortOnError,          v => Settings.Default.NoAbortOnError = v));
            _options.Add(new RobustnessOption(ChkConcurrentFragments,   "--concurrent-fragments 3",     () => Settings.Default.ConcurrentFragments,     v => Settings.Default.ConcurrentFragments = v));
            _options.Add(new RobustnessOption(ChkShutdownOnFinish,      "--shutdown",                  () => Settings.Default.ShutdownOnFinish,        v => Settings.Default.ShutdownOnFinish = v));
        }

        private void LoadOptions()
        {
            foreach (var o in _options)
                o.Control.IsChecked = o.Get();
        }

        private void SaveOptions()
        {
            foreach (var o in _options)
                o.Set(o.Control.IsChecked == true);
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            var s = Settings.Default;

            TxtUrl.Text = s.LastUrl;
            TxtCookiesPath.Text = s.CookiesPath;
            TxtYtdlpPath.Text = s.YtdlpPath;
            TxtFormat.Text = s.FormatString;
            TxtOutputDir.Text = s.OutputDirectory;

            LoadOptions();
            UpdatePreview();
            BtnExecute.IsEnabled = true;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var s = Settings.Default;

            s.LastUrl = TxtUrl.Text;
            s.CookiesPath = TxtCookiesPath.Text;
            s.YtdlpPath = TxtYtdlpPath.Text;
            s.FormatString = TxtFormat.Text;
            s.OutputDirectory = TxtOutputDir.Text;

            SaveOptions();

            s.Save();
        }

        private void OnInputChanged(object? sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            TxtCommandPreview.Text = BuildCommand();
        }

        // Note: the output folder is set as ProcessStartInfo.WorkingDirectory, not as a -o arg, so yt-dlp's default filename rules still apply.
        private string BuildCommand()
        {
            var sb = new StringBuilder();

            string ytdlp = string.IsNullOrWhiteSpace(TxtYtdlpPath.Text) ? "yt-dlp" : QuoteArgument(Normalize(TxtYtdlpPath.Text));
            sb.Append(ytdlp);

            foreach (var o in _options)
            {
                if (o.Control.IsChecked == true)
                {
                    sb.Append(' ').Append(o.Flag);
                }
            }

            string cookies = Normalize(TxtCookiesPath.Text);
            if (!string.IsNullOrEmpty(cookies))
            {
                sb.Append($" --cookies {QuoteArgument(cookies)}");
            }

            string format = Normalize(TxtFormat.Text);
            if (!string.IsNullOrEmpty(format))
            {
                sb.Append($" -f {QuoteArgument(format)}");
            }

            string url = Normalize(TxtUrl.Text);
            if (!string.IsNullOrEmpty(url))
            {
                sb.Append($" {QuoteArgument(url)}");
            }

            if (ChkShutdownOnFinish.IsChecked == true)
            {
                // Chain via && so the shutdown only fires if yt-dlp exits cleanly.
                // timeout gives the user a visible "Press CTRL+C to cancel" prompt in the CMD window;
                // shutdown /t 60 also surfaces a system-tray balloon; either way `shutdown /a` aborts.
                sb.Append(" && timeout /t 60 && shutdown /s /t 60 /c \"yt-dlp On24 download finished\"");
            }

            return sb.ToString();
        }

        // Quotes a single argument for cmd.exe: doubles embedded quotes, escapes trailing backslashes before a quote, wraps in "...".
        // See https://docs.microsoft.com/en-us/cpp/cpp/maintaining-command-line-arguments-quotation and CommandLineToArgvW rules.
        private static string QuoteArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return argument;

            if (argument.IndexOfAny(new[] { ' ', '\t', '"', '&', '|', '<', '>', '^', '(', ')' }) < 0)
                return argument;

            var sb = new StringBuilder(argument.Length + 2);
            int backslashes = 0;
            sb.Append('"');
            foreach (char c in argument)
            {
                if (c == '\\')
                {
                    backslashes++;
                }
                else if (c == '"')
                {
                    sb.Append('\\', backslashes * 2 + 1);
                    sb.Append('"');
                    backslashes = 0;
                }
                else
                {
                    if (backslashes > 0)
                    {
                        sb.Append('\\', backslashes);
                        backslashes = 0;
                    }
                    sb.Append(c);
                }
            }
            sb.Append('\\', backslashes * 2);
            sb.Append('"');
            return sb.ToString();
        }

        private void BtnBrowseCookies_Click(object? sender, RoutedEventArgs e)
        {
            BrowseForFile(TxtCookiesPath, "Select cookies.txt", "Text files (*.txt)|*.txt|All files (*.*)|*.*");
        }

        private void BtnBrowseYtdlp_Click(object? sender, RoutedEventArgs e)
        {
            BrowseForFile(TxtYtdlpPath, "Select yt-dlp executable", "Executable files (*.exe)|*.exe|All files (*.*)|*.*");
        }

        private void BtnBrowseOutput_Click(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog
            {
                Title = "Select output folder",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                TxtOutputDir.Text = dlg.FolderName;
            }
        }

        private static void BrowseForFile(TextBox target, string title, string filter)
        {
            var dlg = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                target.Text = dlg.FileName;
            }
        }

        private void BtnExecute_Click(object? sender, RoutedEventArgs e)
        {
            TxtStatus.Foreground = Brushes.DarkRed;

            string url = Normalize(TxtUrl.Text);
            if (string.IsNullOrEmpty(url))
            {
                TxtStatus.Text = "Error: URL is required.";
                return;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? parsedUri) ||
                (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
            {
                TxtStatus.Text = "Error: URL does not appear to be a valid HTTP/HTTPS link.";
                return;
            }

            string cookies = Normalize(TxtCookiesPath.Text);
            if (string.IsNullOrEmpty(cookies))
            {
                TxtStatus.Text = "Error: Please select a cookies file.";
                return;
            }

            if (!File.Exists(cookies))
            {
                TxtStatus.Text = $"Error: Cookies file not found: {cookies}";
                return;
            }

            string ytdlpPath = Normalize(TxtYtdlpPath.Text);
            if (!string.IsNullOrEmpty(ytdlpPath) && !File.Exists(ytdlpPath))
            {
                TxtStatus.Text = $"Error: yt-dlp executable not found: {ytdlpPath}";
                return;
            }

            string outputDir = Normalize(TxtOutputDir.Text);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                TxtStatus.Text = $"Error: Output folder does not exist: {outputDir}";
                return;
            }

            string command = BuildCommand();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k {command}",
                    UseShellExecute = true,
                    WorkingDirectory = string.IsNullOrEmpty(outputDir)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                        : outputDir
                };

                Process.Start(psi);
                BtnExecute.IsEnabled = false;

                TxtStatus.Foreground = Brushes.DarkGreen;
                TxtStatus.Text = "Download process launched. Check the new CMD window for progress.";
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Failed to launch process: {ex.Message}";
            }
        }

        private sealed record RobustnessOption(CheckBox Control, string Flag, Func<bool> Get, Action<bool> Set);
    }
}
