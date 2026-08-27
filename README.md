<h1 align="center">🧠 Machine Vision App</h1>

<p align="center">
  A modern host computer (上位机) desktop application for real-time video processing,
  edge detection, face recognition, barcode scanning, color detection, and template matching
  over local or network cameras.
  <br/>
  Built with <strong>WPF</strong> · <strong>OpenCvSharp</strong> · <strong>ZXing.Net</strong> · <strong>.NET 8</strong>
</p>

---

## 🚀 Features

- 📡 **Dual Source** – Local USB camera or network IP camera (RTSP / MJPEG over HTTP)
- 🧍 **Face Detection** – Haar cascade classifier with configurable overlay style
- 🪞 **8 Processing Modes** – Canny, Sobel, Laplacian, Binary Threshold, Contour Detection, QR/Barcode, Color Detection, Template Matching
- 📷 **QR / Barcode** – Real-time decoding of QR codes and 1D barcodes (EAN/UPC/Code128/Code39) with on-screen result display
- 🎨 **Color Detection** – HSV-based target color detection with 9 preset colors and object counting
- 🎯 **Template Matching** – Load a template image and locate it in the live feed with a match score
- 🎥 **Video Recording** – Record processed video to AVI files with one click
- 📸 **Screenshot** – Save current frame as PNG screenshot
- 🌐 **Network Camera** – Connect to phone cameras via IP Webcam apps
- 🌍 **i18n Support** – Built-in Chinese & English, switchable at runtime
- 🎨 **Modern Dark UI** – Refined GitHub-dark theme with card layout, status bar, collapsible log panel, resizable window
- 🖼 **Image Analysis** – Load static images and apply the full processing pipeline
- 📊 **Real-time Stats** – FPS counter, processing time, face/contour counts

---

## 🖥️ User Interface

```
┌─────────────────────────────────────────────────────────────┐
│  [MV] Machine Vision App              [中/EN] [─] [□] [×]  │
├──────────────────────────┬──────────────────────────────────┤
│  ● Original Stream       │  ◆ Processing Result             │
│  ┌──────────────────────┐│  ┌──────────────────────┐        │
│  │                      ││  │                      │        │
│  │     Camera Feed      ││  │   Processed Output   │        │
│  │                      ││  │                      │        │
│  └──────────────────────┘│  └──────────────────────┘        │
│  ● Connected             │  QR/Barcode          内容文本    │
├─────────────────────────────────────────────────────────────┤
│ [Source: ▼ Local] [● Connected] [Connect] [Disconnect]      │
│ ─────────────────────────────────────────────────────────── │
│ [Mode: ▼ QR/Barcode] [Threshold: 100] ~ [200] [Apply]       │
│ [Mode: ▼ Color] [Color: ▼ Red] [Mode: ▼ Template] [📁Load]  │
│ [📸 Save Snapshot] [🎥 Record] │ [▶ Start] [■ Stop] [📁 Load]│
├─────────────────────────────────────────────────────────────┤
│  30.2 FPS · 12 ms              Machine Vision App · v2.1.0  │
├─────────────────────────────────────────────────────────────┤
│  📋 Log                                       [Clear]      │
│  [14:23:01] [INFO] 设备已连接                              │
│  [14:23:01] [INFO] 识别到内容: https://github.com          │
└─────────────────────────────────────────────────────────────┘
```

---

## ⚙️ How It Works

1. **Select Source** – Choose "Local Camera" or "Network Stream"  
2. **Configure** – For network, enter IP address and port  
3. **Connect** – Establish video stream connection  
4. **Choose Mode** – Select processing algorithm: Canny / Sobel / Laplacian / Binary / Contour / QR-Barcode / Color Detection / Template Matching  
5. **Process** – Real-time image processing + face detection, with FPS and timing stats  
6. **Adjust** – Fine-tune Canny thresholds and re-apply (for Canny / Contour modes)  
7. **Scan / Detect / Match** – Point the camera at a QR code, colored object, or a loaded template  
8. **Save** – Take screenshots or record video at any time  
9. **Load Image** – Offline analysis from image files

All processing runs asynchronously on a background thread, keeping the UI responsive.

---

## 📱 Using Your Phone as Camera

1. Install **IP Webcam** (Android) or similar app on your phone  
2. Make sure phone and PC are on the same Wi-Fi network  
3. Launch the app and note the URL (e.g., `http://192.168.1.100:8080/video`)  
4. In this application:
   - Select **"Network Stream"** as source
   - Enter the IP and port
   - Click **Connect**

