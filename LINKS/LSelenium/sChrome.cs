using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace LSelenium
{
    class sChrome
    {
        static SeleniumChrome sChromeInstance;
        
        public static string SpeakTranslation(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate)
        {
            string retVal = "";
            
            SetChromeInstance();

            sChromeInstance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, "");

            return retVal;
        }

        private static void SetChromeInstance(bool fullScreen = false, bool headless = true)
        {
            if (sChromeInstance == null)
            {
                sChromeInstance = new SeleniumChrome(fullScreen);
            }
        }

        public static string GetTranslation(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate, string GetSet)
        {
            string retVal = "";

            SetChromeInstance();

            retVal = sChromeInstance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, GetSet);
            if (GetSet.Equals("set", StringComparison.OrdinalIgnoreCase))
            {
                if (retVal != null && retVal != string.Empty)
                {
                    System.Windows.Forms.Clipboard.SetText(retVal);
                }
                retVal = "";
            }
            else if (GetSet.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                if (retVal != null)
                {
                    Console.WriteLine(retVal);
                }
            }

            return retVal;
        }
        
        public static string ClickElementByCssSelector(string cssSelector)
        {
            return sChromeInstance.ClickElementByCssSelector(cssSelector);

        }
        public static string GetElementValueByCssSelector(string cssSelector)
        {
            return sChromeInstance.GetElementValueByCssSelector(cssSelector);
        }

        public static string GetElementValueByCssSelectorAndAttribute(string cssSelector, string attributeName)
        {
            return sChromeInstance.GetElementValueByCssSelectorAndAttribute(cssSelector, attributeName);
        }

        public static string SetElementValueByCssSelector(string cssSelector, string sendKeys)
        {
            return sChromeInstance.SetElementValueByCssSelector(cssSelector, sendKeys);
        }

        public static string WaitForCssSelectorWithAttibuteAndValue(string cssSelector, string attributeName, string timeout)
        {
            return sChromeInstance.WaitForCssSelectorWithAttibuteAndValue(cssSelector, attributeName, int.Parse(timeout));
        }

        public static string ClickElementByClass(string singleClassName)
        {
            return sChromeInstance.ClickElementByClass(singleClassName);
        }

        public static string ClickElementByID(string elementID)
        {
            return sChromeInstance.ClickElementByID(elementID);
        }

        public static string Goto(string url, string fullScreen)
        {
            if (url == string.Empty) url = "http://zunair.rocks";

            SetChromeInstance(bool.Parse(fullScreen), false);

            return sChromeInstance.Goto(url);
        }

        public static string SetElementTextByClass(string singleClassName, string sendKeys)
        {
            return sChromeInstance.SetElementTextByClass(singleClassName, sendKeys);
        }

        public static string SetElementTextByID(string elementID, string sendKeys)
        {
            return sChromeInstance.SetElementTextByID(elementID, sendKeys);
        }

        public static string WaitForClassWithAttibuteAndValue(string singleClassName, string attributeName, string attributeValue, string timeout)
        {
            return sChromeInstance.WaitForClassWithAttibuteAndValue(singleClassName, attributeName, attributeValue, int.Parse(timeout));
        }
        
        public static string WaitForURLContains(string caseSensitiveString, string timeout)
        {
            return sChromeInstance.WaitForURLContains(caseSensitiveString , int.Parse(timeout));
        }

        public static string WaitForSourceContains(string sourceString, string timeout)
        {
            return sChromeInstance.WaitForSourceContains(sourceString, int.Parse(timeout));
        }

        public static string WaitForTitleContains(string titleString, string timeout)
        {
            return sChromeInstance.WaitForTitleContains(titleString, int.Parse(timeout));
        }

        public static string WaitTillElementByClassIsEnabled(string singleClassName, string timeout)
        {
            return sChromeInstance.WaitTillElementByClassIsEnabled(singleClassName, int.Parse(timeout));
        }

        public static string WaitTillElementByIDIsEnabled(string elementID, string timeout)
        {
            return sChromeInstance.WaitTillElementByIDIsEnabled(elementID, int.Parse(timeout));
        }

        public static string ClickElementByXPath(string xPath)
        {
            return sChromeInstance.ClickElementByXPath(xPath);
        }

        public static string SetElementValueByXPath(string xPath, string sendKeys)
        {
            return sChromeInstance.SetElementValueByXPath(xPath, sendKeys);
        }

        public static string GetScreenshot(string filePath)
        {
            return sChromeInstance.GetScreenshot(filePath);
        }

        public static string Close()
        {
            string retVal = "";

            if (sChromeInstance != null)
            {
                if (sChromeInstance.CD != null)
                {
                    try
                    {
                        sChromeInstance.CD.Close();
                        sChromeInstance.CD.Quit();
                        sChromeInstance.CD = null;
                    }
                    catch {}
                }
                sChromeInstance = null;
            }

            return retVal;
        }

        public static void OnDispose()
        {
            Close();
        }
        //public static string Hangouts(string command, string phoneNum = "")
        //{
        //    if (sChromeInstance == null)
        //    {
        //        sChromeInstance = new SeleniumChrome();
        //    }

        //    string retVal = "";

        //    sChromeInstance.Hangouts("call", phoneNum);

        //    return retVal;
        //}

    }

    class SeleniumChrome
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

        string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string ChromeDir = string.Empty;

        public SeleniumChrome(bool fullScreen = false, bool headless = true)
        {
            _cDs = ChromeDriverService.CreateDefaultService();
            _cDs.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();

            ChromeDir = System.IO.Path.Combine(AppData, "LINKS\\Customization\\Plugins\\Google\\Chrome\\");
            string ChromeProfile = System.IO.Path.Combine(ChromeDir, "profile");
            System.IO.Directory.CreateDirectory(ChromeDir);
            options.AddArguments("user-data-dir=" + ChromeProfile);
            if (fullScreen) options.AddArguments("--start-fullscreen");
            if (headless) options.AddArguments("--headless");

            _cD = new ChromeDriver(_cDs, options);
        }

        public string Translate(string languageTranslateFrom, string languageTo, string phraseToTranslate, string getStringOrSetAsClipboard)
        {
            string retVal = "";
            Uri uri = new Uri("https://translate.google.com/m/translate?hl=" + languageTranslateFrom + "#auto/" + languageTo + "/" + phraseToTranslate);
            _tries++;

            try
            {
                _cD.Navigate().GoToUrl(uri);

                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(40));
                wait.Until(d => d.Title.Contains("Google"));
                _cD.Manage();

                IWebElement wE;
                wait.Until(d => d.PageSource.Contains("translation"));

                if (getStringOrSetAsClipboard != string.Empty)
                {
                    wE = _cD.FindElement(By.ClassName("translation"));
                    retVal = wE.Text;
                }

                wait.Until(d => d.PageSource.Contains("res-tts"));
                wait.Until(d => d.FindElement(By.ClassName("res-tts")).Enabled);
                wE = _cD.FindElement(By.ClassName("res-tts"));
                wE.Click();

                wait.Until(d => d.FindElement(By.ClassName("res-tts")).GetAttribute("aria-pressed") == "false");

                _tries = 0;
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("chrome not") && _tries < 3)
                {
                    _cD = null;
                    _cD = new ChromeDriver(_cDs);
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

            var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));

            wait.Until(d => d.Title.Contains(titleString));
            _cD.Manage();

            return retVal;
        }

        public string WaitForCssSelectorWithAttibuteAndValue(string cssSelector, string attributeName, int timeout)
        {
            string retVal = string.Empty;

            var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));
            wait.Until(d => d.FindElement(By.CssSelector(cssSelector)).GetAttribute(attributeName));
            _cD.Manage();

            return retVal;
        }

        public string WaitForSourceContains(string sourceString, int timeout)
        {
            string retVal = string.Empty;

            var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));

            wait.Until(d => d.PageSource.Contains(sourceString));
            _cD.Manage();

            return retVal;
        }

        public string WaitForClassWithAttibuteAndValue(string singleClassName, string attributeName, string attributeValue, int timeout)
        {
            string retVal = string.Empty;

            try
            {
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));

                wait.Until(d => d.FindElement(By.ClassName(singleClassName)).GetAttribute(attributeName) == attributeValue);
                _cD.Manage();
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
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));
                wait.Until(d => d.Url.Contains(caseSensitiveString));
                _cD.Manage();
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

                wE = _cD.FindElementByCssSelector(cssSelector);
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

                wE = _cD.FindElement(By.ClassName(singleClassName));
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

                wE = _cD.FindElement(By.Id(elementID));
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

                wE = _cD.FindElementByXPath(xPath);
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

                wE = _cD.FindElementByXPath(xPath);
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

                wE = _cD.FindElementByCssSelector(cssSelector);
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

                wE = _cD.FindElementByCssSelector(cssSelector);
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
                retVal = _cD.FindElementByCssSelector(cssSelector).GetAttribute(attribute);
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
                string screenshotsDir = System.IO.Path.Combine(ChromeDir, "Screenshots");
                System.IO.Directory.CreateDirectory(screenshotsDir);
                filePath = System.IO.Path.Combine(screenshotsDir, filePath);
            }

            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            try
            {
                _cD.GetScreenshot().SaveAsFile(filePath);
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
                
                retVal = _cD.PageSource;
                
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
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));
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
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(timeout));
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

                wE = _cD.FindElement(By.ClassName(singleClassName));
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

                wE = _cD.FindElement(By.Id(elementID));
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
                _cD.Navigate().GoToUrl(uri);

                _tries = 0;
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("chrome not") && _tries < 3)
                {
                    _cD = null;
                    _cD = new ChromeDriver(_cDs);
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
        //        _cD.Navigate().GoToUrl(uri);

        //        var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(40));
        //        wait.Until(d => d.Title.Contains("Google"));
        //        _cD.Manage();

        //        IWebElement wE;
        //        wait.Until(d => d.PageSource.Contains("Your call is free."));

        //        //_cD.FileDetector;
        //        System.Threading.Thread.Sleep(3000);
        //        _cD.Keyboard.PressKey("{TAB}");
        //        System.Threading.Thread.Sleep(60);
        //        _cD.Keyboard.ReleaseKey("{TAB}");
        //        //_cD.Keyboard.SendKeys("{TAB}");


        //        {
        //            wE = _cD.FindElement(By.TagName("OK"));
        //            retVal = wE.Text;
        //        }

        //        wait.Until(d => d.PageSource.Contains("res-tts"));
        //        wait.Until(d => d.FindElement(By.ClassName("res-tts")).Enabled);
        //        wE = _cD.FindElement(By.ClassName("res-tts"));
        //        wE.Click();

        //        wait.Until(d => d.FindElement(By.ClassName("res-tts")).GetAttribute("aria-pressed") == "false");

        //        _tries = 0;
        //    }
        //    catch (Exception e)
        //    {

        //        if (e.Message.StartsWith("chrome not") && _tries < 3)
        //        {
        //            _cD = null;
        //            _cD = new ChromeDriver(_cDs);
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
