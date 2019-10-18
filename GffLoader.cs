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
		/// Loads a GFF file into a specified TreeList.
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
						field = new GffData.Field();

						field.label  = i.ToString();		// NOTE: Structs in Lists do not have a Label inside a GFF-file.
						field.type   = FieldTypes.Struct;	// so give Structs in Lists a pseudo-Label for their treenode(s)
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

							field = new GffData.Field();
							field.localeid = (uint)i;
							field.label = GffData.Locale.GetLanguageString(locale.langid);
							if (locale.F)
								field.label += GeneralGFF.SUF_F;

							field.type = FieldTypes.locale;

							AddField(field, node, locale);
						}
					}
					break;
			}
		}
		#endregion Methods
	}
}
