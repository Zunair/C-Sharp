using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHue
{
    public partial class Functions
    {
        static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        enum LightsCommand
        {
            on, off, up, down, full, medium, low
        }

        public static async Task<string> Switch(string DeviceState, string DeviceName, string ConfirmingPhrase, string AlreadyInStatePhrase)
        {
            string retVal = "";

            if (myCustomWindow.wpfTest.SwitchWindows.Exists(s => s.LSwitch.Name == DeviceName))
            {
                var light = myCustomWindow.wpfTest.SwitchWindows.First(s => s.LSwitch.Name == DeviceName);
                //bool stateTo = DeviceState.ToLower() == "on" ? true : false;

                var stateTo = (LightsCommand)Enum.Parse(typeof(LightsCommand), DeviceState.ToLower());
                bool stateAlready = false;

                switch (stateTo)
                {
                    case LightsCommand.on:
                        if (light.LSwitch.State.On)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            await light.SwitchState(true, null, null);
                        }
                        break;

                    case LightsCommand.off:
                        if (light.LSwitch.State.On == false)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            await light.SwitchState(true, null, null);
                        }
                        break;

                    case LightsCommand.low:
                        if (light.LSwitch.State.On && light.LSwitch.State.Brightness == 20)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            await light.SwitchState(true, 20, null);
                        }
                        break;

                    case LightsCommand.medium:
                        if (light.LSwitch.State.On && light.LSwitch.State.Brightness == 50)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            await light.SwitchState(true, 50, null);
                        }
                        break;

                    case LightsCommand.full:
                        if (light.LSwitch.State.On && light.LSwitch.State.Brightness == 100)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            await light.SwitchState(true, 100, null);
                        }
                        break;

                    case LightsCommand.up:
                        if (light.LSwitch.State.On && light.LSwitch.State.Brightness == 100)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            int b = light.LSwitch.State.Brightness + 20;
                            await light.SwitchState(true, b > 100 ? 100: b, null);
                        }
                        break;

                    case LightsCommand.down:
                        if (light.LSwitch.State.On && light.LSwitch.State.Brightness == 10)
                        {
                            stateAlready = true;
                        }
                        else
                        {
                            int b = light.LSwitch.State.Brightness - 20;
                            await light.SwitchState(true, b < 0 ? 10 : b, null);
                        }
                        break;
                }

                if (stateAlready)
                {
                    retVal = AlreadyInStatePhrase;
                }
                else
                {
                    retVal = ConfirmingPhrase;
                }

                retVal = retVal.Replace("{state}", Enum.GetName(typeof(LightsCommand), stateTo));
            }

            return retVal;
        }

        internal static async Task<string> MakeWordList(string AppName, bool overWrite)
        {
            string retVal = "";
            
            if (AppData != null)
            {
                string switches_WordList_path = AppData + @"\LINKS\Data\WordList\test_LHue.txt";
                string switchState_wordList_path = AppData + @"\LINKS\Data\WordList\test_SwitchState.txt";

                // Delete wordlist if called to make wordlist manually
                if (overWrite && System.IO.File.Exists(switches_WordList_path))
                {
                    System.IO.File.Delete(switches_WordList_path);
                }

                // Only create the word list if it does not exist
                if (!System.IO.File.Exists(switches_WordList_path) && myCustomWindow.wpfTest != null)
                {
                    retVal = "Grammar\tDeviceName\r\n";
                    if (myCustomWindow.wpfTest.currentBridge != null)
                    {
                        var bridge = await myCustomWindow.wpfTest.currentBridge.Client.GetBridgeAsync();
                        foreach (var b in bridge.Lights)
                        {
                            if (b.State != null)
                            {
                                retVal += string.Format("{0}\t{0}\r\n", b.Name);
                            }
                        }

                        System.IO.File.WriteAllText(switches_WordList_path, retVal);

                        if (!System.IO.File.Exists(switchState_wordList_path))
                        {
                            retVal = "Grammar\tState\r\n" +
                                 "on\ton\r\n" +
                                 "off\toff\r\n";

                            System.IO.File.WriteAllText(switchState_wordList_path, retVal);
                        }

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
            }
            return retVal;
        }
    }
}
