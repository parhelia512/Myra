using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Specifies the style type name for a class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class StyleTypeNameAttribute: Attribute
	{
		private readonly string _name;

		/// <summary>
		/// Gets the name of the style type.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StyleTypeNameAttribute"/> class.
		/// </summary>
		/// <param name="name">The name of the style type.</param>
		public StyleTypeNameAttribute(string name)
		{
			_name = name;
		}
	}
}
