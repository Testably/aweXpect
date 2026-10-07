using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Action<IThatSubject<Exception?>> expectations)
	{
		expectations.ThrowIfNull();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder
				.ForMember(
					MemberAccessor<Exception, Exception?>.FromFunc(
						e => e.InnerException,
						"the inner exception"),
					(_, s) => s.Append(" that "),
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);
	}

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithInner()
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			this);

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>>
		WithInner<TInnerException>(
			Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception
	{
		expectations.ThrowIfNull();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars, true))
				.AddExpectations<TInnerException?>(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);
	}

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithInner<
		TInnerException>()
		where TInnerException : Exception?
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			this);

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Type type,
		Action<IThatSubject<Exception?>> expectations)
	{
		type.ThrowIfNotAnExceptionType();
		expectations.ThrowIfNull();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder
				// An inner exception of another type is hidden like a missing one, as the type mismatch already fails.
				.ForMember<Exception, Exception?>(
					e => e.InnerException.IsOfType(type) ? e.InnerException : null,
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(type, it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);
	}

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Type type)
	{
		type.ThrowIfNotAnExceptionType();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder
				.AddConstraint(type, static (innerExceptionType, it, grammars) =>
					new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars)),
			this);
	}

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner()
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars).Invert()),
			this);

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner<TInnerException>()
		where TInnerException : Exception?
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars).Invert()),
			this);

	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner(
		Type type)
	{
		type.ThrowIfNotAnExceptionType();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder
				.AddConstraint(type, static (innerExceptionType, it, grammars) =>
					new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars).Invert()),
			this);
	}

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
			Outcome = (actual?.InnerException).IsOfType(innerExceptionType)
				? Outcome.Success
				: Outcome.Failure;
			// Expectations on a missing inner exception or one of another type could only repeat the mismatch.
			FurtherProcessingStrategy = hasMemberExpectations && Outcome != Outcome.Success
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
			=> stringBuilder.Append(It).Append(" had ")
				.Append(ThatDelegate.FormatForMessage(Actual!.InnerException!, indentation, "inner "));
	}
}
