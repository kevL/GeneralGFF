using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace generalgff
{
	sealed partial class TreeList
	{
		const string LABEL_CLASSLIST        = "ClassList";
		const string LABEL_FEATLIST         = "FeatList";
		const string LABEL_ITEMLIST         = "ItemList";
		const string LABEL_EQUIPITEMLIST    = "Equip_ItemList";
//		const string LABEL_TEMPLATELIST     = "TemplateList";
		const string LABEL_VARTABLE         = "VarTable";

		const string LABEL_PREFIX_KNOWN     = "KnownList";
		const string LABEL_PREFIX_MEMORIZED = "MemorizedList";

		const int Max_CLASSES  = 4;
		const int Max_EQUIPPED = 18;
//		const int Max_LOCALES  = 20;


		/// <summary>
		/// Populates the extended context.
		/// @note UTC files are defined in such a way that only one type of
		/// operation will be available on any given treenode and further that a
		/// type of operation will be specific to a node-level (1..4).
		/// </summary>
		/// <param name="node"></param>
		void context_Extension(TreeNode node)
		{
			// TODO: Add "Cloak" Struct to root ...

			// if (node.Tag != null) ...

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
									break;

//								case LABEL_TEMPLATELIST: // I have no clue what this is.
//									it = new MenuItem("add template", contextclick_AddTemplate);
//									break;

								case LABEL_VARTABLE:
									it = new MenuItem("add variable", contextclick_AddVariable);
									break;

								// TODO: DmgReduction
							}
							break;

						case FieldTypes.CExoLocString:
//							if (node.Nodes.Count < Max_LOCALES) // too complicated wrt/ gender
							it = new MenuItem("add localized string", contextclick_AddLocale);
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
//										_isInventoryIt = false;
										it = new MenuItem("delete Class", contextclick_Delete);
										break;

									case LABEL_FEATLIST:
//										_isInventoryIt = false;
										it = new MenuItem("delete Feat", contextclick_Delete);
										break;

									case LABEL_ITEMLIST:
//										_isInventoryIt = true;
										it = new MenuItem("delete inventory item", contextclick_Delete);
										break;

									case LABEL_EQUIPITEMLIST:
//										_isInventoryIt = false;
										it = new MenuItem("delete equipped item", contextclick_Delete);
										break;

//									case LABEL_TEMPLATELIST: // I have no clue what this is.
//										_isInventoryIt = false;
//										it = new MenuItem("delete template", contextclick_Delete);
//										break;

									case LABEL_VARTABLE:
//										_isInventoryIt = false;
										it = new MenuItem("delete variable", contextclick_Delete);
										break;

									// TODO: DmgReduction
								}
							}
							break;
						}

						case FieldTypes.locale:
							it = new MenuItem("delete localized string", contextclick_Delete);
							break;
					}
					break;
				}

				case 3:
				{
					var field = (GffData.Field)node.Tag;
					if (   field.label.StartsWith(LABEL_PREFIX_KNOWN,     StringComparison.Ordinal)
						|| field.label.StartsWith(LABEL_PREFIX_MEMORIZED, StringComparison.Ordinal))
					{
						it = new MenuItem("add Spell", contextclick_AddSpell);
					}
					break;
				}

				case 4:
				{
					var field = (GffData.Field)node.Parent.Tag;
					if (   field.label.StartsWith(LABEL_PREFIX_KNOWN,     StringComparison.Ordinal)
						|| field.label.StartsWith(LABEL_PREFIX_MEMORIZED, StringComparison.Ordinal))
					{
//						_isInventoryIt = false;
						it = new MenuItem("delete Spell", contextclick_Delete);
					}
					break;
				}
			}

			if (it != null)
			{
				if (toggle != null) ContextMenu.MenuItems.Add(new MenuItem("-"));
				ContextMenu.MenuItems.Add(it);
			}
		}


		/// <summary>
		/// Adds a Struct (a Class structure) to the "ClassList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddClass(object sender, EventArgs e)
		{
			BeginUpdate();

			TreeNode top = TopNode;

			var field = new GffData.Field();
			field.type = FieldTypes.Struct;
			field.label = SelectedNode.Nodes.Count.ToString(); // Structs in Lists do not have a Label
			field.Struct = new Struct();
			field.Struct.typeid = 2; // <- that's what's in the UTCs I've looked at.

			string text = GeneralGFF.ConstructNodetext(field);
			var node = new Sortable(text, field.label);
			node.Tag = field;
			int id = SelectedNode.Nodes.Add(node);


			field = new GffData.Field();
			field.type = FieldTypes.INT;
			field.label = "Class";
			field.INT = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type  = FieldTypes.SHORT;
			field.label = "ClassLevel";
			field.SHORT = 1;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			for (int i = 0; i != 10; ++i)
			{
				field = new GffData.Field();
				field.type = FieldTypes.List;
				field.label = LABEL_PREFIX_KNOWN + i;

				text = GeneralGFF.ConstructNodetext(field);
				node = new Sortable(text, field.label);
				node.Tag = field;
				SelectedNode.Nodes[id].Nodes.Add(node);
			}

			for (int i = 0; i != 10; ++i)
			{
				field = new GffData.Field();
				field.type = FieldTypes.List;
				field.label = LABEL_PREFIX_MEMORIZED + i;

				text = GeneralGFF.ConstructNodetext(field);
				node = new Sortable(text, field.label);
				node.Tag = field;
				SelectedNode.Nodes[id].Nodes.Add(node);
			}

			SelectedNode = SelectedNode.Nodes[id];
			SelectedNode.Expand();

			TopNode = top;
			SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


			_f.GffData.Changed = true;
			_f.GffData = _f.GffData;

			EndUpdate();
		}


		/// <summary>
		/// Adds a Struct (a Feat structure) to the "FeatList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddFeat(object sender, EventArgs e)
		{
			BeginUpdate();

			TreeNode top = TopNode;

			var field = new GffData.Field();
			field.type = FieldTypes.Struct;
			field.label = SelectedNode.Nodes.Count.ToString(); // Structs in Lists do not have a Label
			field.Struct = new Struct();
			field.Struct.typeid = 1; // <- that's what's in the UTCs I've looked at.

			string text = GeneralGFF.ConstructNodetext(field);
			var node = new Sortable(text, field.label);
			node.Tag = field;
			int id = SelectedNode.Nodes.Add(node);


			field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = "Feat";
			field.WORD = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			SelectedNode = SelectedNode.Nodes[id];
			SelectedNode.Expand();

			TopNode = top;
			SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


			_f.GffData.Changed = true;
			_f.GffData = _f.GffData;

			EndUpdate();
		}


		/// <summary>
		/// Adds a Struct (an InventoryItem structure) to the "ItemList" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddInventoryItem(object sender, EventArgs e)
		{
			BeginUpdate();

			TreeNode top = TopNode;

			int id = SelectedNode.Nodes.Count;

			var field = new GffData.Field();
			field.type = FieldTypes.Struct;
			field.label = id.ToString(); // Structs in Lists do not have a Label
			field.Struct = new Struct();
			field.Struct.typeid = (uint)id; // <- that's what's in the UTCs I've looked at.

			string text = GeneralGFF.ConstructNodetext(field);
			var node = new Sortable(text, field.label);
			node.Tag = field;
			id = SelectedNode.Nodes.Add(node);


			field = new GffData.Field();
			field.type = FieldTypes.BYTE;
			field.label = "Dropable";
			field.BYTE = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.CResRef;
			field.label = "EquippedRes";
			field.CResRef = String.Empty;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.BYTE;
			field.label = "Pickpocketable";
			field.BYTE = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = "Repos_PosX";
			field.WORD = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = "Repos_PosY";
			field.WORD = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			SelectedNode = SelectedNode.Nodes[id];
			SelectedNode.Expand();

			TopNode = top;
			SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


			_f.GffData.Changed = true;
			_f.GffData = _f.GffData;

			EndUpdate();
		}


		internal uint _bitslot;

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

			using (var f = new EquippedItemDialog(_bitslot))
			{
				_bitslot = 0;
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					BeginUpdate();

					TreeNode top = TopNode;

					field = new GffData.Field();
					field.type = FieldTypes.Struct;
					field.label = SelectedNode.Nodes.Count.ToString(); // Structs in Lists do not have a Label
					field.Struct = new Struct();
					field.Struct.typeid = _bitslot; // <- that's what's in the UTCs I've looked at.

					string text = GeneralGFF.ConstructNodetext(field);
					var node = new Sortable(text, field.label);
					node.Tag = field;
					int id = SelectedNode.Nodes.Add(node);


					field = new GffData.Field();
					field.type = FieldTypes.BYTE;
					field.label = "Dropable";
					field.BYTE = 0;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.type = FieldTypes.CResRef;
					field.label = "EquippedRes";
					field.CResRef = String.Empty;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.type = FieldTypes.BYTE;
					field.label = "Pickpocketable";
					field.BYTE = 0;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.type = FieldTypes.WORD;
					field.label = "Repos_PosX";
					field.WORD = 0;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.type = FieldTypes.WORD;
					field.label = "Repos_PosY";
					field.WORD = 0;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);


					// keep the List's nodes in the correct sequence ->
					// they are arranged by Struct.typeid ascending

					int i; // iterator for the sortables
					int j; // tracker for the node # added

					// store all the sortable Structs in the List ->
					// NOTE: This is not an ideal routine. Because it adds all
					// the nodes to the tree, then removes them, then adds them
					// all again. It should, rather, just construct the Struct
					// and its fields, then add the Struct to the tree.

					var sortables = new List<Sortable>();
					for (i = 0; i != SelectedNode.Nodes.Count; ++i)
					{
						sortables.Add((Sortable)SelectedNode.Nodes[i]);
					}

					SelectedNode.Nodes.Clear(); // NOTE: 'SelectedNode' is the "Equip_ItemList" list-field.


					// re-add the sortables before the added sortable ->
					i = j = 0;
					while (i != sortables.Count)
					{
						field = (GffData.Field)sortables[i].Tag;
						if (field.Struct.typeid < _bitslot)
						{
							// NOTE: 'field.label' doesn't need to be changed here
							sortables[i].Text = GeneralGFF.ConstructNodetextEquipped(field);
							SelectedNode.Nodes.Add(sortables[i]);
							j = i;
						}
						++i;
					}

					id = ++j;

					// re-add the last sortable into the sequence ->
					// replace vals that were set above for the Struct's label and its Sortable's text and label
					field = (GffData.Field)sortables[sortables.Count - 1].Tag;
					field.label = sortables[sortables.Count - 1]._label = j.ToString();
					sortables[sortables.Count - 1].Text = GeneralGFF.ConstructNodetextEquipped(field);
					SelectedNode.Nodes.Add(sortables[sortables.Count - 1]);


					// re-add the sortables that go after the added sortable ->
					i = 0;
					while (i != sortables.Count)
					{
						field = (GffData.Field)sortables[i].Tag;
						if (field.Struct.typeid > _bitslot)
						{
							field.label = sortables[i]._label = (++j).ToString();
							sortables[i].Text = GeneralGFF.ConstructNodetextEquipped(field);
							SelectedNode.Nodes.Add(sortables[i]);
						}
						++i;
					}

					SelectedNode = SelectedNode.Nodes[id];
					SelectedNode.Expand();

					TopNode = top;
					SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


					_f.GffData.Changed = true;
					_f.GffData = _f.GffData;

					EndUpdate();
				}
			}
		}


