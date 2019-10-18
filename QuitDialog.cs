using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	sealed class QuitDialog
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="allowsave"></param>
		internal QuitDialog(bool allowsave)
		{
			InitializeComponent();

			bt_Save.Enabled = allowsave;
			bt_Cancel.Select();
		}
		#endregion cTor



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_Head;
		Button bt_Save;
		Button bt_Cancel;
		Button bt_Quit;


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
			this.bt_Save = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.bt_Quit = new System.Windows.Forms.Button();
			this.la_Head = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bt_Save
			// 
			this.bt_Save.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.bt_Save.Location = new System.Drawing.Point(85, 25);
			this.bt_Save.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Save.Name = "bt_Save";
			this.bt_Save.Size = new System.Drawing.Size(70, 25);
			this.bt_Save.TabIndex = 2;
			this.bt_Save.Text = "Save";
			this.bt_Save.UseVisualStyleBackColor = true;
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.bt_Cancel.Location = new System.Drawing.Point(165, 25);
			this.bt_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(70, 25);
			this.bt_Cancel.TabIndex = 3;
			this.bt_Cancel.Text = "Cancel";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			// 
			// bt_Quit
			// 
			this.bt_Quit.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.bt_Quit.Location = new System.Drawing.Point(5, 25);
			this.bt_Quit.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Quit.Name = "bt_Quit";
			this.bt_Quit.Size = new System.Drawing.Size(70, 25);
			this.bt_Quit.TabIndex = 1;
			this.bt_Quit.Text = "Quit";
			this.bt_Quit.UseVisualStyleBackColor = true;
			// 
			// la_Head
			// 
			this.la_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Head.ForeColor = System.Drawing.Color.Firebrick;
			this.la_Head.Location = new System.Drawing.Point(0, 0);
			this.la_Head.Margin = new System.Windows.Forms.Padding(0);
			this.la_Head.Name = "la_Head";
			this.la_Head.Size = new System.Drawing.Size(239, 20);
			this.la_Head.TabIndex = 0;
			this.la_Head.Text = "Data has changed ...";
			this.la_Head.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// QuitDialog
			// 
			this.AcceptButton = this.bt_Save;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(239, 51);
			this.Controls.Add(this.la_Head);
			this.Controls.Add(this.bt_Quit);
			this.Controls.Add(this.bt_Save);
			this.Controls.Add(this.bt_Cancel);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "QuitDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Quit";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
