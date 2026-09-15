using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatChar
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<char, IThat<char>> IsEqualTo(this IThat<char> subject,
		char? expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<char, IThat<char>> IsNotEqualTo(this IThat<char> subject,
		char? unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, unexpected).Invert()),
			subject);

	private sealed class IsEqualToConstraint(string it, ExpectationGrammars grammars, char? expected)
		: ConstraintResult.WithEqualToValue<char>(it, grammars, expected is null),
			IValueConstraint<char>
	{
		public ConstraintResult IsMetBy(char actual)
		{
			Actual = actual;
			Outcome = actual.Equals(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is equal to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not equal to ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
