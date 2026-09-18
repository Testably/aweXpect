using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberSelector" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, TMember?> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(
				MemberAccessor<T, TMember?>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
				(member, stringBuilder) => stringBuilder.Append("whose ").Append(member))
			.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)));
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(2)]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, Task<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForAsyncMember(
				MemberAccessor<T, Task<TMember>>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
				(member, stringBuilder) => stringBuilder.Append("whose ").Append(member))
			.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)));
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder, subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(1)]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, ValueTask<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> subject.Whose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
#endif
}
