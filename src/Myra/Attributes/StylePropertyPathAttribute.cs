using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Specifies a style property path for a property or field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StylePropertyPathAttribute : Attribute
	{
		private readonly string _name;

		/// <summary>
		/// Gets the name of the style property path.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StylePropertyPathAttribute"/> class.
		/// </summary>
		/// <param name="name">The name of the style property path.</param>
		public StylePropertyPathAttribute(string name)
		{
			_name = name;
		}
	}
}