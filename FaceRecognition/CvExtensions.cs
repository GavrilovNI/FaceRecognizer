using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.BetterCv
{
    public static class CvExtensions
    {
        public static Image<TColor, TDepth> EqualizeHist<TColor, TDepth>(this Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new()
        {
            CvInvoke.EqualizeHist(image, image);
            return image;
        }

        public static Image<TColor, TDepth> Crop<TColor, TDepth>(this Image<TColor, TDepth> image, Rectangle rectangle) where TColor : struct, IColor where TDepth : new()
        {
            image.ROI = rectangle;
            return image;
        }

        public static Image<TColor, TDepth> Resize<TColor, TDepth>(this Image<TColor, TDepth> image, Size size, double fx = 0, double fy = 0, Inter interpolation = Inter.Linear) where TColor : struct, IColor where TDepth : new()
        {
            CvInvoke.Resize(image, image, size, fx, fy, interpolation);
            return image;
        }

        public static Mat ToMat<TColor, TDepth>(this Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new()
        {
            return image.GetInputArray().GetMat();
        }

        public static void Show<TColor, TDepth>(this Image<TColor, TDepth> image, string windowName = "") where TColor : struct, IColor where TDepth : new()
        {
            CvInvoke.Imshow(windowName, image);
            CvInvoke.WaitKey();
        }
    }
}
