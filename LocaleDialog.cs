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
		#region Fields (static)
		internal const uint Loc_non           = 0x000000; // error.

		internal const uint Loc_ENGLISH       = 0x000001; // bitwise values for already used locales ->
//		internal const uint Loc_ENGLISH_F     = 0x000002; // NOTE: Toolset doesn't allow English[F].
		internal const uint Loc_FRENCH        = 0x000004;
		internal const uint Loc_FRENCH_F      = 0x000008;
		internal const uint Loc_GERMAN        = 0x000010;
		internal const uint Loc_GERMAN_F      = 0x000020;
		internal const uint Loc_ITALIAN       = 0x000040;
		internal const uint Loc_ITALIAN_F     = 0x000080;
		internal const uint Loc_SPANISH       = 0x000100;
		internal const uint Loc_SPANISH_F     = 0x000200;
		internal const uint Loc_POLISH        = 0x000400;
		internal const uint Loc_POLISH_F      = 0x000800;
		internal const uint Loc_RUSSIAN       = 0x001000;
		internal const uint Loc_RUSSIAN_F     = 0x002000;
		internal const uint Loc_KOREAN        = 0x004000;
		internal const uint Loc_KOREAN_F      = 0x008000;
		internal const uint Loc_CHINESETRAD   = 0x010000;
		internal const uint Loc_CHINESETRAD_F = 0x020000;
		internal const uint Loc_CHINESESIMP   = 0x040000;
		internal const uint Loc_CHINESESIMP_F = 0x080000;
		internal const uint Loc_JAPANESE      = 0x100000;
		internal const uint Loc_JAPANESE_F    = 0x200000;

		internal const uint Loc_GFFTOKEN      = 0x400000;

		internal const uint Loc_ALL           = 0x7FFFFD;
		#endregion Fields (static)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="bitloc">bitflags denoting already used locales</param>
		/// <param name="edit">true if editing, false if adding</param>
		internal LocaleDialog(uint bitloc, bool edit = false)
		{
			//logfile.Log("bitloc= " + bin(bitloc));

			InitializeComponent();

			if (edit) la_head.Text = "edit Locale";
			else      la_head.Text = "add Locale";

			rb_English .Tag                    = Languages.English;
			rb_French  .Tag = rb_FrenchF  .Tag = Languages.French;
			rb_German  .Tag = rb_GermanF  .Tag = Languages.German;
			rb_Italian .Tag = rb_ItalianF .Tag = Languages.Italian;
			rb_Spanish .Tag = rb_SpanishF .Tag = Languages.Spanish;
			rb_Polish  .Tag = rb_PolishF  .Tag = Languages.Polish;
			rb_Russian .Tag = rb_RussianF .Tag = Languages.Russian;
			rb_Korean  .Tag = rb_KoreanF  .Tag = Languages.Korean;
			rb_ChineseT.Tag = rb_ChineseTF.Tag = Languages.ChineseTraditional;
			rb_ChineseS.Tag = rb_ChineseSF.Tag = Languages.ChineseSimplified;
			rb_Japanese.Tag = rb_JapaneseF.Tag = Languages.Japanese;
			rb_GffToken.Tag                    = Languages.GffToken;

			rb_English  .Enabled = (bitloc & Loc_ENGLISH)       == 0;
			rb_French   .Enabled = (bitloc & Loc_FRENCH)        == 0;
			rb_German   .Enabled = (bitloc & Loc_GERMAN)        == 0;
			rb_Italian  .Enabled = (bitloc & Loc_ITALIAN)       == 0;
			rb_Spanish  .Enabled = (bitloc & Loc_SPANISH)       == 0;
			rb_Polish   .Enabled = (bitloc & Loc_POLISH)        == 0;
			rb_Russian  .Enabled = (bitloc & Loc_RUSSIAN)       == 0;
			rb_Korean   .Enabled = (bitloc & Loc_KOREAN)        == 0;
			rb_ChineseT .Enabled = (bitloc & Loc_CHINESETRAD)   == 0;
			rb_ChineseS .Enabled = (bitloc & Loc_CHINESESIMP)   == 0;
			rb_Japanese .Enabled = (bitloc & Loc_JAPANESE)      == 0;

			rb_FrenchF  .Enabled = (bitloc & Loc_FRENCH_F)      == 0;
			rb_GermanF  .Enabled = (bitloc & Loc_GERMAN_F)      == 0;
			rb_ItalianF .Enabled = (bitloc & Loc_ITALIAN_F)     == 0;
			rb_SpanishF .Enabled = (bitloc & Loc_SPANISH_F)     == 0;
			rb_PolishF  .Enabled = (bitloc & Loc_POLISH_F)      == 0;
			rb_RussianF .Enabled = (bitloc & Loc_RUSSIAN_F)     == 0;
			rb_KoreanF  .Enabled = (bitloc & Loc_KOREAN_F)      == 0;
			rb_ChineseTF.Enabled = (bitloc & Loc_CHINESETRAD_F) == 0;
			rb_ChineseSF.Enabled = (bitloc & Loc_CHINESESIMP_F) == 0;
			rb_JapaneseF.Enabled = (bitloc & Loc_JAPANESE_F)    == 0;

			rb_GffToken .Enabled = (bitloc & Loc_GFFTOKEN)      == 0;

			if (edit) // else let .NET choose a default
			{
				if (TreeList._langid == Languages.GffToken)
				{
					rb_GffToken.Checked = true;
				}
				else if (TreeList._langf)
				{
					switch (TreeList._langid)
					{
						case Languages.French:             rb_FrenchF  .Checked = true; break;
						case Languages.German:             rb_GermanF  .Checked = true; break;
						case Languages.Italian:            rb_ItalianF .Checked = true; break;
						case Languages.Spanish:            rb_SpanishF .Checked = true; break;
						case Languages.Polish:             rb_PolishF  .Checked = true; break;
						case Languages.Russian:            rb_RussianF .Checked = true; break;
						case Languages.Korean:             rb_KoreanF  .Checked = true; break;
						case Languages.ChineseTraditional: rb_ChineseTF.Checked = true; break;
						case Languages.ChineseSimplified:  rb_ChineseSF.Checked = true; break;
						case Languages.Japanese:           rb_JapaneseF.Checked = true; break;
					}
				}
				else
				{
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
					}
				}
			}
		}
		#endregion cTor

