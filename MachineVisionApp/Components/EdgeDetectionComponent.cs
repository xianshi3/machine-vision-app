using OpenCvSharp;

namespace MachineVisionApp.Components
{
    /// <summary>
    /// Canny 边缘检测组件。
    /// 在执行 Canny 算子前会自动进行高斯模糊预处理，减少噪声干扰，
    /// 使检测出的边缘更连续、更干净。
    /// </summary>
    public class EdgeDetectionComponent
    {
        private int _gaussianKernelSize = 5; // 高斯模糊核大小（必须为奇数）

        /// <summary>
        /// 高斯模糊核大小（默认 5），用于边缘检测前的降噪。
        /// 值越大模糊越强，细小边缘可能被过滤。
        /// </summary>
        public int GaussianKernelSize
        {
            get => _gaussianKernelSize;
            set
            {
                // 确保为奇数且 >= 3
                int v = Math.Max(value, 3);
                _gaussianKernelSize = v % 2 == 1 ? v : v + 1;
            }
        }

        /// <summary>
        /// 对灰度图像执行 Canny 边缘检测。
        /// 流程：高斯模糊降噪 → Canny 算子提取边缘。
        /// </summary>
        /// <param name="grayFrame">输入灰度图像</param>
        /// <param name="threshold1">Canny 低阈值</param>
        /// <param name="threshold2">Canny 高阈值</param>
        /// <returns>边缘图像（二值图）</returns>
        public Mat DetectEdges(Mat grayFrame, int threshold1, int threshold2)
        {
            // 第一步：高斯模糊降噪，使边缘检测更稳定
            using Mat blurred = new Mat();
            Cv2.GaussianBlur(grayFrame, blurred, new OpenCvSharp.Size(_gaussianKernelSize, _gaussianKernelSize), 1.5);

            // 第二步：Canny 边缘检测
            Mat edges = new Mat();
            Cv2.Canny(blurred, edges, threshold1, threshold2);

            return edges;
        }
    }
}
