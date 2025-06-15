# Multi-Platform Video Evidence Collector v1.0
## Professional Digital Evidence Collection Tool

---

## 🎯 EXECUTIVE SUMMARY

The Multi-Platform Video Evidence Collector is a specialized forensic tool designed for law enforcement, private investigators, and legal professionals to legally collect, preserve, and document digital video evidence from social media and online platforms. This tool ensures chain of custody, maintains evidence integrity, and generates court-admissible documentation.

---

## 📋 KEY FEATURES

### 1. **Multi-Platform Support**
- ✅ **Social Media**: Facebook, Instagram, Twitter/X, TikTok, LinkedIn
- ✅ **Video Platforms**: YouTube, Pinterest, CapCut
- ✅ **Messaging**: Telegram
- ✅ **Audio Platforms**: Spotify, SoundCloud
- ✅ **Adult Content Sites**: For criminal investigations
- ✅ **17+ platforms** total supported

### 2. **Evidence Collection Features**
- 📸 **Automatic Screenshot Capture**: Preserves webpage appearance at time of collection
- 🔐 **SHA-256 Hash Generation**: Cryptographic proof of file integrity
- 📄 **Detailed Evidence Reports**: Court-ready documentation with full metadata
- 📁 **Organized Evidence Folders**: Each download creates timestamped evidence package
- ⏱️ **Timestamp Documentation**: Precise collection time for legal proceedings

### 3. **Download Capabilities**
- 🎥 **Multiple Quality Options**: From 144p to 4K
- 🎵 **Audio-Only Extraction**: MP3 format in various bitrates
- 📝 **Subtitle Download**: Captures available captions
- 🖼️ **Thumbnail Preservation**: Saves video preview images
- 📊 **Metadata Collection**: Complete technical information

### 4. **Batch Processing**
- 📋 **Download Queue System**: Process multiple URLs automatically
- ⏸️ **Smart Delays**: Prevents detection and rate limiting
- 🔄 **Auto-Retry Failed Downloads**: Ensures evidence collection
- 📈 **Progress Tracking**: Visual feedback for each download

### 5. **Authentication Support**
- 🔑 **Credential Storage**: Encrypted password saving
- 🍪 **Browser Cookie Integration**: Access private content
- 👤 **Multi-Account Support**: Different credentials for each platform
- 🔒 **Secure Storage**: Windows-encrypted credential protection

---

## 💡 BENEFITS FOR INVESTIGATORS

### Legal & Compliance
- ✅ **Court-Admissible Documentation**: Meets legal evidence standards
- ✅ **Chain of Custody**: Complete audit trail from source to storage
- ✅ **Tamper Detection**: Cryptographic hashes verify integrity
- ✅ **Time-Stamped Evidence**: Precise collection chronology

### Efficiency & Productivity
- ⚡ **One-Click Collection**: Simple interface for non-technical users
- 🚀 **Batch Processing**: Collect multiple pieces of evidence simultaneously
- 📦 **All-in-One Package**: No need for multiple tools
- 💾 **Organized Storage**: Automatic folder structure for case files

### Technical Advantages
- 🖥️ **No Installation Required**: Self-contained executable
- 🔧 **No Technical Knowledge Needed**: Designed for investigators, not IT professionals
- 📱 **Works Offline**: Once downloaded, evidence is stored locally
- 🛡️ **Privacy Focused**: All processing done on your computer

---

## 🚀 SETUP INSTRUCTIONS

### Step 1: System Requirements
- ✅ Windows 10 or Windows 11 (64-bit)
- ✅ 4GB RAM minimum (8GB recommended)
- ✅ 500MB free disk space for program
- ✅ Additional space for downloaded videos
- ✅ Internet connection

### Step 2: Download Required Files

1. **Download the Evidence Collector Package** (provided by IT department)
   - Contains: `VideoDownloader.exe` and `yt-dlp.exe`

