using System;
using System.Collections;
//using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

		/// <summary>
		/// Prevents a treenode from expanding after selected if true.
		/// </summary>
		bool _bypassExpand;
		#endregion Fields


//		#region Properties
//		internal bool PrintVoidDec
//		{ private get; set; }
//		#endregion Properties


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
			ContextMenu.Popup += contextopening;
		}
		#endregion cTor


		#region Context
		/// <summary>
		/// Opens the ContextMenu for a treenode or serves as an RMB-click on a
		/// treenode (expand/collapse).
		/// @note UTC files are defined in such a way that only one type of
		/// operation will be available on any given treenode and further that a
		/// type of operation will be specific to a node-level (1..4).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextopening(object sender, EventArgs e)
		{
/*			ContextMenu.MenuItems.Clear();

			TreeViewHitTestInfo info = HitTest(PointToClient(Cursor.Position)); // NOTE: That is fullrow.
			if (info != null)
			{
				TreeNode node = info.Node;
				if (node != null)
				{
					// TODO: Add "Cloak" Struct to root ...

					// if (node.Tag != null) ...
					bool autotoggle = true;

					SelectedNode = node;

					MenuItem it = null;
					switch (node.Level)
					{
						case 1:
						{
							var field = (GffData.Field)node.Tag;
							switch (field.type)
							{
								case FieldTypes.List:
									switch (field.label)
									{
										case LABEL_CLASSLIST:
											if (node.Nodes.Count < Max_CLASSES)
												it = new MenuItem("add Class", contextclick_AddClass);
											else
												autotoggle = false;
											break;

										case LABEL_FEATLIST:
											it = new MenuItem("add Feat", contextclick_AddFeat);
											break;

										case LABEL_ITEMLIST:
											it = new MenuItem("add inventory item", contextclick_AddInventoryItem);
											break;

										case LABEL_EQUIPITEMLIST:
											if (node.Nodes.Count < Max_EQUIPPED)
												it = new MenuItem("add equipped item", contextclick_AddEquippedItem);
											else
												autotoggle = false;
											break;

//										case LABEL_TEMPLATELIST: // I have no clue what this is.
//											it = new MenuItem("add template", contextclick_AddTemplate);
//											break;

										case LABEL_VARTABLE:
											it = new MenuItem("add variable", contextclick_AddVariable);
											break;

										// TODO: DmgReduction
									}
									break;

								case FieldTypes.CExoLocString:
//									if (node.Nodes.Count < Max_LOCALES) // too complicated wrt/ gender
									it = new MenuItem("add localized string", contextclick_AddLocale);
//									else
//										autotoggle = false;
									break;
							}
							break;
						}

						case 2:
						{
							var field = (GffData.Field)node.Tag;
							switch (field.type)
							{
								case FieldTypes.Struct: // parent shall be a List here ->
								{
									field = (GffData.Field)(node.Parent.Tag);
									if (field.type == FieldTypes.List)
									{
										switch (field.label)
										{
											case LABEL_CLASSLIST:
												_isInventoryIt = false;
												it = new MenuItem("delete Class", contextclick_DeleteStruct);
												break;

											case LABEL_FEATLIST:
												_isInventoryIt = false;
												it = new MenuItem("delete Feat", contextclick_DeleteStruct);
												break;

											case LABEL_ITEMLIST:
												_isInventoryIt = true;
												it = new MenuItem("delete inventory item", contextclick_DeleteStruct);
												break;

											case LABEL_EQUIPITEMLIST:
												_isInventoryIt = false;
												it = new MenuItem("delete equipped item", contextclick_DeleteStruct);
												break;

//											case LABEL_TEMPLATELIST: // I have no clue what this is.
//												_isInventoryIt = false;
//												it = new MenuItem("delete template", contextclick_DeleteStruct);
//												break;

											case LABEL_VARTABLE:
												_isInventoryIt = false;
												it = new MenuItem("delete variable", contextclick_DeleteStruct);
												break;

											// TODO: DmgReduction
										}
									}
									break;
								}

								case FieldTypes.locale:
									it = new MenuItem("delete localized string", contextclick_DeleteLocale);
									break;
							}
							break;
						}

						case 3:
						{
							if (node.Nodes == null || node.Nodes.Count == 0)
							{
								var field = (GffData.Field)node.Tag;
								string label = field.label;
								if (   (label.Length >  9 && label.Substring(0, 9) == LABEL_PREFIX_KNOWN)
									|| (label.Length > 13 && label.Substring(0,13) == LABEL_PREFIX_MEMORIZED))
								{
									it = new MenuItem("add Spell", contextclick_AddSpell);
								}
							}
							break;
						}

						case 4:
						{
							var field = (GffData.Field)node.Parent.Tag;
							string label = field.label;
							if (   (label.Length >  9 && label.Substring(0, 9) == LABEL_PREFIX_KNOWN)
								|| (label.Length > 13 && label.Substring(0,13) == LABEL_PREFIX_MEMORIZED))
							{
								_isInventoryIt = false;
								it = new MenuItem("delete Spell", contextclick_DeleteStruct);
							}
							break;
						}
					}

					if (it != null || !autotoggle)
					{
						if (it != null)
							ContextMenu.MenuItems.Add(it);

						if (node.Nodes != null && node.Nodes.Count != 0)
						{
							if (it != null)
								ContextMenu.MenuItems.Add("-");

							if (node.IsExpanded)
							{
								ContextMenu.MenuItems.Add("Collapse", contextclick_Toggle);

								bool found1 = false;
								bool found2 = false;
								for (int i = 0; i != node.Nodes.Count; ++i)
								{
									if (node.Nodes[i].IsExpanded)
									{
										if (!found1)
										{
											found1 = true;

											if (node.Nodes[i].Nodes != null && node.Nodes[i].Nodes.Count != 0)
												ContextMenu.MenuItems.Add("Collapse children", contextclick_CollapseChildren);
											else
												break;

											if (found2) break;
										}
									}
									else if (!found2)
									{
										found2 = true;

										if (node.Nodes[i].Nodes != null && node.Nodes[i].Nodes.Count != 0)
											ContextMenu.MenuItems.Add("Expand children", contextclick_ExpandChildren);
										else
											break;

										if (found1) break;
									}
								}
							}
							else
								ContextMenu.MenuItems.Add("Expand", contextclick_Toggle);
						}
					}
					else
						node.Toggle();
				}
			} */
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

		/// <summary>
		/// Handles clicking CollapseChildren on the context.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_CollapseChildren(object sender, EventArgs e)
		{
			foreach (TreeNode node in SelectedNode.Nodes)
				node.Collapse();
		}

		/// <summary>
		/// Handles clicking ExpandChildren on the context.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_ExpandChildren(object sender, EventArgs e)
		{
			foreach (TreeNode node in SelectedNode.Nodes)
				node.Expand();
		}


		/// <summary>
		/// Deletes a deletable Struct.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_DeleteStruct(object sender, EventArgs e)
		{
			var parent = SelectedNode.Parent;

			Sortable node;

			var field = (GffData.Field)SelectedNode.Tag;
			int id = Int32.Parse(field.label) + 1; // NOTE: Structs in Lists do not have a Label.
			for (; id != parent.Nodes.Count; ++id)
			{
				node = parent.Nodes[id] as Sortable;
				field = (GffData.Field)node.Tag;
				node._label =
				field.label = (id - 1).ToString();

				node.Text = _f.ConstructNodeText(field, null);
			}

			_bypassExpand = true;
			SelectedNode.Remove();	// that will select the next node (not the parent
									// node) so _bypassExpand for that as well as for
									// selecting the parent node ->

									// But only if there are other child nodes
									// else the parent node gets selected auto ...

									// I hope I don't end up writing my own TreeView class
									// like I had to do for the so-called DataGridView

			if (SelectedNode != parent)
			{
				_bypassExpand = true;
				SelectedNode = parent;
			}
		}
		// TODO:
		// a) remove the Fields and their FieldIds of current Struct's Lists' Structs' Fields
		// b) remove the Fields and their FieldIds of current Struct's Lists' Structs
		// c) remove the Fields and their FieldIds of current Struct's Fields
		// d) remove the Field  and       FieldId  of current Struct
		//
		// d) remove the fieldid from _f.Data._fieldids
		// d) remove the field   from _f.Data._fields
		//
		// Right now this merely deletes the treenode. The Fields and all
		// their subFields and all those FieldIds are still cached in
		// _f.Data._fields and _f.Data._fieldids respectively.
		//
		// Hence a decision: either,
		// 1. Leave all that data intact (helps for Undo/Redo) since writing
		//    the file won't rely on it anyway; data will be gathered from
		//    the state of the tree.
		// 2. Keep the caches tidy.
		//
		// or perhaps
		// 3. Delete all caches except Fields right after a UTC-file loads.
		//    And construct output-to-file by rifling through the treenodes
		//    using the fact that each node is Tagged with its Field
		//    (except for the TopLevelStruct the state of which is entirely
		//    dependent on the treenodes, which are tagged w/ Fields,
		//    anyway).


/*		/// <summary>
		/// Adds a Struct (a Class structure) to the "ClassList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddClass(object sender, EventArgs e)
		{
			var st = new Struct();
			st.typeid   = 2; // <- that's what's in the UTCs I've looked at.
			st.fieldids = new List<uint>();

			int id = SelectedNode.Nodes.Count;

			var fieldst = new GffData.Field();
			fieldst.label  = id.ToString(); // Structs in Lists do not have a Label
			fieldst.type   = FieldTypes.Struct;
			fieldst.Struct = st;

			uint fieldid = (uint)_f.Data._fields.Count;
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(fieldst);


			GffData.Field field;

			field = new GffData.Field();
			field.label = "Class";
			field.type  = FieldTypes.INT;
			field.INT   = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "ClassLevel";
			field.type  = FieldTypes.SHORT;
			field.SHORT = 1;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			for (int i = 0; i != 10; ++i)
			{
				field = new GffData.Field();
				field.label = LABEL_PREFIX_KNOWN + i;
				field.type  = FieldTypes.List;
				field.List  = new List<uint>();

				fieldid = (uint)_f.Data._fields.Count;
				st.fieldids.Add(fieldid);
				_f.Data._fieldids.Add(fieldid);
				_f.Data._fields.Add(field);
			}

			for (int i = 0; i != 10; ++i)
			{
				field = new GffData.Field();
				field.label = LABEL_PREFIX_MEMORIZED + i;
				field.type  = FieldTypes.List;
				field.List  = new List<uint>();

				fieldid = (uint)_f.Data._fields.Count;
				st.fieldids.Add(fieldid);
				_f.Data._fieldids.Add(fieldid);
				_f.Data._fields.Add(field);
			}

			GffData.allStructs.Add(st);
			_f.AddField(fieldst, SelectedNode); // NOTE: That will sort the tree-branch auto.

			SelectedNode = SelectedNode.Nodes[id];
		} */


/*		/// <summary>
		/// Adds a Struct (a Feat structure) to the "FeatList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddFeat(object sender, EventArgs e)
		{
			var st = new Struct();
			st.typeid   = 1; // <- that's what's in the UTCs I've looked at.
			st.fieldids = new List<uint>();

			int id = SelectedNode.Nodes.Count;

			var fieldst = new GffData.Field();
			fieldst.label  = id.ToString();
			fieldst.type   = FieldTypes.Struct;
			fieldst.Struct = st;

			uint fieldid = (uint)_f.Data._fields.Count;
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(fieldst);


			GffData.Field field;

			field = new GffData.Field();
			field.label = "Feat";
			field.type  = FieldTypes.WORD;
			field.WORD  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			GffData.allStructs.Add(st);
			_f.AddField(fieldst, SelectedNode);

			SelectedNode = SelectedNode.Nodes[id];
		} */


/*		/// <summary>
		/// Adds a Struct (an InventoryItem structure) to the "ItemList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddInventoryItem(object sender, EventArgs e)
		{
			int id = SelectedNode.Nodes.Count;

			var st = new Struct();
			st.typeid   = (uint)id; // <- that's what's in the UTCs I've looked at.
			st.fieldids = new List<uint>();

			var fieldst = new GffData.Field();
			fieldst.label  = id.ToString();
			fieldst.type   = FieldTypes.Struct;
			fieldst.Struct = st;

			uint fieldid = (uint)_f.Data._fields.Count;
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(fieldst);


			GffData.Field field;

			field = new GffData.Field();
			field.label = "Dropable";
			field.type  = FieldTypes.BYTE;
			field.BYTE  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label   = "EquippedRes";
			field.type    = FieldTypes.CResRef;
			field.CResRef = String.Empty;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "PickPocketable";
			field.type  = FieldTypes.BYTE;
			field.BYTE  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "Repos_PosX";
			field.type  = FieldTypes.WORD;
			field.WORD  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "Repos_PosY";
			field.type  = FieldTypes.WORD;
			field.WORD  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			GffData.allStructs.Add(st);
			_f.AddField(fieldst, SelectedNode);

			SelectedNode = SelectedNode.Nodes[id];
		} */


/*		internal uint _bitslot;

		/// <summary>
		/// Adds a Struct (an EquippedItem structure) to the "Equip_ItemList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddEquippedItem(object sender, EventArgs e)
		{
			GffData.Field field;

			_bitslot = 0;
			for (int i = 0; i != SelectedNode.Nodes.Count; ++i)
			{
				field = (GffData.Field)SelectedNode.Nodes[i].Tag;
				_bitslot |= field.Struct.typeid;
			}

			using (var dialog = new Dialog_EquippedItem(_bitslot))
			{
				_bitslot = 0;
				if (dialog.ShowDialog(this) == DialogResult.OK
					&& _bitslot != 0) // safety.
				{
					var st = new Struct();
					st.typeid   = _bitslot; // <- that's what's in the UTCs I've looked at.
					st.fieldids = new List<uint>();

					var fieldst = new GffData.Field();
					fieldst.type   = FieldTypes.Struct;
					fieldst.Struct = st;

					uint fieldid = (uint)_f.Data._fields.Count;
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(fieldst);


					field = new GffData.Field();
					field.label = "Dropable";
					field.type  = FieldTypes.BYTE;
					field.BYTE  = 0;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label   = "EquippedRes";
					field.type    = FieldTypes.CResRef;
					field.CResRef = String.Empty;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label = "PickPocketable";
					field.type  = FieldTypes.BYTE;
					field.BYTE  = 0;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label = "Repos_PosX";
					field.type  = FieldTypes.WORD;
					field.WORD  = 0;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label = "Repos_PosY";
					field.type  = FieldTypes.WORD;
					field.WORD  = 0;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					GffData.allStructs.Add(st);

					// keep the List's nodes in the correct sequence ->
					// they are arranged by Struct.typeid
					BeginUpdate();

					TreeNode top = TopNode;

					int i;
					var nodes = new List<Sortable>();
					for (i = 0; i != SelectedNode.Nodes.Count; ++i)
						nodes.Add((Sortable)SelectedNode.Nodes[i]);

					SelectedNode.Nodes.Clear();

					i = 0;
					while (i != nodes.Count)
					{
						field = (GffData.Field)nodes[i].Tag;
						if (field.Struct.typeid < st.typeid)
						{
							SelectedNode.Nodes.Add(nodes[i++]);
						}
						else break;
					}

					fieldst.label = i.ToString();
					_f.AddField(fieldst, SelectedNode, null, true);
					int j = i;

					Sortable node;
					while (i != nodes.Count)
					{
						node = nodes[i];
						field = (GffData.Field)node.Tag;
						field.label =
						node._label = (i + 1).ToString();
						node.Text = _f.ConstructNodeText(field, null, true);

						SelectedNode.Nodes.Add(nodes[i++]);
					}

					SelectedNode = SelectedNode.Nodes[j];
					TopNode = top;

					EndUpdate();

					SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those function calls are in a specific sequence.
				}
			}
		} */


/*		internal string _varLabel;
		internal string _varValue;
		internal uint   _varType;

		/// <summary>
		/// Adds a Struct (a Variable structure) to the "VarTable" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddVariable(object sender, EventArgs e)
		{
			using (var dialog = new Dialog_Variable())
			{
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					var st = new Struct();
					st.typeid   = 0; // <- that's what's in the UTCs I've looked at.
					st.fieldids = new List<uint>();

					int id = SelectedNode.Nodes.Count;

					var fieldst = new GffData.Field();
					fieldst.label  = id.ToString(); // Structs in Lists do not have a Label
					fieldst.type   = FieldTypes.Struct;
					fieldst.Struct = st;

					uint fieldid = (uint)_f.Data._fields.Count;
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(fieldst);


					GffData.Field field;

					field = new GffData.Field();
					field.label      = "Name";
					field.type       = FieldTypes.CExoString;
					field.CExoString = _varLabel;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label = "Type";
					field.type  = FieldTypes.DWORD;
					field.DWORD = _varType;

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					field = new GffData.Field();
					field.label = "Value";

					switch (_varType)
					{
						case Dialog_Variable.Type_non:		// not stable in toolset. is Disabled in the dialog
							return;

						case Dialog_Variable.Type_INT:
							field.type = FieldTypes.INT;
							field.INT  = Int32.Parse(_varValue);
							break;

						case Dialog_Variable.Type_FLOAT:
							field.type  = FieldTypes.FLOAT;
							field.FLOAT = float.Parse(_varValue);
							break;

						case Dialog_Variable.Type_STRING:
							field.type       = FieldTypes.CExoString;
							field.CExoString = _varValue;
							break;

						case Dialog_Variable.Type_LOCATION:	// not stable in toolset. is Disabled in the dialog
							return;

						case Dialog_Variable.Type_UINT:		// and I can't see this being useful at all.
							field.type  = FieldTypes.DWORD;
							field.DWORD = UInt32.Parse(_varValue);
							break;
					}

					fieldid = (uint)_f.Data._fields.Count;
					st.fieldids.Add(fieldid);
					_f.Data._fieldids.Add(fieldid);
					_f.Data._fields.Add(field);


					GffData.allStructs.Add(st);
					_f.AddField(fieldst, SelectedNode);

					SelectedNode = SelectedNode.Nodes[id];
				}
			}
		} */

/*		/// <summary>
		/// Checks for a duplicated variable-label + variable-type.
		/// @note The toolset allows setting variables with duplicated
		/// type+label but that's illogical so it's disallowed here. Duplicated
		/// labels are allowed if their types are different.
		/// </summary>
		/// <param name="typeid"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		internal bool is_VarRedundant(uint typeid, string label)
		{
			GffData.Field field;
			int state;

			foreach (TreeNode st in SelectedNode.Nodes)
			{
				state = 0;
				for (int i = 0; i != st.Nodes.Count; ++i)
				{
					field = (GffData.Field)st.Nodes[i].Tag;
					if (field.label == "Name" && field.CExoString == label && ++state == 2)
						return true;

					if (field.DWORD == typeid && ++state == 2)
						return true;
				}
			}
			return false;
		} */


		internal Languages _langid;

		/// <summary>
		/// Adds a Locale to a CExoLocString Field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddLocale(object sender, EventArgs e)
		{
			using (var dialog = new Dialog_Locale())
			{
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					var locale = new GffData.Locale();
					locale.langid = _langid;		// the dialog defaults to English
					locale.F      = false;			// default
					locale.local  = String.Empty;	// default

					((GffData.Field)SelectedNode.Tag).Locales.Add(locale);

					var field = new GffData.Field();
					field.type     = FieldTypes.locale;
					field.localeid = (uint)SelectedNode.Nodes.Count;
					field.label    = GffData.Locale.GetLanguageString(locale.langid);
					if (locale.F)
						field.label += GeneralGFF.SUF_F;

					_f.AddField(field, SelectedNode, locale);
				}
			}
		}

		/// <summary>
		/// Deletes a Locale from a CExoLocString.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_DeleteLocale(object sender, EventArgs e)
		{
			var parent = SelectedNode.Parent;
			var CExoLocString = (GffData.Field)parent.Tag;

			uint localeid = ((GffData.Field)SelectedNode.Tag).localeid;
			GffData.Locale locale = CExoLocString.Locales[(int)localeid];

			GffData.Field field;

			var locales = parent.Nodes;
			for (++localeid; localeid != locales.Count; ++localeid)
			{
				field = (GffData.Field)locales[(int)localeid].Tag;
				--field.localeid;
			}

			CExoLocString.Locales.Remove(locale);

			_bypassExpand = true;
			SelectedNode.Remove();

			if (SelectedNode != parent)
			{
				_bypassExpand = true;
				SelectedNode = parent;
			}
		}



