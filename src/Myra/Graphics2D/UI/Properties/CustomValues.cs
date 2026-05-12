using System;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Represents a custom value with a display name and associated value.
	/// </summary>
	public class CustomValue
	{
		/// <summary>
		/// Gets the display name for this custom value.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the associated value.
		/// </summary>
		public object Value { get; }

		/// <summary>
		/// Initializes a new instance of the CustomValue class.
		/// </summary>
		/// <param name="name">The display name.</param>
		/// <param name="value">The value.</param>
		public CustomValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	/// <summary>
	/// A collection of custom values with selection support.
	/// </summary>
	public class CustomValues
	{
		/// <summary>
		/// Gets the array of custom values.
		/// </summary>
		public CustomValue[] Values { get; }

		/// <summary>
		/// Gets or sets the index of the selected value.
		/// </summary>
		public int? SelectedIndex { get; set; }

		/// <summary>
		/// Initializes a new instance of the CustomValues class.
		/// </summary>
		/// <param name="values">An enumerable collection of custom values.</param>
		/// <exception cref="ArgumentNullException">Thrown when values is null.</exception>
		public CustomValues(IEnumerable<CustomValue> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			Values = values.ToArray();
		}
	}
}
