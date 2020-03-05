using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A dialog for user to choose a standard GFF-type.
	/// </summary>
	sealed class TypeDialog
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TypeDialog()
		{
			InitializeComponent();
			SetTooltips();

			rb_Gen.Tag = GffType.generic;
			rb_ARE.Tag = GffType.ARE;
			rb_BIC.Tag = GffType.BIC;
			rb_CAM.Tag = GffType.CAM;
			rb_DLG.Tag = GffType.DLG;
			rb_FAC.Tag = GffType.FAC;
			rb_GIC.Tag = GffType.GIC;
			rb_GIT.Tag = GffType.GIT;
			rb_IFO.Tag = GffType.IFO;
			rb_JRL.Tag = GffType.JRL;
			rb_ROS.Tag = GffType.ROS;
			rb_RST.Tag = GffType.RST;
			rb_ULT.Tag = GffType.ULT;
			rb_UPE.Tag = GffType.UPE;
			rb_UTC.Tag = GffType.UTC;
			rb_UTD.Tag = GffType.UTD;
			rb_UTE.Tag = GffType.UTE;
			rb_UTI.Tag = GffType.UTI;
			rb_UTM.Tag = GffType.UTM;
			rb_UTP.Tag = GffType.UTP;
			rb_UTR.Tag = GffType.UTR;
			rb_UTS.Tag = GffType.UTS;
			rb_UTT.Tag = GffType.UTT;
			rb_UTW.Tag = GffType.UTW;
			rb_WMP.Tag = GffType.WMP;

			switch (TreeList._typeid)
			{
				case GffType.generic: rb_Gen.Checked = true; break;
				case GffType.ARE:     rb_ARE.Checked = true; break;
				case GffType.BIC:     rb_BIC.Checked = true; break;
				case GffType.CAM:     rb_CAM.Checked = true; break;
				case GffType.DLG:     rb_DLG.Checked = true; break;
				case GffType.FAC:     rb_FAC.Checked = true; break;
				case GffType.GIC:     rb_GIC.Checked = true; break;
				case GffType.GIT:     rb_GIT.Checked = true; break;
				case GffType.IFO:     rb_IFO.Checked = true; break;
				case GffType.JRL:     rb_JRL.Checked = true; break;
				case GffType.ROS:     rb_ROS.Checked = true; break;
				case GffType.RST:     rb_RST.Checked = true; break;
				case GffType.ULT:     rb_ULT.Checked = true; break;
				case GffType.UPE:     rb_UPE.Checked = true; break;
				case GffType.UTC:     rb_UTC.Checked = true; break;
				case GffType.UTD:     rb_UTD.Checked = true; break;
				case GffType.UTE:     rb_UTE.Checked = true; break;
				case GffType.UTI:     rb_UTI.Checked = true; break;
				case GffType.UTM:     rb_UTM.Checked = true; break;
				case GffType.UTP:     rb_UTP.Checked = true; break;
				case GffType.UTR:     rb_UTR.Checked = true; break;
				case GffType.UTS:     rb_UTS.Checked = true; break;
				case GffType.UTT:     rb_UTT.Checked = true; break;
				case GffType.UTW:     rb_UTW.Checked = true; break;
				case GffType.WMP:     rb_WMP.Checked = true; break;
			}
		}


		/// <summary>
		/// Assigns tooltips to the radio-buttons.
		/// </summary>
		void SetTooltips()
		{
			toolTip1.SetToolTip(rb_Gen, GffData.GFF);
			toolTip1.SetToolTip(rb_ARE, GffData.ARE);
			toolTip1.SetToolTip(rb_BIC, GffData.BIC);
			toolTip1.SetToolTip(rb_CAM, GffData.CAM);
			toolTip1.SetToolTip(rb_DLG, GffData.DLG);
			toolTip1.SetToolTip(rb_FAC, GffData.FAC);
			toolTip1.SetToolTip(rb_GIC, GffData.GIC);
			toolTip1.SetToolTip(rb_GIT, GffData.GIT);
			toolTip1.SetToolTip(rb_IFO, GffData.IFO);
			toolTip1.SetToolTip(rb_JRL, GffData.JRL);
			toolTip1.SetToolTip(rb_ROS, GffData.ROS);
			toolTip1.SetToolTip(rb_RST, GffData.RST);
			toolTip1.SetToolTip(rb_ULT, GffData.ULT);
			toolTip1.SetToolTip(rb_UPE, GffData.UPE);
			toolTip1.SetToolTip(rb_UTC, GffData.UTC);
			toolTip1.SetToolTip(rb_UTD, GffData.UTD);
			toolTip1.SetToolTip(rb_UTE, GffData.UTE);
			toolTip1.SetToolTip(rb_UTI, GffData.UTI);
			toolTip1.SetToolTip(rb_UTM, GffData.UTM);
			toolTip1.SetToolTip(rb_UTP, GffData.UTP);
			toolTip1.SetToolTip(rb_UTR, GffData.UTR);
			toolTip1.SetToolTip(rb_UTS, GffData.UTS);
			toolTip1.SetToolTip(rb_UTT, GffData.UTT);
			toolTip1.SetToolTip(rb_UTW, GffData.UTW);
			toolTip1.SetToolTip(rb_WMP, GffData.WMP);
		}
		#endregion cTor


		#region Handlers
		/// <summary>
		/// Sets the variable's typeid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged(object sender, EventArgs e)
		{
			TreeList._typeid = (GffType)((RadioButton)sender).Tag;
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
		RadioButton rb_Gen;
		RadioButton rb_ARE;
		RadioButton rb_BIC;
		RadioButton rb_CAM;
		RadioButton rb_DLG;
		RadioButton rb_FAC;
		RadioButton rb_GIC;
		RadioButton rb_GIT;
		RadioButton rb_IFO;
		RadioButton rb_JRL;
		RadioButton rb_ROS;
		RadioButton rb_RST;
		RadioButton rb_ULT;
		RadioButton rb_UPE;
		RadioButton rb_UTC;
		RadioButton rb_UTD;
		RadioButton rb_UTE;
		RadioButton rb_UTI;
		RadioButton rb_UTM;
		RadioButton rb_UTP;
		RadioButton rb_UTR;
		RadioButton rb_UTS;
		RadioButton rb_UTT;
		RadioButton rb_UTW;
		RadioButton rb_WMP;

		ToolTip toolTip1;


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
			this.components = new System.ComponentModel.Container();
			this.la_head = new System.Windows.Forms.Label();
			this.bt_Accept = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.pl_bot = new System.Windows.Forms.Panel();
			this.gb_locale = new System.Windows.Forms.GroupBox();
			this.rb_Gen = new System.Windows.Forms.RadioButton();
			this.rb_ARE = new System.Windows.Forms.RadioButton();
			this.rb_BIC = new System.Windows.Forms.RadioButton();
			this.rb_CAM = new System.Windows.Forms.RadioButton();
			this.rb_DLG = new System.Windows.Forms.RadioButton();
			this.rb_FAC = new System.Windows.Forms.RadioButton();
			this.rb_GIC = new System.Windows.Forms.RadioButton();
			this.rb_GIT = new System.Windows.Forms.RadioButton();
			this.rb_IFO = new System.Windows.Forms.RadioButton();
			this.rb_JRL = new System.Windows.Forms.RadioButton();
			this.rb_ROS = new System.Windows.Forms.RadioButton();
			this.rb_RST = new System.Windows.Forms.RadioButton();
			this.rb_ULT = new System.Windows.Forms.RadioButton();
			this.rb_UPE = new System.Windows.Forms.RadioButton();
			this.rb_UTC = new System.Windows.Forms.RadioButton();
			this.rb_UTD = new System.Windows.Forms.RadioButton();
			this.rb_UTE = new System.Windows.Forms.RadioButton();
			this.rb_UTI = new System.Windows.Forms.RadioButton();
			this.rb_UTM = new System.Windows.Forms.RadioButton();
			this.rb_UTP = new System.Windows.Forms.RadioButton();
			this.rb_UTR = new System.Windows.Forms.RadioButton();
			this.rb_UTS = new System.Windows.Forms.RadioButton();
			this.rb_UTT = new System.Windows.Forms.RadioButton();
			this.rb_UTW = new System.Windows.Forms.RadioButton();
			this.rb_WMP = new System.Windows.Forms.RadioButton();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
			this.la_head.Text = "edit GFF Type";
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
			this.pl_bot.Location = new System.Drawing.Point(0, 300);
			this.pl_bot.Margin = new System.Windows.Forms.Padding(0);
			this.pl_bot.Name = "pl_bot";
			this.pl_bot.Size = new System.Drawing.Size(164, 36);
			this.pl_bot.TabIndex = 2;
			// 
			// gb_locale
			// 
			this.gb_locale.Controls.Add(this.rb_Gen);
			this.gb_locale.Controls.Add(this.rb_ARE);
			this.gb_locale.Controls.Add(this.rb_BIC);
			this.gb_locale.Controls.Add(this.rb_CAM);
			this.gb_locale.Controls.Add(this.rb_DLG);
			this.gb_locale.Controls.Add(this.rb_FAC);
			this.gb_locale.Controls.Add(this.rb_GIC);
			this.gb_locale.Controls.Add(this.rb_GIT);
			this.gb_locale.Controls.Add(this.rb_IFO);
			this.gb_locale.Controls.Add(this.rb_JRL);
			this.gb_locale.Controls.Add(this.rb_ROS);
			this.gb_locale.Controls.Add(this.rb_RST);
			this.gb_locale.Controls.Add(this.rb_ULT);
			this.gb_locale.Controls.Add(this.rb_UPE);
			this.gb_locale.Controls.Add(this.rb_UTC);
			this.gb_locale.Controls.Add(this.rb_UTD);
			this.gb_locale.Controls.Add(this.rb_UTE);
			this.gb_locale.Controls.Add(this.rb_UTI);
			this.gb_locale.Controls.Add(this.rb_UTM);
			this.gb_locale.Controls.Add(this.rb_UTP);
			this.gb_locale.Controls.Add(this.rb_UTR);
			this.gb_locale.Controls.Add(this.rb_UTS);
			this.gb_locale.Controls.Add(this.rb_UTT);
			this.gb_locale.Controls.Add(this.rb_UTW);
			this.gb_locale.Controls.Add(this.rb_WMP);
			this.gb_locale.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gb_locale.Location = new System.Drawing.Point(0, 20);
			this.gb_locale.Margin = new System.Windows.Forms.Padding(0);
			this.gb_locale.Name = "gb_locale";
			this.gb_locale.Size = new System.Drawing.Size(164, 280);
			this.gb_locale.TabIndex = 1;
			this.gb_locale.TabStop = false;
			// 
			// rb_Gen
			// 
			this.rb_Gen.Location = new System.Drawing.Point(20, 15);
			this.rb_Gen.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Gen.Name = "rb_Gen";
			this.rb_Gen.Size = new System.Drawing.Size(45, 20);
			this.rb_Gen.TabIndex = 0;
			this.rb_Gen.TabStop = true;
			this.rb_Gen.Text = "GFF";
			this.rb_Gen.UseVisualStyleBackColor = true;
			this.rb_Gen.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ARE
			// 
			this.rb_ARE.Location = new System.Drawing.Point(20, 35);
			this.rb_ARE.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ARE.Name = "rb_ARE";
			this.rb_ARE.Size = new System.Drawing.Size(45, 20);
			this.rb_ARE.TabIndex = 1;
			this.rb_ARE.TabStop = true;
			this.rb_ARE.Text = "ARE";
			this.rb_ARE.UseVisualStyleBackColor = true;
			this.rb_ARE.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_BIC
			// 
			this.rb_BIC.Location = new System.Drawing.Point(20, 55);
			this.rb_BIC.Margin = new System.Windows.Forms.Padding(0);
			this.rb_BIC.Name = "rb_BIC";
			this.rb_BIC.Size = new System.Drawing.Size(45, 20);
			this.rb_BIC.TabIndex = 2;
			this.rb_BIC.TabStop = true;
			this.rb_BIC.Text = "BIC";
			this.rb_BIC.UseVisualStyleBackColor = true;
			this.rb_BIC.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_CAM
			// 
			this.rb_CAM.Location = new System.Drawing.Point(20, 75);
			this.rb_CAM.Margin = new System.Windows.Forms.Padding(0);
			this.rb_CAM.Name = "rb_CAM";
			this.rb_CAM.Size = new System.Drawing.Size(45, 20);
			this.rb_CAM.TabIndex = 3;
			this.rb_CAM.TabStop = true;
			this.rb_CAM.Text = "CAM";
			this.rb_CAM.UseVisualStyleBackColor = true;
			this.rb_CAM.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_DLG
			// 
			this.rb_DLG.Location = new System.Drawing.Point(20, 95);
			this.rb_DLG.Margin = new System.Windows.Forms.Padding(0);
			this.rb_DLG.Name = "rb_DLG";
			this.rb_DLG.Size = new System.Drawing.Size(45, 20);
			this.rb_DLG.TabIndex = 4;
			this.rb_DLG.TabStop = true;
			this.rb_DLG.Text = "DLG";
			this.rb_DLG.UseVisualStyleBackColor = true;
			this.rb_DLG.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_FAC
			// 
			this.rb_FAC.Location = new System.Drawing.Point(20, 115);
			this.rb_FAC.Margin = new System.Windows.Forms.Padding(0);
			this.rb_FAC.Name = "rb_FAC";
			this.rb_FAC.Size = new System.Drawing.Size(45, 20);
			this.rb_FAC.TabIndex = 5;
			this.rb_FAC.TabStop = true;
			this.rb_FAC.Text = "FAC";
			this.rb_FAC.UseVisualStyleBackColor = true;
			this.rb_FAC.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_GIC
			// 
			this.rb_GIC.Location = new System.Drawing.Point(20, 135);
			this.rb_GIC.Margin = new System.Windows.Forms.Padding(0);
			this.rb_GIC.Name = "rb_GIC";
			this.rb_GIC.Size = new System.Drawing.Size(45, 20);
			this.rb_GIC.TabIndex = 6;
			this.rb_GIC.TabStop = true;
			this.rb_GIC.Text = "GIC";
			this.rb_GIC.UseVisualStyleBackColor = true;
			this.rb_GIC.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_GIT
			// 
			this.rb_GIT.Location = new System.Drawing.Point(20, 155);
			this.rb_GIT.Margin = new System.Windows.Forms.Padding(0);
			this.rb_GIT.Name = "rb_GIT";
			this.rb_GIT.Size = new System.Drawing.Size(45, 20);
			this.rb_GIT.TabIndex = 7;
			this.rb_GIT.TabStop = true;
			this.rb_GIT.Text = "GIT";
			this.rb_GIT.UseVisualStyleBackColor = true;
			this.rb_GIT.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_IFO
			// 
			this.rb_IFO.Location = new System.Drawing.Point(20, 175);
			this.rb_IFO.Margin = new System.Windows.Forms.Padding(0);
			this.rb_IFO.Name = "rb_IFO";
			this.rb_IFO.Size = new System.Drawing.Size(45, 20);
			this.rb_IFO.TabIndex = 8;
			this.rb_IFO.TabStop = true;
			this.rb_IFO.Text = "IFO";
			this.rb_IFO.UseVisualStyleBackColor = true;
			this.rb_IFO.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_JRL
			// 
			this.rb_JRL.Location = new System.Drawing.Point(20, 195);
			this.rb_JRL.Margin = new System.Windows.Forms.Padding(0);
			this.rb_JRL.Name = "rb_JRL";
			this.rb_JRL.Size = new System.Drawing.Size(45, 20);
			this.rb_JRL.TabIndex = 9;
			this.rb_JRL.TabStop = true;
			this.rb_JRL.Text = "JRL";
			this.rb_JRL.UseVisualStyleBackColor = true;
			this.rb_JRL.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ROS
			// 
			this.rb_ROS.Location = new System.Drawing.Point(20, 215);
			this.rb_ROS.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ROS.Name = "rb_ROS";
			this.rb_ROS.Size = new System.Drawing.Size(45, 20);
			this.rb_ROS.TabIndex = 10;
			this.rb_ROS.TabStop = true;
			this.rb_ROS.Text = "ROS";
			this.rb_ROS.UseVisualStyleBackColor = true;
			this.rb_ROS.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_RST
			// 
			this.rb_RST.Location = new System.Drawing.Point(20, 235);
			this.rb_RST.Margin = new System.Windows.Forms.Padding(0);
			this.rb_RST.Name = "rb_RST";
			this.rb_RST.Size = new System.Drawing.Size(45, 20);
			this.rb_RST.TabIndex = 11;
			this.rb_RST.TabStop = true;
			this.rb_RST.Text = "RST";
			this.rb_RST.UseVisualStyleBackColor = true;
			this.rb_RST.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_ULT
			// 
			this.rb_ULT.Location = new System.Drawing.Point(20, 255);
			this.rb_ULT.Margin = new System.Windows.Forms.Padding(0);
			this.rb_ULT.Name = "rb_ULT";
			this.rb_ULT.Size = new System.Drawing.Size(45, 20);
			this.rb_ULT.TabIndex = 12;
			this.rb_ULT.TabStop = true;
			this.rb_ULT.Text = "ULT";
			this.rb_ULT.UseVisualStyleBackColor = true;
			this.rb_ULT.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UPE
			// 
			this.rb_UPE.Location = new System.Drawing.Point(100, 35);
			this.rb_UPE.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UPE.Name = "rb_UPE";
			this.rb_UPE.Size = new System.Drawing.Size(45, 20);
			this.rb_UPE.TabIndex = 13;
			this.rb_UPE.TabStop = true;
			this.rb_UPE.Text = "UPE";
			this.rb_UPE.UseVisualStyleBackColor = true;
			this.rb_UPE.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTC
			// 
			this.rb_UTC.Location = new System.Drawing.Point(100, 55);
			this.rb_UTC.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTC.Name = "rb_UTC";
			this.rb_UTC.Size = new System.Drawing.Size(45, 20);
			this.rb_UTC.TabIndex = 14;
			this.rb_UTC.TabStop = true;
			this.rb_UTC.Text = "UTC";
			this.rb_UTC.UseVisualStyleBackColor = true;
			this.rb_UTC.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTD
			// 
			this.rb_UTD.Location = new System.Drawing.Point(100, 75);
			this.rb_UTD.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTD.Name = "rb_UTD";
			this.rb_UTD.Size = new System.Drawing.Size(45, 20);
			this.rb_UTD.TabIndex = 15;
			this.rb_UTD.TabStop = true;
			this.rb_UTD.Text = "UTD";
			this.rb_UTD.UseVisualStyleBackColor = true;
			this.rb_UTD.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTE
			// 
			this.rb_UTE.Location = new System.Drawing.Point(100, 95);
			this.rb_UTE.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTE.Name = "rb_UTE";
			this.rb_UTE.Size = new System.Drawing.Size(45, 20);
			this.rb_UTE.TabIndex = 16;
			this.rb_UTE.TabStop = true;
			this.rb_UTE.Text = "UTE";
			this.rb_UTE.UseVisualStyleBackColor = true;
			this.rb_UTE.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTI
			// 
			this.rb_UTI.Location = new System.Drawing.Point(100, 115);
			this.rb_UTI.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTI.Name = "rb_UTI";
			this.rb_UTI.Size = new System.Drawing.Size(45, 20);
			this.rb_UTI.TabIndex = 17;
			this.rb_UTI.TabStop = true;
			this.rb_UTI.Text = "UTI";
			this.rb_UTI.UseVisualStyleBackColor = true;
			this.rb_UTI.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTM
			// 
			this.rb_UTM.Location = new System.Drawing.Point(100, 135);
			this.rb_UTM.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTM.Name = "rb_UTM";
			this.rb_UTM.Size = new System.Drawing.Size(45, 20);
			this.rb_UTM.TabIndex = 18;
			this.rb_UTM.TabStop = true;
			this.rb_UTM.Text = "UTM";
			this.rb_UTM.UseVisualStyleBackColor = true;
			this.rb_UTM.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTP
			// 
			this.rb_UTP.Location = new System.Drawing.Point(100, 155);
			this.rb_UTP.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTP.Name = "rb_UTP";
			this.rb_UTP.Size = new System.Drawing.Size(45, 20);
			this.rb_UTP.TabIndex = 19;
			this.rb_UTP.TabStop = true;
			this.rb_UTP.Text = "UTP";
			this.rb_UTP.UseVisualStyleBackColor = true;
			this.rb_UTP.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTR
			// 
			this.rb_UTR.Location = new System.Drawing.Point(100, 175);
			this.rb_UTR.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTR.Name = "rb_UTR";
			this.rb_UTR.Size = new System.Drawing.Size(45, 20);
			this.rb_UTR.TabIndex = 20;
			this.rb_UTR.TabStop = true;
			this.rb_UTR.Text = "UTR";
			this.rb_UTR.UseVisualStyleBackColor = true;
			this.rb_UTR.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTS
			// 
			this.rb_UTS.Location = new System.Drawing.Point(100, 195);
			this.rb_UTS.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTS.Name = "rb_UTS";
			this.rb_UTS.Size = new System.Drawing.Size(45, 20);
			this.rb_UTS.TabIndex = 21;
			this.rb_UTS.TabStop = true;
			this.rb_UTS.Text = "UTS";
			this.rb_UTS.UseVisualStyleBackColor = true;
			this.rb_UTS.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTT
			// 
			this.rb_UTT.Location = new System.Drawing.Point(100, 215);
			this.rb_UTT.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTT.Name = "rb_UTT";
			this.rb_UTT.Size = new System.Drawing.Size(45, 20);
			this.rb_UTT.TabIndex = 22;
			this.rb_UTT.TabStop = true;
			this.rb_UTT.Text = "UTT";
			this.rb_UTT.UseVisualStyleBackColor = true;
			this.rb_UTT.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_UTW
			// 
			this.rb_UTW.Location = new System.Drawing.Point(100, 235);
			this.rb_UTW.Margin = new System.Windows.Forms.Padding(0);
			this.rb_UTW.Name = "rb_UTW";
			this.rb_UTW.Size = new System.Drawing.Size(45, 20);
			this.rb_UTW.TabIndex = 23;
			this.rb_UTW.TabStop = true;
			this.rb_UTW.Text = "UTW";
			this.rb_UTW.UseVisualStyleBackColor = true;
			this.rb_UTW.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// rb_WMP
			// 
			this.rb_WMP.Location = new System.Drawing.Point(100, 255);
			this.rb_WMP.Margin = new System.Windows.Forms.Padding(0);
			this.rb_WMP.Name = "rb_WMP";
			this.rb_WMP.Size = new System.Drawing.Size(45, 20);
			this.rb_WMP.TabIndex = 24;
			this.rb_WMP.TabStop = true;
			this.rb_WMP.Text = "WMP";
			this.rb_WMP.UseVisualStyleBackColor = true;
			this.rb_WMP.CheckedChanged += new System.EventHandler(this.checkchanged);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 3750;
			this.toolTip1.InitialDelay = 150;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.ShowAlways = true;
			this.toolTip1.StripAmpersands = true;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// TypeDialog
			// 
			this.AcceptButton = this.bt_Accept;
			this.CancelButton = this.bt_Cancel;
			this.ClientSize = new System.Drawing.Size(164, 336);
			this.Controls.Add(this.gb_locale);
			this.Controls.Add(this.pl_bot);
			this.Controls.Add(this.la_head);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TypeDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "GFF Type";
			this.pl_bot.ResumeLayout(false);
			this.gb_locale.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
