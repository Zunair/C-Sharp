using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        /// Initializes window
        /// </summary>
        public Test_WPFWindow()
        {
            //Debugger.Launch();
            //Debugger.Break();
            
            InitializeComponent();

            // Attaches drag window event
            MouseLeftButtonDown += DragWindow;

            Left = Properties.Settings.Default.WindowLocation.X;
            Top = Properties.Settings.Default.WindowLocation.Y;

            InitializeContextMenu();
        }

        /// <summary>
        /// Allows to drag window
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public Configuration config = new Configuration();
        public List<SwitchView> SwitchWindows = new List<SwitchView>();
        

        /// <summary>
        /// Speak Button Click event
        /// </summary>
        private void button_Connect_Click(object sender, RoutedEventArgs e)
        {
            Test_WPFWindow currentWindow = this;
            currentBridge = config.SelectBridge(currentWindow.comboBox.SelectedValue.ToString());
            if ((bool)currentWindow.checkBox_SetAsDefault.IsChecked)
            {
                config.SetAsDefault(currentBridge.Name);
                config.Save();
            }

            currentWindow.textBox_ip.Text = currentBridge.Ip;
            currentWindow.textBox_name.Text = currentBridge.Name;            
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
            TrayIcon_RemoveEvents();
            SwitchWindows.ForEach(a => a.Close());
            config.Save();

            //Properties.Settings.Default.Test_URL = textBox_URL.Text;
            //Properties.Settings.Default.Save();
            Properties.Settings.Default.WindowLocation = new System.Drawing.Point((int)Left, (int)Top);
            Properties.Settings.Default.LoadOnStart = RunOnStart;
            Properties.Settings.Default.Save();
        }

        private void test_checkBox_LoadOnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            //test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
        }

        private void test_checkBox_LoadOnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            DoubleAnimation ani = new DoubleAnimation(.08, TimeSpan.FromMilliseconds(100));
            chkBox.BeginAnimation(OpacityProperty, ani);
            //test_labelBlock_LoadOnStart.BeginAnimation(OpacityProperty, ani);
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

        private async void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (userId == string.Empty) return;
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            try
            {

                string base_url = "http://" + ip  + "/api/" + userId + "/";
                double bri = ((Slider)sender).Value / 100 * 254;
                await LREST.Put(base_url + textBox_URL.Text, "{\"bri\":" + bri + "}", "", ".json");
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
            this.Close();
        }

        private void uiElement_MoustEnter(object sender, MouseEventArgs e)
        {
            //AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.In);
            
        }

        private void uiElement_MoustLeave(object sender, MouseEventArgs e)
        {
            //AnimateUI.Fade(new object[] { sender, test_labelBlock_LoadOnStart }, UIAnimations.FadeType.Out);
        }

        private void btn_uiElement_MoustEnter(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.In);
        }

        private void btn_uiElement_MoustLeave(object sender, MouseEventArgs e)
        {
            AnimateUI.Fade(new object[] { sender }, UIAnimations.FadeType.Out);
        }

        public Bridge currentBridge = new Bridge();
        private async void window_Initialized(object sender, EventArgs e)
        {
            try
            {
                config.Load();
                if (config.Bridges.Count == 0)
                {
                    await Dispatcher.BeginInvoke(new Action(async delegate
                    {
                        this.textBox_ip.Text = await currentBridge.TryGetIP(); // same thread or object that it can use
                }));

                    //textBox_ip.Text = await currentBridge.TryGetIP(); // diffrent thread
                }
                else
                {
                    await Dispatcher.BeginInvoke(new Action(delegate
                    {
                        ((Test_WPFWindow)sender).comboBox.ItemsSource = config.Bridges.Select(b => b.Name);
                    }));


                    currentBridge = config.DefaultBridge;
                    if (currentBridge != null)
                    {
                        await Dispatcher.BeginInvoke(new Action(delegate
                        {
                            this.comboBox.SelectedValue = currentBridge.Name;
                            this.textBox_ip.Text = currentBridge.Ip;
                            this.textBox_name.Text = currentBridge.Name;
                            this.checkBox_SetAsDefault.IsChecked = currentBridge.IsDefault;
                        }));
                        // TODO: Check if bridge is reachable
                        // Notify visually
                        // Enable commands

                    }
                }
                TrayIcon_AttachEvents();
            }
            catch (Exception error)
            {
                MessageBox.Show("Error 03: " + error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private async void button_showLights_Click(object sender, RoutedEventArgs e)
        {
            //Debugger.Break();

            try
            {
                ((Button)sender).IsEnabled = false;

                var bridge = await currentBridge.Client.GetBridgeAsync();


                //var lights = await currentBridge.Client.GetLightsAsync();
                if (bridge.Lights.Count() == 0)
                {
                    MessageBox.Show("No lights found.", "Lights - Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    //Response webResponse = await currentBridge.GetResponse();

                    // Discard all windows that were closed
                    SwitchWindows.RemoveAll(s => s.IsClosed);

                    foreach (Light light in bridge.Lights)
                    {
                        //string lSwitchID = light.Key + currentBridge.Name;
                        //if (!SwitchWindows.Exists(s => s.lSwitchID == lSwitchID))
                        if (!SwitchWindows.Exists(s => s.LSwitch.Id == light.Id && s.CurrentBridge.Name == currentBridge.Name))
                        {
                            SwitchWindows.Add(new SwitchView(light, currentBridge, ref SwitchWindows));
                            Console.WriteLine("Window Created: " + light.Name);
                        }
                        //Console.WriteLine(light.Key);
                        //Console.WriteLine(light.Value["name"]);
                        //Console.WriteLine(light.Value["state"]["on"]);
                    }


                    // Show all switch windows
                    SwitchWindows.ForEach(a => a.Show());

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (error.InnerException != null)
                    MessageBox.Show(error.InnerException.Message, "Inner Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            finally
            {
                ((Button)sender).IsEnabled = true;
            }
        }

        static DateTime lastLightUpdate = DateTime.Now;
        public static void Synthesizer_UpdateAnimation(object sender, VisemeReachedEventArgs e)
        {
            if (lastLightUpdate.AddMilliseconds(9) < DateTime.Now)
            {
                if (Global.LINKS_Viseme != null && Global.LINKS_Viseme.IsInitialized)
                {
                    Console.WriteLine(e.Viseme + " from links.");
                    Console.WriteLine("Synth: " + ((SwitchView)sender).Title);
                    //LINKS_Viseme.SwitchDimmer(LINKS_Viseme, e.Viseme);
                    lastLightUpdate = DateTime.Now;
                }
            }
        }

        private async void button_AddBridge_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                await Dispatcher.BeginInvoke(new Action(delegate
                {
                    this.button_AddBridge.IsEnabled = false;
                }));
                

                bool readyToAdd = true;
                Bridge newBridge = new Bridge();
                newBridge.Ip = this.textBox_ip.Text;
                newBridge.Name = this.textBox_name.Text;

                if (config.BridgeExists(newBridge.Name))
                {
                    if (MessageBox.Show("This bridge name is already taken, press OK to UnLink this bridge and Link the new bridge with same name.",
                                        "Already Exists", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        // TODO: UnLink bridge
                        currentBridge = config.SelectBridge(newBridge.Name);

                        if (await currentBridge.UnLink(currentBridge.UserId))
                        {
                            // Remove bridge from config
                            config.RemoveBridge(newBridge.Name);
                            config.Save();

                        }
                        else
                        {
                            readyToAdd = false;
                            MessageBox.Show("Please use another name, can not replace this bridge at this time.");
                        }
                    }
                    else
                    {
                        readyToAdd = false;
                    }
                }

                if (readyToAdd)
                {
                    MessageBox.Show("Press LINK button on the hue bridge then click OK.");
                    if (await newBridge.LinkBridge(textBox_name.Text, textBox_ip.Text))
                    {
                        newBridge.IsDefault = (bool)checkBox_SetAsDefault.IsChecked;
                        config.AddBridge(newBridge);
                        config.Save();
                        currentBridge = config.SelectBridge(newBridge.Name);
                        MessageBox.Show("Connected.");
                    }
                    else
                    {
                        MessageBox.Show("Try again after pressing LINK button on the hue bridge.");
                    }
                }

                await Dispatcher.BeginInvoke(new Action(delegate
                {
                    this.button_AddBridge.IsEnabled = true;
                }));

            }
            catch(Exception error)
            {
                MessageBox.Show("Error 02: " + error.Message);
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Test_WPFWindow currentWindow = this;

            if (currentWindow.comboBox.SelectedValue != null)
            {
                currentBridge = config.SelectBridge(currentWindow.comboBox.SelectedValue.ToString());
                currentWindow.textBox_ip.Text = currentBridge.Ip;
                currentWindow.textBox_name.Text = currentBridge.Name;
                currentWindow.checkBox_SetAsDefault.IsChecked = currentBridge.IsDefault;
            }
        }

        private void checkBox_SetAsDefault_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)((CheckBox)sender).IsChecked && currentBridge.Name != string.Empty)
            {
                config.SetAsDefault(currentBridge.Name);
                config.Save();
            }
        }

        
        public bool IsClosed = false;
        private void window_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
        }

        public void Play1(string sound)
        {
            MediaElement m = new MediaElement();
            main_grid.Children.Add(m);
            MediaTimeline mT = new MediaTimeline(new Uri(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Hover_buttons.wav"));
            Storyboard.SetTarget(mT, media);
            Storyboard.SetTarget(mT, m);
            Storyboard s = new Storyboard();
            s.Children.Add(mT);
            s.Begin();
        }

        
        //fail
        public void Play2(string sound)
        {
            MediaElement m = new MediaElement();
            MediaTimeline mT = new MediaTimeline(new Uri(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Hover_buttons.wav"));
            Storyboard s = new Storyboard();
            main_grid.Children.Add(m);
            Storyboard.SetTarget(mT, media);
            Storyboard.SetTarget(mT, m);

            s.Children.Add(mT);
            System.Threading.Tasks.Task.Run(() =>
            {                
                s.Begin();
            });
            System.Threading.Tasks.Task.WaitAll();

        }

        //works
        public void Play3(string sound)
        {
            Window w = new Window();
            Grid g = new Grid();
            w.Content = g;
            w.Visibility = Visibility.Collapsed;
            w.Show();
            MediaElement m = new MediaElement();
            g.Children.Add(m);
            MediaTimeline mT = new MediaTimeline(new Uri(@"C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Hover_buttons.wav"));
            Storyboard.SetTarget(mT, media);
            Storyboard.SetTarget(mT, m);
            Storyboard s = new Storyboard();
            s.Children.Add(mT);
            s.Begin();
        }


        static SoundEffectsPlayer AudioPlayer1 = new SoundEffectsPlayer();
        static SoundEffectsPlayer AudioPlayer2 = new SoundEffectsPlayer();
        static SoundEffectsPlayer AudioPlayer3 = new SoundEffectsPlayer();
        static SoundEffectsPlayer AudioPlayer4 = new SoundEffectsPlayer();
        public void Play(string sound, int player)
        {
            //System.Threading.Tasks.Task.Run(() =>
            //{
            //    tClass tC = new tClass();
            //    tC.Play(sound);
            //});
            //tC.Play(sound);

            Thread t = new Thread(()=>
            {
                switch (player)
                {
                    case 1:
                        AudioPlayer1.Play(sound);
                        break;
                    case 2:
                        AudioPlayer2.Play(sound);
                        break;
                    case 3:
                        AudioPlayer3.Play(sound);
                        break;
                    case 4:
                        AudioPlayer4.Play(sound);
                        break;
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();

            //tC.Play("C:\Users\Zunair\AppData\Roaming\LINKS\Customization\Sound Effects\Show_Window.wav");
        }

        class SoundEffectsPlayer : Window
        {
            MediaElement m = null;
            MediaTimeline mT = null;
            Grid g = null;

            public SoundEffectsPlayer()
            {
                WindowStyle = WindowStyle.None;
                Visibility = Visibility.Hidden;
                WindowState = WindowState.Normal;
                WindowStartupLocation = WindowStartupLocation.Manual;
                ShowInTaskbar = false;
                Opacity = 0;
                Width = 0;
                Height = 0;
                WindowStartupLocation = 0;

                g = new Grid();
                Content = g;    
                m = new MediaElement();
                g.Children.Add(m);

                Show();
                Hide();
            }

            public void Play(string sound)
            {
                Dispatcher.BeginInvoke((Action)(() => 
                {
                    Storyboard s = null;
                    mT = new MediaTimeline(new Uri(sound));
                    Storyboard.SetTarget(mT, m);
                    s = new Storyboard();
                    s.Children.Add(mT);
                    s.Completed += S_Completed;
                    s.Begin();                    
                }));
            }

            private void S_Completed(object sender, EventArgs e)
            {
                Close();
            }
        }
    }
}
