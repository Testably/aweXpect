using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches at least <paramref name="minimum" /> items.
	/// </summary>
	public static EnumerableQuantifier AtLeast(int minimum)
		=> new BoundedQuantifier(CountBounds.AtLeast(minimum));

	/// <summary>
	///     Matches at most <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier AtMost(int maximum)
		=> new BoundedQuantifier(CountBounds.AtMost(maximum));

	/// <summary>
	///     Matches between <paramref name="minimum" /> and <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier Between(int minimum, int maximum)
		=> new BoundedQuantifier(CountBounds.Between(minimum, maximum));

	/// <summary>
	///     Matches exactly <paramref name="expected" /> items.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> count matches no collection, because a count is never <see langword="null" />.
	/// </remarks>
	public static EnumerableQuantifier Exactly(int? expected)
		=> expected is null
			? new NullCountQuantifier("exactly <null>", false)
			: new BoundedQuantifier(CountBounds.Exactly(expected.Value));

	/// <summary>
	///     Matches fewer than <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier LessThan(int maximum)
		=> new BoundedQuantifier(CountBounds.LessThan(maximum));

	/// <summary>
	///     Matches more than <paramref name="minimum" /> items.
	/// </summary>
	public static EnumerableQuantifier MoreThan(int minimum)
		=> new BoundedQuantifier(CountBounds.MoreThan(minimum));

	private sealed class BoundedQuantifier(CountBounds bounds) : EnumerableQuantifier
	{
		public override string ToString()
		{
			StringBuilder stringBuilder = new();
			bounds.AppendItems(stringBuilder);
			return stringBuilder.ToString();
		}

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> bounds.Check(matchingCount, false) is not null;

		/// <inheritdoc />
		public override bool IsSingle() => bounds.IsSingleItem();

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
			=> bounds.Check(matchingCount, totalCount.HasValue) switch
			{
				true => Outcome.Success,
				false => Outcome.Failure,
				null when totalCount.HasValue => Outcome.Failure,
				null => Outcome.Undecided,
			};

		/// <inheritdoc />
		public override QuantifierContexts GetQuantifierContext()
			=> (bounds.Minimum, bounds.Maximum, bounds.AllowEqual) switch
			{
				(null, _, _) => QuantifierContexts.MatchingItems,
				(_, null, false) => QuantifierContexts.NotMatchingItems,
				_ => QuantifierContexts.None,
			};

		/// <inheritdoc />
		private protected override EnumerableQuantifier? GetComplement(ExpectationGrammars grammars)
			=> bounds.Complement() switch
			{
				{ IsNever: true, } => None(grammars),
				{ } complement => new BoundedQuantifier(complement),
				null => null,
			};

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb,
				!grammars.IsNegated() && bounds.IsBelowMinimum(matchingCount));
	}
}
