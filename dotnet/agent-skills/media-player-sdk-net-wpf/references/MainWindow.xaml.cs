using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.MediaInfo;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.Events;

namespace Simple_Player_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// The timer flag.
        /// </summary>
        private volatile bool _timerFlag;

        /// <summary>
        /// The media player.
        /// </summary>
        private MediaPlayerCore _player;

        /// <summary>
        /// The is closing flag.
        /// </summary>
        private volatile bool _isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Window loaded.
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += _timer_Elapsed;

            CreateEngine();

            Title += $" (SDK v{_player.SDK_Version()})";

            foreach (var device in _player.Audio_OutputDevices())
            {
                cbAudioOutput.Items.Add(device);
            }

            if (cbAudioOutput.Items.Count > 0)
            {
                cbAudioOutput.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Player on error.
        /// </summary>
        private void Player_OnError(object sender, ErrorsEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                edLog.Text = edLog.Text + e.Message + Environment.NewLine;
            }));
        }

        /// <summary>
        /// Player on stop.
        /// </summary>
        private void Player_OnStop(object sender, StopEventArgs e)
        {
            if (_isClosing)
            {
                return;
            }

            Dispatcher.Invoke((Action)(() =>
            {
                tbTimeline.Value = 0;
            }));
        }

        /// <summary>
        /// Create engine.
        /// </summary>
        private void CreateEngine()
        {
            _player = new MediaPlayerCore(VideoView1);
            _player.OnError += Player_OnError;
            _player.OnStop += Player_OnStop;

            _player.Debug_Dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");
            _player.Debug_Mode = cbDebugMode.IsChecked == true;
        }

        /// <summary>
        /// Timer elapsed.
        /// </summary>
        private async void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _timerFlag = true;

                var player = _player;
                if (player == null)
                {
                    return;
                }

                var position = await player.Position_Get_TimeAsync();
                var duration = await player.Duration_TimeAsync();

                if (_player == null)
                {
                    return;
                }

                Dispatcher.Invoke((Action)(() =>
                {
                    tbTimeline.Maximum = (int)duration.TotalSeconds;

                    lbTime.Text = position.ToString("hh\\:mm\\:ss") + " | " + duration.ToString("hh\\:mm\\:ss");

                    if (tbTimeline.Maximum >= position.TotalSeconds)
                    {
                        tbTimeline.Value = (int)position.TotalSeconds;
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _timerFlag = false;
            }
        }

        /// <summary>
        /// Destroy engine async.
        /// </summary>
        private async Task DestroyEngineAsync()
        {
            if (_player != null)
            {
                _player.OnError -= Player_OnError;
                _player.OnStop -= Player_OnStop;
                await _player.StopAsync();
                await _player.DisposeAsync();
                _player = null;
            }
        }

        /// <summary>
        /// Handles the bt select file click event.
        /// </summary>
        private void btSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                edFilename.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Tb timeline value changed.
        /// </summary>
        private async void tbTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_timerFlag && _player != null)
            {
                await _player.Position_Set_TimeAsync(TimeSpan.FromSeconds(tbTimeline.Value));
            }
        }

        /// <summary>
        /// Handles the bt start click event.
        /// </summary>
        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            edLog.Clear();

            await DestroyEngineAsync();

            CreateEngine();

            _player.Audio_OutputDevice = cbAudioOutput.Text;

            _player.Playlist_Clear();
            _player.Playlist_Add(edFilename.Text);

            await _player.PlayAsync();

            _timer.Start();
        }

        /// <summary>
        /// Handles the bt stop click event.
        /// </summary>
        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();

            if (_player != null)
            {
                await _player.StopAsync();
                await DestroyEngineAsync();
                _player = null;
            }

            tbTimeline.Value = 0;
        }

        /// <summary>
        /// Handles the bt pause click event.
        /// </summary>
        private async void btPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_player != null)
                {
                    await _player.PauseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Handles the bt resume click event.
        /// </summary>
        private async void btResume_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_player != null)
                {
                    await _player.ResumeAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Tb volume value changed.
        /// </summary>
        private void tbVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null)
            {
                _player.Audio_OutputDevice_Volume_Set(0, (int)tbVolume.Value);
            }
        }

        /// <summary>
        /// Window closing.
        /// </summary>
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();

            _isClosing = true;

            await DestroyEngineAsync();
        }
    }
}
