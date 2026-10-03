using System;
using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Core.Helpers;

/// <remarks>
///     <c>ThatGeneric.Whose</c> in the <c>aweXpect</c> assembly keeps its own copy, because outside of the <c>Debug</c>
///     configuration it is compiled against the released <c>aweXpect.Core</c> package.
/// </remarks>
internal static class MemberGrammars
{
	/// <summary>
	///     The <paramref name="grammars" /> for the expectations on a member of type <typeparamref name="TMember" />.
	/// </summary>
	/// <remarks>
	///     The number of the member follows its static type alone: a collection other than a <see langword="string" />
	///     or a dictionary is plural (<c>whose Items are</c>), anything else is singular, whatever the number of the
	///     enclosing subject.
	/// </remarks>
	public static ExpectationGrammars ForMember<TMember>(this ExpectationGrammars grammars)
		=> IsCollection(typeof(TMember))
			? grammars | ExpectationGrammars.Plural
			: grammars & ~ExpectationGrammars.Plural;

	private static bool IsCollection(Type type)
	{
		if (type == typeof(string) || IsDictionary(type))
		{
			return false;
		}

		if (typeof(IEnumerable).IsAssignableFrom(type))
		{
			return true;
		}

#if NET8_0_OR_GREATER
		// Only the interface itself, because searching the implemented interfaces is not trim-safe.
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
#else
		return false;
#endif
	}

	/// <remarks>
	///     A dictionary reads as a single lookup (<c>whose Map contains key 1</c>), not as a plural noun.<br />
	///     The generic interfaces are only matched by their definition, because searching the implemented interfaces is
	///     not trim-safe; the framework dictionaries also implement <see cref="IDictionary" />.
	/// </remarks>
	private static bool IsDictionary(Type type)
	{
		if (typeof(IDictionary).IsAssignableFrom(type))
		{
			return true;
		}

		if (!type.IsGenericType)
		{
			return false;
		}

		Type definition = type.GetGenericTypeDefinition();
		return definition == typeof(IDictionary<,>) || definition == typeof(IReadOnlyDictionary<,>);
	}
}
