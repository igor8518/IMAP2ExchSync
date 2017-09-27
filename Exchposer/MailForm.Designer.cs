namespace IMAP2ExchSync
{
    partial class MailsForm
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
            this.components = new System.ComponentModel.Container();
            this.MonitoringGrid = new System.Windows.Forms.DataGridView();
            this.MonitoringBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // MonitoringGrid
            // 
            this.MonitoringGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MonitoringGrid.Location = new System.Drawing.Point(12, 12);
            this.MonitoringGrid.Name = "MonitoringGrid";
            this.MonitoringGrid.Size = new System.Drawing.Size(984, 343);
            this.MonitoringGrid.TabIndex = 0;
            this.MonitoringGrid.VirtualMode = true;
            // 
            // MailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 408);
            this.Controls.Add(this.MonitoringGrid);
            this.Name = "MailsForm";
            this.Text = "Mails";
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView MonitoringGrid;
        private System.Windows.Forms.BindingSource MonitoringBindingSource;
    }
}