//		void contextclick_AddTemplate(object sender, EventArgs e)
//		{}
//		void contextclick_DeleteTemplate(object sender, EventArgs e)
//		{}


		internal string _varLabel;
		internal string _varValue;
		internal uint   _varType;

		/// <summary>
		/// Adds a Struct (a Variable structure) to the "VarTable" List.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddVariable(object sender, EventArgs e)
		{
			using (var dialog = new VariableDialog())
			{
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					BeginUpdate();

					TreeNode top = TopNode;

					var field = new GffData.Field();
					field.type = FieldTypes.Struct;
					field.label = SelectedNode.Nodes.Count.ToString(); // Structs in Lists do not have a Label
					field.Struct = new Struct();
					field.Struct.typeid = 0; // <- that's what's in the UTCs I've looked at.

					string text = GeneralGFF.ConstructNodetext(field);
					var node = new Sortable(text, field.label);
					node.Tag = field;
					int id = SelectedNode.Nodes.Add(node);


					field = new GffData.Field();
					field.type = FieldTypes.CExoString;
					field.label = "Name";
					field.CExoString = _varLabel;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.type = FieldTypes.DWORD;
					field.label = "Type";
					field.DWORD = _varType;

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					field = new GffData.Field();
					field.label = "Value";

					switch (_varType) // TODO: delete the nodes above or put this above there
					{
						case VariableDialog.Type_non:		// not stable in toolset. is Disabled in the dialog
							return;

						case VariableDialog.Type_INT:
							field.type = FieldTypes.INT;
							field.INT = Int32.Parse(_varValue);
							break;

						case VariableDialog.Type_FLOAT:
							field.type = FieldTypes.FLOAT;
							field.FLOAT = Single.Parse(_varValue);
							break;

						case VariableDialog.Type_STRING:
							field.type = FieldTypes.CExoString;
							field.CExoString = _varValue;
							break;

						case VariableDialog.Type_LOCATION:	// not stable in toolset. is Disabled in the dialog
							return;

						case VariableDialog.Type_UINT:		// and I can't see this being useful at all.
							field.type = FieldTypes.DWORD;
							field.DWORD = UInt32.Parse(_varValue);
							break;
					}

					text = GeneralGFF.ConstructNodetext(field);
					node = new Sortable(text, field.label);
					node.Tag = field;
					SelectedNode.Nodes[id].Nodes.Add(node);

					SelectedNode = SelectedNode.Nodes[id];
					SelectedNode.Expand();

					TopNode = top;
					SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


					_f.GffData.Changed = true;
					_f.GffData = _f.GffData;
		
					EndUpdate();
				}
			}
		}

		/// <summary>
		/// Checks for a duplicated variable-label + variable-type.
		/// @note The toolset allows setting variables with duplicated
		/// type+label but that's illogical so it's disallowed here. Duplicated
		/// labels are allowed if their types are different.
		/// </summary>
		/// <param name="typeid"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		internal bool CheckVariableForRedundancy(uint typeid, string label)
		{
			GffData.Field field;
			int state;

			foreach (TreeNode n in SelectedNode.Nodes)
			{
				state = 0;
				for (int i = 0; i != n.Nodes.Count; ++i)
				{
					field = (GffData.Field)n.Nodes[i].Tag;
					if (field.label == "Name" && field.CExoString == label && ++state == 2)
						return true;

					if (field.DWORD == typeid && ++state == 2) // uh ...
						return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Adds a Struct (a Spell structure) to either a "KnownList*" or a
		/// "MemorizedList*" List (in the "ClassList").
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void contextclick_AddSpell(object sender, EventArgs e)
		{
			BeginUpdate();

			TreeNode top = TopNode;

			var field = new GffData.Field();
			field.type = FieldTypes.Struct;
			field.label = SelectedNode.Nodes.Count.ToString(); // Structs in Lists do not have a Label
			field.Struct = new Struct();
			field.Struct.typeid = 3; // <- that's what's in the UTCs I've looked at.

			string text = GeneralGFF.ConstructNodetext(field);
			var node = new Sortable(text, field.label);
			node.Tag = field;
			int id = SelectedNode.Nodes.Add(node);


			field = new GffData.Field();
			field.type = FieldTypes.WORD;
			field.label = "Spell";
			field.WORD = 0; // default spell-id #0

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.BYTE;
			field.label = "SpellFlags";
			field.BYTE = 1; // what is that

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			field = new GffData.Field();
			field.type = FieldTypes.BYTE;
			field.label = "SpellMetaMagic";
			field.BYTE = 0;

			text = GeneralGFF.ConstructNodetext(field);
			node = new Sortable(text, field.label);
			node.Tag = field;
			SelectedNode.Nodes[id].Nodes.Add(node);

			SelectedNode = SelectedNode.Nodes[id];
			SelectedNode.Expand();

			TopNode = top;
			SelectedNode.Nodes[SelectedNode.Nodes.Count - 1].EnsureVisible(); // yes those calls are in a specific sequence.


			_f.GffData.Changed = true;
			_f.GffData = _f.GffData;

			EndUpdate();
		}
	}
}
