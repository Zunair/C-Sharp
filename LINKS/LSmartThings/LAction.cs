using System;
using System.Diagnostics;
using uAuth2;

namespace LSmartThings
{
    class LAction
    {
        private static EndPoints stDevices;
        private static string _appdata = null;

        private static void InitEndPoints(string AppName)
        {
            if (stDevices == null)
            {
                stDevices = new EndPoints(It.InitAuthWindowAsync(AppName));
                _appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        //[LSmartThings.LAction.Switch("{{a}},{{st},"SmartThings_MVC_LINKS")]
        public static string Switch(string DeviceState, string DeviceName, string AppName)
        {
            string retVal = "";

            //Debugger.Break();

            InitEndPoints(AppName);
            retVal = stDevices.ChangeDevState(DeviceName, DeviceState);


            return retVal;
        }

        public static string MakeWordList(string AppName)
        {
            return MakeWordList(AppName, true);
        } 

        internal static string MakeWordList(string AppName, bool overWrite)
        {
            string retVal = "";
            InitEndPoints(AppName);

            if (_appdata != null)
            {
                string switches_WordList_path = _appdata + @"\LINKS\Data\WordList\test_SmartThings.txt";
                string switchState_wordList_path = _appdata + @"\LINKS\Data\WordList\test_SwitchState.txt";

                // Delete wordlist if called to make wordlist manually
                if (overWrite && System.IO.File.Exists(switches_WordList_path))
                {
                    System.IO.File.Delete(switches_WordList_path);
                }

                // Only create the word list if it does not exist
                if (!System.IO.File.Exists(switches_WordList_path))
                {
                    retVal = "Grammar\tDeviceName\r\n";
                    foreach (EndPoints.SmartThingsDevice d in stDevices.StDevices)
                    {
                        if (d.state != null)
                        {
                            retVal += string.Format("{0}\t{0}\r\n", d.name);
                        }
                    }

                    System.IO.File.WriteAllText(switches_WordList_path, retVal);

                    retVal = "Grammar\tState\r\n" +
                             "on\ton\r\n" +
                             "off\toff\r\n" ;
                    System.IO.File.WriteAllText(switchState_wordList_path, retVal);

                    if (overWrite)
                    {
                        retVal = "Completed;Word lists re-generated;Word lists re-created";
                    }
                    else
                    {
                        retVal = "";
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// This method is called automatically from LINKS on start
        /// </summary>
        public static void OnLoad()
        {
            MakeWordList("SmartThings_MVC_LINKS", false);
        }
    }
}
