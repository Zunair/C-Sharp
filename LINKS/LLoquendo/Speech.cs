using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTTS7Lib;

namespace LLoquendo
{
    public class Speech
    {
        static _DLTTS7 loquendo = null;

        //[LLoquendo.Speech.Speak("\\item=Breath 1, 2, 3, 4.","Kate","100")]
        public static string Speak(string phrase, string voice, string volume)
        {
            string retVal = string.Empty;

            try
            {
                //System.Diagnostics.Debugger.Break();

                if (loquendo == null)
                {
                    loquendo = new LTTS7();
                }

                loquendo.Voice = voice;
                loquendo.Volume = int.Parse(volume);

                loquendo.SetAttribute("TextFormat", "SSML");
                loquendo.Device = 0;

                phrase = phrase.Replace("\\\\", "\\");
                loquendo.Read(phrase);
            }
            catch
            {
                retVal = voice;
            }

            return retVal;
        }
    }
}
