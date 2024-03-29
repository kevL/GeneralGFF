﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//using NWN2Toolset.NWN2.Data.Blueprints;

//using OEIShared.IO.GFF;


namespace generalgff
{
	/// <summary>
	/// 
	/// </summary>
	sealed partial class GeneralGFF
		:
			Form
	{
		#region Fields (static)
		const string TITLE = "GeneralGFF";

		/// <summary>
		/// Total width in characters allowed for the label in the treenode text.
		/// </summary>
		const int LENGTH_LABEL = 17;
		/// <summary>
		/// Total width in characters allowed for the type-descriptor in the
		/// treenode text.
		/// </summary>
		const int LENGTH_TYPE  = 17;

		internal const int DIRTY_non   = 0x0; // these constants are used to track the
				 const int DIRTY_TEXTS = 0x1; // state of the Apply and Revert buttons ->
				 const int DIRTY_CZECH = 0x2;
		#endregion Fields (static)


		#region Fields
		internal TreeList _tree;

		string _prevalText_rt = String.Empty;	// cached text used by Revert
		string _prevalText_tb = String.Empty;	// cached text used by Revert
		bool   _prevalCusto;					// cached check-state used by Revert

		string _edittext = String.Empty;	// stored val that's reverted to if user enters an invalid character
		int    _posCaret = 0;				// tracks the position of the caret in case text gets reset to '_edittext'

		/// <summary>
		/// A previous file's GffType. Used to deter if Extension should remain
		/// enabled when a different file loads.
		/// </summary>
		GffType _lastGffType = GffType.generic;
		#endregion Fields


		#region Properties
		GffData _data;
		/// <summary>
		/// The currently loaded GFF data.
		/// </summary>
		internal GffData GffData
		{
			get { return _data; }
			set
			{
				bool @checked;

				if ((_data = value) != null)
				{
					Text = TITLE + " - " + _data.Pfe;

					if (_data.Changed)
						Text += " *";

					@checked = _data.Type != GffType.generic	// TODO: these statements run way more than
							&& _data.Type == _lastGffType;		// they need to. Do it in file loading and the
					_lastGffType = _data.Type;					// GffType-editor-dialog instead of here.
				}
				else
				{
					Text = TITLE;
					@checked = false;
				}

				@checked = @checked
						&& Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[MenuCreator.MI_EXTS_EXT].Checked;
				SetExtensionEnabled(@checked);
			}
		}

		int _dirtystate;
		/// <summary>
		/// Enables or disables the Apply and Revert buttons in the editor-panel.
		/// </summary>
		internal int DirtyState
		{
			get { return _dirtystate; }
			set
			{
				btn_Revert.Enabled =
				btn_Apply .Enabled = (_dirtystate = value) != DIRTY_non;
			}
		}

		/// <summary>
		/// A cloned treenode along with its Field, plus any subnodes and their
		/// Fields.
		/// </summary>
		Sortable Copied
		{ get; set; }

		/// <summary>
		/// Pointer to the Search dialog.
		/// </summary>
		internal SearchDialog Search
		{ private get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates the GeneralGFF f.
		/// </summary>
		/// <param name="filearg"></param>
		internal GeneralGFF(string filearg)
		{
			logfile.CreateLog(); // works in debug-build only.
			InitializeComponent();

			_tree = new TreeList(this);
			sc_body.Panel1.Controls.Add(_tree);
			_tree.Select();

			SubscribeOperations();


			sc_body.Panel1MinSize = sc_body.Panel1MinSize + SystemInformation.VerticalScrollBarWidth;
			MinimumSize  = new Size(sc_body.Panel1MinSize + sc_body.SplitterWidth
														  + Width - ClientSize.Width, // <- border
									150);
			sc_body.Panel1.ClientSize = new Size(sc_body.Panel1MinSize,
												 sc_body.Panel1.Height);

			sc_body.MouseDown += splitCont_MouseDown;
			sc_body.MouseUp   += splitCont_MouseUp;
			sc_body.MouseMove += splitCont_MouseMove;


			if (File.Exists(filearg))
				GffLoader.LoadGFFfile(this, filearg);
		}

		/// <summary>
		/// Creates the mainmenu and subscribes handlers to its operations.
		/// </summary>
		void SubscribeOperations()
		{
			Menu = MenuCreator.Create();

			Menu.MenuItems[MenuCreator.MI_FILE].Popup += filepop;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_CRAT].Click += fileclick_Create;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_OPEN].Click += fileclick_Open;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_RELD].Click += fileclick_Reload;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_SAVE].Click += fileclick_Save;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_SAVA].Click += fileclick_SaveAs;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_EXPT].Click += fileclick_Export;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_QUIT].Click += fileclick_Quit;

			Menu.MenuItems[MenuCreator.MI_EDIT].Popup += editpop;
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_SER].Click += editclick_Search;
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_CUT].Click += editclick_Cut;
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_COP].Click += editclick_Copy;
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_PAS].Click += editclick_Paste;
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_DEL].Click += editclick_Delete;

			Menu.MenuItems[MenuCreator.MI_VIEW].Popup += viewpop;
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_EXPD].Click += viewclick_ExpandSelected;
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_COLP].Click += viewclick_CollapseSelected;
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_SORT].Click += viewclick_Sort;

			Menu.MenuItems[MenuCreator.MI_EXTS].Popup += extensionpop;
			Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[MenuCreator.MI_EXTS_EXT].Click += extensionclick_Enable;
