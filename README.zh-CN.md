<p align="center">
  <a href="README.md">English</a> | <a href="README.zh-CN.md">中文</a>
</p>

<h1 align="center">Machine Vision App</h1>

<p align="center">
  <img src="https://img.shields.io/github/v/release/xianshi3/machine-vision-app?style=flat-square&label=release" alt="release"/>
  <img src="https://img.shields.io/github/stars/xianshi3/machine-vision-app?style=flat-square" alt="stars"/>
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/WPF-Dark%20UI-58A6FF?style=flat-square" alt="WPF"/>
  <img src="https://img.shields.io/badge/OpenCV-4.11-5C3EE8?style=flat-square&logo=opencv" alt="OpenCV 4.11"/>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2F11-0078D4?style=flat-square" alt="Windows"/>
  <img src="https://img.shields.io/badge/lang-EN%20%2F%20中文-3FB950?style=flat-square" alt="i18n"/>
</p>

<p align="center">
  一款现代机器视觉桌面应用，支持本地/网络摄像头的实时视频处理、
  图像识别、条码扫描与测量。
  <br/>
  基于 <strong>WPF</strong> · <strong>OpenCvSharp</strong> · <strong>ZXing.Net</strong> · <strong>.NET 8</strong> 构建
</p>

<p align="center">
  <img src="docs/images/01_ui.png" alt="主界面" width="880"/>
</p>

---

## 功能特性

- **双信号源** – 本地 USB 摄像头或网络 IP 摄像头（RTSP / MJPEG over HTTP）
- **人脸检测** – Haar 级联分类器，支持自定义检测框样式
- **11 种处理模式** – Canny、Sobel、Laplacian、二值化、轮廓、QR/条码、颜色检测、模板匹配、形状识别、特征点匹配、图像增强
- **QR / 条码** – 实时解码二维码与一维条码（EAN/UPC/Code128/Code39），画面直接显示识别结果
- **颜色检测** – 基于 HSV 的 9 种预设颜色检测与目标计数，支持点击画面直接取色
- **模板匹配** – 加载模板图像，在实时画面中定位并显示匹配分数
- **形状识别** – 自动分类圆形、矩形、三角形、五边形、多边形，并输出分类统计
- **特征点匹配** – ORB 特征点 + RANSAC 单应矩阵，抗旋转、缩放变化
- **图像增强** – CLAHE 直方图均衡 + 非锐化掩模，改善低对比度画面
- **视频录像** – 一键将处理后的视频录制为 AVI 文件
- **截图** – 将当前帧保存为 PNG 图片
- **网络摄像头** – 通过 IP Webcam 类应用连接手机摄像头
- **双语支持** – 内置中文与英文，运行时一键切换
- **现代深色界面** – GitHub 深色主题、卡片布局、状态栏、可折叠日志面板、可调整窗口
- **图片分析** – 加载静态图片并应用完整处理流水线
- **实时统计** – FPS、处理耗时、人脸/轮廓计数

---

## 处理模式效果

以下效果图均由应用自身使用内置测试图 `TestImages/test_scene.png` 生成。

<table>
  <tr>
    <td align="center"><b>Canny 边缘检测</b><br/><img src="docs/images/02_canny.png" width="360"/></td>
    <td align="center"><b>轮廓检测</b><br/><img src="docs/images/03_contour.png" width="360"/></td>
  </tr>
  <tr>
    <td align="center"><b>二值化</b><br/><img src="docs/images/04_binary.png" width="360"/></td>
    <td align="center"><b>形状识别</b><br/><img src="docs/images/05_shape.png" width="360"/></td>
  </tr>
  <tr>
    <td align="center"><b>颜色检测（红色）</b><br/><img src="docs/images/06_color.png" width="360"/></td>
    <td align="center"><b>QR / 条码识别</b><br/><img src="docs/images/07_qr.png" width="360"/></td>
  </tr>
  <tr>
    <td align="center"><b>模板匹配</b><br/><img src="docs/images/08_template.png" width="360"/></td>
    <td align="center"><b>特征点匹配（ORB）</b><br/><img src="docs/images/09_feature.png" width="360"/></td>
  </tr>
  <tr>
    <td align="center"><b>图像增强（CLAHE + 锐化）</b><br/><img src="docs/images/10_enhance.png" width="360"/></td>
    <td align="center"><b>人脸检测</b><br/><img src="docs/images/11_face.png" width="360"/></td>
  </tr>
