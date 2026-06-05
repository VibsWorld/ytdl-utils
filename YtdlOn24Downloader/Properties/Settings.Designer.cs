namespace YtdlOn24Downloader.Properties {

    [global::System.Configuration.SettingsProvider(typeof(global::System.Configuration.LocalFileSettingsProvider))]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default {
            get {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CookiesPath {
            get {
                return ((string)(this["CookiesPath"]));
            }
            set {
                this["CookiesPath"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string YtdlpPath {
            get {
                return ((string)(this["YtdlpPath"]));
            }
            set {
                this["YtdlpPath"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("bestvideo+bestaudio/best")]
        public string FormatString {
            get {
                return ((string)(this["FormatString"]));
            }
            set {
                this["FormatString"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastUrl {
            get {
                return ((string)(this["LastUrl"]));
            }
            set {
                this["LastUrl"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OutputDirectory {
            get {
                return ((string)(this["OutputDirectory"]));
            }
            set {
                this["OutputDirectory"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool RetryInfinite {
            get {
                return ((bool)(this["RetryInfinite"]));
            }
            set {
                this["RetryInfinite"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FragmentRetryInfinite {
            get {
                return ((bool)(this["FragmentRetryInfinite"]));
            }
            set {
                this["FragmentRetryInfinite"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FileAccessRetries {
            get {
                return ((bool)(this["FileAccessRetries"]));
            }
            set {
                this["FileAccessRetries"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ContinuePartial {
            get {
                return ((bool)(this["ContinuePartial"]));
            }
            set {
                this["ContinuePartial"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PartFile {
            get {
                return ((bool)(this["PartFile"]));
            }
            set {
                this["PartFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SkipUnavailableFragments {
            get {
                return ((bool)(this["SkipUnavailableFragments"]));
            }
            set {
                this["SkipUnavailableFragments"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HlsUseMpegts {
            get {
                return ((bool)(this["HlsUseMpegts"]));
            }
            set {
                this["HlsUseMpegts"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SocketTimeout {
            get {
                return ((bool)(this["SocketTimeout"]));
            }
            set {
                this["SocketTimeout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ThrottledRate {
            get {
                return ((bool)(this["ThrottledRate"]));
            }
            set {
                this["ThrottledRate"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool NoAbortOnError {
            get {
                return ((bool)(this["NoAbortOnError"]));
            }
            set {
                this["NoAbortOnError"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ConcurrentFragments {
            get {
                return ((bool)(this["ConcurrentFragments"]));
            }
            set {
                this["ConcurrentFragments"] = value;
            }
        }
    }
}
