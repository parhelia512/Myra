using System.Collections;
using System.Reflection;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Linq;
using System.Xml.Serialization;
using System;
using Myra.MML;
using System.Collections.Generic;
using Myra.Attributes;
using System.Linq;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp;
using Myra.Utility;
using Myra.Graphics2D.UI.File;
using AssetManagementBase;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Options for exporting a UI project.
	/// </summary>
	public class ExportOptions
	{
		/// <summary>
		/// Gets or sets the namespace for the exported code.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the class name for the exported code.
		/// </summary>
		public string Class { get; set; }

		/// <summary>
		/// Gets or sets the output path for the exported files.
		/// </summary>
		public string OutputPath { get; set; }

		/// <summary>
		/// Gets or sets the template for the designer file.
		/// </summary>
		public string TemplateDesigner { get; set; }

		/// <summary>
		/// Gets or sets the template for the main file.
		/// </summary>
		public string TemplateMain { get; set; }
	}

	/// <summary>
	/// Represents the position of an object in a document.
	/// </summary>
	public class ObjectPosition
	{
		/// <summary>
		/// Gets the object.
		/// </summary>
		public object Object { get; private set; }

		/// <summary>
		/// Gets the starting position.
		/// </summary>
		public int Start { get; private set; }

		/// <summary>
		/// Gets the ending position.
		/// </summary>
		public int End { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectPosition"/> class with the specified object and positions.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="start">The starting position.</param>
		/// <param name="end">The ending position.</param>
		public ObjectPosition(object obj, int start, int end)
		{
			Object = obj;
			Start = start;
			End = end;
		}
	}

	/// <summary>
	/// Represents a UI project that can be saved, loaded, and exported to code.
	/// </summary>
	public class Project
	{
		private struct StylesheetChanger: IDisposable
		{
			private readonly Stylesheet _oldStylesheet;

			public StylesheetChanger(Stylesheet newStylesheet)
			{
				_oldStylesheet = Stylesheet.Current;
				Stylesheet.Current = newStylesheet;
			}

			public void Dispose()
			{
				Stylesheet.Current = _oldStylesheet;
			}
		}

		/// <summary>Constant name for proportion values.</summary>
		public const string ProportionName = "Proportion";
		/// <summary>Constant name for default proportion values.</summary>
		public const string DefaultProportionName = "DefaultProportion";
		/// <summary>Constant name for default column proportion values.</summary>
		public const string DefaultColumnProportionName = "DefaultColumnProportion";
		/// <summary>Constant name for default row proportion values.</summary>
		public const string DefaultRowProportionName = "DefaultRowProportion";

		private static readonly Dictionary<string, string> LegacyClassNames = new Dictionary<string, string>();

		private readonly ExportOptions _exportOptions = new ExportOptions();

		/// <summary>
		/// Gets the export options for this project.
		/// </summary>
		[Browsable(false)]
		public ExportOptions ExportOptions
		{
			get { return _exportOptions; }
		}

		/// <summary>
		/// Gets or sets the root widget of the project.
		/// </summary>
		[Browsable(false)]
		[Content]
		public Widget Root { get; set; }

		/// <summary>
		/// Gets or sets the path to the stylesheet file.
		/// </summary>
		[Browsable(false)]
		public string StylesheetPath { get; set; }

		/// <summary>
		/// Gets or sets the stylesheet for this project.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Stylesheet Stylesheet { get; set; }

		/// <summary>
		/// Gets or sets the designer runtime assets folder path.
		/// </summary>
		[FilePath(FileDialogMode.ChooseFolder)]
		public string DesignerRtfAssetsPath { get; set; }

		/// <summary>
		/// Gets the mapping of loaded objects to their respective XML nodes.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public List<Tuple<object, XElement>> ObjectsNodes { get; internal set; }

		static Project()
		{
			LegacyClassNames["VerticalBox"] = "VerticalStackPanel";
			LegacyClassNames["HorizontalBox"] = "HorizontalStackPanel";
			LegacyClassNames["TextField"] = "TextBox";
			LegacyClassNames["TextBlock"] = "Label";
			LegacyClassNames["ScrollPane"] = "ScrollViewer";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Project"/> class.
		/// </summary>
		public Project()
		{
			Stylesheet = Stylesheet.Current;
		}

		/// <summary>
		/// Determines whether the specified name is a proportion property name.
		/// </summary>
		/// <param name="s">The name to check.</param>
		/// <returns>true if the name is a proportion name; otherwise, false.</returns>
		public static bool IsProportionName(string s)
		{
			return s.EndsWith(ProportionName) ||
				s.EndsWith(DefaultProportionName) ||
				s.EndsWith(DefaultColumnProportionName) ||
				s.EndsWith(DefaultRowProportionName);
		}

		/// <summary>
		/// Determines whether a property should be serialized for the specified object.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for comparison.</param>
		/// <param name="o">The object containing the property.</param>
		/// <param name="p">The property information.</param>
		/// <returns>true if the property should be serialized; otherwise, false.</returns>
		public static bool ShouldSerializeProperty(Stylesheet stylesheet, object o, PropertyInfo p)
		{
			var asWidget = o as Widget;
			if (asWidget != null && asWidget.Parent != null && asWidget.Parent is Grid)
			{
				var container = asWidget.Parent.Parent;
				if (container != null &&
				   (container is StackPanel || container is SplitPane) &&
				   (p.Name == "GridRow" || p.Name == "GridColumn"))
				{
					// Skip serializing auto-assigned GridRow/GridColumn for SplitPane and Box containers
					return false;
				}
			}

			var asGrid = o as Grid;
			if (asGrid != null)
			{
				var value = p.GetValue(o);
				if ((p.Name == DefaultColumnProportionName || p.Name == DefaultRowProportionName) &&
					value == Proportion.GridDefault)
				{
					return false;
				}
			}

			var asBox = o as StackPanel;
			if (asBox != null)
			{
				var value = p.GetValue(o);
				if (p.Name == DefaultProportionName && value == Proportion.StackPanelDefault)
				{
					return false;
				}
			}

			if (SaveContext.HasDefaultValue(o, p))
			{
				return false;
			}

			if (asWidget != null && HasStylesheetValue(asWidget, p, stylesheet))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Determines whether a property should be serialized for the specified object using this project's stylesheet.
		/// </summary>
		/// <param name="o">The object containing the property.</param>
		/// <param name="p">The property information.</param>
		/// <returns>true if the property should be serialized; otherwise, false.</returns>
		public bool ShouldSerializeProperty(object o, PropertyInfo p)
		{
			return ShouldSerializeProperty(Stylesheet, o, p);
		}

		internal static SaveContext CreateSaveContext(Stylesheet stylesheet)
		{
			return new SaveContext
			{
				ShouldSerializeProperty = (o, p) => ShouldSerializeProperty(stylesheet, o, p)
			};
		}

		internal SaveContext CreateSaveContext()
		{
			return CreateSaveContext(Stylesheet);
		}

		/// <summary>
		/// Gets or sets the extra widget assemblies and namespaces to include during project loading and saving.
		/// </summary>
		public static Dictionary<Assembly, string[]> ExtraWidgetAssembliesAndNamespaces = new Dictionary<Assembly, string[]>();
		
		internal static LoadContext CreateLoadContext(AssetManager assetManager)
		{
			Func<Type, string, object> resourceGetter = (t, name) =>
			{
				if (t == typeof(IBrush))
				{
					return new SolidBrush(name);
				}
				else if (t == typeof(IImage))
				{
					return assetManager.LoadTextureRegion(name);
				}
				else if (t == typeof(SpriteFontBase))
				{
					return assetManager.LoadFont(name);
				}

				throw new Exception(string.Format("Type {0} isn't supported", t.Name));
			};

			Dictionary<Assembly, string[]> assemblies = new Dictionary<Assembly, string[]>(ExtraWidgetAssembliesAndNamespaces);
			assemblies.Add(typeof(Widget).Assembly, new string[] { typeof(Widget).Namespace, typeof(PropertyGrid).Namespace });
			return new LoadContext
			{
				Assemblies = assemblies,
				LegacyClassNames = LegacyClassNames,
				ObjectCreator = (t, el) => CreateItem(t, el),
				ResourceGetter = resourceGetter
			};
		}

		/// <summary>
		/// Saves the project to an XML string.
		/// </summary>
		/// <returns>An XML string representation of the project.</returns>
		public string Save()
		{
			var saveContext = CreateSaveContext();
			var root = saveContext.Save(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
		}

		/// <summary>
		/// Loads a project from an XDocument with an optional handler.
		/// </summary>
		/// <typeparam name="T">The type of the handler.</typeparam>
		/// <param name="xDoc">The XDocument to load from.</param>
		/// <param name="assetManager">The asset manager for loading resources. Required if the project has an external stylesheet.</param>
		/// <param name="handler">Optional handler for loading events.</param>
		/// <returns>The loaded project.</returns>
		public static Project LoadFromXml<T>(XDocument xDoc, AssetManager assetManager = null, T handler = null) where T : class
		{
			var stylesheet = Stylesheet.Current;
			var stylesheetPathAttr = xDoc.Root.Attribute("StylesheetPath");
			if (stylesheetPathAttr != null)
			{
				stylesheet = assetManager.LoadStylesheet(stylesheetPathAttr.Value);
			}

			var result = new Project();

			if (stylesheetPathAttr != null)
			{
				if (assetManager == null)
				{
					throw new Exception($"assetManager couldn't be null if the project has external stylesheet");
				}

				result.Stylesheet = stylesheet;
				using(var stylesheetChanger = new StylesheetChanger(stylesheet))
				{
					var loadContext = CreateLoadContext(assetManager);
					loadContext.Load(result, xDoc.Root, handler);
					result.ObjectsNodes = loadContext.ObjectsNodes;
				}
			}
			else
			{
				var loadContext = CreateLoadContext(assetManager);
				loadContext.Load(result, xDoc.Root, handler);
				result.ObjectsNodes = loadContext.ObjectsNodes;
			}

			return result;
		}

		/// <summary>
		/// Loads a project from XML string data with an optional handler.
		/// </summary>
		/// <typeparam name="T">The type of the handler.</typeparam>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <param name="handler">Optional handler for loading events.</param>
		/// <returns>The loaded project.</returns>
		public static Project LoadFromXml<T>(string data, AssetManager assetManager = null, T handler = null) where T : class
		{
			return LoadFromXml(XDocument.Parse(data, LoadOptions.SetLineInfo), assetManager, handler);
		}

		/// <summary>
		/// Loads a project from an XDocument.
		/// </summary>
		/// <param name="xDoc">The XDocument to load from.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <returns>The loaded project.</returns>
		public static Project LoadFromXml(XDocument xDoc, AssetManager assetManager = null)
		{
			return LoadFromXml<object>(xDoc, assetManager, null);
		}

		/// <summary>
		/// Loads a project from XML string data.
		/// </summary>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <returns>The loaded project.</returns>
		public static Project LoadFromXml(string data, AssetManager assetManager = null)
		{
			return LoadFromXml<object>(XDocument.Parse(data, LoadOptions.SetLineInfo), assetManager, null);
		}

		/// <summary>
		/// Loads a single object from XML string data.
		/// </summary>
		/// <typeparam name="T">The type of the handler.</typeparam>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <param name="stylesheet">The stylesheet to apply to loaded objects.</param>
		/// <param name="handler">Optional handler for loading events.</param>
		/// <param name="parentType">The parent type context for loading.</param>
		/// <returns>The loaded object.</returns>
		public static object LoadObjectFromXml<T>(string data, AssetManager assetManager = null, Stylesheet stylesheet = null, T handler = null, Type parentType = null) where T : class
		{
			XDocument xDoc = XDocument.Parse(data, LoadOptions.SetLineInfo);

			var name = xDoc.Root.Name.ToString();
			Type itemType;

			if (name == "PropertyGrid")
			{
				itemType = typeof(PropertyGrid);
			}
			else if (!IsProportionName(name))
			{
				string newName;
				if (LegacyClassNames.TryGetValue(name, out newName))
				{
					name = newName;
				}

				itemType = GetWidgetTypeByName(name);
			}
			else
			{
				itemType = typeof(Proportion);
			}

			if (itemType == null)
			{
				return null;
			}

			object item = null;
			if (stylesheet != null)
			{
				using (var stylesheetChanger = new StylesheetChanger(stylesheet))
				{
					item = CreateItem(itemType, xDoc.Root);
					var loadContext = CreateLoadContext(assetManager);
					loadContext.Load(item, xDoc.Root, handler);
				}
			}
			else
			{
				item = CreateItem(itemType, xDoc.Root);
				var loadContext = CreateLoadContext(assetManager);
				loadContext.Load(item, xDoc.Root, handler);
			}

			return item;
		}

		/// <summary>
		/// Loads a single object from XML string data using the specified stylesheet.
		/// </summary>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <param name="stylesheet">The stylesheet to apply to loaded objects.</param>
		/// <returns>The loaded object.</returns>
		public static object LoadObjectFromXml(string data, AssetManager assetManager, Stylesheet stylesheet)
		{
			return LoadObjectFromXml<object>(data, assetManager, stylesheet, null);
		}

		/// <summary>
		/// Loads a single object from XML string data using this project's stylesheet.
		/// </summary>
		/// <param name="data">The XML data as a string.</param>
		/// <param name="assetManager">The asset manager for loading resources.</param>
		/// <returns>The loaded object.</returns>
		public object LoadObjectFromXml(string data, AssetManager assetManager)
		{
			return LoadObjectFromXml(data, assetManager, Stylesheet);
		}

		/// <summary>
		/// Saves an object to an XML string.
		/// </summary>
		/// <param name="obj">The object to save.</param>
		/// <param name="tagName">The XML tag name for the object.</param>
		/// <param name="parentType">The parent type context for saving.</param>
		/// <returns>An XML string representation of the object.</returns>
		public string SaveObjectToXml(object obj, string tagName, Type parentType)
		{
			var saveContext = CreateSaveContext(Stylesheet);
			return saveContext.Save(obj, true, tagName, parentType).ToString();
		}

		private static object CreateItem(Type type, XElement element)
		{
			if (typeof(Widget).IsAssignableFrom(type))
			{
				// Check whether it accepts style name parameter
				var acceptsStyleName = false;
				foreach (var c in type.GetConstructors())
				{
					var p = c.GetParameters();
					if (p != null && p.Length == 1)
					{
						if (p[0].ParameterType == typeof(string))
						{
							acceptsStyleName = true;
							break;
						}
					}
				}

				if (acceptsStyleName)
				{
					// Determine style name
					var styleName = Stylesheet.DefaultStyleName;
					var styleNameAttr = element.Attribute("StyleName");
					if (styleNameAttr != null)
					{
						var stylesNames = Stylesheet.Current.GetStylesByWidgetName(type.Name);
						if (stylesNames != null && stylesNames.Contains(styleNameAttr.Value))
						{
							styleName = styleNameAttr.Value;
						}
						else
						{
							// Remove property with absent value
							styleNameAttr.Remove();
						}
					}

					return (Widget)Activator.CreateInstance(type, styleName);
				}
			}

			return Activator.CreateInstance(type);
		}

		private static bool HasStylesheetValue(Widget w, PropertyInfo property, Stylesheet stylesheet)
		{
			if (stylesheet == null)
			{
				return false;
			}

			var styleName = w.StyleName;
			if (string.IsNullOrEmpty(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			// Find styles dict of that widget
			var typeName = w.GetType().Name;
			var styleTypeNameAttribute = w.GetType().FindAttribute<StyleTypeNameAttribute>();
			if (styleTypeNameAttribute != null)
			{
				typeName = styleTypeNameAttribute.Name;
			}

			var stylesDictPropertyName = typeName + "Styles";
			var stylesDictProperty = stylesheet.GetType().GetRuntimeProperty(stylesDictPropertyName);
			if (stylesDictProperty == null)
			{
				return false;
			}

			var stylesDict = (IDictionary)stylesDictProperty.GetValue(stylesheet);
			if (stylesDict == null)
			{
				return false;
			}

			// Fetch style from the dict
			if (!stylesDict.Contains(styleName))
			{
				styleName = Stylesheet.DefaultStyleName;
			}

			object obj = stylesDict[styleName];

			// Now find corresponding property
			PropertyInfo styleProperty = null;

			var stylePropertyPathAttribute = property.FindAttribute<StylePropertyPathAttribute>();
			if (stylePropertyPathAttribute != null)
			{
				var path = stylePropertyPathAttribute.Name;
				if (path.StartsWith("/"))
				{
					obj = stylesheet;
					path = path.Substring(1);
				}

				var parts = path.Split('/');
				for (var i = 0; i < parts.Length; ++i)
				{
					styleProperty = obj.GetType().GetRuntimeProperty(parts[i]);

					if (i < parts.Length - 1)
					{
						obj = styleProperty.GetValue(obj);
					}
				}
			}
			else
			{
				styleProperty = obj.GetType().GetRuntimeProperty(property.Name);
			}

			if (styleProperty == null)
			{
				return false;
			}

			// Compare values
			var styleValue = styleProperty.GetValue(obj);
			var value = property.GetValue(w);
			if (!Equals(styleValue, value))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the widget type by its name.
		/// </summary>
		/// <param name="name">The name of the widget type.</param>
		/// <returns>The widget type, or null if not found.</returns>
		public static Type GetWidgetTypeByName(string name)
		{
			var itemNamespace = typeof(Widget).Namespace;
			return typeof(Widget).Assembly.GetType(itemNamespace + "." + name);
		}
	}
}