The app automatically constructs the MJPEG URL and starts streaming.

---

## 📁 Project Structure

```
MachineVisionApp/
├── App.xaml / App.xaml.cs          # Application entry & global styles
├── MainWindow.xaml / .cs           # Main UI & event orchestration
├── TranslationService.cs           # i18n singleton with INotifyPropertyChanged
├── AppLogger.cs                    # Logging service (singleton)
├── Resources/
│   ├── Strings.resx                # Chinese resource strings (default)
│   └── Strings.en.resx             # English resource strings
├── Components/
│   ├── VideoCaptureComponent.cs    # Camera source (local + network)
│   ├── ImageDisplayComponent.cs    # WPF Image display & frame update
│   ├── ImageProcessingComponent.cs # 5 classic modes (Canny/Sobel/Laplacian/Binary/Contour)
│   ├── FaceDetectionComponent.cs   # Haar cascade face detection
│   ├── BarcodeDetectionComponent.cs # QR/barcode decoding (ZXing.Net)
│   ├── ColorDetectionComponent.cs  # HSV color detection + object counting
│   ├── TemplateMatchComponent.cs   # Template matching with score
│   ├── RecordingComponent.cs       # AVI video recording via VideoWriter
│   └── ThresholdParameterComponent # Canny threshold UI logic
├── Views/
│   ├── CustomTitleBar.xaml / .cs   # Custom window chrome + lang switch
└── haarcascade_frontalface_default.xml
```

### Key Architecture

| Component | Responsibility |
|-----------|----------------|
| `VideoCaptureComponent` | `VideoSourceType.LocalCamera` / `.NetworkStream`, auto-fallback APIs (DSHOW → MSMF → ANY), connection state machine |
| `ImageDisplayComponent` | Batched `Dispatcher.Invoke` for dual-image update |
| `ImageProcessingComponent` | 5 classic modes: Canny, Sobel, Laplacian, Binary Threshold, Contour Detection |
| `FaceDetectionComponent` | `DetectMultiScale` + histogram equalization preprocessing + bounding box rendering |
| `BarcodeDetectionComponent` | ZXing.Net decoding of QR/DataMatrix/EAN/UPC/Code128/Code39, frame-throttled with result caching |
| `ColorDetectionComponent` | HSV `InRange` masks + morphology + contour counting for 9 preset target colors |
| `TemplateMatchComponent` | `MatchTemplate` (CCoeffNormed) with threshold gating and score overlay |
| `RecordingComponent` | `VideoWriter`-based AVI recording with MJPG codec |
| `ThresholdParameterComponent` | Validates input and fires `OnThresholdsChanged` |
| `TranslationService` | `INotifyPropertyChanged` singleton, `ResourceManager`-backed, fires full refresh on culture switch |
| `AppLogger` | `ObservableCollection<LogEntry>` singleton with INFO/WARN/ERROR levels |

---

## 🌍 Internationalization

- Default language is **Chinese**
- Click **中/EN** in the title bar to toggle between Chinese and English
- All UI strings are managed via `.resx` resource files
- To add a new language: copy `Strings.en.resx`, rename to `Strings.xx.resx`, translate values

---

## 🛠 Requirements

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- ✅ Windows 10/11 with WPF support
- ✅ NuGet Packages (restored automatically):
  - `OpenCvSharp4` – OpenCV bindings
  - `OpenCvSharp4.runtime.win` – Native OpenCV binaries
  - `OpenCvSharp4.WpfExtensions` – `BitmapSource` conversion
  - `ZXing.Net` – QR code & barcode decoding

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/xianshi3/machine-vision-app.git
cd machine-vision-app

# Restore and build
dotnet restore
dotnet build -c Release

# Run
dotnet run --project MachineVisionApp/MachineVisionApp.csproj
```

Or open `MachineVisionApp.sln` in Visual Studio 2022 and press **F5**.

---

## 📦 Tech Stack

| Technology      | Description                     |
|-----------------|---------------------------------|
| `WPF`           | UI framework for Windows apps   |
| `OpenCvSharp`   | .NET wrapper for OpenCV 4.x     |
| `ZXing.Net`     | QR code & barcode decoding      |
| `C#`            | Primary programming language    |
| `XAML`          | UI design and layout            |
| `.resx`         | Resource files for i18n         |

---

## 📄 License

This project is for learning and demonstration purposes.
