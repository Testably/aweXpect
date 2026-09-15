using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableChar
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsOneOf(this IThat<char?> subject,
		params char?[] expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsOneOf(this IThat<char?> subject,
		IEnumerable<char?> expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsOneOf(this IThat<char?> subject,
		IEnumerable<char> expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected.Cast<char?>())),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsNotOneOf(this IThat<char?> subject,
		params char?[] unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsNotOneOf(this IThat<char?> subject,
		IEnumerable<char?> unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<char?, IThat<char?>> IsNotOneOf(this IThat<char?> subject,
		IEnumerable<char> unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected.Cast<char?>()).Invert()),
			subject);

	private sealed class IsOneOfConstraint(string it, ExpectationGrammars grammars, IEnumerable<char?> expected)
		: ConstraintResult.WithValue<char?>(grammars),
			IValueConstraint<char?>
	{
		public ConstraintResult IsMetBy(char? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (char? value in expected)
			{
				hasValues = true;
				if (actual.Equals(value))
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (!hasValues)
			{
				throw ThrowHelper.EmptyCollection();
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
