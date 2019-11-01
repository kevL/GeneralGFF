using System;
using System.Windows.Forms;


namespace generalgff
{
	static class MenuCreator
	{
		#region Fields (static)
		internal const int MI_FILE = 0;
		internal const int MI_EDIT = 1;
		internal const int MI_VIEW = 2;
		internal const int MI_HELP = 3;

		internal const int MI_FILE_CRAT   = 0; // create
		// 1 is Separator
		internal const int MI_FILE_OPEN   = 2; // open
		internal const int MI_FILE_RLOD   = 3; // reload
		internal const int MI_FILE_SAVE   = 4; // save
		internal const int MI_FILE_SAVS   = 5; // saveas
		// 6 is Separator
		internal const int MI_FILE_QUIT   = 7; // quit

		internal const int MI_EDIT_SER = 0; // search
		// 1 is Separator
		internal const int MI_EDIT_CUT = 2; // cut
		internal const int MI_EDIT_COP = 3; // copy
		internal const int MI_EDIT_PAS = 4; // paste
		internal const int MI_EDIT_DEL = 5; // delete

		internal const int MI_VIEW_EXPAND = 0; // expand
		internal const int MI_VIEW_COLLAP = 1; // collapse
		// 2 is Separator
		internal const int MI_VIEW_SORTER = 3; // sort

		internal const int MI_HELP_ABOUT  = 0; // about
		#endregion Fields (static)


		#region Methods (static)
		internal static MainMenu Create()
		{
			var Xenu = new MainMenu();

			Xenu.MenuItems.Add("&File"); // #0

			Xenu.MenuItems[MI_FILE].MenuItems.Add("Crea&te GFF file ...");	// #0
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_CRAT].Shortcut = Shortcut.CtrlT;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("-");						// #1

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Open GFF file ...");	// #2
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_OPEN].Shortcut = Shortcut.CtrlO;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Reload GFF file");		// #3
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_RLOD].Shortcut = Shortcut.CtrlR;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Save GFF file");		// #4
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVE].Shortcut = Shortcut.CtrlS;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("Sav&e GFF file As ...");	// #5
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVS].Shortcut = Shortcut.CtrlE;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("-");						// #6

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Quit");					// #7
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_QUIT].Shortcut = Shortcut.CtrlQ;


			Xenu.MenuItems.Add("&Edit"); // #1

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("&Search");	// #0
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_SER].Shortcut = Shortcut.CtrlF;

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("-");			// #1

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("&Cut");		// #2
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_CUT].Shortcut = Shortcut.CtrlX;

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("C&opy");		// #3
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_COP].Shortcut = Shortcut.CtrlC;

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("&Paste");	// #4
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_PAS].Shortcut = Shortcut.CtrlV;

			Xenu.MenuItems[MI_EDIT].MenuItems.Add("&Delete");	// #5
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_DEL].Shortcut = Shortcut.Del;


			Xenu.MenuItems.Add("&View"); // #2

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("&Expand all under selected");	// #0
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPAND].Shortcut = Shortcut.F5;

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("&Collapse all under selected");	// #1
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLLAP].Shortcut = Shortcut.F6;

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("-");								// #2

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("&Sort");							// #3
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_SORTER].Shortcut = Shortcut.F7;


			Xenu.MenuItems.Add("&Help"); // #3

//			Xenu.MenuItems[MI_HELP].MenuItems.Add("&Help");		// #

			Xenu.MenuItems[MI_HELP].MenuItems.Add("&About");	// #0
			Xenu.MenuItems[MI_HELP].MenuItems[MI_HELP_ABOUT].Shortcut = Shortcut.F2;

			return Xenu;
		}
		#endregion Methods (static)
	}
}
