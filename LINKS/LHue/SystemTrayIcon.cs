using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LHue
{
    public partial class Test_WPFWindow : Window
    {
        /// <summary>
        /// Setup all context menu items and the events that follow
        /// </summary>     
        #region initialize context menu

        public static NotifyIcon notifyIcon = null;
        ToolStripMenuItem ToolStripMenuItem_ActivateDeactivateMainWindow;
        private MainWindowState windowState = MainWindowState.Visible;

        private MainWindowState WindowState1
        {
            get
            {
                return windowState;
            }

            set
            {
                windowState = value;
                if (WindowState1 == MainWindowState.Visible)
                {
                    ToolStripMenuItem_ActivateDeactivateMainWindow.Text = "&Hide Settings";
                }
                else
                {
                    ToolStripMenuItem_ActivateDeactivateMainWindow.Text = "&Show Settings";
                }
            }
        }

        enum MainWindowState
        {
            Visible,
            Hidden
        }

        private void InitializeContextMenu()
        {
            try
            {
                System.ComponentModel.Container components = new System.ComponentModel.Container();

                notifyIcon = new NotifyIcon(components)
                {
                    ContextMenuStrip = new ContextMenuStrip(),
                    Icon = Properties.Resources.TrayIcon, //, new System.Drawing.Icon(IconFileName),
                    Text = "Minimalistic - Hue",
                    Visible = true
                };


                ToolStripMenuItem_ActivateDeactivateMainWindow = new ToolStripMenuItem("&Hide Settings", null, null, "ActivateMainWindow");
                ToolStripMenuItem_ActivateDeactivateMainWindow.Click += ToolStripMenuItem_ActivateMainWindow_Click;
                notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItem_ActivateDeactivateMainWindow);

                notifyIcon.ContextMenuStrip.Items.Add("-");

                ToolStripMenuItem ToolStripMenuItem_Help = new ToolStripMenuItem("&Help", null, null, "Help");
                ToolStripMenuItem_Help.Enabled = false;
                ToolStripMenuItem_Help.Click += ToolStripMenuItem_Help_Click;
                notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItem_Help);

                ToolStripMenuItem ToolStripMenuItem_About = new ToolStripMenuItem("&About", null, null, "About");
                ToolStripMenuItem_About.Enabled = false;
                ToolStripMenuItem_About.Click += ToolStripMenuItem_About_Click;
                notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItem_About);

                notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem ToolStripMenuItem_Exit = new ToolStripMenuItem("&Exit", null, null, "Exit");
                ToolStripMenuItem_Exit.Click += ToolStripMenuItem_Exit_Click;
                notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItem_Exit);
            }
            catch (Exception error)
            {
                //PublicClass.ErrorLog(error);
            }
        }

        public void TrayIcon_AttachEvents()
        {
            if (notifyIcon != null)
            {
                notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
                notifyIcon.DoubleClick += notifyIcon_DoubleClick;
                notifyIcon.MouseUp += notifyIcon_MouseUp;
            }
        }

        public void TrayIcon_RemoveEvents()
        {   
            if (notifyIcon != null)
            {
                notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
                notifyIcon.DoubleClick += notifyIcon_DoubleClick;
                notifyIcon.MouseUp += notifyIcon_MouseUp;
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = false;
            }
            catch (Exception error)
            {
                //PublicClass.ErrorLog(error);
            }
        }

        

        private void ToolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            //PublicClass.GotoWebsite("http://mega-voice-command.com/index.html");
        }

        private void ToolStripMenuItem_Help_Click(object sender, EventArgs e)
        {
            //PublicClass.GotoWebsite("http://forum.ai-dot.net/viewtopic.php?f=6&t=1257");
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            config.Save();
            Close();
            //System.Windows.Application.Current.Shutdown();
        }

        private void ToolStripMenuItem_ActivateMainWindow_Click(object sender, EventArgs e)
        {
            if (WindowState1 == MainWindowState.Visible)
            {
                WindowState1 = MainWindowState.Hidden;
                Hide();
            }
            else
            {
                WindowState1 = MainWindowState.Visible;               
                Show();
            }
            
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState1 == MainWindowState.Visible)
            {
                Activate();
            }
            else
            {
                WindowState1 = MainWindowState.Visible;
                Show();
            }
        }

        void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }
        #endregion

        private void button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState1 = MainWindowState.Hidden;
            Hide();
        }
    }
}
