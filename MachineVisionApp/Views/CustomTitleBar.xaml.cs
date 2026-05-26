using System.Windows;
using System.Windows.Controls;

namespace MachineVisionApp.Views
{
    /// <summary>
    /// 自定义标题栏控件，提供窗口拖动、最小化、最大化/还原、关闭功能，
    /// 以及中英文语言切换按钮。
    /// </summary>
    public partial class CustomTitleBar : UserControl
    {
        private bool _isChinese = true; // 当前是否为中文

        public CustomTitleBar()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateButtonIcon();
        }

        /// <summary>语言切换按钮：在中英文之间切换</summary>
        private void LanguageSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            _isChinese = !_isChinese;
            string culture = _isChinese ? "zh-CN" : "en-US";
            TranslationService.Instance.ChangeLanguage(culture);
            LanguageSwitchButton.Content = _isChinese ? "中/EN" : "EN/中";
        }

        /// <summary>最小化按钮</summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        /// <summary>最大化/还原按钮</summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            UpdateButtonIcon();
        }

        /// <summary>根据窗口状态更新最大化/还原图标</summary>
        private void UpdateButtonIcon()
        {
            var window = Window.GetWindow(this);
            if (window == null) return;
            MaximizeButton.Content = window.WindowState == WindowState.Maximized
                ? "\uE923" // 还原图标
                : "\uE922"; // 最大化图标
        }

        /// <summary>关闭按钮</summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        /// <summary>
        /// 标题栏鼠标左键按下：支持窗口拖动和双击最大化。
        /// </summary>
        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (e.ClickCount == 2 && window?.ResizeMode != ResizeMode.NoResize)
            {
                // 双击最大化/还原
                MaximizeButton_Click(sender, e);
            }
            else
            {
                window?.DragMove();
            }
        }
    }
}
