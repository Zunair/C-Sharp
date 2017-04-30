using System;
using System.Collections.Generic;
using System.Linq;
using LTTS7Lib;
using System.Text.RegularExpressions;

namespace LLoquendo
{

    public class Speech
    {
        static List<LoqSynth> SpeechSynth = new List<LoqSynth>();
        static string lastVoice = "";
        static string lastVolume = "";
        static string lastRate = "";
        static string lastPhrase = "";
        //[LLoquendo.Speech.Speak("phrase","volume","rate","voice")]
        public static string Speak(string phrase, string volume, string rate, string voice)
        {
            //System.Diagnostics.Debugger.Break();

            try
            {
                if (SpeechSynth.Count == 0)
                {
                    LoqSynth ls = new LoqSynth(voice);
                    if (ls.Voice != "")
                    {
                        ls.LoquendoX.EndOfSpeech += new _DLTTS7Events_EndOfSpeechEventHandler(LoquendoX_EndOfSpeech);                        
                        ls.LoquendoX.text += LoquendoX_text;
                        SpeechSynth.Add(ls);
                    }
                    else
                    {
                        return phrase;
                    }
                }

                
                //#if LINKSEXISTS
                try
                {
                    if (jarvisWPF.PublicClass.SpeechSynth != null)
                    {
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();

                        while (jarvisWPF.PublicClass.SpeechSynth.SpeechPrompts.Count > 0)
                        {
                            System.Threading.Thread.Sleep(300);
                            if (stopWatch.Elapsed.Seconds > 30)
                            {
                                jarvisWPF.PublicClass.SpeechSynth.SpeechPrompts.Clear();
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Can't wait for LINKS to finish talking...");
                    //System.Diagnostics.Debugger.Break();
                }
                //#endif


                lastVoice = voice;
                lastVolume = volume;
                lastRate = rate;
                lastPhrase = phrase;
                

                return SpeechSynth.FirstOrDefault().Speak(phrase, volume, rate, voice);
            }
            catch ( Exception error)
            {
                Console.WriteLine(error.Message);
                return phrase;
            }

            //if (SpeechSynth.Find(ls => ls.Voice == voice) == null)
            //{
            //    SpeechSynth.Add(new LoqSynth(voice));
            //}

            //foreach (LoqSynth ls in SpeechSynth.FindAll(ls => ls.Voice != voice))
            //{
            //    ls.Speak(phrase, "0", rate, "");
            //    Console.WriteLine("foreach voice: " + ls.Voice);
            //}

            //Console.WriteLine("return voice: " + voice);
            //return SpeechSynth.First(ls => ls.Voice == voice).Speak(phrase, volume, rate, voice);
        }

        public static void LoquendoX_text(string text)
        {
            throw new NotImplementedException();
        }

        public static void LoquendoX_EndOfSpeech()
        {
            throw new NotImplementedException();
        }

        //[LLoquendo.Speech.Stop()]
        public static string Stop()
        {
            if (jarvisWPF.PublicClass.SpeechSynth.SpeechPrompts.Count > 0)
            {
                jarvisWPF.PublicClass.SpeechSynth.StopSpeech();
                jarvisWPF.PublicClass.SpeechSynth.SpeechPrompts.Clear();
            }
            SpeechSynth.FirstOrDefault().LoquendoX.Stop();
            return "";
        }

        public static string RepeatLastPhrase()
        {
            if (lastPhrase.Contains("=English"))
            {
                lastPhrase = lastPhrase.Substring((@"\language=English ".Length + 2));
            }
            SpeechSynth.FirstOrDefault().Speak(lastPhrase, lastVolume, lastRate, lastVoice);
            return "";
        }

        public static string Stop(string phrase)
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Stop();
            System.Threading.Thread.Sleep(500);

            if (lastPhrase.Contains("EnglishUs"))
            {
                phrase = @"\language=EnglishUs " + phrase;
            }
            else if (lastPhrase.Contains("EnglishGb"))
            {
                phrase = @"\language=EnglishGb " + phrase;
            }

            SpeechSynth.FirstOrDefault().Speak(phrase, lastVolume, lastRate, lastVoice);
            return "";
        }

        public static string Pause()
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Pause();
            return "";
        }

        public static string Resume()
        {
            SpeechSynth.FirstOrDefault().LoquendoX.Resume();
            return "";
        }
        
    }


    public class LoqSynth
    {
        LTTS7 loquendoX = null;
        string _voice = "";
        private List<string> voices;

        internal LTTS7 LoquendoX
        {
            get
            {
                return loquendoX;
            }

            set
            {
                loquendoX = value;
            }
        }

        internal string Voice
        {
            get
            {
                return _voice;
            }

            set
            {
                _voice = value;
            }
        }

