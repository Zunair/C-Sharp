namespace LSamplePlugin
{
    partial class Form_Test
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.test_label = new System.Windows.Forms.Label();
            this.test_textBox_Speak = new System.Windows.Forms.TextBox();
            this.test_button_Speak = new System.Windows.Forms.Button();
            this.test_button_Emiulate = new System.Windows.Forms.Button();
            this.test_textBox_Emulate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // test_label
            // 
            this.test_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.test_label.AutoSize = true;
            this.test_label.Location = new System.Drawing.Point(124, 9);
            this.test_label.Name = "test_label";
            this.test_label.Size = new System.Drawing.Size(28, 13);
            this.test_label.TabIndex = 0;
            this.test_label.Text = "Test";
            this.test_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // test_textBox_Speak
            // 
            this.test_textBox_Speak.Location = new System.Drawing.Point(12, 32);
            this.test_textBox_Speak.Name = "test_textBox_Speak";
            this.test_textBox_Speak.Size = new System.Drawing.Size(185, 20);
            this.test_textBox_Speak.TabIndex = 1;
            // 
            // test_button_Speak
            // 
            this.test_button_Speak.Location = new System.Drawing.Point(203, 32);
            this.test_button_Speak.Name = "test_button_Speak";
            this.test_button_Speak.Size = new System.Drawing.Size(75, 20);
            this.test_button_Speak.TabIndex = 2;
            this.test_button_Speak.Text = "Speak";
            this.test_button_Speak.UseVisualStyleBackColor = true;
            this.test_button_Speak.Click += new System.EventHandler(this.test_button_Click);
            // 
            // test_button_Emiulate
            // 
            this.test_button_Emiulate.Location = new System.Drawing.Point(203, 58);
            this.test_button_Emiulate.Name = "test_button_Emiulate";
            this.test_button_Emiulate.Size = new System.Drawing.Size(75, 20);
            this.test_button_Emiulate.TabIndex = 4;
            this.test_button_Emiulate.Text = "Emulate";
            this.test_button_Emiulate.UseVisualStyleBackColor = true;
            this.test_button_Emiulate.Click += new System.EventHandler(this.test_button_Emiulate_Click);
            // 
            // test_textBox_Emulate
            // 
            this.test_textBox_Emulate.Location = new System.Drawing.Point(12, 58);
            this.test_textBox_Emulate.Name = "test_textBox_Emulate";
            this.test_textBox_Emulate.Size = new System.Drawing.Size(185, 20);
            this.test_textBox_Emulate.TabIndex = 3;
            // 
            // Form_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(290, 86);
            this.Controls.Add(this.test_button_Emiulate);
            this.Controls.Add(this.test_textBox_Emulate);
            this.Controls.Add(this.test_button_Speak);
            this.Controls.Add(this.test_textBox_Speak);
            this.Controls.Add(this.test_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form_Test";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Plugin Form Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label test_label;
        private System.Windows.Forms.TextBox test_textBox_Speak;
        private System.Windows.Forms.Button test_button_Speak;
        private System.Windows.Forms.Button test_button_Emiulate;
        private System.Windows.Forms.TextBox test_textBox_Emulate;
    }
}