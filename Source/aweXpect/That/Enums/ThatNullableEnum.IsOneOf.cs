using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsOneOf<TEnum>(this IThat<TEnum?> subject,
		params TEnum?[] expected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsOneOf<TEnum>(this IThat<TEnum?> subject,
		IEnumerable<TEnum?> expected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsOneOf<TEnum>(this IThat<TEnum?> subject,
		IEnumerable<TEnum> expected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, expected.Cast<TEnum?>())),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsNotOneOf<TEnum>(this IThat<TEnum?> subject,
		params TEnum?[] unexpected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, unexpected).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsNotOneOf<TEnum>(this IThat<TEnum?> subject,
		IEnumerable<TEnum?> unexpected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, unexpected).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<TEnum?, IThat<TEnum?>> IsNotOneOf<TEnum>(this IThat<TEnum?> subject,
		IEnumerable<TEnum> unexpected)
		where TEnum : struct, Enum
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint<TEnum>(it, grammars, unexpected.Cast<TEnum?>()).Invert()),
			subject);

	private sealed class IsOneOfConstraint<TEnum>(string it, ExpectationGrammars grammars, IEnumerable<TEnum?> expected)
		: ConstraintResult.WithValue<TEnum?>(grammars),
			IValueConstraint<TEnum?>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum? actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (TEnum? value in expected)
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
