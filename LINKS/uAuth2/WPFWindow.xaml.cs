using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace uAuth2
{
    /// <summary>
    /// Interaction logic for Test_WPFWindow.xaml
    /// </summary>
    public partial class WPFWindow : Window
    {
        /// <summary>
        /// Animation class
        /// Store for all custom animations
        /// </summary>
        UIAnimations AnimateUI = new UIAnimations();

        string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        bool runOnStart = true;
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

        //WebBrowser webBrowser;

        /// <summary>
        /// Initializes window
        /// </summary>
        public WPFWindow()
        {
            //this._oAuth2 = oAuth2;
            //this._jObject = this._oAuth2._jObject;
            //this._windowTitle = this._oAuth2._app_name.Replace("_", " ");

            InitializeComponent();
            
            // Attaches drag window event
            MouseLeftButtonDown += DragWindow;

            //Left = Properties.Settings.Default.WindowLocation.X;
            //Top = Properties.Settings.Default.WindowLocation.Y;

            //this.Title = this._windowTitle;
            //this.Hide();
            //this.AllowsTransparency = false;
            this.WindowStyle = WindowStyle.ToolWindow;
            this.ResizeMode = ResizeMode.NoResize;
            this.ShowInTaskbar = false;
            //this.WindowState = WindowState.Normal;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Minimized;

            //this.Opacity = 0;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //this.Top = 1;
            //this.Left = 1;
            //this.Width = 0;
            //this.Height = 0;
            this.Width = 600;
            this.Height = 820;
            //this.Center();
            //this.Top = 9999999;

            //this.grid.Children.Add(webBrowser);

            //webBrowser.CompositionMode = System.Windows.Interop.CompositionMode.Full;
            //webBrowser.IsRedirected = true;

            this.Closing += AuthWindow_Closing;
        }

        private void AuthWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_retrieved)
            {
                _jObject = null;
            }
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
        private void test_button_speak_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LINKSInitialized())
                {
                    //jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(test_textBox_Speak.Text);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
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
            //AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.In);
        }

        private void test_checkBox_LoadOnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            //AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.Out);            
        }

        public bool LINKSInitialized()
        {
            bool retVal = true;

            if (jarvisWPF.PublicClass.SpeechSynth == null)
            {
                //Console.WriteLine("This dll is not loaded in LINKS. You will not be able to run any LINKS functionalites.");
                retVal = false;
            }

            return retVal;
        }
        
        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            Task.WaitAll(); // This allows to complete all animations safely
            this.Close();
        }

        private void button_cancel_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.In);
        }

        private void button_cancel_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.Out);
        }

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            //loadAuthUrl();            
        }

        private void window_Activated(object sender, EventArgs e)
        {
            webBrowser.Height = double.NaN;
        }

        private void window_Deactivated(object sender, EventArgs e)
        {
            webBrowser.Height = 0;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            //AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.In, 2);
        }
    }
}
