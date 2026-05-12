using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Myra.MML
{
	/// <summary>
	/// Provides an interface for objects that can be notified when an attached property changes.
	/// </summary>
	public interface INotifyAttachedPropertyChanged
	{
		/// <summary>
		/// Called when an attached property value changes.
		/// </summary>
		/// <param name="propertyInfo">Information about the attached property that changed.</param>
		void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo);
	}

	/// <summary>
	/// Specifies how a property change affects the layout system.
	/// </summary>
	public enum AttachedPropertyOption
	{
		/// <summary>
		/// The property change does not affect the layout.
		/// </summary>
		None,

		/// <summary>
		/// The property change requires arrangement recalculation.
		/// </summary>
		AffectsArrange,

		/// <summary>
		/// The property change requires measurement recalculation.
		/// </summary>
		AffectsMeasure,
	}

	/// <summary>
	/// Base class for attached property definitions.
	/// </summary>
	public abstract class BaseAttachedPropertyInfo
	{
		/// <summary>
		/// Gets the type that owns this attached property.
		/// </summary>
		public Type OwnerType { get; private set; }

		/// <summary>
		/// Gets the unique identifier for this attached property.
		/// </summary>
		public int Id { get; private set; }

		/// <summary>
		/// Gets the name of the attached property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the option specifying how this property affects the layout system.
		/// </summary>
		public AttachedPropertyOption Option { get; private set; }

		/// <summary>
		/// Gets the attributes applied to this attached property.
		/// </summary>
		public Attribute[] Attributes { get; private set; }

		/// <summary>
		/// Gets the type of value this property holds.
		/// </summary>
		public abstract Type PropertyType { get; }

		/// <summary>
		/// Gets the default value for this property.
		/// </summary>
		public abstract object DefaultValueObject { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseAttachedPropertyInfo"/> class.
		/// </summary>
		/// <param name="id">The unique identifier for this attached property.</param>
		/// <param name="name">The name of the attached property.</param>
		/// <param name="ownerType">The type that owns this attached property.</param>
		/// <param name="option">The option specifying how this property affects the layout system.</param>
		/// <param name="attributes">Optional attributes to apply to this property.</param>
		protected BaseAttachedPropertyInfo(int id, string name, Type ownerType, AttachedPropertyOption option, Attribute[] attributes = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
			Name = name;
			Id = id;
			Option = option;
			Attributes = attributes;
		}

		/// <summary>
		/// Gets the value of this property from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the property value from.</param>
		/// <returns>The property value as an object.</returns>
		public abstract object GetValueObject(BaseObject obj);

		/// <summary>
		/// Sets the value of this property on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the property value on.</param>
		/// <param name="value">The property value to set.</param>
		public abstract void SetValueObject(BaseObject obj, object value);
	}

	/// <summary>
	/// Represents a typed attached property definition.
	/// </summary>
	/// <typeparam name="T">The type of value this property holds.</typeparam>
	public class AttachedPropertyInfo<T> : BaseAttachedPropertyInfo
	{
		/// <summary>
		/// Gets the default value for this property.
		/// </summary>
		public T DefaultValue { get; private set; }

		/// <summary>
		/// Gets the type of value this property holds.
		/// </summary>
		public override Type PropertyType => typeof(T);

		/// <summary>
		/// Gets the default value for this property as an object.
		/// </summary>
		public override object DefaultValueObject => DefaultValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="AttachedPropertyInfo{T}"/> class.
		/// </summary>
		/// <param name="id">The unique identifier for this attached property.</param>
		/// <param name="name">The name of the attached property.</param>
		/// <param name="ownerType">The type that owns this attached property.</param>
		/// <param name="defaultValue">The default value for this property.</param>
		/// <param name="option">The option specifying how this property affects the layout system.</param>
		/// <param name="attributes">Optional attributes to apply to this property.</param>
		public AttachedPropertyInfo(int id, string name, Type ownerType, T defaultValue, AttachedPropertyOption option, Attribute[] attributes = null) :
			base(id, name, ownerType, option, attributes)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the value of this property from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the property value from.</param>
		/// <returns>The property value, or the default value if not set.</returns>
		public T GetValue(BaseObject obj)
		{
			if (obj.AttachedPropertiesValues.TryGetValue(Id, out var value))
			{
				return (T)value;
			}

			return DefaultValue;
		}

		/// <summary>
		/// Sets the value of this property on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the property value on.</param>
		/// <param name="value">The property value to set.</param>
		public void SetValue(BaseObject obj, T value)
		{
			if (GetValue(obj).Equals(value))
			{
				return;
			}

			obj.AttachedPropertiesValues[Id] = value;

			var asWidget = obj as Widget;
			if (asWidget != null)
			{
				switch (Option)
				{
					case AttachedPropertyOption.None:
						break;
					case AttachedPropertyOption.AffectsArrange:
						asWidget.InvalidateArrange();
						break;
					case AttachedPropertyOption.AffectsMeasure:
						asWidget.InvalidateMeasure();
						break;
				}
			}

			obj.OnAttachedPropertyChanged(this);
		}

		/// <summary>
		/// Gets the value of this property from the specified object as an object.
		/// </summary>
		/// <param name="widget">The object to get the property value from.</param>
		/// <returns>The property value as an object.</returns>
		public override object GetValueObject(BaseObject widget) => GetValue(widget);

		/// <summary>
		/// Sets the value of this property on the specified object from an object value.
		/// </summary>
		/// <param name="widget">The object to set the property value on.</param>
		/// <param name="value">The property value to set as an object.</param>
		public override void SetValueObject(BaseObject widget, object value) => SetValue(widget, (T)value);
	}


	public static class AttachedPropertiesRegistry
	{
		private static readonly Dictionary<int, BaseAttachedPropertyInfo> _properties = new Dictionary<int, BaseAttachedPropertyInfo>();
		private static readonly Dictionary<Type, BaseAttachedPropertyInfo[]> _propertiesByType = new Dictionary<Type, BaseAttachedPropertyInfo[]>();

		public static AttachedPropertyInfo<T> Create<T>(Type type, string name, T defaultValue, AttachedPropertyOption option, Attribute[] attributes = null)
		{
			var result = new AttachedPropertyInfo<T>(_properties.Count, name, type, defaultValue, option, attributes);
			_properties[result.Id] = result;

			return result;
		}

		public static BaseAttachedPropertyInfo[] GetPropertiesOfType(Type type)
		{
			BaseAttachedPropertyInfo[] result;
			if (_propertiesByType.TryGetValue(type, out result))
			{
				return result;
			}

			var propertiesList = new List<BaseAttachedPropertyInfo>();

			// Build list of all attached properties
			var currentType = type;
			while (currentType != null && currentType != typeof(object))
			{
				// Make sure all static fields of type are initialized
				RuntimeHelpers.RunClassConstructor(currentType.TypeHandle);
				foreach (var pair in _properties)
				{
					if (pair.Value.OwnerType == currentType)
					{
						propertiesList.Add(pair.Value);
					}
				}

				currentType = currentType.BaseType;
			}

			result = propertiesList.ToArray();
			_propertiesByType[type] = result;

			return result;
		}
	}
}
