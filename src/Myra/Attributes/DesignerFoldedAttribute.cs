using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a property to be displayed in a folded state in the designer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DesignerFoldedAttribute : Attribute
	{
	}
}
