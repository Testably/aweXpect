using System;
using aweXpect.Core;

namespace aweXpect.Snapshots;

internal static class StructuralRules
{
	/// <summary>
	///     Throws for a member or type rule of an expectation on a <see cref="string" /> subject.
	/// </summary>
	/// <remarks>
	///     A <see cref="string" /> is compared as raw text and has no members, so such a rule could never apply. It
	///     throws where the rule is written, like an invalid argument, instead of failing the expectation, as it is a
	///     mistake in the test and not in the subject. Global rules are ignored for strings instead, as they apply to all
	///     snapshots.
	/// </remarks>
	public static void ThrowIfString<TType>()
	{
		if (typeof(TType) == typeof(string))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				"A member or type rule only applies to a snapshot of an object, but a string is compared as raw text. Mask it with '.Masking.Text(…)' or '.Masking.Regex(…)' instead."));
		}
	}
}
