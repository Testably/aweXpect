using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception recursively has inner exceptions which satisfy the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     Recursively applies the expectations on the <see cref="Exception.InnerException" /> (if not <see langword="null" />
	///     and for <see cref="AggregateException" /> also on the <see cref="AggregateException.InnerExceptions" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasRecursiveInnerExceptions(
		this IThat<Exception?> source,
		Action<IThatSubject<IEnumerable<Exception>>> expectations)
		=> new(source.Get().ExpectationBuilder
				.ForMember<Exception?, IEnumerable<Exception?>>(
					e => e.GetInnerExceptions(),
					" which ",
					false)
				.Validate((it, grammars) => new HasRecursiveInnerExceptionsConstraint(it, grammars))
				.AddExpectations(e => expectations(
						new ThatSubject<IEnumerable<Exception>>(e)),
					grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural),
			source);

	internal class HasRecursiveInnerExceptionsConstraint(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Exception>(it, grammars),
			IValueConstraint<Exception?>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with recursive inner exceptions");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("recursive inner exceptions are");
			}
			else
			{
				stringBuilder.Append("has recursive inner exceptions");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			stringBuilder.Append(Actual!.FormatForMessage());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("without recursive inner exceptions");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("recursive inner exceptions are not");
			}
			else
			{
				stringBuilder.Append("does not have recursive inner exceptions");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
