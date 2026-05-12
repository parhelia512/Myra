using Myra.Attributes;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Base abstract class for controls that contain a single content widget.
	/// </summary>
	public abstract class ContentControl: Widget, IContent
	{
		/// <summary>
		/// Gets or sets the content widget.
		/// </summary>
		[Content]
		[DefaultValue(null)]
		public abstract Widget Content { get; set; }

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var contentControl = (ContentControl)w;
			Content = contentControl.Content.Clone();
		}
	}
}
