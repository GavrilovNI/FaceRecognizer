using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.BetterSelenium
{
    public static class SeleniumExtensions
    {
        public static void Wait(this IWebDriver driver, double delay, double interval)
        {
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay));
            wait.PollingInterval = TimeSpan.FromMilliseconds(interval);
            wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromMilliseconds(delay) > TimeSpan.Zero);
        }

        public static void Wait(this IWebDriver driver, double delay)
        {
            Wait(driver, delay, delay);
        }

        public static void ScrollTo(this IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor je = (IJavaScriptExecutor)driver;
            je.ExecuteScript("arguments[0].scrollIntoView(false);", element);
        }

        public static void ScrollToThenClick(this IWebDriver driver, IWebElement element, double clickTryingInterval = 100)
        {
            ScrollTo(driver, element);
            while (true)
            {
                try
                {
                    element.Click();
                    break;
                }
                catch (Exception ex)
                {
                    Wait(driver, clickTryingInterval);
                }
            }
        }

        public static void Press(this IWebDriver driver, string keysToSend)
        {
            Actions actions = new Actions(driver);
            actions.SendKeys(keysToSend);
            actions.Perform();
        }

        public static IWebElement? TryFindElement(this IWebElement webElement, By by)
        {
            IWebElement? result;
            try
            {
                result = webElement.FindElement(by);
            }
            catch(NoSuchElementException ex)
            {
                result = null;
            }
            return result;
        }
    }
}
