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

namespace SpeechEmulator
{
    /// <summary>
    /// Interaction logic for Test_WPFWindow.xaml
    /// </summary>
    public partial class Test_WPFWindow : System.Windows.Window
    {
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
                    jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(test_textBox_Speak.Text);
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
                    jarvisWPF.Classes.Plugins.PluginController.Emulate.EmulateSpeech(test_textBox_Emulate.Text);
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

            if (jarvisWPF.PublicClass.SpeechSynth == null)
            {
                Console.WriteLine("This dll is not loaded in LINKS. You will not be able to run any LINKS functionalites.");
                retVal = false;
            }

            return retVal;
        }
    }
}
