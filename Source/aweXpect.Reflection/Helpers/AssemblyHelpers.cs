using System;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="Assembly" />.
/// </summary>
internal static class AssemblyHelpers
{
	/// <summary>
	///     Checks if the <paramref name="assembly" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="assembly">The <see cref="Assembly" /> which is checked to have the attribute.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute<TAttribute>(
		this Assembly assembly,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(assembly, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="assembly" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="assembly">The <see cref="Assembly" /> which is checked to have the attribute.</param>
	/// <param name="attributeType">The type of the attribute to check for.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute(
		this Assembly? assembly,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = assembly?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}
}
