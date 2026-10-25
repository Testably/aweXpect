using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
public static partial class ThatException
{
	/// <summary>
	///     Verifies that the subject has no inner exception.
	/// </summary>
	/// <remarks>
	///     Continues on <see cref="Exception" /> instead of the exception type of the subject, as a generic overload
	///     would shadow <see cref="DoesNotHaveInner{TInnerException}(IThat{Exception})" /> for a type argument the
	///     subject is assignable to.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> DoesNotHaveInner(
		this IThat<Exception?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject has no inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	/// <remarks>
	///     Continues on <see cref="Exception" /> instead of the exception type of the subject, as the explicit
	///     <typeparamref name="TInnerException" /> leaves no room to infer the subject type.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Exception, IThat<Exception?>> DoesNotHaveInner<TInnerException>(
		this IThat<Exception?> subject)
		where TInnerException : Exception?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(TInnerException), it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the subject has no inner exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException?>> DoesNotHaveInner<TException>(
		this IThat<TException?> subject,
		Type type)
		where TException : Exception
	{
		type.ThrowIfNotAnExceptionType();
		return new AndOrResult<TException, IThat<TException?>>(subject.Get().ExpectationBuilder.AddConstraint(type, static (innerExceptionType, it, grammars)
				=> new HasInnerExceptionValueConstraint(innerExceptionType, it, grammars).Invert()),
			subject);
	}
}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
