using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsOneOf(
		this IThat<DateTime?> subject,
		params DateTime?[] expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new IsOneOfConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsOneOf(
		this IThat<DateTime?> subject,
		IEnumerable<DateTime> expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new IsOneOfConstraint(it, grammars, expected.Cast<DateTime?>(), tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is one of the <paramref name="expected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsOneOf(
		this IThat<DateTime?> subject,
		IEnumerable<DateTime?> expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new IsOneOfConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsNotOneOf(
		this IThat<DateTime?> subject,
		params DateTime?[] unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsNotOneOf(
		this IThat<DateTime?> subject,
		IEnumerable<DateTime> unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected.Cast<DateTime?>(), tolerance).Invert()),
			subject,
			tolerance);
	}

	/// <summary>
	///     Verifies that the subject is not one of the <paramref name="unexpected" /> values.
	/// </summary>
	public static TimeToleranceResult<DateTime?, IThat<DateTime?>> IsNotOneOf(
		this IThat<DateTime?> subject,
		IEnumerable<DateTime?> unexpected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<DateTime?, IThat<DateTime?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOneOfConstraint(it, grammars, unexpected, tolerance).Invert()),
			subject,
			tolerance);
	}

	private sealed class IsOneOfConstraint(
		string it,
		ExpectationGrammars grammars,
		IEnumerable<DateTime?> expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateTime?>(it, grammars),
			IValueConstraint<DateTime?>
	{
		private DateTimeKind? _incompatibleKind;

		public ConstraintResult IsMetBy(DateTime? actual)
		{
			ThrowHelper.ThrowIfEmpty(expected);
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected.Any(x => x is null) ? Outcome.Success : Outcome.Failure;
			}
			else
			{
				Outcome = GetOutcomeFor(actual.Value);
			}

			return this;
		}

		private Outcome GetOutcomeFor(DateTime actual)
		{
			TimeSpan timeTolerance = tolerance.Tolerance ??
			                         Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get();
			bool hasComparableValue = false;
			DateTimeKind? incomparableKind = null;
			foreach (DateTime? value in expected)
			{
				if (value is null)
				{
					continue;
				}

				if (!actual.IsKindCompatibleWith(value.Value))
				{
					incomparableKind = value.Value.Kind;
					continue;
				}

				hasComparableValue = true;
				if (actual - value.Value <= timeTolerance &&
				    actual - value.Value >= timeTolerance.Negate())
				{
					return Outcome.Success;
				}
			}

			_incompatibleKind = hasComparableValue ? null : incomparableKind;
			return Outcome.Failure;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_incompatibleKind is not null)
			{
				stringBuilder.Append(It).Append(" had Kind ").Append(Actual?.Kind)
					.Append(", which cannot be compared with ").Append(_incompatibleKind);
			}
			else
			{
				stringBuilder.Append(It).Append(" was ");
				Formatter.Format(stringBuilder, Actual);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not one of ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
