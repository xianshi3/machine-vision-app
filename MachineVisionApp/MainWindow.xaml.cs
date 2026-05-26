using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using OpenCvSharp;

namespace MachineVisionApp
{
    /// <summary>
    /// 主窗口，负责整个应用的 UI 交互和业务逻辑编排。
    /// 协调视频采集、图像处理、人脸检测、边缘检测等各组件的协作。
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // ---- 各功能组件 ----
        private Components.VideoCaptureComponent _videoCaptureComponent;         // 视频采集
        private Components.ImageDisplayComponent _imageDisplayComponent;         // 图像显示
        private Components.ThresholdParameterComponent _thresholdParameterComponent; // 阈值参数
        private Components.FaceDetectionComponent _faceDetectionComponent;       // 人脸检测
        private Components.EdgeDetectionComponent _edgeDetectionComponent;       // 边缘检测

        // ---- Canny 阈值 ----
        private int _threshold1 = 100; // 低阈值
        private int _threshold2 = 200; // 高阈值

        private bool _networkConfigured; // 网络流是否已配置

        /// <summary>
        /// 构造函数：初始化组件、注册事件。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // 初始化各功能组件
            _faceDetectionComponent = new Components.FaceDetectionComponent("haarcascade_frontalface_default.xml");
            _edgeDetectionComponent = new Components.EdgeDetectionComponent();
            _videoCaptureComponent = new Components.VideoCaptureComponent();
            _imageDisplayComponent = new Components.ImageDisplayComponent(OriginalImage, EdgeImage);
            _thresholdParameterComponent = new Components.ThresholdParameterComponent(
                Threshold1TextBox, Threshold2TextBox, ApplyThresholdsButton);

            // 注册视频采集事件
            _videoCaptureComponent.OnFrameCaptured += ProcessFrame;
            _videoCaptureComponent.OnCaptureStopped += OnCaptureStoppedHandler;
            _videoCaptureComponent.OnCaptureError += OnCaptureErrorHandler;
            _videoCaptureComponent.OnConnectionStateChanged += OnConnectionStateChangedHandler;

            // 注册阈值变更事件
            _thresholdParameterComponent.OnThresholdsChanged += UpdateThresholds;

