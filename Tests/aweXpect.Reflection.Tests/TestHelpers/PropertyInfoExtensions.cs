using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class PropertyInfoExtensions
{
	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is an extension property declared with the C# extension block
	///     syntax (i.e. its declaring type is a compiler-generated <c>&lt;G&gt;$…</c> grouping type).
	/// </summary>
	/// <remarks>
	///     This is a parallel reference implementation used to derive the expected property set in the filter and
	///     collection tests, so it must not call into the production code. The behaviour of individual properties is
	///     pinned by the hard-coded assertions in <c>ThatProperty.IsAnExtensionProperty</c>.
	/// </remarks>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	public static bool IsReallyExtensionProperty(this PropertyInfo? propertyInfo)
		=> propertyInfo?.DeclaringType is { } declaringType &&
		   declaringType.Name.StartsWith("<", StringComparison.Ordinal) &&
		   declaringType.IsDefined(typeof(ExtensionAttribute), false);

#if NET10_0_OR_GREATER
	/// <summary>
	///     Fetches the extension property with the given <paramref name="name" /> from the <c>&lt;G&gt;$…</c> grouping
	///     types nested in the <paramref name="owner" /> static class.
	/// </summary>
	public static PropertyInfo GetExtensionProperty(this Type owner, string name)
		=> owner.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
			.Where(t => t.Name.StartsWith("<", StringComparison.Ordinal) &&
			            t.IsDefined(typeof(ExtensionAttribute), false))
			.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
			                                 BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Single(p => p.Name == name);
#endif

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is required (marked with the <c>required</c> modifier).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	public static bool IsRequired(this PropertyInfo? propertyInfo)
		=> propertyInfo != null && propertyInfo.GetCustomAttributes(true)
			.Any(attribute => attribute.GetType().FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute");

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is abstract (based on its accessor methods).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	/// <returns><see langword="true" /> if the property is abstract; otherwise, <see langword="false" />.</returns>
	public static bool IsReallyAbstract(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsAbstract == true ||
		   propertyInfo?.SetMethod?.IsAbstract == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is sealed (based on its accessor methods).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	/// <returns><see langword="true" /> if the property is sealed; otherwise, <see langword="false" />.</returns>
	public static bool IsReallySealed(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsFinal == true ||
		   propertyInfo?.SetMethod?.IsFinal == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is static.
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> which is checked to be static.</param>
	public static bool IsReallyStatic(
		this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsStatic == true ||
		   propertyInfo?.SetMethod?.IsStatic == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> is virtual (based on its accessor methods).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	public static bool IsReallyVirtual(this PropertyInfo? propertyInfo)
		=> propertyInfo?.GetMethod?.IsVirtual == true ||
		   propertyInfo?.SetMethod?.IsVirtual == true;

	/// <summary>
	///     Checks if the <paramref name="propertyInfo" /> overrides a base class property (based on its accessor methods).
	/// </summary>
	/// <param name="propertyInfo">The <see cref="PropertyInfo" /> to check.</param>
	public static bool IsReallyOverride(this PropertyInfo? propertyInfo)
		=> IsAccessorOverride(propertyInfo?.GetMethod) ||
		   IsAccessorOverride(propertyInfo?.SetMethod);

	private static bool IsAccessorOverride(MethodInfo? methodInfo)
		=> methodInfo is not null &&
		   methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;
}