        public List<string> Voices
        {
            get
            {
                return voices;
            }

            set
            {
                voices = value;
            }
        }

        public LoqSynth(string voice)
        {
            LoquendoX = new LTTS7();
            GatherVoices();

            if (IsLoquendoVoice(voice))
            {

                Voice = voice;

                ((_DLTTS7)loquendoX).Voice = Voice;

                LoquendoX.SetAttribute("TextFormat", "SSML");
                LoquendoX.AudioChannels = "Stereo";
                LoquendoX.Device = 0;
                LoquendoX.Volume = 50;
                try
                {
                    LoquendoX.Volume = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceVolume;
                }
                catch
                {
                    Console.WriteLine("Can't get volume from LINKS...");
                    //System.Diagnostics.Debugger.Break();
                }
                //LoquendoX.EndOfSpeech += new _DLTTS7Events_EndOfSpeechEventHandler(Loquendo_EndOfSpeech);
            }
        }

        private void GatherVoices()
        {
            Voices = new List<string>();

            string enumVoice = LoquendoX.EnumFirstVoice("");
            while (enumVoice != "")
            {
                Voices.Add(enumVoice);
                enumVoice = LoquendoX.EnumNextVoice();
                //string lan = LoquendoX.GuessLanguage(enumVoice);
                //lan = LoquendoX.GuessFileLanguage(enumVoice);
            }
            
        }

        private bool IsLoquendoVoice(string voice)
        {
            if (Voices == null)
            {
                Voices = new List<string>();
            }
            bool retVal = false;

            string enumVoice = LoquendoX.EnumFirstVoice("");
            while (enumVoice != "")
            {
                if (enumVoice.ToLower() == voice.ToLower())
                {
                    retVal = true;
                    break;

                }
                
                enumVoice = LoquendoX.EnumNextVoice();
            }
            return retVal;
        }

        internal string Speak(string phrase, string volume, string rate, string voice)
        {
            string retVal = string.Empty;
            //System.Diagnostics.Debugger.Break();

            try
            {
                //System.Diagnostics.Debugger.Break();
                if (rate == string.Empty)
                {
                    rate = "50";
                }
                
                if (voice != string.Empty && Voices.Find(v => v == voice) == null)
                {
                    if (phrase.Contains("\\"))
                    {
                        phrase = GetParsedPhrase(phrase, PhraseFor.Other);
                    }
                    retVal = phrase;
                }
                else
                {
                    
                    if (volume == "")
                    {
                        //LoquendoX.Volume = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceVolume;
//#if LINKSEXISTS
                        try
                        {
                            volume = "20";
                        }
                        catch
                        {
                            Console.WriteLine("Can't get volume from LINKS...");
                        }
//#endif
                    }
                    else
                    {
                        //LoquendoX.Volume = int.Parse(volume);
                    }
                    //LoquendoX.Speed = int.Parse(rate);

                    phrase = phrase.Replace("\\\\", "\\");

                    //if (voice != "USE FIRST VOICE")
                    
                    if (phrase.StartsWith(@"\language", StringComparison.OrdinalIgnoreCase))
                    {
                        LoquendoX.Read(string.Format(@"\voice={0} \volume={2} \speed={3} {1}", voice, phrase, volume, rate));
                    }
                    else
                    {
                        string lang = null;
//#if LINKSEXISTS
                        try
                        {
                            if (jarvisWPF.PublicClass.SpeechSynth != null)
                            {
                                try
                                {
                                    lang = jarvisWPF.PublicClass.SpeechSynth.VoiceConfigs.Get(Voice).VoiceLanguage;
                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Can't get language from LINKS...");
                            //System.Diagnostics.Debugger.Break();
                        }
                        //#endif


                        LoquendoX.ReadFile("c:\\temp\\test.txt");
                        LoquendoX.Read(string.Format(@"\language={4} \voice={0} \volume={2} \speed={3} {1}", voice, phrase, volume, rate, lang == null ? "": lang));
                    }
                }
                //Task.WaitAll();
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



        public void LoquendoX_EndOfSpeech()
        {
            throw new NotImplementedException();
        }

        public void Loquendo_StartOfSpeech()
        {
            //throw new NotImplementedException();
        }

        private void Loquendo_EndOfSpeech()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string log = AppData + "\\LINKS\\Customization\\Plugins\\LLoquendo\\";
            System.IO.Directory.CreateDirectory(log);
            log += "log.txt";
            System.IO.File.WriteAllText(log, "Speech Completed");
            System.Threading.Thread.Sleep(100);
        }

        enum PhraseFor
        {
            Loquendo, Other
        }

        private string GetParsedPhrase(string phrase, PhraseFor phraseFor)
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


