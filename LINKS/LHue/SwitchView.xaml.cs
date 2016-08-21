using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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

namespace LHue
{
    /// <summary>
    /// Interaction logic for Switch.xaml
    /// </summary>
    public partial class SwitchView : Window
    {
        //public readonly string lSwitchID;
        private Light lSwitch;
        public readonly Bridge CurrentBridge;
        //bool isOn;
        string base_url;
        internal bool IsClosed = false;
        float newMin = 20, newMax = 254;

        public Light LSwitch
        {
            get
            {
                return lSwitch;
            }

            set
            {
                lSwitch = value;
            }
        }

        public async void UpdateView(Light light = null, bool refresh = false)
        {            
            if (refresh)
            {
                Console.WriteLine("UpdateView: " + LSwitch.Name);
                LSwitch = await CurrentBridge.Client.GetLightAsync(LSwitch.Id);
            }
            else
            {
                LSwitch = light;
                Console.WriteLine("UpdateView Refresh:" + LSwitch.Name);
            }

            if (LSwitch.State.On)
            {
                Brightness = map(LSwitch.State.Brightness, 0, 254, 0, 100);
            }
            IsOn = LSwitch.State.On;

            RGBColor hue = LSwitch.State.ToRgb();
            SetColorSlider(hue);

            //return lSwitch;
        }

        private async void SetColorSlider(RGBColor hueColor)
        {
            colorSlider.Value = 0;
            for (int s = 1; s <= 1000; s++)
            {
                
                await this.colorSlider.Dispatcher.BeginInvoke(new Action(delegate
                {
                    colorSlider.Value += 1;
                    colorSlider.SetCurrentValue(Slider.ValueProperty, colorSlider.Value + 1);
                }));
                
                RGBColor hue = new RGBColor(colorSlider.SelectedColor.ScR, colorSlider.SelectedColor.ScG, colorSlider.SelectedColor.ScB);
                if (hueColor.ToHex().Equals(hue.ToHex()))
                {
                    Console.WriteLine("Hue Set on view to: " + hue.ToHex());
                    break;
                }
            }
        }

        public SwitchView(Light light, Bridge currentBridge)
        {
            InitializeComponent();
            NameText.Opacity = 0.0;
            CheckBox_LINKS.Opacity = 0.0;

            //pbar_switch.Maximum = 250;

            // On first load, load them in a row
            this.Left = (int.Parse(light.Id) * this.Width);
            this.Top = (currentBridge.Id * this.Height);
            
            UpdateView(light);
            this.CurrentBridge = currentBridge;            

            Console.WriteLine(LSwitch.Id);
            //lSwitchID = LSwitch.Key + "_" +  currentBridge.Name;
            this.Title = LSwitch.Name.ToString();            
            NameText.Content = this.Title;            

            base_url = "http://" + currentBridge.Ip + "/api/" + currentBridge.UserId + "/";

            //AnimateProperty(pbar_switch);

            if (!CurrentBridge.SwitchSettings.Exists(s => s.Id == LSwitch.Id))
            {
                this.CurrentBridge.SwitchSettings.Add(new SwitchSetting(this, LSwitch));
            }
            else
            {
                SwitchSetting ss = CurrentBridge.SwitchSettings.Find(s => s.Id == LSwitch.Id);
                Left = ss.Left;
                Top = ss.Top;
                Height = ss.Height;
                Width = ss.Width;
                CheckBox_LINKS.IsChecked = ss.VisemeAttached;


                try
                {
                    newMin = ss.Min;
                    newMax = ss.Max;
                }
                catch
                {
                    ss.Min = newMin;
                    ss.Max = newMax;
                }
            }

            //UpdateView(light);

            // Attaches drag window event
            //MouseLeftButtonDown += DragWindow;
        }

        public SwitchView(Light light, Bridge currentBridge, ref List<SwitchView> switchWindows) : this(light, currentBridge)
        {
            this.switchWindows = switchWindows;
        }

        /// <summary>
        /// Allows to drag window
        /// </summary>
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void AnimateProperty(object sender)
        {
            UIElement uiElement = (UIElement)sender;
            DoubleAnimation ani = new DoubleAnimation(isOn ? 254 : 0, TimeSpan.FromMilliseconds(160));
            uiElement.BeginAnimation(ProgressBar.ValueProperty, ani);
        }

