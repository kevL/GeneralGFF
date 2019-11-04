using System;
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

		internal Label la_Des;
		internal Label la_Val;
		internal TextBox tb_Val;
		internal TextBox rt_Val;
		internal CheckBox cb_Custo;
		internal CheckBox cb_Wordwrap;

		Button btn_Apply;
		Button btn_Revert;

		StatusStrip ss_bot;
		internal ToolStripStatusLabel tssl_info;


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
			this.rt_Val = new System.Windows.Forms.TextBox();
			this.cb_Wordwrap = new System.Windows.Forms.CheckBox();
			this.btn_Apply = new System.Windows.Forms.Button();
			this.cb_Custo = new System.Windows.Forms.CheckBox();
			this.btn_Revert = new System.Windows.Forms.Button();
			this.la_Val = new System.Windows.Forms.Label();
			this.tb_Val = new System.Windows.Forms.TextBox();
			this.la_Des = new System.Windows.Forms.Label();
			this.ss_bot = new System.Windows.Forms.StatusStrip();
			this.tssl_info = new System.Windows.Forms.ToolStripStatusLabel();
			this.sc_body.Panel2.SuspendLayout();
			this.sc_body.SuspendLayout();
			this.ss_bot.SuspendLayout();
			this.SuspendLayout();
			// 
			// sc_body
			// 
			this.sc_body.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sc_body.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sc_body.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.sc_body.Location = new System.Drawing.Point(0, 0);
			this.sc_body.Margin = new System.Windows.Forms.Padding(0);
			this.sc_body.Name = "sc_body";
			this.sc_body.Panel1MinSize = 450;
			// 
			// sc_body.Panel2
			// 
			this.sc_body.Panel2.BackColor = System.Drawing.Color.Linen;
			this.sc_body.Panel2.Controls.Add(this.rt_Val);
			this.sc_body.Panel2.Controls.Add(this.cb_Wordwrap);
			this.sc_body.Panel2.Controls.Add(this.btn_Apply);
			this.sc_body.Panel2.Controls.Add(this.cb_Custo);
			this.sc_body.Panel2.Controls.Add(this.btn_Revert);
			this.sc_body.Panel2.Controls.Add(this.la_Val);
			this.sc_body.Panel2.Controls.Add(this.tb_Val);
			this.sc_body.Panel2.Controls.Add(this.la_Des);
			this.sc_body.Panel2MinSize = 0;
			this.sc_body.Size = new System.Drawing.Size(792, 574);
			this.sc_body.SplitterDistance = 450;
			this.sc_body.SplitterWidth = 2;
			this.sc_body.TabIndex = 0;
			// 
			// rt_Val
			// 
			this.rt_Val.AcceptsReturn = true;
			this.rt_Val.AcceptsTab = true;
			this.rt_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.rt_Val.BackColor = System.Drawing.Color.Thistle;
			this.rt_Val.Enabled = false;
			this.rt_Val.HideSelection = false;
			this.rt_Val.Location = new System.Drawing.Point(0, 150);
			this.rt_Val.Margin = new System.Windows.Forms.Padding(0);
			this.rt_Val.MaxLength = 0;
			this.rt_Val.Multiline = true;
			this.rt_Val.Name = "rt_Val";
			this.rt_Val.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.rt_Val.Size = new System.Drawing.Size(338, 422);
			this.rt_Val.TabIndex = 7;
			this.rt_Val.WordWrap = false;
			this.rt_Val.TextChanged += new System.EventHandler(this.textchanged_Multi);
			this.rt_Val.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keydown_Multi);
			this.rt_Val.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyup);
			this.rt_Val.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mousedown);
			// 
			// cb_Wordwrap
			// 
			this.cb_Wordwrap.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.cb_Wordwrap.Location = new System.Drawing.Point(10, 130);
			this.cb_Wordwrap.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Wordwrap.Name = "cb_Wordwrap";
			this.cb_Wordwrap.Size = new System.Drawing.Size(75, 16);
			this.cb_Wordwrap.TabIndex = 6;
			this.cb_Wordwrap.Text = "wordwrap";
			this.cb_Wordwrap.UseVisualStyleBackColor = true;
			this.cb_Wordwrap.Visible = false;
			this.cb_Wordwrap.CheckedChanged += new System.EventHandler(this.checkchanged_Wordwrap);
			// 
			// btn_Apply
			// 
			this.btn_Apply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Apply.Enabled = false;
			this.btn_Apply.Location = new System.Drawing.Point(1, 99);
			this.btn_Apply.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Apply.Name = "btn_Apply";
			this.btn_Apply.Size = new System.Drawing.Size(336, 25);
			this.btn_Apply.TabIndex = 5;
			this.btn_Apply.Text = "APPLY";
			this.btn_Apply.UseVisualStyleBackColor = true;
			this.btn_Apply.Click += new System.EventHandler(this.click_Apply);
			// 
			// cb_Custo
			// 
			this.cb_Custo.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.cb_Custo.Location = new System.Drawing.Point(10, 80);
			this.cb_Custo.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Custo.Name = "cb_Custo";
			this.cb_Custo.Size = new System.Drawing.Size(125, 16);
			this.cb_Custo.TabIndex = 4;
			this.cb_Custo.Text = "Custom talktable";
			this.cb_Custo.UseVisualStyleBackColor = true;
			this.cb_Custo.Visible = false;
			this.cb_Custo.CheckedChanged += new System.EventHandler(this.checkchanged_Custo);
			// 
			// btn_Revert
			// 
			this.btn_Revert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Revert.Enabled = false;
			this.btn_Revert.Location = new System.Drawing.Point(280, 56);
			this.btn_Revert.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Revert.Name = "btn_Revert";
			this.btn_Revert.Size = new System.Drawing.Size(57, 20);
			this.btn_Revert.TabIndex = 3;
			this.btn_Revert.Text = "revert";
			this.btn_Revert.UseVisualStyleBackColor = true;
			this.btn_Revert.Click += new System.EventHandler(this.click_Revert);
			// 
			// la_Val
			// 
			this.la_Val.Location = new System.Drawing.Point(5, 56);
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
			this.tb_Val.BackColor = System.Drawing.Color.Thistle;
			this.tb_Val.Enabled = false;
			this.tb_Val.HideSelection = false;
			this.tb_Val.Location = new System.Drawing.Point(0, 36);
			this.tb_Val.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Val.MaxLength = 0;
			this.tb_Val.Name = "tb_Val";
			this.tb_Val.Size = new System.Drawing.Size(338, 20);
			this.tb_Val.TabIndex = 1;
			this.tb_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Val.WordWrap = false;
			this.tb_Val.TextChanged += new System.EventHandler(this.textchanged_Single);
			this.tb_Val.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keydown_Single);
			this.tb_Val.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyup);
			this.tb_Val.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mousedown);
			// 
			// la_Des
			// 
			this.la_Des.Location = new System.Drawing.Point(5, 3);
			this.la_Des.Margin = new System.Windows.Forms.Padding(0);
			this.la_Des.Name = "la_Des";
			this.la_Des.Size = new System.Drawing.Size(332, 29);
			this.la_Des.TabIndex = 0;
			this.la_Des.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// ss_bot
			// 
			this.ss_bot.Font = new System.Drawing.Font("Consolas", 6.25F, System.Drawing.FontStyle.Bold);
			this.ss_bot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tssl_info});
			this.ss_bot.Location = new System.Drawing.Point(0, 552);
			this.ss_bot.Name = "ss_bot";
			this.ss_bot.Size = new System.Drawing.Size(792, 22);
			this.ss_bot.TabIndex = 1;
			// 
			// tssl_info
			// 
			this.tssl_info.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.tssl_info.Name = "tssl_info";
			this.tssl_info.Size = new System.Drawing.Size(0, 0);
			this.tssl_info.Spring = true;
			this.tssl_info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// GeneralGFF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 574);
			this.Controls.Add(this.ss_bot);
			this.Controls.Add(this.sc_body);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "GeneralGFF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GeneralGFF";
			this.sc_body.Panel2.ResumeLayout(false);
			this.sc_body.Panel2.PerformLayout();
			this.sc_body.ResumeLayout(false);
			this.ss_bot.ResumeLayout(false);
			this.ss_bot.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}



	/// <summary>
	/// A derived SplitContainer that prevents flicker.
	/// </summary>
	sealed class SplitContainerCp
		: SplitContainer
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
