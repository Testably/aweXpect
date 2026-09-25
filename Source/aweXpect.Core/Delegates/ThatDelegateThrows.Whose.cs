using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
	///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
	///     evaluation is cancelled aborts the evaluation instead.
	/// </remarks>
	public AndOrResult<TException, ThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, TMember?> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> new(ExpectationBuilder.ForMember(
					MemberAccessor<TException, TMember?>.FromFuncAsMemberAccessor(memberSelector,
						doNotPopulateThisValue),
					(member, expectation) => expectation.Append("whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars | ExpectationGrammars.Introduced),
			this);

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" /> and a timeout fails it with
	///     <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(2)]
	public AndOrResult<TException, ThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, Task<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> new(ExpectationBuilder.ForAsyncMember(
					MemberAccessor<TException, Task<TMember>>.FromFuncAsMemberAccessor(memberSelector,
						doNotPopulateThisValue),
					(member, expectation) => expectation.Append("whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars | ExpectationGrammars.Introduced),
			this);

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" /> and a timeout fails it with
	///     <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(1)]
	public AndOrResult<TException, ThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, ValueTask<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> Whose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
}
