using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace generalgff
{
	sealed class SearchDialog
		:
			Form
	{
		enum DontBeepType
		{
			DBT_D,
			DBT_U
		}


		#region Delegates
		/// <summary>
		/// Good fuckin Lord I just wrote a "DontBeep" delegate.
		/// </summary>
		internal delegate void DontBeepEventHandler();
		#endregion Delegates


		#region Events
		internal event DontBeepEventHandler DontBeepEvent;
		#endregion Events


		#region Fields (static)
		static string _text = String.Empty;
		static bool _substring = true;

		static int _x = -1;
		static int _y;
		static int _w;
		static int _h;
		#endregion Fields (static)


		#region Fields
		GeneralGFF _f;
		TreeList _tl;
		TreeNode _start0;
		bool _reverse;
		#endregion Fields


		#region Properties
		DontBeepType DBType
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SearchDialog(GeneralGFF f)
		{
			InitializeComponent();

			DontBeepEvent += SearchDontBeep;

			_f  =  f;
			_tl = _f._tl;

			if (_x != -1)
			{
				Location   = new Point(_x, _y);
				ClientSize = new Size( _w, _h);
			}
			else
				Location = new Point(_f.Left + 20, _f.Top  + 20);

			tb_Search.Text = _text;

			if (_substring)
				rb_Substring.Checked = true;
			else
				rb_Wholeword.Checked = true;

			tb_Search.Select();
			tb_Search.SelectionStart = tb_Search.Text.Length;
		}
		#endregion cTor


		#region Handlers (override)
		/// <summary>
		/// Closes the dialog on [Escape].
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					Close();
					break;

				case Keys.Enter:
				case Keys.F3:
					e.SuppressKeyPress = true;

					DBType = DontBeepType.DBT_D;
					BeginInvoke(DontBeepEvent);
					break;

				case Keys.Shift | Keys.Enter:
				case Keys.Shift | Keys.F3:
					e.SuppressKeyPress = true;

					DBType = DontBeepType.DBT_U;
					BeginInvoke(DontBeepEvent);
					break;
			}
		}

		/// <summary>
		/// Caches the state of the radio-buttons (substring or wholeword).
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			_f._search = null;

			_text = tb_Search.Text;
			_substring = rb_Substring.Checked;

			_x = Math.Max(0, Location.X);
			_y = Math.Max(0, Location.Y);
			_w = ClientSize.Width;
			_h = ClientSize.Height;
		}
		#endregion Handlers (override)


		#region Handlers
		/// <summary>
		/// Searches the TreeList in the down direction.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void click_Down(object sender, EventArgs e)
		{
			_reverse = false;
			Search();
		}

		/// <summary>
		/// Searches the TreeList in the up direction.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void click_Up(object sender, EventArgs e)
		{
			_reverse = true;
			Search();
		}

		/// <summary>
		/// Handles a DontBeep event.
		/// </summary>
		void SearchDontBeep()
		{
			switch (DBType)
			{
				case DontBeepType.DBT_D: _reverse = false; break;
				case DontBeepType.DBT_U: _reverse = true;  break;
			}
			Search();
		}


		/// <summary>
		/// Resets the textcolor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textchanged_Search(object sender, EventArgs e)
		{
			tb_Search.ForeColor = SystemColors.ControlText;
		}
		#endregion Handlers


		#region Methods
		/// <summary>
		/// Searches the TreeList for a given string.
		/// </summary>
		void Search()
		{
			tb_Search.ForeColor = SystemColors.ControlText;

			_text = tb_Search.Text.ToLower(CultureInfo.CurrentCulture);

			if (_tl.Nodes.Count != 0 && !String.IsNullOrEmpty(_text))
			{
				if (_tl.SelectedNode != null)
					_start0 = _tl.SelectedNode;
				else
					_start0 = _tl.Nodes[0];

				TreeNode next = _start0; // find a node after (or before) the SelectedNode to start search at
				while ((next = GetNextNode(next)) != _start0)
				{
					if (Match(next) != null)
					{
						_tl.SelectedNode = next;
						return;
					}
				}

				tb_Search.ForeColor = Color.Crimson;
			}
		}

		/// <summary>
		/// Checks if the text of a treenode matches the search-text.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		TreeNode Match(TreeNode node)
		{
			string text = node.Text.ToLower(CultureInfo.CurrentCulture);
			
			if ((rb_Substring.Checked && text.Contains(_text))
				|| Regex.IsMatch(text, @"\b" + _text + @"\b"))
			{
				return node; // found
			}
			return null; // NOT found
		}

		/// <summary>
		/// Gets the next node to check (in the appropriate direction).
		/// </summary>
		/// <param name="start"></param>
		/// <returns></returns>
		TreeNode GetNextNode(TreeNode start)
		{
			if (!_reverse)
			{
				if (start.Nodes.Count != 0)		// check for child-node
					return start.Nodes[0];

				if (start.NextNode != null)		// check for next sibling-node
					return start.NextNode;

				while (start.Parent != null)	// check for next of kin (next sibling of parent, or grandparent, etc)
				{
					if (start.Parent.NextNode != null)
						return start.Parent.NextNode;

					start = start.Parent;
				}

				if (start == null)				// no living relations so query The Ancestor
					return _tl.Nodes[0];
			}
			else // reverse direction ->		// lalala talk to the hand.
			{
				if (start.PrevNode != null)
				{
					if (start.PrevNode.Nodes.Count == 0)
						return start.PrevNode;

					start = start.PrevNode.LastNode;
					while (start.Nodes.Count != 0)
						start = start.LastNode;
				}
				else
				{
					if (start.Parent != null)
						return start.Parent;

					start = _tl.Nodes[0];
					while (start.Nodes.Count != 0)
						start = start.LastNode;
				}
			}
			return start; // shall never return null
		}
		#endregion Methods



		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		IContainer components = null;

		TextBox tb_Search;
		Button bt_Down;
		Button bt_Up;
		RadioButton rb_Substring;
		RadioButton rb_Wholeword;


		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.tb_Search = new System.Windows.Forms.TextBox();
			this.bt_Down = new System.Windows.Forms.Button();
			this.bt_Up = new System.Windows.Forms.Button();
			this.rb_Substring = new System.Windows.Forms.RadioButton();
			this.rb_Wholeword = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// tb_Search
			// 
			this.tb_Search.BackColor = System.Drawing.Color.White;
			this.tb_Search.Dock = System.Windows.Forms.DockStyle.Top;
			this.tb_Search.HideSelection = false;
			this.tb_Search.Location = new System.Drawing.Point(0, 0);
			this.tb_Search.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Search.Name = "tb_Search";
			this.tb_Search.Size = new System.Drawing.Size(172, 20);
			this.tb_Search.TabIndex = 0;
			this.tb_Search.WordWrap = false;
			this.tb_Search.TextChanged += new System.EventHandler(this.textchanged_Search);
			// 
			// bt_Down
			// 
			this.bt_Down.Location = new System.Drawing.Point(3, 23);
			this.bt_Down.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Down.Name = "bt_Down";
			this.bt_Down.Size = new System.Drawing.Size(75, 25);
			this.bt_Down.TabIndex = 1;
			this.bt_Down.Text = "▼";
			this.bt_Down.UseVisualStyleBackColor = true;
			this.bt_Down.Click += new System.EventHandler(this.click_Down);
			// 
			// bt_Up
			// 
			this.bt_Up.Location = new System.Drawing.Point(3, 49);
			this.bt_Up.Margin = new System.Windows.Forms.Padding(0);
			this.bt_Up.Name = "bt_Up";
			this.bt_Up.Size = new System.Drawing.Size(75, 25);
			this.bt_Up.TabIndex = 2;
			this.bt_Up.Text = "▲";
			this.bt_Up.UseVisualStyleBackColor = true;
			this.bt_Up.Click += new System.EventHandler(this.click_Up);
			// 
			// rb_Substring
			// 
			this.rb_Substring.Location = new System.Drawing.Point(85, 25);
			this.rb_Substring.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Substring.Name = "rb_Substring";
			this.rb_Substring.Size = new System.Drawing.Size(85, 20);
			this.rb_Substring.TabIndex = 3;
			this.rb_Substring.Text = "substring";
			this.rb_Substring.UseVisualStyleBackColor = true;
			// 
			// rb_Wholeword
			// 
			this.rb_Wholeword.Location = new System.Drawing.Point(85, 51);
			this.rb_Wholeword.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Wholeword.Name = "rb_Wholeword";
			this.rb_Wholeword.Size = new System.Drawing.Size(85, 20);
			this.rb_Wholeword.TabIndex = 4;
			this.rb_Wholeword.Text = "wholeword";
			this.rb_Wholeword.UseVisualStyleBackColor = true;
			// 
			// SearchDialog
			// 
			this.ClientSize = new System.Drawing.Size(172, 76);
			this.Controls.Add(this.rb_Wholeword);
			this.Controls.Add(this.rb_Substring);
			this.Controls.Add(this.bt_Up);
			this.Controls.Add(this.bt_Down);
			this.Controls.Add(this.tb_Search);
			this.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = global::GeneralGFF.Properties.Resources.generalgff_32;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Search";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
