using System;
using System.IO;


namespace generalgff
{
	/// <summary>
	/// File IO.
	/// </summary>
	static class FileService
	{
		#region Fields (static)
		// Note: .NET intermittently bugs out if these suffixes are appended as
		// extensions; it can cause the real extension to be duplicated in the
		// final file+extension -> file+extension+extension. The bug is not
		// reliably reproducible and happens independently of a SaveFileDialog's
		// DefaultExt/AddExtension properties; in fact it can happen when a Save
		// is done without a dialog (can't recall for sure).
		// See: SaveFileDialog extension added unexpectedly on multidotExtension
		// https://stackoverflow.com/questions/30747664/savefiledialog-extension-added-unexpectedly-on-multidotextension
		// I did not test the SupportMultiDottedExtensions property but it's
		// unlikely to alleviate the issue since it can occur even without a
		// dialog (I think).
		internal const string EXT_T = "t"; // a temporary file
				 const string EXT_B = "b"; // a backup file
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Reads all the bytes of a specified file and returns it in a buffer.
		/// The file will be closed.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to read</param>
		/// <returns>an array of <c>bytes</c> else null</returns>
		public static byte[] ReadFile(string pfe)
		{
			byte[] bytes = null;

			if (File.Exists(pfe))
			{
				try
				{
					bytes = File.ReadAllBytes(pfe);
				}
				catch (Exception ex)
				{
					error("File could not be read."
						  + Environment.NewLine + Environment.NewLine
						  + pfe
						  + Environment.NewLine + Environment.NewLine
						  + ex);
					return null;
				}
			}
			else
				error("File does not exist." + Environment.NewLine + Environment.NewLine + pfe);

			return bytes;
		}

		/// <summary>
		/// Creates a file and returns a <c>FileStream</c> for writing after
		/// backing up a pre-existing file if it exists. The file will not be
		/// closed.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// @note If file exists call this only to create a file+ext+[t] file.
		/// Then call <c><see cref="ReplaceFile()">ReplaceFile()</see></c> by
		/// passing in file+ext.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be created</param>
		/// <returns>the <c>FileStream</c> if valid else <c>null</c></returns>
		internal static FileStream CreateFile(string pfe)
		{
			FileStream fs = null;

			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(pfe));
				fs = File.Create(pfe);
			}
			catch (Exception ex)
			{
				error("File could not be created."
					  + Environment.NewLine + Environment.NewLine
					  + pfe
					  + Environment.NewLine + Environment.NewLine
					  + ex);
				return null;
			}
			return fs;
		}

		/// <summary>
		/// Replaces a file with another file (that has a "t" extension) after
		/// making a backup ("b") of the destination file. If the destination
		/// file does not exist, a copy-delete operation is performed instead of
		/// a backup.
		/// IMPORTANT: The source file must have the name and extension of the
		/// destination file plus the extension "t". In other words, the
		/// standard save-procedure is to write to file+ext+[t] then call
		/// <c><see cref="ReplaceFile()">ReplaceFile()</see></c> by passing in
		/// the original file+ext.
		/// </summary>
		/// <param name="pfe">path-file-extension of the destination file</param>
		/// <returns><c>true</c> if everything goes according to plan</returns>
		internal static bool ReplaceFile(string pfe)
		{
			if (File.Exists(pfe))
			{
				string pfeBackup = pfe + EXT_B;

				if (File.Exists(pfeBackup))
				{
					try
					{
						File.Delete(pfeBackup);
					}
					catch (Exception ex)
					{
						error("File backup could not be deleted."
							  + Environment.NewLine + Environment.NewLine
							  + pfeBackup
							  + Environment.NewLine + Environment.NewLine
							  + ex);
						return false;
					}
				}

				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(pfeBackup));
					File.Replace(
							pfe + EXT_T,
							pfe,
							pfeBackup,
							true);
				}
				catch (Exception ex)
				{
					error("File could not be replaced."
						  + Environment.NewLine + Environment.NewLine
						  + "src : " + pfe + EXT_T
						  + Environment.NewLine
						  + "dst : " + pfe
						  + Environment.NewLine + Environment.NewLine
						  + ex);
					return false;
				}

				// this deletes the "b" backup. Disable this try/catch block to keep the backup file.
				try
				{
					File.Delete(pfeBackup);
				}
				catch (Exception ex)
				{
					error("File could not be deleted."
						  + Environment.NewLine + Environment.NewLine
						  + pfeBackup
						  + Environment.NewLine + Environment.NewLine
						  + ex);
					return false;
				}
			}
			else
				MoveFile(pfe + EXT_T, pfe);

			return true;
		}

		/// <summary>
		/// Moves a file by copying it to another location before deleting the
		/// old file.
		/// @note Ensure that the destination file doesn't already exist.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		/// <returns><c>true</c> if everything goes according to plan</returns>
		internal static bool MoveFile(string src, string dst)
		{
			try
			{
				File.Copy(src, dst);
			}
			catch (Exception ex)
			{
				error("File could not be copied."
					  + Environment.NewLine + Environment.NewLine
					  + "src : " + src
					  + Environment.NewLine
					  + "dst : " + dst
					  + Environment.NewLine + Environment.NewLine
					  + ex);
				return false;
			}

			try
			{
				File.Delete(src);
			}
			catch (Exception ex)
			{
				error("File could not be deleted."
					  + Environment.NewLine + Environment.NewLine
					  + src
					  + Environment.NewLine + Environment.NewLine
					  + ex);
				return false;
			}
			return true;
		}


		/// <summary>
		/// A generic error dialog.
		/// </summary>
		/// <param name="er"></param>
		internal static void error(string er)
		{
			using (var f = new InfoDialog(Globals.Error, er))
				f.ShowDialog();
		}
		#endregion Methods (static)
	}
}
