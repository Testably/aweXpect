using System;
using aweXpect.Core;

namespace aweXpect.Snapshots;

internal static class StructuralRules
{
	/// <summary>
	///     Throws for a member or value rule of an expectation on a <see cref="string" /> subject, and for a line rule on
	///     any other subject.
	/// </summary>
	/// <remarks>
	///     A <see cref="string" /> is compared as raw text and has no members, and the lines of an object snapshot are
	///     written by the formatter, so a line rule would depend on its layout. Such a rule could never apply as meant,
	///     so it throws where it is written, like an invalid argument, instead of failing the expectation, as it is a
	///     mistake in the test and not in the subject. Rules for all snapshots are ignored there instead.
	/// </remarks>
	public static void ThrowIfNotApplicable<TType>(SnapshotRule rule)
	{
		if (rule.ValueRule is not null && typeof(TType) == typeof(string))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				"A member or value rule only applies to a snapshot of an object, but a string is compared as raw text. Select its lines with '.For.Lines()' or its text with '.For.Text(…)' instead."));
		}

		if (rule.LineRule is not null && typeof(TType) != typeof(string))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				"A line rule only applies to a snapshot of a string, but an object is written by the formatter. Select its members with '.For.Members()' instead."));
		}
	}
}
