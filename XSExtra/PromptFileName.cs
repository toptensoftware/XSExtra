using System;
using MonoDevelop.Core;
using Gtk;

namespace MonoDevelop.XSExtra
{
	public partial class PromptFileName : Gtk.Dialog
	{
		public PromptFileName()
		{
			this.Build();
		}

		FilePath _originalFile;
		public FilePath OriginalFile
		{
			get
			{
				return _originalFile;
			}
			set
			{
				_originalFile = value;
			}
		}

		public string NewBaseName
		{
			get;
			set;
		}

		public string NewFileName
		{
			get;
			set;
		}

		public bool SearchAndReplace
		{
			get
			{
				return _searchAndReplace.Active;
			}
			set
			{
				_searchAndReplace.Active = value;
			}
		}

		public new ResponseType Run()
		{
			_newFileName.Text = _originalFile.FileNameWithoutExtension;
			_newFileName.SelectRegion(0, _newFileName.Text.Length);

			return (ResponseType)base.Run();
		}


		protected void OnOK(object sender, EventArgs e)
		{
			// Check if the new filename already exists
			string newFileName = _originalFile.ParentDirectory.Combine(_newFileName.Text) + _originalFile.Extension;

			if (string.IsNullOrEmpty(newFileName))
			{
				Utils.MessageBox(this, 
				                 "Please specify a new name",
				                 "Duplicate File",
				                 MessageType.Info,
				                 ButtonsType.Ok);
			}

			if (System.IO.File.Exists(newFileName))
			{
				Utils.MessageBox(this, 
								string.Format("{0}\n\nFile already exists", newFileName),
				                "Duplicate File",
				                 MessageType.Info,
				                 ButtonsType.Ok);
				return;
			}

			NewBaseName = _newFileName.Text;
			NewFileName = newFileName;
			Respond(ResponseType.Ok);
		}
	}
}

