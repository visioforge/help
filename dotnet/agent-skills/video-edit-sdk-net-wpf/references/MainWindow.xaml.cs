using System;
using System.IO;
using System.Windows;
using VisioForge.Core.Types.Events;
using VisioForge.Core.VideoEdit;

namespace Cut_Video_File
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The video editing engine instance.
        /// </summary>
        private VideoEditCore _core;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btSelectSourceFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".mp4",
                Filter = "Video files|*.mp4;*.avi;*.mpg;*.mkv;*.ts;*.wmv;*.vob|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                edSourceVideoFile.Text = dlg.FileName;
            }
        }

        private void btSelectOutputFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".mp4",
                Filter = "Video files|*.mp4;*.avi;*.mpg;*.mkv;*.ts;*.wmv;*.vob|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                edOutputVideoFile.Text = dlg.FileName;
            }
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(edSourceVideoFile.Text))
            {
                MessageBox.Show("Please select a source video file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(edSourceVideoFile.Text))
            {
                MessageBox.Show("Source video file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(edOutputVideoFile.Text))
            {
                MessageBox.Show("Please select an output video file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(edStartTime.Text, out int startTime) || startTime < 0)
            {
                MessageBox.Show("Please enter a valid start time (non-negative integer).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(edStopTime.Text, out int stopTime) || stopTime < 0)
            {
                MessageBox.Show("Please enter a valid stop time (non-negative integer).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (stopTime <= startTime)
            {
                MessageBox.Show("Stop time must be greater than start time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            btStart.IsEnabled = false;

            try
            {
                // FastEdit_CutFileAsync does a stream-copy cut without re-encoding when the
                // input is a supported MP4 / MOV / M4A. For arbitrary input formats or when
                // you need a cut that is also re-encoded into a different format, build a
                // timeline (Input_AddVideoFileAsync + Input_AddAudioFileAsync) and call
                // StartAsync with Mode = VideoEditMode.Convert. See the SKILL.md.
                await _core.FastEdit_CutFileAsync(
                    edSourceVideoFile.Text,
                    TimeSpan.FromSeconds(startTime),
                    TimeSpan.FromSeconds(stopTime),
                    edOutputVideoFile.Text);
            }
            catch (Exception ex)
            {
                edLog.Text += ex.Message + Environment.NewLine;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btStart.IsEnabled = true;
            }
        }

        private void VideoEdit1_OnProgress(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(() => { ProgressBar1.Value = e.Progress; });
        }

        private void VideoEdit1_OnStop(object sender, StopEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar1.Value = 0;
                MessageBox.Show(e.Successful ? "Completed successfully" : "Stopped with error");
            });
        }

        private void VideoEdit1_OnError(object sender, ErrorsEventArgs e)
        {
            Dispatcher.Invoke(() => { edLog.Text += e.Message + Environment.NewLine; });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _core = new VideoEditCore();
            _core.OnError += VideoEdit1_OnError;
            _core.OnStop += VideoEdit1_OnStop;
            _core.OnProgress += VideoEdit1_OnProgress;

            // To register a purchased licence, load the .vflicense bytes and call:
            //   var cert = File.ReadAllBytes("path/to/your.vflicense");
            //   await _core.SetLicenseCertificateAsync(cert);
            // This must run after the constructor and before the first StartAsync /
            // FastEdit_*Async call. Without it, the SDK runs in 30-day trial mode.
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_core != null)
            {
                _core.OnError -= VideoEdit1_OnError;
                _core.OnStop -= VideoEdit1_OnStop;
                _core.OnProgress -= VideoEdit1_OnProgress;

                _core.Stop();
                _core.Dispose();
                _core = null;
            }
        }
    }
}