        public async Task<HueResults> SwitchState(bool? isOn = null, double? brightness = null, RGBColor? hue = null)
        {
            //string state = "lights/" + LSwitch.Key + "/state";
            //await LREST.Put(base_url + state, "{\"on\":" + isOn.ToString().ToLower() + ", \"transitiontime\":0}", "", ".json");
            HueResults retVal = null;

            try
            {
                var command = new LightCommand();
                command.TransitionTime = TimeSpan.FromMilliseconds(9);

                if (brightness == null)
                {
                    if (Brightness != 0)
                    {
                        command.Brightness = (byte)map(Convert.ToInt16(Brightness), 0, 100, 0, 254);
                    }
                }
                else
                {
                    command.Brightness = (byte)map(Convert.ToInt16(brightness), 0, 100, 0, 254);
                }

                try
                {
                    if (hue != null)
                    {
                        if (((RGBColor)hue).ToHex() != "000000")
                        {
                            command.SetColor(((RGBColor)hue).ToHex().Substring(2, 6));
                            command.TransitionTime = TimeSpan.FromMilliseconds(44);
                        }
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }

                if (isOn != null)
                {
                    command.On = isOn;
                }
                else
                {
                    if (brightness == 0)
                    {
                        command.On = false;
                    }
                    else
                    {
                        command.On = true;
                    }
                }
                
                retVal = await CurrentBridge.Client.SendCommandAsync(command, new List<string> { LSwitch.Id });
                if (retVal.Count > 0 && retVal[0].Success != null && !string.IsNullOrEmpty(retVal[0].Success.Id))
                {
                    // TODO: Error Control
                    //MessageBox.Show("Woot!");                
                }

                //var u = await 
                UpdateView(refresh: true);
            }
            catch (Exception error)
            {
                try
                {
                    jarvisWPF.PublicClass.MainControlCenter.TextToDebugWindow(error.Message);
                }
                catch { }
            }

            return retVal;
        }

        private async void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentBridge == null) return;
            SwitchView currentSwitchWindow = ((SwitchView)((Viewbox)((Grid)((CheckBox)sender).Parent).Parent).Parent);
            if ((bool)currentSwitchWindow.CheckBox_LINKS.IsChecked) return;

