using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches at least <paramref name="minimum" /> items.
	/// </summary>
	public static EnumerableQuantifier AtLeast(int minimum,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new AtLeastQuantifier(minimum);

	private sealed class AtLeastQuantifier(int minimum) : EnumerableQuantifier
	{
		public override string ToString()
			=> minimum switch
			{
				1 => "at least one",
				_ => $"at least {minimum}",
			};

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount >= minimum;

		/// <inheritdoc />
		public override bool IsSingle() => minimum == 1;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount >= minimum)
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
			=> QuantifierContexts.None;

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			if (grammars.IsNegated())
			{
				if (totalCount.HasValue)
				{
					stringBuilder.Append("found ").Append(matchingCount);
				}
				else
				{
					stringBuilder.Append("found at least ").Append(matchingCount);
				}

				return;
			}

			if (!totalCount.HasValue)
			{
				return;
			}

			if (verb != null)
			{
				stringBuilder.Append("only ").Append(matchingCount).Append(" of ").Append(totalCount.Value)
					.Append(' ').Append(verb);
			}
			else
			{
				stringBuilder.Append("found only ").Append(matchingCount);
			}
		}
	}
}
