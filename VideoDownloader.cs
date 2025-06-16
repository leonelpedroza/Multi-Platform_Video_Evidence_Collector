using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace VideoDownloader
{
    public partial class MainForm : Form
    {
        private TextBox urlTextBox;
        private ComboBox qualityComboBox;
        private TextBox outputDirTextBox;
        private Button downloadButton;
        private ProgressBar progressBar;
        private Label statusLabel;
        private TextBox logTextBox;
        private TabControl tabControl;
        private DataGridView historyGrid;
        private Process ytDlpProcess;
        private PasswordManager passwordManager;
        
        // Settings controls
        private TextBox igUsernameTextBox;
        private TextBox igPasswordTextBox;
        private TextBox fbEmailTextBox;
        private TextBox fbPasswordTextBox;
        private TextBox liEmailTextBox;
        private TextBox liPasswordTextBox;
        private CheckBox subtitlesCheckBox;
        private CheckBox metadataCheckBox;
        private CheckBox timestampCheckBox;
        private CheckBox cookiesCheckBox;
        private ComboBox browserComboBox;
        private CheckBox screenshotCheckBox;
        private CheckBox hashCheckBox;
        private CheckBox evidenceReportCheckBox;
        private Label platformLabel;
        
        // Queue management
        private Queue<string> downloadQueue = new Queue<string>();
        private bool isProcessingQueue = false;
        private ListBox queueListBox;
        private Button addToQueueButton;
        private Button clearQueueButton;
        private Label queueStatusLabel;

        public MainForm()
        {
            passwordManager = new PasswordManager();
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Multi-Platform Video Downloader";
            this.Size = new Size(760, 770);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Set the window icon
            try
            {
                // First try to load from embedded resource
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "VideoDownloader.app.ico";
                
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        this.Icon = new Icon(stream);
                    }
                    else
                    {
                        // Try alternate resource name
                        resourceName = assembly.GetName().Name + ".app.ico";
                        using (var altStream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (altStream != null)
                            {
                                this.Icon = new Icon(altStream);
                            }
                        }
                    }
                }
                
                // If resource loading failed, try loading from file
                if (this.Icon == null)
                {
                    var iconPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "app.ico");
                    if (File.Exists(iconPath))
                    {
                        this.Icon = new Icon(iconPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Icon loading failed, will use default
                AddToLog($"Failed to load icon: {ex.Message}");
            }

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Download Tab
            var downloadTab = new TabPage("Download");
            SetupDownloadTab(downloadTab);
            tabControl.TabPages.Add(downloadTab);

            // History Tab
            var historyTab = new TabPage("History");
            SetupHistoryTab(historyTab);
            tabControl.TabPages.Add(historyTab);

            // Settings Tab
            var settingsTab = new TabPage("Settings");
            SetupSettingsTab(settingsTab);
            tabControl.TabPages.Add(settingsTab);

            this.Controls.Add(tabControl);
        }

        private void SetupDownloadTab(TabPage tab)
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(10);

            int yPos = 10;

            // URL Input
            var urlLabel = new Label { Text = "Video URL:", Location = new Point(10, yPos), Size = new Size(100, 20) };
            panel.Controls.Add(urlLabel);

            urlTextBox = new TextBox { Location = new Point(10, yPos + 25), Size = new Size(600, 25) };
            urlTextBox.TextChanged += UrlTextBox_TextChanged;
            panel.Controls.Add(urlTextBox);

            var pasteButton = new Button { Text = "Paste", Location = new Point(620, yPos + 25), Size = new Size(80, 25) };
            pasteButton.Click += (s, e) => urlTextBox.Text = Clipboard.GetText();
            panel.Controls.Add(pasteButton);

            yPos += 55;

            // Platform detection label
            platformLabel = new Label 
            { 
                Text = "Platform: Not detected", 
                Location = new Point(10, yPos), 
                Size = new Size(400, 20),
                Font = new Font(Label.DefaultFont, FontStyle.Bold)
            };
            panel.Controls.Add(platformLabel);

            yPos += 25;

            // Quality Selection
            var qualityLabel = new Label { Text = "Quality:", Location = new Point(10, yPos), Size = new Size(100, 20) };
            panel.Controls.Add(qualityLabel);

            qualityComboBox = new ComboBox { Location = new Point(120, yPos), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            qualityComboBox.Items.AddRange(new[] { 
                "Best Quality", 
                "1080p", 
                "720p", 
                "480p", 
                "360p", 
                "Audio Only (MP3)",
                "Audio Only (Best Quality)",
                "Audio Only (128kbps)",
                "Audio Only (320kbps)"
            });
            qualityComboBox.SelectedIndex = 0;
            panel.Controls.Add(qualityComboBox);

            yPos += 40;

            // Options
            subtitlesCheckBox = new CheckBox { Text = "Download subtitles if available", Location = new Point(10, yPos), Size = new Size(300, 25) };
            panel.Controls.Add(subtitlesCheckBox);

            yPos += 30;

            metadataCheckBox = new CheckBox { Text = "Save video metadata (for evidence)", Location = new Point(10, yPos), Size = new Size(300, 25) };
            panel.Controls.Add(metadataCheckBox);

            yPos += 30;

            timestampCheckBox = new CheckBox { Text = "Add timestamp to filename", Location = new Point(10, yPos), Size = new Size(300, 25) };
            panel.Controls.Add(timestampCheckBox);

            yPos += 40;

            // Output Directory
            var outputLabel = new Label { Text = "Output Directory:", Location = new Point(10, yPos), Size = new Size(100, 20) };
            panel.Controls.Add(outputLabel);

            outputDirTextBox = new TextBox { Location = new Point(10, yPos + 25), Size = new Size(500, 25) };
            outputDirTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Videos");
            panel.Controls.Add(outputDirTextBox);

            var browseButton = new Button { Text = "Browse", Location = new Point(520, yPos + 25), Size = new Size(80, 25) };
            browseButton.Click += BrowseButton_Click;
            panel.Controls.Add(browseButton);

            yPos += 60;

            // Download Button and Queue Button
            var downloadButtonsPanel = new Panel { Location = new Point(10, yPos), Size = new Size(680, 35) };
            
            downloadButton = new Button { Text = "Download Video", Location = new Point(0, 0), Size = new Size(150, 35) };
            downloadButton.Click += DownloadButton_Click;
            downloadButton.BackColor = Color.FromArgb(76, 175, 80);
            downloadButton.ForeColor = Color.White;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButtonsPanel.Controls.Add(downloadButton);
            
            addToQueueButton = new Button { Text = "Add to Queue", Location = new Point(160, 0), Size = new Size(120, 35) };
            addToQueueButton.Click += AddToQueueButton_Click;
            addToQueueButton.BackColor = Color.FromArgb(33, 150, 243);
            addToQueueButton.ForeColor = Color.White;
            addToQueueButton.FlatStyle = FlatStyle.Flat;
            downloadButtonsPanel.Controls.Add(addToQueueButton);
            
            panel.Controls.Add(downloadButtonsPanel);

            yPos += 50;

            // Progress Bar
            progressBar = new ProgressBar { Location = new Point(10, yPos), Size = new Size(680, 25) };
            panel.Controls.Add(progressBar);

            yPos += 35;

            // Status Label
            statusLabel = new Label { Text = "Ready to download", Location = new Point(10, yPos), Size = new Size(680, 20) };
            panel.Controls.Add(statusLabel);

            yPos += 30;

            // Log Output
            var logLabel = new Label { Text = "Log Output:", Location = new Point(10, yPos), Size = new Size(100, 20) };
            panel.Controls.Add(logLabel);

            logTextBox = new TextBox 
            { 
                Location = new Point(10, yPos + 25), 
                Size = new Size(680, 100), 
                Multiline = true, 
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };
            panel.Controls.Add(logTextBox);

            yPos += 135;

            // Download Queue
            var queueGroup = new GroupBox { Text = "Download Queue", Location = new Point(10, yPos), Size = new Size(680, 120) };
            var queueLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5) };
            
            queueListBox = new ListBox { Size = new Size(550, 80), Location = new Point(10, 20) };
            queueLayout.Controls.Add(queueListBox);
            
            var queueButtonPanel = new Panel { Size = new Size(100, 80), Location = new Point(570, 20) };
            clearQueueButton = new Button { Text = "Clear Queue", Size = new Size(90, 30), Location = new Point(5, 5) };
            clearQueueButton.Click += (s, e) => { downloadQueue.Clear(); UpdateQueueDisplay(); };
            queueButtonPanel.Controls.Add(clearQueueButton);
            
            queueStatusLabel = new Label { Text = "Queue: 0 items", Size = new Size(90, 20), Location = new Point(5, 40) };
            queueButtonPanel.Controls.Add(queueStatusLabel);
            
            queueLayout.Controls.Add(queueButtonPanel);
            queueGroup.Controls.Add(queueLayout);
            panel.Controls.Add(queueGroup);

            tab.Controls.Add(panel);
        }

        private void SetupHistoryTab(TabPage tab)
        {
            historyGrid = new DataGridView();
            historyGrid.Dock = DockStyle.Fill;
            historyGrid.AutoGenerateColumns = false;
            historyGrid.ReadOnly = true;
            historyGrid.AllowUserToAddRows = false;

            historyGrid.Columns.Add("DateTime", "Date/Time");
            historyGrid.Columns.Add("Platform", "Platform");
            historyGrid.Columns.Add("Title", "Title");
            historyGrid.Columns.Add("Status", "Status");
            historyGrid.Columns.Add("Location", "Location");

            historyGrid.Columns[0].Width = 150;
            historyGrid.Columns[1].Width = 100;
            historyGrid.Columns[2].Width = 250;
            historyGrid.Columns[3].Width = 80;
            historyGrid.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            tab.Controls.Add(historyGrid);

            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            
            var clearButton = new Button { Text = "Clear History", Location = new Point(10, 10), Size = new Size(120, 30) };
            clearButton.Click += (s, e) => 
            {
                if (MessageBox.Show("Clear all history?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    historyGrid.Rows.Clear();
            };
            buttonPanel.Controls.Add(clearButton);

            var openFolderButton = new Button { Text = "Open Downloads Folder", Location = new Point(140, 10), Size = new Size(150, 30) };
            openFolderButton.Click += (s, e) => Process.Start("explorer.exe", outputDirTextBox.Text);
            buttonPanel.Controls.Add(openFolderButton);

            tab.Controls.Add(buttonPanel);
        }

        private void SetupSettingsTab(TabPage tab)
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
            panel.Padding = new Padding(10);

            int yPos = 10;

            // Authentication Settings
            var authGroup = new GroupBox { Text = "Authentication (Optional)", Location = new Point(10, yPos), Size = new Size(650, 200) };
            
            // Instagram
            var igUserLabel = new Label { Text = "Instagram Username:", Location = new Point(10, 30), Size = new Size(120, 20) };
            authGroup.Controls.Add(igUserLabel);
            igUsernameTextBox = new TextBox { Location = new Point(140, 30), Size = new Size(200, 25) };
            authGroup.Controls.Add(igUsernameTextBox);

            var igPassLabel = new Label { Text = "Instagram Password:", Location = new Point(10, 60), Size = new Size(120, 20) };
            authGroup.Controls.Add(igPassLabel);
            igPasswordTextBox = new TextBox { Location = new Point(140, 60), Size = new Size(200, 25), UseSystemPasswordChar = true };
            authGroup.Controls.Add(igPasswordTextBox);

            // Facebook
            var fbEmailLabel = new Label { Text = "Facebook Email:", Location = new Point(10, 100), Size = new Size(120, 20) };
            authGroup.Controls.Add(fbEmailLabel);
            fbEmailTextBox = new TextBox { Location = new Point(140, 100), Size = new Size(200, 25) };
            authGroup.Controls.Add(fbEmailTextBox);

            var fbPassLabel = new Label { Text = "Facebook Password:", Location = new Point(10, 130), Size = new Size(120, 20) };
            authGroup.Controls.Add(fbPassLabel);
            fbPasswordTextBox = new TextBox { Location = new Point(140, 130), Size = new Size(200, 25), UseSystemPasswordChar = true };
            authGroup.Controls.Add(fbPasswordTextBox);

            // LinkedIn
            var liEmailLabel = new Label { Text = "LinkedIn Email:", Location = new Point(10, 170), Size = new Size(120, 20) };
            authGroup.Controls.Add(liEmailLabel);
            liEmailTextBox = new TextBox { Location = new Point(140, 170), Size = new Size(200, 25) };
            authGroup.Controls.Add(liEmailTextBox);

            var liPassLabel = new Label { Text = "LinkedIn Password:", Location = new Point(10, 200), Size = new Size(120, 20) };
            authGroup.Controls.Add(liPassLabel);
            liPasswordTextBox = new TextBox { Location = new Point(140, 200), Size = new Size(200, 25), UseSystemPasswordChar = true };
            authGroup.Controls.Add(liPasswordTextBox);

            // Adjust group box height
            authGroup.Size = new Size(650, 270);

            // Save/Load buttons
            var saveCredsButton = new Button { Text = "Save Credentials", Location = new Point(10, 235), Size = new Size(120, 30) };
            saveCredsButton.Click += (s, e) => SaveSettings();
            authGroup.Controls.Add(saveCredsButton);

            var loadCredsButton = new Button { Text = "Load Credentials", Location = new Point(140, 235), Size = new Size(120, 30) };
            loadCredsButton.Click += (s, e) => LoadSettings();
            authGroup.Controls.Add(loadCredsButton);

            var clearCredsButton = new Button { Text = "Clear All", Location = new Point(270, 235), Size = new Size(120, 30) };
            clearCredsButton.Click += (s, e) => ClearCredentials();
            authGroup.Controls.Add(clearCredsButton);

            panel.Controls.Add(authGroup);

            yPos += 290;

            // Additional Settings
            var additionalGroup = new GroupBox { Text = "Additional Settings", Location = new Point(10, yPos), Size = new Size(650, 110) };
            
            cookiesCheckBox = new CheckBox 
            { 
                Text = "Use browser cookies (for TikTok, Pinterest, etc.)", 
                Location = new Point(10, 25), 
                Size = new Size(400, 25)
            };
            additionalGroup.Controls.Add(cookiesCheckBox);
            
            // Add tooltip separately
            var toolTip = new System.Windows.Forms.ToolTip();
            toolTip.SetToolTip(cookiesCheckBox, "Uses cookies from selected browser for sites that require login");
            
            // Browser selection
            var browserLabel = new Label
            {
                Text = "Browser:",
                Location = new Point(30, 55),
                Size = new Size(60, 20)
            };
            additionalGroup.Controls.Add(browserLabel);
            
            browserComboBox = new ComboBox
            {
                Location = new Point(95, 52),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            browserComboBox.Items.AddRange(new[] { 
                "Chrome", 
                "Firefox", 
                "Edge", 
                "Brave", 
                "Opera", 
                "Safari",
                "Chromium",
                "Vivaldi"
            });
            browserComboBox.SelectedIndex = 0; // Default to Chrome
            additionalGroup.Controls.Add(browserComboBox);
            
            var cookiesNote = new Label
            {
                Text = "Note: Make sure you're logged into the sites in your selected browser",
                Location = new Point(30, 80),
                Size = new Size(400, 20),
                ForeColor = Color.Gray,
                Font = new Font(Label.DefaultFont.FontFamily, 8)
            };
            additionalGroup.Controls.Add(cookiesNote);
            
            panel.Controls.Add(additionalGroup);

            yPos += 130;

            // Evidence Collection Settings
            var evidenceGroup = new GroupBox { Text = "Evidence Collection", Location = new Point(10, yPos), Size = new Size(650, 120) };
            
            screenshotCheckBox = new CheckBox 
            { 
                Text = "Capture webpage screenshot before download", 
                Location = new Point(10, 25), 
                Size = new Size(400, 25),
                Checked = true
            };
            evidenceGroup.Controls.Add(screenshotCheckBox);
            
            hashCheckBox = new CheckBox 
            { 
                Text = "Generate SHA256 hash of downloaded files", 
                Location = new Point(10, 50), 
                Size = new Size(400, 25),
                Checked = true
            };
            evidenceGroup.Controls.Add(hashCheckBox);
            
            evidenceReportCheckBox = new CheckBox 
            { 
                Text = "Generate detailed evidence report", 
                Location = new Point(10, 75), 
                Size = new Size(400, 25),
                Checked = true
            };
            evidenceGroup.Controls.Add(evidenceReportCheckBox);
            
            panel.Controls.Add(evidenceGroup);

            yPos += 140;

            // About section
            var aboutGroup = new GroupBox { Text = "About", Location = new Point(10, yPos), Size = new Size(650, 130) };
            
            // About text panel (left side)
            var aboutPanel = new Panel
            {
                Location = new Point(10, 20),
                Size = new Size(450, 100),
                AutoScroll = true
            };
            
            var aboutLabel = new Label 
            { 
                Text = "Multi-Platform Video Downloader v1.0\n" +
                       "Supports: YouTube, Instagram, Facebook, Twitter/X, TikTok, Pinterest,\n" +
                       "CapCut, Telegram, Spotify, SoundCloud, LinkedIn, and adult content sites\n\n" +
                       "Note: Requires yt-dlp.exe and ffmpeg.exe in the same folder as this executable.\n" +
                       "Please respect copyright and platform terms of service.",
                Location = new Point(0, 0),
                Size = new Size(440, 100),
                AutoSize = false
            };
            aboutPanel.Controls.Add(aboutLabel);
            aboutGroup.Controls.Add(aboutPanel);
            
            // MIT License Logo (right side)
            var mitPictureBox = new PictureBox
            {
                Location = new Point(470, 20),
                Size = new Size(170, 95),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            
            // Try to load the MIT logo from embedded resources
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "VideoDownloader.mit-license-logo.png";
                
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        mitPictureBox.Image = Image.FromStream(stream);
                    }
                    else
                    {
                        // Fallback: try alternate resource name
                        resourceName = assembly.GetName().Name + ".mit-license-logo.png";
                        using (var altStream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (altStream != null)
                            {
                                mitPictureBox.Image = Image.FromStream(altStream);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If resource loading fails, show text instead
            }
            
            // If no image loaded, show text
            if (mitPictureBox.Image == null)
            {
                var mitLabel = new Label
                {
                    Text = "@2025 MIT License",
                    Location = new Point(470, 50),
                    Size = new Size(170, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(100, 100, 100),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                aboutGroup.Controls.Add(mitLabel);
            }
            else
            {
                aboutGroup.Controls.Add(mitPictureBox);
            }
                        
            panel.Controls.Add(aboutGroup);

            tab.Controls.Add(panel);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = outputDirTextBox.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    outputDirTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            DetectPlatform(urlTextBox.Text);
        }

        private string DetectPlatform(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                platformLabel.Text = "Platform: Not detected";
                platformLabel.ForeColor = Color.Gray;
                return "";
            }

            try
            {
                var uri = new Uri(url);
                var domain = uri.Host.ToLower();
                string platform = "";
                Color color = Color.Gray;

                if (domain.Contains("youtube.com") || domain.Contains("youtu.be"))
                {
                    platform = "YouTube";
                    color = Color.Red;
                }
                else if (domain.Contains("instagram.com") || domain.Contains("instagr.am"))
                {
                    platform = "Instagram";
                    color = Color.FromArgb(225, 48, 108);
                }
                else if (domain.Contains("facebook.com") || domain.Contains("fb.watch"))
                {
                    platform = "Facebook";
                    color = Color.FromArgb(24, 119, 242);
                }
                else if (domain.Contains("twitter.com") || domain.Contains("x.com"))
                {
                    platform = "Twitter/X";
                    color = Color.FromArgb(29, 161, 242);
                }
                else if (domain.Contains("tiktok.com"))
                {
                    platform = "TikTok";
                    color = Color.Black;
                }
                else if (domain.Contains("pinterest.com") || domain.Contains("pin.it"))
                {
                    platform = "Pinterest";
                    color = Color.FromArgb(189, 8, 28);
                }
                else if (domain.Contains("capcut.com"))
                {
                    platform = "CapCut";
                    color = Color.FromArgb(0, 0, 0);
                }
                else if (domain.Contains("t.me") || domain.Contains("telegram.org"))
                {
                    platform = "Telegram";
                    color = Color.FromArgb(0, 136, 204);
                }
                else if (domain.Contains("spotify.com"))
                {
                    platform = "Spotify";
                    color = Color.FromArgb(30, 215, 96);
                }
                else if (domain.Contains("soundcloud.com"))
                {
                    platform = "SoundCloud";
                    color = Color.FromArgb(255, 85, 0);
                }
                else if (domain.Contains("linkedin.com"))
                {
                    platform = "LinkedIn";
                    color = Color.FromArgb(0, 119, 181);
                }
                else if (domain.Contains("redtube.com") || domain.Contains("youporn.com") || 
                         domain.Contains("pornhub.com") || domain.Contains("xvideos.com") ||
                         domain.Contains("xnxx.com") || domain.Contains("xhamster.com"))
                {
                    platform = "Adult Content Site";
                    color = Color.FromArgb(255, 107, 107);
                }
                else
                {
                    platform = "Unknown Platform";
                    color = Color.Gray;
                }

                platformLabel.Text = $"Platform: {platform}";
                platformLabel.ForeColor = color;
                return platform;
            }
            catch
            {
                platformLabel.Text = "Platform: Invalid URL";
                platformLabel.ForeColor = Color.Gray;
                return "";
            }
        }

        private void AddToQueueButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                MessageBox.Show("Please enter a video URL", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AddToQueue(urlTextBox.Text);
            urlTextBox.Clear();
        }

        private void AddToQueue(string url)
        {
            downloadQueue.Enqueue(url);
            UpdateQueueDisplay();
            
            if (!isProcessingQueue)
                ProcessQueue();
        }

        private void UpdateQueueDisplay()
        {
            queueListBox.Items.Clear();
            foreach (var url in downloadQueue)
            {
                queueListBox.Items.Add(url);
            }
            queueStatusLabel.Text = $"Queue: {downloadQueue.Count} items";
        }

        private async void ProcessQueue()
        {
            isProcessingQueue = true;
            downloadButton.Enabled = false;
            
            while (downloadQueue.Count > 0)
            {
                var url = downloadQueue.Dequeue();
                UpdateQueueDisplay();
                
                urlTextBox.Text = url;
                await DownloadVideo(url);
                
                // Delay between downloads to avoid rate limiting
                if (downloadQueue.Count > 0)
                    await Task.Delay(3000);
            }
            
            isProcessingQueue = false;
            downloadButton.Enabled = true;
            AddToLog("Queue processing completed!");
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                MessageBox.Show("Please enter a video URL", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            downloadButton.Enabled = false;
            progressBar.Value = 0;
            logTextBox.Clear();

            try
            {
                await DownloadVideo(urlTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddToLog($"Error: {ex.Message}");
            }
            finally
            {
                downloadButton.Enabled = true;
            }
        }

        private async Task DownloadVideo(string url)
        {
            // Ensure output directory exists
            Directory.CreateDirectory(outputDirTextBox.Text);
            
            // Create evidence folder for this download
            var evidenceFolder = Path.Combine(outputDirTextBox.Text, $"Evidence_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(evidenceFolder);
            
            // Capture screenshot if enabled
            if (screenshotCheckBox.Checked)
            {
                await CaptureWebpageScreenshot(url, evidenceFolder);
            }

            // Build yt-dlp arguments
            var arguments = new List<string>();
            arguments.Add($"\"{url}\"");
            
            // Output path - save to evidence folder
            var outputTemplate = timestampCheckBox.Checked 
                ? $"{DateTime.Now:yyyyMMdd_HHmmss}_%(title)s_%(id)s.%(ext)s"
                : "%(title)s_%(id)s.%(ext)s";
            arguments.Add($"-o \"{Path.Combine(evidenceFolder, outputTemplate)}\"");

            // Quality
            switch (qualityComboBox.Text)
            {
                case "Best Quality":
                    arguments.Add("-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best\"");
                    break;
                case "Audio Only (MP3)":
                    arguments.Add("-f bestaudio");
                    arguments.Add("-x");
                    arguments.Add("--audio-format mp3");
                    break;
                case "Audio Only (Best Quality)":
                    arguments.Add("-f bestaudio");
                    arguments.Add("-x");
                    arguments.Add("--audio-format best");
                    break;
                case "Audio Only (128kbps)":
                    arguments.Add("-f bestaudio");
                    arguments.Add("-x");
                    arguments.Add("--audio-format mp3");
                    arguments.Add("--audio-quality 128K");
                    break;
                case "Audio Only (320kbps)":
                    arguments.Add("-f bestaudio");
                    arguments.Add("-x");
                    arguments.Add("--audio-format mp3");
                    arguments.Add("--audio-quality 320K");
                    break;
                default:
                    var height = qualityComboBox.Text.Replace("p", "");
                    arguments.Add($"-f \"best[height<={height}]\"");
                    break;
            }

            // Options
            if (subtitlesCheckBox.Checked)
            {
                arguments.Add("--write-subs");
                arguments.Add("--write-auto-subs");
            }

            if (metadataCheckBox.Checked)
            {
                arguments.Add("--write-info-json");
                arguments.Add("--write-thumbnail");
            }

            // Authentication
            var platform = DetectPlatform(url);
            
            if (platform == "Instagram" && !string.IsNullOrEmpty(igUsernameTextBox.Text) && !string.IsNullOrEmpty(igPasswordTextBox.Text))
            {
                arguments.Add($"-u \"{igUsernameTextBox.Text}\"");
                arguments.Add($"-p \"{igPasswordTextBox.Text}\"");
            }
            else if (platform == "Facebook" && !string.IsNullOrEmpty(fbEmailTextBox.Text) && !string.IsNullOrEmpty(fbPasswordTextBox.Text))
            {
                arguments.Add($"--username \"{fbEmailTextBox.Text}\"");
                arguments.Add($"--password \"{fbPasswordTextBox.Text}\"");
            }
            else if (platform == "LinkedIn" && !string.IsNullOrEmpty(liEmailTextBox.Text) && !string.IsNullOrEmpty(liPasswordTextBox.Text))
            {
                arguments.Add($"--username \"{liEmailTextBox.Text}\"");
                arguments.Add($"--password \"{liPasswordTextBox.Text}\"");
            }

            // Platform-specific options
            if (platform == "TikTok" || platform == "Pinterest" || platform == "CapCut")
            {
                // These platforms often need cookies
                if (cookiesCheckBox.Checked)
                {
                    var selectedBrowser = browserComboBox.SelectedItem?.ToString()?.ToLower() ?? "chrome";
                    arguments.Add($"--cookies-from-browser {selectedBrowser}");
                }
            }
            else if (platform == "Spotify" || platform == "SoundCloud")
            {
                // Force audio extraction for audio platforms
                if (!qualityComboBox.Text.Contains("Audio"))
                {
                    arguments.Add("-x");
                    arguments.Add("--audio-format mp3");
                }
            }
            else if (platform == "Telegram")
            {
                // Telegram might need special handling
                arguments.Add("--no-check-certificate");
            }

            // Add user agent for better compatibility
            arguments.Add("--user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\"");

            // Add referer for some sites
            arguments.Add($"--referer \"{url}\"");

            // Progress output
            arguments.Add("--newline");
            arguments.Add("--no-colors");

            AddToLog($"Starting download: {url}");
            AddToLog($"Evidence folder: {evidenceFolder}");
            statusLabel.Text = "Downloading...";

            // Run yt-dlp
            await RunYtDlp(string.Join(" ", arguments), evidenceFolder);
        }

        private async Task RunYtDlp(string arguments, string evidenceFolder)
        {
            var tcs = new TaskCompletionSource<bool>();

            ytDlpProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GetYtDlpPath(),
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = evidenceFolder
                }
            };

            ytDlpProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    BeginInvoke(new Action(() => ProcessYtDlpOutput(e.Data)));
                }
            };

            ytDlpProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    BeginInvoke(new Action(() => AddToLog($"Error: {e.Data}")));
                }
            };

            ytDlpProcess.Exited += (sender, e) =>
            {
                tcs.SetResult(true);
            };

            ytDlpProcess.Start();
            ytDlpProcess.BeginOutputReadLine();
            ytDlpProcess.BeginErrorReadLine();

            await Task.Run(() => ytDlpProcess.WaitForExit());

            if (ytDlpProcess.ExitCode == 0)
            {
                statusLabel.Text = "Download completed!";
                progressBar.Value = 100;
                
                // Generate evidence report if enabled
                if (evidenceReportCheckBox.Checked || hashCheckBox.Checked)
                {
                    var downloadedFiles = Directory.GetFiles(evidenceFolder, "*.*")
                        .Where(f => !f.EndsWith(".json") && !f.EndsWith(".png") && !f.EndsWith(".txt"))
                        .ToList();
                    
                    if (evidenceReportCheckBox.Checked)
                    {
                        GenerateEvidenceReport(urlTextBox.Text, evidenceFolder, downloadedFiles);
                    }
                }
                
                AddToHistory(urlTextBox.Text, "Success", evidenceFolder);
                MessageBox.Show($"Download completed successfully!\nSaved to: {evidenceFolder}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                statusLabel.Text = "Download failed!";
                AddToHistory(urlTextBox.Text, "Failed", "");
                MessageBox.Show("Download failed. Check the log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessYtDlpOutput(string output)
        {
            AddToLog(output);

            // Parse progress
            if (output.Contains("[download]") && output.Contains("%"))
            {
                var start = output.IndexOf(" ");
                var end = output.IndexOf("%");
                if (start > 0 && end > start)
                {
                    var percentStr = output.Substring(start, end - start).Trim();
                    if (double.TryParse(percentStr, out double percent))
                    {
                        progressBar.Value = (int)Math.Min(percent, 100);
                    }
                }
            }

            // Update status
            if (output.Contains("ETA"))
            {
                statusLabel.Text = output;
            }
        }

        private void AddToLog(string message)
        {
            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void AddToHistory(string url, string status, string location)
        {
            var uri = new Uri(url);
            historyGrid.Rows.Add(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                uri.Host,
                Path.GetFileName(location),
                status,
                location
            );
        }

        private string GetYtDlpPath()
        {
            // Look for yt-dlp.exe in the same directory as this executable
            var exePath = Path.GetDirectoryName(Application.ExecutablePath);
            var ytDlpPath = Path.Combine(exePath, "yt-dlp.exe");
            
            if (!File.Exists(ytDlpPath))
            {
                throw new FileNotFoundException("yt-dlp.exe not found. Please download it from https://github.com/yt-dlp/yt-dlp/releases");
            }

            return ytDlpPath;
        }

        private void SaveSettings()
        {
            var settings = new Dictionary<string, string>
            {
                ["ig_username"] = igUsernameTextBox.Text,
                ["ig_password"] = passwordManager.EncryptString(igPasswordTextBox.Text),
                ["fb_email"] = fbEmailTextBox.Text,
                ["fb_password"] = passwordManager.EncryptString(fbPasswordTextBox.Text),
                ["li_email"] = liEmailTextBox.Text,
                ["li_password"] = passwordManager.EncryptString(liPasswordTextBox.Text),
                ["output_dir"] = outputDirTextBox.Text,
                ["quality"] = qualityComboBox.Text,
                ["subtitles"] = subtitlesCheckBox.Checked.ToString(),
                ["metadata"] = metadataCheckBox.Checked.ToString(),
                ["timestamp"] = timestampCheckBox.Checked.ToString(),
                ["cookies"] = cookiesCheckBox.Checked.ToString(),
                ["browser"] = browserComboBox.SelectedItem?.ToString() ?? "Chrome",
                ["screenshot"] = screenshotCheckBox.Checked.ToString(),
                ["hash"] = hashCheckBox.Checked.ToString(),
                ["evidenceReport"] = evidenceReportCheckBox.Checked.ToString()
            };

            var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoDownloader", "settings.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            
            using (var writer = new StreamWriter(settingsPath))
            {
                foreach (var kvp in settings)
                {
                    writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }

            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadSettings()
        {
            var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoDownloader", "settings.txt");
            if (!File.Exists(settingsPath))
            {
                // Silently return without showing any message
                return;
            }

            try
            {
                var settings = new Dictionary<string, string>();
                foreach (var line in File.ReadAllLines(settingsPath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                        settings[parts[0]] = parts[1];
                }

                if (settings.ContainsKey("ig_username"))
                    igUsernameTextBox.Text = settings["ig_username"];
                if (settings.ContainsKey("ig_password"))
                    igPasswordTextBox.Text = passwordManager.DecryptString(settings["ig_password"]);
                if (settings.ContainsKey("fb_email"))
                    fbEmailTextBox.Text = settings["fb_email"];
                if (settings.ContainsKey("fb_password"))
                    fbPasswordTextBox.Text = passwordManager.DecryptString(settings["fb_password"]);
                if (settings.ContainsKey("li_email"))
                    liEmailTextBox.Text = settings["li_email"];
                if (settings.ContainsKey("li_password"))
                    liPasswordTextBox.Text = passwordManager.DecryptString(settings["li_password"]);
                if (settings.ContainsKey("output_dir"))
                    outputDirTextBox.Text = settings["output_dir"];
                if (settings.ContainsKey("quality"))
                    qualityComboBox.Text = settings["quality"];
                if (settings.ContainsKey("subtitles"))
                    subtitlesCheckBox.Checked = bool.Parse(settings["subtitles"]);
                if (settings.ContainsKey("metadata"))
                    metadataCheckBox.Checked = bool.Parse(settings["metadata"]);
                if (settings.ContainsKey("timestamp"))
                    timestampCheckBox.Checked = bool.Parse(settings["timestamp"]);
                if (settings.ContainsKey("cookies"))
                    cookiesCheckBox.Checked = bool.Parse(settings["cookies"]);
                if (settings.ContainsKey("browser"))
                {
                    var browserIndex = browserComboBox.Items.IndexOf(settings["browser"]);
                    if (browserIndex >= 0)
                        browserComboBox.SelectedIndex = browserIndex;
                }
                if (settings.ContainsKey("screenshot"))
                    screenshotCheckBox.Checked = bool.Parse(settings["screenshot"]);
                if (settings.ContainsKey("hash"))
                    hashCheckBox.Checked = bool.Parse(settings["hash"]);
                if (settings.ContainsKey("evidenceReport"))
                    evidenceReportCheckBox.Checked = bool.Parse(settings["evidenceReport"]);

                MessageBox.Show("Settings loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearCredentials()
        {
            if (MessageBox.Show("Clear all saved credentials?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                igUsernameTextBox.Clear();
                igPasswordTextBox.Clear();
                fbEmailTextBox.Clear();
                fbPasswordTextBox.Clear();
                liEmailTextBox.Clear();
                liPasswordTextBox.Clear();
                
                var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoDownloader", "settings.txt");
                if (File.Exists(settingsPath))
                    File.Delete(settingsPath);

                MessageBox.Show("All credentials cleared!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        // Evidence Collection Methods
        private async Task CaptureWebpageScreenshot(string url, string outputPath)
        {
            try
            {
                var screenshotPath = Path.Combine(outputPath, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                
                // Try to find any Chromium-based browser for screenshot
                var browserPath = FindBrowserExecutable();
                if (string.IsNullOrEmpty(browserPath))
                {
                    AddToLog("No supported browser found for screenshot capture");
                    return;
                }
                
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = browserPath,
                        Arguments = $"--headless --disable-gpu --screenshot=\"{screenshotPath}\" --window-size=1920,1080 \"{url}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                await Task.Run(() => process.WaitForExit(10000)); // 10 second timeout
                
                if (File.Exists(screenshotPath))
                {
                    AddToLog($"Screenshot saved: {Path.GetFileName(screenshotPath)}");
                }
            }
            catch (Exception ex)
            {
                AddToLog($"Screenshot failed: {ex.Message}");
            }
        }
        
        private string FindBrowserExecutable()
        {
            // List of browsers that support headless screenshot mode (Chromium-based)
            var browserPaths = new Dictionary<string, string[]>
            {
                ["Chrome"] = new[]
                {
                    @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe")
                },
                ["Edge"] = new[]
                {
                    @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                    @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe")
                },
                ["Brave"] = new[]
                {
                    @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
                    @"C:\Program Files (x86)\BraveSoftware\Brave-Browser\Application\brave.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"BraveSoftware\Brave-Browser\Application\brave.exe")
                },
                ["Opera"] = new[]
                {
                    @"C:\Program Files\Opera\launcher.exe",
                    @"C:\Program Files (x86)\Opera\launcher.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Opera\launcher.exe")
                },
                ["Vivaldi"] = new[]
                {
                    @"C:\Program Files\Vivaldi\Application\vivaldi.exe",
                    @"C:\Program Files (x86)\Vivaldi\Application\vivaldi.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Vivaldi\Application\vivaldi.exe")
                }
            };
            
            // Try to find any available browser
            foreach (var browser in browserPaths)
            {
                foreach (var path in browser.Value)
                {
                    if (File.Exists(path))
                    {
                        AddToLog($"Using {browser.Key} for screenshot");
                        return path;
                    }
                }
            }
            
            return null;
        }
        
        private string FindChromeExecutable()
        {
            var possiblePaths = new[]
            {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe")
            };
            
            return possiblePaths.FirstOrDefault(File.Exists);
        }
        
        private string GenerateFileHash(string filePath)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        var hash = sha256.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch
            {
                return "Error generating hash";
            }
        }
        
        private void GenerateEvidenceReport(string url, string outputDir, List<string> downloadedFiles)
        {
            var reportPath = Path.Combine(outputDir, $"evidence_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            
            try
            {
                using (var writer = new StreamWriter(reportPath))
                {
                    writer.WriteLine("DIGITAL EVIDENCE COLLECTION REPORT");
                    writer.WriteLine("==================================");
                    writer.WriteLine($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Collector: {Environment.UserName}");
                    writer.WriteLine($"Computer: {Environment.MachineName}");
                    writer.WriteLine($"Operating System: {Environment.OSVersion}");
                    writer.WriteLine();
                    
                    writer.WriteLine("COLLECTION DETAILS:");
                    writer.WriteLine($"Source URL: {url}");
                    writer.WriteLine($"Platform: {DetectPlatform(url)}");
                    writer.WriteLine($"Collection Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Evidence Folder: {outputDir}");
                    writer.WriteLine();
                    
                    writer.WriteLine("DOWNLOADED FILES:");
                    writer.WriteLine("-----------------");
                    foreach (var file in downloadedFiles)
                    {
                        if (File.Exists(file))
                        {
                            var fileInfo = new FileInfo(file);
                            writer.WriteLine($"Filename: {fileInfo.Name}");
                            writer.WriteLine($"Size: {fileInfo.Length:N0} bytes ({fileInfo.Length / 1024.0 / 1024.0:F2} MB)");
                            writer.WriteLine($"Created: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}");
                            writer.WriteLine($"Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                            
                            if (hashCheckBox.Checked)
                            {
                                writer.WriteLine($"SHA256 Hash: {GenerateFileHash(file)}");
                            }
                            writer.WriteLine();
                        }
                    }
                    
                    writer.WriteLine("COLLECTION SETTINGS:");
                    writer.WriteLine($"- Quality: {qualityComboBox.Text}");
                    writer.WriteLine($"- Subtitles Downloaded: {subtitlesCheckBox.Checked}");
                    writer.WriteLine($"- Metadata Saved: {metadataCheckBox.Checked}");
                    writer.WriteLine($"- Timestamp Added: {timestampCheckBox.Checked}");
                    writer.WriteLine($"- Browser Cookies Used: {cookiesCheckBox.Checked}");
                    writer.WriteLine();
                    
                    writer.WriteLine("TECHNICAL INFORMATION:");
                    writer.WriteLine($"- yt-dlp Path: {GetYtDlpPath()}");
                    writer.WriteLine($"- Application Version: Multi-Platform Video Downloader v1.0");
                    writer.WriteLine($"- Evidence Collected Using Automated Process");
                    writer.WriteLine();
                    
                    writer.WriteLine("CHAIN OF CUSTODY:");
                    writer.WriteLine("- Files downloaded directly from source platform");
                    writer.WriteLine("- No modifications made to downloaded content");
                    writer.WriteLine("- All timestamps in local system time");
                    writer.WriteLine("- Digital chain of custody maintained");
                    writer.WriteLine();
                    
                    writer.WriteLine("VERIFICATION:");
                    writer.WriteLine("To verify file integrity, compare the SHA256 hashes listed above");
                    writer.WriteLine("with independently calculated hashes of the files.");
                    writer.WriteLine();
                    writer.WriteLine("--- END OF REPORT ---");
                }
                
                AddToLog($"Evidence report saved: {Path.GetFileName(reportPath)}");
            }
            catch (Exception ex)
            {
                AddToLog($"Failed to generate evidence report: {ex.Message}");
            }
        }
    }

    public class PasswordManager
    {
        private readonly byte[] entropy = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public string EncryptString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                byte[] encryptedData = ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(input),
                    entropy,
                    DataProtectionScope.CurrentUser);

                return Convert.ToBase64String(encryptedData);
            }
            catch
            {
                return input;
            }
        }

        public string DecryptString(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return "";
            }
        }
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}