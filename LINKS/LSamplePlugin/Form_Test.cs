using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSamplePlugin
{
    public partial class Form_Test : Form
    {
        public Form_Test()
        {
            InitializeComponent();
        }

        private void test_button_Click(object sender, EventArgs e)
        {
            jarvisWPF.PublicClass.SpeechSynth.SpeakRandomPhrase(test_textBox_Speak.Text);
        }

        private void test_button_Emiulate_Click(object sender, EventArgs e)
        {
            jarvisWPF.Classes.Plugins.PluginController.EmulateSpeech(test_textBox_Emulate.Text);
        }
    }
}
