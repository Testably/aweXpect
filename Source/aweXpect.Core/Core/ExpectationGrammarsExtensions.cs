using System.Linq;
using System.Runtime.CompilerServices;

namespace aweXpect.Core;

/// <summary>
///     Extension methods on <see cref="ExpectationGrammars" />.
/// </summary>
public static class ExpectationGrammarsExtensions
{
	/// <summary>
	///     Toggles the <see cref="ExpectationGrammars.Negated" /> flag.
	/// </summary>
	public static ExpectationGrammars Negate(this ExpectationGrammars grammars)
		=> grammars ^ ExpectationGrammars.Negated;

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Negated" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegated(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Negated);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Nested" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNested(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Nested);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has the <see cref="ExpectationGrammars.Plural" /> flag.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPlural(this ExpectationGrammars grammars)
		=> grammars.HasFlag(ExpectationGrammars.Plural);

	/// <summary>
	///     Checks if the <paramref name="grammars" /> has any of the given <paramref name="flags" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasAnyFlag(this ExpectationGrammars grammars, params ExpectationGrammars[] flags)
		=> flags.Any(flag => grammars.HasFlag(flag));
}
