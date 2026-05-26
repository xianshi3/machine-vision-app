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
        // 资源管理器，指向 MachineVisionApp.Resources.Strings 资源文件
        private static readonly ResourceManager _resourceManager = new ResourceManager(
            "MachineVisionApp.Resources.Strings", typeof(TranslationService).Assembly);

        private static readonly TranslationService _instance = new();
        public static TranslationService Instance => _instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>切换当前 UI 语言</summary>
        /// <param name="cultureName">区域名称，如 "zh-CN" 或 "en-US"</param>
        public void ChangeLanguage(string cultureName)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            // 触发所有绑定刷新
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        /// <summary>根据 key 获取字符串（索引器语法）</summary>
        public string this[string key] => GetString(key);

        // ---- XAML 绑定属性 ----
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

        /// <summary>格式化摄像头信息字符串</summary>
        public string FormatCameraData(double fps, double width, double height)
        {
            return $"{fps:F1} FPS \u00B7 {width}x{height}";
        }

        /// <summary>根据连接状态返回对应的本地化文本</summary>
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

        /// <summary>根据 key 从资源文件获取本地化字符串</summary>
        public string GetString(string key)
        {
            string? value = _resourceManager.GetString(key);
            return value ?? key;
        }

        /// <summary>获取带格式化参数的本地化字符串</summary>
        public string GetString(string key, params object[] args)
        {
            string? value = _resourceManager.GetString(key);
            return value != null ? string.Format(value, args) : key;
        }

        /// <summary>静态方法：根据 key 获取本地化字符串（供无实例场景使用）</summary>
        public static string GetStringStatic(string key)
        {
            string? value = _resourceManager.GetString(key);
            return value ?? key;
        }
    }
}