/*		static string bin(uint bitloc)
		{
			string ret = Convert.ToString(bitloc, 2);
			ret = ret.PadLeft(32, '0');

			ret = ret.Insert(24, " ");
			ret = ret.Insert(16, " ");
			ret = ret.Insert( 8, " ");

			return ret;
		} */

		#region Handlers
		/// <summary>
		/// Sets the variable's typeid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged(object sender, EventArgs e)
		{
			var rb = sender as RadioButton;
			TreeList._langid = (Languages)rb.Tag;
			TreeList._langf  = rb.Name.EndsWith("F", StringComparison.Ordinal);
		}
		#endregion Handlers


		#region Methods (static)
		/// <summary>
		/// Sets a locale flag.
		/// </summary>
		/// <param name="bitloc"></param>
		/// <param name="langid"></param>
		/// <param name="f"></param>
		internal static void SetLocaleFlag(ref uint bitloc, Languages langid, bool f)
		{
			switch (langid)
			{
				case Languages.English:
					bitloc |= Loc_ENGLISH;
					break;

				case Languages.French:
					if (f) bitloc |= Loc_FRENCH_F;
					else   bitloc |= Loc_FRENCH;
					break;

				case Languages.German:
					if (f) bitloc |= Loc_GERMAN_F;
					else   bitloc |= Loc_GERMAN;
					break;

				case Languages.Italian:
					if (f) bitloc |= Loc_ITALIAN_F;
					else   bitloc |= Loc_ITALIAN;
					break;

				case Languages.Spanish:
					if (f) bitloc |= Loc_SPANISH_F;
					else   bitloc |= Loc_SPANISH;
					break;

				case Languages.Polish:
					if (f) bitloc |= Loc_POLISH_F;
					else   bitloc |= Loc_POLISH;
					break;

				case Languages.Russian:
					if (f) bitloc |= Loc_RUSSIAN_F;
					else   bitloc |= Loc_RUSSIAN;
					break;

				case Languages.Korean:
					if (f) bitloc |= Loc_KOREAN_F;
					else   bitloc |= Loc_KOREAN;
					break;

				case Languages.ChineseTraditional:
					if (f) bitloc |= Loc_CHINESETRAD_F;
					else   bitloc |= Loc_CHINESETRAD;
					break;

				case Languages.ChineseSimplified:
					if (f) bitloc |= Loc_CHINESESIMP_F;
					else   bitloc |= Loc_CHINESESIMP;
					break;

				case Languages.Japanese:
					if (f) bitloc |= Loc_JAPANESE_F;
					else   bitloc |= Loc_JAPANESE;
					break;

				case Languages.GffToken:
					bitloc |= Loc_GFFTOKEN;
					break;
			}
		}

		/// <summary>
		/// Clears a locale flag.
		/// </summary>
		/// <param name="bitloc"></param>
		/// <param name="langid"></param>
		/// <param name="f"></param>
		internal static void ClearLocaleFlag(ref uint bitloc, Languages langid, bool f)
		{
			switch (langid)
			{
				case Languages.English:
					bitloc &= ~Loc_ENGLISH;
					break;

				case Languages.French:
					if (f) bitloc &= ~Loc_FRENCH_F;
					else   bitloc &= ~Loc_FRENCH;
					break;

				case Languages.German:
					if (f) bitloc &= ~Loc_GERMAN_F;
					else   bitloc &= ~Loc_GERMAN;
					break;

				case Languages.Italian:
					if (f) bitloc &= ~Loc_ITALIAN_F;
					else   bitloc &= ~Loc_ITALIAN;
					break;

				case Languages.Spanish:
					if (f) bitloc &= ~Loc_SPANISH_F;
					else   bitloc &= ~Loc_SPANISH;
					break;

				case Languages.Polish:
					if (f) bitloc &= ~Loc_POLISH_F;
					else   bitloc &= ~Loc_POLISH;
					break;

				case Languages.Russian:
					if (f) bitloc &= ~Loc_RUSSIAN_F;
					else   bitloc &= ~Loc_RUSSIAN;
					break;

				case Languages.Korean:
					if (f) bitloc &= ~Loc_KOREAN_F;
					else   bitloc &= ~Loc_KOREAN;
					break;

				case Languages.ChineseTraditional:
					if (f) bitloc &= ~Loc_CHINESETRAD_F;
					else   bitloc &= ~Loc_CHINESETRAD;
					break;

				case Languages.ChineseSimplified:
					if (f) bitloc &= ~Loc_CHINESESIMP_F;
					else   bitloc &= ~Loc_CHINESESIMP;
					break;

				case Languages.Japanese:
					if (f) bitloc &= ~Loc_JAPANESE_F;
					else   bitloc &= ~Loc_JAPANESE;
					break;

				case Languages.GffToken:
					bitloc &= ~Loc_GFFTOKEN;
					break;
			}
		}

		/// <summary>
		/// Gets a Locale's locale-flag.
		/// </summary>
		/// <param name="locale"></param>
		/// <returns></returns>
		internal static uint GetLocaleFlag(GffData.Locale locale)
		{
			switch (locale.langid)
			{
				case Languages.English:
					return Loc_ENGLISH;

				case Languages.French:
					if (locale.F) return Loc_FRENCH_F;
					return Loc_FRENCH;

				case Languages.German:
					if (locale.F) return Loc_GERMAN_F;
					return Loc_GERMAN;

				case Languages.Italian:
					if (locale.F) return Loc_ITALIAN_F;
					return Loc_ITALIAN;

				case Languages.Spanish:
					if (locale.F) return Loc_SPANISH_F;
					return Loc_SPANISH;

				case Languages.Polish:
					if (locale.F) return Loc_POLISH_F;
					return Loc_POLISH;

				case Languages.Russian:
					if (locale.F) return Loc_RUSSIAN_F;
					return Loc_RUSSIAN;

				case Languages.Korean:
					if (locale.F) return Loc_KOREAN_F;
					return Loc_KOREAN;

				case Languages.ChineseTraditional:
					if (locale.F) return Loc_CHINESETRAD_F;
					return Loc_CHINESETRAD;

				case Languages.ChineseSimplified:
					if (locale.F) return Loc_CHINESESIMP_F;
					return Loc_CHINESESIMP;

				case Languages.Japanese:
					if (locale.F) return Loc_JAPANESE_F;
					return Loc_JAPANESE;

				case Languages.GffToken:
					return Loc_GFFTOKEN;
			}
			return Loc_non; // error.
		}
		#endregion Methods (static)



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
		RadioButton rb_RussianF;
		RadioButton rb_KoreanF;
		RadioButton rb_ChineseTF;
		RadioButton rb_ChineseSF;
		RadioButton rb_JapaneseF;
		RadioButton rb_FrenchF;
		RadioButton rb_GermanF;
		RadioButton rb_ItalianF;
		RadioButton rb_SpanishF;
		RadioButton rb_PolishF;


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
			this.rb_RussianF = new System.Windows.Forms.RadioButton();
			this.rb_KoreanF = new System.Windows.Forms.RadioButton();
			this.rb_ChineseTF = new System.Windows.Forms.RadioButton();
			this.rb_ChineseSF = new System.Windows.Forms.RadioButton();
			this.rb_JapaneseF = new System.Windows.Forms.RadioButton();
			this.rb_FrenchF = new System.Windows.Forms.RadioButton();
			this.rb_GermanF = new System.Windows.Forms.RadioButton();
			this.rb_ItalianF = new System.Windows.Forms.RadioButton();
			this.rb_SpanishF = new System.Windows.Forms.RadioButton();
			this.rb_PolishF = new System.Windows.Forms.RadioButton();
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
			this.la_head.Size = new System.Drawing.Size(319, 20);
			this.la_head.TabIndex = 0;
			this.la_head.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// bt_Accept
			// 
			this.bt_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bt_Accept.Location = new System.Drawing.Point(239, 6);
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
			this.pl_bot.Size = new System.Drawing.Size(319, 36);
			this.pl_bot.TabIndex = 2;
			// 
			// gb_locale
			// 
			this.gb_locale.Controls.Add(this.rb_RussianF);
			this.gb_locale.Controls.Add(this.rb_KoreanF);
			this.gb_locale.Controls.Add(this.rb_ChineseTF);
			this.gb_locale.Controls.Add(this.rb_ChineseSF);
			this.gb_locale.Controls.Add(this.rb_JapaneseF);
			this.gb_locale.Controls.Add(this.rb_FrenchF);
			this.gb_locale.Controls.Add(this.rb_GermanF);
			this.gb_locale.Controls.Add(this.rb_ItalianF);
			this.gb_locale.Controls.Add(this.rb_SpanishF);
			this.gb_locale.Controls.Add(this.rb_PolishF);
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
			this.gb_locale.Size = new System.Drawing.Size(319, 260);
			this.gb_locale.TabIndex = 1;
			this.gb_locale.TabStop = false;
			// 
			// rb_RussianF
			// 
			this.rb_RussianF.Location = new System.Drawing.Point(155, 135);
			this.rb_RussianF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_RussianF.Name = "rb_RussianF";
			this.rb_RussianF.Size = new System.Drawing.Size(160, 20);
			this.rb_RussianF.TabIndex = 12;
			this.rb_RussianF.TabStop = true;
			this.rb_RussianF.Text = "Russian[F]";
			this.rb_RussianF.UseVisualStyleBackColor = true;
			this.rb_RussianF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_KoreanF
			// 
			this.rb_KoreanF.Location = new System.Drawing.Point(155, 155);
			this.rb_KoreanF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_KoreanF.Name = "rb_KoreanF";
			this.rb_KoreanF.Size = new System.Drawing.Size(160, 20);
			this.rb_KoreanF.TabIndex = 14;
			this.rb_KoreanF.TabStop = true;
			this.rb_KoreanF.Text = "Korean[F]";
			this.rb_KoreanF.UseVisualStyleBackColor = true;
			this.rb_KoreanF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ChineseTF
			// 
			this.rb_ChineseTF.Location = new System.Drawing.Point(155, 175);
			this.rb_ChineseTF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseTF.Name = "rb_ChineseTF";
			this.rb_ChineseTF.Size = new System.Drawing.Size(160, 20);
			this.rb_ChineseTF.TabIndex = 16;
			this.rb_ChineseTF.TabStop = true;
			this.rb_ChineseTF.Text = "Chinese Traditional[F]";
			this.rb_ChineseTF.UseVisualStyleBackColor = true;
			this.rb_ChineseTF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ChineseSF
			// 
			this.rb_ChineseSF.Location = new System.Drawing.Point(155, 195);
			this.rb_ChineseSF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseSF.Name = "rb_ChineseSF";
			this.rb_ChineseSF.Size = new System.Drawing.Size(160, 20);
			this.rb_ChineseSF.TabIndex = 18;
			this.rb_ChineseSF.TabStop = true;
			this.rb_ChineseSF.Text = "Chinese Simplified[F]";
			this.rb_ChineseSF.UseVisualStyleBackColor = true;
			this.rb_ChineseSF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_JapaneseF
			// 
			this.rb_JapaneseF.Location = new System.Drawing.Point(155, 215);
			this.rb_JapaneseF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_JapaneseF.Name = "rb_JapaneseF";
			this.rb_JapaneseF.Size = new System.Drawing.Size(160, 20);
			this.rb_JapaneseF.TabIndex = 20;
			this.rb_JapaneseF.TabStop = true;
			this.rb_JapaneseF.Text = "Japanese[F]";
			this.rb_JapaneseF.UseVisualStyleBackColor = true;
			this.rb_JapaneseF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_FrenchF
			// 
			this.rb_FrenchF.Location = new System.Drawing.Point(155, 35);
			this.rb_FrenchF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_FrenchF.Name = "rb_FrenchF";
			this.rb_FrenchF.Size = new System.Drawing.Size(160, 20);
			this.rb_FrenchF.TabIndex = 2;
			this.rb_FrenchF.TabStop = true;
			this.rb_FrenchF.Text = "French[F]";
			this.rb_FrenchF.UseVisualStyleBackColor = true;
			this.rb_FrenchF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_GermanF
			// 
			this.rb_GermanF.Location = new System.Drawing.Point(155, 55);
			this.rb_GermanF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_GermanF.Name = "rb_GermanF";
			this.rb_GermanF.Size = new System.Drawing.Size(160, 20);
			this.rb_GermanF.TabIndex = 4;
			this.rb_GermanF.TabStop = true;
			this.rb_GermanF.Text = "German[F]";
			this.rb_GermanF.UseVisualStyleBackColor = true;
			this.rb_GermanF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ItalianF
			// 
			this.rb_ItalianF.Location = new System.Drawing.Point(155, 75);
			this.rb_ItalianF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ItalianF.Name = "rb_ItalianF";
			this.rb_ItalianF.Size = new System.Drawing.Size(160, 20);
			this.rb_ItalianF.TabIndex = 6;
			this.rb_ItalianF.TabStop = true;
			this.rb_ItalianF.Text = "Italian[F]";
			this.rb_ItalianF.UseVisualStyleBackColor = true;
			this.rb_ItalianF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_SpanishF
			// 
			this.rb_SpanishF.Location = new System.Drawing.Point(155, 95);
			this.rb_SpanishF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_SpanishF.Name = "rb_SpanishF";
			this.rb_SpanishF.Size = new System.Drawing.Size(160, 20);
			this.rb_SpanishF.TabIndex = 8;
			this.rb_SpanishF.TabStop = true;
			this.rb_SpanishF.Text = "Spanish[F]";
			this.rb_SpanishF.UseVisualStyleBackColor = true;
			this.rb_SpanishF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_PolishF
			// 
			this.rb_PolishF.Location = new System.Drawing.Point(155, 115);
			this.rb_PolishF.Margin = new System.Windows.Forms.Padding(0);
			this.rb_PolishF.Name = "rb_PolishF";
			this.rb_PolishF.Size = new System.Drawing.Size(160, 20);
			this.rb_PolishF.TabIndex = 10;
			this.rb_PolishF.TabStop = true;
			this.rb_PolishF.Text = "Polish[F]";
			this.rb_PolishF.UseVisualStyleBackColor = true;
			this.rb_PolishF.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_GffToken
			// 
			this.rb_GffToken.Location = new System.Drawing.Point(10, 235);
			this.rb_GffToken.Margin = new System.Windows.Forms.Padding(0);
			this.rb_GffToken.Name = "rb_GffToken";
			this.rb_GffToken.Size = new System.Drawing.Size(145, 20);
			this.rb_GffToken.TabIndex = 21;
			this.rb_GffToken.TabStop = true;
			this.rb_GffToken.Text = "GffToken";
			this.rb_GffToken.UseVisualStyleBackColor = true;
			this.rb_GffToken.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Russian
			// 
			this.rb_Russian.Location = new System.Drawing.Point(10, 135);
			this.rb_Russian.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Russian.Name = "rb_Russian";
			this.rb_Russian.Size = new System.Drawing.Size(145, 20);
			this.rb_Russian.TabIndex = 11;
			this.rb_Russian.TabStop = true;
			this.rb_Russian.Text = "Russian";
			this.rb_Russian.UseVisualStyleBackColor = true;
			this.rb_Russian.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Korean
			// 
			this.rb_Korean.Location = new System.Drawing.Point(10, 155);
			this.rb_Korean.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Korean.Name = "rb_Korean";
			this.rb_Korean.Size = new System.Drawing.Size(145, 20);
			this.rb_Korean.TabIndex = 13;
			this.rb_Korean.TabStop = true;
			this.rb_Korean.Text = "Korean";
			this.rb_Korean.UseVisualStyleBackColor = true;
			this.rb_Korean.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ChineseT
			// 
			this.rb_ChineseT.Location = new System.Drawing.Point(10, 175);
			this.rb_ChineseT.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseT.Name = "rb_ChineseT";
			this.rb_ChineseT.Size = new System.Drawing.Size(145, 20);
			this.rb_ChineseT.TabIndex = 15;
			this.rb_ChineseT.TabStop = true;
			this.rb_ChineseT.Text = "Chinese Traditional";
			this.rb_ChineseT.UseVisualStyleBackColor = true;
			this.rb_ChineseT.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ChineseS
			// 
			this.rb_ChineseS.Location = new System.Drawing.Point(10, 195);
			this.rb_ChineseS.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ChineseS.Name = "rb_ChineseS";
			this.rb_ChineseS.Size = new System.Drawing.Size(145, 20);
			this.rb_ChineseS.TabIndex = 17;
			this.rb_ChineseS.TabStop = true;
			this.rb_ChineseS.Text = "Chinese Simplified";
			this.rb_ChineseS.UseVisualStyleBackColor = true;
			this.rb_ChineseS.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Japanese
			// 
			this.rb_Japanese.Location = new System.Drawing.Point(10, 215);
			this.rb_Japanese.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Japanese.Name = "rb_Japanese";
			this.rb_Japanese.Size = new System.Drawing.Size(145, 20);
			this.rb_Japanese.TabIndex = 19;
			this.rb_Japanese.TabStop = true;
			this.rb_Japanese.Text = "Japanese";
			this.rb_Japanese.UseVisualStyleBackColor = true;
			this.rb_Japanese.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_English
			// 
			this.rb_English.Location = new System.Drawing.Point(10, 15);
			this.rb_English.Margin = new System.Windows.Forms.Padding(0);
			this.rb_English.Name = "rb_English";
			this.rb_English.Size = new System.Drawing.Size(145, 20);
			this.rb_English.TabIndex = 0;
			this.rb_English.TabStop = true;
			this.rb_English.Text = "English";
			this.rb_English.UseVisualStyleBackColor = true;
			this.rb_English.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_French
			// 
			this.rb_French.Location = new System.Drawing.Point(10, 35);
			this.rb_French.Margin = new System.Windows.Forms.Padding(0);
			this.rb_French.Name = "rb_French";
			this.rb_French.Size = new System.Drawing.Size(145, 20);
			this.rb_French.TabIndex = 1;
			this.rb_French.TabStop = true;
			this.rb_French.Text = "French";
			this.rb_French.UseVisualStyleBackColor = true;
			this.rb_French.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_German
			// 
			this.rb_German.Location = new System.Drawing.Point(10, 55);
			this.rb_German.Margin = new System.Windows.Forms.Padding(0);
			this.rb_German.Name = "rb_German";
			this.rb_German.Size = new System.Drawing.Size(145, 20);
			this.rb_German.TabIndex = 3;
			this.rb_German.TabStop = true;
			this.rb_German.Text = "German";
			this.rb_German.UseVisualStyleBackColor = true;
			this.rb_German.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Italian
			// 
			this.rb_Italian.Location = new System.Drawing.Point(10, 75);
			this.rb_Italian.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Italian.Name = "rb_Italian";
			this.rb_Italian.Size = new System.Drawing.Size(145, 20);
			this.rb_Italian.TabIndex = 5;
			this.rb_Italian.TabStop = true;
			this.rb_Italian.Text = "Italian";
			this.rb_Italian.UseVisualStyleBackColor = true;
			this.rb_Italian.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Spanish
			// 
			this.rb_Spanish.Location = new System.Drawing.Point(10, 95);
			this.rb_Spanish.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Spanish.Name = "rb_Spanish";
			this.rb_Spanish.Size = new System.Drawing.Size(145, 20);
			this.rb_Spanish.TabIndex = 7;
			this.rb_Spanish.TabStop = true;
			this.rb_Spanish.Text = "Spanish";
			this.rb_Spanish.UseVisualStyleBackColor = true;
			this.rb_Spanish.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_Polish
			// 
			this.rb_Polish.Location = new System.Drawing.Point(10, 115);
			this.rb_Polish.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Polish.Name = "rb_Polish";
			this.rb_Polish.Size = new System.Drawing.Size(145, 20);
			this.rb_Polish.TabIndex = 9;
			this.rb_Polish.TabStop = true;
			this.rb_Polish.Text = "Polish";
			this.rb_Polish.UseVisualStyleBackColor = true;
			this.rb_Polish.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// LocaleDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(319, 316);
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
			this.Text = "Locale";
			this.pl_bot.ResumeLayout(false);
			this.gb_locale.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