/*		/// <summary>
		/// Adds a Struct (a Spell structure) to either a "KnownList*" or a
		/// "MemorizedList*" List (in the "ClassList").
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddSpell(object sender, EventArgs e)
		{
			var st = new Struct();
			st.typeid   = 3; // <- that's what's in the UTCs I've looked at.
			st.fieldids = new List<uint>();

			int id = SelectedNode.Nodes.Count;

			var fieldst = new GffData.Field();
			fieldst.label  = id.ToString(); // Structs in Lists do not have a Label
			fieldst.type   = FieldTypes.Struct;
			fieldst.Struct = st;

			uint fieldid = (uint)_f.Data._fields.Count;
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(fieldst);


			GffData.Field field;

			field = new GffData.Field();
			field.label = "Spell";
			field.type  = FieldTypes.WORD;
			field.WORD  = 0; // default spell-id #0

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "SpellFlags";
			field.type  = FieldTypes.BYTE;
			field.BYTE  = 1; // what is that

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			field = new GffData.Field();
			field.label = "SpellMetaMagic";
			field.type  = FieldTypes.BYTE;
			field.BYTE  = 0;

			fieldid = (uint)_f.Data._fields.Count;
			st.fieldids.Add(fieldid);
			_f.Data._fieldids.Add(fieldid);
			_f.Data._fields.Add(field);


			GffData.allStructs.Add(st);
			_f.AddField(fieldst, SelectedNode);

			SelectedNode = SelectedNode.Nodes[id];
		} */


		// debug funct.
