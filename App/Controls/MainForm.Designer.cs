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
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(184, 151);
			this.Controls.Add(this.lbl_Threads);
			this.Controls.Add(this.box_Threads);
			this.Controls.Add(this.btn_Select);
			this.Controls.Add(this.btn_Cancel);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Select;
		private System.Windows.Forms.TextBox box_Threads;
		private System.Windows.Forms.Label lbl_Threads;

		#endregion
	}
}