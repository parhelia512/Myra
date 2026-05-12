using System;
using System.Collections.Generic;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Extension methods for style dictionaries.
	/// </summary>
	public static class StylesheetExtensions
	{
		/// <summary>
		/// Safely retrieves a style from a style dictionary, throwing an exception if not found.
		/// </summary>
		/// <typeparam name="T">The type of style to retrieve.</typeparam>
		/// <param name="styles">The style dictionary.</param>
		/// <param name="id">The identifier of the style to retrieve.</param>
		/// <returns>The style with the specified identifier.</returns>
		/// <exception cref="Exception">Thrown when the style with the specified id is not found.</exception>
		public static T SafelyGetStyle<T>(this Dictionary<string, T> styles, string id) where T : WidgetStyle
		{
			T result;
			if (!styles.TryGetValue(id, out result))
			{
				if (id == Stylesheet.DefaultStyleName)
				{
					throw new Exception("Stylesheet doesn't have the default " + typeof(T).Name);
				}
				else
				{
					throw new Exception("Stylesheet lacks the " + typeof(T).Name + " with id '" + id + "'");
				}
			}

			return result;
		}
	}
}
