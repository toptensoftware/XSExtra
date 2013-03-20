using System;
using Gtk;
using System.Collections.Generic;

namespace MonoDevelop.XSExtra
{
	public static class Utils
	{
		public static ResponseType MessageBox(Window parent, string message, string title, MessageType messageType, ButtonsType buttonsType)
		{
			var md = new MessageDialog(parent, 
			                           DialogFlags.DestroyWithParent | DialogFlags.Modal,
			                           messageType,
			                           buttonsType,
			                           message);
			md.Title=title;
			
			try
			{
				return (ResponseType)md.Run();
			}
			finally
			{
				md.Destroy();
			}
		}

		public static void GetFilesRecursively(string folder, List<string> files)
		{
			// Add files in this folder
			files.AddRange(System.IO.Directory.GetFiles(folder));

			// Recurse sub-folders
			foreach (var subfolder in System.IO.Directory.GetDirectories(folder))
			{
				GetFilesRecursively(subfolder, files);
			}
		}

		public static void GetFoldersRecursively(string folder, List<string> folders)
		{
			// Add this folder
			folders.Add(folder);

			// Recurse sub-folders
			foreach (var subfolder in System.IO.Directory.GetDirectories(folder))
			{
				GetFoldersRecursively(subfolder, folders);
			}
		}
	}
}

