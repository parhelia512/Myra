using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a property with a numeric range constraint.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RangeAttribute : Attribute
	{
		/// <summary>
		/// Gets the minimum value of the range, or null if no minimum is set.
		/// </summary>
		public float? Minimum { get; }

		/// <summary>
		/// Gets the maximum value of the range, or null if no maximum is set.
		/// </summary>
		public float? Maximum { get; }

		private RangeAttribute(float? min, float? max)
		{
			if (min != null && max != null && min > max)
			{
				throw new ArgumentException("min > max");
			}

			Minimum = min;
			Maximum = max;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeAttribute"/> class with a minimum value.
		/// </summary>
		/// <param name="min">The minimum value of the range.</param>
		public RangeAttribute(float min) : this(min, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeAttribute"/> class with minimum and maximum values.
		/// </summary>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		public RangeAttribute(float min, float max) : this((float?)min, (float?)max)
		{
		}
	}
}
