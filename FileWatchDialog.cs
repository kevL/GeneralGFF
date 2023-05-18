using System;
using System.Windows.Forms;


namespace generalgff
{
	sealed class FileWatchDialog
		:
			Form
	{
		#region Fields (static)
		internal const int FILE_DEL = 0; // file's not there, Jim.
		internal const int FILE_WSC = 1; // file's writestamp changed
		#endregion Fields (static)


		#region Properties (static)
		/// <summary>
		/// Bypasses file-exists (FILE_DEL) checks if true.
		/// </summary>
		internal static bool Bypass
		{ get; set; }
		#endregion Properties (static)


		#region Fields
		GeneralGFF _f;

		int _fwType;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="fwType"></param>
		internal FileWatchDialog(GeneralGFF f, int fwType)
		{
			InitializeComponent();

			_f = f;

			if (_f.WindowState == FormWindowState.Minimized)
				_f.WindowState  = FormWindowState.Normal;

			tb_Path.Text = _f.GffData.Pfe;
			Width = TextRenderer.MeasureText(tb_Path.Text, tb_Path.Font).Width + 20;

			string text = String.Empty;
			switch (_fwType = fwType)
			{
				case FILE_DEL:
					text = "The file cannot be found on disk.";
					btn_Action.Text = "Save";
					break;

				case FILE_WSC:
					text = "The file on disk has changed.";
					btn_Action.Text = "Reload";
					break;
			}

			la_Info.Text = text;

			btn_Action.Select();
		}
		#endregion cTor


		#region Events (override)
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			tb_Path.SelectionStart = 0;
			tb_Path.SelectionStart = tb_Path.Text.Length;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (   e.CloseReason != CloseReason.WindowsShutDown
				&& e.CloseReason != CloseReason.TaskManagerClosing)
			{
				switch (DialogResult)
				{
					case DialogResult.Cancel:	// btn_Cancel
						_f.GffData.Changed = true;
						_f.GffData = _f.GffData;
						break;

					case DialogResult.Yes:		// btn_Action
						switch (_fwType)
						{
							case FILE_DEL:
								_f.fileclick_Save(null, EventArgs.Empty);
								break;

							case FILE_WSC:
								_f.GffData.Changed = false; // bypass close-check
								_f.fileclick_Reload(null, EventArgs.Empty);
								break;
						}
						break;
				}
			}
		}
		#endregion Events (override)



		#region Designer
		Button btn_Cancel;
		Button btn_Action;
		Label la_Info;
		TextBox tb_Path;
		Panel pnl_Bot;
		Panel pnl_Top;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_Action = new System.Windows.Forms.Button();
			this.la_Info = new System.Windows.Forms.Label();
			this.tb_Path = new System.Windows.Forms.TextBox();
			this.pnl_Bot = new System.Windows.Forms.Panel();
			this.pnl_Top = new System.Windows.Forms.Panel();
			this.pnl_Bot.SuspendLayout();
			this.pnl_Top.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(200, 2);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(100, 30);
			this.btn_Cancel.TabIndex = 1;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			// 
			// btn_Action
			// 
			this.btn_Action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.btn_Action.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btn_Action.Location = new System.Drawing.Point(25, 2);
			this.btn_Action.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Action.Name = "btn_Action";
			this.btn_Action.Size = new System.Drawing.Size(100, 30);
			this.btn_Action.TabIndex = 0;
			this.btn_Action.UseVisualStyleBackColor = true;
			// 
			// la_Info
			// 
			this.la_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Info.Location = new System.Drawing.Point(0, 0);
			this.la_Info.Margin = new System.Windows.Forms.Padding(0);
			this.la_Info.Name = "la_Info";
			this.la_Info.Size = new System.Drawing.Size(325, 20);
			this.la_Info.TabIndex = 0;
			this.la_Info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tb_Path
			// 
			this.tb_Path.BackColor = System.Drawing.Color.LightPink;
			this.tb_Path.Dock = System.Windows.Forms.DockStyle.Top;
			this.tb_Path.Location = new System.Drawing.Point(0, 20);
			this.tb_Path.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Path.Name = "tb_Path";
			this.tb_Path.ReadOnly = true;
			this.tb_Path.Size = new System.Drawing.Size(325, 20);
			this.tb_Path.TabIndex = 1;
			this.tb_Path.WordWrap = false;
			// 
			// pnl_Bot
			// 
			this.pnl_Bot.Controls.Add(this.btn_Cancel);
			this.pnl_Bot.Controls.Add(this.btn_Action);
			this.pnl_Bot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_Bot.Location = new System.Drawing.Point(0, 40);
			this.pnl_Bot.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Bot.Name = "pnl_Bot";
			this.pnl_Bot.Size = new System.Drawing.Size(325, 34);
			this.pnl_Bot.TabIndex = 1;
			// 
			// pnl_Top
			// 
			this.pnl_Top.Controls.Add(this.tb_Path);
			this.pnl_Top.Controls.Add(this.la_Info);
			this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl_Top.Location = new System.Drawing.Point(0, 0);
			this.pnl_Top.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Top.Name = "pnl_Top";
			this.pnl_Top.Size = new System.Drawing.Size(325, 40);
			this.pnl_Top.TabIndex = 0;
			// 
			// FileWatchDialog
			// 
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(325, 74);
			this.Controls.Add(this.pnl_Bot);
			this.Controls.Add(this.pnl_Top);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(333, 100);
			this.Name = "FileWatchDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "File watcher";
			this.pnl_Bot.ResumeLayout(false);
			this.pnl_Top.ResumeLayout(false);
			this.pnl_Top.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
