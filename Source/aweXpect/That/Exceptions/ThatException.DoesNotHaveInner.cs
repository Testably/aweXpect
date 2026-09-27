using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception has no inner exception.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> DoesNotHaveInner(
		this IThat<Exception?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the actual exception has no inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> DoesNotHaveInner<TInnerException>(
		this IThat<Exception?> subject)
		where TInnerException : Exception?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the actual exception has no inner exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> DoesNotHaveInner(
		this IThat<Exception?> subject,
		Type type)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasInnerExceptionValueConstraint(type, it, grammars).Invert()),
			subject);
}
