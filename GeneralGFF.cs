﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;


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

		const int LENGTH_LABEL = 17;
		const int LENGTH_TYPE  = 17;

		const int MI_FILE = 0;
		const int MI_EDIT = 1;
		const int MI_VIEW = 2;
		const int MI_HELP = 3;

		const int MI_FILE_OPEN   = 0;
		const int MI_FILE_SAVE   = 1;
		const int MI_FILE_QUIT   = 3;

		const int MI_EDIT_SEARCH = 0;

		const int MI_VIEW_EXPAND = 0;
		const int MI_VIEW_COLLAP = 1;

		const int MI_HELP_ABOUT  = 0;
		#endregion Fields (static)


		#region Fields
		internal TreeList _tl;

		internal string _prevalText = String.Empty;
		internal bool _prevalChecker;

		string _editText = String.Empty;
		int _posCaret = 0;
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
				if ((_data = value) != null)
				{
					Text = TITLE + " - " + _data.Pfe;

					if (_data.Changed)
						Text += " *";
				}
				else
					Text = TITLE;
			}
		}
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


			Menu = new MainMenu();

			Menu.MenuItems.Add("&File"); // #0

			Menu.MenuItems[MI_FILE].Popup += filepop;

			Menu.MenuItems[MI_FILE].MenuItems.Add("&Open GFF file ...");	// #0
			Menu.MenuItems[MI_FILE].MenuItems[MI_FILE_OPEN].Click += fileclick_Open;

			Menu.MenuItems[MI_FILE].MenuItems.Add("&Save GFF file ...");	// #1
			Menu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVE].Click += fileclick_Save;

			Menu.MenuItems[MI_FILE].MenuItems.Add("-");						// #2

			Menu.MenuItems[MI_FILE].MenuItems.Add("&Quit");					// #3
			Menu.MenuItems[MI_FILE].MenuItems[MI_FILE_QUIT].Click += fileclick_Quit;


			Menu.MenuItems.Add("&Edit"); // #1

			Menu.MenuItems[MI_EDIT].MenuItems.Add("&Search");	// #0
			Menu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_SEARCH].Click += editclick_Search;


			Menu.MenuItems.Add("&View"); // #2

			Menu.MenuItems[MI_VIEW].Popup += viewpop;

			Menu.MenuItems[MI_VIEW].MenuItems.Add("&Expand all under selected");	// #0
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPAND].Click += viewclick_ExpandSelected;

			Menu.MenuItems[MI_VIEW].MenuItems.Add("&Collapse all under selected");	// #1
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLLAP].Click += viewclick_CollapseSelected;


			Menu.MenuItems.Add("&Help"); // #3

