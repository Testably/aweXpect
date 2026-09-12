using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TInnerException" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> source,
		Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception?
		=> new(source.Get().ExpectationBuilder
				.ForMember<Exception?, Exception?>(e => e?.InnerException,
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<TInnerException?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner<TInnerException>(
		this IThat<Exception?> source)
		where TInnerException : Exception?
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars)),
			source);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <paramref name="innerExceptionType" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> source,
		Type innerExceptionType,
		Action<IThatSubject<Exception?>> expectations)
		=> new(source.Get().ExpectationBuilder
				.ForMember<Exception?, Exception?>(e => e?.InnerException,
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <paramref name="innerExceptionType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInner(
		this IThat<Exception?> source,
		Type innerExceptionType)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars)),
			source);
}
