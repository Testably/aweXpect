using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Recognizes the predicate of an attribute expectation that only checks the type of the attribute.
/// </summary>
/// <remarks>
///     The attributes are looked up by their type, so such a predicate always holds, and the attribute only needs to
///     be defined. <see cref="MemberInfo.IsDefined" /> answers this without instantiating the attributes.
/// </remarks>
internal static class AttributePredicates
{
	/// <summary>
	///     Returns a predicate that checks that the attribute is a <typeparamref name="TAttribute" />.
	/// </summary>
	public static Func<Attribute, bool> IsOfType<TAttribute>()
		where TAttribute : Attribute
		=> TypeCheck<TAttribute>.Predicate;

	/// <summary>
	///     Returns a predicate that checks that the attribute is an <paramref name="attributeType" />.
	/// </summary>
	public static Func<Attribute, bool> IsOfType(Type attributeType)
		=> new TypeCheckOf(attributeType).Matches;

	/// <summary>
	///     Checks if the <paramref name="predicate" /> is missing or only checks the type of the attribute.
	/// </summary>
	public static bool OnlyChecksType([NotNullWhen(false)] Func<Attribute, bool>? predicate)
		=> predicate is null || predicate.Target is TypeCheck;

	private abstract class TypeCheck;

	private sealed class TypeCheck<TAttribute> : TypeCheck
		where TAttribute : Attribute
	{
		public static readonly Func<Attribute, bool> Predicate = new TypeCheck<TAttribute>().Matches;

		public bool Matches(Attribute attribute) => attribute is TAttribute;
	}

	private sealed class TypeCheckOf(Type attributeType) : TypeCheck
	{
		public bool Matches(Attribute attribute) => attributeType.IsInstanceOfType(attribute);
	}
}
