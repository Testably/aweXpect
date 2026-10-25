using System.Collections.Generic;
using System.Text;
#if !NET8_0_OR_GREATER
using System.Linq;
#endif

namespace aweXpect.Reflection.Collections;

internal static class AccessModifiersExtensions
{
	/// <summary>
	///     Appends the expectation that a member has the <paramref name="accessModifier" />, e.g. "is public".
	/// </summary>
	/// <remarks>
	///     <see cref="AccessModifiers.Any" /> has no keyword of its own and reads "has any access modifier".<br />
	///     The plural is never negated, as the quantifier carries the negation on a collection.
	/// </remarks>
	public static StringBuilder AppendExpectation(this StringBuilder stringBuilder, AccessModifiers accessModifier,
		bool isNegated, bool isPlural)
	{
		if (accessModifier == AccessModifiers.Any)
		{
			return stringBuilder.Append(Verb(isNegated, isPlural, "have", "does not have", "has"))
				.Append(" any access modifier");
		}

		return stringBuilder.Append(Verb(isNegated, isPlural, "are ", "is not ", "is "))
			.Append(accessModifier.GetString());
	}

	private static string Verb(bool isNegated, bool isPlural, string plural, string negated, string singular)
	{
		if (isPlural)
		{
			return plural;
		}

		return isNegated ? negated : singular;
	}

	public static string GetString(this AccessModifiers accessModifier, string suffix = "")
	{
		if (accessModifier == AccessModifiers.Public)
		{
			return "public" + suffix;
		}

		if (accessModifier == AccessModifiers.Protected)
		{
			return "protected" + suffix;
		}

		if (accessModifier == AccessModifiers.Private)
		{
			return "private" + suffix;
		}

		if (accessModifier == AccessModifiers.Internal)
		{
			return "internal" + suffix;
		}

		if (accessModifier == AccessModifiers.ProtectedInternal)
		{
			return "protected internal" + suffix;
		}

		if (accessModifier == AccessModifiers.PrivateProtected)
		{
			return "private protected" + suffix;
		}

		if (accessModifier == AccessModifiers.Any)
		{
			return string.Empty;
		}

		List<string> modifiers = [];
		if (accessModifier.HasFlag(AccessModifiers.Public))
		{
			modifiers.Add("public");
		}

		if (accessModifier.HasFlag(AccessModifiers.Protected))
		{
			modifiers.Add("protected");
		}

		if (accessModifier.HasFlag(AccessModifiers.Private))
		{
			modifiers.Add("private");
		}

		if (accessModifier.HasFlag(AccessModifiers.Internal))
		{
			modifiers.Add("internal");
		}

		if (accessModifier.HasFlag(AccessModifiers.PrivateProtected))
		{
			modifiers.Add("private protected");
		}

		if (accessModifier.HasFlag(AccessModifiers.ProtectedInternal))
		{
			modifiers.Add("protected internal");
		}

#if NET8_0_OR_GREATER
		return string.Join(", ", modifiers[..^1]) + " or " + modifiers[^1] + suffix;
#else
		return string.Join(", ", modifiers.Take(modifiers.Count - 1)) + " or " + modifiers[modifiers.Count - 1] + suffix;
#endif
	}
}
