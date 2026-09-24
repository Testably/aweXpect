using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the thrown exception has an inner exception which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner(
		Action<IThatSubject<Exception?>> expectations)
		=> new(ExpectationBuilder
				.ForMember(
					MemberAccessor<Exception?, Exception?>.FromFunc(
						e => e?.InnerException,
						"the inner exception"),
					(_, s) => s.Append(" which "),
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner()
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <typeparamref name="TInnerException" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>>
		WithInner<TInnerException>(
			Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception
		=> new(ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" which ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars, true))
				.AddExpectations<TInnerException?>(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner<
		TInnerException>()
		where TInnerException : Exception?
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <paramref name="type" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner(
		Type type,
		Action<IThatSubject<Exception?>> expectations)
		=> new(ExpectationBuilder
				// An inner exception of another type is hidden like a missing one, as the type mismatch already fails.
				.ForMember<Exception, Exception?>(
					e => type.IsInstanceOfType(e.InnerException) ? e.InnerException : null,
					" which ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(type, it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <paramref name="type" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner(
		Type type)
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(type, it, grammars)),
			this);

	private sealed class HasInnerExceptionValueConstraint(
		Type innerExceptionType,
		string it,
		ExpectationGrammars grammars,
		bool hasMemberExpectations = false)
		: ConstraintResult.WithNotNullValue<Exception>(it, grammars),
			IValueConstraint<Exception?>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			Outcome = innerExceptionType.IsAssignableFrom(actual?.InnerException?.GetType())
				? Outcome.Success
				: Outcome.Failure;
			// Expectations on a missing inner exception or one of another type could only repeat the mismatch.
			FurtherProcessingStrategy = hasMemberExpectations && Outcome == Outcome.Failure
				? FurtherProcessingStrategy.IgnoreResult
				: FurtherProcessingStrategy.Continue;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("with an inner ");
			if (innerExceptionType == typeof(Exception))
			{
				stringBuilder.Append("exception");
			}
			else
			{
				Formatter.Format(stringBuilder, innerExceptionType);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual!.InnerException is null)
			{
				stringBuilder.Append(It).Append(" had no inner exception");
			}
			else
			{
				stringBuilder.Append(It).Append(" had ");
				stringBuilder.Append(ThatDelegate.FormatForMessage(Actual.InnerException, indentation, "inner "));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("without an inner ");
			if (innerExceptionType == typeof(Exception))
			{
				stringBuilder.Append("exception");
			}
			else
			{
				Formatter.Format(stringBuilder, innerExceptionType);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
