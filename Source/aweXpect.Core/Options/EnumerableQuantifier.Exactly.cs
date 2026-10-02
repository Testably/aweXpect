using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches exactly <paramref name="expected" /> items.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> count matches no collection, because a count is never <see langword="null" />.
	/// </remarks>
	public static EnumerableQuantifier Exactly(int? expected)
	{
		if (expected is null)
		{
			return new NullCountQuantifier("exactly <null>", false);
		}

		ThrowHelper.ThrowIfCountIsNegative(expected, "expected count");
		return new ExactlyQuantifier(expected.Value);
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
