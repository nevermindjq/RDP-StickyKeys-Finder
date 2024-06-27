namespace App
{
    partial class Form1
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.rdp = new AxMSTSCLib.AxMsRdpClient8NotSafeForScripting();
            this.btn_pick = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.rdp)).BeginInit();
            this.SuspendLayout();
            // 
            // rdp
            // 
            this.rdp.Enabled = true;
            this.rdp.Location = new System.Drawing.Point(12, 12);
            this.rdp.Name = "rdp";
            this.rdp.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdp.OcxState")));
            this.rdp.Size = new System.Drawing.Size(776, 426);
            this.rdp.TabIndex = 0;
            // 
            // btn_pick
            // 
            this.btn_pick.Location = new System.Drawing.Point(794, 12);
            this.btn_pick.Name = "btn_pick";
            this.btn_pick.Size = new System.Drawing.Size(94, 23);
            this.btn_pick.TabIndex = 1;
            this.btn_pick.Text = "Select";
            this.btn_pick.UseVisualStyleBackColor = true;
            this.btn_pick.Click += new System.EventHandler(this.btn_pick_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(794, 41);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(94, 23);
            this.btn_cancel.TabIndex = 2;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 450);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_pick);
            this.Controls.Add(this.rdp);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.rdp)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btn_cancel;

        private System.Windows.Forms.Button btn_pick;

        private AxMSTSCLib.AxMsRdpClient8NotSafeForScripting rdp;

        #endregion
    }
}