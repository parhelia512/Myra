using System;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// A record that provides access to an object's field via reflection.
	/// </summary>
	internal class FieldRecord : ReflectionRecord
	{
		private readonly FieldInfo _fieldInfo;

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		public override string Name
		{
			get { return _fieldInfo.Name; }
		}

		/// <summary>
		/// Gets the type of the field.
		/// </summary>
		public override Type Type
		{
			get { return _fieldInfo.FieldType; }
		}

		/// <summary>
		/// Gets the underlying FieldInfo for this record.
		/// </summary>
		public override MemberInfo MemberInfo => _fieldInfo;

		/// <summary>
		/// Initializes a new instance of the FieldRecord class.
		/// </summary>
		/// <param name="fieldInfo">The FieldInfo to wrap.</param>
		public FieldRecord(FieldInfo fieldInfo)
		{
			_fieldInfo = fieldInfo;
		}

		/// <summary>
		/// Gets the value of the field from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the value from.</param>
		/// <returns>The field value.</returns>
		public override object GetValue(object obj)
		{
			return _fieldInfo.GetValue(obj);
		}

		/// <summary>
		/// Sets the value of the field on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the value on.</param>
		/// <param name="value">The new field value.</param>
		public override void SetValue(object obj, object value)
		{
			_fieldInfo.SetValue(obj, value);
		}
	}
}
