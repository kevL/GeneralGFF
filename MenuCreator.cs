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
		internal const int MI_EXTS = 3;
		internal const int MI_HELP = 4;

		internal const int MI_FILE_CRAT = 0; // create
		// 1 is Separator
		internal const int MI_FILE_OPEN = 2; // open
		internal const int MI_FILE_SAVE = 3; // save
		internal const int MI_FILE_SAVA = 4; // saveas
		// 5 is Separator
		internal const int MI_FILE_RELD = 6; // reload
		internal const int MI_FILE_EXPT = 7; // export
		// 8 is Separator
		internal const int MI_FILE_QUIT = 9; // quit

		internal const int MI_EDIT_SER  = 0; // search
		// 1 is Separator
		internal const int MI_EDIT_CUT  = 2; // cut
		internal const int MI_EDIT_COP  = 3; // copy
		internal const int MI_EDIT_PAS  = 4; // paste
		internal const int MI_EDIT_DEL  = 5; // delete

		internal const int MI_VIEW_EXPD = 0; // expand
		internal const int MI_VIEW_COLP = 1; // collapse
		// 2 is Separator
		internal const int MI_VIEW_SORT = 3; // sort

		internal const int MI_EXTS_EXT  = 0; // enable extension

		internal const int MI_HELP_ABT  = 0; // about
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

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Save GFF file");		// #3
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVE].Shortcut = Shortcut.CtrlS;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("Sav&e GFF file As ...");	// #4
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_SAVA].Shortcut = Shortcut.CtrlE;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("-");						// #5

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Reload GFF file");		// #6
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_RELD].Shortcut = Shortcut.CtrlR;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("Ex&port GFF file ...");	// #7
			Xenu.MenuItems[MI_FILE].MenuItems[MI_FILE_EXPT].Shortcut = Shortcut.CtrlP;

			Xenu.MenuItems[MI_FILE].MenuItems.Add("-");						// #8

			Xenu.MenuItems[MI_FILE].MenuItems.Add("&Quit");					// #9
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
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_EXPD].Shortcut = Shortcut.F5;

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("&Collapse all under selected");	// #1
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_COLP].Shortcut = Shortcut.F6;

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("-");								// #2

			Xenu.MenuItems[MI_VIEW].MenuItems.Add("&Sort");							// #3
			Xenu.MenuItems[MI_VIEW].MenuItems[MI_VIEW_SORT].Shortcut = Shortcut.F7;


			Xenu.MenuItems.Add("E&xtension"); // #3
			Xenu.MenuItems[MI_EXTS].Visible =
			Xenu.MenuItems[MI_EXTS].Enabled = false; // DISABLE Creature Extension and its shortcut

			Xenu.MenuItems[MI_EXTS].MenuItems.Add("&Enable"); // #0
//			Xenu.MenuItems[MI_EXTS].MenuItems[MI_EXTS_EXT].Shortcut = Shortcut.F8;

//			Xenu.MenuItems[MI_EXTS].MenuItems.Add("&Creature Visualizer"); // #1
//			Xenu.MenuItems[MI_EXTS].MenuItems[1].Shortcut = Shortcut.F8;


			Xenu.MenuItems.Add("&Help"); // #4

//			Xenu.MenuItems[MI_HELP].MenuItems.Add("&Help");		// #

			Xenu.MenuItems[MI_HELP].MenuItems.Add("&About");	// #0
			Xenu.MenuItems[MI_HELP].MenuItems[MI_HELP_ABT].Shortcut = Shortcut.F2;

			return Xenu;
		}
		#endregion Methods (static)
	}
}
