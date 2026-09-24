using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableEnum
{
	/// <summary>
	///     Verifies that the subject has the <paramref name="expected" /> flag set.
	/// </summary>
	/// <remarks>
	///     Unlike the other <c>Has…</c> expectations this one has no continuation: testing a flag asks whether a bit is
	///     set, so the comparisons a continuation offers (greater than, between, …) have no meaning for it.<br />
	///     A <see langword="null" /> <paramref name="expected" /> flag throws an <see cref="ArgumentNullException" />.
	///     A zero flag is always set (as in <see cref="Enum.HasFlag(Enum)" />), so it matches every subject that is not
	///     <see langword="null" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TEnum?, IThat<TEnum?>> HasFlag<TEnum>(
		this IThat<TEnum?> subject,
		TEnum? expected)
		where TEnum : struct, Enum
	{
		expected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasFlagConstraint<TEnum>(it, grammars, expected!.Value)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject does not have the <paramref name="unexpected" /> flag set.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> <paramref name="unexpected" /> flag throws an <see cref="ArgumentNullException" />.
	///     A zero flag is always set (as in <see cref="Enum.HasFlag(Enum)" />), so it fails for every subject.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TEnum?, IThat<TEnum?>> DoesNotHaveFlag<TEnum>(
		this IThat<TEnum?> subject,
		TEnum? unexpected)
		where TEnum : struct, Enum
	{
		unexpected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasFlagConstraint<TEnum>(it, grammars, unexpected!.Value).Invert()),
			subject);
	}

	private sealed class HasFlagConstraint<TEnum>(string it, ExpectationGrammars grammars, TEnum expectedFlag)
		: ConstraintResult.WithNotNullValue<TEnum?>(it, grammars),
			IValueConstraint<TEnum?>
		where TEnum : struct, Enum
	{
		public ConstraintResult IsMetBy(TEnum? actual)
		{
			Actual = actual;
			Outcome = actual?.HasFlag(expectedFlag) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has flag ", "have flag "));
			Formatter.Format(stringBuilder, expectedFlag);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have flag ", "do not have flag "));
			Formatter.Format(stringBuilder, expectedFlag);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
