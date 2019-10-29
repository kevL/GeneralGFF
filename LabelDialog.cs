using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	sealed class LabelDialog
		:
			Form
	{
		#region Fields
		bool _stop;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		internal LabelDialog(string label)
		{
			InitializeComponent();

			tb_Label.Text = label;
			tb_Label.Select();

			if (label == "label")
			{
				tb_Label.SelectionStart = 0;
				tb_Label.SelectionLength = label.Length;
			}
			else
				tb_Label.SelectionStart = tb_Label.Text.Length;
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			e.Cancel = _stop;
			_stop = false;

//			base.OnFormClosing(e);
		}
		#endregion Handlers


		#region Handlers
		/// <summary>
		/// Stops this dialog from closing if the textbox contains an invalid
		/// label.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Accept(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(tb_Label.Text)
				|| tb_Label.Text.Length > Globals.Length_LABEL
				|| !isAlphanumeric(tb_Label.Text))
			{
				_stop = true;
				baddog();

				tb_Label.Select();
				tb_Label.SelectionStart = tb_Label.Text.Length;
			}
			else
				_stop = false;
		}

		/// <summary>
		/// Allows this dialog to close.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Cancel(object sender, EventArgs e)
		{
			_stop = false;
		}
		#endregion Handlers


		#region Methods
		/// <summary>
		/// Checks if a string is alphanumerical.
		/// @note Underscores are also valid.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		bool isAlphanumeric(string text)
		{
			int c;
			for (int i = 0; i != text.Length; ++i)
			{
				if ((c = (int)text[i]) != 95
					&& (    c <  48
						|| (c >  57 && c < 65)
						|| (c >  90 && c < 97)
						||  c > 122))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Generic error dialog.
		/// </summary>
		void baddog()
		{
			string info = "Labels have a max length of 16 characters, a" + Environment.NewLine
						+ "min length of 1 character, and may contain"   + Environment.NewLine
						+ "only alphanumeric or underscore characters.";
			using (var f = new InfoDialog(Globals.Error, info))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Methods



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Button bt_Accept;
		Button bt_Cancel;

		internal TextBox tb_Label;


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
			this.bt_Accept = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.tb_Label = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// bt_Accept
			// 
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Accept.Location = new System.Drawing.Point(100, 20);
			this.bt_Accept.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Accept.Name = "bt_Accept";
			this.bt_Accept.Size = new System.Drawing.Size(95, 25);
			this.bt_Accept.TabIndex = 2;
			this.bt_Accept.Text = "Accept";
			this.bt_Accept.UseVisualStyleBackColor = true;
			this.bt_Accept.Click += new System.EventHandler(this.click_Accept);
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bt_Cancel.Location = new System.Drawing.Point(0, 20);
			this.bt_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(95, 25);
			this.bt_Cancel.TabIndex = 1;
			this.bt_Cancel.Text = "Cancel";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			this.bt_Cancel.Click += new System.EventHandler(this.click_Cancel);
			// 
			// tb_Label
			// 
			this.tb_Label.BackColor = System.Drawing.Color.LemonChiffon;
			this.tb_Label.Dock = System.Windows.Forms.DockStyle.Top;
			this.tb_Label.Location = new System.Drawing.Point(0, 0);
			this.tb_Label.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Label.Name = "tb_Label";
			this.tb_Label.Size = new System.Drawing.Size(195, 20);
			this.tb_Label.TabIndex = 0;
			this.tb_Label.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LabelDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(195, 45);
			this.Controls.Add(this.tb_Label);
			this.Controls.Add(this.bt_Accept);
			this.Controls.Add(this.bt_Cancel);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LabelDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "edit Label";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
