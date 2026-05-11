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
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.UI.Avalonia;
using VisioForge.Core.VideoCaptureX;

namespace SimpleVideoCaptureA
{
    public partial class MainWindow : Window, IDisposable
    {
        private bool _closingHandled;
        private bool _initialized;

        private System.Timers.Timer tmRecording = new System.Timers.Timer(1000);

        private VideoCaptureCoreX VideoCapture1;

        private bool disposedValue;

        public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VideoInputDevices { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VideoInputFormats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> VideoInputFrameRates { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioInputDevices { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioInputFormats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> AudioOutputDevices { get; set; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            // Mandatory engine boot. On a fresh machine the first call builds the GStreamer
            // plugin registry (~2-5s); subsequent launches are instant.
            VisioForgeX.InitSDK();

            InitControls();

            Activated += MainWindow_Activated;
            Closing += Window_Closing;

            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void MainWindow_Activated(object sender, EventArgs e)
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

#if __MACOS__
            // Camera permission prompt — required for capture on macOS 10.14+.
            AVFoundation.AVCaptureDevice.RequestAccessForMediaType(
                AVFoundation.AVAuthorizationMediaType.Video,
                (bool granted) => { Debug.WriteLine($"Camera access: {granted}"); });
#endif

            CreateEngine();

            Title += $" (SDK v{VideoCaptureCoreX.SDK_Version})";
            VideoCapture1.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

            tmRecording.Elapsed += (senderx, args) => { UpdateRecordingTimeAsync(); };

            // Populate device lists.
            var videoInputs = await DeviceEnumerator.Shared.VideoSourcesAsync();
            foreach (var device in videoInputs)
            {
                VideoInputDevices.Add(device.DisplayName);
            }
            if (VideoInputDevices.Count > 0) cbVideoInputDevice.SelectedIndex = 0;

            var audioInputs = await DeviceEnumerator.Shared.AudioSourcesAsync();
            foreach (var device in audioInputs)
            {
                AudioInputDevices.Add(device.DisplayName);
            }
            if (AudioInputDevices.Count > 0) cbAudioInputDevice.SelectedIndex = 0;

            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
            foreach (var device in audioOutputs)
            {
                AudioOutputDevices.Add(device.DisplayName);
            }
            if (AudioOutputDevices.Count > 0) cbAudioOutputDevice.SelectedIndex = 0;

            edOutput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
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
            mmLog = this.FindControl<ListBox>("mmLog");
            mmLog.ItemsSource = Logs;
        }

        private void CreateEngine()
        {
            VideoCapture1 = new VideoCaptureCoreX(VideoView1);
            VideoCapture1.OnError += VideoCapture1_OnError;

            // To register a purchased licence, add the two lines below:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await VideoCapture1.SetLicenseCertificateAsync(cert);
            // Otherwise the engine runs in 30-day trial mode.
        }

        private async Task DestroyEngineAsync()
        {
            if (VideoCapture1 != null)
            {
                VideoCapture1.OnError -= VideoCapture1_OnError;
                await VideoCapture1.DisposeAsync();
                VideoCapture1 = null;
            }
        }

        private async Task<string> SaveVideoFileDialogAsync()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
            {
                throw new InvalidOperationException("No top-level window available for file dialog.");
            }

            var exts = new string[] { "mp4", "avi", "wmv", "wma", "mp3", "ogg" };
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
            string filename = await SaveVideoFileDialogAsync();
            if (!string.IsNullOrEmpty(filename))
            {
                edOutput.Text = filename;
            }
        }

        private void Log(string txt) => Logs.Add(txt);

        private void VideoCapture1_OnError(object sender, ErrorsEventArgs e)
        {
            // OnError can fire on a non-UI thread; marshal back before mutating UI state.
            Dispatcher.UIThread.Post(() => Log(e.Message));
        }

        private void tbAudioVolume_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (VideoCapture1 == null)
            {
                return;
            }

            if (e.Property.ToString() == "Value")
            {
                VideoCapture1.Audio_OutputDevice_Volume = ((int)tbAudioVolume.Value);
            }
        }

        private async void cbVideoInputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVideoInputDevice.SelectedIndex != -1 && e != null && e.AddedItems.Count > 0)
            {
                VideoInputFormats.Clear();

                var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                    .FirstOrDefault(device => device.DisplayName == e.AddedItems[0].ToString());
                if (deviceItem == null) return;

                foreach (var format in deviceItem.VideoFormats)
                {
                    VideoInputFormats.Add(format.Name);
                }

                if (VideoInputFormats.Count > 0)
                {
                    cbVideoInputFormat.SelectedIndex = 0;
                    cbVideoInputFormat_SelectionChanged(null, null);
                }
            }
        }

