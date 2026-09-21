using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGuid
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsOneOf(this IThat<Guid> subject,
		params Guid?[] expected)
	{
		expected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsOneOf(this IThat<Guid> subject,
		IEnumerable<Guid?> expected)
	{
		expected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsOneOf(this IThat<Guid> subject,
		IEnumerable<Guid> expected)
	{
		expected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, expected.Cast<Guid?>())),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsNotOneOf(this IThat<Guid> subject,
		params Guid?[] unexpected)
	{
		unexpected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected).Invert()),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsNotOneOf(this IThat<Guid> subject,
		IEnumerable<Guid?> unexpected)
	{
		unexpected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected).Invert()),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static AndOrResult<Guid, IThat<Guid>> IsNotOneOf(this IThat<Guid> subject,
		IEnumerable<Guid> unexpected)
	{
		unexpected.ThrowIfNull();
		return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected.Cast<Guid?>()).Invert()),
			subject);
	}

	private sealed class IsOneOfConstraint(string it, ExpectationGrammars grammars, IEnumerable<Guid?> expected)
		: ConstraintResult.WithNotNullValue<Guid>(it, grammars),
			IValueConstraint<Guid>
	{
		public ConstraintResult IsMetBy(Guid actual)
		{
			Actual = actual;
			bool hasValues = false;
			foreach (Guid? value in expected)
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
				throw Tracing.WriteException(ThrowHelper.EmptyCollection());
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
			stringBuilder.Append(It).Append(" was ");
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
