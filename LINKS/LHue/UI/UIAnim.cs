using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace LHue
{
    class UIAnimations
    {
        public enum FadeType
        {
            In,
            Out
        }

        public void Fade(object[] sender, FadeType type, double time = 0)
        {
            if (type == FadeType.In)
            {
                time = time == 0 ? 1 : time;
            }
            else
            {
                time = time == 0 ? .1 : time;
            }

            DoubleAnimation fadeAnimation = new DoubleAnimation(time, TimeSpan.FromMilliseconds(100));
            for (int i = 0; i < sender.Length; i++)
            {
                try
                {
                    ((UIElement)sender[i]).BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
