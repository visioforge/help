using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.UI;
using VisioForge.Core.UI.Avalonia;
using VisioForge.Core.VideoCaptureX;

namespace SimpleVideoCaptureAMB
{
    /// <summary>
    /// Simple Video Capture Demo on Media Blocks SDK .Net + Avalonia.
    ///
    /// Topology (preview-only):
    ///   SystemVideoSourceBlock --> VideoRendererBlock(VideoView1)
    ///   SystemAudioSourceBlock --> AudioRendererBlock(speakers)        [if audio enabled]
    ///
    /// Topology (capture mode — preview + MP4 record via tee):
    ///   SystemVideoSourceBlock --> TeeBlock(video) --[0]--> VideoRendererBlock
    ///                                              --[1]--> MP4OutputBlock(video pad)
    ///   SystemAudioSourceBlock --> TeeBlock(audio) --[0]--> AudioRendererBlock
    ///                                              --[1]--> MP4OutputBlock(audio pad)   [if audio enabled]
    ///
    /// Runs in 30-day trial mode by design — no license certificate is loaded. To register a
    /// purchased license: read the .vflicense as bytes and call
    /// `await _pipeline.SetLicenseCertificateAsync(certBytes)` in btStart_Click after the
    /// `_pipeline = new MediaBlocksPipeline()` line and before any `Connect()` call.
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private bool _initialized;
        private bool _closingHandled;
        private System.Timers.Timer tmRecording = new System.Timers.Timer(1000);

        private MediaBlocksPipeline _pipeline;
        private VideoRendererBlock _videoRenderer;
        private AudioRendererBlock _audioRenderer;
        private SystemVideoSourceBlock _videoSource;
        private MediaBlock _audioSource;
        private MP4OutputBlock _mp4Output;
        private TeeBlock _videoTee;
        private TeeBlock _audioTee;
        private bool disposedValue;

