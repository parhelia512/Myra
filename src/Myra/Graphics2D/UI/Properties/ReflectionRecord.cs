using Myra.Utility;
using System.Reflection;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Abstract base class for records that use reflection to access property and field members.
	/// </summary>
	internal abstract class ReflectionRecord : Record
	{
		/// <summary>
		/// Gets the underlying reflection member info (property or field).
		/// </summary>
		public abstract MemberInfo MemberInfo { get; }

		/// <summary>
		/// Finds an attribute of the specified type on the underlying member.
		/// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <returns>The attribute if found; otherwise null.</returns>
		public override T FindAttribute<T>()
		{
			return MemberInfo.FindAttribute<T>();
		}
	}
}
