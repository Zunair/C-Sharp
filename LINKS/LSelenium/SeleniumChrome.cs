using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Chrome
{
    public static class Chrome
    {
        public static SeleniumChrome Instance;
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string ChromeDir = string.Empty;

        public static void SetChromeInstance(bool fullScreen = false, bool headless = true, bool appMode = false, string appUrl = "")
        {
            if (Chrome.Instance == null || Chrome.Instance.CD == null)
            {
                Chrome.Instance = new SeleniumChrome(fullScreen: fullScreen, headless: headless, appMode: appMode, appUrl: appUrl);
            }
        }

        public static string Close()
        {
            string retVal = "";

            if (Chrome.Instance != null)
            {
                if (Chrome.Instance.CD != null)
                {
                    try
                    {
                        try
                        {
                            Chrome.Instance.CD.Close();
                        }
                        catch { }

                        Chrome.Instance.CD.Quit();
                        Chrome.Instance.CD = null;
                    }
                    catch { }
                }
                Chrome.Instance = null;
            }

            return retVal;
        }

        public static void OnLoad()
        {   
            ChromeDir = System.IO.Path.Combine(AppData, "LINKS\\Customization\\Plugins\\Google\\Chrome\\");
            System.IO.Directory.CreateDirectory(ChromeDir);
        }

        public static void OnDispose()
        {
            Close();
        }
    }

    public class SeleniumChrome
    {

        ChromeDriverService _cDs;
        ChromeDriver _cD;
        int _tries = 0;

        public ChromeDriver CD
        {
            get
            {
                return _cD;
            }

            set
            {
                _cD = value;
            }
        }
        
        public SeleniumChrome(bool fullScreen = false, bool headless = true, bool appMode = true, string appUrl = "")
        {
            _cDs = ChromeDriverService.CreateDefaultService();
            _cDs.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();

            Chrome.ChromeDir = System.IO.Path.Combine(Chrome.AppData, "LINKS\\Customization\\Plugins\\Google\\Chrome\\");
            string ChromeProfile = System.IO.Path.Combine(Chrome.ChromeDir, "profile");
            System.IO.Directory.CreateDirectory(Chrome.ChromeDir);
            options.AddArguments("user-data-dir=" + ChromeProfile);
            if (fullScreen) options.AddArguments("start-fullscreen");
            if (headless) options.AddArguments("headless");
            if (appMode) options.AddArguments("app=" + appUrl);
            options.AddArguments("dns-prefetch-enable");

            try
            {
                CD = new ChromeDriver(_cDs, options, TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                if (!ex.Message.ToLower().Contains("timed out"))
                    throw new Exception("Error: " + ex.Message);
            }
        }

        public string Translate(string languageTranslateFrom, string languageTo, string phraseToTranslate, string getStringOrSetAsClipboard)
        {
            string retVal = "";
            Uri uri = new Uri("https://translate.google.com/m/translate?hl=" + languageTranslateFrom + "#auto/" + languageTo + "/" + phraseToTranslate);
            _tries++;

            try
            {
                CD.Navigate().GoToUrl(uri);
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(40));
                wait.Until(d => d.Title.Contains("Google"));
                CD.Manage();

                IWebElement wE;
                wait.Until(d => d.PageSource.Contains("translation"));

                if (getStringOrSetAsClipboard != string.Empty)
                {
                    wE = CD.FindElement(By.ClassName("translation"));
                    retVal = wE.Text;
                }

                wait.Until(d => d.PageSource.Contains("res-tts"));
                wait.Until(d => d.FindElement(By.ClassName("res-tts")).Enabled);
                wE = CD.FindElement(By.ClassName("res-tts"));
                wE.Click();

                wait.Until(d => d.FindElement(By.ClassName("res-tts")).GetAttribute("aria-pressed") == "false");

                CD.Navigate().Back(); // fixes timed out issues

                _tries = 0;
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("chrome not") && _tries < 3)
                {
                    CD = null;
                    CD = new ChromeDriver(_cDs);
                    retVal = Translate(languageTranslateFrom, languageTo, phraseToTranslate, getStringOrSetAsClipboard);
                }
                else
                {
                    Console.WriteLine("Error:" + e.Message);
                }
            }

            return retVal;
        }

        public string WaitForTitleContains(string titleString, int timeout)
        {
            string retVal = string.Empty;

            var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));

            wait.Until(d => d.Title.Contains(titleString));
            CD.Manage();

            return retVal;
        }

        public string WaitForCssSelectorWithAttibuteAndValue(string cssSelector, string attributeName, int timeout)
        {
            string retVal = string.Empty;

            var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));
            wait.Until(d => d.FindElement(By.CssSelector(cssSelector)).GetAttribute(attributeName));
            CD.Manage();

            return retVal;
        }

        public string WaitForSourceContains(string sourceString, int timeout)
        {
            string retVal = string.Empty;

            var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));

            wait.Until(d => d.PageSource.Contains(sourceString));
            CD.Manage();

            return retVal;
        }

        public string WaitForClassWithAttibuteAndValue(string singleClassName, string attributeName, string attributeValue, int timeout)
        {
            string retVal = string.Empty;

            try
            {
                var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));

                wait.Until(d => d.FindElement(By.ClassName(singleClassName)).GetAttribute(attributeName) == attributeValue);
                CD.Manage();
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string WaitForURLContains(string caseSensitiveString, int timeout)
        {
            string retVal = string.Empty;

            try
            {
                var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));
                wait.Until(d => d.Url.Contains(caseSensitiveString));
                CD.Manage();
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string ClickElementByCssSelector(string cssSelector)
        {
            string retVal = string.Empty;

            cssSelector = cssSelector.Replace('{', '[').Replace('}', ']');

            try
            {
                IWebElement wE;

                wE = CD.FindElementByCssSelector(cssSelector);
                wE.Click();

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;

        }

        public string ClickElementByClass(string singleClassName)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElement(By.ClassName(singleClassName));
                wE.Click();

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;

        }

        public string ClickElementByID(string elementID)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElement(By.Id(elementID));
                wE.Click();

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string ClickElementByXPath(string xPath)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElementByXPath(xPath);
                wE.Click();

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;

        }

        public string SetElementValueByXPath(string xPath, string sendKeys)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElementByXPath(xPath);
                wE.SendKeys(sendKeys);

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;

        }

        public string SetElementValueByCssSelector(string cssSelector, string sendKeys)
        {
            string retVal = string.Empty;

            cssSelector = cssSelector.Replace('{', '[').Replace('}', ']');

            try
            {
                IWebElement wE;

                wE = CD.FindElementByCssSelector(cssSelector);
                wE.SendKeys(sendKeys);

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string GetElementValueByCssSelector(string cssSelector)
        {
            string retVal = string.Empty;

            cssSelector = cssSelector.Replace('{', '[').Replace('}', ']');

            try
            {
                IWebElement wE;

                wE = CD.FindElementByCssSelector(cssSelector);
                retVal = wE.Text;

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string GetElementValueByCssSelectorAndAttribute(string cssSelector, string attribute)
        {
            string retVal = string.Empty;

            cssSelector = cssSelector.Replace('{', '[').Replace('}', ']');

            try
            {
                retVal = CD.FindElementByCssSelector(cssSelector).GetAttribute(attribute);
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string GetScreenshot(string filePath)
        {
            string retVal = string.Empty;

            if (System.IO.Path.GetPathRoot(filePath) == string.Empty)
            {
                string screenshotsDir = System.IO.Path.Combine(Chrome.ChromeDir, "Screenshots");
                System.IO.Directory.CreateDirectory(screenshotsDir);
                filePath = System.IO.Path.Combine(screenshotsDir, filePath);
            }

            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            try
            {
                CD.GetScreenshot().SaveAsFile(filePath);
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string GetPageSource()
        {
            string retVal = string.Empty;

            try
            {

                retVal = CD.PageSource;

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;

        }

        public string WaitTillElementByIDIsEnabled(string elementID, int timeout)
        {
            string retVal = string.Empty;

            try
            {
                var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));
                wait.Until(d => d.FindElement(By.ClassName(elementID)).Enabled);
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string WaitTillElementByClassIsEnabled(string singleClassName, int timeout)
        {
            string retVal = string.Empty;

            try
            {
                var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(timeout));
                wait.Until(d => d.FindElement(By.ClassName(singleClassName)).Enabled);
            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string SetElementTextByClass(string singleClassName, string sendKeys)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElement(By.ClassName(singleClassName));
                wE.SendKeys(sendKeys);

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string SetElementTextByID(string elementID, string sendKeys)
        {
            string retVal = string.Empty;

            try
            {
                IWebElement wE;

                wE = CD.FindElement(By.Id(elementID));
                wE.SendKeys(sendKeys);

            }
            catch (Exception ex)
            {
                retVal = "Error: " + ex.Message;
            }

            return retVal;
        }

        public string Goto(string url)
        {
            string retVal = "";
            Uri uri = new Uri(url);
            _tries++;

            try
            {
                CD.Navigate().GoToUrl(uri);

                _tries = 0;
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("chrome not") && _tries < 3)
                {
                    CD = null;
                    CD = new ChromeDriver(_cDs);
                }
                else
                {
                    retVal = "Error:" + e.Message;
                }
            }

            return retVal;
        }



        //public string Hangouts(string command, string phoneNum = "")
        //{

        //    string retVal = "";
        //    Uri uri = new Uri("https://hangouts.google.com/?hs=0&action=chat&pn=" + phoneNum);
        //    _tries++;

        //    try
        //    {
        //        CD.Navigate().GoToUrl(uri);

        //        var wait = new WebDriverWait(CD, TimeSpan.FromSeconds(40));
        //        wait.Until(d => d.Title.Contains("Google"));
        //        CD.Manage();

        //        IWebElement wE;
        //        wait.Until(d => d.PageSource.Contains("Your call is free."));

        //        //CD.FileDetector;
        //        System.Threading.Thread.Sleep(3000);
        //        CD.Keyboard.PressKey("{TAB}");
        //        System.Threading.Thread.Sleep(60);
        //        CD.Keyboard.ReleaseKey("{TAB}");
        //        //CD.Keyboard.SendKeys("{TAB}");


        //        {
        //            wE = CD.FindElement(By.TagName("OK"));
        //            retVal = wE.Text;
        //        }

        //        wait.Until(d => d.PageSource.Contains("res-tts"));
        //        wait.Until(d => d.FindElement(By.ClassName("res-tts")).Enabled);
        //        wE = CD.FindElement(By.ClassName("res-tts"));
        //        wE.Click();

        //        wait.Until(d => d.FindElement(By.ClassName("res-tts")).GetAttribute("aria-pressed") == "false");

        //        _tries = 0;
        //    }
        //    catch (Exception e)
        //    {

        //        if (e.Message.StartsWith("chrome not") && _tries < 3)
        //        {
        //            CD = null;
        //            CD = new ChromeDriver(_cDs);
        //            //retVal = Translate(languageTranslateFrom, languageTo, phraseToTranslate, getStringOrSetAsClipboard);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Error:" + e.Message);
        //        }
        //    }

        //    return retVal;
        //}
    }
}
