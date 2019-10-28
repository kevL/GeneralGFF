using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace generalgff
{
	sealed class GffLoader
	{
		#region Methods
		/// <summary>
		/// Loads a GFF file into a TreeList.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="pfe"></param>
		internal void LoadGFFfile(GeneralGFF f, string pfe)
		{
			f._tl.BeginUpdate();
			f._tl.Nodes.Clear();

			f.GffData = GffReader.ReadGFFfile(pfe);
			if (f.GffData != null && GffReader.Structs.Count != 0)
			{
				// Load the TopLevelStruct - all else follows ->
				// NOTE: The TLS has no Field ... thus the rootnode of the
				// TreeList has no Tag.

				string label = Path.GetFileNameWithoutExtension(f.GffData.Pfe).ToUpper();
				TreeNode root = f._tl.Nodes.Add(label); // NOTE: TreeView doesn't like the root to be a Sortable. or bleh

				// instantiate the TLS's fieldids as treenodes ->
				List<uint> fieldids = GffReader.Structs[0].fieldids;
				for (int i = 0; i != fieldids.Count; ++i)
				{
					AddField(GffReader.Fields[(int)fieldids[i]], root);
				}

				f._tl.Nodes[0].Expand();
				f._tl.SelectedNode = f._tl.Nodes[0];
			}

			f._tl.EndUpdate();
		}

		/// <summary>
		/// Adds a Field to a treenode.
		/// </summary>
		/// <param name="field">a Field to add</param>
		/// <param name="parent">a treenode to add it to</param>
		/// <param name="locale">a locale if applicable</param>
		internal void AddField(GffData.Field field, TreeNode parent, GffData.Locale locale = null)
		{
			string text = GeneralGFF.ConstructNodetext(field, locale);

			var node = new Sortable(text, field.label);
			node.Tag = field;
			parent.Nodes.Add(node);

			switch (field.type)
			{
				case FieldTypes.Struct: // childs can be of any Type.
				{
					List<uint> fieldids = field.Struct.fieldids;
					for (int i = 0; i != fieldids.Count; ++i)
					{
						AddField(GffReader.Fields[(int)fieldids[i]], node);
					}
					break;
				}

				case FieldTypes.List: // childs are Structs.
				{
					List<uint> list = field.List;
					for (int i = 0; i != list.Count; ++i)
					{
						// NOTE: Structs in Lists do not have a Label inside a GFF-file.
						// so give Structs in Lists a pseudo-label for their treenode(s)
						field = new GffData.Field();
						field.type   = FieldTypes.Struct;
						field.label  = i.ToString();
						field.Struct = GffReader.Structs[(int)list[i]];

						AddField(field, node, null);
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

							var fieldloc = new GffData.Field();
							fieldloc.type = FieldTypes.locale;
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
