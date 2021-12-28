using System;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using Emgu.CV.CvEnum;
using System.Drawing;
using Emgu.CV.Face;
using System.Net;
using System.Drawing.Imaging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using FaceRecognition.BetterCv;

namespace FaceRecognition
{

    public class Program
    {
        private const string SolutionDirectory = "./../../../../";
        private static CascadeClassifier faceCascadeClassifier = new CascadeClassifier(SolutionDirectory + "haarcascades/haarcascade_frontalface_alt.xml");

        public static Image<Bgr, byte> Test(Image<Bgr, byte> image, EigenFaceRecognizer recognizer, List<string> names)
        {
            Image<Bgr, byte> imageWithRects = image.Clone();
            Image<Gray, byte> grayscale = imageWithRects.Convert<Gray, byte>();
            var imageForFaceSearching = grayscale.Clone().EqualizeHist();

            Rectangle[] faceRects = faceCascadeClassifier.DetectMultiScale(imageForFaceSearching, 1.1, 3, Size.Empty, Size.Empty);

            if (faceRects.Length > 0)
            {
                foreach (var faceRect in faceRects)
                {
                    var face = grayscale.Clone().Crop(faceRect).Resize(new Size(200, 200)).EqualizeHist();

                    var result = recognizer.Predict(face);

                    string name = result.Label >= 0 ? names[result.Label] : "Unknown";
                    Color faceRectColor = result.Label >= 0 ? Color.Green : Color.Red;

                    string info = name + ", distance: " + result.Distance;
                    Console.WriteLine(info);

                    CvInvoke.PutText(imageWithRects, name, new Point(faceRect.X - 2, faceRect.Y + 20),
                            FontFace.HersheyComplex, 0.5, new Bgr(Color.Red).MCvScalar);
                    CvInvoke.Rectangle(imageWithRects, faceRect, new Bgr(faceRectColor).MCvScalar, 2);
                }
            }

            return imageWithRects;
        }


        public static void Main()
        {
            //ImageLoader.DownloadImagesFromYandex("emma watson", SolutionDirectory + "TestImages/emma/", 100);
            //ImageLoader.DownloadImagesFromYandex("daniel radcliffe", SolutionDirectory + "TestImages/daniel/", 100);
            //ImageLoader.DownloadImagesFromYandex("rupert grint", SolutionDirectory + "TestImages/rupert/", 100);

            (EigenFaceRecognizer recognizer, List<string> names) = TrainImages();

            while(true)
            {
                string[] files = Directory.GetFiles(SolutionDirectory + "TestImages/test", "*.*", SearchOption.AllDirectories)
                                              .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")).ToArray();

                foreach (string file in files)
                {
                    Console.WriteLine(file);
                    var result = Test(new Image<Bgr, byte>(file), recognizer, names);
                    result.Show();
                }
            }
        }



        public static (EigenFaceRecognizer, List<string>) TrainImages(double threshold = 200000)
        {
            int imagesCount = 0;

            List<Mat> trainedFaces = new List<Mat>();
            List<int> trainedLabels = new List<int>();
            List<string> trainedNames = new List<string>();

            EigenFaceRecognizer recognizer = new EigenFaceRecognizer(imagesCount, threshold);

            try
            {
                string pathToFolders = SolutionDirectory + "TestImages";
                string[] subjects = new string[] { "emma", "daniel", "rupert" };
                foreach (string subject in subjects)
                {
                    string pathToSubject = pathToFolders + "/" + subject;
                    string[] files = Directory.GetFiles(pathToSubject, "*.*", SearchOption.AllDirectories)
                                              .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")).ToArray();

                    foreach (string file in files)
                    {
                        Image<Gray, byte> grayscale = new Image<Bgr, byte>(file).Convert<Gray, byte>();
                        var imageForFaceSearching = grayscale.Clone().EqualizeHist();

                        Rectangle[] faceRects = faceCascadeClassifier.DetectMultiScale(imageForFaceSearching, 1.1, 3, Size.Empty, Size.Empty);

                        foreach (Rectangle faceRect in faceRects)
                        {
                            var face = grayscale.Clone().Crop(faceRect).Resize(new Size(200, 200)).EqualizeHist();

                            trainedFaces.Add(face.ToMat());
                            trainedLabels.Add(imagesCount);
                        }
                        imagesCount++;
                        trainedNames.Add(subject);
                    }
                }
                recognizer.Train(trainedFaces.ToArray(), trainedLabels.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return (recognizer, trainedNames);
        }

    }
}
