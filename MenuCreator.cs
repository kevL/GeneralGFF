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

		internal const int MI_FILE_CRAT   = 0;
		// 1 is Separator
		internal const int MI_FILE_OPEN   = 2;
		internal const int MI_FILE_RLOD   = 3;
		internal const int MI_FILE_SAVE   = 4;
		internal const int MI_FILE_SAVS   = 5;
		// 6 is Separator
		internal const int MI_FILE_QUIT   = 7;

		internal const int MI_EDIT_SEARCH = 0;

		internal const int MI_VIEW_EXPAND = 0;
		internal const int MI_VIEW_COLLAP = 1;
		// 2 is Separator
		internal const int MI_VIEW_SORTER = 3;

		internal const int MI_HELP_ABOUT  = 0;
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
			Xenu.MenuItems[MI_EDIT].MenuItems[MI_EDIT_SEARCH].Shortcut = Shortcut.CtrlF;


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