</table>

---

## 使用流程

1. **选择信号源** – 选择"本地摄像头"或"网络视频流"
2. **配置** – 网络模式输入 IP 地址和端口
3. **连接** – 建立视频流连接
4. **选择模式** – 从 11 种处理算法中选择一种
5. **处理** – 实时处理 + 人脸检测，实时显示 FPS 和处理耗时
6. **调整** – 微调 Canny 阈值并应用（Canny / 轮廓模式）
7. **扫描 / 检测 / 匹配** – 将摄像头对准二维码、彩色物体、形状或已加载的模板
8. **保存** – 随时截图或录制视频
9. **加载图片** – 从图片文件离线分析

所有处理均在后台线程异步执行，UI 始终保持流畅。

---

## 使用手机作为摄像头

1. 在手机上安装 **IP Webcam**（Android）或同类应用
2. 确保手机与电脑在同一 Wi-Fi 网络
3. 启动应用并记录 URL（如 `http://192.168.1.100:8080/video`）
4. 在本应用中：
   - 信号源选择 **"网络视频流"**
   - 输入 IP 和端口
   - 点击 **连接**

应用会自动拼接 MJPEG URL 并开始拉流。

---

## 项目结构

```
MachineVisionApp/
├── App.xaml / App.xaml.cs          # 应用入口与默认语言（en-US）
├── MainWindow.xaml / .cs           # 主界面与事件编排
├── TranslationService.cs           # i18n 单例服务（INotifyPropertyChanged）
├── AppLogger.cs                    # 日志服务（单例）
├── Resources/
│   ├── Strings.resx                # 中文资源（回退语言）
│   └── Strings.en.resx             # 英文资源
├── Components/
│   ├── VideoCaptureComponent.cs    # 视频采集（本地 + 网络）
│   ├── ImageDisplayComponent.cs    # WPF 图像显示与帧更新
│   ├── ImageProcessingComponent.cs # 5 种经典模式（Canny/Sobel/Laplacian/二值化/轮廓）
│   ├── FaceDetectionComponent.cs   # Haar 级联人脸检测
│   ├── BarcodeDetectionComponent.cs # QR/条码解码（ZXing.Net）
│   ├── ColorDetectionComponent.cs  # HSV 颜色检测 + 点击取色
│   ├── TemplateMatchComponent.cs   # 模板匹配（含分数）
│   ├── ShapeDetectionComponent.cs  # 几何形状分类 + 统计
│   ├── FeatureMatchComponent.cs    # ORB 特征点匹配 + RANSAC
│   ├── EnhancementComponent.cs     # CLAHE + 锐化增强
│   ├── RecordingComponent.cs       # AVI 视频录制
│   └── ThresholdParameterComponent # Canny 阈值参数逻辑
├── Views/
│   ├── CustomTitleBar.xaml / .cs   # 自定义标题栏 + 语言切换
├── TestImages/                     # 测试图片（场景、模板、人脸照片）
├── docs/images/                    # README 使用的截图
└── haarcascade_frontalface_default.xml
```

### 核心架构

