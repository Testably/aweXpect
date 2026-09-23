using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches exactly <paramref name="expected" /> items.
	/// </summary>
	public static EnumerableQuantifier Exactly(int expected)
	{
		ThrowHelper.ThrowIfCountIsNegative(expected);
		return new ExactlyQuantifier(expected);
	}

	private sealed class ExactlyQuantifier(int expected) : EnumerableQuantifier
	{
		public override string ToString()
			=> expected switch
			{
				1 => "exactly one",
				_ => $"exactly {expected}",
			};

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount > expected;

		/// <inheritdoc />
		public override bool IsSingle() => expected == 1;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount > expected)
			{
				return Outcome.Failure;
			}

			if (totalCount.HasValue)
			{
				return matchingCount == expected
					? Outcome.Success
					: Outcome.Failure;
			}

			return Outcome.Undecided;
		}

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb,
				matchingCount < expected && !grammars.IsNegated());
	}
}