        public ObservableCollection<string> VideoInputDevices { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VideoInputFormats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VideoInputFrameRates { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioInputDevices { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioInputFormats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioInputLines { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioOutputDevices { get; set; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            // Mandatory engine boot — runs once per process. First call on a fresh machine
            // builds the GStreamer-equivalent plugin registry (~2-5 s); later launches are instant.
            VisioForgeX.InitSDK();

            DeviceEnumerator.Shared.OnVideoSourceAdded += DeviceEnumerator_OnVideoSourceAdded;

            InitControls();

            Activated += MainWindow_Activated;

            DataContext = this;
        }

        private async void DeviceEnumerator_OnVideoSourceAdded(object sender, VideoCaptureDeviceInfo e)
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    VideoInputDevices.Add(e.DisplayName);

                    if (cbVideoInputDevice.Items.Count == 1)
                    {
                        cbVideoInputDevice.SelectedIndex = 0;
                    }
                });
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private async void MainWindow_Activated(object sender, EventArgs e)
        {
            try
            {
                if (_initialized) return;
                _initialized = true;

#if __MACOS__
                // macOS 10.14+ requires explicit user consent for camera access.
                // Without this call, capture starts but produces only black frames.
                // Also requires NSCameraUsageDescription in the .app's Info.plist.
                AVFoundation.AVCaptureDevice.RequestAccessForMediaType(
                    AVFoundation.AVAuthorizationMediaType.Video,
                    granted => Debug.WriteLine($"Camera access: {granted}"));
#endif

                Closing += Window_Closing;

                await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();

                var audioInputs = await DeviceEnumerator.Shared.AudioSourcesAsync();
                foreach (var device in audioInputs) AudioInputDevices.Add(device.DisplayName);

                if (AudioInputDevices.Count > 0)
                {
                    cbAudioInputDevice.SelectedIndex = 0;
                    cbAudioInputDevice_SelectionChanged(null, null);
                }

                var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
                foreach (var d in audioOutputs) AudioOutputDevices.Add(d.DisplayName);
                if (AudioOutputDevices.Count > 0) cbAudioOutputDevice.SelectedIndex = 0;

                Title += $" (SDK v{VideoCaptureCoreX.SDK_Version})";

                tmRecording.Elapsed += (senderx, args) => { UpdateRecordingTimeAsync(); };

                edOutput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void InitControls()
        {
            VideoView1 = this.FindControl<VideoView>("VideoView1");

            cbVideoInputDevice = this.FindControl<ComboBox>("cbVideoInputDevice");
            cbVideoInputDevice.SelectionChanged += cbVideoInputDevice_SelectionChanged;
            cbVideoInputDevice.ItemsSource = VideoInputDevices;

            cbVideoInputFormat = this.FindControl<ComboBox>("cbVideoInputFormat");
            cbVideoInputFormat.SelectionChanged += cbVideoInputFormat_SelectionChanged;
            cbVideoInputFormat.ItemsSource = VideoInputFormats;

            cbVideoInputFrameRate = this.FindControl<ComboBox>("cbVideoInputFrameRate");
            cbVideoInputFrameRate.ItemsSource = VideoInputFrameRates;

            cbAudioInputDevice = this.FindControl<ComboBox>("cbAudioInputDevice");
            cbAudioInputDevice.SelectionChanged += cbAudioInputDevice_SelectionChanged;
            cbAudioInputDevice.ItemsSource = AudioInputDevices;

            cbAudioInputFormat = this.FindControl<ComboBox>("cbAudioInputFormat");
            cbAudioInputFormat.ItemsSource = AudioInputFormats;

            cbAudioOutputDevice = this.FindControl<ComboBox>("cbAudioOutputDevice");
            cbAudioOutputDevice.ItemsSource = AudioOutputDevices;

            cbRecordAudio = this.FindControl<CheckBox>("cbRecordAudio");

            tbAudioVolume = this.FindControl<Slider>("tbAudioVolume");
            tbAudioVolume.PropertyChanged += tbAudioVolume_PropertyChanged;

            edOutput = this.FindControl<TextBox>("edOutput");

            btSelectOutput = this.FindControl<Button>("btSelectOutput");
            btSelectOutput.Click += btSelectOutput_Click;

            cbDebugMode = this.FindControl<CheckBox>("cbDebugMode");

            rbPreview = this.FindControl<RadioButton>("rbPreview");
            rbCapture = this.FindControl<RadioButton>("rbCapture");

            lbTimestamp = this.FindControl<TextBlock>("lbTimestamp");

            btStart = this.FindControl<Button>("btStart");
            btStart.Click += btStart_Click;

            btStop = this.FindControl<Button>("btStop");
            btStop.Click += btStop_Click;

            btResume = this.FindControl<Button>("btResume");
            btResume.Click += btResume_Click;

            btPause = this.FindControl<Button>("btPause");
            btPause.Click += btPause_Click;

            tcMain = this.FindControl<TabControl>("tcMain");
            edLog = this.FindControl<TextBox>("edLog");
        }

        // Avalonia's cross-platform replacement for SaveFileDialog — works on Windows, macOS, Linux.
        private async Task<string> SaveVideoFileDialogAsync()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return null;

            var exts = new[] { "mp4", "avi", "wmv", "wma", "mp3", "ogg" };
            var fileTypes = exts.Select(ext => new FilePickerFileType(ext.ToUpperInvariant())
            {
                Patterns = new[] { "*." + ext }
            }).ToList();

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                SuggestedFileName = "video.mp4",
                DefaultExtension = "mp4",
                FileTypeChoices = fileTypes
            });

            return file?.Path?.LocalPath;
        }

        private async void btSelectOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filename = await SaveVideoFileDialogAsync();
                if (!string.IsNullOrEmpty(filename)) edOutput.Text = filename;
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        // Pipeline.OnError fires on a non-UI thread — marshal to UI before touching controls.
        // Touching Avalonia controls off-UI throws on Linux (X11 single-threaded) and intermittently
        // corrupts state on macOS.
        private void Log(string txt) =>
            Dispatcher.UIThread.InvokeAsync(() => edLog.Text += txt + Environment.NewLine);

        private void Pipeline_OnError(object sender, ErrorsEventArgs e) => Log(e.Message);

        private void tbAudioVolume_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_audioRenderer != null && e.Property.ToString() == "Value")
                _audioRenderer.Volume = tbAudioVolume.Value / 100;
        }

        private async void cbVideoInputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbVideoInputDevice.SelectedIndex == -1 || e == null || e.AddedItems.Count == 0) return;

                VideoInputFormats.Clear();

                var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                    .FirstOrDefault(d => d.DisplayName == e.AddedItems[0].ToString());
                if (deviceItem == null) return;

                foreach (var format in deviceItem.VideoFormats) VideoInputFormats.Add(format.Name);

                if (VideoInputFormats.Count > 0)
                {
                    cbVideoInputFormat.SelectedIndex = 0;
                    cbVideoInputFormat_SelectionChanged(null, null);
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void cbAudioInputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbAudioInputDevice.SelectedIndex == -1 || e == null || e.AddedItems.Count == 0) return;

                AudioInputFormats.Clear();

                var deviceItem = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                    .FirstOrDefault(d => d.DisplayName == e.AddedItems[0].ToString());
                if (deviceItem == null) return;

                foreach (var format in deviceItem.Formats) AudioInputFormats.Add(format.Name);
                if (AudioInputFormats.Count > 0) cbAudioInputFormat.SelectedIndex = 0;
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                edLog.Text = string.Empty;

                // Start -> Stop -> Start guard: tear down any previous pipeline and its blocks first.
                if (_pipeline != null)
                {
                    await _pipeline.StopAsync();
                    _pipeline.OnError -= Pipeline_OnError;
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                    DisposeBlocks();
                }

                _pipeline = new MediaBlocksPipeline();
                _pipeline.OnError += Pipeline_OnError;
                _pipeline.Debug_Mode = cbDebugMode.IsChecked == true;
                _pipeline.Debug_Dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

                // For a purchased licence, add these two lines here:
                //   var cert = System.IO.File.ReadAllBytes(
                //       Path.Combine(AppContext.BaseDirectory, "your.vflicense"));
                //   await _pipeline.SetLicenseCertificateAsync(cert);

                _videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);

                bool audioStream = cbRecordAudio.IsChecked == true;
                bool capture = rbCapture.IsChecked == true;

                // ---- video source ----
                VideoCaptureDeviceSourceSettings videoSourceSettings = null;
                var deviceName = cbVideoInputDevice.SelectedItem?.ToString();
                var format = cbVideoInputFormat.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(format))
                {
                    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                        .FirstOrDefault(x => x.DisplayName == deviceName);
                    if (device != null)
                    {
                        var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
                        if (formatItem != null)
                        {
                            videoSourceSettings = new VideoCaptureDeviceSourceSettings(device, formatItem.ToFormat());
                            if (cbVideoInputFrameRate.SelectedIndex != -1)
                            {
                                videoSourceSettings.Format.FrameRate =
                                    new VideoFrameRate(Convert.ToDouble(cbVideoInputFrameRate.SelectedItem.ToString()));
                            }
                        }
                    }
                }

                _videoSource = new SystemVideoSourceBlock(videoSourceSettings);

                // ---- audio source (optional) ----
                if (audioStream)
                {
                    IAudioCaptureDeviceSourceSettings audioSourceSettings = null;
                    deviceName = cbAudioInputDevice.SelectedItem?.ToString();
                    format = cbAudioInputFormat.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                            .FirstOrDefault(x => x.DisplayName == deviceName);
                        if (device != null)
                        {
                            var formatItem = device.Formats.FirstOrDefault(x => x.Name == format);
                            if (formatItem != null)
                                audioSourceSettings = device.CreateSourceSettings(formatItem.ToFormat());
                        }
                    }

                    _audioSource = new SystemAudioSourceBlock(audioSourceSettings);

                    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())
                        .First(d => d.DisplayName == cbAudioOutputDevice.SelectedItem.ToString());
                    _audioRenderer = new AudioRendererBlock(audioOutputDevice);
                }

                // ---- wire the graph ----
                if (capture)
                {
                    // Tee splits each source into two paths: preview + MP4 record.
                    _videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);
                    _pipeline.Connect(_videoSource, _videoTee);

                    _mp4Output = new MP4OutputBlock(edOutput.Text);
                    _pipeline.Connect(_videoTee, _mp4Output);
                    _pipeline.Connect(_videoTee, _videoRenderer);

                    if (audioStream)
                    {
                        _audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio);
                        _pipeline.Connect(_audioSource, _audioTee);
                        _pipeline.Connect(_audioTee, _mp4Output);
                        _pipeline.Connect(_audioTee, _audioRenderer);
                    }
                }
                else
                {
                    _pipeline.Connect(_videoSource, _videoRenderer);
                    if (audioStream) _pipeline.Connect(_audioSource, _audioRenderer);
                }

                await _pipeline.StartAsync();

                tcMain.SelectedIndex = 3;
                tmRecording.Start();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void btResume_Click(object sender, RoutedEventArgs e)
        {
            try { if (_pipeline != null) await _pipeline.ResumeAsync(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void btPause_Click(object sender, RoutedEventArgs e)
        {
            try { if (_pipeline != null) await _pipeline.PauseAsync(); }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tmRecording.Stop();

                if (_pipeline != null)
                {
                    // StopAsync (not Stop) — sync overload deadlocks the UI thread when the renderer
                    // still holds the last frame. Always use the async forms.
                    await _pipeline.StopAsync();

                    _pipeline.OnError -= Pipeline_OnError;
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }

                DisposeBlocks();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        // Dispose the individual MediaBlock instances created in btStart_Click.
        // DisposeAsync on the pipeline does NOT dispose blocks wired only via Connect —
        // sources/renderers/tees/sinks remain owned by the caller.
        private void DisposeBlocks()
        {
            _videoSource?.Dispose(); _videoSource = null;
            _audioSource?.Dispose(); _audioSource = null;
            _videoRenderer?.Dispose(); _videoRenderer = null;
            _audioRenderer?.Dispose(); _audioRenderer = null;
            _videoTee?.Dispose(); _videoTee = null;
            _audioTee?.Dispose(); _audioTee = null;
            _mp4Output?.Dispose(); _mp4Output = null;
        }

        private async void cbVideoInputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbVideoInputFormat.SelectedItem?.ToString()) ||
                    string.IsNullOrEmpty(cbVideoInputDevice.SelectedItem?.ToString())) return;

                if (cbVideoInputDevice.SelectedIndex == -1) return;

                VideoInputFrameRates.Clear();

                var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                    .FirstOrDefault(d => d.DisplayName == cbVideoInputDevice.SelectedValue.ToString());
                if (deviceItem == null) return;

                var videoFormat = deviceItem.VideoFormats.FirstOrDefault(f => f.Name == cbVideoInputFormat.SelectedValue.ToString());
                if (videoFormat == null) return;

                foreach (var item in videoFormat.GetFrameRateRangeAsStringList())
                    VideoInputFrameRates.Add(item);

                if (VideoInputFrameRates.Count > 0) cbVideoInputFrameRate.SelectedIndex = 0;
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async Task UpdateRecordingTimeAsync()
        {
            if (_pipeline == null) return;

            var pos = await _pipeline.Position_GetAsync();
            if (Math.Abs(pos.TotalMilliseconds) < 0.01) return;

            // Marshal to UI — Position_GetAsync continues on a worker thread.
            await Dispatcher.UIThread.InvokeAsync(() =>
                lbTimestamp.Text = "Recording time: " + pos.ToString(@"hh\:mm\:ss"));
        }

        // Shutdown order matters: StopAsync → DisposeAsync → DestroySDK.
        // Skipping StopAsync leaks worker threads (and risks tearing down a still-Playing
        // GStreamer pipeline); skipping DestroySDK leaks the plugin registry.
        // Avalonia's Closing does not await async-void handlers, so the first close is cancelled,
        // cleanup runs asynchronously, then Close() is called for real.
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_closingHandled) return;

            e.Cancel = true;
            _closingHandled = true;

            try
            {
                tmRecording?.Stop();

                if (_pipeline != null)
                {
                    await _pipeline.StopAsync();
                    _pipeline.OnError -= Pipeline_OnError;
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }

                DisposeBlocks();
                VisioForgeX.DestroySDK();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }

            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                tmRecording?.Dispose();
                tmRecording = null;

                if (_pipeline != null)
                {
                    _pipeline.OnError -= Pipeline_OnError;
                    _pipeline.Dispose();
                    _pipeline = null;
                }

                DisposeBlocks();

                VideoView1?.Dispose();
                VideoView1 = null;
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
