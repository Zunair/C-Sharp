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
            LLoquendo.Speech.Speak("\\item=Breath 1, 2, 3, 4.", "Kate", "100");

            LSamplePluginWPF.MyCustomClass.Test_InitWPFWindow();
        }
    }
}
