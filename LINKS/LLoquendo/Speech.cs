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
        public static void Speak(string phrase, string voice, string volume)
        {
            if (loquendo == null)
            {
                loquendo = new LTTS7();
            }

            loquendo.Voice = voice;
            loquendo.Volume = int.Parse(volume);

            loquendo.SetAttribute("TextFormat", "SSML");
            loquendo.Device = 0;
            loquendo.Read(phrase);            
        }
    }
}
