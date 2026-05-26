using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using OpenCvSharp;

namespace MachineVisionApp
{
    /// <summary>
    /// 主窗口，负责整个应用的 UI 交互和业务逻辑编排。
    /// 协调视频采集、图像处理（多模式）、人脸检测、录制、日志等各组件的协作。
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // ---- 各功能组件 ----
        private Components.VideoCaptureComponent _videoCaptureComponent;
        private Components.ImageDisplayComponent _imageDisplayComponent;
        private Components.ThresholdParameterComponent _thresholdParameterComponent;
        private Components.FaceDetectionComponent _faceDetectionComponent;
        private Components.ImageProcessingComponent _imageProcessingComponent;
        private Components.RecordingComponent _recordingComponent;

        // ---- 处理参数 ----
        private Components.ProcessingMode _currentMode = Components.ProcessingMode.Canny;
        private int _threshold1 = 100;
        private int _threshold2 = 200;
        private bool _networkConfigured;

        // ---- 性能追踪 ----
        private readonly Stopwatch _frameStopwatch = new();
        private int _frameCount;
        private double _currentFps;
        private DateTime _lastFpsUpdate = DateTime.Now;

        /// <summary>
        /// 构造函数：初始化组件、注册事件。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _faceDetectionComponent = new Components.FaceDetectionComponent("haarcascade_frontalface_default.xml");
            _imageProcessingComponent = new Components.ImageProcessingComponent();
            _videoCaptureComponent = new Components.VideoCaptureComponent();
            _imageDisplayComponent = new Components.ImageDisplayComponent(OriginalImage, EdgeImage);
            _thresholdParameterComponent = new Components.ThresholdParameterComponent(
                Threshold1TextBox, Threshold2TextBox, ApplyThresholdsButton);
            _recordingComponent = new Components.RecordingComponent();

            _videoCaptureComponent.OnFrameCaptured += ProcessFrame;
            _videoCaptureComponent.OnCaptureStopped += OnCaptureStoppedHandler;
            _videoCaptureComponent.OnCaptureError += OnCaptureErrorHandler;
            _videoCaptureComponent.OnConnectionStateChanged += OnConnectionStateChangedHandler;

            _thresholdParameterComponent.OnThresholdsChanged += UpdateThresholds;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        /// <summary>
        /// 窗口加载完成：初始化下拉框和网络面板状态。
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SourceTypeComboBox.ItemsSource = new string[]
            {
                TranslationService.Instance.LocalCamera,
                TranslationService.Instance.NetworkStream
            };
            SourceTypeComboBox.SelectedIndex = 0;
            NetworkConfigPanel.IsEnabled = false;
            NetworkConfigPanel.Opacity = 0.4;

            ProcessingModeComboBox.ItemsSource = new string[]
            {
                TranslationService.Instance.ModeCanny,
                TranslationService.Instance.ModeSobel,
                TranslationService.Instance.ModeLaplacian,
                TranslationService.Instance.ModeBinary,
                TranslationService.Instance.ModeContour
            };
            ProcessingModeComboBox.SelectedIndex = 0;

