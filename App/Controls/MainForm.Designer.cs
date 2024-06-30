using System.ComponentModel;

namespace App.Controls {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_Select = new System.Windows.Forms.Button();
			this.box_Threads = new System.Windows.Forms.TextBox();
			this.lbl_Threads = new System.Windows.Forms.Label();
			this.lbl_Connection = new System.Windows.Forms.Label();
			this.box_ConnectionTimeout = new System.Windows.Forms.TextBox();
			this.box_CertificateWarningDelay = new System.Windows.Forms.TextBox();
			this.lbl_CertificateWarning = new System.Windows.Forms.Label();
			this.box_LoadingDelay = new System.Windows.Forms.TextBox();
			this.lbl_Loading = new System.Windows.Forms.Label();
			this.box_StickyKeysWarningDelay = new System.Windows.Forms.TextBox();
			this.lbl_StickyKeysWarning = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Enabled = false;
			this.btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_Cancel.Location = new System.Drawing.Point(12, 109);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(160, 30);
			this.btn_Cancel.TabIndex = 0;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
			// 
			// btn_Select
			// 
			this.btn_Select.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_Select.Location = new System.Drawing.Point(12, 73);
			this.btn_Select.Name = "btn_Select";
			this.btn_Select.Size = new System.Drawing.Size(160, 30);
			this.btn_Select.TabIndex = 1;
			this.btn_Select.Text = "Select";
			this.btn_Select.UseVisualStyleBackColor = true;
			this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
			// 
			// box_Threads
			// 
			this.box_Threads.Location = new System.Drawing.Point(12, 47);
			this.box_Threads.Name = "box_Threads";
			this.box_Threads.Size = new System.Drawing.Size(160, 20);
			this.box_Threads.TabIndex = 2;
			// 
			// lbl_Threads
			// 
			this.lbl_Threads.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Threads.Location = new System.Drawing.Point(12, 9);
			this.lbl_Threads.Name = "lbl_Threads";
			this.lbl_Threads.Size = new System.Drawing.Size(160, 35);
			this.lbl_Threads.TabIndex = 3;
			this.lbl_Threads.Text = "Threads";
			this.lbl_Threads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_Connection
			// 
			this.lbl_Connection.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Connection.Location = new System.Drawing.Point(178, 19);
			this.lbl_Connection.Name = "lbl_Connection";
			this.lbl_Connection.Size = new System.Drawing.Size(160, 25);
			this.lbl_Connection.TabIndex = 4;
			this.lbl_Connection.Text = "Connection";
			this.lbl_Connection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// box_ConnectionTimeout
			// 
			this.box_ConnectionTimeout.Location = new System.Drawing.Point(178, 47);
			this.box_ConnectionTimeout.Name = "box_ConnectionTimeout";
			this.box_ConnectionTimeout.Size = new System.Drawing.Size(160, 20);
			this.box_ConnectionTimeout.TabIndex = 5;
			// 
			// box_CertificateWarningDelay
			// 
			this.box_CertificateWarningDelay.Enabled = false;
			this.box_CertificateWarningDelay.Location = new System.Drawing.Point(344, 107);
			this.box_CertificateWarningDelay.Name = "box_CertificateWarningDelay";
			this.box_CertificateWarningDelay.Size = new System.Drawing.Size(160, 20);
			this.box_CertificateWarningDelay.TabIndex = 7;
			this.box_CertificateWarningDelay.Visible = false;
			// 
			// lbl_CertificateWarning
			// 
			this.lbl_CertificateWarning.Enabled = false;
			this.lbl_CertificateWarning.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_CertificateWarning.Location = new System.Drawing.Point(344, 76);
			this.lbl_CertificateWarning.Name = "lbl_CertificateWarning";
			this.lbl_CertificateWarning.Size = new System.Drawing.Size(160, 25);
			this.lbl_CertificateWarning.TabIndex = 6;
			this.lbl_CertificateWarning.Text = "Certificate Warning";
			this.lbl_CertificateWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbl_CertificateWarning.Visible = false;
			// 
			// box_LoadingDelay
			// 
			this.box_LoadingDelay.Location = new System.Drawing.Point(344, 47);
			this.box_LoadingDelay.Name = "box_LoadingDelay";
			this.box_LoadingDelay.Size = new System.Drawing.Size(160, 20);
			this.box_LoadingDelay.TabIndex = 9;
			// 
			// lbl_Loading
			// 
			this.lbl_Loading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Loading.Location = new System.Drawing.Point(344, 19);
			this.lbl_Loading.Name = "lbl_Loading";
			this.lbl_Loading.Size = new System.Drawing.Size(160, 25);
			this.lbl_Loading.TabIndex = 8;
			this.lbl_Loading.Text = "Loading";
			this.lbl_Loading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// box_StickyKeysWarningDelay
			// 
			this.box_StickyKeysWarningDelay.Location = new System.Drawing.Point(178, 109);
			this.box_StickyKeysWarningDelay.Name = "box_StickyKeysWarningDelay";
			this.box_StickyKeysWarningDelay.Size = new System.Drawing.Size(160, 20);
			this.box_StickyKeysWarningDelay.TabIndex = 11;
			// 
			// lbl_StickyKeysWarning
			// 
			this.lbl_StickyKeysWarning.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_StickyKeysWarning.Location = new System.Drawing.Point(178, 76);
			this.lbl_StickyKeysWarning.Name = "lbl_StickyKeysWarning";
			this.lbl_StickyKeysWarning.Size = new System.Drawing.Size(160, 25);
			this.lbl_StickyKeysWarning.TabIndex = 10;
			this.lbl_StickyKeysWarning.Text = "Sticky Keys Warning";
			this.lbl_StickyKeysWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 151);
			this.Controls.Add(this.box_StickyKeysWarningDelay);
			this.Controls.Add(this.lbl_StickyKeysWarning);
			this.Controls.Add(this.box_LoadingDelay);
			this.Controls.Add(this.lbl_Loading);
			this.Controls.Add(this.box_CertificateWarningDelay);
			this.Controls.Add(this.lbl_CertificateWarning);
			this.Controls.Add(this.box_ConnectionTimeout);
			this.Controls.Add(this.lbl_Connection);
			this.Controls.Add(this.lbl_Threads);
			this.Controls.Add(this.box_Threads);
			this.Controls.Add(this.btn_Select);
			this.Controls.Add(this.btn_Cancel);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.TextBox box_CertificateWarningDelay;
		private System.Windows.Forms.Label lbl_CertificateWarning;
		private System.Windows.Forms.TextBox box_LoadingDelay;
		private System.Windows.Forms.Label lbl_Loading;
		private System.Windows.Forms.TextBox box_StickyKeysWarningDelay;
		private System.Windows.Forms.Label lbl_StickyKeysWarning;

		private System.Windows.Forms.TextBox box_ConnectionTimeout;

		private System.Windows.Forms.Label lbl_Connection;

		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Select;
		private System.Windows.Forms.TextBox box_Threads;
		private System.Windows.Forms.Label lbl_Threads;

		#endregion
	}
}