using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
public static partial class ThatException
{
	/// <summary>
	///     Verifies that the subject has an inner exception which satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Action<IThatSubject<Exception?>> expectations)
	{
		expectations.ThrowIfNull();
		return new AndOrResult<Exception, IThat<Exception?>>(subject.Get().ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);
	}

	/// <summary>
	///     Verifies that the subject has an inner exception.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject has an inner exception of type <typeparamref name="TInnerException" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> subject,
		Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception?
	{
		expectations.ThrowIfNull();
		return new AndOrResult<Exception, IThat<Exception?>>(subject.Get().ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars, true))
				.AddExpectations<TInnerException?>(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);
	}

	/// <summary>
	///     Verifies that the subject has an inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> subject)
		where TInnerException : Exception?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the subject has an inner exception of type <paramref name="type" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Type type,
		Action<IThatSubject<Exception?>> expectations)
	{
		type.ThrowIfNotAnExceptionType();
		expectations.ThrowIfNull();
		return new AndOrResult<Exception, IThat<Exception?>>(subject.Get().ExpectationBuilder
				// An inner exception of another type is hidden like a missing one, as the type mismatch already fails.
				.ForMember<Exception, Exception?>(
					e => e.InnerException is { } inner && type.IsOrImplements(inner) ? inner : null,
					" that ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(type, it, grammars, true))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);
	}

	/// <summary>
	///     Verifies that the subject has an inner exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Type type)
	{
		type.ThrowIfNotAnExceptionType();
		return new AndOrResult<Exception, IThat<Exception?>>(subject.Get().ExpectationBuilder.AddConstraint(type, static (innerExceptionType, it, grammars)
				=> new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars)),
			subject);
	}
}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
