using System;
using System.Windows.Forms;


namespace generalgff
{
	/// <summary>
	/// Entry point.
	/// </summary>
	sealed class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			string filearg;
			if (args.Length != 0)
				filearg = args[0];
			else
				filearg = null;

			Application.Run(new GeneralGFF(filearg));
		}
	}
}
