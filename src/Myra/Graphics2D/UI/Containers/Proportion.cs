using Myra.Utility;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies how a widget should be sized relative to available space in a layout.
	/// </summary>
	public enum ProportionType
	{
		/// <summary>
		/// Size based on content (minimum size).
		/// </summary>
		Auto,

		/// <summary>
		/// Size based on a portion of remaining space.
		/// </summary>
		Part,

		/// <summary>
		/// Fill all available space.
		/// </summary>
		Fill,

		/// <summary>
		/// Size based on exact pixel count.
		/// </summary>
		Pixels
	}

	/// <summary>
	/// Specifies sizing proportions for widgets in layout containers.
	/// </summary>
	public class Proportion
	{
		/// <summary>
		/// A read-only proportion with Auto type.
		/// </summary>
		public static readonly Proportion Auto = new Proportion(ProportionType.Auto);

		/// <summary>
		/// A read-only proportion with Fill type.
		/// </summary>
		public static readonly Proportion Fill = new Proportion(ProportionType.Fill);

		/// <summary>
		/// The default proportion for Grid layout.
		/// </summary>
		public static readonly Proportion GridDefault = new Proportion(ProportionType.Part, 1.0f);

		/// <summary>
		/// The default proportion for StackPanel layout.
		/// </summary>
		public static readonly Proportion StackPanelDefault = new Proportion(ProportionType.Auto);

		private ProportionType _type;
		private float _value = 1.0f;

		/// <summary>
		/// Gets or sets the type of proportion.
		/// </summary>
		public ProportionType Type
		{
			get { return _type; }

			set
			{
				if (value == _type) return;
				_type = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets the proportion value.
		/// </summary>
		[DefaultValue(1.0f)]
		public float Value
		{
			get { return _value; }
			set
			{
				if (value.EpsilonEquals(_value))
				{
					return;
				}

				_value = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Raised when the proportion type or value changes.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with Auto type.
		/// </summary>
		public Proportion()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with the specified type.
		/// </summary>
		/// <param name="type">The proportion type.</param>
		public Proportion(ProportionType type)
		{
			_type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Proportion"/> class with the specified type and value.
		/// </summary>
		/// <param name="type">The proportion type.</param>
		/// <param name="value">The proportion value.</param>
		public Proportion(ProportionType type, float value)
			: this(type)
		{
			_value = value;
		}

		/// <summary>
		/// Returns a string representation of this proportion.
		/// </summary>
		/// <returns>A string representation of the proportion.</returns>
		public override string ToString()
		{
			if (_type == ProportionType.Auto || _type == ProportionType.Fill)
			{
				return _type.ToString();
			}

			if (_type == ProportionType.Part)
			{
				return string.Format("{0}: {1:0.00}", _type, _value);
			}

			// Pixels
			return string.Format("{0}: {1}", _type, (int)_value);
		}

		private void FireChanged()
		{
			var ev = Changed;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
