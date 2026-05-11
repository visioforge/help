namespace Sample
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            VideoView1 = new VisioForge.Core.UI.WinForms.VideoView();
            edFilenameOrURL = new System.Windows.Forms.TextBox();
            btSelectFile = new System.Windows.Forms.Button();
            btStart = new System.Windows.Forms.Button();
            btPause = new System.Windows.Forms.Button();
            btResume = new System.Windows.Forms.Button();
            btStop = new System.Windows.Forms.Button();
            tbTimeline = new System.Windows.Forms.TrackBar();
            lbTimeline = new System.Windows.Forms.Label();
            tbVolume = new System.Windows.Forms.TrackBar();
            lbVolume = new System.Windows.Forms.Label();
            cbPlayAudio = new System.Windows.Forms.CheckBox();
            cbAudioOutputDevice = new System.Windows.Forms.ComboBox();
            lbAudioOutputDevice = new System.Windows.Forms.Label();
            mmLog = new System.Windows.Forms.TextBox();
            lbLog = new System.Windows.Forms.Label();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            tmPosition = new System.Windows.Forms.Timer(components);

            ((System.ComponentModel.ISupportInitialize)tbTimeline).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbVolume).BeginInit();
            SuspendLayout();

            //
            // VideoView1
            //
            VideoView1.BackColor = System.Drawing.Color.Black;
            VideoView1.Location = new System.Drawing.Point(12, 12);
            VideoView1.Name = "VideoView1";
            VideoView1.Size = new System.Drawing.Size(720, 405);
            VideoView1.TabIndex = 0;

            //
            // edFilenameOrURL
            //
            edFilenameOrURL.Location = new System.Drawing.Point(12, 430);
            edFilenameOrURL.Name = "edFilenameOrURL";
            edFilenameOrURL.Size = new System.Drawing.Size(580, 23);
            edFilenameOrURL.TabIndex = 1;

            //
            // btSelectFile
            //
            btSelectFile.Location = new System.Drawing.Point(598, 429);
            btSelectFile.Name = "btSelectFile";
            btSelectFile.Size = new System.Drawing.Size(134, 25);
            btSelectFile.TabIndex = 2;
            btSelectFile.Text = "Select File...";
            btSelectFile.UseVisualStyleBackColor = true;
            btSelectFile.Click += btSelectFile_Click;

            //
            // btStart
            //
            btStart.Location = new System.Drawing.Point(12, 465);
            btStart.Name = "btStart";
            btStart.Size = new System.Drawing.Size(80, 28);
            btStart.TabIndex = 3;
            btStart.Text = "Play";
            btStart.UseVisualStyleBackColor = true;
            btStart.Click += btStart_Click;

            //
            // btPause
            //
            btPause.Location = new System.Drawing.Point(98, 465);
            btPause.Name = "btPause";
            btPause.Size = new System.Drawing.Size(80, 28);
            btPause.TabIndex = 4;
            btPause.Text = "Pause";
            btPause.UseVisualStyleBackColor = true;
            btPause.Click += btPause_Click;

            //
            // btResume
            //
            btResume.Location = new System.Drawing.Point(184, 465);
            btResume.Name = "btResume";
            btResume.Size = new System.Drawing.Size(80, 28);
            btResume.TabIndex = 5;
            btResume.Text = "Resume";
            btResume.UseVisualStyleBackColor = true;
            btResume.Click += btResume_Click;

            //
            // btStop
            //
            btStop.Location = new System.Drawing.Point(270, 465);
            btStop.Name = "btStop";
            btStop.Size = new System.Drawing.Size(80, 28);
            btStop.TabIndex = 6;
            btStop.Text = "Stop";
            btStop.UseVisualStyleBackColor = true;
            btStop.Click += btStop_Click;

            //
            // tbTimeline
            //
            tbTimeline.Location = new System.Drawing.Point(12, 505);
            tbTimeline.Maximum = 100;
            tbTimeline.Name = "tbTimeline";
            tbTimeline.Size = new System.Drawing.Size(580, 45);
            tbTimeline.TabIndex = 7;
            tbTimeline.TickStyle = System.Windows.Forms.TickStyle.None;
            tbTimeline.Scroll += tbTimeline_Scroll;

            //
            // lbTimeline
            //
            lbTimeline.AutoSize = true;
            lbTimeline.Location = new System.Drawing.Point(598, 510);
            lbTimeline.Name = "lbTimeline";
            lbTimeline.Size = new System.Drawing.Size(110, 15);
            lbTimeline.TabIndex = 8;
            lbTimeline.Text = "00:00:00 | 00:00:00";

            //
            // tbVolume
            //
            tbVolume.Location = new System.Drawing.Point(370, 460);
            tbVolume.Maximum = 100;
            tbVolume.Name = "tbVolume";
            tbVolume.Size = new System.Drawing.Size(150, 45);
            tbVolume.TabIndex = 9;
            tbVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            tbVolume.Value = 80;
            tbVolume.Scroll += tbVolume_Scroll;

            //
            // lbVolume
            //
            lbVolume.AutoSize = true;
            lbVolume.Location = new System.Drawing.Point(526, 470);
            lbVolume.Name = "lbVolume";
            lbVolume.Size = new System.Drawing.Size(50, 15);
            lbVolume.TabIndex = 10;
            lbVolume.Text = "Volume";

            //
            // cbPlayAudio
            //
            cbPlayAudio.AutoSize = true;
            cbPlayAudio.Checked = true;
            cbPlayAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            cbPlayAudio.Location = new System.Drawing.Point(598, 470);
            cbPlayAudio.Name = "cbPlayAudio";
            cbPlayAudio.Size = new System.Drawing.Size(82, 19);
            cbPlayAudio.TabIndex = 11;
            cbPlayAudio.Text = "Play audio";
            cbPlayAudio.UseVisualStyleBackColor = true;

            //
            // lbAudioOutputDevice
            //
            lbAudioOutputDevice.AutoSize = true;
            lbAudioOutputDevice.Location = new System.Drawing.Point(12, 562);
            lbAudioOutputDevice.Name = "lbAudioOutputDevice";
            lbAudioOutputDevice.Size = new System.Drawing.Size(110, 15);
            lbAudioOutputDevice.TabIndex = 12;
            lbAudioOutputDevice.Text = "Audio output device:";

            //
            // cbAudioOutputDevice
            //
            cbAudioOutputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAudioOutputDevice.Location = new System.Drawing.Point(140, 558);
            cbAudioOutputDevice.Name = "cbAudioOutputDevice";
            cbAudioOutputDevice.Size = new System.Drawing.Size(300, 23);
            cbAudioOutputDevice.TabIndex = 13;

            //
            // lbLog
            //
            lbLog.AutoSize = true;
            lbLog.Location = new System.Drawing.Point(12, 595);
            lbLog.Name = "lbLog";
            lbLog.Size = new System.Drawing.Size(28, 15);
            lbLog.TabIndex = 14;
            lbLog.Text = "Log:";

            //
            // mmLog
            //
            mmLog.Location = new System.Drawing.Point(12, 613);
            mmLog.Multiline = true;
            mmLog.Name = "mmLog";
            mmLog.ReadOnly = true;
            mmLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            mmLog.Size = new System.Drawing.Size(720, 80);
            mmLog.TabIndex = 15;

            //
            // tmPosition
            //
            tmPosition.Interval = 500;
            tmPosition.Tick += tmPosition_Tick;

            //
            // Form1
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(744, 705);
            Controls.Add(VideoView1);
            Controls.Add(edFilenameOrURL);
            Controls.Add(btSelectFile);
            Controls.Add(btStart);
            Controls.Add(btPause);
            Controls.Add(btResume);
            Controls.Add(btStop);
            Controls.Add(tbTimeline);
            Controls.Add(lbTimeline);
            Controls.Add(tbVolume);
            Controls.Add(lbVolume);
            Controls.Add(cbPlayAudio);
            Controls.Add(lbAudioOutputDevice);
            Controls.Add(cbAudioOutputDevice);
            Controls.Add(lbLog);
            Controls.Add(mmLog);
            Name = "Form1";
            Text = "Media Player SDK X — WinForms";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;

            ((System.ComponentModel.ISupportInitialize)tbTimeline).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private VisioForge.Core.UI.WinForms.VideoView VideoView1;
        private System.Windows.Forms.TextBox edFilenameOrURL;
        private System.Windows.Forms.Button btSelectFile;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btPause;
        private System.Windows.Forms.Button btResume;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.TrackBar tbTimeline;
        private System.Windows.Forms.Label lbTimeline;
        private System.Windows.Forms.TrackBar tbVolume;
        private System.Windows.Forms.Label lbVolume;
        private System.Windows.Forms.CheckBox cbPlayAudio;
        private System.Windows.Forms.ComboBox cbAudioOutputDevice;
        private System.Windows.Forms.Label lbAudioOutputDevice;
        private System.Windows.Forms.TextBox mmLog;
        private System.Windows.Forms.Label lbLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer tmPosition;
    }
}
