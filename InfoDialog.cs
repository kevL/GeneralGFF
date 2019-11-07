using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace generalgff
{
	sealed class InfoDialog
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="info"></param>
		internal InfoDialog(string title, string info)
		{
			InitializeComponent();

			Text = title;
			la_Info.Text = info;

			switch (title)
			{
				case Globals.About:
					KeyDown += keydown;
					break;

				case Globals.Error:
					System.Media.SystemSounds.Exclamation.Play();
					bt_Okay.Text = "Cancel";
					break;
			}


			int hPadBot = ClientSize.Height - la_Info.Height;

			int w = 0, wTest, h = 0, hTest;
			string[] separators = { "\r\n", "\r", "\n" };
			string[] lines = info.Split(separators, StringSplitOptions.None);
			logfile.Log("lines= " + lines.Length);

			var size = new Size();
			foreach (var line in lines)
			{
				size = TextRenderer.MeasureText(line, la_Info.Font);
				if ((wTest = size.Width) > w)
					w = wTest;

				if ((hTest = size.Height) > h)
					h = hTest;
			}
			w += la_Info.Padding.Left + la_Info.Padding.Right;

			if (w < bt_Okay.Width)
				w = bt_Okay.Width;

			la_Info.Height = h * lines.Length + la_Info.Padding.Top;

			ClientSize = new Size(w, la_Info.Height + hPadBot);

			bt_Okay.Location = new Point((w - bt_Okay.Width) / 2, bt_Okay.Top);
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Special handling for the About box - close on F2.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2)
				Close();
		}
		#endregion Handlers



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_Info;
		Button bt_Okay;


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
			this.bt_Okay = new System.Windows.Forms.Button();
			this.la_Info = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bt_Okay
			// 
			this.bt_Okay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Okay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Okay.Location = new System.Drawing.Point(170, 28);
			this.bt_Okay.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Okay.Name = "bt_Okay";
			this.bt_Okay.Size = new System.Drawing.Size(70, 25);
			this.bt_Okay.TabIndex = 1;
			this.bt_Okay.Text = "Ok";
			this.bt_Okay.UseVisualStyleBackColor = true;
			// 
			// la_Info
			// 
			this.la_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Info.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_Info.Location = new System.Drawing.Point(0, 0);
			this.la_Info.Margin = new System.Windows.Forms.Padding(0);
			this.la_Info.Name = "la_Info";
			this.la_Info.Padding = new System.Windows.Forms.Padding(5, 5, 6, 0);
			this.la_Info.Size = new System.Drawing.Size(244, 22);
			this.la_Info.TabIndex = 0;
			this.la_Info.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// InfoDialog
			// 
			this.AcceptButton = this.bt_Okay;
			this.CancelButton = this.bt_Okay;
			this.ClientSize = new System.Drawing.Size(244, 56);
			this.Controls.Add(this.la_Info);
			this.Controls.Add(this.bt_Okay);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InfoDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
