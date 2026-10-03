using System;
using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     The comparer of a collection subject, which decides whether two of its items are equal.
/// </summary>
internal sealed class SubjectComparer<T>(Func<T, T, bool> areEqual, object comparer)
{
	/// <summary>
	///     Indicates whether the comparer considers the <paramref name="actual" /> and the <paramref name="expected" />
	///     value equal.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> value only equals <see langword="null" /> and is never handed to the comparer,
	///     because a comparer may reject it. A value that is not a <typeparamref name="T" /> never equals one.
	/// </remarks>
	public bool AreEqual<TActual, TExpected>(TActual actual, TExpected expected)
		=> actual is T typedActual && expected is T typedExpected
			? UserCode.Invoke(static values => values.AreEqual(values.Actual, values.Expected),
				(AreEqual: areEqual, Actual: typedActual, Expected: typedExpected), "the comparer")
			: actual is null && expected is null;

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString() => CollectionComparerHelpers.DescribeSubjectComparer(comparer);
}
