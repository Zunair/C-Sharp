using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LSamplePluginWPF_Blank
{

    public abstract class LINKS_WPF_Window : Window
    {
        public MainWindow()
        {

        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Runs when LINKS says "Loading Custom Componenets" on startup
        /// </summary>
        public static void OnLoad()
        {
            //System.Diagnostics.Debugger.Break();

            Thread t = new Thread(LoadMainWindow);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Runs when LINKS is shutting down
        /// </summary>
        public static void OnDispose()
        {
            //System.Diagnostics.Debugger.Break();
        }
        
        private static void LoadMainWindow()
        {
            LSampleClass.mainWindow = new MainWindow();
            LSampleClass.mainWindow.Show();
            LSampleClass.mainWindow.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.Run();
        }
    }

    /// <summary>
    /// LINKS interaction class
    /// All public static functions with string args in this calls are accessible from LINKS
    /// </summary>
    public class LSampleClass
    {
        public static MainWindow mainWindow;

        /// <summary>
        /// Synchronous function
        /// This function can be called from LINKS as follows
        /// [LSamplePluginWPF_Blank.LSampleClass.ChangeWindowTitle("New Window Title")]
        /// </summary>
        /// <param name="title">String to show as window title</param>
        /// <returns>String when completed</returns>
        public static string ChangeWindowTitle(string title)
        {
            string retVal = "";
            TimeSpan timeout = TimeSpan.FromSeconds(5);

            mainWindow.Dispatcher.Invoke (new Action(delegate
            {
                mainWindow.Title = title;                
            }), timeout);
            retVal = "title change completed.";

            return retVal;
        }

        /// <summary>
        /// Asynchronous function
        /// This function can be called from LINKS as follows
        /// [LSamplePluginWPF_Blank.LSampleClass.ChangeWindowTitleAsync("New Window Title")]
        /// </summary>
        /// <param name="title">String to show as window title</param>
        /// <returns>String when completed</returns>
        public static async Task<string> ChangeWindowTitleAsync(string title)
        {
            string retVal = "";
            
            await mainWindow.Dispatcher.BeginInvoke(new Action(delegate
            {
                mainWindow.Title = title;
                retVal = "title change completed.";
            }), DispatcherPriority.Send);

            return retVal;
        }
    }
}
