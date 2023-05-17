using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A dialog for user to specify what type and value of variable to add.
	/// </summary>
	sealed class VariableDialog
		:
			Form
	{
		#region Fields (static)
		internal const uint Type_non      = 0; // not stable. rb is Disabled
		internal const uint Type_INT      = 1;
		internal const uint Type_FLOAT    = 2;
		internal const uint Type_STRING   = 3;
		internal const uint Type_UINT     = 4;
		internal const uint Type_LOCATION = 5; // not stable. rb is Disabled
		#endregion Fields (static)


		#region Fields
		uint _varType;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal VariableDialog()
		{
			InitializeComponent();

			rb_int   .Tag = Type_INT;
			rb_float .Tag = Type_FLOAT;
			rb_string.Tag = Type_STRING;
			rb_uint  .Tag = Type_UINT;
			rb_loc   .Tag = Type_LOCATION;
			rb_none  .Tag = Type_non;

			rb_int.Checked = true;

			tb_var.Text =
			tb_val.Text = String.Empty;

			tb_var.Select();
		}
		#endregion cTor


		#region Handlers (override)
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK
				&& e.CloseReason != CloseReason.WindowsShutDown
				&& e.CloseReason != CloseReason.TaskManagerClosing)
			{
				TreeList tree = ((GeneralGFF)Owner)._tree;

				string error = null;
				if (String.IsNullOrEmpty(tb_var.Text))
				{
					error = "The variable label cannot be null.";
					tb_var.Select();
				}
				else if (String.IsNullOrEmpty(tb_val.Text))
				{
					error = "The variable value cannot be null.";
					tb_val.Select();
				}
				else if (tree.CheckVariableForRedundancy(_varType, tb_var.Text))
				{
					error = "That variable label+type already exists.";
					tb_var.Select();
				}
				else
				{
					switch (_varType)
					{
						case Type_non: // not stable in toolset. Disabled
							error = "Type is not stable in toolset. Disabled";
							break;

						case Type_INT:
						{
							int result;
							if (!Int32.TryParse(tb_val.Text, out result))
								error = "That is not a valid value.";
							break;
						}

						case Type_FLOAT:
						{
							float result;
							if (!Single.TryParse(tb_val.Text, out result))
								error = "That is not a valid value.";
							break;
						}

//						case Type_STRING: // is okay.
//							break;

						case Type_UINT:
						{
							uint result;
							if (!UInt32.TryParse(tb_val.Text, out result))
								error = "That is not a valid value.";
							break;
						}

						case Type_LOCATION: // not stable in toolset. Disabled
							error = "Type is not stable in toolset. Disabled";
							break;
					}
				}

				if (error != null)
				{
					e.Cancel = true;
					using (var f = new InfoDialog(Globals.Error, error))
						f.ShowDialog(this);
				}
				else
				{
					tree._varLabel = tb_var.Text;
					tree._varValue = tb_val.Text;
					tree._varType  = _varType; // uh ...
				}
			}
		}
		#endregion Handlers (override)


		#region Handlers
		/// <summary>
		/// Sets the variable's typeid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_radio(object sender, EventArgs e)
		{
			_varType = (uint)((RadioButton)sender).Tag;
		}
		#endregion Handlers



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_head;
		Button bt_Accept;
		Button bt_Cancel;
		Panel pl_bot;
		GroupBox gb_type;
		RadioButton rb_int;
		RadioButton rb_float;
		RadioButton rb_string;
		RadioButton rb_uint;
		RadioButton rb_loc;
		RadioButton rb_none;
		TextBox tb_val;
		TextBox tb_var;


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
			this.la_head = new System.Windows.Forms.Label();
			this.bt_Accept = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.pl_bot = new System.Windows.Forms.Panel();
			this.gb_type = new System.Windows.Forms.GroupBox();
			this.rb_int = new System.Windows.Forms.RadioButton();
			this.rb_float = new System.Windows.Forms.RadioButton();
			this.rb_string = new System.Windows.Forms.RadioButton();
			this.rb_uint = new System.Windows.Forms.RadioButton();
			this.rb_loc = new System.Windows.Forms.RadioButton();
			this.rb_none = new System.Windows.Forms.RadioButton();
			this.tb_val = new System.Windows.Forms.TextBox();
			this.tb_var = new System.Windows.Forms.TextBox();
			this.pl_bot.SuspendLayout();
			this.gb_type.SuspendLayout();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Size = new System.Drawing.Size(194, 20);
			this.la_head.TabIndex = 0;
			this.la_head.Text = "Add variable";
			this.la_head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// bt_Accept
			// 
			this.bt_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Accept.Location = new System.Drawing.Point(114, 6);
			this.bt_Accept.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Accept.Name = "bt_Accept";
			this.bt_Accept.Size = new System.Drawing.Size(75, 25);
			this.bt_Accept.TabIndex = 1;
			this.bt_Accept.Text = "Accept";
			this.bt_Accept.UseVisualStyleBackColor = true;
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bt_Cancel.Location = new System.Drawing.Point(5, 6);
			this.bt_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(75, 25);
			this.bt_Cancel.TabIndex = 0;
			this.bt_Cancel.Text = "Cancel";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			// 
			// pl_bot
			// 
			this.pl_bot.Controls.Add(this.bt_Accept);
			this.pl_bot.Controls.Add(this.bt_Cancel);
			this.pl_bot.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pl_bot.Location = new System.Drawing.Point(0, 220);
			this.pl_bot.Margin = new System.Windows.Forms.Padding(0);
			this.pl_bot.Name = "pl_bot";
			this.pl_bot.Size = new System.Drawing.Size(194, 36);
			this.pl_bot.TabIndex = 4;
			// 
			// gb_type
			// 
			this.gb_type.Controls.Add(this.rb_int);
			this.gb_type.Controls.Add(this.rb_float);
			this.gb_type.Controls.Add(this.rb_string);
			this.gb_type.Controls.Add(this.rb_uint);
			this.gb_type.Controls.Add(this.rb_loc);
			this.gb_type.Controls.Add(this.rb_none);
			this.gb_type.Location = new System.Drawing.Point(0, 45);
			this.gb_type.Margin = new System.Windows.Forms.Padding(0);
			this.gb_type.Name = "gb_type";
			this.gb_type.Size = new System.Drawing.Size(190, 145);
			this.gb_type.TabIndex = 2;
			this.gb_type.TabStop = false;
			this.gb_type.Text = " type ";
			// 
			// rb_int
			// 
			this.rb_int.Location = new System.Drawing.Point(10, 20);
			this.rb_int.Margin = new System.Windows.Forms.Padding(0);
			this.rb_int.Name = "rb_int";
			this.rb_int.Size = new System.Drawing.Size(170, 20);
			this.rb_int.TabIndex = 0;
			this.rb_int.TabStop = true;
			this.rb_int.Text = "int";
			this.rb_int.UseVisualStyleBackColor = true;
			this.rb_int.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_float
			// 
			this.rb_float.Location = new System.Drawing.Point(10, 40);
			this.rb_float.Margin = new System.Windows.Forms.Padding(0);
			this.rb_float.Name = "rb_float";
			this.rb_float.Size = new System.Drawing.Size(170, 20);
			this.rb_float.TabIndex = 1;
			this.rb_float.TabStop = true;
			this.rb_float.Text = "float";
			this.rb_float.UseVisualStyleBackColor = true;
			this.rb_float.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_string
			// 
			this.rb_string.Location = new System.Drawing.Point(10, 60);
			this.rb_string.Margin = new System.Windows.Forms.Padding(0);
			this.rb_string.Name = "rb_string";
			this.rb_string.Size = new System.Drawing.Size(170, 20);
			this.rb_string.TabIndex = 2;
			this.rb_string.TabStop = true;
			this.rb_string.Text = "string";
			this.rb_string.UseVisualStyleBackColor = true;
			// 
			// rb_uint
			// 
			this.rb_uint.Location = new System.Drawing.Point(10, 80);
			this.rb_uint.Margin = new System.Windows.Forms.Padding(0);
			this.rb_uint.Name = "rb_uint";
			this.rb_uint.Size = new System.Drawing.Size(170, 20);
			this.rb_uint.TabIndex = 3;
			this.rb_uint.TabStop = true;
			this.rb_uint.Text = "unsigned int";
			this.rb_uint.UseVisualStyleBackColor = true;
			this.rb_uint.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_loc
			// 
			this.rb_loc.Enabled = false;
			this.rb_loc.Location = new System.Drawing.Point(10, 100);
			this.rb_loc.Margin = new System.Windows.Forms.Padding(0);
			this.rb_loc.Name = "rb_loc";
			this.rb_loc.Size = new System.Drawing.Size(170, 20);
			this.rb_loc.TabIndex = 4;
			this.rb_loc.TabStop = true;
			this.rb_loc.Text = "location";
			this.rb_loc.UseVisualStyleBackColor = true;
			this.rb_loc.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_none
			// 
			this.rb_none.Enabled = false;
			this.rb_none.Location = new System.Drawing.Point(10, 120);
			this.rb_none.Margin = new System.Windows.Forms.Padding(0);
			this.rb_none.Name = "rb_none";
			this.rb_none.Size = new System.Drawing.Size(170, 20);
			this.rb_none.TabIndex = 5;
			this.rb_none.TabStop = true;
			this.rb_none.Text = "none";
			this.rb_none.UseVisualStyleBackColor = true;
			this.rb_none.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// tb_val
			// 
			this.tb_val.BackColor = System.Drawing.Color.Honeydew;
			this.tb_val.Location = new System.Drawing.Point(5, 195);
			this.tb_val.Margin = new System.Windows.Forms.Padding(0);
			this.tb_val.Name = "tb_val";
			this.tb_val.Size = new System.Drawing.Size(185, 20);
			this.tb_val.TabIndex = 3;
			this.tb_val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tb_var
			// 
			this.tb_var.BackColor = System.Drawing.Color.Honeydew;
			this.tb_var.Location = new System.Drawing.Point(5, 25);
			this.tb_var.Margin = new System.Windows.Forms.Padding(0);
			this.tb_var.Name = "tb_var";
			this.tb_var.Size = new System.Drawing.Size(185, 20);
			this.tb_var.TabIndex = 1;
			this.tb_var.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// VariableDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(194, 256);
			this.Controls.Add(this.tb_var);
			this.Controls.Add(this.tb_val);
			this.Controls.Add(this.gb_type);
			this.Controls.Add(this.pl_bot);
			this.Controls.Add(this.la_head);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "VariableDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add variable";
			this.pl_bot.ResumeLayout(false);
			this.gb_type.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