            try
            {
                double bri = Brightness;

                if (bri % 10 == 0)
                {
                    //if (bri == 0 && isOn)
                    //{
                    //    isOn = false; // lights off
                    //    await toggleSwitch(currentSwitchWindow.pbar_switch);
                    //}
                    //else if (bri > 0 && !isOn)
                    //{
                    //    isOn = true; // lights on                
                    //    await toggleSwitch(currentSwitchWindow.pbar_switch);
                    //}

                    //if (bri > 0)
                    //{
                    if (bri == 250) bri = 254;
                    //string state = "lights/" + LSwitch.Id + "/state";
                    //await LREST.Put(base_url + state, "{\"bri\":" + bri + "}", "", ".json");

                    var command = new LightCommand();
                    command.On = LSwitch.State.On;
                    //command.On = true;
                    command.Brightness = (byte)bri;
                    command.TransitionTime = TimeSpan.FromSeconds(9);

                    HueResults retVal = null;
                    retVal = await CurrentBridge.Client.SendCommandAsync(command, new List<string> { LSwitch.Id });
                    if (retVal.Count > 0 && retVal[0].Success != null && !string.IsNullOrEmpty(retVal[0].Success.Id))
                    {
                        //LSwitch.State.On =retVal[0].Success.Id

                    }
                    else if (retVal.Count == 0)
                    {
                        isOn = (bool)command.On;
                        //AnimateProperty(currentSwitchWindow.pbar_switch);
                    }

                    //}
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        public async void SwitchDimmer(object sender, int value = -1)
        {
            SwitchView currentSwitchWindow = (SwitchView)sender;
            double bri = (value * (100.0 / 21)) / 100.0 * 254;

            float X, x1, x2, y1, y2;
            X = (float)bri;
            x1 = 0;     // Range 1
            x2 = 254;   // Range 1
            y1 = newMin;    // Range 2
            y2 = newMax;   // Range 2


            int res = Convert.ToInt16( RangeConv(X, x1, x2, y1, y2));
            bri = Convert.ToInt32(res);
            Brightness = bri;

            if (bri == 0 && isOn)
            {
                //isOn = false; // lights off
                await SwitchState(false);
            }
            else if (bri > 0 && !isOn)
            {
                //isOn = true; // lights on                
                await SwitchState(true);
            }

            if (bri > 0)
            {
                if (bri == 250) bri = 254;
                string state = "lights/" + LSwitch.Id + "/state";
                await LREST.Put(base_url + state, "{\"bri\":" + bri + "}", "", ".json");
            }

            
        }
        
        private float RangeConv(float input, float x1, float x2, float y1, float y2)
        {
            return (((input - x1) * (y2 - y1)) / (x2 - x1)) + y1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SwitchSetting sS = CurrentBridge.SwitchSettings.Find(s => s.Id == LSwitch.Id);
            sS.Left = Left;
            sS.Top = Top;
            sS.Height = Height;
            sS.Width = Width;
            IsClosed = true;
        }
                
        private void Window_Activated(object sender, EventArgs e)
        {
//            //System.Diagnostics.Debugger.Break();
//            Console.WriteLine("Window Activated: " + Title);
//            Global.LINKS_Viseme = this;
//            //jarvisWPF.PublicClass.SpeechSynth.VisemeReached((x, y) => Synthesizer_UpdateSwitch(sender, y));
////#if LINKSEXISTS

//            try
//            {
//                if (jarvisWPF.PublicClass.SpeechSynth != null)
//                {
//                    jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached -= Synthesizer_UpdateSwitch;
//                    jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached += Synthesizer_UpdateSwitch;
//                    jarvisWPF.PublicClass.SpeechSynth.Synth.SpeakCompleted -= Synth_SpeakCompleted;
//                    jarvisWPF.PublicClass.SpeechSynth.Synth.SpeakCompleted += Synth_SpeakCompleted;
//                }
//            }
//            catch
//            {

//                if (System.Diagnostics.Debugger.IsAttached)
//                //    System.Diagnostics.Debugger.Break();
//                Console.WriteLine("Can not update lights when LINKS speaks.");
//            }
////#endif
        }

        private async void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Brightness = last_bri;
            last_bri = -1;
            await ChangeBrightness(Brightness);
        }

        DateTime lastLightUpdate = DateTime.Now;
        private List<SwitchView> switchWindows;
        private double last_bri = -1;

        //This method is called from LINKS so it's a seperate thread than the window
        public void Synthesizer_UpdateSwitch(object sender, VisemeReachedEventArgs e)
        {
            if (last_bri == -1)
            {
                last_bri = Brightness;
            }

            if (lastLightUpdate.AddMilliseconds(9) < DateTime.Now)
            {
                if (Global.LINKS_Viseme != null && Global.LINKS_Viseme.IsInitialized)
                {
                    if (Global.LINKS_Viseme.LSwitch.Equals(LSwitch))
                    {
                        Console.WriteLine(e.Viseme + " from links.");
                        Console.WriteLine("Synth: " + Global.LINKS_Viseme.LSwitch.Name);

                        this.Dispatcher.BeginInvoke(new Action ( delegate
                        {
                           SwitchDimmer(this, e.Viseme);
                        }));

                        lastLightUpdate = DateTime.Now;
                    }
                }
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            //jarvisWPF.PublicClass.SpeechSynth.VisemeReached((x, y) => Synthesizer_UpdateSwitch(sender, y), false);
            //jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached -= Synthesizer_UpdateSwitch;
        }

        private void checkBox_LINKS_Checked(object sender, RoutedEventArgs e)
        {
            if (switchWindows == null) return;

            string currentSwitchId = LSwitch.Id;

            List<SwitchView> switches = 
                switchWindows.FindAll(windows => (bool)windows.CheckBox_LINKS.IsChecked &&
                                    windows.LSwitch.Id != currentSwitchId);

            //    .ForEach(sW => sW.CheckBox_LINKS.IsChecked = false);

            if (switches != null)
            {
                CurrentBridge.SwitchSettings
                    .FindAll(s => s.VisemeAttached)
                    .ForEach(s => s.VisemeAttached = false);
            }

            CurrentBridge.SwitchSettings
                .Find(s => s.Id == currentSwitchId).VisemeAttached = true;

            //SwitchView currentSwitchWindow = ((SwitchView)((Viewbox)((Grid)((CheckBox)sender).Parent).Parent).Parent);
            //currentSwitchWindow.slider.IsEnabled = false;

            //System.Diagnostics.Debugger.Break();
            Console.WriteLine("Window Attached to VISEME: " + Title);
            Global.LINKS_Viseme = this;
            //jarvisWPF.PublicClass.SpeechSynth.VisemeReached((x, y) => Synthesizer_UpdateSwitch(sender, y));
            //#if LINKSEXISTS

            try
            {
                if (jarvisWPF.PublicClass.SpeechSynth != null)
                {
                    jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached -= Synthesizer_UpdateSwitch;
                    jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached += Synthesizer_UpdateSwitch;
                    jarvisWPF.PublicClass.SpeechSynth.Synth.SpeakCompleted -= Synth_SpeakCompleted;
                    jarvisWPF.PublicClass.SpeechSynth.Synth.SpeakCompleted += Synth_SpeakCompleted;
                }
            }
            catch
            {

                if (System.Diagnostics.Debugger.IsAttached)
                    //    System.Diagnostics.Debugger.Break();
                    Console.WriteLine("Can not update lights when LINKS speaks.");
            }
            //#endif
        }

        private void checkBox_LINKS_Unchecked(object sender, RoutedEventArgs e)
        {
            CurrentBridge.SwitchSettings.Find(s => s.Id == LSwitch.Id).VisemeAttached = false;
            //((Test_WPFWindow)this.Parent).config.Save();

            try
            {
                if (jarvisWPF.PublicClass.SpeechSynth != null)
                {
                    jarvisWPF.PublicClass.SpeechSynth.Synth.VisemeReached -= Synthesizer_UpdateSwitch;
                    jarvisWPF.PublicClass.SpeechSynth.Synth.SpeakCompleted -= Synth_SpeakCompleted;
                }
            }
            catch
            {

                if (System.Diagnostics.Debugger.IsAttached)
                    //    System.Diagnostics.Debugger.Break();
                    Console.WriteLine("Can not update lights when LINKS speaks.");
            }

            Global.LINKS_Viseme = null;

        }

        private bool isOn;
        private double brightness = 100.0;        

        /// <summary>
        /// GETS CURRENT BULB VIEW COLOR
        /// </summary>
        public Color Color
        {
            get
            {
                return GetColor();
            }
        }

        /// <summary>
        /// GETS/SETS CURRENT BULB isOn VIEW STATE
        /// </summary>
        public bool IsOn
        {
            get
            {
                return isOn;
            }

            set
            {
                isOn = value;
                if (isOn)
                {
                    Storyboard sb = (Storyboard)TryFindResource("On");                    
                    ((DoubleAnimationUsingKeyFrames)(sb.Children.First(c => c.Name == "OnBrightness")))
                        .KeyFrames[0].Value = Brightness / 100.0;
                    sb.Begin();
                }
                else
                {
                    Storyboard sb = (Storyboard)TryFindResource("Off");
                    ((DoubleAnimationUsingKeyFrames)(sb.Children.First(c => c.Name == "OffBrightness")))
                        .KeyFrames[0].Value = 0.0;
                    sb.Begin();
                }
            }
        }

        /// <summary>
        /// GETS/SETS CURRENT BULB VIEW BRIGHTNESS
        /// </summary>
        public double Brightness
        {
            get
            {
                return brightness;
            }

            set
            {
                if (value != brightness)
                {
                    brightness = value;
                    Console.WriteLine("Brightness changed to : " + brightness);

                    //double newOpacity = (brightness / 100.0);
                    //DoubleAnimation animation = new DoubleAnimation(newOpacity, TimeSpan.FromMilliseconds(40));
                    //BulbEmitingLight.BeginAnimation(OpacityProperty, animation);
                }
            }
        }


#region Private Methods

        private async Task<HueResults> TurnOff()
        {
            HueResults retVal = null;
            if (IsOn)
            {
                retVal = await SwitchState(false);
            }

            return retVal;
        }

        public async Task<HueResults> TurnOn()
        {
            HueResults retVal = null;
            if (!IsOn)
            {
                retVal = await SwitchState(true);
            }
            return retVal;
        }

        private async Task<HueResults> ChangeBrightness(double brightness = -1)
        {
            double b = 0;
            HueResults retVal = null;

            if (brightness != -1)
            {
                b = brightness;
            }
            else
            {
                b = this.Brightness;
            }
             
            if (brightness == 0)
            {
                retVal = await TurnOff();
            }
            else
            {
                retVal = await SwitchState(brightness: b);
            }

            return retVal;
        }

        

        private async void window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double lastBulbEmitingLightOpacity = Brightness;
            double b = e.Delta / 24;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                

                if (e.Delta > 0)
                {
                    if (this.Height < 254)
                    {
                        this.Width += 4;
                        this.Height += 4;
                    }
                }
                else
                {
                    if (this.Height > 56)
                    {
                        this.Width -= 4;
                        this.Height -= 4;
                    }
                }
            }
            else
            {
                if (IsOn)
                {
                    if (b > 0)
                    {
                        if (Brightness < 100)
                        {
                            if (Brightness + b > 100)
                            {
                                Brightness = 100.0;                                
                            }
                            else
                            {
                                Brightness += b;
                            }
                        }
                    }
                    else
                    {
                        if (Brightness > 0)
                        {
                            if (Brightness - b < 0)
                            {
                                Brightness = 0.0;
                            }
                            else
                            {
                                Brightness -= b;
                            }
                        }
                    }

                    if (lastBulbEmitingLightOpacity != Brightness)
                    {
                        var x = await ChangeBrightness();                        
                    }
                }
            }

        }

