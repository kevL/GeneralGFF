using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A dialog for user to specify what type of equipment-slot to add.
	/// </summary>
	sealed class EquippedItemDialog
		:
			Form
	{
		#region Fields (static)
		internal const uint Slot_HEAD    = 0x00001; // bitwise values for slots ->
		internal const uint Slot_CHEST   = 0x00002; // these need to correspond to Struct-type ids of equippable itemslots
		internal const uint Slot_FEET    = 0x00004;
		internal const uint Slot_WRISTS  = 0x00008;
		internal const uint Slot_RHAND   = 0x00010;
		internal const uint Slot_LHAND   = 0x00020;
		internal const uint Slot_BACK    = 0x00040;
		internal const uint Slot_LRING   = 0x00080;
		internal const uint Slot_RRING   = 0x00100;
		internal const uint Slot_NECK    = 0x00200;
		internal const uint Slot_WAIST   = 0x00400;
		internal const uint Slot_ARROWS  = 0x00800;
		internal const uint Slot_BULLETS = 0x01000;
		internal const uint Slot_BOLTS   = 0x02000;
		internal const uint Slot_WEAP1   = 0x04000;
		internal const uint Slot_WEAP2   = 0x08000;
		internal const uint Slot_WEAP3   = 0x10000;
		internal const uint Slot_SKIN    = 0x20000;
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="bits_equipped"></param>
		internal EquippedItemDialog(uint bits_equipped)
		{
			InitializeComponent();

			rb_head   .Tag = Slot_HEAD;
			rb_chest  .Tag = Slot_CHEST;
			rb_neck   .Tag = Slot_NECK;
			rb_wrists .Tag = Slot_WRISTS;
			rb_waist  .Tag = Slot_WAIST;
			rb_feet   .Tag = Slot_FEET;
			rb_back   .Tag = Slot_BACK;

			rb_lring  .Tag = Slot_LRING;
			rb_rring  .Tag = Slot_RRING;

			rb_arrows .Tag = Slot_ARROWS;
			rb_bolts  .Tag = Slot_BOLTS;
			rb_bullets.Tag = Slot_BULLETS;

			rb_rhand  .Tag = Slot_RHAND;
			rb_lhand  .Tag = Slot_LHAND;

			rb_skin   .Tag = Slot_SKIN;
			rb_weapon1.Tag = Slot_WEAP1;
			rb_weapon2.Tag = Slot_WEAP2;
			rb_weapon3.Tag = Slot_WEAP3;

			rb_head   .Enabled = (bits_equipped & Slot_HEAD)    == 0;
			rb_chest  .Enabled = (bits_equipped & Slot_CHEST)   == 0;
			rb_neck   .Enabled = (bits_equipped & Slot_NECK)    == 0;
			rb_wrists .Enabled = (bits_equipped & Slot_WRISTS)  == 0;
			rb_waist  .Enabled = (bits_equipped & Slot_WAIST)   == 0;
			rb_feet   .Enabled = (bits_equipped & Slot_FEET)    == 0;
			rb_back   .Enabled = (bits_equipped & Slot_BACK)    == 0;

			rb_lring  .Enabled = (bits_equipped & Slot_LRING)   == 0;
			rb_rring  .Enabled = (bits_equipped & Slot_RRING)   == 0;

			rb_arrows .Enabled = (bits_equipped & Slot_ARROWS)  == 0;
			rb_bolts  .Enabled = (bits_equipped & Slot_BOLTS)   == 0;
			rb_bullets.Enabled = (bits_equipped & Slot_BULLETS) == 0;

			rb_rhand  .Enabled = (bits_equipped & Slot_RHAND)   == 0;
			rb_lhand  .Enabled = (bits_equipped & Slot_LHAND)   == 0;

			rb_skin   .Enabled = (bits_equipped & Slot_SKIN)    == 0;
			rb_weapon1.Enabled = (bits_equipped & Slot_WEAP1)   == 0;
			rb_weapon2.Enabled = (bits_equipped & Slot_WEAP2)   == 0;
			rb_weapon3.Enabled = (bits_equipped & Slot_WEAP3)   == 0;
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Sets the slot-bit to be used by the TreeList.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_radio(object sender, EventArgs e)
		{
			((GeneralGFF)Owner)._tree._bitslot = (uint)((RadioButton)sender).Tag;
		}

		/// <summary>
		/// Draws a horizontal divider between the standard equipment itemslots
		/// and the creature equipment itemslots.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void paint_Group(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black,
								25,332,
								gb_slots.Width - 25, 332);
		}
		#endregion Handlers



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		Label la_head;
		RadioButton rb_head;
		RadioButton rb_chest;
		RadioButton rb_neck;
		RadioButton rb_rhand;
		RadioButton rb_lhand;
		RadioButton rb_lring;
		RadioButton rb_wrists;
		RadioButton rb_rring;
		RadioButton rb_waist;
		RadioButton rb_feet;
		RadioButton rb_back;
		RadioButton rb_arrows;
		RadioButton rb_bolts;
		RadioButton rb_bullets;
		RadioButton rb_skin;
		RadioButton rb_weapon1;
		RadioButton rb_weapon2;
		RadioButton rb_weapon3;
		Button bt_Accept;
		Button bt_Cancel;
		GroupBox gb_slots;
		Panel pl_bot;


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
			this.rb_head = new System.Windows.Forms.RadioButton();
			this.rb_chest = new System.Windows.Forms.RadioButton();
			this.rb_neck = new System.Windows.Forms.RadioButton();
			this.rb_rhand = new System.Windows.Forms.RadioButton();
			this.rb_lhand = new System.Windows.Forms.RadioButton();
			this.rb_lring = new System.Windows.Forms.RadioButton();
			this.rb_wrists = new System.Windows.Forms.RadioButton();
			this.rb_rring = new System.Windows.Forms.RadioButton();
			this.rb_waist = new System.Windows.Forms.RadioButton();
			this.rb_feet = new System.Windows.Forms.RadioButton();
			this.rb_back = new System.Windows.Forms.RadioButton();
			this.rb_arrows = new System.Windows.Forms.RadioButton();
			this.rb_bolts = new System.Windows.Forms.RadioButton();
			this.rb_bullets = new System.Windows.Forms.RadioButton();
			this.rb_skin = new System.Windows.Forms.RadioButton();
			this.rb_weapon1 = new System.Windows.Forms.RadioButton();
			this.rb_weapon2 = new System.Windows.Forms.RadioButton();
			this.rb_weapon3 = new System.Windows.Forms.RadioButton();
			this.bt_Accept = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.gb_slots = new System.Windows.Forms.GroupBox();
			this.pl_bot = new System.Windows.Forms.Panel();
			this.gb_slots.SuspendLayout();
			this.pl_bot.SuspendLayout();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Size = new System.Drawing.Size(169, 20);
			this.la_head.TabIndex = 0;
			this.la_head.Text = "Choose a slot";
			this.la_head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// rb_head
			// 
			this.rb_head.Location = new System.Drawing.Point(15, 10);
			this.rb_head.Margin = new System.Windows.Forms.Padding(0);
			this.rb_head.Name = "rb_head";
			this.rb_head.Size = new System.Drawing.Size(135, 20);
			this.rb_head.TabIndex = 0;
			this.rb_head.TabStop = true;
			this.rb_head.Text = "head";
			this.rb_head.UseVisualStyleBackColor = true;
			this.rb_head.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_chest
			// 
			this.rb_chest.Location = new System.Drawing.Point(15, 30);
			this.rb_chest.Margin = new System.Windows.Forms.Padding(0);
			this.rb_chest.Name = "rb_chest";
			this.rb_chest.Size = new System.Drawing.Size(135, 20);
			this.rb_chest.TabIndex = 1;
			this.rb_chest.TabStop = true;
			this.rb_chest.Text = "chest";
			this.rb_chest.UseVisualStyleBackColor = true;
			this.rb_chest.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_neck
			// 
			this.rb_neck.Location = new System.Drawing.Point(15, 50);
			this.rb_neck.Margin = new System.Windows.Forms.Padding(0);
			this.rb_neck.Name = "rb_neck";
			this.rb_neck.Size = new System.Drawing.Size(135, 20);
			this.rb_neck.TabIndex = 2;
			this.rb_neck.TabStop = true;
			this.rb_neck.Text = "neck";
			this.rb_neck.UseVisualStyleBackColor = true;
			this.rb_neck.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_rhand
			// 
			this.rb_rhand.Location = new System.Drawing.Point(15, 280);
			this.rb_rhand.Margin = new System.Windows.Forms.Padding(0);
			this.rb_rhand.Name = "rb_rhand";
			this.rb_rhand.Size = new System.Drawing.Size(135, 20);
			this.rb_rhand.TabIndex = 12;
			this.rb_rhand.TabStop = true;
			this.rb_rhand.Text = "right hand";
			this.rb_rhand.UseVisualStyleBackColor = true;
			this.rb_rhand.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_lhand
			// 
			this.rb_lhand.Location = new System.Drawing.Point(15, 300);
			this.rb_lhand.Margin = new System.Windows.Forms.Padding(0);
			this.rb_lhand.Name = "rb_lhand";
			this.rb_lhand.Size = new System.Drawing.Size(135, 20);
			this.rb_lhand.TabIndex = 13;
			this.rb_lhand.TabStop = true;
			this.rb_lhand.Text = "left hand";
			this.rb_lhand.UseVisualStyleBackColor = true;
			this.rb_lhand.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_lring
			// 
			this.rb_lring.Location = new System.Drawing.Point(15, 160);
			this.rb_lring.Margin = new System.Windows.Forms.Padding(0);
			this.rb_lring.Name = "rb_lring";
			this.rb_lring.Size = new System.Drawing.Size(135, 20);
			this.rb_lring.TabIndex = 7;
			this.rb_lring.TabStop = true;
			this.rb_lring.Text = "left ring";
			this.rb_lring.UseVisualStyleBackColor = true;
			this.rb_lring.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_wrists
			// 
			this.rb_wrists.Location = new System.Drawing.Point(15, 70);
			this.rb_wrists.Margin = new System.Windows.Forms.Padding(0);
			this.rb_wrists.Name = "rb_wrists";
			this.rb_wrists.Size = new System.Drawing.Size(135, 20);
			this.rb_wrists.TabIndex = 3;
			this.rb_wrists.TabStop = true;
			this.rb_wrists.Text = "wrists";
			this.rb_wrists.UseVisualStyleBackColor = true;
			this.rb_wrists.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_rring
			// 
			this.rb_rring.Location = new System.Drawing.Point(15, 180);
			this.rb_rring.Margin = new System.Windows.Forms.Padding(0);
			this.rb_rring.Name = "rb_rring";
			this.rb_rring.Size = new System.Drawing.Size(135, 20);
			this.rb_rring.TabIndex = 8;
			this.rb_rring.TabStop = true;
			this.rb_rring.Text = "right ring";
			this.rb_rring.UseVisualStyleBackColor = true;
			this.rb_rring.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_waist
			// 
			this.rb_waist.Location = new System.Drawing.Point(15, 90);
			this.rb_waist.Margin = new System.Windows.Forms.Padding(0);
			this.rb_waist.Name = "rb_waist";
			this.rb_waist.Size = new System.Drawing.Size(135, 20);
			this.rb_waist.TabIndex = 4;
			this.rb_waist.TabStop = true;
			this.rb_waist.Text = "waist";
			this.rb_waist.UseVisualStyleBackColor = true;
			this.rb_waist.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_feet
			// 
			this.rb_feet.Location = new System.Drawing.Point(15, 110);
			this.rb_feet.Margin = new System.Windows.Forms.Padding(0);
			this.rb_feet.Name = "rb_feet";
			this.rb_feet.Size = new System.Drawing.Size(135, 20);
			this.rb_feet.TabIndex = 5;
			this.rb_feet.TabStop = true;
			this.rb_feet.Text = "feet";
			this.rb_feet.UseVisualStyleBackColor = true;
			this.rb_feet.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_back
			// 
			this.rb_back.Location = new System.Drawing.Point(15, 130);
			this.rb_back.Margin = new System.Windows.Forms.Padding(0);
			this.rb_back.Name = "rb_back";
			this.rb_back.Size = new System.Drawing.Size(135, 20);
			this.rb_back.TabIndex = 6;
			this.rb_back.TabStop = true;
			this.rb_back.Text = "back";
			this.rb_back.UseVisualStyleBackColor = true;
			this.rb_back.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_arrows
			// 
			this.rb_arrows.Location = new System.Drawing.Point(15, 210);
			this.rb_arrows.Margin = new System.Windows.Forms.Padding(0);
			this.rb_arrows.Name = "rb_arrows";
			this.rb_arrows.Size = new System.Drawing.Size(135, 20);
			this.rb_arrows.TabIndex = 9;
			this.rb_arrows.TabStop = true;
			this.rb_arrows.Text = "arrows";
			this.rb_arrows.UseVisualStyleBackColor = true;
			this.rb_arrows.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_bolts
			// 
			this.rb_bolts.Location = new System.Drawing.Point(15, 230);
			this.rb_bolts.Margin = new System.Windows.Forms.Padding(0);
			this.rb_bolts.Name = "rb_bolts";
			this.rb_bolts.Size = new System.Drawing.Size(135, 20);
			this.rb_bolts.TabIndex = 10;
			this.rb_bolts.TabStop = true;
			this.rb_bolts.Text = "bolts";
			this.rb_bolts.UseVisualStyleBackColor = true;
			this.rb_bolts.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_bullets
			// 
			this.rb_bullets.Location = new System.Drawing.Point(15, 250);
			this.rb_bullets.Margin = new System.Windows.Forms.Padding(0);
			this.rb_bullets.Name = "rb_bullets";
			this.rb_bullets.Size = new System.Drawing.Size(135, 20);
			this.rb_bullets.TabIndex = 11;
			this.rb_bullets.TabStop = true;
			this.rb_bullets.Text = "bullets";
			this.rb_bullets.UseVisualStyleBackColor = true;
			this.rb_bullets.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_skin
			// 
			this.rb_skin.Location = new System.Drawing.Point(15, 345);
			this.rb_skin.Margin = new System.Windows.Forms.Padding(0);
			this.rb_skin.Name = "rb_skin";
			this.rb_skin.Size = new System.Drawing.Size(135, 20);
			this.rb_skin.TabIndex = 14;
			this.rb_skin.TabStop = true;
			this.rb_skin.Text = "skin";
			this.rb_skin.UseVisualStyleBackColor = true;
			this.rb_skin.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_weapon1
			// 
			this.rb_weapon1.Location = new System.Drawing.Point(15, 375);
			this.rb_weapon1.Margin = new System.Windows.Forms.Padding(0);
			this.rb_weapon1.Name = "rb_weapon1";
			this.rb_weapon1.Size = new System.Drawing.Size(135, 20);
			this.rb_weapon1.TabIndex = 15;
			this.rb_weapon1.TabStop = true;
			this.rb_weapon1.Text = "weapon1";
			this.rb_weapon1.UseVisualStyleBackColor = true;
			this.rb_weapon1.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_weapon2
			// 
			this.rb_weapon2.Location = new System.Drawing.Point(15, 395);
			this.rb_weapon2.Margin = new System.Windows.Forms.Padding(0);
			this.rb_weapon2.Name = "rb_weapon2";
			this.rb_weapon2.Size = new System.Drawing.Size(135, 20);
			this.rb_weapon2.TabIndex = 16;
			this.rb_weapon2.TabStop = true;
			this.rb_weapon2.Text = "weapon2";
			this.rb_weapon2.UseVisualStyleBackColor = true;
			this.rb_weapon2.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_weapon3
			// 
			this.rb_weapon3.Location = new System.Drawing.Point(15, 415);
			this.rb_weapon3.Margin = new System.Windows.Forms.Padding(0);
			this.rb_weapon3.Name = "rb_weapon3";
			this.rb_weapon3.Size = new System.Drawing.Size(135, 20);
			this.rb_weapon3.TabIndex = 17;
			this.rb_weapon3.TabStop = true;
			this.rb_weapon3.Text = "weapon3";
			this.rb_weapon3.UseVisualStyleBackColor = true;
			this.rb_weapon3.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// bt_Accept
			// 
			this.bt_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Accept.Location = new System.Drawing.Point(89, 6);
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
			// gb_slots
			// 
			this.gb_slots.Controls.Add(this.rb_skin);
			this.gb_slots.Controls.Add(this.rb_weapon1);
			this.gb_slots.Controls.Add(this.rb_head);
			this.gb_slots.Controls.Add(this.rb_weapon3);
			this.gb_slots.Controls.Add(this.rb_chest);
			this.gb_slots.Controls.Add(this.rb_neck);
			this.gb_slots.Controls.Add(this.rb_weapon2);
			this.gb_slots.Controls.Add(this.rb_rhand);
			this.gb_slots.Controls.Add(this.rb_lhand);
			this.gb_slots.Controls.Add(this.rb_lring);
			this.gb_slots.Controls.Add(this.rb_bullets);
			this.gb_slots.Controls.Add(this.rb_wrists);
			this.gb_slots.Controls.Add(this.rb_bolts);
			this.gb_slots.Controls.Add(this.rb_rring);
			this.gb_slots.Controls.Add(this.rb_arrows);
			this.gb_slots.Controls.Add(this.rb_waist);
			this.gb_slots.Controls.Add(this.rb_back);
			this.gb_slots.Controls.Add(this.rb_feet);
			this.gb_slots.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_slots.Location = new System.Drawing.Point(0, 20);
			this.gb_slots.Margin = new System.Windows.Forms.Padding(0);
			this.gb_slots.Name = "gb_slots";
			this.gb_slots.Padding = new System.Windows.Forms.Padding(0);
			this.gb_slots.Size = new System.Drawing.Size(169, 440);
			this.gb_slots.TabIndex = 1;
			this.gb_slots.TabStop = false;
			this.gb_slots.Paint += new System.Windows.Forms.PaintEventHandler(this.paint_Group);
			// 
			// pl_bot
			// 
			this.pl_bot.Controls.Add(this.bt_Accept);
			this.pl_bot.Controls.Add(this.bt_Cancel);
			this.pl_bot.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pl_bot.Location = new System.Drawing.Point(0, 460);
			this.pl_bot.Margin = new System.Windows.Forms.Padding(0);
			this.pl_bot.Name = "pl_bot";
			this.pl_bot.Size = new System.Drawing.Size(169, 36);
			this.pl_bot.TabIndex = 2;
			// 
			// EquippedItemDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(169, 496);
			this.Controls.Add(this.gb_slots);
			this.Controls.Add(this.pl_bot);
			this.Controls.Add(this.la_head);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EquippedItemDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Equip slot";
			this.gb_slots.ResumeLayout(false);
			this.pl_bot.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
