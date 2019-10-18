using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	sealed class DeleteDialog
		:
			Form
	{
		TreeList _tl;

		/// <summary>
		/// cTor.
		/// </summary>
		/// <param>tl</param>
		/// <param>head</param>
		internal DeleteDialog(TreeList tl, string head)
		{
			InitializeComponent();

			_tl = tl;
			la_Head.Text = head;

			bt_Accept.Select();
		}



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_Head;
		Button bt_Accept;
		Button bt_Cancel;
		internal CheckBox cb_Bypass;


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
			this.cb_Bypass = new System.Windows.Forms.CheckBox();
			this.la_Head = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bt_Accept
			// 
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.bt_Accept.Location = new System.Drawing.Point(105, 25);
			this.bt_Accept.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Accept.Name = "bt_Accept";
			this.bt_Accept.Size = new System.Drawing.Size(90, 25);
			this.bt_Accept.TabIndex = 2;
			this.bt_Accept.Text = "Yes";
			this.bt_Accept.UseVisualStyleBackColor = true;
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.No;
			this.bt_Cancel.Location = new System.Drawing.Point(5, 25);
			this.bt_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(90, 25);
			this.bt_Cancel.TabIndex = 1;
			this.bt_Cancel.Text = "No";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			// 
			// cb_Bypass
			// 
			this.cb_Bypass.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cb_Bypass.Location = new System.Drawing.Point(55, 57);
			this.cb_Bypass.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Bypass.Name = "cb_Bypass";
			this.cb_Bypass.Size = new System.Drawing.Size(135, 18);
			this.cb_Bypass.TabIndex = 3;
			this.cb_Bypass.Text = "bypass for session";
			this.cb_Bypass.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.cb_Bypass.UseVisualStyleBackColor = true;
			// 
			// la_Head
			// 
			this.la_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Head.Location = new System.Drawing.Point(0, 0);
			this.la_Head.Margin = new System.Windows.Forms.Padding(0);
			this.la_Head.Name = "la_Head";
			this.la_Head.Size = new System.Drawing.Size(199, 20);
			this.la_Head.TabIndex = 0;
			this.la_Head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// DeleteDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(199, 76);
			this.Controls.Add(this.la_Head);
			this.Controls.Add(this.cb_Bypass);
			this.Controls.Add(this.bt_Accept);
			this.Controls.Add(this.bt_Cancel);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeleteDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Delete warning";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
