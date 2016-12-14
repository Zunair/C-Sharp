using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace uAuth2
{
    public class It
    {
        public static AuthWindowThread authWindowThread;

        public static string InitAuthWindowAsync(string appName = null, string elementName = null)
        {
            string retVal = string.Empty;

            if (authWindowThread == null)
            {
                //retVal = await Task.Run(() =>
                //{
                    // Creates window instance
                    authWindowThread = new AuthWindowThread();
                    retVal = authWindowThread.AuthWindowThreadSync(appName, elementName);
                    authWindowThread = null;
                //    return retVal;
                //});
            }

            return retVal;
        }

        /// <summary>
        /// Initializes a new instance of WPF window
        /// Can be called from any reponse or web [uAuth2.MyCustomClass.Test_InitWPFWindow]
        /// </summary>
        /// <returns></returns>
        public static string InitAuthWindow()
        {            
            string retVal = string.Empty;

            if (authWindowThread == null)
            {
                // Creates window instance
                authWindowThread = new AuthWindowThread();
            }

            return retVal;
        }

        /// <summary>
        /// Allows user to change Title of the window
        /// Can be called from any reponse or web [uAuth2.MyCustomClass.Test_ChangeTitleInMyCustomWindow("Hello World")]
        /// </summary>
        /// <param name="labelText"></param>
        /// <returns></returns>
        public static string Test_ChangeTitleInMyCustomWindow(string labelText)
        {
            string retVal = string.Empty;

            authWindowThread.WPFTest_ChangeTitle(labelText);

            return retVal;
        }

        /// <summary>
        /// Closes the window
        /// Can be called from any reponse or web [uAuth2.MyCustomClass.Test_FormClose]
        /// </summary>
        /// <returns></returns>
        public static string Test_FormClose()
        {
            string retVal = string.Empty;

            if (authWindowThread != null)
            {
                authWindowThread.FormTest_Close();
                authWindowThread = null;
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
        //public static void OnLoad()
        //{
        //    if (Properties.Settings.Default.LoadOnStart)
        //    {
        //        InitAuthWindowAsync();
        //        jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(Properties.Settings.Default.StartUpPhrase);
        //    }
        //}
    }


    /// <summary>
    /// Interaction logic for calling functions from LINKS
    /// This sample class is used to initialize and access window's UI elements from LINKS
    /// Note that we have to invoke the window if we want to access any of the UI elements from LINKS
    /// </summary>
    public class AuthWindowThread
    {
        string elementValue;
        internal string AuthWindowThreadSync(string appName = null, string elementName = null, string exeMode = "false")
        {
            Thread t = new Thread(() => InitializeWindow(appName, elementName, exeMode));

            // Therad has to be STA if it has to be called from LINKS
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            // In order to get syncronous response from window, we need to create a window and attach that to calling thread.
            t.Join(); // this should pause the thread at this point untill window is closed.

            return elementValue;
        }
        

        /// <summary>
        /// Initializes class
        /// Initializes wpf window in a new thread
        /// </summary>
        internal AuthWindowThread()
        {
            //Thread t = new Thread(InitializeWindow);
            //t.SetApartmentState(ApartmentState.STA);
            //
            //t.Start();
        }


        /// <summary>
        /// Declare test window
        /// </summary>
        WPFWindow wpfTest;

        /// <summary>
        /// Method to initialize test window
        /// Sets visuals for the window
        /// </summary>
        internal void InitializeWindow(string appName, string elementName, string exeMode)
        {
            // Initialize Window Instance
            wpfTest = new WPFWindow();

            try
            {
                if (wpfTest.LINKSInitialized())
                {
                    // Get Templates for controls
                    // Note: This will not work on v2.6.2.3 or older
                    ControlTemplate buttonTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("ButtonControl_CutLeftRight");
                    ControlTemplate textBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("TextBox_ControlTemplate");
                    ControlTemplate checkBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("CheckBox_ControlTemplate");
                    ControlTemplate comboBoxTemplate = (ControlTemplate)jarvisWPF.App.Current.FindResource("ComboBox_ControlTemplate");

                    // Apply templates
                    // Note: This will not work on v2.6.2.3 or older            
                    //wpfTest.test_comboBox.Template = comboBoxTemplate;
                    //wpfTest.test_button_Speak.Template = buttonTemplate;
                    //wpfTest.test_textBox_Speak.Template = textBoxTemplate;
                    //wpfTest.test_textBox_Emulate.Template = textBoxTemplate;
                    //wpfTest.button_cancel.Template = buttonTemplate;
                    //wpfTest.checkBox_LoadOnStart.Template = checkBoxTemplate;
                }
            }
            catch
            {
                //Console.WriteLine("Not all templates work on v2.6.2.3 or older. Only Button Template can work after commenting out the rest of the template lines.");
            }

            // Set properties
            //wpfTest.test_comboBox.IsDropDownOpen = false;
            //wpfTest.test_comboBox.Visibility = Visibility.Hidden;
            //wpfTest.checkBox_LoadOnStart.IsChecked = Properties.Settings.Default.LoadOnStart;
            //wpfTest.test_labelBlock_LoadOnStart.FontFamily = jarvisWPF.PublicClass.GetFont();

            wpfTest.Show();
            wpfTest.Closed += (s, e) => Dispatcher.ExitAllFrames();
            elementValue = It.authWindowThread.GetElement(appName, elementName, exeMode);
            //wpfTest.loadAuthUrl();
            //It.Test_FormClose();

            //Dispatcher.Run();
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
            wpfTest.Dispatcher.Invoke(() =>
            {
                wpfTest.Hide();
                wpfTest.Close();
            });
        }

        internal string GetElement(string appName = null, string elementName = null, string exeMode = "false")
        {
            //Debugger.Launch();
            //Debugger.Break();
            string retVal = null;

            try
            {

                oAuth2 oA2 = null;
                string[] args = Environment.GetCommandLineArgs();
//                appName = "test";
                if (args.Length == 1 && appName != null)
                {
                    string arguments = args[0] + "\tAppName=" + appName;
                    args = arguments.Split('\t');
                }

                args[0] = "File=" + args[0];
                var parsedArgs = args
                .Select(s => s.Split(new[] { '=' }))
                .ToDictionary(s => s[0], s => s[1]);

                bool ExeMode = exeMode.ToLower() == "false" ? false : true;

                if (args.Length == 4 && parsedArgs.ContainsKey("ClientID"))
                {
                    //oA2 = new oAuth2(wpfTest, args[1], args[2], args[3]);
                    oA2 = new oAuth2(wpfTest, parsedArgs["AppName"], parsedArgs["ClientID"], parsedArgs["ClientSecret"]);
                }
                else if (parsedArgs.ContainsKey("AppName"))
                {
                    //oA2 = new oAuth2(wpfTest, args[1]);
                    oA2 = new oAuth2(wpfTest, parsedArgs["AppName"]);
                }
                else
                {
                    if (appName == null || appName == "")
                    {
                        appName = "SmartThings_MVC_LINKS";
                    }
                    oA2 = new oAuth2(wpfTest, appName);
                }


                if (oA2 == null)
                {
                    if (ExeMode)
                    {
                        MessageBox.Show("Please provide app name.\r\nA .json file name from %AppDat%\\LINKS\\Customization\\Plugins\\oAuth2\\", "Authorization Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        retVal = "Error: " + "Please provide app name, .json file from %AppDat%\\LINKS\\Customization\\Plugins\\oAuth2\\";
                    }
                }
                else
                {
                    string reply = oA2.getAccessToken(ref elementName);
                    if (reply != null)
                    {
                        if (ExeMode)
                        {
                            MessageBox.Show(elementName + ": " + reply, "Authorized", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            retVal = reply;
                        }
                    }
                    else
                    {
                        if (ExeMode)
                        {
                            MessageBox.Show("Authorization Failed.\r\n" + oA2._error, "Authorization Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            retVal = "Error: Auth Failed. " + oA2._error;
                        }
                    }
                }
                                
                this.FormTest_Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retVal;
        }
    }
}