        private void DragablePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async Task<HueResults> ChangeHue()
        {
            HueResults retVal = null;

            //colorSlider.SelectedColor
            
            RGBColor hue = new RGBColor(colorSlider.SelectedColor.R, colorSlider.SelectedColor.G, colorSlider.SelectedColor.B);
            retVal = await SwitchState(hue: hue);

            return retVal;
        }

        private async void MainGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            await MouseHue();
        }

        private async Task<HueResults> MouseHue()
        {
            HueResults retVal = null;
            if (IsOn && Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                float lHue = 0;
                float mX = 0;
                //mX = System.Windows.Forms.Control.MousePosition.X; // gets mouse position on screen
                Point position = Mouse.GetPosition(MainGrid);
                mX = (float)position.X;
                Console.WriteLine(mX);
                Console.WriteLine("MarginLeft: " + (float)MainGrid.Margin.Left);
                Console.WriteLine("MarginRight: " + ((float)MainGrid.Width + (float)MainGrid.Margin.Left));
                lHue = map(mX, (float)MainGrid.Margin.Left,
                                 (float)MainGrid.Width - (float)BulbEmitingLight.Margin.Right,
                                 10, 1000);

                Console.WriteLine(lHue);
                Console.WriteLine();
                colorSlider.Value = lHue;

                retVal = await ChangeHue();
            }
            return retVal;
        }

