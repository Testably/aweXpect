using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception has an inner exception which satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Action<IThatSubject<Exception?>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" which ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TInnerException" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> subject,
		Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception?
		=> new(subject.Get().ExpectationBuilder
				.ForMember<Exception?, Exception?>(e => e?.InnerException,
					" which ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars))
				.AddExpectations<TInnerException?>(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> subject)
		where TInnerException : Exception?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <paramref name="type" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Type type,
		Action<IThatSubject<Exception?>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForMember<Exception?, Exception?>(e => e?.InnerException,
					" which ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(type, it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> subject,
		Type type)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasInnerExceptionValueConstraint(type, it, grammars)),
			subject);
}
