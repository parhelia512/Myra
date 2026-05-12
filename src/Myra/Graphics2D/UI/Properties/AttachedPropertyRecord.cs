using Myra.MML;
using System;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// A record that provides access to an attached property on a widget.
	/// </summary>
	internal class AttachedPropertyRecord : Record
	{
		private readonly BaseAttachedPropertyInfo _property;

		/// <summary>
		/// Initializes a new instance of the AttachedPropertyRecord class.
		/// </summary>
		/// <param name="property">The attached property info to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when property is null.</exception>
		public AttachedPropertyRecord(BaseAttachedPropertyInfo property)
		{
			_property = property ?? throw new ArgumentNullException(nameof(property));
			HasSetter = true;
		}

		/// <summary>
		/// Gets the name of the attached property.
		/// </summary>
		public override string Name => _property.Name;

		/// <summary>
		/// Gets the type of the attached property.
		/// </summary>
		public override Type Type => _property.PropertyType;

		/// <summary>
		/// Gets the value of the attached property from the specified widget.
		/// </summary>
		/// <param name="obj">The widget to get the value from.</param>
		/// <returns>The property value.</returns>
		public override object GetValue(object obj) => _property.GetValueObject((Widget)obj);

		/// <summary>
		/// Sets the value of the attached property on the specified widget.
		/// </summary>
		/// <param name="obj">The widget to set the value on.</param>
		/// <param name="value">The new property value.</param>
		public override void SetValue(object obj, object value) => _property.SetValueObject((Widget)obj, value);

		/// <summary>
		/// Finds an attribute of the specified type on the attached property.
		/// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <returns>The attribute if found; otherwise null.</returns>
		public override T FindAttribute<T>()
		{
			if (_property.Attributes == null)
			{
				return null;
			}

			return (T)(from a in _property.Attributes where a.GetType() == typeof(T) select a).FirstOrDefault();
		}
	}
}
