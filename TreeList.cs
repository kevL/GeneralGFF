using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;


namespace generalgff
{
	sealed class TreeList
		:
			TreeView
	{
		#region Delegates
		/// <summary>
		/// Good fuckin Lord I just wrote a "DontBeep" delegate.
		/// </summary>
		internal delegate void DontBeepEventHandler();
		#endregion Delegates


		#region Events
		internal event DontBeepEventHandler DontBeepEvent;
		#endregion Events


		#region Fields
		/// <summary>
		/// This list's top-level F.
		/// </summary>
		readonly GeneralGFF _f;
		#endregion Fields


		#region Properties (override)
		/// <summary>
		/// Prevents flicker.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal TreeList(GeneralGFF f)
		{
			_f = f;

			BackColor     = Color.PaleTurquoise;
			Dock          = DockStyle.Fill;
			Width         = 500;
			HideSelection = false;
			BorderStyle   = BorderStyle.None;
			Indent        = 12;

			TreeViewNodeSorter = new NodeSorter();

			DontBeepEvent += Toggle;


			// NOTE: ContextMenuStrip fails to invoke on first RMB-click
			// that's why ContextMenu (because it works as advertised).
			ContextMenu = new ContextMenu();
			ContextMenu.Popup += contextpop;
		}
		#endregion cTor


		#region Context
		void contextpop(object sender, EventArgs e)
		{
			ContextMenu.MenuItems.Clear();

			if (SelectedNode != null)
				SelectField(SelectedNode); // revert the editpanel

			TreeViewHitTestInfo info = HitTest(PointToClient(Cursor.Position)); // NOTE: That is fullrow.
			if (info != null)
			{
				TreeNode node = info.Node;
				if (node != null)
				{
					SelectedNode = node;

					string toggle = null;
					if (SelectedNode.Nodes.Count != 0)
					{
						if (SelectedNode.IsExpanded)
							toggle = "Collapse";
						else
							toggle = "Expand";

						ContextMenu.MenuItems.Add(new MenuItem(toggle, contextclick_Toggle));
					}

					FieldTypes type;

					if (SelectedNode.Tag == null // is TopLevelStruct's node
						|| (type = ((GffData.Field)SelectedNode.Tag).type) == FieldTypes.Struct)
					{
						if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
						else toggle = String.Empty;

						ContextMenu.MenuItems.Add(new MenuItem("add BYTE (1-byte ubyte)",           contextclick_AddByte));
						ContextMenu.MenuItems.Add(new MenuItem("add CHAR (1-byte byte)",            contextclick_AddChar));
						ContextMenu.MenuItems.Add(new MenuItem("add WORD (2-byte ushort)",          contextclick_AddWord));
						ContextMenu.MenuItems.Add(new MenuItem("add SHORT (2-byte short)",          contextclick_AddShort));
						ContextMenu.MenuItems.Add(new MenuItem("add DWORD (4-byte uint)",           contextclick_AddDword));
						ContextMenu.MenuItems.Add(new MenuItem("add INT (4-byte int)",              contextclick_AddInt));
						ContextMenu.MenuItems.Add(new MenuItem("add DWORD64 (8-byte ulong)",        contextclick_AddDword64));
						ContextMenu.MenuItems.Add(new MenuItem("add INT64 (8-byte long)",           contextclick_AddInt64));
						ContextMenu.MenuItems.Add(new MenuItem("add FLOAT (4-byte float)",          contextclick_AddFloat));
						ContextMenu.MenuItems.Add(new MenuItem("add DOUBLE (8-byte double)",        contextclick_AddDouble));
						ContextMenu.MenuItems.Add(new MenuItem("add CResRef (32-chars ASCII)",      contextclick_AddCResRef));
						ContextMenu.MenuItems.Add(new MenuItem("add CExoString (ASCII)",            contextclick_AddCExoString));
						ContextMenu.MenuItems.Add(new MenuItem("add CExoLocString (24-bit strref)", contextclick_AddCExoLocString));
						ContextMenu.MenuItems.Add(new MenuItem("add VOID (raw byte data)",          contextclick_AddVoid));
						ContextMenu.MenuItems.Add(new MenuItem("add List (list of structs)",        contextclick_AddList));
						ContextMenu.MenuItems.Add(new MenuItem("add Struct (list of fields)",       contextclick_AddStruct));
					}
					else
					{
						switch (type)
						{
							case FieldTypes.List:
								if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
								else toggle = String.Empty;

								ContextMenu.MenuItems.Add(new MenuItem("add Struct (list of fields)", contextclick_AddStruct));
								break;

							case FieldTypes.CExoLocString:
								if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
								else toggle = String.Empty;

								ContextMenu.MenuItems.Add(new MenuItem("add Locale (localized UTF8)", contextclick_AddLocale));
								break;
						}
					}


					if (SelectedNode.Tag != null) // is not TopLevelStruct
					{
						switch (((GffData.Field)SelectedNode.Tag).type)
						{
							case FieldTypes.Struct:
								if (SelectedNode.Parent.Tag != null
									&& ((GffData.Field)SelectedNode.Parent.Tag).type == FieldTypes.List)
								{
									break;
								}
								goto case FieldTypes.BYTE;

							case FieldTypes.BYTE:
							case FieldTypes.CHAR:
							case FieldTypes.WORD:
							case FieldTypes.SHORT:
							case FieldTypes.DWORD:
							case FieldTypes.INT:
							case FieldTypes.DWORD64:
							case FieldTypes.INT64:
							case FieldTypes.FLOAT:
							case FieldTypes.DOUBLE:
							case FieldTypes.CResRef:
							case FieldTypes.CExoString:
							case FieldTypes.CExoLocString:
							case FieldTypes.VOID:
							case FieldTypes.List:
								if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
								else toggle = String.Empty;

								ContextMenu.MenuItems.Add(new MenuItem("edit Label", contextclick_EditLabel));
								break;

							case FieldTypes.locale:
								if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
								else toggle = String.Empty;

								ContextMenu.MenuItems.Add(new MenuItem("edit LanguageId", contextclick_EditLanguageId));
								break;
						}
					}

					if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
					ContextMenu.MenuItems.Add(new MenuItem("DELETE", contextclick_Delete));
				}
				else if (Nodes.Count == 0) // is blank GFF - req'd.
				{
					ContextMenu.MenuItems.Add(new MenuItem("add TopLevelStruct", contextclick_AddTopLevelStruct));
				}
			}
		}

		void contextclick_AddByte(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.BYTE;
			field.label = "label";
			field.BYTE = 0;

			AddField(field);
		}

		void contextclick_AddChar(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CHAR;
			field.label = "label";
			field.CHAR = 0;

			AddField(field);
		}

		void contextclick_AddWord(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = "label";
			field.WORD = 0;

			AddField(field);
		}

		void contextclick_AddShort(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.SHORT;
			field.label = "label";
			field.SHORT = 0;

			AddField(field);
		}

		void contextclick_AddDword(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DWORD;
			field.label = "label";
			field.DWORD = 0;

			AddField(field);
		}

		void contextclick_AddInt(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.INT;
			field.label = "label";
			field.INT = 0;

			AddField(field);
		}

		void contextclick_AddDword64(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DWORD64;
			field.label = "label";
			field.DWORD64 = 0;

			AddField(field);
		}

		void contextclick_AddInt64(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.INT64;
			field.label = "label";
			field.INT64 = 0;

			AddField(field);
		}

		void contextclick_AddFloat(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.FLOAT;
			field.label = "label";
			field.FLOAT = 0;

			AddField(field);
		}

		void contextclick_AddDouble(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DOUBLE;
			field.label = "label";
			field.DOUBLE = 0;

			AddField(field);
		}

		void contextclick_AddCResRef(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CResRef;
			field.label = "label";
			field.CResRef = String.Empty;

			AddField(field);
		}

		void contextclick_AddCExoString(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CExoString;
			field.label = "label";
			field.CExoString = String.Empty;

			AddField(field);
		}

		void contextclick_AddCExoLocString(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CExoLocString;
			field.label = "label";
			field.CExoLocStrref = UInt32.MaxValue;

			AddField(field);
		}

		void contextclick_AddVoid(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.VOID;
			field.label = "label";
			field.VOID = new byte[0];

			AddField(field);
		}

		void contextclick_AddList(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.List;
			field.label = "label";
			field.List = new List<uint>();

			AddField(field);
		}

		void contextclick_AddStruct(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.Struct;

			if (SelectedNode.Tag != null
				&& ((GffData.Field)SelectedNode.Tag).type == FieldTypes.List)
			{
				field.label = SelectedNode.Nodes.Count.ToString();
			}
			else
				field.label = "label";

			field.Struct = new Struct();
			field.Struct.typeid = 0;

			AddField(field);
		}

		void contextclick_AddTopLevelStruct(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.Struct;

			if (_f.GffData != null)
				field.label = Path.GetFileNameWithoutExtension(_f.GffData.Pfe).ToUpper();
			else
			{
				_f.GffData = new GffData(); // init GffData! ->
				_f.GffData.Ver = "GFF V3.2";
				_f.GffData.Type = GffType.generic;

				field.label = Globals.TopLevelStruct;
			}

			field.Struct = new Struct();
			field.Struct.typeid = UInt32.MaxValue;

			Nodes.Add(field.label);

			SelectedNode = Nodes[0];
		}


		internal static Languages _langid;

		/// <summary>
		/// Adds a Locale to a CExoLocString Field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddLocale(object sender, EventArgs e)
		{
			using (var f = new LocaleDialog())
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					var locale = new GffData.Locale();
					locale.langid = _langid;
					locale.local = String.Empty;
					locale.F = false;

					if (((GffData.Field)SelectedNode.Tag).Locales == null)
						((GffData.Field)SelectedNode.Tag).Locales = new List<GffData.Locale>();

					((GffData.Field)SelectedNode.Tag).Locales.Add(locale);

					var field = new GffData.Field();
					field.type = FieldTypes.locale;
					field.label = GffData.Locale.GetLanguageString(locale.langid);
					field.localeid = (uint)SelectedNode.Nodes.Count;

					AddField(field, locale);
				}
			}
		}


		/// <summary>
		/// Adds a field to the TreeList and selects it.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="locale"></param>
		void AddField(GffData.Field field, GffData.Locale locale = null)
		{
			string text = GeneralGFF.ConstructNodetext(field, locale);
			var node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes.Add(node);

			SelectedNode = node;

			_f.GffData.Changed = true;
			_f.GffData = _f.GffData;
		}


		/// <summary>
		/// User-option to bypass delete-warnings for the session.
		/// </summary>
		static bool _bypassDeleteWarning;

		/// <summary>
		/// Deletes a field in the TreeList along with any of its subnodes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_Delete(object sender, EventArgs e)
		{
			bool delete;

			if (SelectedNode.Tag == null) // is TopLevelStruct's node
			{
				delete = false;
				using (var f = new DeleteDialog(this, "Confirm delete TopLevelStruct"))
				{
					f.cb_Bypass.Visible = false;
					delete = (f.ShowDialog(this) == DialogResult.Yes);
				}
			}
			else if (!_bypassDeleteWarning)
			{
				string head = "Confirm delete";
				if (SelectedNode.Nodes.Count != 0)
					head += " multiple fields";

				delete = false;
				using (var f = new DeleteDialog(this, head))
				{
					f.cb_Bypass.Visible = true;

					if (f.ShowDialog(this) == DialogResult.Yes)
					{
						delete = true;
						_bypassDeleteWarning = f.cb_Bypass.Checked;
					}
				}
			}
			else
				delete = true;

			if (delete)
			{
				if (SelectedNode.Tag != null)
				{
					switch (((GffData.Field)SelectedNode.Tag).type)
					{
						case FieldTypes.Struct:	// Structs in Lists do not have a Label so
						{						// keep the pseudo-labels' sequential order ->
							var parent = SelectedNode.Parent;
							if (((GffData.Field)parent.Tag).type == FieldTypes.List)
							{
								Sortable node;

								var field = (GffData.Field)SelectedNode.Tag;
								int id = Int32.Parse(field.label);
								while (++id != parent.Nodes.Count)
								{
									node = parent.Nodes[id] as Sortable;
									field = (GffData.Field)node.Tag;
									node._label =
									field.label = (id - 1).ToString();

									node.Text = GeneralGFF.ConstructNodetext(field);
								}
							}
							break;
						}

						case FieldTypes.locale:
						{
							var parent = SelectedNode.Parent;
							var CExoLocString = (GffData.Field)parent.Tag;

							int localeid = (int)((GffData.Field)SelectedNode.Tag).localeid;
							GffData.Locale locale = CExoLocString.Locales[localeid];

							GffData.Field field;

							var locales = parent.Nodes;
							for (++localeid; localeid != locales.Count; ++localeid)
							{
								field = (GffData.Field)locales[localeid].Tag;
								--field.localeid;
							}

							CExoLocString.Locales.Remove(locale);
							break;
						}
					}
				}

				SelectedNode.Remove();

				if (SelectedNode == null)
					DisableEditPanel();

				_f.GffData.Changed = true;
				_f.GffData = _f.GffData;
			}
		}


		/// <summary>
		/// Opens a dialog to edit a Field's Label.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_EditLabel(object sender, EventArgs e)
		{
			using (var f = new LabelDialog(((GffData.Field)SelectedNode.Tag).label))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					var field = (GffData.Field)SelectedNode.Tag;
					if (field.label != f.tb_Label.Text)
					{
						field.label = f.tb_Label.Text;
						SelectedNode.Text = GeneralGFF.ConstructNodetext(field);

						_f.GffData.Changed = true;
						_f.GffData = _f.GffData;
					}
				}
			}
		}

		/// <summary>
		/// Opens a dialog to edit a Locale's languageid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_EditLanguageId(object sender, EventArgs e)
		{
			using (var f = new LocaleDialog())
			{
				var parent = (GffData.Field)SelectedNode.Parent.Tag;
				var field = ((GffData.Field)SelectedNode.Tag);

				var locale = parent.Locales[(int)field.localeid];
				_langid = locale.langid;

				if (f.ShowDialog(this) == DialogResult.OK)
				{
					locale.langid = _langid;
					field.label = GffData.Locale.GetLanguageString(locale.langid);

					if (_langid == Languages.GffToken)
					{
						locale.F = false;
						SelectField(SelectedNode); // freshen the editpanel (hide Feminine checkbox etc.)
					}
					else if (locale.F)
						field.label += Globals.SUF_F;

					SelectedNode.Text = GeneralGFF.ConstructNodetext(field, locale);
				}
			}
		}


		/// <summary>
		/// Handles clicking Expand/Collapse on the context.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_Toggle(object sender, EventArgs e)
		{
			SelectedNode.Toggle();
		}
		#endregion Context


		#region Handlers (override)
