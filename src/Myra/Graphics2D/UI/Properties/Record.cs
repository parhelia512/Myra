using System;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Abstract base class for property records that provide access to object properties and fields.
	/// </summary>
	public abstract class Record
	{
		/// <summary>
		/// Gets or sets a value indicating whether this record supports setting values.
		/// </summary>
		public bool HasSetter { get; set; }

		/// <summary>
		/// Gets the name of the property or field.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the type of the property or field.
		/// </summary>
		public abstract Type Type { get; }

		/// <summary>
		/// Gets or sets the category name for this record, used for grouping in property displays.
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets the value of the property or field from the specified object.
		/// </summary>
		/// <param name="obj">The object to retrieve the value from.</param>
		/// <returns>The value of the property or field.</returns>
		public abstract object GetValue(object obj);

		/// <summary>
		/// Sets the value of the property or field on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the value on.</param>
		/// <param name="value">The new value to set.</param>
		public abstract void SetValue(object obj, object value);

		/// <summary>
		/// Finds an attribute of the specified type on this record's property or field.
		/// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <returns>The attribute if found; otherwise null.</returns>
		public abstract T FindAttribute<T>() where T : Attribute;
	}
}
