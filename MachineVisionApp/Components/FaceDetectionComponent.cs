using OpenCvSharp;

namespace MachineVisionApp.Components
{
    /// <summary>
    /// 人脸检测组件，基于 Haar 级联分类器实现。
    /// 支持配置检测灵敏度、最小人脸尺寸以及绘制边框的样式。
    /// </summary>
    public class FaceDetectionComponent
    {
        private readonly CascadeClassifier _faceCascade;

        // 检测参数
        private double _scaleFactor = 1.15;         // 图像缩放比例，越小越精确但越慢
        private int _minNeighbors = 4;              // 最小邻近检测数，越高误检越少
        private int _minFaceSize = 40;               // 最小人脸尺寸（像素），过滤噪点

        // 绘制参数
        private Scalar _rectangleColor = new Scalar(0, 255, 0); // 绿色框
        private int _rectangleThickness = 2;

        /// <summary>
        /// 初始化人脸检测组件，加载 Haar 级联分类器模型文件。
        /// </summary>
        /// <param name="cascadePath">Haar 级联文件路径（如 haarcascade_frontalface_default.xml）</param>
        public FaceDetectionComponent(string cascadePath)
        {
            _faceCascade = new CascadeClassifier(cascadePath);
        }

        /// <summary>检测缩放系数（默认 1.15），值越小检测越精细但耗时更长</summary>
        public double ScaleFactor
        {
            get => _scaleFactor;
            set => _scaleFactor = Math.Clamp(value, 1.01, 3.0);
        }

        /// <summary>最小邻近检测数（默认 4），值越高误检越少但可能漏检</summary>
        public int MinNeighbors
        {
            get => _minNeighbors;
            set => _minNeighbors = Math.Clamp(value, 1, 10);
        }

        /// <summary>最小人脸尺寸（默认 40px），低于此尺寸的候选区域将被过滤</summary>
        public int MinFaceSize
        {
            get => _minFaceSize;
            set => _minFaceSize = Math.Max(value, 10);
        }

        /// <summary>人脸框颜色（默认绿色）</summary>
        public Scalar RectangleColor
        {
            get => _rectangleColor;
            set => _rectangleColor = value;
        }

        /// <summary>人脸框线宽（默认 2）</summary>
        public int RectangleThickness
        {
            get => _rectangleThickness;
            set => _rectangleThickness = Math.Max(value, 1);
        }

        /// <summary>
        /// 在灰度图像中检测人脸，并在原始图像上绘制矩形框。
        /// 检测前会自动进行直方图均衡化，提高低光照环境下的检出率。
        /// </summary>
        /// <param name="grayFrame">输入灰度图像</param>
        /// <param name="originalFrame">原始彩色图像（用于绘制）</param>
        /// <returns>检测到的人脸数量</returns>
        public int DetectFaces(Mat grayFrame, Mat originalFrame)
        {
            // 直方图均衡化：增强对比度，改善暗光下人脸检测效果
            using Mat equalized = new Mat();
            Cv2.EqualizeHist(grayFrame, equalized);

            // 执行多尺度人脸检测
            OpenCvSharp.Rect[] faces = _faceCascade.DetectMultiScale(
                equalized,
                _scaleFactor,
                _minNeighbors,
                HaarDetectionTypes.ScaleImage,
                new OpenCvSharp.Size(_minFaceSize, _minFaceSize));

            // 在原始帧上绘制人脸框
            foreach (OpenCvSharp.Rect face in faces)
            {
                Cv2.Rectangle(originalFrame, face, _rectangleColor, _rectangleThickness);
            }

            return faces.Length;
        }
    }
}
