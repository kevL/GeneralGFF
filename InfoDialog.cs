using System;
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

			KeyDown += keydown_Copy;

			switch (title)
			{
				case Globals.About:
					KeyDown += keydown_Close;
					break;

				case Globals.Error:
					System.Media.SystemSounds.Exclamation.Play();
					bu_Okay.Text = "cancel";
					break;
			}


			SuspendLayout();

			int w = 0, wTest, h = 0, hTest;
			string[] separators = { "\r\n", "\r", "\n" };
			string[] lines = info.Split(separators, StringSplitOptions.None);

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
			if (w < 250) w = 250;

			la_Info.Height = h * lines.Length + la_Info.Padding.Top + la_Info.Padding.Bottom;

			ClientSize = new Size(w, la_Info.Height + bu_Okay.Height);

			bu_Okay.Location = new Point(w - bu_Okay.Width - 3, la_Info.Bottom + 6);

			ResumeLayout();
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Special handling for the About box - close on F2.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Close(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.F2:
					Close();
					break;
			}
		}

		/// <summary>
		/// Copys text to clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Copy(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Control | Keys.C:
					Clipboard.SetText(la_Info.Text);
					break;
			}
		}
		#endregion Handlers



		#region Designer
		Label la_Info;
		Button bu_Okay;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.bu_Okay = new System.Windows.Forms.Button();
			this.la_Info = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bu_Okay
			// 
			this.bu_Okay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_Okay.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bu_Okay.Location = new System.Drawing.Point(0, 30);
			this.bu_Okay.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Okay.Name = "bu_Okay";
			this.bu_Okay.Size = new System.Drawing.Size(244, 23);
			this.bu_Okay.TabIndex = 1;
			this.bu_Okay.Text = "ok";
			this.bu_Okay.UseVisualStyleBackColor = true;
			// 
			// la_Info
			// 
			this.la_Info.Dock = System.Windows.Forms.DockStyle.Fill;
			this.la_Info.ForeColor = System.Drawing.SystemColors.ControlText;
			this.la_Info.Location = new System.Drawing.Point(0, 0);
			this.la_Info.Margin = new System.Windows.Forms.Padding(0);
			this.la_Info.Name = "la_Info";
			this.la_Info.Padding = new System.Windows.Forms.Padding(5, 6, 8, 5);
			this.la_Info.Size = new System.Drawing.Size(244, 30);
			this.la_Info.TabIndex = 0;
			this.la_Info.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// InfoDialog
			// 
			this.AcceptButton = this.bu_Okay;
			this.CancelButton = this.bu_Okay;
			this.ClientSize = new System.Drawing.Size(244, 53);
			this.Controls.Add(this.la_Info);
			this.Controls.Add(this.bu_Okay);
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
