using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace uAuth2
{
    public partial class WPFWindow : Window
    {
        internal string _windowTitle;
        bool centered = false;

        private void Center()
        {
            if (!centered)
            {
                System.Windows.Forms.Screen scr = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
                WindowInteropHelper helper = new WindowInteropHelper(this);
                helper.Owner = new WindowInteropHelper(this).Handle; // Get hWnd for non-WPF window
                HwndSource source = HwndSource.FromHwnd(helper.Handle);
                Matrix matrix = source.CompositionTarget.TransformFromDevice;
                Point ownerWPFPosition = matrix.Transform(new Point(scr.Bounds.Width, scr.Bounds.Height));
                this.Left = (ownerWPFPosition.X / 2) - ((this.Width) / 2);
                this.Top = (ownerWPFPosition.Y / 2) - ((this.Height) / 2);
                this.Activate();
                centered = true;
            }
        }

    }
}
