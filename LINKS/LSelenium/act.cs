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

            if (sChromeInstance == null)
            {
                sChromeInstance = new SeleniumChrome();
            }

            sChromeInstance.SpeakTranslation(languageTranslateFrom, languageTranslateTo, phraseToTranslate, "");


            return retVal;
        }

        public static string GetTranslation(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate, string GetSet)
        {
            string retVal = "";

            if (sChromeInstance == null)
            {
                sChromeInstance = new SeleniumChrome();
            }

            retVal = sChromeInstance.SpeakTranslation(languageTranslateFrom, languageTranslateTo, phraseToTranslate, GetSet);
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

        public static string Close()
        {
            string retVal = "";

            sChromeInstance.cD.Quit();
            sChromeInstance.cD = null;


            return retVal;
        }
    }

    class SeleniumChrome
    {

        ChromeDriverService _cDs;
        ChromeDriver _cD;
        int _tries = 0;

        public ChromeDriver cD
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

        public SeleniumChrome()
        {
            _cDs = ChromeDriverService.CreateDefaultService();
            _cDs.HideCommandPromptWindow = true;
            _cD = new ChromeDriver(_cDs);
        }
        
        public string SpeakTranslation(string languageTranslateFrom, string languageTo , string phraseToTranslate, string getStringOrSetAsClipboard)
        {
            string retVal = "";
            Uri uri = new Uri("https://translate.google.cn/m/translate?hl="  + languageTranslateFrom + "#auto/" + languageTo +  "/" + phraseToTranslate);
            _tries++;
            
            try
            {   
                _cD.Navigate().GoToUrl(uri);
                
                var wait = new WebDriverWait(_cD, TimeSpan.FromSeconds(60));
                wait.Until(d => d.Title == "Google Translate");
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

                wait.Until(d => d.FindElement(By.ClassName("res-tts")).GetAttribute("aria-pressed")=="false");

                _tries = 0;
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("chrome not") && _tries < 3)
                {
                    _cD = null;
                    _cD = new ChromeDriver(_cDs);
                    retVal = SpeakTranslation(languageTranslateFrom, languageTo, phraseToTranslate, getStringOrSetAsClipboard);
                }
                else
                {
                    Console.WriteLine("Error:" + e.Message);
                }
            }

            return retVal;
        }
    }
}
