using System;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace LHue
{
    public partial class Functions
    {
        public static MyCustomWindow myCustomWindow;

        /// <summary>
        /// Initializes a new instance of WPF window
        /// Can be called from any reponse or web [LHue.MyCustomClass.Test_InitWPFWindow]
        /// </summary>
        /// <returns></returns>
        public static string Test_InitWPFWindow()
        {
            string retVal = string.Empty;

            if (myCustomWindow == null)
            {
                // Creates window instance
                Configuration c = new Configuration();
                c.Load();
                myCustomWindow = new MyCustomWindow();                
            }

            return retVal;
        }

        /// <summary>
        /// Allows user to change Title of the window
        /// Can be called from any reponse or web [LHue.MyCustomClass.Test_ChangeTitleInMyCustomWindow("Hello World")]
        /// </summary>
        /// <param name="labelText"></param>
        /// <returns></returns>
        public static string Test_ChangeTitleInMyCustomWindow(string labelText)
        {
            string retVal = string.Empty;

            myCustomWindow.WPFTest_ChangeTitle(labelText);

            return retVal;
        }

        /// <summary>
        /// Closes the window
        /// Can be called from any reponse or web [LHue.MyCustomClass.Test_FormClose]
        /// </summary>
        /// <returns></returns>
        public static string Test_FormClose()
        {
            string retVal = string.Empty;

            try
            {
                myCustomWindow.FormTest_Close();
            }
            catch(Exception error)
            {
                Debugger.Break();
                Console.WriteLine(error.Message);
            }
            finally
            {
                myCustomWindow = null;
            }


            return retVal;
        }

        /// <summary>
        /// This method is called automatically from LINKS on close
        /// </summary>
        public static void OnDispose()
        {
            Test_FormClose();
        }

        /// <summary>
        /// This method is called automatically from LINKS on start
        /// </summary>
        public static void OnLoad()
        {
            if (Properties.Settings.Default.LoadOnStart)
            {
                Test_InitWPFWindow();

                

                //jarvisWPF.PublicClass.SpeechSynth.AttachToVisemeReached((x, y) => Test_WPFWindow.Synthesizer_UpdateAnimation(x, y));
                //jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(Properties.Settings.Default.StartUpPhrase);
            }
        }
        
    }


    /// <summary>
    /// This sample class is used to initialize and access window's UI elements from LINKS
    /// Note that we have to invoke the window if we want to access any of the UI elements from LINKS
    /// </summary>
    public class MyCustomWindow
    {
        /// <summary>
        /// Initializes class
        /// Initializes wpf window in a new thread
        /// </summary>
        public MyCustomWindow()
        {
            //Debugger.Launch();
            //Debugger.Break();

            Thread t = new Thread(InitializeWindow);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }

        /// <summary>
        /// Declare test window
        /// </summary>
        public Test_WPFWindow wpfTest;

        /// <summary>
        /// Method to initialize test window
        /// Sets visuals for the window
        /// </summary>
        private void InitializeWindow()
        {
            //Debugger.Launch();
            //Debugger.Break();

            // Initialize Window Instance
            wpfTest = new Test_WPFWindow();            

            //wpfTest.Play(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Hover_buttons.wav",1);
            //Thread.Sleep(200);
            //Thread.Sleep(2000);

            //wpfTest.Play(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Hover_main_button.wav",2);
            //Thread.Sleep(200);
            //Thread.Sleep(2000);

            //wpfTest.Play(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Media_KeyPress.wav",3);
            //Thread.Sleep(200);
            //Thread.Sleep(2000);

            //wpfTest.Play(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Speech_Pause.wav",4);
            //Thread.Sleep(200);
            //Thread.Sleep(4000);

            //Task.WaitAll();
            Thread.Sleep(200);
            Console.WriteLine("");
            try
            {
                if (wpfTest.LINKSInitialized())
                {
                    //// Get Templates for controls
                    //// Note: This will not work on v2.6.2.3 or older
                    //ControlTemplate buttonTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("ButtonControl_CutLeftRight");
                    //ControlTemplate textBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("TextBox_ControlTemplate");
                    //ControlTemplate checkBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("CheckBox_ControlTemplate");
                    //ControlTemplate comboBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("ComboBox_ControlTemplate");
                    //
                    //// Apply templates
                    //// Note: This will not work on v2.6.2.3 or older            
                    //wpfTest.test_comboBox.Template = comboBoxTemplate;
                    //wpfTest.button_Connect.Template = buttonTemplate;
                    //wpfTest.textBox_ip.Template = textBoxTemplate;
                    //wpfTest.textBox_URL.Template = textBoxTemplate;
                    //wpfTest.test_checkBox_LoadOnStart.Template = checkBoxTemplate;
                }
            }
            catch
            {
                Console.WriteLine("Not all templates work on v2.6.2.3 or older. Only Button Template can work after commenting out the rest of the template lines.");
            }

            // Set properties
            //wpfTest.test_comboBox.IsDropDownOpen = false;
            //wpfTest.test_comboBox.Visibility = Visibility.Hidden;
            wpfTest.test_checkBox_LoadOnStart.IsChecked = Properties.Settings.Default.LoadOnStart;
            //wpfTest.test_labelBlock_LoadOnStart.FontFamily = jarvisWPF.PublicClass.GetFont();

            wpfTest.Show();
            wpfTest.Closed += (s, e) => Dispatcher.ExitAllFrames();
            Dispatcher.Run();
        }

        /// <summary>
        /// Allows external classes to modify window title
        /// </summary>
        /// <param name="str"></param>
        internal void WPFTest_ChangeTitle(string str)
        {
            wpfTest.Dispatcher.Invoke(() =>
            {
                wpfTest.Title = str;
            });
        }

        /// <summary>
        /// Allows external classes to close the window
        /// </summary>
        internal void FormTest_Close()
        {
            //Debugger.Break();
            if (wpfTest != null)
            {
                if (!wpfTest.IsClosed)
                {
                    wpfTest.Dispatcher.Invoke(() =>
                    {

                        wpfTest.Close();
                        Console.WriteLine("wpfTest: " + wpfTest.IsClosed);
                        wpfTest = null;

                    });
                }
            }
        }
    }
}
