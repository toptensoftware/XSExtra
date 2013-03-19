using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Projects;
using Gtk;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Desktop;

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
	}
}