            // 窗口生命周期事件
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
        }

        /// <summary>
        /// 窗口关闭：释放视频采集资源。
        /// </summary>
        private void MainWindow_Closed(object? sender, EventArgs e)
        {
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
            }
            else
            {
                ConnectButton.IsEnabled = true;
                DisconnectButton.IsEnabled = false;
            }
        }

        /// <summary>断开按钮：停止视频采集</summary>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _videoCaptureComponent.StopCapture();
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
            }
            else
            {
                StartCameraButton.IsEnabled = true;
                StopCameraButton.IsEnabled = false;
            }
        }

        /// <summary>关闭摄像头按钮</summary>
        private void StopCameraButton_Click(object sender, RoutedEventArgs e)
        {
            _videoCaptureComponent.StopCapture();
        }

        /// <summary>
        /// 连接状态变更处理：更新指示灯颜色、按钮启用状态、空状态遮罩。
        /// </summary>
        private void OnConnectionStateChangedHandler(Components.ConnectionState state)
        {
            Dispatcher.Invoke(() =>
            {
                // 根据状态选择颜色
                Color color = state switch
                {
                    Components.ConnectionState.Connected => Color.FromRgb(0x3F, 0xB9, 0x50),  // 绿
                    Components.ConnectionState.Connecting => Color.FromRgb(0xD2, 0x99, 0x22), // 黄
                    Components.ConnectionState.Failed => Color.FromRgb(0xF8, 0x51, 0x49),    // 红
                    _ => Color.FromRgb(0x48, 0x4F, 0x58)                                     // 灰
                };

                var brush = new SolidColorBrush(color);
                ConnectionIndicator.Fill = brush;
                StatusIndicator.Fill = brush;
                StatusText.Text = TranslationService.Instance.GetConnectionStatusText(state);
                StatusTextFooter.Text = TranslationService.Instance.GetConnectionStatusText(state);

                if (state == Components.ConnectionState.Connected)
                {
                    // 已连接：启用停止，隐藏空遮罩
                    ConnectButton.IsEnabled = false;
                    DisconnectButton.IsEnabled = true;
                    StartCameraButton.IsEnabled = false;
                    StopCameraButton.IsEnabled = true;
                    EmptyOverlayLeft.Visibility = Visibility.Collapsed;
                    EmptyOverlayRight.Visibility = Visibility.Collapsed;
                    StatusTextFooter.Foreground = new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50));
                }
                else if (state == Components.ConnectionState.Disconnected)
                {
                    // 已断开：启用连接，显示空遮罩
                    ConnectButton.IsEnabled = true;
                    DisconnectButton.IsEnabled = false;
                    StartCameraButton.IsEnabled = true;
                    StopCameraButton.IsEnabled = false;
                    EmptyOverlayLeft.Visibility = Visibility.Visible;
                    EmptyOverlayRight.Visibility = Visibility.Visible;
                    StatusTextFooter.Foreground = new SolidColorBrush(Color.FromRgb(0x48, 0x4F, 0x58));
                }
            });
        }

        /// <summary>
        /// 每帧处理：执行边缘检测 + 人脸检测 + 更新显示。
        /// 该方法在后台线程调用，UI 更新通过 Dispatcher 封送。
        /// </summary>
        private void ProcessFrame(Mat originalFrame, Mat grayFrame)
        {
            try
            {
                // 边缘检测
                Mat edges = _edgeDetectionComponent.DetectEdges(grayFrame, _threshold1, _threshold2);
                // 人脸检测
                int faceCount = _faceDetectionComponent.DetectFaces(grayFrame, originalFrame);
                // 更新显示
                _imageDisplayComponent.UpdateImages(originalFrame, edges, _threshold1, _threshold2);

                // 更新 UI 文本
                Dispatcher.Invoke(() =>
                {
                    FaceCountTextBlock.Text = $"{faceCount}";
                    UpdateCameraData();
                });
            }
            catch (Exception ex)
            {
                ShowError(TranslationService.GetStringStatic("FrameProcessError") + $": {ex.Message}");
            }
        }

        /// <summary>阈值更新回调</summary>
        private void UpdateThresholds(int threshold1, int threshold2)
        {
            _threshold1 = threshold1;
            _threshold2 = threshold2;
            ThresholdInfoText.Text = $"{threshold1} ~ {threshold2}";
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

                    // 灰度转换 → 边缘检测 → 人脸检测 → 显示
                    Mat grayImage = new Mat();
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
                    Mat edges = _edgeDetectionComponent.DetectEdges(grayImage, _threshold1, _threshold2);
                    int faceCount = _faceDetectionComponent.DetectFaces(grayImage, image);
                    _imageDisplayComponent.UpdateImages(image, edges, _threshold1, _threshold2);
                    FaceCountTextBlock.Text = $"{faceCount}";
                    EmptyOverlayLeft.Visibility = Visibility.Collapsed;
                    EmptyOverlayRight.Visibility = Visibility.Collapsed;
                    HideError();
                }
                catch (Exception ex)
                {
                    ShowError(TranslationService.GetStringStatic("ImageProcessError") + $": {ex.Message}");
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
                // 清空图像
                OriginalImage.Source = null;
                EdgeImage.Source = null;
                CameraDataTextBlock.Text = "";
                FaceCountTextBlock.Text = "0";
                // 显示空状态
                EmptyOverlayLeft.Visibility = Visibility.Visible;
                EmptyOverlayRight.Visibility = Visibility.Visible;
                // 按钮状态
                StopCameraButton.IsEnabled = false;
                StartCameraButton.IsEnabled = true;

                if (!string.IsNullOrEmpty(reason))
                    ShowError(TranslationService.GetStringStatic("CameraStopped") + $": {reason}");
            });
        }

        /// <summary>捕获错误事件处理</summary>
        private void OnCaptureErrorHandler(string? error)
        {
            Dispatcher.Invoke(() => ShowError(error ?? TranslationService.GetStringStatic("UnknownError")));
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
