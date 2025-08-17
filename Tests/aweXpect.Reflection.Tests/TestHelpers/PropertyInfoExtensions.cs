using System.Reflection;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class PropertyInfoExtensions
{
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
}
