using jarvisWPF;
using System.Speech.Recognition;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.QualityTools.Testing.Fakes;
using System.Threading;

/// Add reference to jarvisWPF from %APPDATA%\LINKS\Customization\Plugins\jarvisWPF.exe

/// Make sure the functions called from LINKS are public static string.
/// params can only be string, convert them inside the method.
/// Methods are not case sensitive.
/// Same method names are not compatiable in public v2.5.6.2, in future they will be.
namespace LSamplePlugin
{
    
    public class MyCustomClass
    {
        /// Uncomment the follwing if needed.
        #region Other Methods
        //static string CurrentPluginDirectory;

        ///// <summary>
        ///// This method is run on load, so all the needed events are attached automatically.
        ///// We can initialize other objects here since this method is run when LINKS is started.
        ///// </summary>
        //public static void OnLoad()
        //{
        //    // Attach events when LINKS recognizes a phrase
        //    PublicClass.Recognizer.SpeechRecognized += _SpeechRecognized;

        //    // Create plugin directory
        //    CurrentPluginDirectory = PublicClass.PluginsPath + typeof(MyCustomClass).Assembly.GetName().Name + "\\";
        //    System.IO.Directory.CreateDirectory(CurrentPluginDirectory);
        //}

        ///// <summary>
        ///// This method is called when a speech is recognized.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private static async void _SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    string RecognizedText = e.Result.Text;
        //    float RecognizedConfidence = e.Result.Confidence;
        //    DateTime dt = DateTime.Now;
        //    string filePath = CurrentPluginDirectory + "LINKS_Heard.txt";

        //    using (StreamWriter ErrorStreamWriter = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write)))
        //    {
        //        string StrToWrite = string.Format("{2}\tRecognized@{0}: \t{1}", new object[] { RecognizedConfidence, RecognizedText, dt });
        //        await ErrorStreamWriter.WriteLineAsync(StrToWrite);
        //    }
        //}
        #endregion

        /// <summary>
        /// Returns a string to LINKS in the response, action or web request.
        /// This method can be called from LINKS like this [LSamplePlugin.MyCustomClass.Test1]
        /// In version after v2.6.2.3, this can also be used[LSamplePlugin.MyCustomClass.Test1()]
        /// </summary>
        /// <returns>Hello World</returns>        
        public static string Test1()
        {
            return "Hello World";
        }

        /// <summary>
        /// Returns a string to LINKS in the response, action or web request.
        /// This method can be called from LINKS like this [LSamplePlugin.MyCustomClass.Test2("Hello World")]
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>phrase variable</returns>
        public static string Test2(string phrase)
        {
            return phrase;
        }
        
        /// <summary>
        /// Returns a string to LINKS in the response, action or web request.
        /// This method can be called from LINKS like this [LSamplePlugin.MyCustomClass.Test3("1","2")]
        /// </summary>
        /// <param name="x">number</param>
        /// <param name="y">number</param>
        /// <returns>x plus y equals to total</returns>
        public static string Test3(string x, string y)
        {
            return string.Format("{0} plus {1} equals to {2}", int.Parse(x), int.Parse(y), int.Parse(x) + int.Parse(y));
        }

        private static MyCustomWindow myCustomWindow;
        public static string Test4()
        {
            string retVal = string.Empty;

            myCustomWindow = new MyCustomWindow();           

            return retVal;
        }

        public static string Test_ChangeLabelInMyCustomWindow(string labelText)
        {
            string retVal = string.Empty;

            myCustomWindow.FormTest_ChangeLabel(labelText);

            return retVal;
        }

        public static string Test_FormClose()
        {
            string retVal = string.Empty;

            myCustomWindow.FormTest_Close();

            return retVal;
        }
    }

    public class MyCustomWindow
    {
        public MyCustomWindow()
        {
            Thread t = new Thread(ThreadProc);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }



        Form_Test formTest;
        private void ThreadProc()
        {
            formTest = new Form_Test();
            formTest.Show();
            formTest.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.Run();
        }

        public void FormTest_ChangeLabel(string str)
        {
            formTest.test_label.Invoke((Action)(() =>
            {
                formTest.test_label.Text = str;
            }));
        }

        public void FormTest_Close()
        {
            formTest.test_label.Invoke((Action)(() =>
            {
                formTest.Close();
            }));
        }
    }
}
