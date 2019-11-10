using System;
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
			AllowDrop     = true;

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
				_f.SelectField(); // revert the editpanel


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


					if (SelectedNode.Tag == null) // is TopLevelStruct
					{
						if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
						else toggle = String.Empty;

						ContextMenu.MenuItems.Add(new MenuItem("edit GFF Type", contextclick_EditGffType));
					}
					else // is NOT TopLevelStruct
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

								ContextMenu.MenuItems.Add(new MenuItem("edit LanguageId", contextclick_EditLocale));
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
			field.label = GetUniqueLabel();
			field.BYTE = 0;

			AddField(field);
		}

		void contextclick_AddChar(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CHAR;
			field.label = GetUniqueLabel();
			field.CHAR = 0;

			AddField(field);
		}

		void contextclick_AddWord(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = GetUniqueLabel();
			field.WORD = 0;

			AddField(field);
		}

		void contextclick_AddShort(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.SHORT;
			field.label = GetUniqueLabel();
			field.SHORT = 0;

			AddField(field);
		}

		void contextclick_AddDword(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DWORD;
			field.label = GetUniqueLabel();
			field.DWORD = 0;

			AddField(field);
		}

		void contextclick_AddInt(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.INT;
			field.label = GetUniqueLabel();
			field.INT = 0;

			AddField(field);
		}

		void contextclick_AddDword64(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DWORD64;
			field.label = GetUniqueLabel();
			field.DWORD64 = 0;

			AddField(field);
		}

		void contextclick_AddInt64(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.INT64;
			field.label = GetUniqueLabel();
			field.INT64 = 0;

			AddField(field);
		}

		void contextclick_AddFloat(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.FLOAT;
			field.label = GetUniqueLabel();
			field.FLOAT = 0;

			AddField(field);
		}

		void contextclick_AddDouble(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.DOUBLE;
			field.label = GetUniqueLabel();
			field.DOUBLE = 0;

			AddField(field);
		}

		void contextclick_AddCResRef(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CResRef;
			field.label = GetUniqueLabel();
			field.CResRef = String.Empty;

			AddField(field);
		}

		void contextclick_AddCExoString(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CExoString;
			field.label = GetUniqueLabel();
			field.CExoString = String.Empty;

			AddField(field);
		}

		void contextclick_AddCExoLocString(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.CExoLocString;
			field.label = GetUniqueLabel();
			field.CExoLocStrref = UInt32.MaxValue;

			AddField(field);
		}

		void contextclick_AddVoid(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.VOID;
			field.label = GetUniqueLabel();
			field.VOID = new byte[0];

			AddField(field);
		}

		void contextclick_AddList(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.List;
			field.label = GetUniqueLabel();
//			field.List = new List<uint>();

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
				field.label = GetUniqueLabel();

			field.Struct = new Struct();
			field.Struct.typeid = 0;

			AddField(field);
		}

		/// <summary>
		/// cf. GeneralGFF.fileclick_Create()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddTopLevelStruct(object sender, EventArgs e)
		{
			var field = new GffData.Field();
			field.type = FieldTypes.Struct;

			if (_f.GffData != null)
				field.label = Path.GetFileNameWithoutExtension(_f.GffData.Pfe).ToUpper();
			else
			{
				_f.GffData = new GffData(); // init GffData! ->
				_f.GffData.TypeVer = "GFF V3.2";
				_f.GffData.Type = GffType.generic;

				field.label = Globals.TopLevelStruct;
			}

			field.Struct = new Struct();
			field.Struct.typeid = UInt32.MaxValue;

			Nodes.Add(field.label);

			SelectedNode = Nodes[0];
		}


		internal static Languages _langid;
		internal static bool      _langf;

		/// <summary>
		/// Adds a Locale to a CExoLocString Field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddLocale(object sender, EventArgs e)
		{
			var field = (GffData.Field)SelectedNode.Tag;

			if (field.localeflags != LocaleDialog.Loc_ALL)
			{
				using (var f = new LocaleDialog(field.localeflags))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						var locale = new GffData.Locale();
						locale.local = String.Empty;

						LocaleDialog.SetLocaleFlag(ref field.localeflags,
												   locale.langid = _langid,
												   locale.F = _langf);

						if (field.Locales == null)
							field.Locales = new List<GffData.Locale>();

						var fieldloc = new GffData.Field();
						fieldloc.type = FieldTypes.locale;
						fieldloc.label = GffData.Locale.GetLanguageString(_langid, _langf);
						fieldloc.localeid = (uint)field.Locales.Count;

						field.Locales.Add(locale);
						AddField(fieldloc, locale);
					}
				}
			}
			else
			{
				using (var f = new InfoDialog(Globals.Error, "All locales are taken."))
					f.ShowDialog(this);
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
		internal void contextclick_Delete(object sender, EventArgs e)
		{
			bool delete;

			if (SelectedNode.Tag == null) // is TopLevelStruct's node
			{
				delete = false;
				using (var f = new DeleteDialog(this, "Confirm delete TopLevelStruct"))
				{
					f.cb_Bypass.Visible = false;
					if (f.ShowDialog(this) == DialogResult.Yes)
					{
						delete = true;

						if (_f.GffData.Pfe == Globals.TopLevelStruct)
							_f.GffData = null;
					}
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
						case FieldTypes.Struct:
						{
							// Structs in Lists do not have a Label so keep their pseudo-labels' sequential order

							var parent = SelectedNode.Parent;
							if (parent.Tag != null // parent is NOT TopLevelStruct
								&& ((GffData.Field)parent.Tag).type == FieldTypes.List)
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
							var parent = (GffData.Field)SelectedNode.Parent.Tag;
							var locales = parent.Locales;

							int localeid = (int)((GffData.Field)SelectedNode.Tag).localeid;
							GffData.Locale locale = locales[localeid];

							LocaleDialog.ClearLocaleFlag(ref parent.localeflags,
														 locale.langid,
														 locale.F);

							for (int i = 0; i != SelectedNode.Parent.Nodes.Count; ++i)
							{
								var field = (GffData.Field)SelectedNode.Parent.Nodes[i].Tag;
								if (field.localeid > localeid)
									--field.localeid;
							}

							locales.Remove(locale);
							break;
						}
					}
				}

				SelectedNode.Remove();

				if (SelectedNode == null)
					_f.ResetEditPanel();

				if (_f.GffData != null) _f.GffData.Changed = true;
				_f.GffData = _f.GffData;
			}
		}


		internal static GffType _typeid;

		/// <summary>
		/// Opens a dialog to choose a standard GFF-type.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_EditGffType(object sender, EventArgs e)
		{
			_typeid = _f.GffData.Type;

			using (var f = new TypeDialog())
			{
				if (f.ShowDialog(this) == DialogResult.OK
					&& _f.GffData.Type != _typeid)
				{
					_f.GffData.Type = _typeid;
					_f.GffData.TypeVer = GffData.GetGffString(_typeid) + Globals.SupportedVersion;

					_f.tb_Val.Text = _f.GffData.TypeVer;

					_f.GffData.Changed = true;
					_f.GffData = _f.GffData;

					_f.DirtyState = GeneralGFF.DIRTY_non;
				}
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
						bool abort = false;

						var parent = (GffData.Field)SelectedNode.Parent.Tag;
						if (parent == null // ie. 'parent' is TopLevelStruct
							|| parent.type == FieldTypes.Struct)
						{
							for (int i = 0; i != SelectedNode.Parent.Nodes.Count; ++i)
							{
								if (SelectedNode.Parent.Nodes[i] != SelectedNode
									&& ((Sortable)SelectedNode.Parent.Nodes[i])._label == f.tb_Label.Text)
								{
									using (var g = new InfoDialog(Globals.Error, "Duplicate labels detected."))
										g.ShowDialog(this);

									abort = true;
									break;
								}
							}
						}

						if (!abort)
						{
							field.label = (((Sortable)SelectedNode)._label = f.tb_Label.Text);
							SelectedNode.Text = GeneralGFF.ConstructNodetext(field);

							_f.GffData.Changed = true;
							_f.GffData = _f.GffData;
						}
					}
				}
			}
		}

		/// <summary>
		/// Opens a dialog to edit a Locale's languageid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_EditLocale(object sender, EventArgs e)
		{
			var parent = (GffData.Field)SelectedNode.Parent.Tag;
			if (parent.localeflags != LocaleDialog.Loc_ALL)
			{
				var field  = (GffData.Field)SelectedNode.Tag;

				GffData.Locale locale;

				locale = parent.Locales[(int)field.localeid];
				_langid = locale.langid;
				_langf  = locale.F;

				using (var f = new LocaleDialog(parent.localeflags, true))
				{
					if (f.ShowDialog(this) == DialogResult.OK
						&& (locale.langid != _langid || locale.F != _langf))
					{
						LocaleDialog.ClearLocaleFlag(ref parent.localeflags,
													 locale.langid,
													 locale.F);

						field.label = GffData.Locale.GetLanguageString(locale.langid = _langid,
																	   locale.F = _langf);
						((Sortable)SelectedNode)._label = field.label;

						LocaleDialog.SetLocaleFlag(ref parent.localeflags,
												   _langid,
												   _langf);

						SelectedNode.Text = GeneralGFF.ConstructNodetext(field, locale);

						_f.GffData.Changed = true;
						_f.GffData = _f.GffData;
					}
				}
			}
			else
			{
				using (var f = new InfoDialog(Globals.Error, "All locales are taken."))
					f.ShowDialog(this);
			}
		}


		/// <summary>
		/// Handles clicking Expand/Collapse on the context.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_Toggle(object sender, EventArgs e)
		{
			if (SelectedNode.IsExpanded)
				SelectedNode.Collapse(true);
			else
				SelectedNode.Expand();
		}
		#endregion Context


		#region Handlers (override)
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
			if (SelectedNode != null)
				contextclick_Toggle(null, EventArgs.Empty);
		}


		/// <summary>
		/// Populates the editpanel after a treenode is selected.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			_f.SelectField();
		}


		/// <summary>
		/// Changes the icon when a file is dragged over this TreeList.
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
				drgevent.Effect = DragDropEffects.Copy;
			else
				drgevent.Effect = DragDropEffects.None;
		}

		/// <summary>
		/// Drops a file onto this TreeList.
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			var files = (string[])drgevent.Data.GetData(DataFormats.FileDrop);
			string file = files[0];

			if (File.Exists(file)) // ie. not a directory
			{
				_f.TopMost = true;
				_f.TopMost = false;

				if (_f.CheckCloseData(Globals.Close))
					GffLoader.LoadGFFfile(_f, file);
			}
		}
		#endregion Handlers (override)


		#region Methods
		const string LABEL = "label";

		/// <summary>
		/// Gets a generic label that's not a duplicate of an already existing
		/// label within a current Struct.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		internal string GetUniqueLabel(string label = LABEL)
		{
			var field = (GffData.Field)SelectedNode.Tag;
			if (field == null // is TopLevelStruct
				|| field.type == FieldTypes.Struct)
			{
				string label0 = label;
				int suf = -1;

				bool valid = false;
				while (!valid)
				{
					valid = true;

					for (int i = 0; i != SelectedNode.Nodes.Count; ++i)
					{
						if (((Sortable)SelectedNode.Nodes[i])._label == label)
						{
							label = label0 + (++suf);
							if (label.Length > Globals.Length_LABEL)
								label = label.Substring(label.Length - Globals.Length_LABEL);

							valid = false;
							break;
						}
					}
				}
			}
			return label;
		}
		#endregion Methods
	}
}