        private async void cbAudioInputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAudioInputDevice.SelectedIndex != -1 && e != null && e.AddedItems.Count > 0)
            {
                AudioInputFormats.Clear();

                var deviceItem = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                    .FirstOrDefault(device => device.DisplayName == e.AddedItems[0].ToString());
                if (deviceItem == null) return;

                foreach (var format in deviceItem.Formats)
                {
                    AudioInputFormats.Add(format.Name);
                }

                if (AudioInputFormats.Count > 0)
                {
                    cbAudioInputFormat.SelectedIndex = 0;
                }
            }
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logs.Clear();

                VideoCapture1.Debug_Mode = cbDebugMode.IsChecked == true;

                if (cbRecordAudio.IsChecked == true)
                {
                    VideoCapture1.Audio_Record = true;
                    VideoCapture1.Audio_Play = true;
                }
                else
                {
                    VideoCapture1.Audio_Record = false;
                    VideoCapture1.Audio_Play = false;
                }

                var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())
                    .First(device => device.DisplayName == cbAudioOutputDevice.SelectedItem.ToString());
                VideoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

                // Video source.
                VideoCaptureDeviceSourceSettings videoSourceSettings = null;
                var deviceName = cbVideoInputDevice.SelectedItem.ToString();
                var format = cbVideoInputFormat.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(format))
                {
                    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                        .FirstOrDefault(x => x.DisplayName == deviceName);
                    if (device != null)
                    {
                        var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
                        if (formatItem != null)
                        {
                            videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
                            {
                                Format = formatItem.ToFormat()
                            };

                            if (cbVideoInputFrameRate.SelectedIndex != -1)
                            {
                                videoSourceSettings.Format.FrameRate = new VideoFrameRate(
                                    Convert.ToDouble(cbVideoInputFrameRate.SelectedItem.ToString()));
                            }
                        }
                    }
                }
                VideoCapture1.Video_Source = videoSourceSettings;

                // Audio source.
                IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;
                deviceName = cbAudioInputDevice.SelectedItem.ToString();
                format = cbAudioInputFormat.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(deviceName))
                {
                    var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())
                        .FirstOrDefault(x => x.DisplayName == deviceName);
                    if (device != null)
                    {
                        var formatItem = device.Formats.FirstOrDefault(x => x.Name == format);
                        if (formatItem != null)
                        {
                            audioSourceSettings = device.CreateSourceSettingsVC(formatItem.ToFormat());
                        }
                    }
                }
                VideoCapture1.Audio_Source = audioSourceSettings;

                VideoCapture1.Snapshot_Grabber_Enabled = true;

                if (rbPreview.IsChecked == false)
                {
                    var mp4output = new MP4Output(edOutput.Text);
                    VideoCapture1.Outputs_Add(mp4output, autostart: true);
                }

                await VideoCapture1.StartAsync();

                tcMain.SelectedIndex = 3;
                tmRecording.Start();
            }
            catch (Exception ex)
            {
                // async-void event handlers must catch — uncaught exceptions silently
                // terminate the process via AppDomain.UnhandledException.
                Log($"Start failed: {ex.Message}");
            }
        }

        private async void btResume_Click(object sender, RoutedEventArgs e) => await VideoCapture1.ResumeAsync();
        private async void btPause_Click(object sender, RoutedEventArgs e) => await VideoCapture1.PauseAsync();

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            tmRecording.Stop();
            await VideoCapture1.StopAsync();
        }

        private async void cbVideoInputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cbVideoInputFormat.SelectedItem?.ToString()) ||
                string.IsNullOrEmpty(cbVideoInputDevice.SelectedItem?.ToString()))
            {
                return;
            }

            if (cbVideoInputDevice.SelectedIndex == -1) return;

            VideoInputFrameRates.Clear();

            var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync())
                .FirstOrDefault(device => device.DisplayName == cbVideoInputDevice.SelectedValue.ToString());
            if (deviceItem == null) return;

            var videoFormat = deviceItem.VideoFormats.FirstOrDefault(f => f.Name == cbVideoInputFormat.SelectedValue.ToString());
            if (videoFormat == null) return;

            foreach (var item in videoFormat.GetFrameRateRangeAsStringList())
            {
                VideoInputFrameRates.Add(item);
            }

            if (VideoInputFrameRates.Count > 0)
            {
                cbVideoInputFrameRate.SelectedIndex = 0;
            }
        }

        private async Task UpdateRecordingTimeAsync()
        {
            var ts = await VideoCapture1.DurationAsync();
            if (Math.Abs(ts.TotalMilliseconds) < 0.01) return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                lbTimestamp.Text = "Recording time: " + ts.ToString(@"hh\:mm\:ss");
            });
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Avalonia's Closing event does not await async-void handlers, so cleanup would
            // otherwise race with Avalonia tearing down the visual tree. Cancel the first
            // close, run cleanup, then close manually. Re-entry must be a no-op.
            if (_closingHandled) return;

            e.Cancel = true;
            _closingHandled = true;

            try
            {
                tmRecording?.Stop();
                tmRecording?.Dispose();
                tmRecording = null;

                if (VideoCapture1 != null)
                {
                    await VideoCapture1.StopAsync();
                    await DestroyEngineAsync();
                }

                VideoView1?.Dispose();
                VideoView1 = null;

                VisioForgeX.DestroySDK();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Window closing error: {ex.Message}");
            }

            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    tmRecording?.Dispose();
                    tmRecording = null;

                    if (VideoCapture1 != null)
                    {
                        try
                        {
                            DestroyEngineAsync().Wait(TimeSpan.FromSeconds(5));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Dispose(true) DestroyEngineAsync error: {ex.Message}");
                        }
                        VideoCapture1 = null;
                    }

                    VideoView1?.Dispose();
                    VideoView1 = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
