using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
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
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it,
						grammars | ExpectationGrammars.Nested))
				.AddExpectations<TInnerException?>(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TException" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner<
		TInnerException>()
		where TInnerException : Exception?
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			this);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <paramref name="innerExceptionType" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner(
		Type innerExceptionType,
		Action<IThatSubject<Exception?>> expectations)
		=> new(ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <paramref name="innerExceptionType" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInner(
		Type innerExceptionType)
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(innerExceptionType, it,
						grammars | ExpectationGrammars.Nested)),
			this);

	private sealed class HasInnerExceptionValueConstraint(
		Type innerExceptionType,
		string it,
		ExpectationGrammars grammars)
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
			if (Actual?.InnerException is null)
			{
				stringBuilder.ItWasNull(It);
			}
			else
			{
				stringBuilder.Append(It).Append(" was ");
				stringBuilder.Append(ThatDelegate.FormatForMessage(Actual.InnerException));
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
