using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace MediaBlocks_Simple_Video_Capture_Demo
{
    /// <summary>
    /// The main form of the application. Demonstrates a Media Blocks pipeline:
    /// camera + microphone -> tees -> preview + (optional) MP4 capture.
    /// </summary>
    public partial class MainForm : Form
    {
        // Pipeline (graph-based: blocks are connected via Connect() calls)
        private MediaBlocksPipeline _pipeline;

        // Sources
        private SystemVideoSourceBlock _videoSource;
        private SystemAudioSourceBlock _audioSource;

        // Splitters
        private TeeBlock _videoTee;
        private TeeBlock _audioTee;

        // Renderers (preview)
        private VideoRendererBlock _videoRenderer;
        private AudioRendererBlock _audioRenderer;

        // Capture path (optional)
        private H264EncoderBlock _h264Encoder;
        private AACEncoderBlock _aacEncoder;
        private MP4SinkBlock _mp4Muxer;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form_Load is the right place to initialise the SDK and enumerate devices.
        /// Do NOT do this from the constructor — the form's HWND isn't realised yet
        /// and DeviceEnumerator marshalling back to the UI thread will throw.
        /// </summary>
        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // First-run builds the GStreamer plugin registry (~5s).
                Text += " [LOADING...]";
                Enabled = false;
                await VisioForgeX.InitSDKAsync();
                Enabled = true;
                Text = Text.Replace(" [LOADING...]", "") +
                       $" (SDK v{MediaBlocksPipeline.SDK_Version})";

                // Live device enumeration. The "Shared" enumerator raises events
                // on a worker thread; marshal into the UI thread with Invoke.
                DeviceEnumerator.Shared.OnVideoSourceAdded += (s, dev) =>
                    Invoke((Action)(() => cbVideoInput.Items.Add(dev.DisplayName)));
                DeviceEnumerator.Shared.OnAudioSourceAdded += (s, dev) =>
                    Invoke((Action)(() => cbAudioInput.Items.Add(dev.DisplayName)));
                DeviceEnumerator.Shared.OnAudioSinkAdded += (s, dev) =>
                    Invoke((Action)(() => cbAudioOutput.Items.Add(dev.DisplayName)));

                await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();
                await DeviceEnumerator.Shared.StartAudioSourceMonitorAsync();
                await DeviceEnumerator.Shared.StartAudioSinkMonitorAsync();

                edFilename.Text = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    "output.mp4");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Pipeline_OnError(object sender, ErrorsEventArgs e)
        {
            Invoke((Action)(() => mmLog.AppendText(e.Message + Environment.NewLine)));
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbVideoInput.SelectedIndex < 0)
                {
                    MessageBox.Show(this, "Select video input device");
                    return;
                }

                if (_pipeline != null)
                {
                    await _pipeline.StopAsync();
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }

                _pipeline = new MediaBlocksPipeline();

                // ---- License registration goes here, once you have a .vflicense ----
                // var cert = File.ReadAllBytes("path/to/your.vflicense");
                // await _pipeline.SetLicenseCertificateAsync(cert);   // per-pipeline, before StartAsync
                // (See SKILL.md "License registration" — without this the SDK runs in 30-day trial.)

                _pipeline.OnError += Pipeline_OnError;

                bool capture = cbCapture.Checked;

                // ---- Video source block ----
                VideoCaptureDeviceSourceSettings videoSettings = null;
                var videoDevice = (await SystemVideoSourceBlock.GetDevicesAsync())
                    .FirstOrDefault(x => x.DisplayName == cbVideoInput.Text);
                if (videoDevice != null && !string.IsNullOrEmpty(cbVideoFormat.Text))
                {
                    var fmt = videoDevice.VideoFormats.FirstOrDefault(x => x.Name == cbVideoFormat.Text);
                    if (fmt != null)
                    {
                        videoSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
                        {
                            Format = fmt.ToFormat()
                        };
                        if (!string.IsNullOrEmpty(cbVideoFrameRate.Text))
                        {
                            videoSettings.Format.FrameRate = new VideoFrameRate(
                                Convert.ToDouble(cbVideoFrameRate.Text));
                        }
                    }
                }
                _videoSource = new SystemVideoSourceBlock(videoSettings);

                // ---- Audio source block ----
                IAudioCaptureDeviceSourceSettings audioSettings = null;
                var audioDevice = (await SystemAudioSourceBlock.GetDevicesAsync())
                    .FirstOrDefault(x => x.DisplayName == cbAudioInput.Text);
                if (audioDevice != null)
                {
                    var fmt = audioDevice.Formats.FirstOrDefault(x => x.Name == cbAudioFormat.Text);
                    if (fmt != null)
                    {
                        audioSettings = audioDevice.CreateSourceSettings(fmt.ToFormat());
                    }
                }
                _audioSource = new SystemAudioSourceBlock(audioSettings);

                // ---- Renderers (preview) ----
                // VideoView1 is VisioForge.Core.UI.WinForms.VideoView (added via Designer).
                _videoRenderer = new VideoRendererBlock(_pipeline, VideoView1) { IsSync = false };

                var audioOut = (await AudioRendererBlock.GetDevicesAsync())
                    .FirstOrDefault(x => x.DisplayName == cbAudioOutput.Text);
                _audioRenderer = new AudioRendererBlock(audioOut) { IsSync = false };

                // ---- Tees split each stream into N branches: preview + (optional) capture ----
                int branches = capture ? 2 : 1;
                _videoTee = new TeeBlock(branches, MediaBlockPadMediaType.Video);
                _audioTee = new TeeBlock(branches, MediaBlockPadMediaType.Audio);

                // ---- Capture path (H.264 + AAC -> MP4) ----
                if (capture)
                {
                    _h264Encoder = new H264EncoderBlock();
                    _aacEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());
                    _mp4Muxer = new MP4SinkBlock(new MP4SinkSettings(edFilename.Text));
                }

                // ---- Wire up the graph ----
                // source -> tee -> renderer
                _pipeline.Connect(_videoSource.Output, _videoTee.Input);
                _pipeline.Connect(_videoTee.Outputs[0], _videoRenderer.Input);

                _pipeline.Connect(_audioSource.Output, _audioTee.Input);
                _pipeline.Connect(_audioTee.Outputs[0], _audioRenderer.Input);

                if (capture)
                {
                    // tee[1] -> encoder -> muxer input
                    _pipeline.Connect(_videoTee.Outputs[1], _h264Encoder.Input);
                    _pipeline.Connect(_h264Encoder.Output,
                        _mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Video));

                    _pipeline.Connect(_audioTee.Outputs[1], _aacEncoder.Input);
                    _pipeline.Connect(_aacEncoder.Output,
                        _mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Audio));
                }

                await _pipeline.StartAsync();
                timer1.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                if (_pipeline != null)
                {
                    await _pipeline.StopAsync();
                    _pipeline.ClearBlocks();
                    _pipeline.OnError -= Pipeline_OnError;
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }

                // Dispose individual MediaBlock instances created in btStart_Click.
                // ClearBlocks/DisposeAsync on the pipeline does NOT dispose blocks
                // that were not added via AddBlock — sources/renderers/encoders
                // wired only via Connect remain owned by the caller.
                _videoSource?.Dispose(); _videoSource = null;
                _audioSource?.Dispose(); _audioSource = null;
                _videoRenderer?.Dispose(); _videoRenderer = null;
                _audioRenderer?.Dispose(); _audioRenderer = null;
                _videoTee?.Dispose(); _videoTee = null;
                _audioTee?.Dispose(); _audioTee = null;
                _h264Encoder?.Dispose(); _h264Encoder = null;
                _aacEncoder?.Dispose(); _aacEncoder = null;
                _mp4Muxer?.Dispose(); _mp4Muxer = null;

                VideoView1.Invalidate();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_pipeline != null)
                {
                    var pos = await _pipeline.Position_GetAsync();
                    lbTime.Text = pos.ToString(@"hh\:mm\:ss");
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timer1.Stop();
                if (_pipeline != null)
                {
                    _pipeline.OnError -= Pipeline_OnError;
                    await _pipeline.StopAsync();
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }
                VisioForgeX.DestroySDK();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private async void cbVideoInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbVideoFormat.Items.Clear();
            var dev = (await SystemVideoSourceBlock.GetDevicesAsync())
                .FirstOrDefault(x => x.DisplayName == cbVideoInput.Text);
            if (dev == null) return;
            foreach (var f in dev.VideoFormats) cbVideoFormat.Items.Add(f.Name);
            if (cbVideoFormat.Items.Count > 0) cbVideoFormat.SelectedIndex = 0;
        }

        private async void cbVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbVideoFrameRate.Items.Clear();
            var dev = (await SystemVideoSourceBlock.GetDevicesAsync())
                .FirstOrDefault(x => x.DisplayName == cbVideoInput.Text);
            var fmt = dev?.VideoFormats.FirstOrDefault(x => x.Name == cbVideoFormat.Text);
            if (fmt == null) return;
            foreach (var fr in fmt.GetFrameRateRangeAsStringList()) cbVideoFrameRate.Items.Add(fr);
            if (cbVideoFrameRate.Items.Count > 0) cbVideoFrameRate.SelectedIndex = 0;
        }

        private async void cbAudioInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAudioFormat.Items.Clear();
            var dev = (await SystemAudioSourceBlock.GetDevicesAsync())
                .FirstOrDefault(x => x.DisplayName == cbAudioInput.Text);
            if (dev == null) return;
            foreach (var f in dev.Formats) cbAudioFormat.Items.Add(f.Name);
            if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = 0;
        }

        private void btSelectOutput_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK) edFilename.Text = dlg.FileName;
        }
    }
}
