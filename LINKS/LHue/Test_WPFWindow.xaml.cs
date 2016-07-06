using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LHue
{
    /// <summary>
    /// Interaction logic for Test_WPFWindow.xaml
    /// </summary>
    public partial class Test_WPFWindow : Window
    {
        /// <summary>
        /// Animation class
        /// Store for all custom animations
        /// </summary>
        UIAnimations AnimateUI = new UIAnimations();

        string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        bool runOnStart = true;
        protected string userId = string.Empty;
        protected string ip = string.Empty;
        XmlDataProvider test_comboBox_DataProvider = null;

        public bool RunOnStart
        {
            get
            {
                return runOnStart;
            }

            set
            {
                runOnStart = value;
            }
        }

        /// <summary>
        /// Used by combo box
        /// </summary>
        public XmlDataProvider test_ComboBox_DataProvider
        {
            get
            {
                if (test_comboBox_DataProvider == null)
                {
                    test_comboBox_DataProvider = new XmlDataProvider();
                    test_comboBox_DataProvider.Source = new Uri(AppData + "\\LINKS\\Customization\\XML\\UserVariables.xml");
                }
                return test_comboBox_DataProvider;
            }
        }

        /// <summary>
        /// Initializes window
        /// </summary>
        public Test_WPFWindow()
        {
            InitializeComponent();

            // Attaches drag window event
            MouseLeftButtonDown += DragWindow;

            Left = Properties.Settings.Default.WindowLocation.X;
            Top = Properties.Settings.Default.WindowLocation.Y;

            textBox_URL.Text = Properties.Settings.Default.Test_URL;
            textBox_ip.Text = Properties.Settings.Default.BridgeIP;
            ip = textBox_ip.Text;
            if (Properties.Settings.Default.AccessToken != string.Empty)
            {
                textBox_ip.IsEnabled = false;
                button_Connect.IsEnabled = false;
                byte[] bytes = Convert.FromBase64String(Properties.Settings.Default.AccessToken);
                userId = Encoding.UTF8.GetString(bytes);
            }
            else
            {
                //if (MessageBox.Show("Press the link button on Hue bridge then click OK.", "Connect", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                //{
                //    button_Connect_Click(this, null);
                //}
            }
            //Test();
        }

        private static async void Test()
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                LREST lRest = new LREST();
                for (int i = 200; i <= 254; i++)
                {
                    string response = await lRest.Put("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights/1/state", "{\"bri\":" + i + "}", "", ".json");
                    //await lRest.Post("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights", "", "what1=hello 1&what2=2", ".json");
                    Console.WriteLine(response + i);
                }
                s.Stop();
                Console.WriteLine(s.Elapsed);
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }
            //await lRest.Get(new Uri("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights"));

            Console.WriteLine("Get Request Completed.");

        }
        /// <summary>
        /// Allows to drag window
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Speak Button Click event
        /// </summary>
        private async void button_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (LINKSInitialized())
                //{
                //    jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(textBox_ip.Text);
                //}
                string retVal = "";
                //LREST lRest = new LREST();
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
                string ip = Properties.Settings.Default.BridgeIP;
                

                //JObject jResponse = (JObject)JsonConvert.DeserializeObject(ip);
                if (ip != "127.0.0.1" | ip != string.Empty)
                {
                    if (Debugger.IsAttached)
                    {
                        ip = "[{\"id\":\"<bruh>\",\"internalipaddress\":\"73.251.151.39\"}]";
                    }
                    else
                    {
                        ip = await lRest.Get("https://www.meethue.com/api/nupnp");
                    }
                    JArray jResponse = (JArray)JsonConvert.DeserializeObject(ip);
                    if (jResponse.Count > 0)
                    {
                        ip = jResponse[0]["internalipaddress"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Can not find Philips Hue SmartHub.\r\nPlease plugin your hue hub and click connect again or type the IP manually.", "Failed to Find Hub", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        ip = "127.0.0.1";
                        return;
                    }

                    textBox_ip.Text = ip;

                }
                Properties.Settings.Default.BridgeIP = ip;
                Properties.Settings.Default.Save();


                string createUser = "{\"devicetype\":\"LINKS#PC " + Environment.UserName.ToUpper() + "\"}";
                if (Debugger.IsAttached)
                {
                    createUser = "[{\"success\":{\"username\":\"7ngKxvpo5fuLWLmCZfb42KzvKSYlA6gGgX4CYFtN\"}}]";
                }
                else
                {
                    createUser = await lRest.Post("http://" + ip + "/api", createUser, "", ".json");
                }

                try
                {
                    try
                    {
                        JArray jResponseO = (JArray)JsonConvert.DeserializeObject(createUser);
                        retVal = jResponseO[0]["success"]["username"].ToString();
                    }
                    catch
                    {
                        JArray jResponse = (JArray)JsonConvert.DeserializeObject(createUser);
                        if (jResponse.Count > 0)
                        {                            
                            MessageBox.Show(jResponse[0]["error"]["description"] + "\r\n Please press the button on the hue bridge and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                catch(Exception error)
                {
                    MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(retVal);
                Properties.Settings.Default.AccessToken = Convert.ToBase64String(bytes);

                ip = textBox_ip.Text;
                userId = retVal;
                MessageBox.Show("Connection Successfull", "Connected", MessageBoxButton.OK, MessageBoxImage.);

                //System.Threading.Thread.Sleep(200);
                //retVal = await lRest.Put("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights/1/state", "{\"bri\":0}", "", ".json");
                //System.Threading.Thread.Sleep(100);

                //sw.Stop();

                //Console.WriteLine("Get Request Completed in " + sw.Elapsed);

                
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Emulate Button Click event
        /// </summary>
        private void test_button_emulate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LINKSInitialized())
                {
                    //jarvisWPF.Classes.Plugins.PluginController.EmulateSpeech(test_textBox_Emulate.Text);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// Highlights move icon
        /// </summary>
        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
            img.BeginAnimation(Image.OpacityProperty, ani);
        }

        /// <summary>
        /// Unhighlights move icon
        /// </summary>
        private void image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            DoubleAnimation ani = new DoubleAnimation(.08, TimeSpan.FromMilliseconds(100));
            img.BeginAnimation(Image.OpacityProperty, ani);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowLocation = new System.Drawing.Point((int)Left, (int)Top);
            Properties.Settings.Default.LoadOnStart = RunOnStart;
            Properties.Settings.Default.Save();
        }

        private void test_checkBox_LoadOnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
        }

        private void test_checkBox_LoadOnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            DoubleAnimation ani = new DoubleAnimation(.08, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
        }

        public bool LINKSInitialized()
        {
            bool retVal = true;

            //if (jarvisWPF.PublicClass.SpeechSynth == null)
            {
                Console.WriteLine("This dll is not loaded in LINKS. You will not be able to run any LINKS functionalites.");
                retVal = false;
            }

            return retVal;
        }

        LREST lRest = new LREST();
        private async void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            try
            {

                string base_url = "http://" + ip  + "/api/" + userId + "/";
                double bri = ((Slider)sender).Value / 100 * 254;
                await lRest.Put(base_url + textBox_URL.Text, "{\"bri\":" + bri + "}", "", ".json");
                //await lRest.Put("http://73.251.151.39/api/lz3nFIcGOelQlsxsDJYqrXGuxivjFmWswZ9fIGcw/lights/1/state", "{\"bri\":" + bri + "}", "", ".json");
                //System.Threading.Thread.Sleep(50);

                //sw.Stop();
                Console.WriteLine(bri);
                //Console.WriteLine("Get Request Completed in " + sw.Elapsed);

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Test_URL = textBox_URL.Text;
            Properties.Settings.Default.Save();
            lRest = null;
            this.Close();
        }

        private void uiElement_MoustEnter(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.In);
            
        }

        private void uiElement_MoustLeave(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.Out);
        }

        private void btn_uiElement_MoustEnter(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.In);
        }

        private void btn_uiElement_MoustLeave(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.Out);
        }
    }
}
