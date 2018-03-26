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
    class Browse
    {
        public static string Goto(string url, string fullScreen)
        {
            if (url == string.Empty) url = "http://zunair.rocks";

            Chrome.SetChromeInstance(bool.Parse(fullScreen), false);

            return Chrome.Instance.Goto(url);
        }

        public static string GotoAsApp(string url, string fullScreen)
        {
            if (url == string.Empty) url = "http://zunair.rocks";

            Chrome.SetChromeInstance(bool.Parse(fullScreen), false, true, url);

            return Chrome.Instance.Goto(url);
        }   
    }
    
}
