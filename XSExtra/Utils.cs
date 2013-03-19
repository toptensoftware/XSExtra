using System;
using Gtk;

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
	}
}

