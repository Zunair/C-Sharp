using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LLoquendo
{
    class main
    {
        static void Main(string[] args)
        {
            Console.Title = "LINKS - Loquendo Plugin";

            Console.WriteLine(((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title);
            Console.WriteLine("Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine(((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright + " " + "All Rights Reserved");
            Console.WriteLine("================================================================================");
            Console.WriteLine();
            Console.WriteLine("Syntax:");
            Console.WriteLine("  [LLoquendo.Speech.Speak(\"phrase\",\"volume\",\"rate\",\"voice\")]");
            Console.WriteLine("  [LLoquendo.Speech.Stop(\"phrase to speak after stop speaking\")]");
            Console.WriteLine("  [LLoquendo.Speech.Stop()]");
            Console.WriteLine("  [LLoquendo.Speech.Pause()]");
            Console.WriteLine("  [LLoquendo.Speech.Resume()]");
            Console.WriteLine("  [LLoquendo.Speech.RepeatLastPhrase()]");
            Console.WriteLine();
            Console.WriteLine("Sample action from LINKS:");
            Console.WriteLine("  LLoquendo.Speech.Speak(\"I am Kate.\\voice=Simon I am Simon.\",\"100\",\"50\",\"Kate\")");
            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine();

            try
            {
                Console.WriteLine("Testing Kate's and Simon's voice...");
                LLoquendo.Speech.Speak("I am Kate.\\voice=Simon I am Simon.", "5", "50", "Kate");
                LLoquendo.Speech.Speak("I am Kate.\\voice=Simon I am Simon.", "100", "50", "Kate");
                LLoquendo.Speech.Speak("Simon again.", "100", "10", "Simon");
                Console.WriteLine("Testing Simon's voice...");
                LLoquendo.Speech.Speak("And Kate again.", "100", "50", "");
                Console.WriteLine("Testing Kates's voice...");
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + error.Message);
                Console.WriteLine("Please install Loquendo SDK to use this plugin.");
            }

            Console.WriteLine();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
            //string t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Loquendo);
            //t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Other);
        }
    }
}
