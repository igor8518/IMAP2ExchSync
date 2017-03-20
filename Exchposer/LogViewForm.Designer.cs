namespace IMAP2ExchSync
{
    partial class LogViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewForm));
            this.txtLogView = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtLogView
            // 
            this.txtLogView.BackColor = System.Drawing.SystemColors.Window;
            this.txtLogView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogView.Location = new System.Drawing.Point(0, 0);
            this.txtLogView.Multiline = true;
            this.txtLogView.Name = "txtLogView";
            this.txtLogView.ReadOnly = true;
            this.txtLogView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogView.Size = new System.Drawing.Size(792, 568);
            this.txtLogView.TabIndex = 0;
            this.txtLogView.TabStop = false;
            // 
            // LogViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 568);
            this.Controls.Add(this.txtLogView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log";
            this.Shown += new System.EventHandler(this.LogViewForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogView;
    }
}