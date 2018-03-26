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
    public class Get
    {
        public static string ElementValueByCssSelectorAndAttribute(string cssSelector, string attributeName)
        {
            return Chrome.Instance.GetElementValueByCssSelectorAndAttribute(cssSelector, attributeName);
        }

        public static string ElementValueByCssSelector(string cssSelector)
        {
            return Chrome.Instance.GetElementValueByCssSelector(cssSelector);
        }
     
        public static string Screenshot(string filePath)
        {
            return Chrome.Instance.GetScreenshot(filePath);
        }

        public static string PageSource()
        {
            return Chrome.Instance.GetPageSource();
        }
    }
}
