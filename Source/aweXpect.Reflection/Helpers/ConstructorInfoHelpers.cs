using System;
using System.Linq;
using System.Reflection;
using aweXpect.Core;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="ConstructorInfo" />.
/// </summary>
internal static class ConstructorInfoHelpers
{
	/// <summary>
	///     Checks if the <paramref name="constructorInfo" /> has the specified <paramref name="accessModifiers" />.
	/// </summary>
	/// <param name="constructorInfo">The <see cref="ConstructorInfo" /> which is checked to have the attribute.</param>
	/// <param name="accessModifiers">
	///     The <see cref="AccessModifiers" />.
	///     <para />
	///     Supports specifying multiple <see cref="AccessModifiers" />.
	/// </param>
	public static bool HasAccessModifier(
		this ConstructorInfo constructorInfo,
		AccessModifiers accessModifiers)
	{
		if ((accessModifiers & AccessModifiers.Internal) != 0 &&
		    constructorInfo.IsAssembly)
		{
			return true;
		}

		if ((accessModifiers & AccessModifiers.Protected) != 0 &&
		    constructorInfo.IsFamily)
		{
			return true;
		}

		if ((accessModifiers & AccessModifiers.Private) != 0 &&
		    constructorInfo.IsPrivate)
		{
			return true;
		}

		if ((accessModifiers & AccessModifiers.Public) != 0 &&
		    constructorInfo.IsPublic)
		{
			return true;
		}

		if ((accessModifiers & AccessModifiers.PrivateProtected) != 0 &&
		    constructorInfo.IsFamilyAndAssembly)
		{
			return true;
		}

		if ((accessModifiers & AccessModifiers.ProtectedInternal) != 0 &&
		    constructorInfo.IsFamilyOrAssembly)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="constructorInfo" /> has an attribute which satisfies the
	///     <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="constructorInfo">The <see cref="ConstructorInfo" /> which is checked to have the attribute.</param>
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
		this ConstructorInfo constructorInfo,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		if (predicate is null)
		{
			return constructorInfo.IsDefined(typeof(TAttribute), inherit);
		}

		object? attribute = constructorInfo.GetCustomAttributes(typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return UserCode.Invoke(predicate, castedAttribute, "the predicate");
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="constructorInfo" /> has an attribute which satisfies the
	///     <paramref name="predicate" />.
	/// </summary>
	/// <param name="constructorInfo">The <see cref="ConstructorInfo" /> which is checked to have the attribute.</param>
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
		this ConstructorInfo? constructorInfo,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		if (AttributePredicates.OnlyChecksType(predicate))
		{
			return constructorInfo?.IsDefined(attributeType, inherit) == true;
		}

		object? attribute = constructorInfo?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate(attributeValue);
		}

		return false;
	}
}
