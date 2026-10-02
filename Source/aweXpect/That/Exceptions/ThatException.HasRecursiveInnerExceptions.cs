using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
public static partial class ThatException
{
	/// <summary>
	///     Verifies that the subject recursively has inner exceptions which satisfy the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     Recursively applies the expectations on the <see cref="Exception.InnerException" /> (if not <see langword="null" />)
	///     and for <see cref="AggregateException" /> also on the <see cref="AggregateException.InnerExceptions" />.
	///     <para />
	///     The exception must have at least one inner exception.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException?>> HasRecursiveInnerExceptions<TException>(
		this IThat<TException?> subject,
		Action<IThatSubject<IEnumerable<Exception>>> expectations)
		where TException : Exception
	{
		expectations.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new(expectationBuilder
				.ForMember<Exception?, IEnumerable<Exception?>>(
					e => e.GetInnerExceptions(),
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasRecursiveInnerExceptionsConstraint(expectationBuilder, it, grammars))
				.AddExpectations(e => expectations(
						new ThatSubject<IEnumerable<Exception>>(e)),
					grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural),
			subject);
	}

	internal class HasRecursiveInnerExceptionsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Exception>(it, grammars),
			IValueConstraint<Exception?>
	{
		private List<Exception>? _innerExceptionsForNegation;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			_innerExceptionsForNegation = null;
			List<Exception> innerExceptions = actual.GetInnerExceptions().ToList();
			if (innerExceptions.Count > 0)
			{
				_innerExceptionsForNegation = innerExceptions;
				Outcome = Outcome.Success;
				return this;
			}

			// Expectations on missing inner exceptions could only repeat that there is nothing to inspect.
			FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			Outcome = Outcome.Failure;
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
				stringBuilder.Append("whose recursive inner exceptions are");
			}
			else
			{
				stringBuilder.Append(Grammars.Verb("has recursive inner exceptions", "have recursive inner exceptions"));
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had no inner exceptions");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("without recursive inner exceptions");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("whose recursive inner exceptions are not");
			}
			else
			{
				stringBuilder.Append(Grammars.Verb("does not have recursive inner exceptions", "do not have recursive inner exceptions"));
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");

		/// <remarks>
		///     The negated result relies on the inner exceptions as context, which the expectations on them do not
		///     always add. Only the first negation after the evaluation (e.g. by <c>DoesNotComplyWith</c>) adds the
		///     context, as the next one reverts it before a repeated evaluation.
		/// </remarks>
		public override ConstraintResult Negate()
		{
			base.Negate();
			if (_innerExceptionsForNegation is not null)
			{
				expectationBuilder.AddCollectionContext(_innerExceptionsForNegation, onlyOnFailureOf: this);
				_innerExceptionsForNegation = null;
			}

			return this;
		}
	}
}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
