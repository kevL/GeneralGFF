using System;
using System.Collections;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// A TreeNode that can be sorted according to a specified label (instead of
	/// its displayed text).
	/// </summary>
	sealed class Sortable
		: TreeNode
	{
		internal string _label;

		internal Sortable(string text, string label)
			: base(text)
		{
			_label = label;
		}


		/// <summary>
		/// Duplicates a specified Sortable since Clone() is effed regardless.
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		internal static Sortable Duplicate(Sortable src)
		{
			var dst = new Sortable(src.Text, src._label);
			dst.Tag = GffData.Field.Duplicate((GffData.Field)src.Tag);

			AddSubs(src, dst);

			return dst;
		}

		/// <summary>
		/// Recurses subnodes for Duplicate().
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		static void AddSubs(TreeNode src, TreeNode dst)
		{
			for (int i = 0; i != src.Nodes.Count; ++i)
			{
				var node = new Sortable(src.Nodes[i].Text, ((Sortable)src.Nodes[i])._label);
				node.Tag = GffData.Field.Duplicate((GffData.Field)src.Nodes[i].Tag);

				dst.Nodes.Add(node);

				AddSubs((Sortable)src.Nodes[i], node);
			}
		}
	}


	/// <summary>
	/// A sorter of Sortable treenodes.
	/// </summary>
	sealed class NodeSorter
		: IComparer
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
