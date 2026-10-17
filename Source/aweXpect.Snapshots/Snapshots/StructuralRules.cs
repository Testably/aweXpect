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

	/// <summary>
	///     Throws for a line rule of an expectation on a subject that is not a <see cref="string" />.
	/// </summary>
	/// <remarks>
	///     The lines of an object snapshot are written by the formatter, so a line rule would depend on its layout. It
	///     throws where the rule is written, like <see cref="ThrowIfString{TType}" />; global line rules are ignored for
	///     objects instead.
	/// </remarks>
	public static void ThrowIfNotString<TType>()
	{
		if (typeof(TType) != typeof(string))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				"A line rule only applies to a snapshot of a string, but an object is written by the formatter. Leave out its member with '.Ignoring.Member(…)' instead."));
		}
	}
}
