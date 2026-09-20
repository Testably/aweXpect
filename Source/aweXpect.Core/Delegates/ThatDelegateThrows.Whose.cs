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

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	[OverloadResolutionPriority(1)]
	public AndOrResult<TException, ThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, ValueTask<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> Whose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
#endif
}
