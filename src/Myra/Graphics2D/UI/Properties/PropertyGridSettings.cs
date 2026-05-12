using System;
using System.ComponentModel;
using System.Xml.Serialization;
using AssetManagementBase;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Configuration settings for PropertyGrid controls.
	/// </summary>
	public class PropertyGridSettings
	{
		/// <summary>
		/// Gets or sets the asset manager used to manage resources.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public AssetManager AssetManager;

		/// <summary>
		/// Gets or sets the base path for relative file paths.
		/// </summary>
		public string BasePath;

		/// <summary>
		/// Gets or sets a function that retrieves the value of an image property.
		/// </summary>
		public Func<string, string> ImagePropertyValueGetter;

		/// <summary>
		/// Gets or sets an action that sets the value of an image property.
		/// </summary>
		public Action<string, string> ImagePropertyValueSetter;
	}
}
