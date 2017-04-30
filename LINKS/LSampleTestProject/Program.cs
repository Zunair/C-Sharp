using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSampleTestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //LWMPlayer.Class1.Test();

            //LLoquendo.Speech.Speak("\\item=Breath 1, 2, 3, 4.", "100", "100", "Kate");
            LLoquendo.Speech.Speak("Volume at 50%", "50", "100", "Kate");
            LLoquendo.Speech.Speak("\\item=Breath 5, 6, 7, 8.", "100", "50", "Kate");
            Console.ReadKey();
            SpeechEmulator.Window.Open();
        }
    }
}
