using System;
using System.Collections.Generic;
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

		internal const string SUF_F = "[F]";

		const int MI_FILE = 0;
		const int MI_VIEW = 1;
		const int MI_HELP = 2;

		const int MI_FILE_OPEN   = 0;
		const int MI_FILE_SAVE   = 1;
		const int MI_FILE_QUIT   = 3;

		const int MI_VIEW_EXPAND = 0;
		const int MI_VIEW_COLLAP = 1;

		const int MI_HELP_ABOUT  = 0;
		#endregion Fields (static)


		#region Fields
		internal TreeList _tl;

		internal string _prevalText = String.Empty;
		internal bool _prevalCheckboxChecked;

		string _editText = String.Empty;
		int _posCaret = 0;
		#endregion Fields


		#region Properties
		GffData _data;
		/// <summary>
		/// The currently loaded GFF data.
		/// </summary>
		internal GffData CurrentData
		{
			get { return _data; }
			set
			{
				if ((_data = value) != null)
					Text = TITLE + " - " + _data._pfe;
				else
					Text = TITLE;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Instantiates the GeneralGFF f.
		/// </summary>
		internal GeneralGFF()
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


			Menu.MenuItems.Add("&View"); // #1

			Menu.MenuItems[MI_VIEW].Popup += editpop;

			Menu.MenuItems[MI_VIEW].MenuItems.Add("&Expand all in selected");	// #0
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPAND].Click += editclick_ExpandSelected;

			Menu.MenuItems[MI_VIEW].MenuItems.Add("&Collapse all in selected");	// #1
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLLAP].Click += editclick_CollapseSelected;


			Menu.MenuItems.Add("&Help"); // #2

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


			var t1 = new Timer(); // workaround that bypasses TextChanged ...
			t1.Tick += OnTick;
			t1.Interval = 100;
			t1.Start();


			//LoadGFFfile(@"C:\Users\User\Documents\Neverwinter Nights 2\override\f03_malarite_out.UTC");
		}
		#endregion cTor


		#region Methods
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
		#endregion Methods


		#region Handlers (menu)
		void filepop(object sender, EventArgs e)
		{
			Menu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVE].Enabled = CurrentData != null;
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Open(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
//				ofd.InitialDirectory = ;

				ofd.Title  = "Select a GFF file";
//				ofd.Filter = "GFF files (*.GFF)|*.GFF|All files (*.*)|*.*";

				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					var loader = new GffLoader();
					loader.LoadGFFfile(this, ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Saves the currently loaded file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Save(object sender, EventArgs e)
		{
			using (var sfd = new SaveFileDialog())
			{
				sfd.Title      = "Save as GFF file";
//				sfd.Filter     = "GFF files (*.GFF)|*.GFF|All files (*.*)|*.*";
//				sfd.DefaultExt = "GFF";

				sfd.InitialDirectory = Path.GetDirectoryName(CurrentData._pfe);
				sfd.FileName         = Path.GetFileName(CurrentData._pfe);

				if (sfd.ShowDialog(this) == DialogResult.OK)
				{
					GffWriter.WriteGFFfile(sfd.FileName, _tl, CurrentData.Ver);
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
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPAND].Enabled =
			Menu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLLAP].Enabled = _tl.SelectedNode != null;
		}

		/// <summary>
		/// Expands the currently selected treenode and all childs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_ExpandSelected(object sender, EventArgs e)
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
		void editclick_CollapseSelected(object sender, EventArgs e)
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
/*		/// <summary>
		/// GLITCH: This funct produces a bad flicker on both the textbox and
		/// the apply-button. But not when the text changes ... when items are
		/// selected in the TreeList. Literally. The text can stay the same (ie,
		/// this funct does not fire) but when clicking on the TreeList, if
		/// this function has a body the textbox and apply-button flicker; but
		/// if the body of this function is commented out, the flicker goes away
		/// completely - that is, the function call itself produces flicker.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Textbox(object sender, EventArgs e)
		{
			//tb_Val
			if (!_tl._bypassTextChanged)
			{
				if ((sender as TextBox).Text != _preval)
					btn_Apply.Enabled = true;
				else
					btn_Apply.Enabled = false;
			}
			else
				_tl._bypassTextChanged = false;
		} */
		void OnTick(object sender, EventArgs e)
		{
			if (_tl.SelectedNode != null)
			{
				string text;
				if (_tl.SelectedNode.Tag != null)
				{
					switch (((GffData.Field)_tl.SelectedNode.Tag).type)
					{
						default:
							text = tb_Val.Text;
							break;

						case FieldTypes.CExoString:
						case FieldTypes.VOID:
						case FieldTypes.locale:
							text = rt_Val.Text;
							break;
					}
				}
				else // is TopLevelStruct
				{
					text = tb_Val.Text;
				}

				btn_Revert.Enabled =
				btn_Apply .Enabled = text != _prevalText
								  || (cb_Checker.Visible && cb_Checker.Checked != _prevalCheckboxChecked);

				return;
			}

			btn_Revert.Enabled =
			btn_Apply .Enabled = false;
		}


		/// <summary>
		/// pseudo-Reselects the current treenode causing panel2 to repopulate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Revert(object sender, EventArgs e)
		{
			_tl.SelectField(_tl.SelectedNode);
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
		/// Tracks the position of the caret in the textbox. Also applies
		/// changed data to a field if the textbox is focused and [Enter] is
		/// keydown'd.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Textbox(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				btn_Apply.PerformClick();
			}
			else
				_posCaret = tb_Val.SelectionStart;
		}

		/// <summary>
		/// Prevents non-ASCII characters in CResRefs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Textbox(object sender, EventArgs e)
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

						// Regex.Replace(tb_Val.Text, @"[^\u0020-\u007E]", string.Empty)
						break;
				}
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
		/// Tracks the position of the caret in the richtextbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Richtextbox(object sender, KeyEventArgs e)
		{
			_posCaret = rt_Val.SelectionStart;
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

					// Regex.Replace(rt_Val.Text, @"[^\u0020-\u007E]", String.Empty);
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
				string val = null; // the string to test ->

				GffData.Field field = null;
				bool valid = false;
				GffData.Locale locale = null;

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

						CurrentData.Ver = val;
						CurrentData.Type = GffData.GetGffType(val.Substring(0,3));

						tb_Val.Text = val;

						++_posCaret;
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
							if (valid = Byte.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = sbyte.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = UInt16.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = Int16.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = UInt32.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = Int32.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = UInt64.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = Int64.TryParse((val = tb_Val.Text.Trim()), out result))
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
							if (valid = float.TryParse((val = tb_Val.Text.Trim()), out result))
							{
								field.FLOAT = result;

								if (length != val.Length)
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
							if (valid = Double.TryParse((val = tb_Val.Text.Trim()), out result))
							{
								field.DOUBLE = result;

								if (length != val.Length)
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
									++_posCaret;
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

							val = tb_Val.Text.Trim();

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

									if (_prevalCheckboxChecked = isCust)
										result |= 0x01000000;
								}
								else
									_prevalCheckboxChecked = cb_Checker.Checked = false;

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

								val = (rt_Val.Text = new string(val2)); // freshen the richtextbox

								++_posCaret;
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

							locale.local = (val = rt_Val.Text);

							field.label = GffData.Locale.GetLanguageString(locale.langid);
							if (locale.F = _prevalCheckboxChecked = cb_Checker.Checked)
								field.label += SUF_F;

							break;
						}
					}
				}

				if (valid)
				{
					if (field != null)
						node.Text = ConstructNodetext(field, locale);

					_prevalText = val;
				}
				else
					baddog("That dog don't hunt.");
			}
		}
		#endregion Handlers (panel2)


		#region Methods
		/// <summary>
		/// Roll yer own keyboard-navigation keys checker.
		/// @note 'keycode' shall include modifers - ie, pass in KeyCode not
		/// KeyData.
		/// </summary>
		/// <param name="keycode"></param>
		/// <returns></returns>
		bool isNavigation(Keys keycode)
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
		bool isPrintableAscii(string text)
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
		bool isHexadecimal(string text)
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
		#endregion Methods


		#region Methods (static)
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
