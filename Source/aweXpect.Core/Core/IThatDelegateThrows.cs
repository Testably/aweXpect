using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Results;

namespace aweXpect.Core;

/// <summary>
///     An expectation on a delegate that is expected to throw an exception of type <typeparamref name="TException" />.
/// </summary>
/// <remarks>
///     It is the continuation after <c>.And</c> and <c>.Or</c> of a thrown exception, which offers further expectations
///     on the exception, but not <c>Within</c> or <c>OnlyIf</c>, which configure the whole <c>Throws…</c> expectation.
/// </remarks>
public interface IThatDelegateThrows<TException> : IExpectThat<TException>
	where TException : Exception?
{
	/// <summary>
	///     Further expectations on the <typeparamref name="TException" />.
	/// </summary>
	IThat<TException> Which { get; }

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
	///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
	///     evaluation is canceled aborts the evaluation instead.
	/// </remarks>
	AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, TMember?> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "");

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a
	///     timeout fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(2)]
	AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, Task<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "");

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a
	///     timeout fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(1)]
	AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, ValueTask<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "");

	/// <summary>
	///     Verifies that the thrown exception has an inner exception which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Action<IThatSubject<Exception?>> expectations);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner();

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <typeparamref name="TInnerException" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner<TInnerException>(
		Action<IThatSubject<TInnerException?>> expectations)
		where TInnerException : Exception;

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner<TInnerException>()
		where TInnerException : Exception?;

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <paramref name="type" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Type type,
		Action<IThatSubject<Exception?>> expectations);

	/// <summary>
	///     Verifies that the thrown exception has an inner exception of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithInner(
		Type type);

	/// <summary>
	///     Verifies that the thrown exception has no inner exception.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner();

	/// <summary>
	///     Verifies that the thrown exception has no inner exception of type <typeparamref name="TInnerException" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner<TInnerException>()
		where TInnerException : Exception?;

	/// <summary>
	///     Verifies that the thrown exception has no inner exception of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TException, IThatDelegateThrows<TException>> WithoutInner(
		Type type);
}
