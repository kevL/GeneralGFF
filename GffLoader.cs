﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// Static object that loads GFF files.
	/// </summary>
	static class GffLoader
	{
		#region Methods
		/// <summary>
		/// Loads a GFF file into a TreeList.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="pfe"></param>
		internal static void LoadGFFfile(GeneralGFF f, string pfe)
		{
			f._tree.BeginUpdate();
			f._tree.Nodes.Clear();

			f.ResetEditPanel();

			f.GffData = GffReader.ReadGFFfile(pfe);
			if (f.GffData != null && GffReader.Structs.Count != 0)
			{
				// Load the TopLevelStruct - all else follows ->
				// NOTE: The TLS has no Field ... thus the rootnode of the
				// TreeList has no Tag.

				string label = Path.GetFileNameWithoutExtension(f.GffData.Pfe).ToUpper();
				TreeNode root = f._tree.Nodes.Add(label); // NOTE: TreeView doesn't like the root to be a Sortable. or bleh

				// instantiate the TLS's fieldids as treenodes ->
				List<uint> fieldids = GffReader.Structs[0].fieldids;
				for (int i = 0; i != fieldids.Count; ++i)
				{
					AddField(GffReader.Fields[(int)fieldids[i]], root);
				}

				f._tree.Nodes[0].Expand();
				f._tree.SelectedNode = f._tree.Nodes[0];
			}

			f._tree.EndUpdate();

			GffReader.Structs.Clear();
			GffReader.Fields .Clear();
		}

		/// <summary>
		/// Adds a Field to a treenode.
		/// </summary>
		/// <param name="field">a Field to add</param>
		/// <param name="parent">a treenode to add it to</param>
		/// <param name="locale">a locale if applicable</param>
		static void AddField(GffData.Field field, TreeNode parent, GffData.Locale locale = null)
		{
			// TODO: Refactor things throughout the code such that the Tag of a
			// treenode is *either* a GffData.Field *or* a GffData.Locale.

			string text = GeneralGFF.ConstructNodetext(field, locale);

			var node = new Sortable(text, field.label);
			node.Tag = field;
			parent.Nodes.Add(node);

			switch (field.type)
			{
				case FieldType.Struct: // childs can be of any Type.
				{
					List<uint> fieldids = field.Struct.fieldids;
					for (int i = 0; i != fieldids.Count; ++i)
					{
						AddField(GffReader.Fields[(int)fieldids[i]], node);
					}
					break;
				}

				case FieldType.List: // childs are Structs.
				{
					List<uint> list = field.List;
					for (int i = 0; i != list.Count; ++i)
					{
						// NOTE: Structs in Lists do not have a Label inside a GFF-file.
						// so give Structs in Lists a pseudo-label for their treenode(s)
						field = new GffData.Field();
						field.type   = FieldType.Struct;
						field.label  = i.ToString();
						field.Struct = GffReader.Structs[(int)list[i]];

						AddField(field, node);
					}
					break;
				}

				case FieldType.CExoLocString: // childs are Locales.
					if (field.Locales != null)
					{
						int locales = field.Locales.Count;
						for (int i = 0; i != locales; ++i)
						{
							locale = field.Locales[i];

							var fieldloc = new GffData.Field();
							fieldloc.type = FieldType.locale;
							fieldloc.localeid = (uint)i;
							fieldloc.label = GffData.Locale.GetLanguageString(locale.langid, locale.F);

							AddField(fieldloc, node, locale);

							LocaleDialog.SetLocaleFlag(ref field.localeflags,
													   locale.langid,
													   locale.F);
						}
					}
					break;
			}
		}
		#endregion Methods
	}
}