//		protected override void WndProc(ref Message m)
//		{
//			if (m.Msg == 0x203) m.Msg = 0x201; // change WM_LBUTTONDBLCLK to WM_LBUTTONCLICK
//			base.WndProc(ref m);
//		}

		/// <summary>
		/// Handles keydown events.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
					e.SuppressKeyPress = true;
					BeginInvoke(DontBeepEvent);
					break;
			}
		}

		/// <summary>
		/// Subscribes to the DontBeepEvent.
		/// </summary>
		void Toggle()
		{
			SelectedNode.Toggle();
		}


		internal bool BypassChanged;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			BypassChanged = true;
			SelectField(e.Node);
			BypassChanged = false;
		}
		#endregion Handlers (override)


		#region Methods
		bool _RtbEnabled;

		/// <summary>
		/// Disables the controls of panel2.
		/// </summary>
		void DisableEditPanel()
		{
			_f.EnableApply = GeneralGFF.DIRTY_non;

			_f.la_Des.Text =
			_f.la_Val.Text =
			_f.tb_Val.Text =
			_f.rt_Val.Text = String.Empty;

			_f.tb_Val.Enabled   = false;
			_f.tb_Val.BackColor = Color.Thistle;

			if (_RtbEnabled)
			{
				_RtbEnabled = false;

				_f.rt_Val.BackColor = Color.Thistle;
				_f.rt_Val.ReadOnly  = true;
				_f.rt_Val.TabStop   = false;
				_f.rt_Val.Cursor    = Cursors.Default;
				_f.rt_Val.Enter    += _f.enter_Richtextbox;
			}

			_f.cb_Checker.Visible = false;

			_f._prevalText = String.Empty;
			_f._prevalChecker = false;
		}

		/// <summary>
		/// Enables the richtextbox.
		/// </summary>
		void EnableRichtextbox()
		{
			if (!_RtbEnabled)
			{
				_RtbEnabled = true;

				_f.rt_Val.BackColor = Color.Honeydew;
				_f.rt_Val.ReadOnly  = false;
				_f.rt_Val.TabStop   = true;
				_f.rt_Val.Cursor    = Cursors.IBeam;
				_f.rt_Val.Enter    -= _f.enter_Richtextbox;
			}
		}

		/// <summary>
		/// @note Ensure that 'node' is valid before call.
		/// </summary>
		/// <param name="node"></param>
		internal void SelectField(TreeNode node)
		{
			DisableEditPanel();

			if (node.Tag == null) // is TopLevelStruct's node
			{
				_f.la_Des.Text = "ASCII";
				_f.la_Val.Text = "GFF type + version";

				_f.tb_Val.Text = _f.GffData.Ver;

				_f.tb_Val.Enabled   = true;
				_f.tb_Val.BackColor = Color.Violet;

				_f._prevalText = _f.tb_Val.Text;
			}
			else
			{
				var field = (GffData.Field)node.Tag;

				switch (field.type)
				{
					case FieldTypes.BYTE:
						_f.la_Des.Text = Byte.MinValue + ".." + Byte.MaxValue;
						_f.la_Val.Text = "BYTE";

						_f.tb_Val.Text = field.BYTE.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.CHAR:
						_f.la_Des.Text = sbyte.MinValue + ".." + sbyte.MaxValue;
						_f.la_Val.Text = "CHAR";

						_f.tb_Val.Text = field.CHAR.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.WORD:
						_f.la_Des.Text = UInt16.MinValue + ".." + UInt16.MaxValue;
						_f.la_Val.Text = "WORD";

						_f.tb_Val.Text = field.WORD.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.SHORT:
						_f.la_Des.Text = Int16.MinValue + ".." + Int16.MaxValue;
						_f.la_Val.Text = "SHORT";

						_f.tb_Val.Text = field.SHORT.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.DWORD:
						_f.la_Des.Text = UInt32.MinValue + ".." + UInt32.MaxValue;
						_f.la_Val.Text = "DWORD";

						_f.tb_Val.Text = field.DWORD.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.INT:
						_f.la_Des.Text = Int32.MinValue + ".." + Int32.MaxValue;
						_f.la_Val.Text = "INT";

						_f.tb_Val.Text = field.INT.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.DWORD64:
						_f.la_Des.Text = UInt64.MinValue + ".." + UInt64.MaxValue;
						_f.la_Val.Text = "DWORD64";

						_f.tb_Val.Text = field.DWORD64.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.INT64:
						_f.la_Des.Text = Int64.MinValue + ".." + Int64.MaxValue;
						_f.la_Val.Text = "INT64";

						_f.tb_Val.Text = field.INT64.ToString();

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.FLOAT:
					{
						_f.la_Des.Text = float.MinValue + ".." + float.MaxValue;
						_f.la_Val.Text = "FLOAT";

						string f = field.FLOAT.ToString();
						if (!f.Contains(".")) f += ".0";
						_f.tb_Val.Text = f;

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;
					}

					case FieldTypes.DOUBLE:
					{
						_f.la_Des.Text = Double.MinValue + ".." + Double.MaxValue;
						_f.la_Val.Text = "DOUBLE";

						string d = field.DOUBLE.ToString();
						if (!d.Contains(".")) d += ".0";
						_f.tb_Val.Text = d;

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;
					}

					case FieldTypes.CResRef:
						_f.la_Des.Text = "32-chars NwN2 / 16-chars NwN1" + Environment.NewLine + "ASCII lc";
						_f.la_Val.Text = "CResRef";

						_f.tb_Val.Text = field.CResRef;

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.CExoString:
						_f.la_Des.Text = "ASCII";
						_f.la_Val.Text = "CExoString";

						_f.rt_Val.Text = field.CExoString;

						EnableRichtextbox();

						_f._prevalText = _f.rt_Val.Text;
						break;

					case FieldTypes.CExoLocString:
					{
						_f.la_Des.Text = "strref" + Environment.NewLine + "-1.." + 0x00FFFFFF;
						_f.la_Val.Text = "CExoLocString";

						uint strref = field.CExoLocStrref;
						if (strref != UInt32.MaxValue)
							_f.tb_Val.Text = (strref & 0x00FFFFFF).ToString();
						else
							_f.tb_Val.Text = "-1";

						bool @checked =  strref != UInt32.MaxValue
									 && (strref & 0x01000000) != 0;
						_f.cb_Checker.Checked = _f._prevalChecker = @checked;

						_f.cb_Checker.Text = "Custom talktable";
						_f.cb_Checker.Visible = true;

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;
					}

					case FieldTypes.VOID:
					{
						_f.la_Des.Text = "binary data";
						_f.la_Val.Text = "VOID";

						_f.rt_Val.Text = BitConverter.ToString(field.VOID).Replace("-", " ");

						EnableRichtextbox();

						_f._prevalText = _f.rt_Val.Text.ToUpper(CultureInfo.InvariantCulture);
						break;
					}

					case FieldTypes.List:
						_f.la_Val.Text = "List";
						break;

					case FieldTypes.Struct:
						_f.la_Des.Text = "TypeId" + Environment.NewLine + UInt32.MinValue + ".." + UInt32.MaxValue;
						_f.la_Val.Text = "Struct";

						_f.tb_Val.Text = "[" + field.Struct.typeid + "]";

						_f.tb_Val.Enabled   = true;
						_f.tb_Val.BackColor = Color.Honeydew;

						_f._prevalText = _f.tb_Val.Text;
						break;

					case FieldTypes.locale:
					{
						var parent = (GffData.Field)node.Parent.Tag;
						GffData.Locale locale = parent.Locales[(int)field.localeid];

						if (locale.langid == Languages.GffToken)
						{
							_f.la_Des.Text = "UTF8";
							_f.la_Val.Text = "GffToken";
						}
						else
						{
							_f.la_Des.Text = "UTF8 localized";
							_f.la_Val.Text = "locale";

							_f.cb_Checker.Checked = _f._prevalChecker = locale.F;
							_f.cb_Checker.Text = "Feminine";
							_f.cb_Checker.Visible = true;
						}

						_f.rt_Val.Text = locale.local;

						EnableRichtextbox();

						_f._prevalText = _f.rt_Val.Text;
						break;
					}
				}
			}
		}
		#endregion Methods
	}



	/// <summary>
	/// A TreeNode that can be sorted according to a specified label (instead of
	/// its displayed text).
	/// </summary>
	sealed class Sortable
		:
			TreeNode
	{
		internal string _label;

		internal Sortable(string text, string label)
			:
				base(text)
		{
			_label = label;
		}
	}


	/// <summary>
	/// A sorter of Sortable treenodes.
	/// </summary>
	sealed class NodeSorter
		:
			IComparer
	{
		public int Compare(object a, object b)
		{
			var a_ = a as Sortable;
			var b_ = b as Sortable;

			int ai, bi;
			if (   Int32.TryParse(a_._label, out ai)
				&& Int32.TryParse(b_._label, out bi))
			{
				return ai.CompareTo(bi);
			}

			return String.Compare(a_._label,
								  b_._label,
								  StringComparison.OrdinalIgnoreCase);
		}
	}
}
