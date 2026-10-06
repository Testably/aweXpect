using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <inheritdoc />
	public AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, TMember?> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		expectations.ThrowIfNull();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder.ForMember(
					MemberAccessor<TException, TMember?>.FromFuncAsMemberAccessor(memberAccessor,
						doNotPopulateThisValue),
					(member, expectation) => expectation.Append("whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars.ForMember<TMember>() | ExpectationGrammars.Introduced),
			this);
	}

	/// <inheritdoc />
	[OverloadResolutionPriority(2)]
	public AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, Task<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		expectations.ThrowIfNull();
		return new AndOrResult<TException, IThatDelegateThrows<TException>>(ExpectationBuilder.ForAsyncMember(
					MemberAccessor<TException, Task<TMember>>.FromFuncAsMemberAccessor(memberAccessor,
						doNotPopulateThisValue),
					(member, expectation) => expectation.Append("whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars.ForMember<TMember>() | ExpectationGrammars.Introduced),
			this);
	}

	/// <inheritdoc />
	[OverloadResolutionPriority(1)]
	public AndOrResult<TException, IThatDelegateThrows<TException>> Whose<TMember>(
		Func<TException, ValueTask<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		return Whose(x => memberAccessor(x).AsTask(), expectations, doNotPopulateThisValue);
	}
}