2. **Download FFmpeg** (REQUIRED - Must do manually)
   - Go to: https://www.gyan.dev/ffmpeg/builds/
   - Click "release builds" → "essentials_build.zip"
   - Extract the ZIP file
   - Find `ffmpeg.exe` in the `bin` folder
   - Copy `ffmpeg.exe` to the same folder as `VideoDownloader.exe`

### Step 3: First-Time Setup

1. **Create a Work Folder**
   ```
   C:\Evidence_Collection\
   ├── VideoDownloader.exe
   ├── yt-dlp.exe
   └── ffmpeg.exe (you must add this)
   ```

2. **Run the Program**
   - Double-click `VideoDownloader.exe`
   - Windows may show a security warning - click "Run anyway"

3. **Configure Settings** (Settings Tab)
   - Set your preferred download folder
   - Enable evidence collection options (all checked by default)
   - Save credentials if needed (optional)

---

## 📖 HOW TO USE - STEP BY STEP

### Basic Evidence Collection

1. **Open the program** - Double-click VideoDownloader.exe

2. **Copy the video URL**
   - Go to the social media post/video
   - Copy the URL from your browser's address bar

3. **Paste and Download**
   - Click "Paste" button in the program
   - Program will detect the platform automatically
   - Choose quality (recommend "Best Quality" for evidence)
   - Click "Download Video"

4. **Evidence Package Created**
   - Program creates folder: `Evidence_20240115_143052`
   - Contains: Video, Screenshot, Metadata, Report

### For Private/Protected Content

1. **Using Browser Cookies** (TikTok, Pinterest)
   - First, log into the platform in Chrome browser
   - In program Settings tab, check "Use browser cookies"
   - Now download private content

2. **Using Credentials** (Instagram, Facebook)
   - Go to Settings tab
   - Enter username/password for the platform
   - Click "Save Credentials"
   - Credentials are encrypted on your computer

### Batch Evidence Collection

1. **Queue Multiple URLs**
   - Paste first URL
   - Click "Add to Queue" instead of Download
   - Repeat for all URLs
   - Program processes automatically with delays

---

## 🔍 UNDERSTANDING THE EVIDENCE REPORT

Each download generates a report containing:

```
DIGITAL EVIDENCE COLLECTION REPORT
==================================
Report Generated: 2024-01-15 14:30:52
Collector: Detective Smith
Computer: DEPT-LAPTOP-045

COLLECTION DETAILS:
Source URL: [Original URL]
Platform: Instagram
Collection Time: 2024-01-15 14:30:52

DOWNLOADED FILES:
Filename: suspect_video_abc123.mp4
Size: 15,234,567 bytes (14.53 MB)
SHA256 Hash: a7b9c3d4e5f6789...
```

**Key Elements:**
- **SHA256 Hash**: Use this to verify file hasn't changed
- **Timestamps**: All in local time - note your timezone
- **Collector Info**: Automatically captured from Windows

---

## ⚠️ IMPORTANT LEGAL NOTICES

### DO's:
- ✅ **Only collect publicly available content** unless you have a warrant
- ✅ **Document your legal authority** for private content collection
- ✅ **Maintain chain of custody** - don't modify downloaded files
- ✅ **Create backups** of evidence immediately
- ✅ **Use official accounts** when collecting private content

### DON'Ts:
- ❌ **Don't use personal accounts** for official investigations
- ❌ **Don't share credentials** between investigators
- ❌ **Don't modify evidence files** after collection
- ❌ **Don't collect content without proper authority**
- ❌ **Don't ignore platform terms of service** without legal justification

---

## 🛠️ TROUBLESHOOTING

### Common Issues and Solutions

**"ffmpeg.exe not found"**
- Solution: Download ffmpeg and place in same folder as program

**"Download failed"**
- Try enabling "Use browser cookies" in Settings
- Check if content is private/deleted
- Verify your internet connection

