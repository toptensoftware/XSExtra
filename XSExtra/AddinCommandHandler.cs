using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using Gtk;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Desktop;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using System.IO;
using System.Collections.Generic;
using MonoDevelop.Core;

namespace MonoDevelop.XSExtra
{
	public class AddinCommandHandler : NodeCommandHandler
	{
		public AddinCommandHandler()
		{
		}

		[CommandHandler(MonoDevelop.XSExtra.Commands.DuplicateFile)]
		protected void OnDuplicateFile()
		{
			// Get the current project/file
			var file = this.CurrentNode.DataItem as ProjectFile;
			var project = file.Project;

			var mimeType = DesktopService.GetMimeTypeForUri(file.Name);

			var prompt = new PromptFileName();
			prompt.OriginalFile = file.FilePath;
			prompt.SearchAndReplace = DesktopService.GetMimeTypeIsText(mimeType);
			var result = prompt.Run();
			prompt.Destroy();

			if (result != ResponseType.Ok)
				return;

			// Generate the new file content
			try
			{
				if (prompt.SearchAndReplace)
				{
					string content = System.IO.File.ReadAllText(file.FilePath.ToString());
					content = content.Replace(file.FilePath.FileNameWithoutExtension, prompt.NewBaseName);
					System.IO.File.WriteAllText(prompt.NewFileName, content);
				}
				else
				{
					System.IO.File.Copy(file.FilePath.ToString(), prompt.NewFileName);
				}
			}
			catch (Exception x)
			{
				Utils.MessageBox(null, string.Format("{0}\n\nFailed to create file - {1}", prompt.NewFileName,  x.Message), "Duplicate File", MessageType.Error, ButtonsType.Ok);
				return;
			}

			// Create a new projecte ntry
			var file2 = (ProjectFile)file.Clone();
			file2.Name = prompt.NewFileName;
			project.AddFile(file2);
			MonoDevelop.Ide.IdeApp.ProjectOperations.Save(project);

			// Open the file
			IdeApp.Workbench.OpenDocument (file2.FilePath);
		}

		[CommandHandler(MonoDevelop.XSExtra.Commands.EditAsText)]
		protected void OnEditAsText()
		{
			string filename=null;

			// Is it a solution?
			var sln = CurrentNode.DataItem as Solution;
			if (sln!=null)
				filename=sln.FileName;
			else 
			{
				// Is it a project?
				var proj = CurrentNode.DataItem as Project;
				if (proj!=null)
					filename = proj.FileName;
			}

			if (filename!=null)
			{
				IdeApp.Workbench.OpenDocument(filename, MonoDevelop.Ide.Gui.OpenDocumentOptions.OnlyInternalViewer | MonoDevelop.Ide.Gui.OpenDocumentOptions.BringToFront);
			}
		}

		[CommandHandler(MonoDevelop.XSExtra.Commands.SyncWithFileSystem)]
		protected void OnSyncWithFileSystem()
		{
			var folder = CurrentNode.DataItem as ProjectFolder;
			var project = folder.Project;

			LoggingService.LogInfo("Synchronizing folder {0}...", folder.Path.ToString());


			// Get the base folder
			var path = folder.Path + Path.DirectorySeparatorChar;

			// Build a list of all files under this folder in the project
			var allProjectFiles = new Dictionary<string, ProjectFile>();
			foreach (var file in folder.Project.Files)
			{
				// Ignore auto generated files and linked files
				if (file.DependsOnFile!=null || file.IsLink)
					continue;
				
				// If the file in the folder?
				if (!file.FilePath.ToString().StartsWith(path))
					continue;

				if (file.Subtype==Subtype.Code)
					allProjectFiles.Add(file.FilePath.ToString(), file);
			}

			// Now build a list of a files from the file system
			var allFiles = new List<string>();
			Utils.GetFilesRecursively(path, allFiles);

			// Create project file entries for all missing files
			int countAdded = 0;
			foreach (var f in allFiles)
			{
				// Do we already have this file?
				if (allProjectFiles.ContainsKey(f))
				{
					// Yes, remove from list of files to be removed
					allProjectFiles.Remove(f);
					continue;
				}

				// No, we need to add this one
				var pf = new ProjectFile(f);
				project.AddFile(f);

				LoggingService.LogInfo(" - adding {0}", f);
				countAdded++;
			}

			// Remove missing files
			int countRemoved = allProjectFiles.Values.Count;
			foreach (var f in allProjectFiles.Values)
				LoggingService.LogInfo(" - removing {0}", f);
			project.Items.RemoveRange(allProjectFiles.Values);


			// Build a list of all folders under this folder in the project
			var allProjectFolders = new Dictionary<string, ProjectFile>();
			foreach (var file in folder.Project.Files)
			{
				// Only interested in folders
				if (file.Subtype != Subtype.Directory)
					continue;

				// If the file in the folder?
				var folderPath = file.FilePath.ToString() + Path.DirectorySeparatorChar;
				if (!folderPath.StartsWith(path))
					continue;

				// Store it
				allProjectFolders[folderPath] = file;
			}

			// Now build a list of all file system folders
			var allFolders = new List<string>();
			Utils.GetFoldersRecursively(path, allFolders);

			// Add any that are not in the project
			foreach (var f in allFolders)
			{
				if (!allProjectFolders.ContainsKey(f))
				{
					var pf = new ProjectFile(f);
					pf.Subtype = Subtype.Directory;
					project.AddFile(pf);
				}
			}

			// Save project and we're done!
			MonoDevelop.Ide.IdeApp.ProjectOperations.Save(project);

			// Display info
			string message;
			if (countRemoved==0 && countAdded==0)
			{
				message = "No changes detected.";
			}
			else if (countRemoved != 0 && countAdded != 0)
			{
				message = string.Format("Added {0} file{1} and removed {2} file{3}.",
					countAdded, countAdded == 1 ? "" : "s",
					countRemoved, countRemoved == 1 ? "" : "s"
					);
			}
			else if (countRemoved != 0)
			{
				message = string.Format("Removed {0} file{1}.",
					countRemoved, countRemoved == 1 ? "" : "s"
					);
			}
			else
			{
				message = string.Format("Added {0} file{1}.",
					countAdded, countAdded == 1 ? "" : "s"
					);
			}

			LoggingService.LogInfo(string.Format("Finished - {0}", message));

			Utils.MessageBox(Window.ListToplevels()[0], message, "Synchronize Folder", MessageType.Info, ButtonsType.Ok);
		}


	}
}