            LogListBox.ItemsSource = AppLogger.Instance.Entries;
        }

        /// <summary>
        /// 窗口关闭：释放所有组件资源。
        /// </summary>
        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _recordingComponent.Dispose();
            _videoCaptureComponent.Dispose();
        }

        /// <summary>
        /// 信号源类型切换：显示/隐藏网络配置面板。
        /// </summary>
        private void SourceTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isNetwork = SourceTypeComboBox.SelectedIndex == 1;
            NetworkConfigPanel.IsEnabled = isNetwork;
            NetworkConfigPanel.Opacity = isNetwork ? 1.0 : 0.4;

            _videoCaptureComponent.SourceType = isNetwork
                ? Components.VideoSourceType.NetworkStream
                : Components.VideoSourceType.LocalCamera;

            if (isNetwork)
                BuildNetworkUrl();
        }

        /// <summary>根据 IP 和端口构建网络流 URL</summary>
        private void BuildNetworkUrl()
        {
            string ip = IPTextBox.Text.Trim();
            string port = PortTextBox.Text.Trim();
            _videoCaptureComponent.NetworkUrl = $"http://{ip}:{port}/video";
            _networkConfigured = !string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(port);
        }

        /// <summary>连接按钮：启动视频采集</summary>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceTypeComboBox.SelectedIndex == 1)
            {
                BuildNetworkUrl();
                if (!_networkConfigured)
                {
                    ShowError(TranslationService.GetStringStatic("CameraOpenError"));
                    return;
                }
            }

            HideError();
            _videoCaptureComponent.StopCapture();
            bool success = _videoCaptureComponent.StartCapture();
            if (success)
            {
                ConnectButton.IsEnabled = false;
                DisconnectButton.IsEnabled = true;
                AppLogger.Instance.Info("视频采集已启动");
            }
            else
            {
                ConnectButton.IsEnabled = true;
                DisconnectButton.IsEnabled = false;
                AppLogger.Instance.Error("视频采集启动失败");
            }
        }

        /// <summary>断开按钮：停止视频采集</summary>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _videoCaptureComponent.StopCapture();
            AppLogger.Instance.Info("视频采集已断开");
        }

        /// <summary>开启摄像头按钮</summary>
        private void StartCameraButton_Click(object sender, RoutedEventArgs e)
        {
            HideError();
            bool success = _videoCaptureComponent.StartCapture();
            if (success)
            {
                StartCameraButton.IsEnabled = false;
                StopCameraButton.IsEnabled = true;
                AppLogger.Instance.Info("摄像头已启动");
            }
            else
            {
                StartCameraButton.IsEnabled = true;
                StopCameraButton.IsEnabled = false;
                AppLogger.Instance.Error("摄像头启动失败");
            }
        }

        /// <summary>关闭摄像头按钮</summary>
        private void StopCameraButton_Click(object sender, RoutedEventArgs e)
        {
            _videoCaptureComponent.StopCapture();
            AppLogger.Instance.Info("摄像头已关闭");
        }

        /// <summary>
        /// 处理模式切换：更新当前处理模式，显示/隐藏阈值面板。
        /// </summary>
        private void ProcessingModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _currentMode = ProcessingModeComboBox.SelectedIndex switch
            {
                1 => Components.ProcessingMode.Sobel,
                2 => Components.ProcessingMode.Laplacian,
                3 => Components.ProcessingMode.Binary,
                4 => Components.ProcessingMode.Contour,
                _ => Components.ProcessingMode.Canny
            };

            ModeLabelText.Text = _currentMode switch
            {
                Components.ProcessingMode.Sobel => TranslationService.Instance.ModeSobel,
                Components.ProcessingMode.Laplacian => TranslationService.Instance.ModeLaplacian,
                Components.ProcessingMode.Binary => TranslationService.Instance.ModeBinary,
                Components.ProcessingMode.Contour => TranslationService.Instance.ModeContour,
                _ => TranslationService.Instance.ModeCanny
            };

            bool showThreshold = _currentMode is Components.ProcessingMode.Canny or Components.ProcessingMode.Contour;
            ThresholdPanel.Visibility = showThreshold ? Visibility.Visible : Visibility.Collapsed;

            AppLogger.Instance.Info($"处理模式切换为: {ModeLabelText.Text}");
        }

        /// <summary>
        /// 保存截图：将当前原始帧保存为 PNG 文件。
        /// </summary>
        private void SaveScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dir = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MachineVisionApp");
                System.IO.Directory.CreateDirectory(dir);
                string path = System.IO.Path.Combine(dir,
                    $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                var source = OriginalImage.Source as System.Windows.Media.Imaging.BitmapSource;
                if (source == null)
                {
                    ShowError(TranslationService.GetStringStatic("SaveFailed"));
                    return;
                }

                using var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create);
                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(source));
                encoder.Save(fileStream);

                AppLogger.Instance.Info($"{TranslationService.Instance.ScreenshotSaved} {path}");
            }
            catch (Exception ex)
            {
                ShowError($"{TranslationService.GetStringStatic("SaveFailed")}: {ex.Message}");
                AppLogger.Instance.Error($"截图保存失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 录制按钮：切换录制状态（开始/停止）。
        /// </summary>
        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_recordingComponent.IsRecording)
            {
                _recordingComponent.StopRecording();
                RecordButton.Content = TranslationService.Instance.StartRecording;
                RecordButton.ClearValue(Button.BackgroundProperty);
                AppLogger.Instance.Info(TranslationService.Instance.RecordingStopped);
            }
            else
            {
                string dir = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "MachineVisionApp");
                System.IO.Directory.CreateDirectory(dir);
                string path = System.IO.Path.Combine(dir,
                    $"Recording_{DateTime.Now:yyyyMMdd_HHmmss}.avi");

                var size = _videoCaptureComponent.GetResolution();
                int width = (int)size.Width;
                int height = (int)size.Height;
                if (width <= 0 || height <= 0)
                {
                    ShowError(TranslationService.GetStringStatic("SaveFailed"));
                    return;
                }

                bool started = _recordingComponent.StartRecording(path, 15.0, width, height);
                if (started)
                {
                    RecordButton.Content = TranslationService.Instance.StopRecording;
                    RecordButton.Background = new SolidColorBrush(Color.FromRgb(0xF8, 0x51, 0x49));
                    AppLogger.Instance.Info($"{TranslationService.Instance.RecordingStarted} {path}");
                }
                else
                {
                    ShowError(TranslationService.GetStringStatic("SaveFailed"));
                    AppLogger.Instance.Error("录像启动失败");
                }
            }
        }

        /// <summary>
        /// 日志切换按钮：显示/隐藏日志面板。
        /// </summary>
        private void LogToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = LogPanel.Visibility == Visibility.Visible;
            LogPanel.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>清空日志按钮：清空所有日志条目</summary>
        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Instance.Clear();
        }

        /// <summary>
        /// 连接状态变更处理：更新指示灯颜色、按钮启用状态、空状态遮罩。
        /// </summary>
        private void OnConnectionStateChangedHandler(Components.ConnectionState state)
        {
            Dispatcher.Invoke(() =>
            {
                Color color = state switch
                {
                    Components.ConnectionState.Connected => Color.FromRgb(0x3F, 0xB9, 0x50),
                    Components.ConnectionState.Connecting => Color.FromRgb(0xD2, 0x99, 0x22),
                    Components.ConnectionState.Failed => Color.FromRgb(0xF8, 0x51, 0x49),
                    _ => Color.FromRgb(0x48, 0x4F, 0x58)
                };

                var brush = new SolidColorBrush(color);
                ConnectionIndicator.Fill = brush;
                StatusIndicator.Fill = brush;
                StatusText.Text = TranslationService.Instance.GetConnectionStatusText(state);
                StatusTextFooter.Text = TranslationService.Instance.GetConnectionStatusText(state);

                if (state == Components.ConnectionState.Connected)
                {
                    ConnectButton.IsEnabled = false;
                    DisconnectButton.IsEnabled = true;
                    StartCameraButton.IsEnabled = false;
                    StopCameraButton.IsEnabled = true;
                    EmptyOverlayLeft.Visibility = Visibility.Collapsed;
                    EmptyOverlayRight.Visibility = Visibility.Collapsed;
                    StatusTextFooter.Foreground = new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50));
                    AppLogger.Instance.Info("设备已连接");
                }
                else if (state == Components.ConnectionState.Disconnected)
                {
                    ConnectButton.IsEnabled = true;
                    DisconnectButton.IsEnabled = false;
                    StartCameraButton.IsEnabled = true;
                    StopCameraButton.IsEnabled = false;
                    EmptyOverlayLeft.Visibility = Visibility.Visible;
                    EmptyOverlayRight.Visibility = Visibility.Visible;
                    StatusTextFooter.Foreground = new SolidColorBrush(Color.FromRgb(0x48, 0x4F, 0x58));
                }
                else if (state == Components.ConnectionState.Failed)
                {
                    AppLogger.Instance.Error("设备连接失败");
                }
                else if (state == Components.ConnectionState.Connecting)
                {
                    AppLogger.Instance.Info("正在连接设备...");
                }
            });
        }

        /// <summary>
        /// 每帧处理：执行图像处理（多模式）+ 人脸检测 + 更新显示 + FPS统计。
        /// 该方法在后台线程调用，UI 更新通过 Dispatcher 封送。
        /// </summary>
        private void ProcessFrame(Mat originalFrame, Mat grayFrame)
        {
            try
            {
                _frameStopwatch.Restart();

                Mat edges = _imageProcessingComponent.Process(
                    grayFrame, _currentMode, _threshold1, _threshold2, out int contourCount);

                int faceCount = _faceDetectionComponent.DetectFaces(grayFrame, originalFrame);

                _imageDisplayComponent.UpdateImages(originalFrame, edges, _threshold1, _threshold2);

                if (_recordingComponent.IsRecording)
                {
                    _recordingComponent.WriteFrame(originalFrame);
                }

                _frameStopwatch.Stop();
                long processTimeMs = _frameStopwatch.ElapsedMilliseconds;

                _frameCount++;
                var now = DateTime.Now;
                if ((now - _lastFpsUpdate).TotalSeconds >= 1.0)
                {
                    _currentFps = _frameCount / (now - _lastFpsUpdate).TotalSeconds;
                    _frameCount = 0;
                    _lastFpsUpdate = now;
                }

                Dispatcher.Invoke(() =>
                {
                    FaceCountTextBlock.Text = $"{faceCount}";
                    ContourCountTextBlock.Text = $"{contourCount}";
                    FpsTextBlock.Text = $"{_currentFps:F1} FPS";
                    ProcessTimeTextBlock.Text = $"{processTimeMs} ms";
                    ThresholdInfoText.Text = $"{_threshold1} ~ {_threshold2}";
                    UpdateCameraData();
                });
            }
            catch (Exception ex)
            {
                ShowError(TranslationService.GetStringStatic("FrameProcessError") + $": {ex.Message}");
                AppLogger.Instance.Error($"帧处理异常: {ex.Message}");
            }
        }

        /// <summary>阈值更新回调</summary>
        private void UpdateThresholds(int threshold1, int threshold2)
        {
            _threshold1 = threshold1;
            _threshold2 = threshold2;
            ThresholdInfoText.Text = $"{threshold1} ~ {threshold2}";
            AppLogger.Instance.Info($"阈值更新: {threshold1} ~ {threshold2}");
        }

        /// <summary>加载图片按钮：打开本地图片并执行处理管线</summary>
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = TranslationService.GetStringStatic("ImageFilter")
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Mat image = Cv2.ImRead(openFileDialog.FileName);
                    if (image.Empty())
                    {
                        ShowError(TranslationService.GetStringStatic("FailedLoadImage"));
                        return;
                    }

                    Mat grayImage = new Mat();
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
                    Mat edges = _imageProcessingComponent.Process(
                        grayImage, _currentMode, _threshold1, _threshold2, out int contourCount);
                    int faceCount = _faceDetectionComponent.DetectFaces(grayImage, image);
                    _imageDisplayComponent.UpdateImages(image, edges, _threshold1, _threshold2);
                    FaceCountTextBlock.Text = $"{faceCount}";
                    ContourCountTextBlock.Text = $"{contourCount}";
                    EmptyOverlayLeft.Visibility = Visibility.Collapsed;
                    EmptyOverlayRight.Visibility = Visibility.Collapsed;
                    HideError();
                    AppLogger.Instance.Info($"已加载图片: {openFileDialog.FileName}");
                }
                catch (Exception ex)
                {
                    ShowError(TranslationService.GetStringStatic("ImageProcessError") + $": {ex.Message}");
                    AppLogger.Instance.Error($"图片处理异常: {ex.Message}");
                }
            }
        }

        /// <summary>更新摄像头信息显示（帧率、分辨率）</summary>
        private void UpdateCameraData()
        {
            double frameRate = _videoCaptureComponent.GetFrameRate();
            System.Windows.Size resolution = _videoCaptureComponent.GetResolution();
            CameraDataTextBlock.Text = TranslationService.Instance.FormatCameraData(
                frameRate, resolution.Width, resolution.Height);
            SourceInfoText.Text = _videoCaptureComponent.GetSourceInfo();
        }

        /// <summary>捕获停止事件处理：清空画面、重置状态</summary>
        private void OnCaptureStoppedHandler(string? reason)
        {
            Dispatcher.Invoke(() =>
            {
                if (_recordingComponent.IsRecording)
                {
                    _recordingComponent.StopRecording();
                    RecordButton.Content = TranslationService.Instance.StartRecording;
                    RecordButton.ClearValue(Button.BackgroundProperty);
                }

                OriginalImage.Source = null;
                EdgeImage.Source = null;
                CameraDataTextBlock.Text = "";
                FaceCountTextBlock.Text = "0";
                ContourCountTextBlock.Text = "0";
                FpsTextBlock.Text = "0 FPS";
                ProcessTimeTextBlock.Text = "0 ms";
                EmptyOverlayLeft.Visibility = Visibility.Visible;
                EmptyOverlayRight.Visibility = Visibility.Visible;
                StopCameraButton.IsEnabled = false;
                StartCameraButton.IsEnabled = true;

                if (!string.IsNullOrEmpty(reason))
                {
                    ShowError(TranslationService.GetStringStatic("CameraStopped") + $": {reason}");
                    AppLogger.Instance.Warn($"采集停止: {reason}");
                }
            });
        }

        /// <summary>捕获错误事件处理</summary>
        private void OnCaptureErrorHandler(string? error)
        {
            string msg = error ?? TranslationService.GetStringStatic("UnknownError");
            Dispatcher.Invoke(() =>
            {
                ShowError(msg);
                AppLogger.Instance.Error(msg);
            });
        }

        /// <summary>显示错误信息</summary>
        private void ShowError(string message)
        {
            ErrorBorder.Visibility = Visibility.Visible;
            ErrorMessageTextBlock.Text = message;
        }

        /// <summary>隐藏错误信息</summary>
        private void HideError()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
            ErrorMessageTextBlock.Text = "";
        }
    }
}