**"Cannot capture screenshot"**
- Chrome browser must be installed
- Screenshot is optional - evidence still valid without it

**Private video won't download**
- Log into the platform in Chrome first
- Enable browser cookies option
- Or use username/password in Settings

**Program won't start**
- Right-click → Properties → Unblock
- Disable antivirus temporarily
- Run as Administrator

---

## 📊 EVIDENCE MANAGEMENT BEST PRACTICES

### 1. **Folder Organization**
```
C:\Cases\
├── Case_2024_001_Smith\
│   ├── Evidence_20240115_143052\
│   ├── Evidence_20240115_145623\
│   └── Evidence_20240115_151234\
└── Case_2024_002_Jones\
    └── Evidence_20240116_093012\
```

### 2. **Immediate Actions After Collection**
1. **Verify** - Check the evidence report
2. **Backup** - Copy to secure storage
3. **Document** - Add to case management system
4. **Hash Check** - Record SHA256 for court

### 3. **Court Preparation**
- Print evidence reports for case file
- Prepare hash verification demonstration
- Document your collection methodology
- Maintain original folder structure

---

## 💾 DATA SECURITY

### Your Evidence is Protected:
- 🔐 **Local Storage Only** - Nothing uploaded to cloud
- 🔑 **Encrypted Credentials** - Windows-level encryption
- 🛡️ **No Tracking** - Tool doesn't phone home
- 📁 **You Control the Data** - All files on your computer

### Security Recommendations:
1. Use a dedicated evidence collection laptop
2. Enable full disk encryption (BitLocker)
3. Regular backups to secure storage
4. Limit access to evidence folders

---

## 📞 QUICK REFERENCE CARD

### Essential Shortcuts:
- **Paste URL**: Paste button or Ctrl+V
- **Download**: Green Download button
- **Queue**: Blue Add to Queue button
- **Settings**: Third tab at top
- **History**: Second tab at top

### File Locations:
- **Program Settings**: `%APPDATA%\VideoDownloader\`
- **Default Downloads**: `%USERPROFILE%\Downloads\Videos\`
- **Evidence Folders**: Named `Evidence_YYYYMMDD_HHMMSS`

### Critical Files in Evidence Folder:
- 🎥 **Video file** - The actual evidence
- 📸 **screenshot_*.png** - Webpage capture
- 📄 **evidence_report_*.txt** - Legal documentation
- 📊 **.info.json** - Technical metadata

---

## 🆘 GETTING HELP

### For Technical Issues:
1. Check the Log Output in the program
2. Review this guide's troubleshooting section
3. Contact IT support with:
   - Screenshot of error
   - The log output text
   - What you were trying to download

### For Legal Questions:
- Consult your department's legal counsel
- Review your agency's digital evidence policies
- Ensure compliance with local laws

### For Updates:
- Program auto-downloads latest yt-dlp
- Check with IT for program updates
- FFmpeg rarely needs updating

---

## ✅ PRE-COLLECTION CHECKLIST

Before collecting evidence, verify:

- [ ] Legal authority to collect the content
- [ ] FFmpeg.exe is in program folder
- [ ] Sufficient disk space available
- [ ] Correct date/time on computer
- [ ] Evidence folder path documented
- [ ] Browser logged in (if needed)
- [ ] VPN disabled (unless required)
- [ ] Screen recording off (privacy)

---

## 📝 FINAL NOTES

This tool is designed to make digital evidence collection simple, legally compliant, and forensically sound. Remember:

1. **The tool is just the beginning** - proper documentation and chain of custody are essential
2. **When in doubt, collect more** - you can't go back to deleted content
3. **Quality matters** - always use highest quality for court
4. **Time is critical** - content can be deleted at any moment
5. **Stay within the law** - this tool doesn't override legal requirements

**Version**: 1.0  
**Last Updated**: January 2024  
**Developed for**: Law Enforcement and Legal Professionals

---

*"Protecting the integrity of digital evidence in an evolving online world"*
