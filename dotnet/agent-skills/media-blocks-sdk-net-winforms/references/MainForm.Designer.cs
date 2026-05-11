namespace MediaBlocks_Simple_Video_Capture_Demo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ---- Capture-side controls ----
            cbVideoInput = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(10, 30), Size = new System.Drawing.Size(300, 28) };
            cbVideoInput.SelectedIndexChanged += cbVideoInput_SelectedIndexChanged;
            cbVideoFormat = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(10, 65), Size = new System.Drawing.Size(200, 28) };
            cbVideoFormat.SelectedIndexChanged += cbVideoFormat_SelectedIndexChanged;
            cbVideoFrameRate = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(220, 65), Size = new System.Drawing.Size(90, 28) };
            cbAudioInput = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(10, 100), Size = new System.Drawing.Size(300, 28) };
            cbAudioInput.SelectedIndexChanged += cbAudioInput_SelectedIndexChanged;
            cbAudioFormat = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(10, 135), Size = new System.Drawing.Size(300, 28) };
            cbAudioOutput = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(10, 170), Size = new System.Drawing.Size(300, 28) };
            cbCapture = new System.Windows.Forms.CheckBox { Text = "Capture to file", Location = new System.Drawing.Point(10, 205), AutoSize = true };
            edFilename = new System.Windows.Forms.TextBox { Location = new System.Drawing.Point(10, 230), Size = new System.Drawing.Size(270, 27) };
            btSelectOutput = new System.Windows.Forms.Button { Text = "...", Location = new System.Drawing.Point(285, 229), Size = new System.Drawing.Size(28, 28) };
            btSelectOutput.Click += btSelectOutput_Click;

            // ---- Transport controls ----
            btStart = new System.Windows.Forms.Button { Text = "Start", Location = new System.Drawing.Point(330, 600), Size = new System.Drawing.Size(80, 30) };
            btStart.Click += btStart_Click;
            btStop = new System.Windows.Forms.Button { Text = "Stop", Location = new System.Drawing.Point(420, 600), Size = new System.Drawing.Size(80, 30) };
            btStop.Click += btStop_Click;
            lbTime = new System.Windows.Forms.Label { Text = "00:00:00", Location = new System.Drawing.Point(520, 605), AutoSize = true };

            // ---- VisioForge VideoView — receives the preview surface ----
            VideoView1 = new VisioForge.Core.UI.WinForms.VideoView
            {
                BackColor = System.Drawing.Color.Black,
                Location = new System.Drawing.Point(330, 30),
                Size = new System.Drawing.Size(640, 560),
                Name = "VideoView1"
            };

            // ---- Log panel ----
            mmLog = new System.Windows.Forms.TextBox { Multiline = true, ScrollBars = System.Windows.Forms.ScrollBars.Vertical, Location = new System.Drawing.Point(10, 270), Size = new System.Drawing.Size(310, 320) };

            timer1 = new System.Windows.Forms.Timer(components);
            timer1.Tick += timer1_Tick;

            // ---- Form ----
            ClientSize = new System.Drawing.Size(990, 650);
            Controls.Add(cbVideoInput);
            Controls.Add(cbVideoFormat);
            Controls.Add(cbVideoFrameRate);
            Controls.Add(cbAudioInput);
            Controls.Add(cbAudioFormat);
            Controls.Add(cbAudioOutput);
            Controls.Add(cbCapture);
            Controls.Add(edFilename);
            Controls.Add(btSelectOutput);
            Controls.Add(btStart);
            Controls.Add(btStop);
            Controls.Add(lbTime);
            Controls.Add(VideoView1);
            Controls.Add(mmLog);
            Name = "MainForm";
            Text = "Media Blocks SDK .Net - WinForms Hello World";
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        #endregion

        private VisioForge.Core.UI.WinForms.VideoView VideoView1;
        private System.Windows.Forms.ComboBox cbVideoInput;
        private System.Windows.Forms.ComboBox cbVideoFormat;
        private System.Windows.Forms.ComboBox cbVideoFrameRate;
        private System.Windows.Forms.ComboBox cbAudioInput;
        private System.Windows.Forms.ComboBox cbAudioFormat;
        private System.Windows.Forms.ComboBox cbAudioOutput;
        private System.Windows.Forms.CheckBox cbCapture;
        private System.Windows.Forms.TextBox edFilename;
        private System.Windows.Forms.Button btSelectOutput;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.TextBox mmLog;
        private System.Windows.Forms.Timer timer1;
    }
}
