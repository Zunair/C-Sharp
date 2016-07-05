using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTTS7Lib;
using System.Text.RegularExpressions;

namespace LLoquendo
{
    public class Speech
    {
        static _DLTTS7 loquendo = null;

        //[LLoquendo.Speech.Speak("\\item=Breath 1, 2, 3, 4.","Kate","100")]
        public static string Speak(string phrase, string volume, string rate, string voice)
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
                loquendo.Speed = int.Parse(rate);

                loquendo.SetAttribute("TextFormat", "SSML");
                loquendo.Device = 0;

                phrase = phrase.Replace("\\\\", "\\");                
                loquendo.Read(phrase);
            }
            catch
            {
                if (phrase.Contains("\\"))
                {
                    phrase = GetParsedPhrase(phrase, PhraseFor.Other);
                }
                    
                retVal = phrase;
            }

            return retVal;
        }

        enum PhraseFor
        {
            Loquendo, Other
        }

        static void Main(string[] args)
        {
            //string t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Loquendo);
            //t = GetParsedPhrase("\\hello_01 what time is it", PhraseFor.Other);
        }

        static string GetParsedPhrase(string phrase, PhraseFor phraseFor)
        {
            string retVal = "";
            string pattern = @"(\\\w+)";

            // Instantiate the regular expression object.
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);

            // Replace \word with string.empty or \item=word
            retVal = regEx.Replace(phrase, delegate (Match match) {
                if (match.Groups[1].Value == "") return match.Value;
                else return phraseFor == PhraseFor.Other ? string.Empty : "\\item=" + match.Value.Substring(1);
            });
            
            return retVal.Trim();
        }
    }
}
