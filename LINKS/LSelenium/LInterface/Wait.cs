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
    public class Wait
    {
        public static string ForCssSelectorWithAttibuteAndValue(string cssSelector, string attributeName, string timeout)
        {
            return Chrome.Instance.WaitForCssSelectorWithAttibuteAndValue(cssSelector, attributeName, int.Parse(timeout));
        }

        public static string ForClassWithAttibuteAndValue(string singleClassName, string attributeName, string attributeValue, string timeout)
        {
            return Chrome.Instance.WaitForClassWithAttibuteAndValue(singleClassName, attributeName, attributeValue, int.Parse(timeout));
        }
        
        public static string ForURLContains(string caseSensitiveString, string timeout)
        {
            return Chrome.Instance.WaitForURLContains(caseSensitiveString , int.Parse(timeout));
        }

        public static string ForSourceContains(string sourceString, string timeout)
        {
            return Chrome.Instance.WaitForSourceContains(sourceString, int.Parse(timeout));
        }

        public static string ForTitleContains(string titleString, string timeout)
        {
            return Chrome.Instance.WaitForTitleContains(titleString, int.Parse(timeout));
        }

        public static string TillElementByClassIsEnabled(string singleClassName, string timeout)
        {
            return Chrome.Instance.WaitTillElementByClassIsEnabled(singleClassName, int.Parse(timeout));
        }

        public static string TillElementByIDIsEnabled(string elementID, string timeout)
        {
            return Chrome.Instance.WaitTillElementByIDIsEnabled(elementID, int.Parse(timeout));
        }

    }
    
}
