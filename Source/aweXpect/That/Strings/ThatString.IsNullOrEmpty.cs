using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is <see langword="null" /> or <see cref="string.Empty" />.
	/// </summary>
	public static AndOrResult<string?, IThat<string?>> IsNullOrEmpty(
		this IThat<string?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNullOrEmptyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the subject is not <see langword="null" /> or <see cref="string.Empty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsNotNullOrEmpty(
		this IThat<string?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsNullOrEmptyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsNullOrEmptyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<string?>(grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = string.IsNullOrEmpty(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is null or empty");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not null or empty");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
