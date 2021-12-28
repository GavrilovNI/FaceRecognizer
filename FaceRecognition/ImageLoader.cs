using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using FaceRecognition.BetterSelenium;
using Emgu.CV.Structure;

namespace FaceRecognition
{
    public static class ImageLoader
    {
        public static void Download(string imageUrl, string filename, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }


        private static IWebElement GetYandexImageDiv(IWebDriver driver, int index)
        {
            int linesCount = driver.FindElements(By.ClassName("justifier__col")).Count;

            if (linesCount == 0)
            {
                int divNumber = 1;
                if ("misspell" == driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div[1]/div[1]"))?.GetAttribute("className"))
                    divNumber = 2; // yandex trying to fix your searchText

                return driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div[1]/div[" + divNumber + "]/div/div[" + (index + 1) + "]/div"));
            }
            else
            {
                int currentLine = index % linesCount + 1;
                int currentImageInLine = index / linesCount + 1;
                return driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div[1]/div[1]/div/div/div[" + currentLine + "]/div[" + currentImageInLine + "]/div"));
            }
        }
        public static void DownloadImagesFromYandex(string searchText, string saveDirection, int count)
        {
            DownloadImagesFromYandex(searchText, saveDirection, 0, count);
        }
        public static void DownloadImagesFromYandex(string searchText, string saveDirection, int startFrom, int count)
        {
            ChromeDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://yandex.ru/");

            var searchLine = driver.FindElement(By.XPath("//*[@id=\"text\"]"));
            searchLine.Clear();
            searchLine.SendKeys(searchText);

            driver.Press(Keys.Enter);

            var imageBtn = driver.FindElement(By.XPath("/html/body/div[2]/nav/ul/li[2]/div[1]/a"));
            imageBtn.Click(); // open images tab

            driver.Wait(1000);
            driver.SwitchTo().Window(driver.WindowHandles.Last());

            for (int i = 0; i < startFrom; i++)
            {
                IWebElement imageDiv = GetYandexImageDiv(driver, i);
                driver.ScrollTo(imageDiv);
                driver.Wait(50);
            }

            for (int i = startFrom; i < count + startFrom; i++)
            {
                IWebElement? image = GetYandexImageDiv(driver, i).TryFindElement(By.TagName("img"));
                if(image != null)
                {
                    driver.ScrollTo(image);
                    driver.Wait(50);

                    string src = image.GetAttribute("src");
                    Download(src, saveDirection + i + ".jpg", ImageFormat.Jpeg);
                }
            }

            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles.First());
            driver.Close();
        }
    }
}
