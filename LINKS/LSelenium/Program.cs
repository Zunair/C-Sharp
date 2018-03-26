using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chrome
{
    /// <summary>
    /// This class is only used when it's called from command prompt or run directly.
    /// It will not be used when the exe is loaded as a plugin.
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            //[LSelenium.sChrome.SpeakTranslation("en","es","what time is it?")];
            //[LSelenium.sChrome.GetTranslation("en","es","what time is it?","Get")]
            Console.Title = "LINKS - Selenium";

            var parsedArgs = args
                .Select(s => s.Split(new[] { '=' }))
                .ToDictionary(s => s[0], s => s[1]);

            //Browse.GotoAsApp("http://zunair.rocks", "false");

            if (args.Length == 3)
            {
                GoogleTranslate.Speak(parsedArgs["From"], parsedArgs["To"], parsedArgs["Phrase"]);
                //GoogleTranslate.Speak(parsedArgs["From"], parsedArgs["To"], "this is a test");

                Chrome.Close();
            }
            else if (args.Length == 4)
            {
                GoogleTranslate.GetTranslation(parsedArgs["From"], parsedArgs["To"], parsedArgs["Phrase"]);
                Chrome.Close();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid syntax.");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Usage: Selenium From=\"{LanguageFrom}\" To=\"{LanguageTo}\" Phrase=\"PhraseToTranslate\"");
                Console.WriteLine("Sample: Selenium From=\"en\" To=\"es\" Phrase=\"What time is it?\"");
                Console.WriteLine("Description: Translates specified phrase from {LanguageFrom} to {LanguageTo} and speaks.");
                //Console.WriteLine();
                //Console.WriteLine("Usage: Selenium \"{LanguageFrom}\" \"{LanguageTo}\" \"PhraseToTranslate\" \"Get\"");
                //Console.WriteLine("Sample: Selenium \"en\" \"es\" \"What time is it?\" \"Set\"");
                //Console.WriteLine("Description: Sets cliboard with translated text. \r\n  Using Get as last parameter returns the translation in console.");
            }
        }
    }    
}