//			Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[1].Click += extensionclick_Visualizer;

			Menu.MenuItems[MenuCreator.MI_HELP].MenuItems[MenuCreator.MI_HELP_ABT].Click += helpclick_About;
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Constructs a string of text for display on a treenode.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="locale"></param>
		/// <returns></returns>
		internal static string ConstructNodetext(GffData.Field field, GffData.Locale locale = null)
		{
			string label = field.label; // 16 char limit (GFF specification)
			while (label.Length != LENGTH_LABEL)
				label += " ";

			bool token = locale != null
					  && locale.langid == Language.GffToken;

			string label2 = " [" + GetTypeString(field.type, token) + "]";
			while (label2.Length != LENGTH_TYPE)
				label2 += " ";

			label += label2;

			switch (field.type)
			{
				default:
					label += "= " + GetValueString(field);
					break;

				case FieldType.locale:
					label += "= " + locale.local;
					break;

				case FieldType.List:
					break;
			}

			if (label.Length > 68)
				label = label.Substring(0,66) + "...";

			return label;
		}

		/// <summary>
		/// Constructs a string of text for display on a treenode. The text
		/// prints the equipment-slot per the creature-extension.
		/// TODO: Toggle these text-fields when the UTC Extension is toggled
		/// on/off.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		internal static string ConstructNodetextEquipped(GffData.Field field)
		{
			string label = field.label; // 16 char limit (GFF specification)
			while (label.Length != LENGTH_LABEL)
				label += " ";

			string label2 = " [" + GeneralGFF.GetTypeString(field.type) + "]";
			while (label2.Length != LENGTH_TYPE)
				label2 += " ";

			return label + label2
				 + "= " + GetValueString(field)
				 + " "  + GetEquippedItemSlot(field.Struct.typeid);
		}

		/// <summary>
		/// Converts a <c><see cref="FieldType"/></c> into a readable string.
		/// @note helper for ConstructNodetext()
		/// </summary>
		/// <param name="type"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		static string GetTypeString(FieldType type, bool token = false)
		{
			switch (type)
			{
				case FieldType.BYTE:          return "BYTE";
				case FieldType.CHAR:          return "CHAR";
				case FieldType.WORD:          return "WORD";
				case FieldType.SHORT:         return "SHORT";
				case FieldType.DWORD:         return "DWORD";
				case FieldType.INT:           return "INT";
				case FieldType.DWORD64:       return "DWORD64";
				case FieldType.INT64:         return "INT64";
				case FieldType.FLOAT:         return "FLOAT";
				case FieldType.DOUBLE:        return "DOUBLE";
				case FieldType.CResRef:       return "CResRef";
				case FieldType.CExoString:    return "CExoString";
				case FieldType.CExoLocString: return "CExoLocString";
				case FieldType.VOID:          return "VOID";
				case FieldType.List:          return "List";
				case FieldType.Struct:        return "Struct";

				case FieldType.locale:
					if (token) return "token";
					return "locale";
			}
			return "ErROr: field type unknown";
		}

		/// <summary>
		/// Gets the value of a
		/// <c><see cref="GffData.Field">GffData.Field</see></c> by its type.
		/// @note helper for ConstructNodetext()
		/// </summary>
		/// <param name="field"></param>
		/// <returns>the value as a string</returns>
		static string GetValueString(GffData.Field field)
		{
			switch (field.type)
			{
				case FieldType.BYTE:
					return field.BYTE.ToString();

				case FieldType.CHAR:
					return field.CHAR.ToString();

				case FieldType.WORD:
					return field.WORD.ToString();

				case FieldType.SHORT:
					return field.SHORT.ToString();

				case FieldType.DWORD:
					return field.DWORD.ToString();

				case FieldType.INT:
					return field.INT.ToString();

				case FieldType.DWORD64:
					return field.DWORD64.ToString();

				case FieldType.INT64:
					return field.INT64.ToString();

				case FieldType.FLOAT:
				{
					string f = field.FLOAT.ToString();
					if (!f.Contains(".")) f += ".0";
					return f;
				}

				case FieldType.DOUBLE:
				{
					string d = field.DOUBLE.ToString();
					if (!d.Contains(".")) d += ".0";
					return d;
				}

				case FieldType.CResRef:
					return field.CResRef;

				case FieldType.CExoString:
					return field.CExoString;

				case FieldType.CExoLocString:
				{
					uint strref = field.CExoLocStrref;
					if (strref != UInt32.MaxValue)
						return strref.ToString();

					return "-1";
				}

				case FieldType.VOID:
				{
					int length = field.VOID.Length;
					string val = length + " byte";
					if (length != 1) val += "s";
					return val;
				}

				case FieldType.List:
					return String.Empty;

				case FieldType.Struct:
					return "[" + field.Struct.typeid + "]";
			}
			return "ErROr: field type unknown";
		}

		/// <summary>
		/// Gets a string value for an Equip_ItemList's Struct's typeid.
		/// </summary>
		/// <param name="typeid"></param>
		/// <returns></returns>
		static string GetEquippedItemSlot(uint typeid)
		{
			switch (typeid)
			{
				case EquippedItemDialog.Slot_HEAD:    return "HEAD";
				case EquippedItemDialog.Slot_CHEST:   return "CHEST";
				case EquippedItemDialog.Slot_FEET:    return "FEET";
				case EquippedItemDialog.Slot_WRISTS:  return "WRISTS";
				case EquippedItemDialog.Slot_RHAND:   return "RIGHTHAND";
				case EquippedItemDialog.Slot_LHAND:   return "LEFTHAND";
				case EquippedItemDialog.Slot_BACK:    return "BACK";
				case EquippedItemDialog.Slot_LRING:   return "LEFTRING";
				case EquippedItemDialog.Slot_RRING:   return "RIGHTRING";
				case EquippedItemDialog.Slot_NECK:    return "NECK";
				case EquippedItemDialog.Slot_WAIST:   return "WAIST";
				case EquippedItemDialog.Slot_ARROWS:  return "ARROWS";
				case EquippedItemDialog.Slot_BULLETS: return "BULLETS";
				case EquippedItemDialog.Slot_BOLTS:   return "BOLTS";
				case EquippedItemDialog.Slot_WEAP1:   return "WEAPON1";
				case EquippedItemDialog.Slot_WEAP2:   return "WEAPON2";
				case EquippedItemDialog.Slot_WEAP3:   return "WEAPON3";
				case EquippedItemDialog.Slot_SKIN:    return "SKIN";
			}
			return "ErROr: typeid unknown";
		}
		#endregion Methods (static)


		#region Handlers (override)
		/// <summary>
		/// Cancels close if appropriate.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (   e.CloseReason != CloseReason.WindowsShutDown
				&& e.CloseReason != CloseReason.TaskManagerClosing)
			{
				e.Cancel = !CheckCloseData(Globals.Quit);
			}
		}

		/// <summary>
		/// Tries to keep the split-container panels somewhat predictable when
		/// resizing the form.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			if (WindowState != FormWindowState.Minimized)
			{
				if (sc_body.Panel2.Width == 0)
					sc_body.FixedPanel = FixedPanel.Panel2;
				else
				{
					sc_body.FixedPanel = FixedPanel.Panel1;
					if (sc_body.SplitterDistance > ClientSize.Width - sc_body.SplitterWidth)
					{
						int dist = ClientSize.Width - sc_body.SplitterWidth - sc_body.Panel2.Width;
						sc_body.SplitterDistance = Math.Max(sc_body.Panel1MinSize, dist);
					}
				}
			}
			base.OnResize(e);
		}

		/// <summary>
		/// Routes edit-key events to the active texbox if appropriate.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns>true if the input gets handled</returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.X:
				case Keys.Control | Keys.C:
				case Keys.Control | Keys.V:
				case Keys.Delete:
					if (tb_Val.ContainsFocus || rt_Val.ContainsFocus)
					{
						Edit(keyData);
						return true;
					}
					break;

				case Keys.F3:
					if (Search == null)
						editclick_Search(null, EventArgs.Empty);
					else
						Search.click_Down(null, EventArgs.Empty);

					return true;

				case Keys.Shift | Keys.F3:
					if (Search == null)
						editclick_Search(null, EventArgs.Empty);
					else
						Search.click_Up(null, EventArgs.Empty);

					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Checks if the file on disk has changed or been deleted when this
		/// form takes focus.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			if (GffData != null && GffData.Pfe != Globals.TopLevelStruct)
			{
				if (!File.Exists(GffData.Pfe))
				{
					if (!FileWatchDialog.Bypass)
					{
						FileWatchDialog.Bypass = true;
						using (var fwd = new FileWatchDialog(this, FileWatchDialog.FILE_DEL))
							fwd.ShowDialog(this);
					}
				}
				else
				{
					DateTime dt = File.GetLastWriteTime(GffData.Pfe);
					if (dt != GffData.Latest)
					{
						GffData.Latest = dt;
						using (var fwd = new FileWatchDialog(this, FileWatchDialog.FILE_WSC))
							fwd.ShowDialog(this);
					}
				}
			}
		}
		#endregion Handlers (override)


		#region Handlers (splitter)
		// https://stackoverflow.com/questions/6521731/refresh-the-panels-of-a-splitcontainer-as-the-splitter-moves#6522741

		/// <summary>
		/// Assign this to the SplitContainer's MouseDown event. This disables
		/// the normal move behavior.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void splitCont_MouseDown(object sender, MouseEventArgs e)
		{
			(sender as SplitContainer).IsSplitterFixed = true;
		}

		/// <summary>
		/// Assign this to the SplitContainer's MouseUp event. This allows the
		/// splitter to be moved normally again.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void splitCont_MouseUp(object sender, MouseEventArgs e)
		{
			(sender as SplitContainer).IsSplitterFixed = false;
			Cursor = Cursors.Default; // kL_add.
		}

		/// <summary>
		/// Assign this to the SplitContainer's MouseMove event. Check to make
		/// sure the splitter won't be updated by the normal move behavior also.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void splitCont_MouseMove(object sender, MouseEventArgs e)
		{
			var sc = sender as SplitContainer;
			if (sc.IsSplitterFixed)
			{
				if (e.Button.Equals(MouseButtons.Left))
				{
					Cursor = Cursors.VSplit; // kL_add.

//					if (sc.Orientation.Equals(Orientation.Vertical))
//					{
					if (e.X > 0 && e.X < sc.Width)
					{
						sc.SplitterDistance = e.X;
						sc.Refresh();
					}
//					}
//					else if (e.Y > 0 && e.Y < sc.Height)
//					{
//						sc.SplitterDistance = e.Y;
//						sc.Refresh();
//					}
				}
				else
					sc.IsSplitterFixed = false;
			}
		}
		#endregion Handlers (splitter)


		#region Handlers (menu)
		/// <summary>
		/// Enables/disables File-menu's items appropriately.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void filepop(object sender, EventArgs e)
		{
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_RELD].Enabled = GffData != null
																						   && File.Exists(GffData.Pfe);
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_SAVE].Enabled = _tree.Nodes.Count != 0
																						   && GffData.Changed
																						   && GffData.Pfe != Globals.TopLevelStruct;
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_SAVA].Enabled =
			Menu.MenuItems[MenuCreator.MI_FILE].MenuItems[MenuCreator.MI_FILE_EXPT].Enabled = _tree.Nodes.Count != 0;
		}

		/// <summary>
		/// Creates a blank GFF-file.
		/// cf. TreeList.contextclick_AddTopLevelStruct()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Create(object sender, EventArgs e)
		{
			if (CheckCloseData(Globals.Close))
			{
				_tree.Nodes.Clear();

				GffData = new GffData(); // init GffData! ->
				GffData.TypeVer = "GFF V3.2";
				GffData.Type = GffType.generic;

				GffData = GffData; // update titlebar text

				var field = new GffData.Field();
				field.type = FieldType.Struct;
				field.label = Globals.TopLevelStruct;

				field.Struct = new Struct();
				field.Struct.typeid = UInt32.MaxValue;

				_tree.Nodes.Add(field.label);

				_tree.SelectedNode = _tree.Nodes[0];
			}
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Open(object sender, EventArgs e)
		{
			if (CheckCloseData(Globals.Close))
			{
				using (var ofd = new OpenFileDialog())
				{
//					ofd.InitialDirectory = ;

					ofd.Title  = "Select a GFF file";
					ofd.Filter = GffData.FileDialogFilter;

					if (ofd.ShowDialog(this) == DialogResult.OK)
						GffLoader.LoadGFFfile(this, ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Saves the currently loaded file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void fileclick_Save(object sender, EventArgs e)
		{
			if (_tree.Nodes.Count != 0 && GffData.Changed
				&& GffData.Pfe != Globals.TopLevelStruct
				&& GffWriter.WriteGFFfile(GffData.Pfe, _tree, GffData.TypeVer))
			{
				FileWatchDialog.Bypass = false;
				GffData.Latest = File.GetLastWriteTime(GffData.Pfe);

				GffData.Changed = false;
				GffData = GffData; // update titlebar text
			}
		}

		/// <summary>
		/// Saves the currently loaded file as a user-labeled file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_SaveAs(object sender, EventArgs e)
		{
			if (_tree.Nodes.Count != 0)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title  = "Save as GFF file";
					sfd.Filter = GffData.FileDialogFilter;

					sfd.DefaultExt = GffData.GetGffString(GffData.Type);

					if (GffData.Pfe != Globals.TopLevelStruct)
					{
						sfd.InitialDirectory = Path.GetDirectoryName(GffData.Pfe);
						sfd.FileName         = Path.GetFileName(GffData.Pfe);
					}

					if (sfd.ShowDialog(this) == DialogResult.OK
						&& GffWriter.WriteGFFfile(sfd.FileName, _tree, GffData.TypeVer))
					{
						string label = Path.GetFileNameWithoutExtension(sfd.FileName).ToUpper();
						_tree.Nodes[0].Text = label; // update TLS-label

						GffData.Pfe = sfd.FileName;

						FileWatchDialog.Bypass = false;
						GffData.Latest = File.GetLastWriteTime(GffData.Pfe);

						GffData.Changed = false;
						GffData = GffData; // update titlebar text
					}
				}
			}
		}

		/// <summary>
		/// Reloads the currently loaded file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void fileclick_Reload(object sender, EventArgs e)
		{
			if (GffData != null && File.Exists(GffData.Pfe)
				&& CheckCloseData(Globals.Reload))
			{
				GffLoader.LoadGFFfile(this, GffData.Pfe);
			}
		}

		/// <summary>
		/// Saves the current data to a user-labeled file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Export(object sender, EventArgs e)
		{
			if (_tree.Nodes.Count != 0)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title  = "Export to GFF file";
					sfd.Filter = GffData.FileDialogFilter;

//					sfd.DefaultExt = GffData.GetGffString(GffData.Type);

					if (GffData.Pfe != Globals.TopLevelStruct)
					{
						sfd.InitialDirectory = Path.GetDirectoryName(GffData.Pfe);
						sfd.FileName         = Path.GetFileName(GffData.Pfe);
					}

					if (sfd.ShowDialog(this) == DialogResult.OK
						&& GffWriter.WriteGFFfile(sfd.FileName, _tree, GffData.TypeVer)
						&& sfd.FileName == GffData.Pfe)
					{
						// NOTE: This happens only if user chooses to export/
						// write the current data to the original file.

						FileWatchDialog.Bypass = false;
						GffData.Latest = File.GetLastWriteTime(GffData.Pfe);

						GffData.Changed = false;
						GffData = GffData; // update titlebar text
					}
				}
			}
		}

		/// <summary>
		/// Exits the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Quit(object sender, EventArgs e)
		{
			Close();
		}


		/// <summary>
		/// Enables/disables Edit-menu's items appropriately.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editpop(object sender, EventArgs e)
		{
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_CUT].Enabled =
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_COP].Enabled = _tree.SelectedNode != null
																						  && _tree.SelectedNode != _tree.Nodes[0];
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_PAS].Enabled = EnablePaste();
			Menu.MenuItems[MenuCreator.MI_EDIT].MenuItems[MenuCreator.MI_EDIT_DEL].Enabled = _tree.SelectedNode != null;
		}

		/// <summary>
		/// Opens the Search dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Search(object sender, EventArgs e)
		{
			Search = new SearchDialog(this);
			Search.Show(this);
		}

		/// <summary>
		/// Cuts the currently selected treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Cut(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null && _tree.SelectedNode != _tree.Nodes[0])
			{
				editclick_Copy(null, EventArgs.Empty);
				_tree.contextclick_Delete(null, EventArgs.Empty);
			}
		}


		GffData.Locale _refLocale;

		/// <summary>
		/// Copies the currently selected treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Copy(object sender, EventArgs e)
		{
			if (sender == null // called by editclick_Cut()
				|| (_tree.SelectedNode != null && _tree.SelectedNode != _tree.Nodes[0]))
			{
				Copied = Sortable.Duplicate((Sortable)_tree.SelectedNode);

				var field = (GffData.Field)_tree.SelectedNode.Tag;
				if (field.type == FieldType.locale) // gotta cache the Locale if relevant
				{
					_refLocale = ((GffData.Field)_tree.SelectedNode.Parent.Tag).Locales[(int)field.localeid];
				}
				else
					_refLocale = null;
			}
		}

		/// <summary>
		/// Pastes the currently copied treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Paste(object sender, EventArgs e)
		{
			if (EnablePaste() && !LocaleExists())
			{
				var node = Sortable.Duplicate(Copied);
				GffData.Field field;

				if (_tree.SelectedNode.Tag == null // is TopLevelStruct
					|| (field = (GffData.Field)_tree.SelectedNode.Tag).type == FieldType.Struct)
				{
					string label = _tree.GetUniqueLabel(node._label);
					if (label != node._label)
					{
						field = (GffData.Field)node.Tag;
						field.label = label;

						node._label = label;
						node.Text = GeneralGFF.ConstructNodetext(field);

						using (var f = new InfoDialog(Globals.Warning, "Duplicate labels detected: Label changed."))
							f.ShowDialog(this);
					}
				}
				else
				{
					switch (field.type)
					{
						case FieldType.List:
							field = (GffData.Field)node.Tag;
							field.label = _tree.SelectedNode.Nodes.Count.ToString();

							node._label = field.label;
							node.Text = GeneralGFF.ConstructNodetext(field);
							break;

						case FieldType.CExoLocString:
							LocaleDialog.SetLocaleFlag(ref field.localeflags,
													   _refLocale.langid,
													   _refLocale.F);

							if (field.Locales == null)
								field.Locales = new List<GffData.Locale>();

							((GffData.Field)node.Tag).localeid = (uint)field.Locales.Count;

							field.Locales.Add(GffData.Locale.Duplicate(_refLocale));
							break;
					}
				}

				_tree.SelectedNode.Nodes.Add(node);
				_tree.SelectedNode.Expand();
				_tree.SelectedNode = node;

				GffData.Changed = true;
				GffData = GffData;
			}
		}

		/// <summary>
		/// Checks if a paste-operation can proceed.
		/// </summary>
		/// <returns></returns>
		bool EnablePaste()
		{
			if (_tree.SelectedNode != null && Copied != null)
			{
				if (_tree.SelectedNode.Tag == null) // is TopLevelStruct
					return true;

				switch (((GffData.Field)_tree.SelectedNode.Tag).type)
				{
					case FieldType.Struct:
						return true;

					case FieldType.List:
						return ((GffData.Field)Copied.Tag).type == FieldType.Struct;

					case FieldType.CExoLocString:
						return ((GffData.Field)Copied.Tag).type == FieldType.locale;
				}
			}
			return false;
		}

		/// <summary>
		/// Disallows pasting a locale if it already exists in the CExoLocString
		/// that user is trying to paste into.
		/// </summary>
		/// <returns></returns>
		bool LocaleExists()
		{
			if (_refLocale != null)
			{
				uint localeflags = ((GffData.Field)_tree.SelectedNode.Tag).localeflags;
				if ((localeflags & LocaleDialog.GetLocaleFlag(_refLocale)) != 0)
				{
					string info = "The currently copied Locale already exists in the branch."
								+ Environment.NewLine + Environment.NewLine
								+ GffData.Locale.GetLanguageString(_refLocale.langid, _refLocale.F);

					using (var f = new InfoDialog(Globals.Error, info))
						f.ShowDialog(this);

					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Deletes a treenode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Delete(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
				_tree.contextclick_Delete(null, EventArgs.Empty);
		}


		/// <summary>
		/// Enables/disables View-menu's items appropriately.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewpop(object sender, EventArgs e)
		{
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_EXPD].Enabled =
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_COLP].Enabled = _tree.SelectedNode != null
																						   && _tree.SelectedNode.Nodes.Count != 0;
			Menu.MenuItems[MenuCreator.MI_VIEW].MenuItems[MenuCreator.MI_VIEW_SORT].Enabled = _tree.Nodes.Count != 0;
		}

		/// <summary>
		/// Expands the currently selected treenode and all childs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewclick_ExpandSelected(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
			{
				_tree.SelectedNode.Expand();
				ExpandChildren(_tree.SelectedNode);

				if (!_tree.SelectedNode.IsVisible)
					_tree.TopNode = _tree.SelectedNode;
			}
		}

		/// <summary>
		/// - helper for editclick_ExpandSelected()
		/// </summary>
		/// <param name="node"></param>
		void ExpandChildren(TreeNode node)
		{
			foreach (TreeNode child in node.Nodes)
			{
				child.Expand();
				ExpandChildren(child);
			}
		}

		/// <summary>
		/// Collapses the currently selected treenode and all childs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewclick_CollapseSelected(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
			{
				_tree.SelectedNode.Collapse();
				CollapseChildren(_tree.SelectedNode);
			}
		}

		/// <summary>
		/// - helper for editclick_CollapseSelected()
		/// </summary>
		/// <param name="node"></param>
		void CollapseChildren(TreeNode node)
		{
			foreach (TreeNode child in node.Nodes)
			{
				child.Collapse();
				CollapseChildren(child);
			}
		}


		/// <summary>
		/// Sorts the tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewclick_Sort(object sender, EventArgs e)
		{
			if (_tree.Nodes.Count != 0)
			{
				_tree.BeginUpdate();

				var node = _tree.SelectedNode;
				_tree.Sort();

				if (!(_tree.SelectedNode = node).IsVisible)
					_tree.SelectedNode.EnsureVisible(); // _tree.TopNode = _tree.SelectedNode;

				_tree.EndUpdate();
			}
		}


		/// <summary>
		/// Enables/disables Extension-menu's items appropriately.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void extensionpop(object sender, EventArgs e)
		{
			string type;
			if (GffData != null) type = " " + GffData.GetGffString(GffData.Type);
			else                 type = String.Empty;

			Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[MenuCreator.MI_EXTS_EXT].Text = "&Enable" + type;
			Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[MenuCreator.MI_EXTS_EXT].Enabled = GffData != null
																						  && GffData.Type == GffType.UTC;
		}


		internal bool _extEnabled;

		/// <summary>
		/// Enables creature-extension.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void extensionclick_Enable(object sender, EventArgs e)
		{
			if (GffData != null)
			{
				switch (GffData.Type)
				{
					case GffType.UTC:
					{
						MenuItem it = Menu.MenuItems[MenuCreator.MI_EXTS].MenuItems[MenuCreator.MI_EXTS_EXT];
						SetExtensionEnabled(it.Checked = !it.Checked);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Sets the extension enabled.
		/// @note This is a workaround for the fact that MenuItem doesn't have
		/// an OnCheckChanged event.
		/// </summary>
		/// <param name="enabled"></param>
		void SetExtensionEnabled(bool enabled)
		{
			if (_extEnabled = enabled)
			{
				_tree.BackColor = Color.BurlyWood;
			}
			else
				_tree.BackColor = Color.PaleTurquoise;
		}

/*		void extensionclick_Visualizer(object sender, EventArgs e)
		{
			var f = new CreatureVisualizerF();

			if (GffData.Pfe != Globals.TopLevelStruct)
			{
				var file = new GFFFile(GffData.Pfe);
				var creature = new NWN2CreatureBlueprint(file.TopLevelStruct);
//				var @struct = new GFFStruct();
//				var creature = new NWN2CreatureBlueprint(@struct);
				f.setCreature(creature);
			}

			f.Show(this);
		} */


		/// <summary>
		/// Shows the About box w/ version and build config.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void helpclick_About(object sender, EventArgs e)
		{
			string text = "GeneralGFF Editor"
						+ Environment.NewLine
						+ "- for Neverwinter Nights 2"
						+ Environment.NewLine + Environment.NewLine;

			var an = Assembly.GetExecutingAssembly().GetName();
			text += an.Version.Major + "."
				  + an.Version.Minor + "."
				  + an.Version.Build + "."
				  + an.Version.Revision;
#if DEBUG
			text += " debug";
#else
			text += " release";
#endif

			DateTime dt = Assembly.GetExecutingAssembly().GetLinkerTime();
			text += Environment.NewLine
				  + String.Format(CultureInfo.CurrentCulture,
								  "{0:yyyy MMM d} {0:HH}:{0:mm}:{0:ss} UTC", // {0:zzz}
								  dt);

			using (var f = new InfoDialog(Globals.About, text))
				f.ShowDialog(this);
		}
		#endregion Handlers (menu)


		#region Handlers (panel2)
		/// <summary>
		/// Tracks the position of the caret in the active textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void mousedown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if ((Control)sender == tb_Val)
					_posCaret = tb_Val.SelectionStart;
				else
					_posCaret = rt_Val.SelectionStart;
			}
		}

		/// <summary>
		/// Tracks the position of the caret in the active textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keyup(object sender, KeyEventArgs e)
		{
			if ((Control)sender == tb_Val)
				_posCaret = tb_Val.SelectionStart;
			else
				_posCaret = rt_Val.SelectionStart;
		}


		/// <summary>
		/// Handles keydowns in the singleline textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Single(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
					e.SuppressKeyPress = true;
					BeginInvoke((MethodInvoker)delegate	// don't beep if/when click_Apply() errors ->
					{ btn_Apply.PerformClick(); });		// it masks the Exclamation sound
					break;

				case Keys.Escape:
					e.SuppressKeyPress = true;
					btn_Revert.PerformClick();
					break;

				case Keys.Control | Keys.A:
					e.SuppressKeyPress = true;
					tb_Val.SelectAll();
					break;
			}
		}

		/// <summary>
		/// Handles keydowns in the multiline textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Multi(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					e.SuppressKeyPress = true;
					btn_Revert.PerformClick();
					break;

				case Keys.Control | Keys.A:
					e.SuppressKeyPress = true;
					rt_Val.SelectAll();
					break;
			}
		}


		/// <summary>
		/// Prevents non-ASCII characters in CResRefs and/or the
		/// TopLevelStruct's type+version info.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Single(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
			{
				object tag = _tree.SelectedNode.Tag;
				if (tag == null // is TopLevelStruct
					|| ((GffData.Field)tag).type == FieldType.CResRef)
				{
					if (!isPrintableAscii(tb_Val.Text))
					{
						ResetEditor(tb_Val);
					}
					else
						_edittext = tb_Val.Text;
				}

				if (tb_Val.Text != _prevalText_tb)
				{
					DirtyState |= DIRTY_TEXTS;
				}
				else
					DirtyState &= ~DIRTY_TEXTS;
			}
		}

		/// <summary>
		/// Prevents non-hexadecimal characters in VOID.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Multi(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
			{
				var field = ((GffData.Field)_tree.SelectedNode.Tag);
				if (field != null && field.type == FieldType.VOID)
				{
					if (!isHexadecimal(rt_Val.Text))
					{
						ResetEditor(rt_Val);
					}
					else
						_edittext = rt_Val.Text;
				}

				if (rt_Val.Text != _prevalText_rt)
				{
					DirtyState |= DIRTY_TEXTS;
				}
				else
					DirtyState &= ~DIRTY_TEXTS;
			}
		}


		/// <summary>
		/// Applies changed data to the currently selected field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Apply(object sender, EventArgs e)
		{
			TreeNode node = _tree.SelectedNode;
			if (node != null) // safety.
			{
				Control editor = tb_Val;

				string val = null; // the string to test

				GffData.Field  field  = null;
				GffData.Locale locale = null;

				bool valid = false;

				if (node.Tag == null) // is TopLevelStruct
				{
					int length = tb_Val.Text.Length;

					val = tb_Val.Text.Trim().ToUpper(CultureInfo.InvariantCulture);

					bool bork = false;
					if (val.Length == Globals.Length_VER)
					{
						for (int i = 0; i != val.Length && !bork; ++i)
						{
							switch (i)
							{
								case 0:
								case 1:
								case 2: bork = (val[i]  < 65 || val[i] > 90); break; // alpha
								case 3: bork = (val[i] != 32);                break; // space
								case 4: bork = (val[i] != 86);                break; // V
								case 5:
								case 7: bork = (val[i]  < 48 || val[i] > 57); break; // numeric
								case 6: bork = (val[i] != 46);                break; // dot
							}
						}
					}
					else
						bork = true;

					if (!bork)
					{
						valid = true;

						GffData.TypeVer = val;
						GffData.Type = GffData.GetGffType(val.Substring(0,3));

						tb_Val.Text = (_prevalText_tb = val);
						RepositionCaret(tb_Val);
					}
				}
				else
				{
					field = (GffData.Field)node.Tag;
					switch (field.type)
					{
						case FieldType.BYTE:
						{
							byte result;
							if (valid = Byte.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.BYTE = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.CHAR:
						{
							sbyte result;
							if (valid = SByte.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.CHAR = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.WORD:
						{
							ushort result;
							if (valid = UInt16.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.WORD = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.SHORT:
						{
							short result;
							if (valid = Int16.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.SHORT = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.DWORD:
						{
							uint result;
							if (valid = UInt32.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.DWORD = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.INT:
						{
							int result;
							if (valid = Int32.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.INT = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.DWORD64:
						{
							ulong result;
							if (valid = UInt64.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.DWORD64 = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.INT64:
						{
							long result;
							if (valid = Int64.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.INT64 = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.FLOAT:
						{
							float result;
							if (valid = Single.TryParse((val = TrimFloat(tb_Val.Text)), out result))
							{
								field.FLOAT = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.DOUBLE:
						{
							double result;
							if (valid = Double.TryParse((val = TrimFloat(tb_Val.Text)), out result))
							{
								field.DOUBLE = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.CResRef:
						{
							// nwn2-style resrefs (32-chars)
							// NOTE: The GFF-specification allows CResRef to be 255 bytes in length.
							if (tb_Val.Text.Length < 33 && isPrintableAscii(tb_Val.Text))
							{
								valid = true;

								val = tb_Val.Text.ToLower(CultureInfo.InvariantCulture);
								field.CResRef = val;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.CExoString:
						{
							editor = rt_Val;

							valid = true;
							field.CExoString = (_prevalText_rt = rt_Val.Text);
							break;
						}

						case FieldType.CExoLocString:
						{
							// NOTE: The GFF-specification stores strrefs as Uint32.
							val = TrimInteger(tb_Val.Text);

							bool isCust = cb_Custo.Checked;

							uint result;
							if (val == "-1")
							{
								valid = true;
								result = UInt32.MaxValue;
							}
							else if (UInt32.TryParse(val, out result))
							{
								if (result == UInt32.MaxValue)
								{
									valid = true;
									val = "-1";
								}
								else
								{
									isCust |= (result & Globals.BITS_CUSTOM) != 0;
									result &= ~Globals.BITS_CUSTOM;
									valid   = (result & Globals.BITS_UNUSED) == 0;
								}
							}

							if (valid)
							{
								if (result == UInt32.MaxValue)
								{
									cb_Custo.Visible =
									cb_Custo.Checked =
									_prevalCusto = false;
								}
								else
								{
									result &= Globals.BITS_STRREF;
									val = result.ToString();

									cb_Custo.Visible = true;

									if (_prevalCusto = isCust)
										result |= Globals.BITS_CUSTOM;
								}

								field.CExoLocStrref = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
								_prevalText_tb = val;
							}
							break;
						}

						case FieldType.VOID:
						{
							editor = rt_Val;

							val = rt_Val.Text.Trim();
							val = Regex.Replace(val, @"\s+", " ");

							char[] val2 = val.ToCharArray();	// formatted hexadecimal chars (all uppercase w/ spaces)
							string val3 = String.Empty;			// 'val' w/out spaces (upper and/or lowercase chars - will be converted to byte[])

							bool bork = false;
							for (int i = 0; i != val.Length && !bork; ++i)
							{
								if (i % 3 == 2)
								{
									if (val[i] != ' ')
									{
										bork = true;
										break;
									}
								}
								else
								{
									switch (val[i])
									{
										case 'a': val2[i] = 'A'; break;
										case 'b': val2[i] = 'B'; break;
										case 'c': val2[i] = 'C'; break;
										case 'd': val2[i] = 'D'; break;
										case 'e': val2[i] = 'E'; break;
										case 'f': val2[i] = 'F'; break;
									}

									switch (val2[i])
									{
										case '0': case '1': case '2': case '3': case '4':
										case '5': case '6': case '7': case '8': case '9':
										case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
											val3 += val2[i];
											break;

										default:
											bork = true;
											break;
									}
								}
							}

							if (!bork && (val3.Length & 1) == 0)
							{
								valid = true;
								field.VOID = ParseHecate(val3);

								rt_Val.Text = (_prevalText_rt = new string(val2));

								RepositionCaret(rt_Val);
							}
							break;
						}

//						case FieldType.List: // not editable

						case FieldType.Struct:
						{
							val = tb_Val.Text.Trim();
							if (   val.StartsWith("[", StringComparison.Ordinal)
								&& val.EndsWith(  "]", StringComparison.Ordinal))
							{
								uint result;
								if (UInt32.TryParse(val.Substring(1, val.Length - 2).Trim(), out result))
								{
									valid = true;
									field.Struct.typeid = result;

									val = Regex.Replace(val, @"\s+", String.Empty);

									if (val != tb_Val.Text)
									{
										tb_Val.Text = val;
										RepositionCaret(tb_Val);
									}
									_prevalText_tb = val;
								}
							}
							break;
						}

						case FieldType.locale:
						{
							editor = rt_Val;

							valid = true;

							locale = ((GffData.Field)node.Parent.Tag).Locales[(int)field.localeid];
							locale.local = (_prevalText_rt = rt_Val.Text);
							break;
						}
					}
				}

				if (valid)
				{
					if (field != null)
						node.Text = ConstructNodetext(field, locale);

					GffData.Changed = true;
					GffData = GffData;

					DirtyState = DIRTY_non;

					_tree.Select();
				}
				else
				{
					using (var f = new InfoDialog(Globals.Error, "That dog don't hunt."))
						f.ShowDialog(this);

					editor.Select();
				}
			}
		}

		/// <summary>
		/// force-Reselects the current treenode causing panel2 to repopulate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Revert(object sender, EventArgs e)
		{
			SelectField();
			_tree.Select();
		}


		/// <summary>
		/// Sets the dirty-state when the talktable flag is changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_Custo(object sender, EventArgs e)
		{
			if (cb_Custo.Checked != _prevalCusto)
			{
				DirtyState |= DIRTY_CZECH;
			}
			else
				DirtyState &= ~DIRTY_CZECH;
		}

		/// <summary>
		/// Sets textwrap in the multiline textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_Wordwrap(object sender, EventArgs e)
		{
			if (rt_Val.WordWrap = cb_Wordwrap.Checked)
			{
				rt_Val.ScrollBars = ScrollBars.Vertical;
			}
			else
				rt_Val.ScrollBars = ScrollBars.Both;
		}
		#endregion Handlers (panel2)


		#region Methods
		void Edit(Keys keyData)
		{
			TextBoxBase tb;
			if (tb_Val.ContainsFocus) tb = tb_Val;
			else                      tb = rt_Val;

			switch (keyData)
			{
				case Keys.Control | Keys.X: tb.Cut();   break;
				case Keys.Control | Keys.C: tb.Copy();  break;
				case Keys.Control | Keys.V: tb.Paste(); break;

				case Keys.Delete:
				{
					// NOTE: multiline textboxes are rather lousy editors
					// TODO: Reconsider a RichTextBox.

					int length;
					if (tb.SelectedText.Length != 0)
					{
						length = tb.SelectedText.Length;
					}
					else if (tb.SelectionStart != tb.Text.Length)
					{
						length = 1;

						if (tb.SelectionStart < tb.Text.Length - 2
							&& tb.Text.Substring(tb.SelectionStart,     1) == "\r"
							&& tb.Text.Substring(tb.SelectionStart + 1, 1) == "\n")
						{
							length = 2;
						}
					}
					else
						return;

					int pos = tb.SelectionStart;
					tb.Text = tb.Text.Substring(0, tb.SelectionStart)
							+ tb.Text.Substring(tb.SelectionStart + length,
												tb.Text.Length - tb.SelectionStart - length);
					tb.SelectionStart = pos;

					tb.ScrollToCaret();
					break;
				}
			}
		}


		/// <summary>
		/// Resets the textbox/richtextbox to a (hopefully) valid state.
		/// </summary>
		void ResetEditor(TextBoxBase editor)
		{
			editor.Text = _edittext;
			RepositionCaret(editor);
		}

		/// <summary>
		/// Repositions the textbox/richtextbox caret to a suitable position.
		/// </summary>
		/// <param name="editor"></param>
		void RepositionCaret(TextBoxBase editor)
		{
			if (_posCaret < editor.Text.Length)
				editor.SelectionStart = _posCaret;
			else
				editor.SelectionStart = editor.Text.Length;
		}


		/// <summary>
		/// Checks if the current data can be closed.
		/// </summary>
		/// <param name="quitbuttontext">the text to print on the quit-button</param>
		/// <returns>true if okay to close</returns>
		internal bool CheckCloseData(string quitbuttontext)
		{
			if (GffData != null && GffData.Changed && _tree.Nodes.Count != 0)
			{
				bool allowsave = GffData.Pfe != Globals.TopLevelStruct
							  && quitbuttontext != Globals.Reload;

				using (var f = new QuitDialog(quitbuttontext, allowsave))
				{
					switch (f.ShowDialog(this))
					{
//						case DialogResult.Ignore:	// "Close/Quit/Reload" - close/quit/reload don't save

						case DialogResult.Retry:	// "Save" - save and quit (not allowed unless CurrentData.Pfe is a valid path)
							return GffWriter.WriteGFFfile(GffData.Pfe, _tree, GffData.TypeVer);

						case DialogResult.Abort:	// "Cancel" - don't quit
							return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Resets the controls of panel2.
		/// </summary>
		internal void ResetEditPanel()
		{
			DirtyState = GeneralGFF.DIRTY_non;

			la_Des.Text =
			la_Val.Text =
			tb_Val.Text =
			rt_Val.Text = String.Empty;

			tb_Val.Enabled   = false;
			tb_Val.BackColor = Color.Thistle;

			rt_Val.Enabled   = false;
			rt_Val.BackColor = Color.Thistle;

			cb_Custo.Visible =
			cb_Custo.Checked = false;

			cb_Wordwrap.Visible = false;

			_edittext =
			_prevalText_rt =
			_prevalText_tb = String.Empty;
			_prevalCusto = false;

			tssl_info.Text = String.Empty;
		}

		/// <summary>
		/// Populates (or re-populates) the editpanel.
		/// @note Ensure that '_tree.SelectedNode' is valid before call.
		/// </summary>
		internal void SelectField()
		{
			ResetEditPanel();

			var labels = new List<string>(); // print tree-path to statusbar ->
			var node = _tree.SelectedNode;
			do
			{
				if (node.Tag == null) // is TopLevelStruct
				{
					labels.Add(node.Text);
					break;
				}
				labels.Add(((Sortable)node)._label);
			}
			while ((node = node.Parent) != null);

			string path = String.Empty;
			for (int i = labels.Count - 1; i != -1; --i)
			{
				path += labels[i];
				if (i != 0) path += "|";
			}
			tssl_info.Text = path;


			if (_tree.SelectedNode.Tag == null) // is TopLevelStruct's node
			{
				la_Des.Text = "ASCII";
				la_Val.Text = "GFF type + version";

				tb_Val.Enabled   = true;
				tb_Val.BackColor = Color.Violet;

				tb_Val.Text = (_prevalText_tb = _edittext = GffData.TypeVer);
			}
			else
			{
				var field = (GffData.Field)_tree.SelectedNode.Tag;

				switch (field.type)
				{
					case FieldType.BYTE:
						la_Des.Text = Byte.MinValue + ".." + Byte.MaxValue;
						la_Val.Text = "BYTE";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.BYTE.ToString());
						break;

					case FieldType.CHAR:
						la_Des.Text = SByte.MinValue + ".." + SByte.MaxValue;
						la_Val.Text = "CHAR";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.CHAR.ToString());
						break;

					case FieldType.WORD:
						la_Des.Text = UInt16.MinValue + ".." + UInt16.MaxValue;
						la_Val.Text = "WORD";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.WORD.ToString());
						break;

					case FieldType.SHORT:
						la_Des.Text = Int16.MinValue + ".." + Int16.MaxValue;
						la_Val.Text = "SHORT";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.SHORT.ToString());
						break;

					case FieldType.DWORD:
						la_Des.Text = UInt32.MinValue + ".." + UInt32.MaxValue;
						la_Val.Text = "DWORD";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.DWORD.ToString());
						break;

					case FieldType.INT:
						la_Des.Text = Int32.MinValue + ".." + Int32.MaxValue;
						la_Val.Text = "INT";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.INT.ToString());
						break;

					case FieldType.DWORD64:
						la_Des.Text = UInt64.MinValue + ".." + UInt64.MaxValue;
						la_Val.Text = "DWORD64";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.DWORD64.ToString());
						break;

					case FieldType.INT64:
						la_Des.Text = Int64.MinValue + ".." + Int64.MaxValue;
						la_Val.Text = "INT64";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.INT64.ToString());
						break;

					case FieldType.FLOAT:
					{
						la_Des.Text = Single.MinValue + ".." + Single.MaxValue;
						la_Val.Text = "FLOAT";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						string f = field.FLOAT.ToString();
						if (!f.Contains(".")) f += ".0";

						tb_Val.Text = (_prevalText_tb = _edittext = f);
						break;
					}

					case FieldType.DOUBLE:
					{
						la_Des.Text = Double.MinValue + ".." + Double.MaxValue;
						la_Val.Text = "DOUBLE";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						string d = field.DOUBLE.ToString();
						if (!d.Contains(".")) d += ".0";

						tb_Val.Text = (_prevalText_tb = _edittext = d);
						break;
					}

					case FieldType.CResRef:
						la_Des.Text = "32-chars NwN2 / 16-chars NwN1" + Environment.NewLine + "ASCII lc";
						la_Val.Text = "CResRef";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = field.CResRef);
						break;

					case FieldType.CExoString:
						la_Des.Text = "UTF8";
						la_Val.Text = "CExoString";

						rt_Val.Enabled   = true;
						rt_Val.BackColor = Color.Honeydew;

						cb_Wordwrap.Visible = true;

						rt_Val.Text = (_prevalText_rt = _edittext = field.CExoString);
						break;

					case FieldType.CExoLocString: // not a string. Is an integer.
					{
						la_Des.Text = "strref" + Environment.NewLine + "-1.." + Globals.BITS_STRREF;
						la_Val.Text = "CExoLocString";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						uint strref = field.CExoLocStrref;
						if (strref == UInt32.MaxValue)
						{
							tb_Val.Text = (_prevalText_tb = _edittext = "-1");
						}
						else
						{
							cb_Custo.Visible = true;
							cb_Custo.Checked = (_prevalCusto = (strref & Globals.BITS_CUSTOM) != 0);

							tb_Val.Text = (_prevalText_tb = _edittext = (strref & Globals.BITS_STRREF).ToString());
						}
						break;
					}

					case FieldType.VOID:
					{
						la_Des.Text = "binary data";
						la_Val.Text = "VOID";

						rt_Val.Enabled   = true;
						rt_Val.BackColor = Color.Honeydew;

						cb_Wordwrap.Visible = true;

						rt_Val.Text = (_prevalText_rt = _edittext = BitConverter.ToString(field.VOID)
																				.Replace("-", " ")
																				.ToUpper(CultureInfo.InvariantCulture));
						break;
					}

					case FieldType.List:
						la_Val.Text = "List";
						break;

					case FieldType.Struct:
						la_Des.Text = "TypeId" + Environment.NewLine + UInt32.MinValue + ".." + UInt32.MaxValue;
						la_Val.Text = "Struct";

						tb_Val.Enabled   = true;
						tb_Val.BackColor = Color.Honeydew;

						tb_Val.Text = (_prevalText_tb = _edittext = "[" + field.Struct.typeid + "]");
						break;

					case FieldType.locale:
					{
						var parent = (GffData.Field)_tree.SelectedNode.Parent.Tag;
						GffData.Locale locale = parent.Locales[(int)field.localeid];

						if (locale.langid == Language.GffToken)
						{
							la_Des.Text = "UTF8";
							la_Val.Text = "GffToken";
						}
						else
						{
							la_Des.Text = "UTF8 localized";
							la_Val.Text = "locale";
						}

						rt_Val.Enabled   = true;
						rt_Val.BackColor = Color.Honeydew;

						cb_Wordwrap.Visible = true;

						rt_Val.Text = (_prevalText_rt = _edittext = locale.local);
						break;
					}
				}
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Checks if a string is printable ascii.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		static bool isPrintableAscii(string text)
		{
			int c;
			for (int i = 0; i != text.Length; ++i)
			{
				c = (int)text[i];
				if (c < 32 || c > 126)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if a string is hexadecimal.
		/// @note Spaces are also valid.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		static bool isHexadecimal(string text)
		{
			int c;
			for (int i = 0; i != text.Length; ++i)
			{
				if ((c = (int)text[i]) != 32
					&& (    c <  48
						|| (c >  57 && c < 65)
						|| (c >  70 && c < 97)
						||  c > 102))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Trims an integer-string.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		static string TrimInteger(string val)
		{
			val = Regex.Replace(val, @"\s+", String.Empty);
			val = val.TrimStart('0');

			bool negative;
			if (val.StartsWith("-", StringComparison.Ordinal))
			{
				negative = true;
				val = val.Substring(1);
			}
			else
				negative = false;

			val = val.TrimStart('0');

			if (String.IsNullOrEmpty(val))
				return "0";

			if (negative)
				val = "-" + val;

			return val;
		}

		/// <summary>
		/// Trims and formats a float-string.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		static string TrimFloat(string val)
		{
			val = Regex.Replace(val, @"\s+", String.Empty);
			val = val.TrimStart('0');

			if (String.IsNullOrEmpty(val))
				return "0.0";

			if (!val.Contains("."))
				return val + ".0";

			val = val.TrimEnd('0');

			if (val.StartsWith(".", StringComparison.Ordinal))
				val = "0" + val;

			if (val.EndsWith(".", StringComparison.Ordinal))
				val += "0";

			return val;
		}


		// https://stackoverflow.com/questions/14332496/most-light-weight-conversion-from-hex-to-byte-in-c/14332574#14332574
		/// <summary>
		/// Parses a hexadecimal string into a byte-array.
		/// @note Ensure that the string has only valid hex-chars before call.
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		static byte[] ParseHecate(string hex)
		{
			int length = hex.Length / 2;
			var b = new byte[length];
			for (int i = 0, j = -1; i != length; ++i)
			{
				int hi = getnibble(hex[++j]);
				int lo = getnibble(hex[++j]);
				b[i] = (byte)((hi << 4) | lo);
			}
			return b;
		}

		/// <summary>
		/// - helper for ParseHex()
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		static int getnibble(char c)
		{
			switch (c)
			{
				case '0': case '1': case '2': case '3': case '4':
				case '5': case '6': case '7': case '8': case '9':
					return c - '0';

				case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
					return c - ('A' - 10);

				case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
					return c - ('a' - 10);
			}
			return -1;
		}
		#endregion Methods (static)
	}


	/// <summary>
	/// Lifted from StackOverflow.com:
	/// https://stackoverflow.com/questions/1600962/displaying-the-build-date#answer-1600990
	/// - what a fucking pain in the ass.
	/// </summary>
	static class DateTimeExtension
	{
		/// <summary>
		/// Gets the time/date of build timestamp.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		internal static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
		{
			var filePath = assembly.Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;

			var buffer = new byte[2048];

			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				stream.Read(buffer, 0, 2048);

			var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			return epoch.AddSeconds(secondsSince1970);
/*			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

			var tz = target ?? TimeZoneInfo.Local;
			var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

			return localTime; */
		}
	}
}
