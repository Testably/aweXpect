using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string?, IThat<string?>> IsEmpty(
		this IThat<string?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEmptyConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsNotEmpty(
		this IThat<string?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEmptyConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsEmptyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual == string.Empty ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is empty");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not empty");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
