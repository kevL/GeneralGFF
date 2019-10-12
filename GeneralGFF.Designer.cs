﻿using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	partial class GeneralGFF
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		SplitContainerCp sc_body;

		internal Label la_Description;
		internal TextBox tb_Val;
		internal Label la_Val;
		internal CheckBox cb_GenderF;
		internal Label la_GenderF;
		Button btn_Apply;
		Button btn_Revert;


		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.sc_body = new generalgff.SplitContainerCp();
			this.la_GenderF = new System.Windows.Forms.Label();
			this.btn_Revert = new System.Windows.Forms.Button();
			this.btn_Apply = new System.Windows.Forms.Button();
			this.la_Description = new System.Windows.Forms.Label();
			this.la_Val = new System.Windows.Forms.Label();
			this.tb_Val = new System.Windows.Forms.TextBox();
			this.cb_GenderF = new System.Windows.Forms.CheckBox();
			this.sc_body.Panel2.SuspendLayout();
			this.sc_body.SuspendLayout();
			this.SuspendLayout();
			// 
			// sc_body
			// 
			this.sc_body.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sc_body.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sc_body.Location = new System.Drawing.Point(0, 0);
			this.sc_body.Margin = new System.Windows.Forms.Padding(0);
			this.sc_body.Name = "sc_body";
			this.sc_body.Panel1MinSize = 450;
			// 
			// sc_body.Panel2
			// 
			this.sc_body.Panel2.BackColor = System.Drawing.Color.Linen;
			this.sc_body.Panel2.Controls.Add(this.la_GenderF);
			this.sc_body.Panel2.Controls.Add(this.btn_Revert);
			this.sc_body.Panel2.Controls.Add(this.btn_Apply);
			this.sc_body.Panel2.Controls.Add(this.la_Description);
			this.sc_body.Panel2.Controls.Add(this.la_Val);
			this.sc_body.Panel2.Controls.Add(this.tb_Val);
			this.sc_body.Panel2.Controls.Add(this.cb_GenderF);
			this.sc_body.Panel2MinSize = 0;
			this.sc_body.Size = new System.Drawing.Size(792, 574);
			this.sc_body.SplitterDistance = 450;
			this.sc_body.SplitterWidth = 2;
			this.sc_body.TabIndex = 0;
			// 
			// la_GenderF
			// 
			this.la_GenderF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_GenderF.Location = new System.Drawing.Point(25, 80);
			this.la_GenderF.Margin = new System.Windows.Forms.Padding(0);
			this.la_GenderF.Name = "la_GenderF";
			this.la_GenderF.Size = new System.Drawing.Size(310, 17);
			this.la_GenderF.TabIndex = 6;
			this.la_GenderF.Text = "Feminine";
			this.la_GenderF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_GenderF.Visible = false;
			// 
			// btn_Revert
			// 
			this.btn_Revert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Revert.Enabled = false;
			this.btn_Revert.Location = new System.Drawing.Point(280, 55);
			this.btn_Revert.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Revert.Name = "btn_Revert";
			this.btn_Revert.Size = new System.Drawing.Size(57, 20);
			this.btn_Revert.TabIndex = 5;
			this.btn_Revert.Text = "revert";
			this.btn_Revert.UseVisualStyleBackColor = true;
			this.btn_Revert.Click += new System.EventHandler(this.click_Revert);
			// 
			// btn_Apply
			// 
			this.btn_Apply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Apply.Enabled = false;
			this.btn_Apply.Location = new System.Drawing.Point(1, 100);
			this.btn_Apply.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Apply.Name = "btn_Apply";
			this.btn_Apply.Size = new System.Drawing.Size(336, 25);
			this.btn_Apply.TabIndex = 4;
			this.btn_Apply.Text = "APPLY";
			this.btn_Apply.UseVisualStyleBackColor = true;
			this.btn_Apply.Click += new System.EventHandler(this.click_Apply);
			// 
			// la_Description
			// 
			this.la_Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_Description.Location = new System.Drawing.Point(5, 3);
			this.la_Description.Margin = new System.Windows.Forms.Padding(0);
			this.la_Description.Name = "la_Description";
			this.la_Description.Size = new System.Drawing.Size(332, 29);
			this.la_Description.TabIndex = 0;
			this.la_Description.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// la_Val
			// 
			this.la_Val.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_Val.Location = new System.Drawing.Point(5, 55);
			this.la_Val.Margin = new System.Windows.Forms.Padding(0);
			this.la_Val.Name = "la_Val";
			this.la_Val.Size = new System.Drawing.Size(275, 20);
			this.la_Val.TabIndex = 2;
			this.la_Val.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tb_Val
			// 
			this.tb_Val.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Val.BackColor = System.Drawing.Color.Violet;
			this.tb_Val.Enabled = false;
			this.tb_Val.Location = new System.Drawing.Point(2, 36);
			this.tb_Val.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Val.Name = "tb_Val";
			this.tb_Val.Size = new System.Drawing.Size(334, 20);
			this.tb_Val.TabIndex = 1;
			this.tb_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Val.WordWrap = false;
			this.tb_Val.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keydown_Val);
			// 
			// cb_GenderF
			// 
			this.cb_GenderF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cb_GenderF.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.cb_GenderF.Location = new System.Drawing.Point(8, 79);
			this.cb_GenderF.Margin = new System.Windows.Forms.Padding(0);
			this.cb_GenderF.Name = "cb_GenderF";
			this.cb_GenderF.Size = new System.Drawing.Size(17, 17);
			this.cb_GenderF.TabIndex = 3;
			this.cb_GenderF.UseVisualStyleBackColor = true;
			this.cb_GenderF.Visible = false;
			// 
			// GeneralGFF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 574);
			this.Controls.Add(this.sc_body);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "GeneralGFF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GeneralGFF";
			this.sc_body.Panel2.ResumeLayout(false);
			this.sc_body.Panel2.PerformLayout();
			this.sc_body.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}



	/// <summary>
	/// A derived SplitContainer that prevents flicker.
	/// </summary>
	sealed class SplitContainerCp : SplitContainer
	{
		/// <summary>
		/// Prevents flicker.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
	}
}
