using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.MML;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Base abstract class for panels that stack child widgets in a single row or column.
	/// </summary>
	public abstract class StackPanel : Container
	{
		/// <summary>
		/// Attached property info for specifying the proportion type of a child widget.
		/// </summary>
		public static readonly AttachedPropertyInfo<ProportionType> ProportionTypeProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionType",
				ProportionType.Auto, AttachedPropertyOption.AffectsMeasure);

		/// <summary>
		/// Attached property info for specifying the proportion value of a child widget.
		/// </summary>
		public static readonly AttachedPropertyInfo<float> ProportionValueProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionValue",
				1.0f, AttachedPropertyOption.AffectsMeasure,
				new Attribute[] { new RangeAttribute(0.0f) });

		private readonly StackPanelLayout _layout;
		private readonly ObservableCollection<Proportion> _proportions = new ObservableCollection<Proportion>();
		private bool _childrenDirty = true;

		/// <summary>
		/// Gets the orientation (horizontal or vertical) of this stack panel.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Gets or sets a value indicating whether to show grid lines for debugging layout.
		/// </summary>
		[Category("Debug")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		/// <summary>
		/// Gets or sets the color of the grid lines shown for debugging.
		/// </summary>
		[Category("Debug")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		/// <summary>
		/// Gets or sets the spacing between child widgets.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(0)]
		public int Spacing
		{
			get => _layout.Spacing;
			set => _layout.Spacing = value;
		}

		/// <summary>
		/// Gets or sets the default proportion for child widgets.
		/// </summary>
		[Browsable(false)]
		public Proportion DefaultProportion
		{
			get => _layout.DefaultProportion;
			set => _layout.DefaultProportion = value;
		}

		/// <summary>
		/// Gets the collection of proportions for child widgets. This property is obsolete.
		/// </summary>
		[Browsable(false)]
		[Obsolete("Use StackPanel.GetProportion/StackPanel.SetProportion")]
		[SkipSave]
		public ObservableCollection<Proportion> Proportions => _proportions;

		/// <summary>
		/// Initializes a new instance of the <see cref="StackPanel"/> class.
		/// </summary>
		protected StackPanel()
		{
			_layout = new StackPanelLayout(Orientation);
			ChildrenLayout = _layout;
			GridLinesColor = Color.White;

			_proportions.CollectionChanged += (s, e) => InvalidateChildren();
		}

		/// <summary>
		/// Gets the size of the cell at the specified index.
		/// </summary>
		/// <param name="index">The index of the cell.</param>
		/// <returns>The size of the cell.</returns>
		public int GetCellSize(int index) => _layout.GetCellSize(index);

		private void InvalidateChildren()
		{
			_childrenDirty = true;
		}

		protected void UpdateChildren()
		{
			if (!_childrenDirty)
			{
				return;
			}

			var index = 0;
			foreach (var widget in ChildrenCopy)
			{
				if (index < _proportions.Count)
				{
					var prop = _proportions[index];
					SetProportionType(widget, prop.Type);
					SetProportionValue(widget, prop.Value);
				}

				++index;
			}

			_childrenDirty = false;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			UpdateChildren();

			return base.InternalMeasure(availableSize);
		}

		protected override void InternalArrange()
		{
			UpdateChildren();

			base.InternalArrange();
		}

		/// <summary>
		/// Renders this stack panel and optionally displays grid lines for debugging layout.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		public override void InternalRender(RenderContext context)
		{

			base.InternalRender(context);

			if (!ShowGridLines)
			{
				return;
			}

			var bounds = ActualBounds;

			int i;
			for (i = 0; i < _layout.GridLinesX.Count; ++i)
			{
				var x = _layout.GridLinesX[i] + bounds.Left;
				context.FillRectangle(new Rectangle(x, bounds.Top, 1, bounds.Height), GridLinesColor);
			}

			for (i = 0; i < _layout.GridLinesY.Count; ++i)
			{
				var y = _layout.GridLinesY[i] + bounds.Top;
				context.FillRectangle(new Rectangle(bounds.Left, y, bounds.Width, 1), GridLinesColor);
			}
		}

		/// <summary>
		/// Called when an attached property of this stack panel or its children changes.
		/// </summary>
		/// <param name="propertyInfo">The attached property info that changed.</param>
		public override void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo)
		{
			base.OnAttachedPropertyChanged(propertyInfo);

			if (propertyInfo.Id == ProportionTypeProperty.Id ||
				propertyInfo.Id == ProportionValueProperty.Id)
			{
				InvalidateChildren();
			}
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var stackPanel = (StackPanel)w;

			ShowGridLines = stackPanel.ShowGridLines;
			GridLinesColor = stackPanel.GridLinesColor;
			Spacing = stackPanel.Spacing;
			DefaultProportion = stackPanel.DefaultProportion;
		}

		/// <summary>
		/// Gets the proportion type of a widget within a stack panel.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The proportion type of the widget.</returns>
		public static ProportionType GetProportionType(Widget widget) => ProportionTypeProperty.GetValue(widget);

		/// <summary>
		/// Sets the proportion type of a widget within a stack panel.
		/// </summary>
		/// <param name="widget">The widget to set the proportion type for.</param>
		/// <param name="value">The proportion type to set.</param>
		public static void SetProportionType(Widget widget, ProportionType value) => ProportionTypeProperty.SetValue(widget, value);

		/// <summary>
		/// Gets the proportion value of a widget within a stack panel.
		/// </summary>
		/// <param name="widget">The widget to query.</param>
		/// <returns>The proportion value of the widget.</returns>
		public static float GetProportionValue(Widget widget) => ProportionValueProperty.GetValue(widget);

		/// <summary>
		/// Sets the proportion value of a widget within a stack panel.
		/// </summary>
		/// <param name="widget">The widget to set the proportion value for.</param>
		/// <param name="value">The proportion value to set.</param>
		public static void SetProportionValue(Widget widget, float value) => ProportionValueProperty.SetValue(widget, value);
	}
}