/*		void PrintNodeinfo(TreeNode parent)
		{
			for (int i = 0; i != parent.Nodes.Count; ++i)
			{
				var node = parent.Nodes[i];
				var field = (GffData.Field)node.Tag;

				logfile.Log("node #" + i + " node.label= " + node._label + " field.label= " + field.label);
			}
		} */

/*		void StationarySort()
		{
			BeginUpdate();
			TreeNode node = TopNode;
			Sort();
			TopNode = node;
			EndUpdate();
		} */

/*		void DeleteField()
		{} */

/*					switch (field.type)
					{
						case FieldTypes.BYTE:
							break;
						case FieldTypes.CHAR:
							break;
						case FieldTypes.WORD:
							break;
						case FieldTypes.SHORT:
							break;
						case FieldTypes.DWORD:
							break;
						case FieldTypes.INT:
							break;
						case FieldTypes.DWORD64:
							break;
						case FieldTypes.INT64:
							break;
						case FieldTypes.FLOAT:
							break;
						case FieldTypes.DOUBLE:
							break;
						case FieldTypes.CExoString:
							break;
						case FieldTypes.CResRef:
							break;
						case FieldTypes.CExoLocString:
							break;
						case FieldTypes.VOID:
							break;
						case FieldTypes.Struct:
							break;
						case FieldTypes.List:
							break;
						case FieldTypes.locale:
							break;
					} */



