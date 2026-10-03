using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatChar
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static CharEqualityResult<char, IThat<char>> IsEqualTo(this IThat<char> subject,
		char? expected)
	{
		CharEqualityOptions options = new();
		return new CharEqualityResult<char, IThat<char>>(subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Options: options),
				static (state, it, grammars) => new IsEqualToConstraint(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static CharEqualityResult<char, IThat<char>> IsNotEqualTo(this IThat<char> subject,
		char? unexpected)
	{
		CharEqualityOptions options = new();
		return new CharEqualityResult<char, IThat<char>>(subject.Get().ExpectationBuilder.AddConstraint(
				(Unexpected: unexpected, Options: options),
				static (state, it, grammars)
					=> new IsEqualToConstraint(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		char? expected,
		CharEqualityOptions options)
		: ConstraintResult.WithEqualToValue<char>(it, grammars, expected is null),
			IValueConstraint<char>
	{
		public ConstraintResult IsMetBy(char actual)
		{
			Actual = actual;
			Outcome = options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not equal to ", "are not equal to "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
