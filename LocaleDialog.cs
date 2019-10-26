using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A dialog for user to add a Locale to a CExoLocString.
	/// </summary>
	sealed class LocaleDialog
		:
			Form
	{
//		#region Fields (static)
//		internal const uint Loc_ENGLISH     = 0x001; // bitwise values for already used locales ->
//		internal const uint Loc_FRENCH      = 0x002; // but they would need to take Gender into consideration also
//		internal const uint Loc_GERMAN      = 0x004;
//		internal const uint Loc_ITALIAN     = 0x008;
//		internal const uint Loc_SPANISH     = 0x010;
//		internal const uint Loc_POLISH      = 0x020;
//		internal const uint Loc_RUSSIAN     = 0x040;
//		internal const uint Loc_KOREAN      = 0x080;
//		internal const uint Loc_CHINESETRAD = 0x100;
//		internal const uint Loc_CHINESESIMP = 0x200;
//		internal const uint Loc_JAPANESE    = 0x400;
//		internal const uint Loc_GFFTOKEN    = 0x800;
//		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="edit">true if editing, false if adding</param>
		internal LocaleDialog(bool edit = false)
		{
			InitializeComponent();

			if (edit)
			{
				Text         = "Edit Locale";
				la_head.Text = "Edit Language type";
			}
			else
			{
				Text         = "Add Locale";
				la_head.Text = "Add Language type";
			}

			rb_English .Tag = Languages.English;
			rb_French  .Tag = Languages.French;
			rb_German  .Tag = Languages.German;
			rb_Italian .Tag = Languages.Italian;
			rb_Spanish .Tag = Languages.Spanish;
			rb_Polish  .Tag = Languages.Polish;
			rb_Russian .Tag = Languages.Russian;
			rb_Korean  .Tag = Languages.Korean;
			rb_ChineseT.Tag = Languages.ChineseTraditional;
			rb_ChineseS.Tag = Languages.ChineseSimplified;
			rb_Japanese.Tag = Languages.Japanese;
			rb_GffToken.Tag = Languages.GffToken;

			// TODO: Disallow duplicate localizations.

//			rb_English .Enabled = (bitloc & Loc_ENGLISH)     == 0;
//			rb_French  .Enabled = (bitloc & Loc_FRENCH)      == 0;
//			rb_German  .Enabled = (bitloc & Loc_GERMAN)      == 0;
//			rb_Italian .Enabled = (bitloc & Loc_ITALIAN)     == 0;
//			rb_Spanish .Enabled = (bitloc & Loc_SPANISH)     == 0;
//			rb_Polish  .Enabled = (bitloc & Loc_POLISH)      == 0;
//			rb_Russian .Enabled = (bitloc & Loc_RUSSIAN)     == 0;
//			rb_Korean  .Enabled = (bitloc & Loc_KOREAN)      == 0;
//			rb_ChineseT.Enabled = (bitloc & Loc_CHINESETRAD) == 0;
//			rb_ChineseS.Enabled = (bitloc & Loc_CHINESESIMP) == 0;
//			rb_Japanese.Enabled = (bitloc & Loc_JAPANESE)    == 0;

			switch (TreeList._langid)
			{
				case Languages.English:            rb_English .Checked = true; break;
				case Languages.French:             rb_French  .Checked = true; break;
				case Languages.German:             rb_German  .Checked = true; break;
				case Languages.Italian:            rb_Italian .Checked = true; break;
				case Languages.Spanish:            rb_Spanish .Checked = true; break;
				case Languages.Polish:             rb_Polish  .Checked = true; break;
				case Languages.Russian:            rb_Russian .Checked = true; break;
				case Languages.Korean:             rb_Korean  .Checked = true; break;
				case Languages.ChineseTraditional: rb_ChineseT.Checked = true; break;
				case Languages.ChineseSimplified:  rb_ChineseS.Checked = true; break;
				case Languages.Japanese:           rb_Japanese.Checked = true; break;
				case Languages.GffToken:           rb_GffToken.Checked = true; break;
			}
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Sets the variable's typeid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_radio(object sender, EventArgs e)
		{
			TreeList._langid = (Languages)((RadioButton)sender).Tag;
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
		GroupBox gb_locale;
		RadioButton rb_English;
		RadioButton rb_French;
		RadioButton rb_German;
		RadioButton rb_Italian;
		RadioButton rb_Spanish;
		RadioButton rb_Polish;
		RadioButton rb_Russian;
		RadioButton rb_Korean;
		RadioButton rb_ChineseT;
		RadioButton rb_ChineseS;
		RadioButton rb_Japanese;
		RadioButton rb_GffToken;


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
			this.gb_locale = new System.Windows.Forms.GroupBox();
			this.rb_GffToken = new System.Windows.Forms.RadioButton();
			this.rb_Russian = new System.Windows.Forms.RadioButton();
			this.rb_Korean = new System.Windows.Forms.RadioButton();
			this.rb_ChineseT = new System.Windows.Forms.RadioButton();
			this.rb_ChineseS = new System.Windows.Forms.RadioButton();
			this.rb_Japanese = new System.Windows.Forms.RadioButton();
			this.rb_English = new System.Windows.Forms.RadioButton();
			this.rb_French = new System.Windows.Forms.RadioButton();
			this.rb_German = new System.Windows.Forms.RadioButton();
			this.rb_Italian = new System.Windows.Forms.RadioButton();
			this.rb_Spanish = new System.Windows.Forms.RadioButton();
			this.rb_Polish = new System.Windows.Forms.RadioButton();
			this.pl_bot.SuspendLayout();
			this.gb_locale.SuspendLayout();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Size = new System.Drawing.Size(164, 20);
			this.la_head.TabIndex = 0;
			this.la_head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// bt_Accept
			// 
			this.bt_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Accept.Location = new System.Drawing.Point(84, 6);
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
			this.pl_bot.Location = new System.Drawing.Point(0, 280);
			this.pl_bot.Margin = new System.Windows.Forms.Padding(0);
			this.pl_bot.Name = "pl_bot";
			this.pl_bot.Size = new System.Drawing.Size(164, 36);
			this.pl_bot.TabIndex = 2;
			// 
			// gb_locale
			// 
			this.gb_locale.Controls.Add(this.rb_GffToken);
			this.gb_locale.Controls.Add(this.rb_Russian);
			this.gb_locale.Controls.Add(this.rb_Korean);
			this.gb_locale.Controls.Add(this.rb_ChineseT);
			this.gb_locale.Controls.Add(this.rb_ChineseS);
			this.gb_locale.Controls.Add(this.rb_Japanese);
			this.gb_locale.Controls.Add(this.rb_English);
			this.gb_locale.Controls.Add(this.rb_French);
			this.gb_locale.Controls.Add(this.rb_German);
			this.gb_locale.Controls.Add(this.rb_Italian);
			this.gb_locale.Controls.Add(this.rb_Spanish);
			this.gb_locale.Controls.Add(this.rb_Polish);
			this.gb_locale.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_locale.Location = new System.Drawing.Point(0, 20);
			this.gb_locale.Margin = new System.Windows.Forms.Padding(0);
			this.gb_locale.Name = "gb_locale";
			this.gb_locale.Size = new System.Drawing.Size(164, 260);
			this.gb_locale.TabIndex = 1;
			this.gb_locale.TabStop = false;
			// 
			// rb_GffToken
			// 
			this.rb_GffToken.Location = new System.Drawing.Point(10, 235);
			this.rb_GffToken.Margin = new System.Windows.Forms.Padding(0);
			this.rb_GffToken.Name = "rb_GffToken";
			this.rb_GffToken.Size = new System.Drawing.Size(150, 20);
			this.rb_GffToken.TabIndex = 11;
			this.rb_GffToken.TabStop = true;
			this.rb_GffToken.Text = "GffToken";
			this.rb_GffToken.UseVisualStyleBackColor = true;
			this.rb_GffToken.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Russian
			// 
			this.rb_Russian.Location = new System.Drawing.Point(10, 135);
			this.rb_Russian.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Russian.Name = "rb_Russian";
			this.rb_Russian.Size = new System.Drawing.Size(150, 20);
			this.rb_Russian.TabIndex = 6;
			this.rb_Russian.TabStop = true;
			this.rb_Russian.Text = "Russian";
			this.rb_Russian.UseVisualStyleBackColor = true;
			this.rb_Russian.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Korean
			// 
			this.rb_Korean.Location = new System.Drawing.Point(10, 155);
			this.rb_Korean.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Korean.Name = "rb_Korean";
			this.rb_Korean.Size = new System.Drawing.Size(150, 20);
			this.rb_Korean.TabIndex = 7;
			this.rb_Korean.TabStop = true;
			this.rb_Korean.Text = "Korean";
			this.rb_Korean.UseVisualStyleBackColor = true;
			this.rb_Korean.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_ChineseT
			// 
			this.rb_ChineseT.Location = new System.Drawing.Point(10, 175);
			this.rb_ChineseT.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseT.Name = "rb_ChineseT";
			this.rb_ChineseT.Size = new System.Drawing.Size(150, 20);
			this.rb_ChineseT.TabIndex = 8;
			this.rb_ChineseT.TabStop = true;
			this.rb_ChineseT.Text = "Chinese Traditional";
			this.rb_ChineseT.UseVisualStyleBackColor = true;
			this.rb_ChineseT.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_ChineseS
			// 
			this.rb_ChineseS.Location = new System.Drawing.Point(10, 195);
			this.rb_ChineseS.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseS.Name = "rb_ChineseS";
			this.rb_ChineseS.Size = new System.Drawing.Size(150, 20);
			this.rb_ChineseS.TabIndex = 9;
			this.rb_ChineseS.TabStop = true;
			this.rb_ChineseS.Text = "Chinese Simplified";
			this.rb_ChineseS.UseVisualStyleBackColor = true;
			this.rb_ChineseS.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Japanese
			// 
			this.rb_Japanese.Location = new System.Drawing.Point(10, 215);
			this.rb_Japanese.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Japanese.Name = "rb_Japanese";
			this.rb_Japanese.Size = new System.Drawing.Size(150, 20);
			this.rb_Japanese.TabIndex = 10;
			this.rb_Japanese.TabStop = true;
			this.rb_Japanese.Text = "Japanese";
			this.rb_Japanese.UseVisualStyleBackColor = true;
			this.rb_Japanese.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_English
			// 
			this.rb_English.Location = new System.Drawing.Point(10, 15);
			this.rb_English.Margin = new System.Windows.Forms.Padding(0);
			this.rb_English.Name = "rb_English";
			this.rb_English.Size = new System.Drawing.Size(150, 20);
			this.rb_English.TabIndex = 0;
			this.rb_English.TabStop = true;
			this.rb_English.Text = "English";
			this.rb_English.UseVisualStyleBackColor = true;
			this.rb_English.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_French
			// 
			this.rb_French.Location = new System.Drawing.Point(10, 35);
			this.rb_French.Margin = new System.Windows.Forms.Padding(0);
			this.rb_French.Name = "rb_French";
			this.rb_French.Size = new System.Drawing.Size(150, 20);
			this.rb_French.TabIndex = 1;
			this.rb_French.TabStop = true;
			this.rb_French.Text = "French";
			this.rb_French.UseVisualStyleBackColor = true;
			this.rb_French.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_German
			// 
			this.rb_German.Location = new System.Drawing.Point(10, 55);
			this.rb_German.Margin = new System.Windows.Forms.Padding(0);
			this.rb_German.Name = "rb_German";
			this.rb_German.Size = new System.Drawing.Size(150, 20);
			this.rb_German.TabIndex = 2;
			this.rb_German.TabStop = true;
			this.rb_German.Text = "German";
			this.rb_German.UseVisualStyleBackColor = true;
			this.rb_German.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Italian
			// 
			this.rb_Italian.Location = new System.Drawing.Point(10, 75);
			this.rb_Italian.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Italian.Name = "rb_Italian";
			this.rb_Italian.Size = new System.Drawing.Size(150, 20);
			this.rb_Italian.TabIndex = 3;
			this.rb_Italian.TabStop = true;
			this.rb_Italian.Text = "Italian";
			this.rb_Italian.UseVisualStyleBackColor = true;
			this.rb_Italian.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Spanish
			// 
			this.rb_Spanish.Location = new System.Drawing.Point(10, 95);
			this.rb_Spanish.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Spanish.Name = "rb_Spanish";
			this.rb_Spanish.Size = new System.Drawing.Size(150, 20);
			this.rb_Spanish.TabIndex = 4;
			this.rb_Spanish.TabStop = true;
			this.rb_Spanish.Text = "Spanish";
			this.rb_Spanish.UseVisualStyleBackColor = true;
			this.rb_Spanish.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// rb_Polish
			// 
			this.rb_Polish.Location = new System.Drawing.Point(10, 115);
			this.rb_Polish.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Polish.Name = "rb_Polish";
			this.rb_Polish.Size = new System.Drawing.Size(150, 20);
			this.rb_Polish.TabIndex = 5;
			this.rb_Polish.TabStop = true;
			this.rb_Polish.Text = "Polish";
			this.rb_Polish.UseVisualStyleBackColor = true;
			this.rb_Polish.CheckedChanged += new System.EventHandler(this.checkchanged_radio);
			// 
			// LocaleDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(164, 316);
			this.Controls.Add(this.gb_locale);
			this.Controls.Add(this.pl_bot);
			this.Controls.Add(this.la_head);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LocaleDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.pl_bot.ResumeLayout(false);
			this.gb_locale.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
