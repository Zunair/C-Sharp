using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Chrome
{
    public class GoogleTranslate
    {
        public static string Speak(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate)
        {
            string retVal = "";
            
            Chrome.SetChromeInstance();

            Chrome.Instance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, "");

            return retVal;
        }

        public static string GetTranslation(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate)
        {
            string retVal = "";

            Chrome.SetChromeInstance();

            retVal = Chrome.Instance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, "Get");

            if (retVal != null)
            {
                Console.WriteLine(retVal);
            }

            return retVal;
        }

        public static string SetTranslationAsClipboard(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate)
        {
            string retVal = string.Empty;

            Chrome.SetChromeInstance();

            retVal = Chrome.Instance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, "Set");

            if (retVal != null && retVal != string.Empty)
            {
                System.Windows.Forms.Clipboard.SetText(retVal);
            }
            retVal = string.Empty;
            
            return retVal;
        }

        //public static string GetTranslation(string languageTranslateFrom, string languageTranslateTo, string phraseToTranslate, string GetSet)
        //{
        //    string retVal = "";

        //    Chrome.SetChromeInstance();

        //    retVal = Chrome.Instance.Translate(languageTranslateFrom, languageTranslateTo, phraseToTranslate, GetSet);
        //    if (GetSet.Equals("set", StringComparison.OrdinalIgnoreCase))
        //    {
        //        if (retVal != null && retVal != string.Empty)
        //        {
        //            System.Windows.Forms.Clipboard.SetText(retVal);
        //        }
        //        retVal = "";
        //    }
        //    else if (GetSet.Equals("get", StringComparison.OrdinalIgnoreCase))
        //    {
        //        if (retVal != null)
        //        {
        //            Console.WriteLine(retVal);
        //        }
        //    }

        //    return retVal;
        //}





    }

}
