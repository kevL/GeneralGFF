using System;
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
		internal const int TYPE_non    = 0x00; // bitwise apparel-types ->
		internal const int TYPE_BELT   = 0x01;
		internal const int TYPE_BOOTS  = 0x02;
		internal const int TYPE_CLOAK  = 0x04;
		internal const int TYPE_GLOVES = 0x08;
		internal const int TYPE_HELM   = 0x10;
		#endregion Fields (static)


		#region cTor
		internal ApparelDialog()
		{
			InitializeComponent();

			rb_Belt  .Tag = TYPE_BELT;
			rb_Boots .Tag = TYPE_BOOTS;
			rb_Cloak .Tag = TYPE_CLOAK;
			rb_Gloves.Tag = TYPE_GLOVES;
			rb_Helm  .Tag = TYPE_HELM;

			rb_Belt  .Enabled = (TreeList._apparel & TYPE_BELT)   == 0;
			rb_Boots .Enabled = (TreeList._apparel & TYPE_BOOTS)  == 0;
			rb_Cloak .Enabled = (TreeList._apparel & TYPE_CLOAK)  == 0;
			rb_Gloves.Enabled = (TreeList._apparel & TYPE_GLOVES) == 0;
			rb_Helm  .Enabled = (TreeList._apparel & TYPE_HELM)   == 0;

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
		Label la_Head;
		Panel pa_Foot;
		Button bu_Accept;
		Button bu_Cancel;
		GroupBox gb_Apparel;
		RadioButton rb_Belt;
		RadioButton rb_Boots;
		RadioButton rb_Cloak;
		RadioButton rb_Gloves;
		RadioButton rb_Helm;

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
			this.rb_Belt = new System.Windows.Forms.RadioButton();
			this.rb_Boots = new System.Windows.Forms.RadioButton();
			this.rb_Cloak = new System.Windows.Forms.RadioButton();
			this.rb_Gloves = new System.Windows.Forms.RadioButton();
			this.rb_Helm = new System.Windows.Forms.RadioButton();
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
			this.la_Head.Size = new System.Drawing.Size(134, 20);
			this.la_Head.TabIndex = 0;
			this.la_Head.Text = "add Apparel";
			this.la_Head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// pa_Foot
			// 
			this.pa_Foot.Controls.Add(this.bu_Accept);
			this.pa_Foot.Controls.Add(this.bu_Cancel);
			this.pa_Foot.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pa_Foot.Location = new System.Drawing.Point(0, 141);
			this.pa_Foot.Margin = new System.Windows.Forms.Padding(0);
			this.pa_Foot.Name = "pa_Foot";
			this.pa_Foot.Size = new System.Drawing.Size(134, 35);
			this.pa_Foot.TabIndex = 2;
			// 
			// bu_Accept
			// 
			this.bu_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_Accept.Location = new System.Drawing.Point(70, 5);
			this.bu_Accept.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Accept.Name = "bu_Accept";
			this.bu_Accept.Size = new System.Drawing.Size(60, 25);
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
			this.bu_Cancel.Size = new System.Drawing.Size(60, 25);
			this.bu_Cancel.TabIndex = 0;
			this.bu_Cancel.Text = "Cancel";
			this.bu_Cancel.UseVisualStyleBackColor = true;
			// 
			// gb_Apparel
			// 
			this.gb_Apparel.Controls.Add(this.rb_Belt);
			this.gb_Apparel.Controls.Add(this.rb_Boots);
			this.gb_Apparel.Controls.Add(this.rb_Cloak);
			this.gb_Apparel.Controls.Add(this.rb_Gloves);
			this.gb_Apparel.Controls.Add(this.rb_Helm);
			this.gb_Apparel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_Apparel.Location = new System.Drawing.Point(0, 20);
			this.gb_Apparel.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Apparel.Name = "gb_Apparel";
			this.gb_Apparel.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Apparel.Size = new System.Drawing.Size(134, 121);
			this.gb_Apparel.TabIndex = 1;
			this.gb_Apparel.TabStop = false;
			// 
			// rb_Belt
			// 
			this.rb_Belt.Location = new System.Drawing.Point(15, 15);
			this.rb_Belt.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Belt.Name = "rb_Belt";
			this.rb_Belt.Size = new System.Drawing.Size(65, 20);
			this.rb_Belt.TabIndex = 0;
			this.rb_Belt.TabStop = true;
			this.rb_Belt.Text = "Belt";
			this.rb_Belt.UseVisualStyleBackColor = true;
			this.rb_Belt.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Boots
			// 
			this.rb_Boots.Location = new System.Drawing.Point(15, 35);
			this.rb_Boots.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Boots.Name = "rb_Boots";
			this.rb_Boots.Size = new System.Drawing.Size(65, 20);
			this.rb_Boots.TabIndex = 1;
			this.rb_Boots.TabStop = true;
			this.rb_Boots.Text = "Boots";
			this.rb_Boots.UseVisualStyleBackColor = true;
			this.rb_Boots.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Cloak
			// 
			this.rb_Cloak.Location = new System.Drawing.Point(15, 55);
			this.rb_Cloak.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Cloak.Name = "rb_Cloak";
			this.rb_Cloak.Size = new System.Drawing.Size(65, 20);
			this.rb_Cloak.TabIndex = 2;
			this.rb_Cloak.TabStop = true;
			this.rb_Cloak.Text = "Cloak";
			this.rb_Cloak.UseVisualStyleBackColor = true;
			this.rb_Cloak.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Gloves
			// 
			this.rb_Gloves.Location = new System.Drawing.Point(15, 75);
			this.rb_Gloves.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Gloves.Name = "rb_Gloves";
			this.rb_Gloves.Size = new System.Drawing.Size(65, 20);
			this.rb_Gloves.TabIndex = 3;
			this.rb_Gloves.TabStop = true;
			this.rb_Gloves.Text = "Gloves";
			this.rb_Gloves.UseVisualStyleBackColor = true;
			this.rb_Gloves.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Helm
			// 
			this.rb_Helm.Location = new System.Drawing.Point(15, 95);
			this.rb_Helm.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Helm.Name = "rb_Helm";
			this.rb_Helm.Size = new System.Drawing.Size(65, 20);
			this.rb_Helm.TabIndex = 4;
			this.rb_Helm.TabStop = true;
			this.rb_Helm.Text = "Helm";
			this.rb_Helm.UseVisualStyleBackColor = true;
			this.rb_Helm.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// ApparelDialog
			// 
			this.AcceptButton = this.bu_Accept;
			this.CancelButton = this.bu_Cancel;
			this.ClientSize = new System.Drawing.Size(134, 176);
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
