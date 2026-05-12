using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// A record that provides access to an object's property via reflection.
	/// </summary>
	internal class PropertyRecord : ReflectionRecord
	{
		private readonly PropertyInfo _propertyInfo;

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		public override string Name
		{
			get { return _propertyInfo.Name; }
		}

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		public override Type Type
		{
			get { return _propertyInfo.PropertyType; }
		}

		/// <summary>
		/// Gets the underlying PropertyInfo for this record.
		/// </summary>
		public override MemberInfo MemberInfo => _propertyInfo;

		/// <summary>
		/// Initializes a new instance of the PropertyRecord class.
		/// </summary>
		/// <param name="propertyInfo">The PropertyInfo to wrap.</param>
		public PropertyRecord(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		/// <summary>
		/// Gets the value of the property from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the value from.</param>
		/// <returns>The property value.</returns>
		public override object GetValue(object obj)
		{
			return _propertyInfo.GetValue(obj, new object[0]);
		}

		/// <summary>
		/// Sets the value of the property on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the value on.</param>
		/// <param name="value">The new property value.</param>
		public override void SetValue(object obj, object value)
		{
			_propertyInfo.SetValue(obj, value);
		}
	}
}