        private float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void Bulb_IsOnChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // toggle isOn
            // and change brightness
            //if (IsOn != LSwitch.State.On)
            //{
            //    //isOn = false;
            //    await SwitchState(false);
            //}
            //else
            //{
            //    //isOn = true;
            //    brightness = Convert.ToInt16(BulbEmitingLight.Opacity * 100);                
            //    await SwitchState(true, brightness);
            //}
        }

        private async void Bulb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            await TurnOff();
        }

        private async void Bulb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await TurnOn();
        }

        private async void window_KeyUp(object sender, KeyEventArgs e)
        {
            bool changeHue = false;
            RGBColor lastHue = new RGBColor(colorSlider.SelectedColor.R, colorSlider.SelectedColor.G, colorSlider.SelectedColor.B);
            if (e.Key == Key.Right)
            {
                if (colorSlider.Value < 1000)
                {
                    if (colorSlider.Value + 10 > 1000)
                    {
                        colorSlider.Value = 1000;
                    }
                    else
                    {
                        colorSlider.Value += 10;
                    }
                }
                changeHue = true;
            }
            else if (e.Key == Key.Left)
            {
                if (colorSlider.Value > 0)
                {
                    if (colorSlider.Value - 10 > 1000)
                    {
                        colorSlider.Value = 0;
                    }
                    else
                    {
                        colorSlider.Value -= 10;
                    }
                }
                changeHue = true;
            }

            if (changeHue)
            {
                RGBColor hue = new RGBColor(colorSlider.SelectedColor.R, colorSlider.SelectedColor.G, colorSlider.SelectedColor.B);
                if (!lastHue.Equals(hue) && hue.ToHex() != "000000")
                {
                    await ChangeHue();
                }
            }
        }

        private Color GetColor()
        {
            return colorSlider.SelectedColor;
        }

#endregion Private Methods
    }
}
