using System;
using aweXpect.Core;

namespace aweXpect.Snapshots;

internal static class StructuralRules
{
	/// <summary>
	///     Throws for a member or value rule of an object snapshot whose subject type is <see cref="string" />.
	/// </summary>
	/// <remarks>
	///     Only a generic subject can be a <see cref="string" /> there, as a <see cref="string" /> subject chooses the
	///     snapshot of a string, which offers no member or value rules. A <see cref="string" /> has no members, so such a
	///     rule could never apply as meant, and it throws where it is written, like an invalid argument, instead of
	///     failing the expectation, as it is a mistake in the test and not in the subject.
	/// </remarks>
	public static void ThrowIfNotApplicable<TType>(SnapshotRule rule)
	{
		if (rule.ValueRule is not null && typeof(TType) == typeof(string))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				"A member or value rule only applies to a snapshot of an object, but the subject is a string. Select its text with '.For.Text(…)' instead."));
		}
	}
}
