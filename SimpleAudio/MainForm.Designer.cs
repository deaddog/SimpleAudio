namespace SimpleAudio
{
    partial class MainForm
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
            this.managerControl1 = new SimpleAudio.ManagerControl();
            this.SuspendLayout();
            // 
            // managerControl1
            // 
            this.managerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.managerControl1.Location = new System.Drawing.Point(90, 52);
            this.managerControl1.Name = "ManagerControl";
            this.managerControl1.Size = new System.Drawing.Size(75, 23);
            this.managerControl1.TabIndex = 0;
            this.managerControl1.Text = "ManagerControl";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 500);
            this.Controls.Add(this.managerControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private ManagerControl managerControl1;

    }
}