using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace GeneralGFF
{
	/// <summary>
	/// 
	/// </summary>
	partial class GeneralGFF
		:
			Form
	{
		#region Fields (static)
		const string TITLE = "GeneralGFF";

		const int LENGTH_LABEL = 17;
		const int LENGTH_TYPE  = 17;

		internal const string SUF_F = "[F]";
		#endregion Fields (static)


		#region Fields
		internal TreeList _tl;
		#endregion Fields


		#region Properties
		GffData _data;
		/// <summary>
		/// The currently loaded GFF data.
		/// </summary>
		internal GffData Data
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

			Menu.MenuItems[0].MenuItems.Add("&Open GFF file ...");	// #0
			Menu.MenuItems[0].MenuItems[0].Click += fileclick_Open;

			Menu.MenuItems[0].MenuItems.Add("&Save GFF file ...");	// #1
			Menu.MenuItems[0].MenuItems[1].Click += fileclick_Save;

			Menu.MenuItems[0].MenuItems.Add("-");					// #2

			Menu.MenuItems[0].MenuItems.Add("&Quit");				// #3
			Menu.MenuItems[0].MenuItems[3].Click += fileclick_Quit;


			Menu.MenuItems.Add("&Edit"); // #1

			Menu.MenuItems[1].MenuItems.Add("&Expand selected");	// #0
			Menu.MenuItems[1].MenuItems[0].Click += editclick_ExpandSelected;

			Menu.MenuItems[1].MenuItems.Add("&Collapse selected");	// #1
			Menu.MenuItems[1].MenuItems[1].Click += editclick_CollapseSelected;


			Menu.MenuItems.Add("&Help"); // #2

//			Menu.MenuItems[2].MenuItems.Add("&Help");	// #
//			Menu.MenuItems[2].MenuItems[0].Click += ;

			Menu.MenuItems[2].MenuItems.Add("&About");	// #0
			Menu.MenuItems[2].MenuItems[0].Click += helpclick_About;

			_tl = new TreeList(this);
			sc_body.Panel1.Controls.Add(_tl);
			_tl.Select();


			MinimumSize = new Size(sc_body.Panel1MinSize + sc_body.SplitterWidth
														 + Width - ClientSize.Width, // <- border
								   150);

//			LoadUtcfile(@"C:\Users\User\Documents\Neverwinter Nights 2\override\creature1_test.UTC");


			var t1 = new Timer(); // workaround that bypasses TextChanged ...
			t1.Tick += OnTick;
			t1.Interval = 100;
			t1.Start();
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Loads a GFF file.
		/// </summary>
		/// <param name="pfe"></param>
		void LoadGFFfile(string pfe)
		{
			_tl.BeginUpdate();
			_tl.Nodes.Clear();

			GffData.allStructs.Clear();

			Data = GffReader.ReadGFFfile(pfe);
			if (Data != null && GffData.allStructs.Count != 0)
			{
				// Load the TopLevelStruct - all else follows ->
				// NOTE: The TLS has no Field ... thus the rootnode of the tree
				// has no Tag.

				string label = Path.GetFileNameWithoutExtension(Data._pfe).ToUpper();
				TreeNode root = _tl.Nodes.Add(label); // NOTE: TreeView doesn't like the root to be a Sortable. or bleh

				//logfile.Log("");
				//logfile.Log(label);

				List<GffData.Field> fields = Data._fields;

				// instantiate the TLS's fieldids as treenodes ->
				List<uint> fieldids = GffData.allStructs[0].fieldids;
				for (int i = 0; i != fieldids.Count; ++i)
				{
					AddField(fields[(int)fieldids[i]], root);
				}

				_tl.Nodes[0].Expand();
				_tl.SelectedNode = _tl.Nodes[0];
			}

			_tl.EndUpdate();
		}

		/// <summary>
		/// Adds a Field to a treenode.
		/// </summary>
		/// <param name="field">a Field to add</param>
		/// <param name="node">a treenode to add it to</param>
		/// <param name="locale">a locale if applicable</param>
		internal void AddField(GffData.Field field, TreeNode node, GffData.Locale locale = null)
		{
			string text = ConstructNodeText(field, locale);
			//logfile.Log(text);

			var node_ = new Sortable(text, field.label);
			node_.Tag = field;
			node.Nodes.Add(node_);

			switch (field.type)
			{
				case FieldTypes.Struct: // childs can be of any Type.
				{
					List<GffData.Field> fields = Data._fields;

					List<uint> fieldids = field.Struct.fieldids;
					for (int i = 0; i != fieldids.Count; ++i)
					{
						AddField(fields[(int)fieldids[i]], node_);
					}
					break;
				}

				case FieldTypes.List: // childs are Structs.
				{
					List<Struct> allStructs = GffData.allStructs;

					List<uint> list = field.List;
					for (int i = 0; i != list.Count; ++i)
					{
						var field_ = new GffData.Field();

						field_.label  = i.ToString();		// NOTE: Structs in Lists do not have a Label inside a GFF-file.
						field_.type   = FieldTypes.Struct;	// so give Structs in Lists a pseudo-Label for their treenode(s)
						field_.Struct = allStructs[(int)list[i]];

						AddField(field_, node_, null);
					}
					break;
				}

				case FieldTypes.CExoLocString: // childs are Locales.
					if (field.Locales != null)
					{
						int locales = field.Locales.Count;
						for (int i = 0; i != locales; ++i)
						{
							locale = field.Locales[i];

							var field_ = new GffData.Field();
							field_.localeid = (uint)i;
							field_.label = GffData.Locale.GetLanguageString(locale.langid);
							if (locale.F)
								field_.label += SUF_F;

							field_.type = FieldTypes.locale;

							AddField(field_, node_, locale);
						}
					}
					break;
			}
		}

		/// <summary>
		/// Constructs a string of text for display on a treenode.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="locale"></param>
		/// <returns></returns>
		internal string ConstructNodeText(GffData.Field field, GffData.Locale locale = null)
		{
			string label = field.label; // 16 char limit (GFF specification)
			while (label.Length != LENGTH_LABEL)
				label += " ";

			bool token = locale != null
					  && locale.langid == Languages.GffToken;

			string label2 = " [" + GffReader.GetTypeString(field.type, token) + "]";
			while (label2.Length != LENGTH_TYPE)
				label2 += " ";

			label += label2;

			switch (field.type)
			{
				default:
					return label + "= " + GffData.GetValueString(field);

				case FieldTypes.locale:
					return label + "= " + locale.local;

				case FieldTypes.List:
					return label;
			}
		}
		#endregion Methods


		#region Handlers
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
					LoadGFFfile(ofd.FileName);
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

				sfd.InitialDirectory = Path.GetDirectoryName(Data._pfe);
				sfd.FileName         = Path.GetFileName(Data._pfe);

				if (sfd.ShowDialog(this) == DialogResult.OK)
				{
					GffWriter.WriteGFFfile(sfd.FileName, _tl, Data.Ver);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void fileclick_Quit(object sender, EventArgs e)
		{
			Close();
		}


		/// <summary>
		/// Expands the currently selected treenode and all childs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void editclick_ExpandSelected(object sender, EventArgs e)
		{
			_tl.SelectedNode.Expand();
			ExpandChildren(_tl.SelectedNode);

			if (!_tl.SelectedNode.IsVisible)
				_tl.TopNode = _tl.SelectedNode;
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
			_tl.SelectedNode.Collapse();
			CollapseChildren(_tl.SelectedNode);
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



		internal string _preval = String.Empty;
		internal bool _prevalF;

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
		void textchanged_Valuebox(object sender, EventArgs e)
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
			btn_Revert.Enabled =
			btn_Apply .Enabled = _tl.SelectedNode != null && _tl.SelectedNode.Tag != null
							  && (tb_Val.Text != _preval
							  || (cb_GenderF.Visible && cb_GenderF.Checked != _prevalF));
		}


		/// <summary>
		/// Reselects the current treenode causing the value-panel to repopulate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_Revert(object sender, EventArgs e)
		{
			_tl.click_Select(_tl.SelectedNode);
		}

		/// <summary>
		/// Applies changed data to a field if the valbox is focused and [Enter]
		/// is keydown'd.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void keydown_Val(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				btn_Apply.PerformClick();
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
				string val = tb_Val.Text; // test string ->

				bool valid = false;
				GffData.Locale locale = null;

				var field = (GffData.Field)node.Tag;
				switch (field.type)
				{
					case FieldTypes.CHAR:
					{
//						char result;
//						if (valid = Char.TryParse(val, out result))
						byte result;
						if (valid = Byte.TryParse(val, out result))
							field.CHAR = result;
						break;
					}

					case FieldTypes.BYTE:
					{
						byte result;
						if (valid = Byte.TryParse(val, out result))
							field.BYTE = result;
						break;
					}

					case FieldTypes.WORD:
					{
						ushort result;
						if (valid = UInt16.TryParse(val, out result))
							field.WORD = result;
						break;
					}

					case FieldTypes.SHORT:
					{
						short result;
						if (valid = Int16.TryParse(val, out result))
							field.SHORT = result;
						break;
					}

					case FieldTypes.DWORD:
					{
						uint result;
						if (valid = UInt32.TryParse(val, out result))
							field.DWORD = result;
						break;
					}

					case FieldTypes.INT:
					{
						int result;
						if (valid = Int32.TryParse(val, out result))
							field.INT = result;
						break;
					}

					case FieldTypes.DWORD64:
					{
						ulong result;
						if (valid = UInt64.TryParse(val, out result))
							field.DWORD64 = result;
						break;
					}

					case FieldTypes.INT64:
					{
						long result;
						if (valid = Int64.TryParse(val, out result))
							field.INT64 = result;
						break;
					}

					case FieldTypes.FLOAT:
					{
						float result;
						if (valid = float.TryParse(val, out result))
							field.FLOAT = result;
						break;
					}

					case FieldTypes.DOUBLE:
					{
						double result;
						if (valid = Double.TryParse(val, out result))
							field.DOUBLE = result;
						break;
					}

					case FieldTypes.CResRef:
					{
						if (valid = val.Length < 33)	// nwn2-style resrefs
							field.CResRef = val;		// NOTE: The GFF-specification allows CResRef to be 255 bytes in length.
						break;
					}

					case FieldTypes.CExoString:
					{
						valid = true;
						field.CExoString = val;
						break;
					}

					case FieldTypes.CExoLocString:
					{
						int result;
						if (valid = Int32.TryParse(val, out result)
							&& result > -2 && result < 16777216)	// NOTE: The GFF-specification stores strrefs as Uint32.
						{											// TODO: Support for CUSTOM.Tlk talktables!
							field.CExoLocStrref = (uint)result;
						}
						break;
					}

					case FieldTypes.VOID: // not editable
					{
						// test for hexadecimal digits
						valid = false;
						break;
					}

					case FieldTypes.Struct: // not editable
					{
						// test for "[" + UInt32 + "]" (StructId)
						valid = false;
						break;
					}

					case FieldTypes.List: // not editable
					{
						valid = false;
						break;
					}

					case FieldTypes.locale:
					{
						valid = true;

						var CExoLocString = (GffData.Field)node.Parent.Tag;
						locale = CExoLocString.Locales[(int)field.localeid];

						locale.local = val;

						field.label = GffData.Locale.GetLanguageString(locale.langid);
						if (locale.F = _prevalF = cb_GenderF.Checked)
							field.label += SUF_F;

						break;
					}
				}

				if (valid)
				{
					node.Text = ConstructNodeText(field, locale);
					_preval = val;
				}
				else
					MessageBox.Show(
								this,
								"That dog don't hunt.",
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
			}
		}
		#endregion Handlers
	}
}
