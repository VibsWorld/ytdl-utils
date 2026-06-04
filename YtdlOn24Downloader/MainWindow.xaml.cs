using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using YtdlOn24Downloader.Properties;

namespace YtdlOn24Downloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var s = Settings.Default;

            TxtUrl.Text = s.LastUrl;
            TxtCookiesPath.Text = s.CookiesPath;
            TxtYtdlpPath.Text = s.YtdlpPath;
            TxtFormat.Text = string.IsNullOrWhiteSpace(s.FormatString) ? "bestvideo+bestaudio/best" : s.FormatString;

            ChkRetryInfinite.IsChecked = s.RetryInfinite;
            ChkFragmentRetryInfinite.IsChecked = s.FragmentRetryInfinite;
            ChkFileAccessRetries.IsChecked = s.FileAccessRetries;
            ChkContinuePartial.IsChecked = s.ContinuePartial;
            ChkPartFile.IsChecked = s.PartFile;
            ChkSkipUnavailableFragments.IsChecked = s.SkipUnavailableFragments;
            ChkHlsUseMpegts.IsChecked = s.HlsUseMpegts;
            ChkSocketTimeout.IsChecked = s.SocketTimeout;
            ChkThrottledRate.IsChecked = s.ThrottledRate;
            ChkNoAbortOnError.IsChecked = s.NoAbortOnError;
            ChkConcurrentFragments.IsChecked = s.ConcurrentFragments;

            UpdatePreview();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var s = Settings.Default;

            s.LastUrl = TxtUrl.Text;
            s.CookiesPath = TxtCookiesPath.Text;
            s.YtdlpPath = TxtYtdlpPath.Text;
            s.FormatString = TxtFormat.Text;

            s.RetryInfinite = ChkRetryInfinite.IsChecked ?? true;
            s.FragmentRetryInfinite = ChkFragmentRetryInfinite.IsChecked ?? true;
            s.FileAccessRetries = ChkFileAccessRetries.IsChecked ?? true;
            s.ContinuePartial = ChkContinuePartial.IsChecked ?? true;
            s.PartFile = ChkPartFile.IsChecked ?? true;
            s.SkipUnavailableFragments = ChkSkipUnavailableFragments.IsChecked ?? true;
            s.HlsUseMpegts = ChkHlsUseMpegts.IsChecked ?? true;
            s.SocketTimeout = ChkSocketTimeout.IsChecked ?? true;
            s.ThrottledRate = ChkThrottledRate.IsChecked ?? true;
            s.NoAbortOnError = ChkNoAbortOnError.IsChecked ?? true;
            s.ConcurrentFragments = ChkConcurrentFragments.IsChecked ?? true;

            s.Save();
        }

        private void OnInputChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            TxtCommandPreview.Text = BuildCommand();
            TxtStatus.Text = string.Empty;
        }

        private string BuildCommand()
        {
            var sb = new StringBuilder();

            string ytdlp = string.IsNullOrWhiteSpace(TxtYtdlpPath.Text) ? "yt-dlp" : QuoteArgument(TxtYtdlpPath.Text.Trim().Replace("\r", "").Replace("\n", ""));
            sb.Append(ytdlp);

            AppendIfChecked(sb, ChkRetryInfinite, "--retries infinite");
            AppendIfChecked(sb, ChkFragmentRetryInfinite, "--fragment-retries infinite");
            AppendIfChecked(sb, ChkFileAccessRetries, "--file-access-retries 10");
            AppendIfChecked(sb, ChkContinuePartial, "--continue");
            AppendIfChecked(sb, ChkPartFile, "--part");
            AppendIfChecked(sb, ChkSkipUnavailableFragments, "--skip-unavailable-fragments");
            AppendIfChecked(sb, ChkHlsUseMpegts, "--hls-use-mpegts");
            AppendIfChecked(sb, ChkSocketTimeout, "--socket-timeout 30");
            AppendIfChecked(sb, ChkThrottledRate, "--throttled-rate 100K");
            AppendIfChecked(sb, ChkNoAbortOnError, "--no-abort-on-error");
            AppendIfChecked(sb, ChkConcurrentFragments, "--concurrent-fragments 3");

            string cookies = TxtCookiesPath.Text.Trim().Replace("\r", "").Replace("\n", "");
            if (!string.IsNullOrEmpty(cookies))
            {
                sb.Append($" --cookies {QuoteArgument(cookies)}");
            }

            string format = TxtFormat.Text.Trim().Replace("\r", "").Replace("\n", "");
            if (!string.IsNullOrEmpty(format))
            {
                sb.Append($" -f {QuoteArgument(format)}");
            }

            string url = TxtUrl.Text.Trim().Replace("\r", "").Replace("\n", "");
            if (!string.IsNullOrEmpty(url))
            {
                sb.Append($" {QuoteArgument(url)}");
            }

            return sb.ToString();
        }

        private static void AppendIfChecked(StringBuilder sb, CheckBox chk, string argument)
        {
            if (chk.IsChecked == true)
            {
                sb.Append(' ').Append(argument);
            }
        }

        private static string QuoteArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return argument;

            return $"\"{argument}\"";
        }

        private void BtnBrowseCookies_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select cookies.txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                TxtCookiesPath.Text = dlg.FileName;
            }
        }

        private void BtnBrowseYtdlp_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select yt-dlp executable",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                TxtYtdlpPath.Text = dlg.FileName;
            }
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            TxtStatus.Foreground = System.Windows.Media.Brushes.DarkRed;

            string url = TxtUrl.Text.Trim();
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

            string cookies = TxtCookiesPath.Text.Trim();
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

            string ytdlpPath = TxtYtdlpPath.Text.Trim();
            if (!string.IsNullOrEmpty(ytdlpPath) && !File.Exists(ytdlpPath))
            {
                TxtStatus.Text = $"Error: yt-dlp executable not found: {ytdlpPath}";
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
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                };

                Process.Start(psi);

                TxtStatus.Foreground = System.Windows.Media.Brushes.DarkGreen;
                TxtStatus.Text = "Download process launched. Check the new CMD window for progress.";
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Failed to launch process: {ex.Message}";
            }
        }
    }
}
