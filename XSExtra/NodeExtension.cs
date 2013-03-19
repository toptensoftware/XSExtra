using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;
using Gdk;

namespace MonoDevelop.XSExtra
{
	public class NodeExtension : NodeBuilderExtension
	{
		public NodeExtension()
		{
		}

		#region implemented abstract members of NodeBuilderExtension

		public override bool CanBuildNode(Type dataType)
		{
			return typeof(ProjectFile).IsAssignableFrom(dataType) || 
					typeof(Project).IsAssignableFrom(dataType) ||
					typeof(Solution).IsAssignableFrom(dataType);
		}

		public override Type CommandHandlerType 
		{
			get 
			{ 
				return typeof(AddinCommandHandler); 
			}
		}

		#endregion
	}
}

