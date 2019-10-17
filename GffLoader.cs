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

			GffReader._structs.Clear();
			GffReader._fields.Clear();
			GffReader._fieldids.Clear();

			f.CurrentData = GffReader.ReadGFFfile(pfe);
			if (f.CurrentData != null && GffReader._structs.Count != 0)
			{
				// Load the TopLevelStruct - all else follows ->
				// NOTE: The TLS has no Field ... thus the rootnode of the
				// TreeList has no Tag.

				string label = Path.GetFileNameWithoutExtension(f.CurrentData._pfe).ToUpper();
				TreeNode root = f._tl.Nodes.Add(label); // NOTE: TreeView doesn't like the root to be a Sortable. or bleh

				// instantiate the TLS's fieldids as treenodes ->
				List<uint> fieldids = GffReader._structs[0].fieldids;
				for (int i = 0; i != fieldids.Count; ++i)
				{
					AddField(GffReader._fields[(int)fieldids[i]], root);
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
		/// <param name="node">a treenode to add it to</param>
		/// <param name="locale">a locale if applicable</param>
		internal void AddField(GffData.Field field, TreeNode node, GffData.Locale locale = null)
		{
			string text = GeneralGFF.ConstructNodetext(field, locale);

			var node_ = new Sortable(text, field.label);
			node_.Tag = field;
			node.Nodes.Add(node_);

			switch (field.type)
			{
				case FieldTypes.Struct: // childs can be of any Type.
				{
					List<uint> fieldids = field.Struct.fieldids;
					for (int i = 0; i != fieldids.Count; ++i)
					{
						AddField(GffReader._fields[(int)fieldids[i]], node_);
					}
					break;
				}

				case FieldTypes.List: // childs are Structs.
				{
					List<uint> list = field.List;
					for (int i = 0; i != list.Count; ++i)
					{
						var field_ = new GffData.Field();

						field_.label  = i.ToString();		// NOTE: Structs in Lists do not have a Label inside a GFF-file.
						field_.type   = FieldTypes.Struct;	// so give Structs in Lists a pseudo-Label for their treenode(s)
						field_.Struct = GffReader._structs[(int)list[i]];

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
								field_.label += GeneralGFF.SUF_F;

							field_.type = FieldTypes.locale;

							AddField(field_, node_, locale);
						}
					}
					break;
			}
		}
		#endregion Methods
	}
}
