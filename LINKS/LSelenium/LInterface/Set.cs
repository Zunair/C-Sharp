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
    public class Set
    {
        public static string ElementTextByClass(string singleClassName, string sendKeys)
        {
            return Chrome.Instance.SetElementTextByClass(singleClassName, sendKeys);
        }

        public static string ElementTextByID(string elementID, string sendKeys)
        {
            return Chrome.Instance.SetElementTextByID(elementID, sendKeys);
        }

        public static string ElementValueByXPath(string xPath, string sendKeys)
        {
            return Chrome.Instance.SetElementValueByXPath(xPath, sendKeys);
        }

        public static string ElementValueByCssSelector(string cssSelector, string value)
        {
            return Chrome.Instance.SetElementValueByCssSelector(cssSelector, value);
        }

    }
}
