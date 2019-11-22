using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A dialog for user to add apparel to a creature.
	/// </summary>
	sealed class ApparelDialog
		:
			Form
	{
		#region Fields (static)
		internal const int TYPE_non   = 0x0; // bitwise apparel-types ->
		internal const int TYPE_CLOAK = 0x1;

		#endregion Fields (static)


		#region cTor
		internal ApparelDialog()
		{
			InitializeComponent();

			rb_Cloak.Tag = TYPE_CLOAK;

			rb_Cloak.Enabled = (TreeList._apparel & TYPE_CLOAK) == 0;

			TreeList._apparel = TYPE_non;
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Sets the apparel-type.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged(object sender, EventArgs e)
		{
			TreeList._apparel = (Int32)((RadioButton)sender).Tag;
		}
		#endregion Handlers



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_Head;
		Panel pa_Foot;
		Button bu_Accept;
		Button bu_Cancel;
		GroupBox gb_Apparel;
		RadioButton rb_Cloak;


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
			this.la_Head = new System.Windows.Forms.Label();
			this.pa_Foot = new System.Windows.Forms.Panel();
			this.bu_Accept = new System.Windows.Forms.Button();
			this.bu_Cancel = new System.Windows.Forms.Button();
			this.gb_Apparel = new System.Windows.Forms.GroupBox();
			this.rb_Cloak = new System.Windows.Forms.RadioButton();
			this.pa_Foot.SuspendLayout();
			this.gb_Apparel.SuspendLayout();
			this.SuspendLayout();
			// 
			// la_Head
			// 
			this.la_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_Head.Location = new System.Drawing.Point(0, 0);
			this.la_Head.Margin = new System.Windows.Forms.Padding(0);
			this.la_Head.Name = "la_Head";
			this.la_Head.Size = new System.Drawing.Size(144, 20);
			this.la_Head.TabIndex = 0;
			this.la_Head.Text = "add Apparel";
			this.la_Head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// pa_Foot
			// 
			this.pa_Foot.Controls.Add(this.bu_Accept);
			this.pa_Foot.Controls.Add(this.bu_Cancel);
			this.pa_Foot.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pa_Foot.Location = new System.Drawing.Point(0, 191);
			this.pa_Foot.Margin = new System.Windows.Forms.Padding(0);
			this.pa_Foot.Name = "pa_Foot";
			this.pa_Foot.Size = new System.Drawing.Size(144, 35);
			this.pa_Foot.TabIndex = 1;
			// 
			// bu_Accept
			// 
			this.bu_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_Accept.Location = new System.Drawing.Point(75, 5);
			this.bu_Accept.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Accept.Name = "bu_Accept";
			this.bu_Accept.Size = new System.Drawing.Size(65, 25);
			this.bu_Accept.TabIndex = 1;
			this.bu_Accept.Text = "Accept";
			this.bu_Accept.UseVisualStyleBackColor = true;
			// 
			// bu_Cancel
			// 
			this.bu_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bu_Cancel.Location = new System.Drawing.Point(5, 5);
			this.bu_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Cancel.Name = "bu_Cancel";
			this.bu_Cancel.Size = new System.Drawing.Size(65, 25);
			this.bu_Cancel.TabIndex = 0;
			this.bu_Cancel.Text = "Cancel";
			this.bu_Cancel.UseVisualStyleBackColor = true;
			// 
			// gb_Apparel
			// 
			this.gb_Apparel.Controls.Add(this.rb_Cloak);
			this.gb_Apparel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_Apparel.Location = new System.Drawing.Point(0, 20);
			this.gb_Apparel.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Apparel.Name = "gb_Apparel";
			this.gb_Apparel.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Apparel.Size = new System.Drawing.Size(144, 171);
			this.gb_Apparel.TabIndex = 2;
			this.gb_Apparel.TabStop = false;
			// 
			// rb_Cloak
			// 
			this.rb_Cloak.Location = new System.Drawing.Point(10, 15);
			this.rb_Cloak.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Cloak.Name = "rb_Cloak";
			this.rb_Cloak.Size = new System.Drawing.Size(125, 20);
			this.rb_Cloak.TabIndex = 0;
			this.rb_Cloak.TabStop = true;
			this.rb_Cloak.Text = "Cloak";
			this.rb_Cloak.UseVisualStyleBackColor = true;
			this.rb_Cloak.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// ApparelDialog
			// 
			this.AcceptButton = this.bu_Accept;
			this.CancelButton = this.bu_Cancel;
			this.ClientSize = new System.Drawing.Size(144, 226);
			this.Controls.Add(this.gb_Apparel);
			this.Controls.Add(this.pa_Foot);
			this.Controls.Add(this.la_Head);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ApparelDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Apparel";
			this.pa_Foot.ResumeLayout(false);
			this.gb_Apparel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
