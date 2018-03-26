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
    public class Click
    {
        public static string ElementByCssSelector(string cssSelector)
        {
            return Chrome.Instance.ClickElementByCssSelector(cssSelector);

        }
       
        public static string ElementByClass(string singleClassName)
        {
            return Chrome.Instance.ClickElementByClass(singleClassName);
        }

        public static string ElementByID(string elementID)
        {
            return Chrome.Instance.ClickElementByID(elementID);
        }

        public static string ElementByXPath(string xPath)
        {
            return Chrome.Instance.ClickElementByXPath(xPath);
        }

    }  
}