//			Menu.MenuItems[MI_HELP].MenuItems.Add("&Help");		// #
//			Menu.MenuItems[MI_HELP].MenuItems[0].Click += ;

			Menu.MenuItems[MI_HELP].MenuItems.Add("&About");	// #0
			Menu.MenuItems[MI_HELP].MenuItems[MI_HELP_ABOUT].Click += helpclick_About;

			_tl = new TreeList(this);
			sc_body.Panel1.Controls.Add(_tl);
			_tl.Select();


			MinimumSize = new Size(sc_body.Panel1MinSize + sc_body.SplitterWidth
														 + Width - ClientSize.Width, // <- border
								   150);
			sc_body.Panel1.ClientSize = new Size(sc_body.Panel1MinSize, sc_body.Panel1.Height);


			if (File.Exists(filearg))
			{
				var loader = new GffLoader();
				loader.LoadGFFfile(this, filearg);
			}
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
					  && locale.langid == Languages.GffToken;

			string label2 = " [" + GetTypeString(field.type, token) + "]";
			while (label2.Length != LENGTH_TYPE)
				label2 += " ";

			label += label2;

			switch (field.type)
			{
				default:
					return label + "= " + GetValueString(field);

				case FieldTypes.locale:
					return label + "= " + locale.local;

				case FieldTypes.List:
					return label;
			}
		}

		/// <summary>
		/// Converts a FieldTypes into a readable string.
		/// @note helper for ConstructNodetext()
		/// </summary>
		/// <param name="type"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		static string GetTypeString(FieldTypes type, bool token)
		{
			switch (type)
			{
				case FieldTypes.BYTE:          return "BYTE";
				case FieldTypes.CHAR:          return "CHAR";
				case FieldTypes.WORD:          return "WORD";
				case FieldTypes.SHORT:         return "SHORT";
				case FieldTypes.DWORD:         return "DWORD";
				case FieldTypes.INT:           return "INT";
				case FieldTypes.DWORD64:       return "DWORD64";
				case FieldTypes.INT64:         return "INT64";
				case FieldTypes.FLOAT:         return "FLOAT";
				case FieldTypes.DOUBLE:        return "DOUBLE";
				case FieldTypes.CResRef:       return "CResRef";
				case FieldTypes.CExoString:    return "CExoString";
				case FieldTypes.CExoLocString: return "CExoLocString";
				case FieldTypes.VOID:          return "VOID";
				case FieldTypes.List:          return "List";
				case FieldTypes.Struct:        return "Struct";

				case FieldTypes.locale:
					if (token) return "token";
					return "locale";
			}
			return "ErROr: field-type unknown";
		}

		/// <summary>
		/// Gets the value of a Field by its type.
		/// @note helper for ConstructNodetext()
		/// </summary>
		/// <param name="field"></param>
		/// <returns>the value as a string</returns>
		static string GetValueString(GffData.Field field)
		{
			switch (field.type)
			{
				case FieldTypes.BYTE:
					return field.BYTE.ToString();

				case FieldTypes.CHAR:
					return field.CHAR.ToString();

				case FieldTypes.WORD:
					return field.WORD.ToString();

				case FieldTypes.SHORT:
					return field.SHORT.ToString();

				case FieldTypes.DWORD:
					return field.DWORD.ToString();

				case FieldTypes.INT:
					return field.INT.ToString();

				case FieldTypes.DWORD64:
					return field.DWORD64.ToString();

				case FieldTypes.INT64:
					return field.INT64.ToString();

				case FieldTypes.FLOAT:
				{
					string f = field.FLOAT.ToString();
					if (!f.Contains(".")) f += ".0";
					return f;
				}

				case FieldTypes.DOUBLE:
				{
					string d = field.DOUBLE.ToString();
					if (!d.Contains(".")) d += ".0";
					return d;
				}

				case FieldTypes.CResRef:
					return field.CResRef;

				case FieldTypes.CExoString:
					return field.CExoString;

				case FieldTypes.CExoLocString:
				{
					uint strref = field.CExoLocStrref;
					if (strref != UInt32.MaxValue)
						return strref.ToString();

					return "-1";
				}

				case FieldTypes.VOID:
					return "bindata";

				case FieldTypes.List:
					return String.Empty;

				case FieldTypes.Struct:
					return "[" + field.Struct.typeid + "]";
			}
			return "ErROr: field type unknown";
		}
		#endregion Methods (static)


		#region Handlers (override)
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			e.Cancel = !CheckCloseData();
		}
		#endregion Handlers (override)


		#region Handlers (menu)
		void filepop(object sender, EventArgs e)
		{
			Menu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVE].Enabled = _tl.Nodes.Count != 0;
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Open(object sender, EventArgs e)
		{
			if (CheckCloseData())
			{
				using (var ofd = new OpenFileDialog())
				{
//					ofd.InitialDirectory = ;

					ofd.Title  = "Select a GFF file";
					ofd.Filter = GffData.FileDialogFilter;

					if (ofd.ShowDialog(this) == DialogResult.OK)
					{
						var loader = new GffLoader();
						loader.LoadGFFfile(this, ofd.FileName);
					}
				}
			}
		}

		/// <summary>
		/// Saves the currently loaded file.
		/// @note This function is actually a SaveAs routine.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Save(object sender, EventArgs e)
		{
			if (_tl.Nodes.Count != 0)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title  = "Save as GFF file";
					sfd.Filter = GffData.FileDialogFilter;

//					sfd.DefaultExt = "GFF";

					if (GffData.Pfe != Globals.TopLevelStruct)
					{
						sfd.InitialDirectory = Path.GetDirectoryName(GffData.Pfe);
						sfd.FileName         = Path.GetFileName(GffData.Pfe);
					}

					if (sfd.ShowDialog(this) == DialogResult.OK
						&& GffWriter.WriteGFFfile(sfd.FileName, _tl, GffData.Ver))
					{
						string label = Path.GetFileNameWithoutExtension(sfd.FileName).ToUpper();
						_tl.Nodes[0].Text = label; // update TLS-label

						GffData.Pfe = sfd.FileName;
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
		/// Opens the Search dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_Search(object sender, EventArgs e)
		{
			var f = new SearchDialog(_tl);
			f.Show(this);
		}


		/// <summary>
		/// Enables/disables Edit-menu's items appropriately.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewpop(object sender, EventArgs e)
		{
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPAND].Enabled =
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLLAP].Enabled = _tl.SelectedNode != null
																	 && _tl.SelectedNode.Nodes.Count != 0;
		}

		/// <summary>
		/// Expands the currently selected treenode and all childs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void viewclick_ExpandSelected(object sender, EventArgs e)
		{
			if (_tl.SelectedNode != null)
			{
				_tl.SelectedNode.Expand();
				ExpandChildren(_tl.SelectedNode);

				if (!_tl.SelectedNode.IsVisible)
					_tl.TopNode = _tl.SelectedNode;
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
			if (_tl.SelectedNode != null)
			{
				_tl.SelectedNode.Collapse();
				CollapseChildren(_tl.SelectedNode);
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
		/// Shows the About box w/ version and build config.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void helpclick_About(object sender, EventArgs e)
		{
			var an = Assembly.GetExecutingAssembly().GetName();
			string text = an.Version.Major + "."
						+ an.Version.Minor + "."
						+ an.Version.Build + "."
						+ an.Version.Revision;
#if DEBUG
			text += " debug";
#else
			text += " release";
#endif
			MessageBox.Show(
						this,
						text,
						" about GeneralGFF",
						MessageBoxButtons.OK,
						MessageBoxIcon.None,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion Handlers (menu)


		#region Handlers (panel2)
		internal const int DIRTY_non   = 0x0;
				 const int DIRTY_TEXTS = 0x1;
				 const int DIRTY_CZECH = 0x2;

		int _enableapply;
		/// <summary>
		/// Enables or disables the apply and revert buttons in the editor-panel.
		/// </summary>
		internal int EnableApply
		{
			get { return _enableapply; }
			set
			{
				btn_Revert.Enabled =
				btn_Apply .Enabled = (_enableapply = value) != DIRTY_non;
			}
		}

		/// <summary>
		/// force-Reselects the current treenode causing panel2 to repopulate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Revert(object sender, EventArgs e)
		{
			_tl.SelectField(_tl.SelectedNode);
			EnableApply = DIRTY_non;
		}


		/// <summary>
		/// Shunts focus away from the richtextbox if it's not "enabled".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void enter_Richtextbox(object sender, EventArgs e)
		{
			tb_Val.Select();
		}


		/// <summary>
		/// Tracks the position of the caret in the textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void mousedown_Textbox(object sender, MouseEventArgs e)
		{
			_posCaret = tb_Val.SelectionStart;
		}

		/// <summary>
		/// Tracks the position of the caret in the textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keyup(object sender, KeyEventArgs e)
		{
			if ((TextBoxBase)sender == tb_Val)
				_posCaret = tb_Val.SelectionStart;
			else
				_posCaret = rt_Val.SelectionStart;
		}

		/// <summary>
		/// Handles keydowns in the textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Textbox(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
					e.SuppressKeyPress = true;
					btn_Apply.PerformClick();

					_tl.Select();
					break;

				case Keys.Escape:
					e.SuppressKeyPress = true;
					btn_Revert.PerformClick();

					_tl.Select();
					break;
			}
		}

		/// <summary>
		/// Prevents non-ASCII characters in CResRefs and/or the
		/// TopLevelStruct's type+version info.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Textbox(object sender, EventArgs e)
		{
			if (_tl.SelectedNode != null)
			{
				object tag = _tl.SelectedNode.Tag;

				if (tag == null) // is TopLevelStruct's node
				{
					if (!isPrintableAscii(tb_Val.Text))
					{
						ResetEditor(tb_Val);
					}
					else
						_editText = tb_Val.Text;
				}
				else
				{
					switch (((GffData.Field)tag).type)
					{
						case FieldTypes.CResRef:
							if (!isPrintableAscii(tb_Val.Text))
							{
								ResetEditor(tb_Val);
							}
							else
								_editText = tb_Val.Text;
							break;
					}
				}
			}

			if (!_tl.BypassChanged) // TODO: this could possibly go inside (_tl.SelectedNode != null) but not sure if/when that can be false.
			{
				if (tb_Val.Text != _prevalText)
				{
					EnableApply |= DIRTY_TEXTS;
				}
				else
					EnableApply &= ~DIRTY_TEXTS;
			}
		}


		/// <summary>
		/// Tracks the position of the caret in the richtextbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void mousedown_Richtextbox(object sender, MouseEventArgs e)
		{
			_posCaret = rt_Val.SelectionStart;
		}

		/// <summary>
		/// Handles keydowns in the richtextbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Richtextbox(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					e.SuppressKeyPress = true;
					btn_Revert.PerformClick();

					_tl.Select();
					break;
			}
		}

		/// <summary>
		/// Prevents non-ASCII characters in CExoString and non-hexadecimal
		/// characters in VOID.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Richtextbox(object sender, EventArgs e)
		{
			switch (((GffData.Field)_tl.SelectedNode.Tag).type)
			{
				case FieldTypes.CExoString:
					if (!isPrintableAscii(rt_Val.Text))
					{
						ResetEditor(rt_Val);
					}
					else
						_editText = rt_Val.Text;
					break;

				case FieldTypes.VOID:
					if (!isHexadecimal(rt_Val.Text))
					{
						ResetEditor(rt_Val);
					}
					else
						_editText = rt_Val.Text;
					break;
			}

			if (!_tl.BypassChanged)
			{
				if (rt_Val.Text != _prevalText)
				{
					EnableApply |= DIRTY_TEXTS;
				}
				else
					EnableApply &= ~DIRTY_TEXTS;
			}
		}


		/// <summary>
		/// Applies changed data to the currently selected field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Apply(object sender, EventArgs e)
		{
			TreeNode node = _tl.SelectedNode;
			if (node != null) // safety.
			{
				string val = null; // the string to test

				GffData.Field field = null;
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

						GffData.Ver = val;
						GffData.Type = GffData.GetGffType(val.Substring(0,3));

						tb_Val.Text = val;
						RepositionCaret(tb_Val);
					}
				}
				else
				{
					field = (GffData.Field)node.Tag;
					switch (field.type)
					{
						case FieldTypes.BYTE:
						{
							int length = tb_Val.Text.Length;

							byte result;
							if (valid = Byte.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.BYTE = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.CHAR:
						{
							int length = tb_Val.Text.Length;

							sbyte result;
							if (valid = sbyte.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.CHAR = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.WORD:
						{
							int length = tb_Val.Text.Length;

							ushort result;
							if (valid = UInt16.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.WORD = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.SHORT:
						{
							int length = tb_Val.Text.Length;

							short result;
							if (valid = Int16.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.SHORT = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.DWORD:
						{
							int length = tb_Val.Text.Length;

							uint result;
							if (valid = UInt32.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.DWORD = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.INT:
						{
							int length = tb_Val.Text.Length;

							int result;
							if (valid = Int32.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.INT = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.DWORD64:
						{
							int length = tb_Val.Text.Length;

							ulong result;
							if (valid = UInt64.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.DWORD64 = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.INT64:
						{
							int length = tb_Val.Text.Length;

							long result;
							if (valid = Int64.TryParse((val = TrimInteger(tb_Val.Text)), out result))
							{
								field.INT64 = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.FLOAT:
						{
							int length = tb_Val.Text.Length;

							float result;
							if (valid = float.TryParse((val = TrimFloat(tb_Val.Text)), out result))
							{
								field.FLOAT = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.DOUBLE:
						{
							int length = tb_Val.Text.Length;

							double result;
							if (valid = Double.TryParse((val = TrimFloat(tb_Val.Text)), out result))
							{
								field.DOUBLE = result;

								if (val != tb_Val.Text)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.CResRef:
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
							}
							break;
						}

						case FieldTypes.CExoString:
						{
							if (isPrintableAscii(rt_Val.Text))
							{
								valid = true;
								field.CExoString = (val = rt_Val.Text);
							}
							break;
						}

						case FieldTypes.CExoLocString:
						{
							// NOTE: The GFF-specification stores strrefs as Uint32.
							int length = tb_Val.Text.Length;

							val = TrimInteger(tb_Val.Text);

							bool isCust = cb_Checker.Checked;

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
									isCust |= (result & 0x01000000) != 0;
									result &= ~(unchecked((uint)0x01000000));
									valid   = (result & 0xFF000000) == 0;
								}
							}

							if (valid)
							{
								if (result != UInt32.MaxValue)
								{
									result &= 0x00FFFFFF;
									val = result.ToString();
									length = -1;

									if (_prevalChecker = isCust)
										result |= 0x01000000;
								}
								else
									_prevalChecker = cb_Checker.Checked = false;

								field.CExoLocStrref = result;

								if (length != val.Length)
								{
									tb_Val.Text = val;
									RepositionCaret(tb_Val);
								}
							}
							break;
						}

						case FieldTypes.VOID:
						{
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

								rt_Val.Text = (val = new string(val2)); // freshen the richtextbox

								RepositionCaret(rt_Val);
							}
							break;
						}

//						case FieldTypes.List: // not editable

						case FieldTypes.Struct:
						{
							int length = tb_Val.Text.Length;

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

									if (length != val.Length)
									{
										tb_Val.Text = val;
										RepositionCaret(tb_Val);
									}
								}
							}
							break;
						}

						case FieldTypes.locale:
						{
							valid = true;

							var CExoLocString = (GffData.Field)node.Parent.Tag;
							locale = CExoLocString.Locales[(int)field.localeid];

							locale.local = val = rt_Val.Text;

							cb_Checker.Checked &= !String.IsNullOrEmpty(val);

							field.label = GffData.Locale.GetLanguageString(locale.langid);
							if (locale.F = _prevalChecker = cb_Checker.Checked)
								field.label += Globals.SUF_F;

							break;
						}
					}
				}

				if (valid)
				{
					if (field != null)
						node.Text = ConstructNodetext(field, locale);

					_prevalText = val;

					GffData.Changed = true;
					GffData = GffData;

					EnableApply = DIRTY_non;
				}
				else
					baddog("That dog don't hunt.");
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void checkchanged_Checker(object sender, EventArgs e)
		{
			if (!_tl.BypassChanged)
			{
				if (cb_Checker.Checked != _prevalChecker)
				{
					EnableApply |= DIRTY_CZECH;
				}
				else
					EnableApply &= ~DIRTY_CZECH;
			}
		}
		#endregion Handlers (panel2)


		#region Methods
		/// <summary>
		/// Resets the textbox/richtextbox to a (hopefully) valid state.
		/// </summary>
		void ResetEditor(TextBoxBase tbb)
		{
			tbb.Text = _editText;
			RepositionCaret(tbb);
		}

		/// <summary>
		/// Repositions the textbox/richtextbox caret to a suitable position.
		/// </summary>
		/// <param name="tbb"></param>
		void RepositionCaret(TextBoxBase tbb)
		{
			if (_posCaret < tbb.Text.Length)
				tbb.SelectionStart = _posCaret;
			else
				tbb.SelectionStart = tbb.Text.Length;
		}


		/// <summary>
		/// Generic error dialog.
		/// </summary>
		/// <param name="error"></param>
		void baddog(string error)
		{
			MessageBox.Show(
						this,
						error,
						" Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}


		/// <summary>
		/// Checks if the current data can be closed.
		/// </summary>
		/// <returns>true if okay to close</returns>
		internal bool CheckCloseData()
		{
			if (GffData != null && GffData.Changed && _tl.Nodes.Count != 0)
			{
				using (var f = new QuitDialog(GffData.Pfe != Globals.TopLevelStruct))
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Abort:	// "Cancel" - don't quit
							return false;

//						case DialogResult.Ignore:	// "Quit" - quit don't save

						case DialogResult.Retry:	// "Save" - save and quit (not allowed unless CurrentData.Pfe is a valid path)
							return GffWriter.WriteGFFfile(GffData.Pfe, _tl, GffData.Ver);
					}
				}
			}
			return true;
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Roll yer own keyboard-navigation keys checker.
		/// @note 'keycode' shall include modifers - ie, pass in KeyCode not
		/// KeyData.
		/// </summary>
		/// <param name="keycode"></param>
		/// <returns></returns>
		static bool isNavigation(Keys keycode)
		{
			switch (keycode)
			{
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Back:
				case Keys.Delete:
					return true;
			}
			return false;
		}

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
}
