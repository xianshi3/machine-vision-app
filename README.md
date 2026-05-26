<h1 align="center">🧠 Host Computer Vision System</h1>

<p align="center">
  A modern host computer (上位机) desktop application for real-time video processing,
  edge detection, and face recognition over local or network cameras.
  <br/>
  Built with <strong>WPF</strong> · <strong>OpenCvSharp</strong> · <strong>.NET 8</strong>
</p>

---

## 🚀 Features

- 📡 **Dual Source** – Local USB camera or network IP camera (RTSP / MJPEG over HTTP)
- 🧍 **Face Detection** – Haar cascade classifier with configurable overlay style
- 🪞 **Edge Detection** – Real-time Canny filter with adjustable thresholds
- 🌐 **Network Camera** – Connect to phone cameras via IP Webcam apps
- 🌍 **i18n Support** – Built-in Chinese & English, switchable at runtime
- 🎨 **Modern Dark UI** – Refined GitHub-dark theme with card layout
- 🖼 **Image Analysis** – Load static images and apply the full processing pipeline

---

## 🖥️ User Interface

```
┌─────────────────────────────────────────────────────────┐
│  [MV] Machine Vision App          [中/EN] [─] [□] [×]  │
├──────────────────────┬──────────────────────────────────┤
│  🔴 Original Stream  │  🔵 Edge Detection       Faces:3 │
│  ┌──────────────────┐│  ┌──────────────────┐            │
│  │                  ││  │                  │            │
│  │   Camera Feed    ││  │   Canny Output   │            │
│  │                  ││  │                  │            │
│  └──────────────────┘│  └──────────────────┘            │
│  ● Connected         │  Threshold: 100 ~ 200            │
├─────────────────────────────────────────────────────────┤
│ [Source: ▼ Network] [IP: 192.168.1.100:8080] [Connect]  │
│ ● Connected                     │ [Disconnect]          │
├─────────────────────────────────────────────────────────┤
│ [Canny Threshold] [100] ~ [200] [Apply]                 │
│ [Start Camera] [Stop Camera] [Load Image]               │
└─────────────────────────────────────────────────────────┘
```

---

## ⚙️ How It Works

1. **Select Source** – Choose "Local Camera" or "Network Stream"  
2. **Configure** – For network, enter IP address and port  
3. **Connect** – Establish video stream connection  
4. **Process** – Real-time Canny edge detection + face detection  
5. **Adjust** – Fine-tune Canny thresholds and re-apply  
6. **Load Image** – Offline analysis from image files

Processing runs asynchronously on a background thread, keeping the UI responsive.

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
├── Resources/
│   ├── Strings.resx                # Chinese resource strings (default)
│   └── Strings.en.resx             # English resource strings
├── Components/
│   ├── VideoCaptureComponent.cs    # Camera source (local + network)
│   ├── ImageDisplayComponent.cs    # WPF Image display & frame update
│   ├── EdgeDetectionComponent.cs   # Canny edge detection
│   ├── FaceDetectionComponent.cs   # Haar cascade face detection
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
| `EdgeDetectionComponent` | Thin wrapper over `Cv2.Canny` |
| `FaceDetectionComponent` | `DetectMultiScale` + bounding box rendering |
| `ThresholdParameterComponent` | Validates input and fires `OnThresholdsChanged` |
| `TranslationService` | `INotifyPropertyChanged` singleton, `ResourceManager`-backed, fires full refresh on culture switch |

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

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone <repo-url>
cd MachineVisionApp

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
| `C#`            | Primary programming language    |
| `XAML`          | UI design and layout            |
| `.resx`         | Resource files for i18n         |

---

## 📄 License

This project is for learning and demonstration purposes.
