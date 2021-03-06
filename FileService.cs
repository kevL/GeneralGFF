﻿using System;
using System.IO;


namespace generalgff
{
	/// <summary>
	/// File IO.
	/// </summary>
	static class FileService
	{
		#region Fields (static)
		internal const string EXT_T = ".t"; // a temporary file
				 const string EXT_B = ".b"; // a backup file
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Reads all the bytes of a specified file and returns it in a buffer.
		/// The file will be closed.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to read</param>
		/// <returns>an array of bytes else null</returns>
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
		/// Creates a file and returns a FileStream for writing after backing up
		/// a pre-existing file if it exists. The file will not be closed.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// @note If file exists call this only to create a file_ext_[.t]
		/// file. Then call ReplaceFile() by passing in file_ext.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be created</param>
		/// <returns>the filestream if valid else null</returns>
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
		/// Replaces a file with another file (that has a ".t" extension) after
		/// making a backup (".b") of the destination file. If the destination
		/// file does not exist, a copy-delete operation is performed instead of
		/// a backup.
		/// IMPORTANT: The source file must have the name and extension of the
		/// destination file plus the extension ".t". In other words, the
		/// standard save-procedure is to write to file_ext_[.t] then call
		/// ReplaceFile() by passing in the original file_ext.
		/// </summary>
		/// <param name="pfe">path-file-extension of the destination file</param>
		/// <returns>true if everything goes according to plan</returns>
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

				// this deletes the ".b" backup. Disable this try/catch block to keep the backup file.
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
		/// <returns></returns>
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
