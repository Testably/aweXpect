using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches more than <paramref name="minimum" /> items.
	/// </summary>
	public static EnumerableQuantifier MoreThan(int minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		return new MoreThanQuantifier(minimum);
	}

	private sealed class MoreThanQuantifier(int minimum) : EnumerableQuantifier
	{
		public override string ToString()
			=> minimum switch
			{
				1 => "more than one",
				_ => $"more than {minimum}",
			};

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount > minimum;

		/// <inheritdoc />
		public override bool IsSingle() => minimum == 1;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount > minimum)
			{
				return Outcome.Success;
			}

			if (totalCount.HasValue)
			{
				return Outcome.Failure;
			}

			return Outcome.Undecided;
		}

		/// <inheritdoc />
		public override QuantifierContexts GetQuantifierContext()
			=> QuantifierContexts.NotMatchingItems;

		/// <inheritdoc />
		private protected override EnumerableQuantifier GetComplement(ExpectationGrammars grammars)
			=> minimum switch
			{
				0 => None(grammars),
				_ => AtMost(minimum),
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
				!grammars.IsNegated());
	}
}
