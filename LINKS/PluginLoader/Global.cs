using System;

namespace LPluginReg
{
    class Global
    {
        public class AppVariables
        {
            public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            public static string CurrentPluginDirectory = AppData + "\\LINKS\\Customization\\Plugins\\"; // + typeof(Configuration).Assembly.GetName().Name + "\\";
            public static string XMLDirectory = AppData + "\\LINKS\\Customization\\XML\\"; // + typeof(Configuration).Assembly.GetName().Name + "\\";
        }
    }
}
