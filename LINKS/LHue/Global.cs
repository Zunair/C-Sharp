using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHue
{
    class Global
    {
        public static SwitchView LINKS_Viseme;

        public Global()
        {

        }

        public class AppVariables
        {
            public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            public static string CurrentPluginDirectory = AppData + "\\LINKS\\Customization\\Plugins\\" + typeof(Configuration).Assembly.GetName().Name + "\\";
        }
    }
}
