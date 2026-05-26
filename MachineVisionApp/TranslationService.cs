using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace MachineVisionApp
{
    /// <summary>
    /// 国际化翻译服务（单例模式）。
    /// 基于 .resx 资源文件提供中英文切换功能。
    /// 实现 INotifyPropertyChanged，切换语言时通过 PropertyChanged("") 刷新所有 XAML 绑定。
    /// </summary>
    public class TranslationService : INotifyPropertyChanged
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(
            "MachineVisionApp.Resources.Strings", typeof(TranslationService).Assembly);

        private static readonly TranslationService _instance = new();
        public static TranslationService Instance => _instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ChangeLanguage(string cultureName)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public string this[string key] => GetString(key);

        // ---- UI 绑定属性 ----
        public string AppTitle => GetString("AppTitle");
        public string OriginalStream => GetString("OriginalStream");
        public string EdgeDetection => GetString("EdgeDetection");
        public string Camera => GetString("Camera");
        public string CameraWaiting => GetString("CameraWaiting");
        public string Faces => GetString("Faces");
        public string CannyThreshold => GetString("CannyThreshold");
        public string ThresholdSeparator => GetString("ThresholdSeparator");
        public string Apply => GetString("Apply");
        public string StartCamera => GetString("StartCamera");
        public string StopCamera => GetString("StopCamera");
        public string LoadImage => GetString("LoadImage");
        public string CameraStopped => GetString("CameraStopped");
        public string MachineVision => GetString("MachineVision");
        public string Minimize => GetString("Minimize");
        public string Maximize => GetString("Maximize");
        public string Close => GetString("Close");
        public string SourceType => GetString("SourceType");
        public string LocalCamera => GetString("LocalCamera");
        public string NetworkStream => GetString("NetworkStream");
        public string IPAddress => GetString("IPAddress");
        public string Port => GetString("Port");
        public string Connect => GetString("Connect");
        public string Disconnect => GetString("Disconnect");
        public string RTSPHint => GetString("RTSPHint");
        public string StatusDisconnected => GetString("StatusDisconnected");
        public string ProcessingMode => GetString("ProcessingMode");
        public string ModeCanny => GetString("ModeCanny");
        public string ModeSobel => GetString("ModeSobel");
        public string ModeLaplacian => GetString("ModeLaplacian");
        public string ModeBinary => GetString("ModeBinary");
        public string ModeContour => GetString("ModeContour");
        public string SaveScreenshot => GetString("SaveScreenshot");
        public string StartRecording => GetString("StartRecording");
        public string StopRecording => GetString("StopRecording");
        public string Recording => GetString("Recording");
        public string Log => GetString("Log");
        public string ClearLog => GetString("ClearLog");
        public string ScreenshotSaved => GetString("ScreenshotSaved");
        public string RecordingStarted => GetString("RecordingStarted");
        public string RecordingStopped => GetString("RecordingStopped");
        public string ProcessingTime => GetString("ProcessingTime");

        public string FormatCameraData(double fps, double width, double height)
        {
            return $"{fps:F1} FPS \u00B7 {width}x{height}";
        }

        public string GetConnectionStatusText(Components.ConnectionState state)
        {
            return state switch
            {
                Components.ConnectionState.Connected => GetString("StatusConnected"),
                Components.ConnectionState.Connecting => GetString("StatusConnecting"),
                Components.ConnectionState.Failed => GetString("StatusFailed"),
                _ => GetString("StatusDisconnected")
            };
        }

        public string GetString(string key)
        {
            string? value = _resourceManager.GetString(key);
            return value ?? key;
        }

        public string GetString(string key, params object[] args)
        {
            string? value = _resourceManager.GetString(key);
            return value != null ? string.Format(value, args) : key;
        }

        public static string GetStringStatic(string key)
        {
            string? value = _resourceManager.GetString(key);
            return value ?? key;
        }
    }
}
