using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Represents the thickness (margins) of a rectangle, with values for each edge.
	/// </summary>
	public struct Thickness
	{
		/// <summary>
		/// A thickness with all values set to zero.
		/// </summary>
		public static readonly Thickness Zero = new Thickness();

		/// <summary>
		/// Gets or sets the left thickness value.
		/// </summary>
		[Range(0)]
		public int Left { get; set; }

		/// <summary>
		/// Gets or sets the right thickness value.
		/// </summary>
		[Range(0)]
		public int Right { get; set; }

		/// <summary>
		/// Gets or sets the top thickness value.
		/// </summary>
		[Range(0)]
		public int Top { get; set; }

		/// <summary>
		/// Gets or sets the bottom thickness value.
		/// </summary>
		[Range(0)]
		public int Bottom { get; set; }

		/// <summary>
		/// Gets the combined horizontal thickness (left + right).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Width
		{
			get
			{
				return Left + Right;
			}
		}

		/// <summary>
		/// Gets the combined vertical thickness (top + bottom).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Height
		{
			get
			{
				return Top + Bottom;
			}
		}

		/// <summary>
		/// Gets a value indicating whether all sides have the same thickness.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool SameSize
		{
			get
			{
				return (Left == Top && Top == Right && Right == Bottom);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with individual values for each edge.
		/// </summary>
		/// <param name="left">The left thickness value.</param>
		/// <param name="top">The top thickness value.</param>
		/// <param name="right">The right thickness value.</param>
		/// <param name="bottom">The bottom thickness value.</param>
		public Thickness(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with separate horizontal and vertical values.
		/// </summary>
		/// <param name="horizontalValue">The value for left and right edges.</param>
		/// <param name="verticalValue">The value for top and bottom edges.</param>
		public Thickness(int horizontalValue, int verticalValue) : this(horizontalValue, verticalValue, horizontalValue, verticalValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thickness"/> struct with the same value for all edges.
		/// </summary>
		/// <param name="value">The value for all edges.</param>
		public Thickness(int value) : this(value, value, value, value)
		{
		}

		/// <summary>
		/// Converts the thickness to a string representation.
		/// </summary>
		/// <returns>A string representation of the thickness.</returns>
		public override string ToString()
		{
			if (SameSize)
			{
				return Left.ToString();
			}

			if (Left == Right && Top == Bottom)
			{
				return string.Format("{0}, {1}", Left, Top);
			}

			return string.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
		}

		/// <summary>
		/// Parses a string representation into a <see cref="Thickness"/> struct.
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <returns>A new <see cref="Thickness"/> instance parsed from the string.</returns>
		public static Thickness FromString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return Zero;
			}

			var parts = (from p in s.Split(',') select p.Trim()).ToArray();
			if (parts.Length != 1 && parts.Length != 2 && parts.Length != 4)
			{
				throw new ArgumentException(string.Format("Could not convert string '{0}' to Thickness", s));
			}

			if (parts.Length == 1)
			{
				return new Thickness(int.Parse(parts[0]));
			}

			if (parts.Length == 2)
			{
				return new Thickness(int.Parse(parts[0]), int.Parse(parts[1]));
			}

			return new Thickness(int.Parse(parts[0]),
				int.Parse(parts[1]),
				int.Parse(parts[2]),
				int.Parse(parts[3]));
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current thickness.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>true if the object is equal to the current thickness; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Thickness))
			{
				return false;
			}

			var thickness = (Thickness)obj;
			return Left == thickness.Left &&
				   Right == thickness.Right &&
				   Top == thickness.Top &&
				   Bottom == thickness.Bottom;
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current thickness.</returns>
		public override int GetHashCode()
		{
			var hashCode = 551583723;
			hashCode = hashCode * -1521134295 + Left.GetHashCode();
			hashCode = hashCode * -1521134295 + Right.GetHashCode();
			hashCode = hashCode * -1521134295 + Top.GetHashCode();
			hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Determines whether two specified thicknesses are equal.
		/// </summary>
		/// <param name="a">The first thickness to compare.</param>
		/// <param name="b">The second thickness to compare.</param>
		/// <returns>true if the thicknesses are equal; otherwise, false.</returns>
		public static bool operator ==(Thickness a, Thickness b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Determines whether two specified thicknesses are not equal.
		/// </summary>
		/// <param name="a">The first thickness to compare.</param>
		/// <param name="b">The second thickness to compare.</param>
		/// <returns>true if the thicknesses are not equal; otherwise, false.</returns>
		public static bool operator !=(Thickness a, Thickness b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Subtracts a thickness from a rectangle, reducing the rectangle's size.
		/// </summary>
		/// <param name="a">The rectangle to reduce.</param>
		/// <param name="b">The thickness to subtract.</param>
		/// <returns>A new rectangle with the thickness subtracted from all sides.</returns>
		public static Rectangle operator -(Rectangle a, Thickness b)
		{
			var result = a;
			result.X += b.Left;
			result.Y += b.Top;

			result.Width -= b.Width;
			if (result.Width < 0)
			{
				result.Width = 0;
			}

			result.Height -= b.Height;
			if (result.Height < 0)
			{
				result.Height = 0;
			}

			return result;
		}
	}
}