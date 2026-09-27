using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses the equality of <see cref="Version" />, where an unset component is not treated as 0, so
	///     <c>1.2</c> is not equal to <c>1.2.0</c>.
	/// </remarks>
	public static AndOrResult<Version?, IThat<Version?>> IsEqualTo(
		this IThat<Version?> subject,
		Version? expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses the equality of <see cref="Version" />, where an unset component is not treated as 0, so
	///     <c>1.2</c> is not equal to <c>1.2.0</c>.
	/// </remarks>
	public static AndOrResult<Version?, IThat<Version?>> IsNotEqualTo(
		this IThat<Version?> subject,
		Version? unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint(it, grammars, unexpected).Invert()),
			subject);

	private sealed class IsEqualToConstraint(string it, ExpectationGrammars grammars, Version? expected)
		: ConstraintResult.WithEqualToValue<Version?>(it, grammars, expected is null),
			IValueConstraint<Version?>
	{
		public ConstraintResult IsMetBy(Version? actual)
		{
			Actual = actual;
			Outcome = Equals(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is equal to ", "are equal to "));
			Formatter.Format(stringBuilder, expected);
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
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
