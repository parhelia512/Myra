using Myra.Graphics2D.UI.File;
using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a property as a file path with configurable file dialog options.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class FilePathAttribute : Attribute
	{
		/// <summary>
		/// Gets the file dialog mode (Open, Save, etc.).
		/// </summary>
		public FileDialogMode DialogMode { get; private set; }

		/// <summary>
		/// Gets the file filter string for the file dialog.
		/// </summary>
		public string Filter { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the file path should be displayed.
		/// </summary>
		public bool ShowPath { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FilePathAttribute"/> class.
		/// </summary>
		/// <param name="dialogMode">The file dialog mode to use.</param>
		/// <param name="filter">The file filter string for the dialog (optional).</param>
		/// <param name="showPath">Whether to display the file path (optional).</param>
		public FilePathAttribute(FileDialogMode dialogMode, string filter = "", bool showPath = false)
		{
			DialogMode = dialogMode;
			Filter = filter;
			ShowPath = showPath;
		}
	}
}
