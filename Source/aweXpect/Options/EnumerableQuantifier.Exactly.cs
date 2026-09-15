using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches exactly <paramref name="expected" /> items.
	/// </summary>
	public static EnumerableQuantifier Exactly(int expected,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new ExactlyQuantifier(expected);

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
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			if (grammars.HasFlag(ExpectationGrammars.Negated))
			{
				stringBuilder.Append("it did");
			}
			else if (matchingCount > expected)
			{
				if (totalCount.HasValue)
				{
					if (verb != null)
					{
						stringBuilder.Append(matchingCount).Append(" of ").Append(totalCount.Value)
							.Append(' ').Append(verb);
					}
					else
					{
						stringBuilder.Append("found ").Append(matchingCount);
					}
				}
				else
				{
					if (verb != null)
					{
						stringBuilder.Append("at least ").Append(matchingCount)
							.Append(' ').Append(verb);
					}
					else
					{
						stringBuilder.Append("found at least ").Append(matchingCount);
					}
				}
			}
			else if (totalCount.HasValue)
			{
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
}
