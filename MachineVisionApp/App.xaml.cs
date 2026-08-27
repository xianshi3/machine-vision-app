using System.Globalization;
using System.Windows;

namespace MachineVisionApp
{
    /// <summary>
    /// 应用程序入口点。
    /// 负责应用级资源加载和启动配置。
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 启动时设置默认语言为英语。
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            base.OnStartup(e);
        }
    }
}
