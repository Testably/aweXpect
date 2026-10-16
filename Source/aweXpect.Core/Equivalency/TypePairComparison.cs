using System;
using System.Text;

namespace aweXpect.Equivalency;

/// <summary>
///     A predicate that decides whether an actual value is equivalent to an expected value of another type, instead of
///     comparing their members.
/// </summary>
internal abstract class TypePairComparison(string description)
{
	/// <summary>
	///     The predicate as written by the caller.
	/// </summary>
	public string Description { get; } = description;

	/// <summary>
	///     Appends the types of the actual and the expected value that the predicate applies to.
	/// </summary>
	public abstract void AppendTypes(StringBuilder stringBuilder);

	/// <summary>
	///     Whether the predicate applies to the <paramref name="actual" /> and the <paramref name="expected" /> value.
	/// </summary>
	public abstract bool AppliesTo(object actual, object expected);

	/// <summary>
	///     Whether the predicate considers the <paramref name="actual" /> and the <paramref name="expected" /> value
	///     equivalent, which it only decides when it <see cref="AppliesTo" /> them.
	/// </summary>
	public abstract bool Compare(object actual, object expected);
}

/// <inheritdoc cref="TypePairComparison" />
/// <remarks>
///     The types are matched with a type check instead of reflection, so that a derived or implementing type matches
///     as well, also when publishing with trimming or Native AOT enabled.
/// </remarks>
internal sealed class TypePairComparison<TActual, TExpected>(
	Func<TActual, TExpected, bool> predicate,
	string description) : TypePairComparison(description)
{
	/// <inheritdoc />
	public override void AppendTypes(StringBuilder stringBuilder)
	{
		Formatter.Format(stringBuilder, typeof(TActual));
		stringBuilder.Append(" with ");
		Formatter.Format(stringBuilder, typeof(TExpected));
	}

	/// <inheritdoc />
	public override bool AppliesTo(object actual, object expected)
		=> actual is TActual && expected is TExpected;

	/// <inheritdoc />
	public override bool Compare(object actual, object expected)
		=> predicate((TActual)actual, (TExpected)expected);
}
