using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class PropertyInfoExtensions
{
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
