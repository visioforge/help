using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

namespace Sample
{
    /// <summary>
    /// Minimal Media Player SDK X WinForms host.
    ///
    /// Distilled from the upstream "Main Demo" sample
    /// (_SETUP/GitHub/Media Player SDK X/WinForms/Main Demo/) — kept just the
    /// load-bearing playback path: InitSDKAsync boot, file/URL open via
    /// UniversalSourceSettings, play/pause/resume/stop, timeline trackbar,
    /// OnError + OnStop wiring, and clean shutdown.
    ///
    /// To register a purchased licence, uncomment the SetLicenseCertificateAsync
    /// lines in Form1_Load. Without it the SDK runs in 30-day trial mode.
    /// </summary>
    public partial class Form1 : Form
    {
        private MediaPlayerCoreX _player;
        private volatile bool _closing;
        private bool _isTimerUpdating;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // async-void event handlers must catch — an exception escaping here
            // goes to AppDomain.UnhandledException and silently terminates the app.
            // First-run triggers: missing native DLLs, registry build failure,
            // trial expired, or a codec missing from the referenced redist.
            try
            {
                // Mandatory engine boot. Builds the GStreamer plugin-registry cache
                // on first run (~2-5 s); subsequent launches are instant. Calling
                // any other SDK API before this returns is the #1 source of
                // "DLL not found" / "no element X" failures on first run.
                Text += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
                Enabled = false;
                await VisioForgeX.InitSDKAsync();
                Enabled = true;
                Text = Text.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

                _player = new MediaPlayerCoreX(VideoView1);
                _player.OnError += Player_OnError;
                _player.OnStop += Player_OnStop;

                // To use a purchased licence, drop your .vflicense beside the exe
                // and uncomment:
                //
                //   var cert = File.ReadAllBytes("your.vflicense");
                //   await _player.SetLicenseCertificateAsync(cert);

                // Populate audio output devices (DirectSound is the most compatible
                // backend on Windows; WASAPI is also available via AudioOutputDeviceAPI.WASAPI).
                foreach (var device in await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound))
                {
                    cbAudioOutputDevice.Items.Add(device.Name);
                }

                if (cbAudioOutputDevice.Items.Count > 0)
                {
                    cbAudioOutputDevice.SelectedIndex = 0;
                }

                Text += $" (SDK v{MediaPlayerCoreX.SDK_Version})";
            }
            catch (Exception ex)
            {
                Enabled = true;
                System.Diagnostics.Debug.WriteLine($"Form1_Load error: {ex}");
                MessageBox.Show($"SDK initialization failed: {ex.Message}");
            }
        }

        private void btSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edFilenameOrURL.Text = openFileDialog1.FileName;
            }
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // async-void event handlers must catch — otherwise an exception
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, registry not built (forgot InitSDKAsync), or a
            // codec missing from the referenced redist.
            try
            {
                mmLog.Clear();

                _player.Audio_Play = cbPlayAudio.Checked;

                if (!string.IsNullOrEmpty(cbAudioOutputDevice.Text))
                {
                    var devices = await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound);
                    var selected = devices.FirstOrDefault(d => d.Name == cbAudioOutputDevice.Text);
                    if (selected != null)
                    {
                        _player.Audio_OutputDevice = new AudioRendererSettings(selected);
                    }
                }

                var input = edFilenameOrURL.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Please enter a file path or URL.");
                    return;
                }

                if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
                {
                    if (!File.Exists(input))
                    {
                        MessageBox.Show("File not found and not a valid URL.");
                        return;
                    }
                    uri = new Uri(Path.GetFullPath(input));
                }

                // UniversalSourceSettings handles local files, http(s), and most
                // network URLs. For RTSP-specific tuning (latency, transport),
                // use RTSPSourceSettings.CreateAsync(...) instead.
                var source = await UniversalSourceSettings.CreateAsync(uri);
                await _player.OpenAsync(source);
                await _player.PlayAsync();

                _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;

                tmPosition.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Playback failed: {ex.Message}");
            }
        }

        private async void btPause_Click(object sender, EventArgs e)
        {
            await _player.PauseAsync();
        }

        private async void btResume_Click(object sender, EventArgs e)
        {
            await _player.ResumeAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            tmPosition.Stop();
            await _player.StopAsync();
            tbTimeline.Value = 0;
            VideoView1.Invalidate();
        }

        private async void tmPosition_Tick(object sender, EventArgs e)
        {
            _isTimerUpdating = true;
            try
            {
                var position = await _player.Position_GetAsync();
                var duration = await _player.DurationAsync();

                tbTimeline.Maximum = Math.Max(1, (int)duration.TotalSeconds);
                if (position.TotalSeconds >= 0 && position.TotalSeconds <= tbTimeline.Maximum)
                {
                    tbTimeline.Value = (int)position.TotalSeconds;
                }
                lbTimeline.Text = position.ToString("hh\\:mm\\:ss") + " | " + duration.ToString("hh\\:mm\\:ss");
            }
            finally
            {
                _isTimerUpdating = false;
            }
        }

        private void tbTimeline_Scroll(object sender, EventArgs e)
        {
            if (!_isTimerUpdating)
            {
                _player.Position_Set(TimeSpan.FromSeconds(tbTimeline.Value));
            }
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
            }
        }

        private void Player_OnError(object sender, ErrorsEventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                mmLog.Text += e.Message + Environment.NewLine;
            }));
        }

        private void Player_OnStop(object sender, StopEventArgs e)
        {
            // Posted from a worker thread — marshal UI work back to the form.
            tmPosition.Stop();
            if (_closing)
            {
                return;
            }
            BeginInvoke((Action)(() =>
            {
                tbTimeline.Value = 0;
                VideoView1.Invalidate();
            }));
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;
            tmPosition.Stop();

            if (_player != null)
            {
                await _player.StopAsync();
                await _player.DisposeAsync();
                _player = null;
            }

            VisioForgeX.DestroySDK();
        }
    }
}