//		void contextclick_AddTemplate(object sender, EventArgs e)
//		{}
//		void contextclick_DeleteTemplate(object sender, EventArgs e)
//		{}
		#endregion Context


		#region Events
//		protected override void WndProc(ref Message m)
//		{
//			if (m.Msg == 0x203) m.Msg = 0x201; // change WM_LBUTTONDBLCLK to WM_LBUTTONCLICK
//			base.WndProc(ref m);
//		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			//logfile.Log("OnAfterSelect() _bypassExpand= " + _bypassExpand);

			SelectField(e.Node);

			if (!_bypassExpand) // prevent key-navigation and/or node-deletion from expanding treenodes ->
			{
				//logfile.Log(". _bypassExpand is false");
				e.Node.Expand();
			}
			else
			{
				//logfile.Log(". set _bypassExpand FALSE");
				_bypassExpand = false;
			}
		}


		bool _RtbEnabled;

		/// <summary>
		/// - helper for click_Select()
		/// </summary>
		void ResetValuePanel()
		{
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

			_f.cb_Checker.Visible =
			_f.la_Checker.Visible = false;

			_f._prevalText = String.Empty;
			_f._prevalCheckboxChecked = false;
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
			ResetValuePanel();

			if (node.Tag != null)
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
						_f.la_Des.Text = "32-chars NwN2 / 16-chars NwN1" + Environment.NewLine + "ASCII";
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
						_f.la_Des.Text = "talktable strref" + Environment.NewLine + "-1.." + 0x00FFFFFF;
						_f.la_Val.Text = "CExoLocString";

						uint strref = field.CExoLocStrref;
						if (strref != UInt32.MaxValue)
							_f.tb_Val.Text = (strref & 0x00FFFFFF).ToString();
						else
							_f.tb_Val.Text = "-1";

						bool @checked =  strref != UInt32.MaxValue
									 && (strref & 0x01000000) != 0;
						_f.cb_Checker.Checked = (_f._prevalCheckboxChecked = @checked);

						_f.la_Checker.Text = "Custom talktable";

						_f.cb_Checker.Visible =
						_f.la_Checker.Visible = true;

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
						var CExoLocString = (GffData.Field)node.Parent.Tag;
						GffData.Locale locale = CExoLocString.Locales[(int)field.localeid];

						if (locale.langid == Languages.GffToken)
						{
							_f.la_Des.Text = "UTF8";
							_f.la_Val.Text = "GffToken";
						}
						else
						{
							_f.la_Des.Text = "UTF8 localized";
							_f.la_Val.Text = "locale";

							_f.cb_Checker.Checked = _f._prevalCheckboxChecked = locale.F;
							_f.la_Checker.Text = "Feminine";

							_f.cb_Checker.Visible =
							_f.la_Checker.Visible = true;
						}

						_f.rt_Val.Text = locale.local;

						EnableRichtextbox();

						_f._prevalText = _f.rt_Val.Text;
						break;
					}
				}
			}
			else // is TopLevelStruct's node
			{
				_f.la_Des.Text = "ASCII";
				_f.la_Val.Text = "GFF type + version";

				_f.tb_Val.Text = _f.Data.Ver;

				_f.tb_Val.Enabled   = true;
				_f.tb_Val.BackColor = Color.Honeydew;

				_f._prevalText = _f.tb_Val.Text;
			}
		}


		/// <summary>
		/// Handles keydown events.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			_bypassExpand = false;

			switch (e.KeyData)
			{
				case Keys.Enter:
					e.SuppressKeyPress = true;
					BeginInvoke(DontBeepEvent);
					break;

				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
					_bypassExpand = true;
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
		#endregion Events
	}



	/// <summary>
	/// A TreeNode that can be sorted according to a specified label.
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
	/// A sorter of treenodes.
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
