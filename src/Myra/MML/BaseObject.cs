using Myra.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.MML
{
	/// <summary>
	/// Base class for objects in the MML (Myra Markup Language) serialization system.
	/// Provides support for unique identifiers, attached properties, and custom data storage.
	/// </summary>
	public class BaseObject: IItemWithId, INotifyAttachedPropertyChanged
	{
		private string _id = null;

		/// <summary>
		/// Gets or sets the unique identifier for this object. Used for referencing in MML.
		/// </summary>
		[DefaultValue(null)]
		public string Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				OnIdChanged();
			}
		}

		/// <summary>
		/// Gets a dictionary of attached property values for this object.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public readonly Dictionary<int, object> AttachedPropertiesValues = new Dictionary<int, object>();

		/// <summary>
		/// Gets a dictionary of custom user data not mapped to the object's properties.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Dictionary<string, string> UserData { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets a dictionary of external files referenced by this object.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Dictionary<string, string> Resources { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// Occurs when the ID of this object changes.
		/// </summary>
		public event EventHandler IdChanged;

		/// <summary>
		/// Called when the ID of this object changes. Raises the IdChanged event.
		/// </summary>
		protected internal virtual void OnIdChanged()
		{
			IdChanged.Invoke(this);
		}

		/// <summary>
		/// Called when an attached property value changes. Override to handle attached property changes.
		/// </summary>
		/// <param name="propertyInfo">Information about the attached property that changed.</param>
		public virtual void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo)
		{
		}
	}
}