| 组件 | 职责 |
|------|------|
| `VideoCaptureComponent` | `LocalCamera` / `NetworkStream` 双信号源，自动降级尝试 API（DSHOW → MSMF → ANY），连接状态机 |
| `ImageDisplayComponent` | 批量 `Dispatcher.Invoke` 双图更新 |
| `ImageProcessingComponent` | 5 种经典模式：Canny、Sobel、Laplacian、二值化、轮廓检测 |
| `FaceDetectionComponent` | `DetectMultiScale` + 直方图均衡预处理 + 检测框绘制 |
| `BarcodeDetectionComponent` | ZXing.Net 解码 QR/DataMatrix/EAN/UPC/Code128/Code39，帧节流 + 结果缓存 |
| `ColorDetectionComponent` | HSV `InRange` 掩码 + 形态学 + 轮廓计数，9 种预设色 + 点击取色 |
| `TemplateMatchComponent` | `MatchTemplate`（CCoeffNormed）+ 阈值过滤 + 分数叠加 |
| `ShapeDetectionComponent` | Canny + 多边形逼近 + 圆形度分析，分类圆形/矩形/三角形/五边形/多边形 |
| `FeatureMatchComponent` | ORB 特征点 + BFMatcher 比率测试 + RANSAC 单应矩阵，绘制透视定位框 |
| `EnhancementComponent` | CLAHE 直方图均衡 + 非锐化掩模 |
| `RecordingComponent` | `VideoWriter` AVI 录制（MJPG 编码） |
| `ThresholdParameterComponent` | 输入校验并触发 `OnThresholdsChanged` |
| `TranslationService` | `INotifyPropertyChanged` 单例，基于 `ResourceManager`，切换文化时全量刷新 |
| `AppLogger` | `ObservableCollection<LogEntry>` 单例，INFO/WARN/ERROR 三级 |

---

## 国际化

- 默认语言为**英文**（点击标题栏 **EN/中** 切换到中文）
- 所有 UI 文案由 `.resx` 资源文件管理
- 新增语言：复制 `Strings.en.resx`，重命名为 `Strings.xx.resx` 并翻译即可

---

## 环境要求

- .NET 8 SDK（仅编译需要；下方发布包已自包含运行时）
- Windows 10/11（WPF）
- NuGet 包（自动还原）：
  - `OpenCvSharp4` – OpenCV 绑定
  - `OpenCvSharp4.runtime.win` – OpenCV 原生库
  - `OpenCvSharp4.WpfExtensions` – `BitmapSource` 转换
  - `ZXing.Net` – 二维码/条码解码

---

## 快速开始

### 直接下载（无需编译）

从 [Releases](../../releases/latest) 下载最新的自包含包：

1. 下载 `MachineVisionApp-win-x64-vX.Y.Z.zip`
2. 解压到任意目录
3. 运行 `MachineVisionApp.exe`

### 从源码构建

```bash
# 克隆仓库
git clone https://github.com/xianshi3/machine-vision-app.git
cd machine-vision-app

# 还原并编译
dotnet restore
dotnet build -c Release

# 运行
dotnet run --project MachineVisionApp/MachineVisionApp.csproj
```

或用 Visual Studio 2022 打开 `MachineVisionApp.sln`，按 **F5** 运行。

### 使用内置测试图片

在"加载图片"对话框中选择 `TestImages/` 目录下的图片：

| 图片 | 测试内容 |
|------|----------|
| `test_scene.png` | 全部处理模式——边缘、轮廓、形状、颜色、QR/条码（内容为 `https://github.com/xianshi3/machine-vision-app`） |
| `template_green.png` | 模板匹配（绿色方块，匹配度 ≈ 1.0） |
| `template_qr.png` | 特征点匹配（纹理丰富的 QR 裁剪图） |
| `face_lena.jpg` | 人脸检测 |

---

## 技术栈

| 技术            | 说明                           |
|-----------------|--------------------------------|
| `WPF`           | Windows 桌面 UI 框架           |
| `OpenCvSharp`   | OpenCV 4.x 的 .NET 封装        |
| `ZXing.Net`     | 二维码与条码解码               |
| `C#`            | 主要编程语言                   |
| `XAML`          | 界面设计与布局                 |
| `.resx`         | 国际化资源文件                 |

---

## 许可证

本项目仅用于学习与演示。
